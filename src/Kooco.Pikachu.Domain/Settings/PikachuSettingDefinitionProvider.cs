using Volo.Abp.Settings;

namespace Kooco.Pikachu.Settings;

public class PikachuSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(PikachuSettings.MySetting1));
    }
}
