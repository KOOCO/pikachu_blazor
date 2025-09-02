using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooco.Pikachu.LogisticsFeeManagements
{
    public class ExportLogisticsFeeRecordsInput : GetTenantLogisticsFeeRecordsInput
    {
        public ExportFileFormat FileFormat { get; set; } = ExportFileFormat.Excel;
    }

    public enum ExportFileFormat
    {
        Csv = 1,
        Excel = 2
    }
}
