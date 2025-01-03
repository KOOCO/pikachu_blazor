using System.ComponentModel.DataAnnotations;

namespace Kooco.Pikachu.WebsiteManagement.TopbarSettings;

public class UpdateTopbarSettingCategoryOptionDto
{
    [Required]
    public TopbarCategoryLinkOption TopbarCategoryLinkOption { get; set; }
    
    public int Index { get; set; }

    [Required]
    public string Title { get; set; }

    [Required]
    public string Link { get; set; }

    public UpdateTopbarSettingCategoryOptionDto(int index, TopbarCategoryLinkOption topbarCategoryLinkOption)
    {
        Index = index;
        TopbarCategoryLinkOption = topbarCategoryLinkOption;
    }
}
