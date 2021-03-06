﻿using System;
using System.Collections.Generic;
using System.Linq;
using PhiClient.UI;
using RimWorld;
using UnityEngine;
using Verse;
using WebSocketSharp;

namespace PhiClient
{
    // Token: 0x02000005 RID: 5
    public class ServerMainTab : MainTabWindow
    {
        // Token: 0x04000010 RID: 16
        private const float TITLE_HEIGHT = 45f;

        // Token: 0x04000011 RID: 17
        private const float CHAT_INPUT_HEIGHT = 30f;

        // Token: 0x04000012 RID: 18
        private const float CHAT_INPUT_SEND_BUTTON_WIDTH = 100f;

        // Token: 0x04000013 RID: 19
        private const float CHAT_MARGIN = 10f;

        // Token: 0x04000014 RID: 20
        private const float STATUS_AREA_WIDTH = 160f;

        // Token: 0x04000017 RID: 23
        private Vector2 chatScroll = Vector2.zero;

        // Token: 0x04000015 RID: 21
        private string enteredMessage = "";

        // Token: 0x04000016 RID: 22
        private string filterName = "";

        // Token: 0x04000018 RID: 24
        private Vector2 userScrollPosition = Vector2.zero;

        // Token: 0x06000030 RID: 48 RVA: 0x00002BC4 File Offset: 0x00000DC4
        public ServerMainTab()
        {
            doCloseX = true;
            closeOnAccept = false;
            closeOnClickedOutside = false;
            forceCatchAcceptAndCancelEventEvenIfUnfocused = true;
        }

        // Token: 0x06000031 RID: 49 RVA: 0x00002C20 File Offset: 0x00000E20
        public override void DoWindowContents(Rect inRect)
        {
            var unused = PhiClient.instance;
            var listContainer = new ListContainer {spaceBetween = 10f};
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

        // Token: 0x06000032 RID: 50 RVA: 0x00002CBC File Offset: 0x00000EBC
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
                listContainer.Add(new ButtonWidget(phi.realmData.users[idx].name + ": " + chatMessage.message,
                    delegate { OnUserClick(phi.realmData.users[idx]); }, false));
            }

            return new ScrollContainer(listContainer, chatScroll, delegate(Vector2 v) { chatScroll = v; });
        }

        // Token: 0x06000033 RID: 51 RVA: 0x00002DF0 File Offset: 0x00000FF0
        private Displayable DoBodyRightBar()
        {
            var instance = PhiClient.instance;
            var listContainer = new ListContainer {spaceBetween = 10f};
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

        // Token: 0x06000034 RID: 52 RVA: 0x00003048 File Offset: 0x00001248
        private void OnConfigurationClick()
        {
            Find.WindowStack.Add(new ServerMainMenuWindow());
        }

        // Token: 0x06000035 RID: 53 RVA: 0x00003059 File Offset: 0x00001259
        private bool ContainsStringIgnoreCase(string hay, string needle)
        {
            return hay.IndexOf(needle, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        // Token: 0x06000036 RID: 54 RVA: 0x0000306C File Offset: 0x0000126C
        private Displayable DoFooter()
        {
            var listContainer = new ListContainer(ListFlow.ROW) {spaceBetween = 10f};
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

        // Token: 0x06000037 RID: 55 RVA: 0x0000310B File Offset: 0x0000130B
        public override void OnAcceptKeyPressed()
        {
            OnSendClick();
        }

        // Token: 0x06000038 RID: 56 RVA: 0x00003113 File Offset: 0x00001313
        public void OnSendClick()
        {
            if (enteredMessage.Trim().IsNullOrEmpty())
            {
                return;
            }

            PhiClient.instance.SendMessage(enteredMessage);
            enteredMessage = "";
        }

        // Token: 0x06000039 RID: 57 RVA: 0x00002B6A File Offset: 0x00000D6A
        public string OnEnteredMessageChange(string newMessage)
        {
            return newMessage;
        }

        // Token: 0x0600003A RID: 58 RVA: 0x00003142 File Offset: 0x00001342
        public void OnReconnectClick()
        {
            PhiClient.instance.TryConnect();
        }

        // Token: 0x0600003B RID: 59 RVA: 0x00003150 File Offset: 0x00001350
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

        // Token: 0x0600003C RID: 60 RVA: 0x0000320D File Offset: 0x0000140D
        public void OnSendColonistOptionClick(User user)
        {
            if (user.preferences.receiveColonists)
            {
                Find.WindowStack.Add(new UserSendColonistWindow(user));
                return;
            }

            Messages.Message(user.name + " does not accept colonists", MessageTypeDefOf.RejectInput);
        }

        // Token: 0x0600003D RID: 61 RVA: 0x00003248 File Offset: 0x00001448
        public void OnSendAnimalOptionClick(User user)
        {
            if (user.preferences.receiveAnimals)
            {
                Find.WindowStack.Add(new UserSendAnimalWindow(user));
                return;
            }

            Messages.Message(user.name + " does not accept animals", MessageTypeDefOf.RejectInput);
        }

        // Token: 0x0600003E RID: 62 RVA: 0x00003284 File Offset: 0x00001484
        public void OnShipItemsOptionClick(User user)
        {
            var unused = PhiClient.instance;
            if (user.preferences.receiveItems)
            {
                Find.WindowStack.Add(new UserGiveWindow(user));
                return;
            }

            Messages.Message(user.name + " does not accept items", MessageTypeDefOf.RejectInput);
        }
    }
}