using System;
using UnityEngine;
using Verse;

namespace PhiClient.Legacy;

public class Dialog_Confirm : Window
{
    private const float TitleHeight = 40f;

    private readonly Action confirmedAction;

    private readonly float createRealTime;

    private readonly bool destructiveAction;

    private readonly string text;

    private readonly string title;

    public string confirmLabel;

    public string goBackLabel;

    public float interactionDelay;

    private Vector2 scrollPos;

    private float scrollViewHeight;

    public bool showGoBack;

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

    private float TimeUntilInteractive => interactionDelay - (Time.realtimeSinceStartup - createRealTime);

    private bool InteractionDelayExpired => TimeUntilInteractive <= 0f;

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
            ? $"{confirmLabel}({Mathf.Ceil(TimeUntilInteractive):F0})"
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