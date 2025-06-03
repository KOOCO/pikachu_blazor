using Kooco.Pikachu.Domain.LogisticStatusRecords;
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
    private readonly IConfiguration _configuration;
    private readonly ILogger<TCatSFTPService> _logger;
    private SftpConfig Config { get; set; } = new();

    public TCatSFTPService(
        IRepository<LogisticStatusRecord, int> logisticStatusRecordRepository,
        IConfiguration configuration,
        ILogger<TCatSFTPService> logger)
    {
        _logisticStatusRecordRepository = logisticStatusRecordRepository;
        _configuration = configuration;
        _logger = logger;
        _configuration.GetSection("T-Cat").Bind(Config);
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
                    .Select(g => g.OrderByDescending(x => x.Order.ShipmentStatusDate).FirstOrDefault()!)
                    .Where(x => x != null)
                    .ToList();

                var orderIds = latestParsedLines.Select(x => x.Order.ConsignmentNoteNumber).Distinct().ToList();

                var records = (await _logisticStatusRecordRepository.GetListAsync(r =>
                    orderIds.Contains(r.OrderId)))
                    .GroupBy(r => r.OrderId)
                    .Select(g => g.OrderByDescending(r => r.DatetimeParsed).FirstOrDefault())
                    .Where(r => r != null)
                    .ToList();

                List<LogisticStatusRecord> toUpdate = [];

                foreach (var entry in latestParsedLines)
                {
                    var order = entry.Order;
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
                    }
                }

                if (toUpdate.Count > 0)
                {
                    _logger.LogInformation("Updating {UpdateCount} records", toUpdate.Count);
                    await _logisticStatusRecordRepository.UpdateManyAsync(toUpdate);
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
}
