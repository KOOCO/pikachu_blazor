using Hangfire;
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
        
        public AutomaticEmailsJob(
            IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        public override async Task ExecuteAsync(AutomaticEmailDto args)
        {
            await _emailSender.SendAsync(
                "mivet61715@newnime.com",
                args.TradeName,
                @$"Start Date: {args.StartDate}<br/>
                    End Date: {args.EndDate}<br/>
                    Send Time: {args.SendTime}"
            );
        }
    }
}
