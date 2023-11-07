using Hangfire;
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
    public class AutomaticEmailsJob : AsyncBackgroundJob<AutomaticEmailDto>, ITransientDependency
    {
        private readonly IEmailSender _emailSender;
        private readonly IGroupBuyAppService _groupBuyAppService;

        public AutomaticEmailsJob(
            IEmailSender emailSender,
            IGroupBuyAppService groupBuyAppService
            )
        {
            _emailSender = emailSender;
            _groupBuyAppService = groupBuyAppService;
        }

        public override async Task ExecuteAsync(AutomaticEmailDto args)
        {
            var attachments = new List<Attachment>();
            if (args.GroupBuys != null)
            {
                foreach (var groupBuy in args.GroupBuys)
                {
                    var data = await _groupBuyAppService.GetListAsExcelFileAsync(groupBuy.GroupBuyId);
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
                DateTimeOffset creationTimeInTimeZone = TimeZoneInfo.ConvertTime(args.SendTime, tz);
                string formattedTime = creationTimeInTimeZone.ToString("hh:mm tt");

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

                args.RecipientsList.ForEach(mailMessage.To.Add);

                await _emailSender.SendAsync(mailMessage);
            }
        }
    }
}
