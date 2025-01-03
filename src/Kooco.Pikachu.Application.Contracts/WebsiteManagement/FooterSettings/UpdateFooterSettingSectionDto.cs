using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities;

namespace Kooco.Pikachu.WebsiteManagement.FooterSettings;

public class UpdateFooterSettingSectionDto
{
    [Required]
    public FooterSettingsPosition FooterSettingsPosition { get; set; }

    [Required]
    [MaxLength(FooterSettingsConsts.MaxTitleLength)]
    public string Title { get; set; }

    [Required]
    public FooterSettingsType? FooterSettingsType { get; set; }

    [RequiredIfFooterSettingsType(FooterSettings.FooterSettingsType.Text)]
    [MaxLength(FooterSettingsConsts.MaxTextLength)]
    public string Text { get; set; }
    public string ImageUrl { get; set; }
    public string ImageBase64 { get; set; }
    
    [RequiredIfFooterSettingsType(FooterSettings.FooterSettingsType.Image)]
    public string ImageName { get; set; }
    public List<UpdateFooterSettingLinkDto> Links { get; set; }
    public Guid? WebsiteBasicSettingsId { get; set; }
    public Guid FooterSettingId { get; set; }

    public UpdateFooterSettingSectionDto(FooterSettingsPosition footerSettingsPosition)
    {
        FooterSettingsPosition = footerSettingsPosition;
        Links = [];
    }

    public class RequiredIfFooterSettingsTypeAttribute : ValidationAttribute
    {
        private readonly FooterSettingsType _requiredType;

        public RequiredIfFooterSettingsTypeAttribute(FooterSettingsType requiredType)
        {
            _requiredType = requiredType;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // Get the FooterSettingsType property value
            var typeProperty = validationContext.ObjectType.GetProperty(nameof(FooterSettingsType));
            var typeValue = typeProperty?.GetValue(validationContext.ObjectInstance, null);

            // Check if FooterSettingsType matches the required type
            if (typeValue != null && typeValue.Equals(_requiredType))
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
}
