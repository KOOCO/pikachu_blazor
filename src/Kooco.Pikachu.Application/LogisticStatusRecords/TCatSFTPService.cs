using Kooco.Pikachu.Domain.LogisticStatusRecords;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Orders.Repositories;
using Kooco.Pikachu.Orders.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.LogisticStatusRecords;

public class TCatSFTPService : ITransientDependency
{
    private readonly IRepository<LogisticStatusRecord, int> _logisticStatusRecordRepository;
    private readonly IOrderDeliveryRepository _orderDeliveryRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger<TCatSFTPService> _logger;
    public readonly OrderHistoryManager _orderHistoryManager;
    private SftpConfig Config { get; set; } = new();

    public TCatSFTPService(
        IRepository<LogisticStatusRecord, int> logisticStatusRecordRepository,
        IOrderDeliveryRepository orderDeliveryRepository,
        IOrderRepository orderRepository,
        ILogger<TCatSFTPService> logger,
        OrderHistoryManager orderHistoryManager,
        IConfiguration configuration
        )
    {
        _logisticStatusRecordRepository = logisticStatusRecordRepository;
        _orderDeliveryRepository = orderDeliveryRepository;
        _orderRepository = orderRepository;
        _logger = logger;
        _orderHistoryManager = orderHistoryManager;
        configuration.GetSection("T-Cat").Bind(Config);
    }

    public async Task ExecuteAsync()
    {
        _logger.LogInformation("Starting T-Cat SFTP service execution.");

        string remoteDirectory = "/Receive/";
        string fileExt = ".SOD";

        using var sftp = new SftpClient(Config.Host, Config.Port, Config.Username, Config.Password);
        sftp.Connect();

        if (sftp.IsConnected)
        {
            _logger.LogInformation("Connected to SFTP");

            var remoteFiles = sftp.ListDirectory(remoteDirectory)
                                  .Where(f => !f.Name.StartsWith('.') && f.Name.EndsWith(fileExt, StringComparison.OrdinalIgnoreCase));

            var latestFile = remoteFiles
                .OrderByDescending(file => file.LastWriteTimeUtc)
                .FirstOrDefault();

            if (latestFile == null)
            {
                _logger.LogInformation("File not found");
            }

            if (latestFile != null)
            {
                _logger.LogInformation("Processing File {FileName}", latestFile.FullName);

                using var stream = sftp.OpenRead(latestFile.FullName);
                using var reader = new StreamReader(stream);

                string content = await reader.ReadToEndAsync();
                if (string.IsNullOrWhiteSpace(content))
                {
                    sftp.Disconnect();
                    _logger.LogInformation("{FileName} is empty", latestFile.FullName);
                    _logger.LogInformation("Finished T-Cat SFTP service execution.");
                    return;
                }

                var lines = content.SplitToLines()
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    ?? [];

                var parsedLines = lines
                    .Select(line => new { Line = line, Order = ParseLineToOrder(line) })
                    .Where(x => x.Order != null)
                    .ToList();

                var latestParsedLines = parsedLines
                    .GroupBy(x => x.Order.ConsignmentNoteNumber)
                    .Select(g => g.OrderByDescending(x => x.Order.ShipmentStatusDate).FirstOrDefault())
                    .Where(x => x != null)
                    .ToList();

                var logisticIds = latestParsedLines.Select(x => x!.Order.ConsignmentNoteNumber).Distinct().ToList();

                var orderDeliveries = await _orderDeliveryRepository.GetListByLogisticsIdsAsync(logisticIds);

                var records = (await _logisticStatusRecordRepository.GetListAsync(r =>
                    logisticIds.Contains(r.OrderId)))
                    .GroupBy(r => r.OrderId)
                    .Select(g => g.OrderByDescending(r => r.DatetimeParsed).FirstOrDefault())
                    .Where(r => r != null)
                    .ToList();

                List<LogisticStatusRecord> toUpdate = [];
                List<Guid> orderIdsToUpdate = [];

                foreach (var entry in latestParsedLines)
                {
                    var order = entry!.Order;
                    var line = entry.Line;

                    var record = records.FirstOrDefault(r => r!.OrderId == order.ConsignmentNoteNumber);

                    if (record != null)
                    {
                        record.Location = order.EstablishmentName;
                        record.Datetime = order.ShipmentStatusDate;
                        record.DatetimeParsed = order.ShipmentStatusDateParsed;
                        record.Code = order.CustomerCode;
                        record.StatusCode = order.StatusId;
                        record.StatusMessage = order.StatusDescription;
                        record.RawLine = line;

                        toUpdate.Add(record);

                        var orderDelivery = orderDeliveries.FirstOrDefault(od => od.AllPayLogisticsID == record.OrderId);
                        if (orderDelivery != null)
                        {
                            var status = DeliveryStatusMapper.MapStatus(order.StatusId);
                            if (status != null)
                            {
                                orderDelivery.DeliveryStatus = status.Value;
                                orderIdsToUpdate.Add(orderDelivery.OrderId);
                            }
                            else
                            {
                                await _orderHistoryManager.AddOrderHistoryAsync(
                                    orderDelivery.OrderId,
                                    "T-CatAbnormalStatus",
                                    [orderDelivery.DeliveryNo ?? string.Empty, record.StatusCode, record.StatusMessage],
                                    null,
                                    null
                                );
                            }
                        }
                    }
                }

                if (toUpdate.Count == 0)
                {
                    _logger.LogInformation("No records to update.");
                }

                if (toUpdate.Count > 0)
                {
                    _logger.LogInformation("Updating {UpdateCount} records", toUpdate.Count);
                    await _logisticStatusRecordRepository.UpdateManyAsync(toUpdate);
                }

                orderIdsToUpdate = [.. orderIdsToUpdate.Distinct()];

                if (orderIdsToUpdate.Count > 0)
                {
                    var ordersToUpdate = await _orderRepository.GetListAsync(o => orderIdsToUpdate.Contains(o.Id));

                    foreach (var order in ordersToUpdate)
                    {
                        var thisOrderDeliveries = orderDeliveries
                            .Where(od => od.OrderId == order.Id)
                            .Select(od => od.DeliveryStatus)
                            .ToList();

                        if (thisOrderDeliveries.Count != 0)
                        {
                            var lowestOrderValue = thisOrderDeliveries
                                .Min(GetStatusRank);

                            var latestStatusAllPassed = DeliveryStatusProgression[lowestOrderValue];

                            var shippingStatus = latestStatusAllPassed switch
                            {
                                DeliveryStatus.Processing => ShippingStatus.PrepareShipment,
                                DeliveryStatus.ToBeShipped => ShippingStatus.ToBeShipped,
                                DeliveryStatus.Shipped => ShippingStatus.Shipped,
                                DeliveryStatus.Delivered => ShippingStatus.Delivered,
                                DeliveryStatus.PickedUp => ShippingStatus.PickedUp,
                                _ => order.ShippingStatus
                            };

                            if (shippingStatus != order.ShippingStatus)
                            {
                                await _orderHistoryManager.AddOrderHistoryAsync(
                                    order.Id,
                                    "ShippingStatusChanged",
                                    [order.ShippingStatus, shippingStatus],
                                    null,
                                    null
                                );

                                order.ShippingStatus = shippingStatus;
                            }
                        }
                    }
                }
            }

            sftp.Disconnect();
            _logger.LogInformation("Finished T-Cat SFTP service execution.");
        }
        else
        {
            _logger.LogInformation("Failed to connect to SFTP server.");
        }
    }

    private ShipmentOrder ParseLineToOrder(string line)
    {
        var parts = line.Split('|').Select(p => p.Trim()).ToArray();

        if (parts.Length < 8)
            Array.Resize(ref parts, 8);

        return new ShipmentOrder
        {
            ConsignmentNoteNumber = parts[0],
            ClientOrderNumber = parts[1],
            EstablishmentName = parts[2],
            ShipmentStatusDate = parts[3],
            ShipmentStatusDateParsed = ParseDateTime(parts[3]),
            CustomerCode = parts[4],
            StatusId = parts[5],
            StatusDescription = parts[6],
            SpecificationReplyCode = parts[7]
        };
    }

    private DateTime? ParseDateTime(string value)
    {
        if (DateTime.TryParseExact(value, "yyyyMMddHHmmss", null, System.Globalization.DateTimeStyles.None, out var result))
            return result;
        return null;
    }

    private static readonly List<DeliveryStatus> DeliveryStatusProgression =
    [
        DeliveryStatus.Processing,
        DeliveryStatus.ToBeShipped,
        DeliveryStatus.Shipped,
        DeliveryStatus.Delivered,
        DeliveryStatus.PickedUp
    ];

    // To get index-based rank:
    int GetStatusRank(DeliveryStatus status) =>
        DeliveryStatusProgression.IndexOf(status);


    private class SftpConfig
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    private class ShipmentOrder
    {
        public string ConsignmentNoteNumber { get; set; }
        public string ClientOrderNumber { get; set; }
        public string EstablishmentName { get; set; }
        public string? ShipmentStatusDate { get; set; } // format: yyyyMMddHHmmss
        public DateTime? ShipmentStatusDateParsed { get; set; }
        public string CustomerCode { get; set; }
        public string StatusId { get; set; }
        public string StatusDescription { get; set; }
        public string SpecificationReplyCode { get; set; }
    }

    private static class DeliveryStatusMapper
    {
        private static readonly Dictionary<string, DeliveryStatus> StatusIdMap = new()
        {
            // PrepareShipment
            ["00001"] = DeliveryStatus.Shipped,
            ["00006"] = DeliveryStatus.Shipped,
            ["00027"] = DeliveryStatus.Shipped,
            ["00019"] = DeliveryStatus.Shipped,
            ["00020"] = DeliveryStatus.Shipped,
            ["00013"] = DeliveryStatus.Shipped,
            ["00015"] = DeliveryStatus.Shipped,
            ["00219"] = DeliveryStatus.Shipped,
            ["168"] = DeliveryStatus.Shipped,
            ["202"] = DeliveryStatus.Shipped,
            ["204"] = DeliveryStatus.Shipped,
            ["205"] = DeliveryStatus.Shipped,
            ["00002"] = DeliveryStatus.Shipped,
            ["00008"] = DeliveryStatus.Shipped,
            ["00009"] = DeliveryStatus.Shipped,
            ["00010"] = DeliveryStatus.Shipped,
            ["00011"] = DeliveryStatus.Shipped,
            ["216"] = DeliveryStatus.Shipped,
            ["208"] = DeliveryStatus.Shipped,
            ["209"] = DeliveryStatus.Shipped,
            ["308"] = DeliveryStatus.Shipped,
            ["420"] = DeliveryStatus.Shipped,

            // Delivered
            ["00003"] = DeliveryStatus.Delivered,

            // Return
            ["00017"] = DeliveryStatus.Shipped,
            ["00016"] = DeliveryStatus.Shipped,
            ["00301"] = DeliveryStatus.Shipped,
            ["00399"] = DeliveryStatus.Shipped,
            ["309"] = DeliveryStatus.Shipped,

            //Closed(Abnormal)
            //["00023"] = DeliveryStatus.Closed,
            //["00024"] = DeliveryStatus.Closed,
            //["00025"] = DeliveryStatus.Closed,
            //["186"] = DeliveryStatus.Closed,
        };

        public static DeliveryStatus? MapStatus(string statusId)
        {
            return StatusIdMap.TryGetValue(statusId, out var status)
                ? status
                : null;
        }
    }
}
