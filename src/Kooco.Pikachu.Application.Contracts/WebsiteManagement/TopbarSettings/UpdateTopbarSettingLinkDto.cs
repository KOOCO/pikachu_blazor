using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kooco.Pikachu.WebsiteManagement.TopbarSettings;

public class UpdateTopbarSettingLinkDto
{
    [Required]
    public TopbarLinkSettings TopbarLinkSettings { get; set; }
    public int Index { get; set; }

    [Required]
    public string Title { get; set; }

    [RequiredIfTopbarLinkSettingsIsLink]
    public string? Url { get; set; }

    [Required]
    public List<UpdateTopbarSettingCategoryOptionDto> CategoryOptions { get; set; } = [];

    public UpdateTopbarSettingLinkDto(int index, TopbarLinkSettings topbarLinkSettings)
    {
        Index = index;
        TopbarLinkSettings = topbarLinkSettings;
    }
}

public class RequiredIfTopbarLinkSettingsIsLinkAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var typeProperty = validationContext.ObjectType.GetProperty(nameof(TopbarLinkSettings));
        var typeValue = typeProperty?.GetValue(validationContext.ObjectInstance, null);

        if (typeValue != null && typeValue.Equals(TopbarLinkSettings.Link))
        {
            // Validate the current property (e.g., Text or ImageUrl)
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return new ValidationResult(ErrorMessage, [validationContext.DisplayName]);
            }
        }

        return ValidationResult.Success;
    }
}