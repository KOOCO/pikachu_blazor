using Microsoft.AspNetCore.Components;
using System;

namespace Kooco.Pikachu.Blazor.Pages.Members;

public partial class EditMember
{
    [Parameter]
    public Guid Id { get; set; }
}