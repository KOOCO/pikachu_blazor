using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Kooco.Pikachu.OBTStatus
{
	public class OBTStatusRequestDto
	{
		[Required]
		[StringLength(12)]
		public string CustomerId { get; set; }

		[Required]
		[StringLength(50)]
		public string CustomerToken { get; set; }

		[Required]
		public List<string> OBTNumbers { get; set; }
	}
}
