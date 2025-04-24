
using System;
using System.Threading.Tasks;
using Localization.Resources.AbpUi;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Volo.Abp.AspNetCore.Components.Messages;
using Volo.Abp.DependencyInjection;

namespace Kooco.Pikachu.Blazor.Components.Messages;

[Dependency(ReplaceServices = true)]
public class MyBlazoriseUiMessageService : IUiMessageService, IScopedDependency
{
    public event EventHandler<UiMessageEventArgs>? MessageReceived;

    private readonly IStringLocalizer<AbpUiResource> _localizer;

    public ILogger<MyBlazoriseUiMessageService> Logger { get; set; }

    public MyBlazoriseUiMessageService(IStringLocalizer<AbpUiResource> localizer)
    {
        _localizer = localizer;
        Logger = NullLogger<MyBlazoriseUiMessageService>.Instance;
    }

    public Task<bool> Confirm(string message, string? title = null, Action<UiMessageOptions>? options = null)
    {
        var uiMessageOptions = CreateDefaultOptions();
        options?.Invoke(uiMessageOptions);

        var callback = new TaskCompletionSource<bool>();

        MessageReceived?.Invoke(this, new UiMessageEventArgs(
            UiMessageType.Confirmation,
            message,
            title,
            uiMessageOptions,
            callback
        ));

        return callback.Task;
    }

    private UiMessageOptions CreateDefaultOptions()
    {
        return new UiMessageOptions
        {
            CenterMessage = true,
            ShowMessageIcon = true,
            OkButtonText = _localizer["Ok"],
            CancelButtonText = _localizer["Cancel"],
            ConfirmButtonText = _localizer["Yes"],
        };
    }

    public Task Info(string message, string? title = null, Action<UiMessageOptions>? options = null)
    {
        var uiMessageOptions = CreateDefaultOptions();
        options?.Invoke(uiMessageOptions);

        MessageReceived?.Invoke(this, new UiMessageEventArgs(UiMessageType.Info, message, title, uiMessageOptions));

        return Task.CompletedTask;
    }

    public Task Success(string message, string? title = null, Action<UiMessageOptions>? options = null)
    {
        var uiMessageOptions = CreateDefaultOptions();
        options?.Invoke(uiMessageOptions);

        MessageReceived?.Invoke(this, new UiMessageEventArgs(UiMessageType.Success, message, title, uiMessageOptions));

        return Task.CompletedTask;
    }

    public Task Warn(string message, string? title = null, Action<UiMessageOptions>? options = null)
    {
        var uiMessageOptions = CreateDefaultOptions();
        options?.Invoke(uiMessageOptions);

        MessageReceived?.Invoke(this, new UiMessageEventArgs(UiMessageType.Warning, message, title, uiMessageOptions));

        return Task.CompletedTask;
    }

    public Task Error(string message, string? title = null, Action<UiMessageOptions>? options = null)
    {
        var uiMessageOptions = CreateDefaultOptions();
        options?.Invoke(uiMessageOptions);

        MessageReceived?.Invoke(this, new UiMessageEventArgs(UiMessageType.Error, message, title, uiMessageOptions));

        return Task.CompletedTask;
    }
}

