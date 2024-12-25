using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.WebsiteManagement.FooterSettings;

public class FooterSettingSection : Entity<Guid>, IMultiTenant
{
    public FooterSettingsPosition FooterSettingsPosition { get; set; }
    public string Title { get; set; }
    public FooterSettingsType FooterSettingsType { get; set; }
    public string? Text { get; private set; }
    public string? ImageUrl { get; private set; }
    public string? ImageName { get; private set; }
    public Guid FooterSettingId { get; set; }
    public Guid? TenantId { get; set; }

    [ForeignKey(nameof(FooterSettingId))]
    public virtual FooterSetting FooterSetting { get; set; }
    public virtual ICollection<FooterSettingLink> Links { get; set; }

    public FooterSettingSection(
        Guid id,
        FooterSettingsPosition footerSettingsPosition,
        string title,
        FooterSettingsType footerSettingsType,
        string text,
        string imageUrl,
        string imageName,
        Guid footerSettingId
        ) : base(id)
    {
        FooterSettingsPosition = footerSettingsPosition;
        FooterSettingsType = footerSettingsType;
        ImageUrl = imageUrl;
        ImageName = imageName;
        FooterSettingId = footerSettingId;
        SetTitle(title);
        SetText(footerSettingsType, text);
        SetImage(footerSettingsType, imageUrl, imageName);
        Links = new List<FooterSettingLink>();
    }

    public void SetTitle(string title)
    {
        Title = Check.NotNullOrWhiteSpace(title, nameof(Title), maxLength: FooterSettingsConsts.MaxTitleLength);
    }

    public void SetText(FooterSettingsType footerSettingsType, string text)
    {
        if (footerSettingsType == FooterSettingsType.Text)
        {
            Text = Check.NotNullOrWhiteSpace(text, nameof(Text), maxLength: FooterSettingsConsts.MaxTextLength);
        }
    }

    public void SetImage(FooterSettingsType footerSettingsType, string imageUrl, string imageName)
    {
        if (footerSettingsType == FooterSettingsType.Image)
        {
            ImageUrl = imageUrl;
            ImageName = imageName;
        }
    }

    public FooterSettingLink AddLink(Guid id, int index, string title, string url)
    {
        if (Links.Count >= FooterSettingsConsts.MaxAllowedLinks)
        {
            throw new InvalidNumberOfLinksException(FooterSettingsConsts.MaxAllowedLinks);
        }
        var link = new FooterSettingLink(id, index, title, url, Id);
        Links.Add(link);
        return link;
    }
}