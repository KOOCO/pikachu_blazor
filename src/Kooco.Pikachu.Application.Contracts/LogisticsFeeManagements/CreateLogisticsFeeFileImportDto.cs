using Kooco.Pikachu.EnumValues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooco.Pikachu.LogisticsFeeManagements
{
    public class CreateLogisticsFeeFileImportDto
    {
        public string FileName { get; set; }
        public string OriginalFileName { get; set; }
        public string FilePath { get; set; }
        public LogisticsFileType FileType { get; set; }
    }
}
