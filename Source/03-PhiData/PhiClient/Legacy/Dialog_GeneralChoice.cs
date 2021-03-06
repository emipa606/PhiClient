﻿using UnityEngine;
using Verse;

namespace PhiClient.Legacy
{
    // Token: 0x0200002C RID: 44
    public class Dialog_GeneralChoice : Window
    {
        // Token: 0x0400009E RID: 158
        private readonly DialogChoiceConfig config;

        // Token: 0x0600007C RID: 124 RVA: 0x000049A1 File Offset: 0x00002BA1
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

        // Token: 0x17000004 RID: 4
        // (get) Token: 0x0600007B RID: 123 RVA: 0x00004990 File Offset: 0x00002B90
        public override Vector2 InitialSize => new Vector2(600f, 400f);

        // Token: 0x0600007D RID: 125 RVA: 0x000049D8 File Offset: 0x00002BD8
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
}