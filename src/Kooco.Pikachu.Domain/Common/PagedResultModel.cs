using System.Collections.Generic;

namespace Kooco.Pikachu.Common;

public class PagedResultModel<T>
{
    public long TotalCount { get; set; }
    public List<T> Items { get; set; } = [];
}
