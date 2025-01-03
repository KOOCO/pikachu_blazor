using System;
using System.Collections.Generic;
using System.Text;

namespace Kooco.Pikachu.OBTStatus
{
	public class OBTStatusResponseDto
	{
		public string SrvTranId { get; set; }
		public string IsOK { get; set; }
		public string Message { get; set; }
		public OBTStatusDataDto Data { get; set; }
	}
}
