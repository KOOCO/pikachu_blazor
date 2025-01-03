using System;
using System.Collections.Generic;
using System.Text;

namespace Kooco.Pikachu.Orders
{
	public class ExpireOrderBackgroundJobArgs
	{
		public Guid OrderId { get; set; }
	}
}
