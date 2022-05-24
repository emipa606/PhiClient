using System;
using System.Collections.Generic;
using System.Linq;
using PhiClient.UI;
using RimWorld;
using UnityEngine;
using Verse;
using WebSocketSharp;

namespace PhiClient;

public class ServerMainTab : MainTabWindow
{
    private const float TITLE_HEIGHT = 45f;

    private const float CHAT_INPUT_HEIGHT = 30f;

    private const float CHAT_INPUT_SEND_BUTTON_WIDTH = 100f;

    private const float CHAT_MARGIN = 10f;

    private const float STATUS_AREA_WIDTH = 160f;

    private Vector2 chatScroll = Vector2.zero;

    private string enteredMessage = "";

    private string filterName = "";

    private Vector2 userScrollPosition = Vector2.zero;

    public ServerMainTab()
    {
        doCloseX = true;
        closeOnAccept = false;
        closeOnClickedOutside = false;
        forceCatchAcceptAndCancelEventEvenIfUnfocused = true;
    }

    public override void DoWindowContents(Rect inRect)
    {
        var unused = PhiClient.instance;
        var listContainer = new ListContainer { spaceBetween = 10f };
        listContainer.Add(new TextWidget("Realm", GameFont.Medium, TextAnchor.MiddleCenter));
        listContainer.Add(new ListContainer(new List<Displayable>
        {
            DoChat(),
            new WidthContainer(DoBodyRightBar(), 160f)
        }, ListFlow.ROW)
        {
            spaceBetween = 10f
        });
        listContainer.Add(new HeightContainer(DoFooter(), 30f));
        listContainer.Draw(inRect);
    }

    private Displayable DoChat()
    {
        var phi = PhiClient.instance;
        var listContainer = new ListContainer(ListFlow.COLUMN, ListDirection.OPPOSITE);
        if (!phi.IsUsable())
        {
            return new ScrollContainer(listContainer, chatScroll, delegate(Vector2 v) { chatScroll = v; });
        }

        foreach (var chatMessage in phi.realmData.chat.Reverse<ChatMessage>().Take(30))
        {
            var idx = phi.realmData.users.LastIndexOf(chatMessage.user);
            listContainer.Add(new ButtonWidget($"{phi.realmData.users[idx].name}: {chatMessage.message}",
                delegate { OnUserClick(phi.realmData.users[idx]); }, false));
        }

        return new ScrollContainer(listContainer, chatScroll, delegate(Vector2 v) { chatScroll = v; });
    }

    private Displayable DoBodyRightBar()
    {
        var instance = PhiClient.instance;
        var listContainer = new ListContainer { spaceBetween = 10f };
        var text = "Status: ";
        WebSocketState? webSocketState;
        if (instance == null)
        {
            webSocketState = null;
        }
        else
        {
            var client = instance.client;
            webSocketState = client != null ? new WebSocketState?(client.state) : null;
        }

        var webSocketState2 = webSocketState;
        if (webSocketState2 != null)
        {
            switch (webSocketState2.GetValueOrDefault())
            {
                case WebSocketState.Connecting:
                    text += "Connecting";
                    goto IL_BC;
                case WebSocketState.Open:
                    text += "Connected";
                    goto IL_BC;
                case WebSocketState.Closing:
                    text += "Disconnecting";
                    goto IL_BC;
                case WebSocketState.Closed:
                    text += "Disconnected";
                    goto IL_BC;
            }
        }

        text += "Disconnected";
        IL_BC:
        listContainer.Add(new TextWidget(text));
        listContainer.Add(
            new HeightContainer(new ButtonWidget("Configuration", OnConfigurationClick), 30f));
        listContainer.Add(new Container(new TextFieldWidget(filterName, delegate(string s) { filterName = s; }),
            150f, 30f));
        if (instance != null && !instance.IsUsable())
        {
            return listContainer;
        }

        var listContainer2 = new ListContainer();
        using (var enumerator = (from u in instance?.realmData.users
                   where u.connected
                   select u).GetEnumerator())
        {
            while (enumerator.MoveNext())
            {
                var item = enumerator.Current;
                if (filterName != "")
                {
                    if (ContainsStringIgnoreCase(item?.name, filterName))
                    {
                        listContainer2.Add(new ButtonWidget(item?.name, delegate { OnUserClick(item); }, false));
                    }
                }
                else
                {
                    listContainer2.Add(new ButtonWidget(item?.name, delegate { OnUserClick(item); }, false));
                }
            }
        }

        listContainer.Add(new ScrollContainer(listContainer2, userScrollPosition,
            delegate(Vector2 v) { userScrollPosition = v; }));

        return listContainer;
    }

    private void OnConfigurationClick()
    {
        Find.WindowStack.Add(new ServerMainMenuWindow());
    }

    private bool ContainsStringIgnoreCase(string hay, string needle)
    {
        return hay.IndexOf(needle, StringComparison.OrdinalIgnoreCase) >= 0;
    }

    private Displayable DoFooter()
    {
        var listContainer = new ListContainer(ListFlow.ROW) { spaceBetween = 10f };
        if (Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.KeypadEnter ||
                                                        Event.current.keyCode == KeyCode.Return))
        {
            OnSendClick();
            Event.current.Use();
        }

        listContainer.Add(new TextFieldWidget(enteredMessage,
            delegate(string s) { enteredMessage = OnEnteredMessageChange(s); }));
        listContainer.Add(new WidthContainer(new ButtonWidget("Send", OnSendClick), 100f));
        return listContainer;
    }

    public override void OnAcceptKeyPressed()
    {
        OnSendClick();
    }

    public void OnSendClick()
    {
        if (enteredMessage.Trim().IsNullOrEmpty())
        {
            return;
        }

        PhiClient.instance.SendMessage(enteredMessage);
        enteredMessage = "";
    }

    public string OnEnteredMessageChange(string newMessage)
    {
        return newMessage;
    }

    public void OnReconnectClick()
    {
        PhiClient.instance.TryConnect();
    }

    public void OnUserClick(User user)
    {
        var instance = PhiClient.instance;
        var unused = user;
        var unused1 = instance.currentUser;
        var list = new List<FloatMenuOption>
        {
            new FloatMenuOption("Ship items", delegate { OnShipItemsOptionClick(user); }),
            new FloatMenuOption("Send colonist", delegate { OnSendColonistOptionClick(user); }),
            new FloatMenuOption("Send animal", delegate { OnSendAnimalOptionClick(user); })
        };
        Find.WindowStack.Add(new FloatMenu(list));
    }

    public void OnSendColonistOptionClick(User user)
    {
        if (user.preferences.receiveColonists)
        {
            Find.WindowStack.Add(new UserSendColonistWindow(user));
            return;
        }

        Messages.Message($"{user.name} does not accept colonists", MessageTypeDefOf.RejectInput);
    }

    public void OnSendAnimalOptionClick(User user)
    {
        if (user.preferences.receiveAnimals)
        {
            Find.WindowStack.Add(new UserSendAnimalWindow(user));
            return;
        }

        Messages.Message($"{user.name} does not accept animals", MessageTypeDefOf.RejectInput);
    }

    public void OnShipItemsOptionClick(User user)
    {
        var unused = PhiClient.instance;
        if (user.preferences.receiveItems)
        {
            Find.WindowStack.Add(new UserGiveWindow(user));
            return;
        }

        Messages.Message($"{user.name} does not accept items", MessageTypeDefOf.RejectInput);
    }
}