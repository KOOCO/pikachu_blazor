using Hangfire;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.GroupBuys;
using MiniExcelLibs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Emailing;

namespace Kooco.Pikachu.AutomaticEmails
{
    [Queue("automatic-emails-job")]
    public class AutomaticEmailsJob : AsyncBackgroundJob<Guid>, ITransientDependency
    {
        private readonly IEmailSender _emailSender;
        private readonly IGroupBuyAppService _groupBuyAppService;
        private readonly IAutomaticEmailAppService _automaticEmailAppService;

        public AutomaticEmailsJob(
            IEmailSender emailSender,
            IGroupBuyAppService groupBuyAppService,
            IAutomaticEmailAppService automaticEmailAppService
            )
        {
            _emailSender = emailSender;
            _groupBuyAppService = groupBuyAppService;
            _automaticEmailAppService = automaticEmailAppService;
        }

        public override async Task ExecuteAsync(Guid id)
        {
            var args = await _automaticEmailAppService.GetWithDetailsByIdAsync(id);
            if(!DateTime.Now.IsBetween(args.StartDate, args.EndDate))
            {
                return;
            }
            try
            {
                var attachments = new List<Attachment>();
                if (args?.GroupBuys != null)
                {
                    await _automaticEmailAppService.UpdateJobStatusAsync(id, JobStatus.Running, args.TenantId);

                    foreach (var groupBuy in args.GroupBuys)
                    {
                        var data = await _groupBuyAppService.GetAttachmentAsync(groupBuy.GroupBuyId, args.TenantId, args.SendTime, args.RecurrenceType);
                        MemoryStream memoryStream = new();
                        await data.GetStream().CopyToAsync(memoryStream);
                        memoryStream.Seek(0, SeekOrigin.Begin);

                        var excelData = MiniExcel.Query(memoryStream, true);
                        if (excelData.Any())
                        {
                            memoryStream.Seek(0, SeekOrigin.Begin);
                            var attachment = new Attachment(memoryStream, data.FileName, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                            attachments.Add(attachment);
                        }
                    }

                    TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById("China Standard Time"); // UTC+8
                    DateTimeOffset sendTimeInChinaTime = TimeZoneInfo.ConvertTime(args.SendTime, tz);
                    string formattedTime = sendTimeInChinaTime.ToString("hh:mm tt");

                    var mailMessage = new MailMessage
                    {
                        Subject = args.TradeName,
                        Body = $"Start Date: {args.StartDate:dd-MM-yyyy}<br/>End Date: {args.EndDate:dd-MM-yyyy}<br/>Send Time: {formattedTime}",
                        IsBodyHtml = true
                    };

                    foreach (var attachment in attachments)
                    {
                        mailMessage.Attachments.Add(attachment); // Attach the Excel files to the email
                    }

                    args.RecipientsList?.ForEach(mailMessage.To.Add);

                    await _emailSender.SendAsync(mailMessage);
                    await _automaticEmailAppService.UpdateJobStatusAsync(id, JobStatus.Success, args.TenantId);
                }
            }
            catch
            {
                await _automaticEmailAppService.UpdateJobStatusAsync(id, JobStatus.Failed, args.TenantId);
            }
        }
    }
}
