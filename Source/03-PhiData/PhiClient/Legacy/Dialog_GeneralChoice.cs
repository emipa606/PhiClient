using UnityEngine;
using Verse;

namespace PhiClient.Legacy;

public class Dialog_GeneralChoice : Window
{
    private readonly DialogChoiceConfig config;

    public Dialog_GeneralChoice(DialogChoiceConfig config)
    {
        this.config = config;
        forcePause = true;
        absorbInputAroundWindow = true;
        if (config.buttonAAction == null)
        {
            config.buttonAText = "OK".Translate();
        }
    }

    public override Vector2 InitialSize => new Vector2(600f, 400f);

    public override void DoWindowContents(Rect inRect)
    {
        Text.Font = GameFont.Small;
        Widgets.Label(new Rect(0f, 0f, inRect.width, inRect.height), config.text);
        if (config.buttonAText != string.Empty && Widgets.ButtonText(
                new Rect(0f, inRect.height - 35f, (inRect.width / 2f) - 20f, 35f), config.buttonAText, true, false))
        {
            config.buttonAAction?.Invoke();

            Close();
        }

        if (config.buttonBText == string.Empty || !Widgets.ButtonText(
                new Rect((inRect.width / 2f) + 20f, inRect.height - 35f, (inRect.width / 2f) - 20f, 35f),
                config.buttonBText, true, false))
        {
            return;
        }

        config.buttonBAction?.Invoke();

        Close();
    }
}