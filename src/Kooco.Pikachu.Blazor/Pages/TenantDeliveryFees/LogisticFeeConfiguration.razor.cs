using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Blazorise;
using Microsoft.AspNetCore.Components;

using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.TenantDeliveryFees;
using Volo.Abp.AspNetCore.Components;
using Microsoft.AspNetCore.Authorization;
using Kooco.Pikachu.Permissions;
namespace Kooco.Pikachu.Blazor.Pages.TenantDeliveryFees
{

    public partial class LogisticFeeConfiguration
    {
        [Parameter] public Guid TenantId { get; set; }



        private Validations? formValidations;

        // UI state flags (bind to buttons/spinners if you want)
        private bool IsLoading { get; set; }
        private bool IsSaving { get; set; }

        // lightweight VM (Blazorise <Validation> drives rules)
        private UpsertTenantDeliveryFeesVM Vm { get; set; } = new();
        private bool CanManage;
        protected override async Task OnInitializedAsync()
        {


            await LoadAsync();
            CanManage = await Authorization.IsGrantedAsync(PikachuPermissions.Feeds.Manage);
        }

        private async Task LoadAsync()
        {
            IsLoading = true;
            try
            {
                var existingPaged = await FeeApp.GetListAsync(new TenantDeliveryFeeGetListInput { TenantId = TenantId });

                Vm.TenantId = TenantId;
                Vm.Items = Enum.GetValues<DeliveryProvider>()
                    .Select(p =>
                    {
                        var found = existingPaged.Items.FirstOrDefault(x => x.DeliveryProvider == p);
                        return new TenantDeliveryFeeItemVM
                        {
                            DeliveryProvider = p,
                            IsEnabled = found?.IsEnabled ?? false,
                            FeeKind = found?.FeeKind ?? FeeKind.Percentage,
                            PercentValue = found?.PercentValue,
                            FixedAmount = found?.FixedAmount
                        };
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
                // Fallback to empty/default rows so the page remains usable
                Vm.TenantId = TenantId;
                Vm.Items = Enum.GetValues<DeliveryProvider>()
                    .Select(p => new TenantDeliveryFeeItemVM { DeliveryProvider = p })
                    .ToList();
            }
            finally
            {
                IsLoading = false;
                await InvokeAsync(StateHasChanged);
            }
        }

        private void OnEnabledChanged(TenantDeliveryFeeItemVM item)
        {
            if (!item.IsEnabled)
            {
                // Keep as you had: zero them out to avoid nulls
                item.PercentValue = 0;
                item.FixedAmount = 0;
            }
            _ = formValidations?.ValidateAll();
        }

        private void OnFeeKindChanged(FeeKind value, TenantDeliveryFeeItemVM item)
        {
            if (value == FeeKind.Percentage)
                item.FixedAmount = 0;
            else
                item.PercentValue = 0;

            item.FeeKind = value;
            _ = formValidations?.ValidateAll();
        }

        // ----- Blazorise validators (ABP-docs style) -----
        private Task ValidatePercent(ValidatorEventArgs e, TenantDeliveryFeeItemVM item)
        {
            if (!item.IsEnabled || item.FeeKind != FeeKind.Percentage)
            {
                e.Status = ValidationStatus.Success;
                return Task.CompletedTask;
            }

            if (e.Value is null || string.IsNullOrWhiteSpace(e.Value.ToString()))
            {
                e.ErrorText = L["Validation:PercentRequired"];
                e.Status = ValidationStatus.Error;
                return Task.CompletedTask;
            }

            if (!decimal.TryParse(e.Value.ToString(), out var v) || v < 0 || v > 100)
            {
                e.ErrorText = L["Validation:PercentRange"];
                e.Status = ValidationStatus.Error;
                return Task.CompletedTask;
            }

            e.Status = ValidationStatus.Success;
            return Task.CompletedTask;
        }

        private Task ValidateFixed(ValidatorEventArgs e, TenantDeliveryFeeItemVM item)
        {
            if (!item.IsEnabled || item.FeeKind != FeeKind.FixedAmount)
            {
                e.Status = ValidationStatus.Success;
                return Task.CompletedTask;
            }

            if (e.Value is null || string.IsNullOrWhiteSpace(e.Value.ToString()))
            {
                e.ErrorText = L["Validation:AmountRequired"];
                e.Status = ValidationStatus.Error;
                return Task.CompletedTask;
            }

            if (!decimal.TryParse(e.Value.ToString(), out var v) || v < 0)
            {
                e.ErrorText = L["Validation:AmountMinZero"];
                e.Status = ValidationStatus.Error;
                return Task.CompletedTask;
            }

            e.Status = ValidationStatus.Success;
            return Task.CompletedTask;
        }

        private async Task SaveAsync()
        {
            if (formValidations is null) return;

            IsSaving = true;
            try
            {
                var ok = await formValidations.ValidateAll();
                if (!ok)
                {
                    await MessageService.Warning(L["Validation:FixErrors"]);
                    return;
                }

                var input = new UpsertTenantDeliveryFeesInput
                {
                    TenantId = Vm.TenantId,
                    Items = Vm.Items.Select(i => new TenantDeliveryFeeItemInput
                    {
                        DeliveryProvider = i.DeliveryProvider,
                        IsEnabled = i.IsEnabled,
                        FeeKind = i.FeeKind,
                        PercentValue = i.IsEnabled && i.FeeKind == FeeKind.Percentage ? i.PercentValue : null,
                        FixedAmount = i.IsEnabled && i.FeeKind == FeeKind.FixedAmount ? i.FixedAmount : null,
                    }).ToList()
                };

                await FeeApp.UpsertManyAsync(input);
                await MessageService.Success(L["Save:Success"]);
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
            finally
            {
                IsSaving = false;
            }
        }

        private async Task ResetAsync()
        {
            try
            {
                await LoadAsync();
                await formValidations?.ClearAll();
                await MessageService.Info(L["Reset:Reloaded"]);
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        // ----- simple VMs -----
        private class UpsertTenantDeliveryFeesVM
        {
            public Guid? TenantId { get; set; }
            public List<TenantDeliveryFeeItemVM> Items { get; set; } = new();
        }

        private class TenantDeliveryFeeItemVM
        {
            public DeliveryProvider DeliveryProvider { get; set; }
            public bool IsEnabled { get; set; }
            public FeeKind FeeKind { get; set; } = FeeKind.Percentage;
            public decimal? PercentValue { get; set; }
            public decimal? FixedAmount { get; set; }
        }
    }


}
