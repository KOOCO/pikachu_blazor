namespace Kooco.Reconciliations;

public class EcPayReconciliationInput
{
    public string MerchantID { get; set; }
    public string HashKey { get; set; }
    public string HashIV { get; set; }
    public DateTime BeginDate { get; set; }
    public DateTime EndDate { get; set; }
}
