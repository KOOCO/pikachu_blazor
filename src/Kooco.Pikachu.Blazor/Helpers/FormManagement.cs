using System.Collections.Generic;
using System;

namespace Kooco.Pikachu.Blazor.Helpers;
public abstract class FormManagement<T1, T2> : FormComponentBase where T1 : FormEntranceBase
{
    protected bool IsInitialLoad = true;
    protected T1 Entrance { get; set; } = Activator.CreateInstance<T1>()!;
    protected IReadOnlyList<T2> Entities { get; set; } = [];
}