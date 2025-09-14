using Kooco.Pikachu.TenantPaymentFees;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Blazor.Pages.TenantPayouts;

public static class TenantPayoutUiHelper
{
    public static List<TenantPayoutStep> ConfigureSteps(TenantPayout component, IStringLocalizer l)
    {
        var steps = new List<TenantPayoutStep>
        {
            new() {
                Step = 1,
                Title = l["TenantList"].Value,
                SubTitle = l["SelectTenantToViewPayoutFeeDetail"].Value,
                CanGoBack = false,
                BackText = null,
                BackFunction = null
            },
            new() {
                Step = 2,
                Title = l["PaymentProviders"].Value,
                SubTitle = l["SelectPaymentProviderToViewPaymentDetails"].Value,
                CanGoBack = true,
                BackText = l["BackToTenants"].Value,
                BackFunction = () =>
                {
                    GoToStep(component, 1);
                    return Task.CompletedTask;
                }
            },
            new() {
                Step = 3,
                Title = l["PayoutByYear"].Value,
                SubTitle = l["SelectYearToViewDetailedPayoutRecords"].Value,
                CanGoBack = true,
                BackText = l["BackToProviders"].Value,
                BackFunction = () =>
                {
                    GoToStep(component, 2);
                    return Task.CompletedTask;
                }
            },
            new() {
                Step = 4,
                Title = l["PayoutDetails"].Value,
                SubTitle = l["TransactionRecordsAndPayoutManagementOperations"].Value,
                CanGoBack = true,
                BackText = l["BackToYears"].Value,
                BackFunction = () =>
                {
                    GoToStep(component, 3);
                    return Task.CompletedTask;
                }
            }
        };

        return steps;
    }

    public static void GoToStep(TenantPayout component, int step)
    {
        if (component.Steps == null || component.Steps.Count == 0) return;

        var target = component.Steps.FirstOrDefault(s => s.Step == step);
        if (target == null) return;

        component.CurrentStep = target;

        switch (step)
        {
            case 1:
                component.Context.TenantId = null;
                component.Context.FeeType = null;
                component.Context.Year = null;
                break;
            case 2:
                component.Context.FeeType = null;
                component.Context.Year = null;
                break;
            case 3:
                component.Context.Year = null;
                break;
        }
    }

    public static string GetInitials(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return string.Empty;

        // Normalize: trim + replace underscores with spaces
        name = name.Trim().Replace("_", " ").Replace("-", " ");

        var parts = new List<string>();

        // First, split by space
        foreach (var part in name.Split(' ', StringSplitOptions.RemoveEmptyEntries))
        {
            // If part is PascalCase or camelCase, split into tokens
            var tokens = System.Text.RegularExpressions.Regex
                .Split(part, @"(?<!^)(?=[A-Z])") // split before uppercase letters
                .Where(t => !string.IsNullOrWhiteSpace(t));

            parts.AddRange(tokens);
        }

        if (parts.Count == 0)
            return string.Empty;

        // Always take first letter of first part
        var first = char.ToUpperInvariant(parts[0][0]).ToString();

        // Second initial: from second part (if exists)
        var second = parts.Count > 1
            ? char.ToUpperInvariant(parts[1][0]).ToString()
            : string.Empty;

        return first + second;
    }

    public static IEnumerable<TenantPayoutBreadcrumb> GetBreadcrumbs(
        string? tenantName,
        PaymentFeeType? feeType,
        int? year
        )
    {
        var crumbs = new List<TenantPayoutBreadcrumb> { new(1, "Dashboard") };

        if (!string.IsNullOrWhiteSpace(tenantName))
            crumbs.Add(new(2, tenantName));

        if (feeType.HasValue)
            crumbs.Add(new(3, $"Enum:TenantPaymentFee.{(int)feeType.Value}"));

        if (year.HasValue)
        {
            crumbs.Add(new(4, year.Value.ToString()));
            crumbs.Add(new(5, "PayoutDetails"));
        }

        return crumbs;
    }
}

public class TenantPayoutStep
{
    public int Step { get; set; }
    public string Title { get; set; }
    public string? SubTitle { get; set; }
    public string? BackText { get; set; }
    public bool CanGoBack { get; set; }
    public Func<Task>? BackFunction { get; set; }
}

public class TenantPayoutBreadcrumb(int step, string name)
{
    public int Step { get; set; } = step;
    public string Name { get; set; } = name;
}