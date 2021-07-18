using System;
using UnityEngine;
using Verse;

namespace PhiClient.Legacy
{
    // Token: 0x0200002B RID: 43
    public class Dialog_Confirm : Window
    {
        // Token: 0x04000092 RID: 146
        private const float TitleHeight = 40f;

        // Token: 0x04000094 RID: 148
        private readonly Action confirmedAction;

        // Token: 0x0400009D RID: 157
        private readonly float createRealTime;

        // Token: 0x04000095 RID: 149
        private readonly bool destructiveAction;

        // Token: 0x04000093 RID: 147
        private readonly string text;

        // Token: 0x04000096 RID: 150
        private readonly string title;

        // Token: 0x04000097 RID: 151
        public string confirmLabel;

        // Token: 0x04000098 RID: 152
        public string goBackLabel;

        // Token: 0x0400009A RID: 154
        public float interactionDelay;

        // Token: 0x0400009B RID: 155
        private Vector2 scrollPos;

        // Token: 0x0400009C RID: 156
        private float scrollViewHeight;

        // Token: 0x04000099 RID: 153
        public bool showGoBack;

        // Token: 0x06000079 RID: 121 RVA: 0x00004708 File Offset: 0x00002908
        public Dialog_Confirm(string text, Action confirmedAction, bool destructive = false, string title = null,
            bool showGoBack = true)
        {
            this.text = text;
            this.confirmedAction = confirmedAction;
            destructiveAction = destructive;
            this.title = title;
            this.showGoBack = showGoBack;
            confirmLabel = "Confirm".Translate();
            goBackLabel = "GoBack".Translate();
            forcePause = true;
            absorbInputAroundWindow = true;
            createRealTime = Time.realtimeSinceStartup;
        }

        // Token: 0x17000001 RID: 1
        // (get) Token: 0x06000076 RID: 118 RVA: 0x000046B0 File Offset: 0x000028B0
        public override Vector2 InitialSize
        {
            get
            {
                var num = 300f;
                if (title != null)
                {
                    num += 40f;
                }

                return new Vector2(500f, num);
            }
        }

        // Token: 0x17000002 RID: 2
        // (get) Token: 0x06000077 RID: 119 RVA: 0x000046DE File Offset: 0x000028DE
        private float TimeUntilInteractive => interactionDelay - (Time.realtimeSinceStartup - createRealTime);

        // Token: 0x17000003 RID: 3
        // (get) Token: 0x06000078 RID: 120 RVA: 0x000046F3 File Offset: 0x000028F3
        private bool InteractionDelayExpired => TimeUntilInteractive <= 0f;

        // Token: 0x0600007A RID: 122 RVA: 0x0000477C File Offset: 0x0000297C
        public override void DoWindowContents(Rect inRect)
        {
            var num = inRect.y;
            if (!title.NullOrEmpty())
            {
                Text.Font = GameFont.Medium;
                Widgets.Label(new Rect(0f, num, inRect.width, 40f), title);
                num += 40f;
            }

            Text.Font = GameFont.Small;
            var rect = new Rect(0f, num, inRect.width, inRect.height - 45f - num);
            var rect2 = new Rect(0f, 0f, inRect.width - 16f, scrollViewHeight);
            Widgets.BeginScrollView(rect, ref scrollPos, rect2);
            Widgets.Label(new Rect(0f, 0f, rect2.width, scrollViewHeight), text);
            if (Event.current.type == EventType.Layout)
            {
                scrollViewHeight = Text.CalcHeight(text, rect2.width);
            }

            Widgets.EndScrollView();
            if (destructiveAction)
            {
                GUI.color = new Color(1f, 0.3f, 0.35f);
            }

            var label = !InteractionDelayExpired
                ? confirmLabel + "(" + Mathf.Ceil(TimeUntilInteractive).ToString("F0") + ")"
                : confirmLabel;
            if (Widgets.ButtonText(
                new Rect((inRect.width / 2f) + 20f, inRect.height - 35f, (inRect.width / 2f) - 20f, 35f), label, true,
                false) && InteractionDelayExpired)
            {
                confirmedAction();
                Close();
            }

            GUI.color = Color.white;
            if (showGoBack && Widgets.ButtonText(new Rect(0f, inRect.height - 35f, (inRect.width / 2f) - 20f, 35f),
                goBackLabel, true, false))
            {
                Close();
            }
        }
    }
}