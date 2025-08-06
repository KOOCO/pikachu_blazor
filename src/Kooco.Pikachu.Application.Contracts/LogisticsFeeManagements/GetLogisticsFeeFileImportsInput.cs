using Kooco.Pikachu.EnumValues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.LogisticsFeeManagements
{
    public class GetLogisticsFeeFileImportsInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
        public LogisticsFileType? FileType { get; set; }
        public FileProcessingStatus? Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
