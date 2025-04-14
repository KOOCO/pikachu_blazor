namespace Kooco.Pikachu.Tenants;

//[Queue("automatic-issue-invoice")]
//public class GenerateInvoiceBackgroundJob : AsyncBackgroundJob<GenerateInvoiceBackgroundJobArgs>, ITransientDependency
//{
//    public override async Task ExecuteAsync(GenerateInvoiceBackgroundJobArgs args)
//    {
//        await OrderInvoiceAppService.CreateInvoiceAsync(args.OrderId);
//    }

//    public IOrderInvoiceAppService OrderInvoiceAppService { get; set; }
//}