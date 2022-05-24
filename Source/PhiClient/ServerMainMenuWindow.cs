using PhiClient.UI;
using UnityEngine;
using Verse;

namespace PhiClient;

internal class ServerMainMenuWindow : Window
{
    private string enteredAddress = "";

    private Vector2 scrollPosition = Vector2.zero;

    private string wantedNickname;

    public ServerMainMenuWindow()
    {
        doCloseX = true;
        closeOnAccept = false;
    }

    public override Vector2 InitialSize => new Vector2(700f, 700f);

    public override void PreOpen()
    {
        base.PreOpen();
        var instance = PhiClient.instance;
        enteredAddress = instance.serverAddress;
        if (instance.IsUsable())
        {
            OnUsableCallback();
        }

        instance.OnUsable += OnUsableCallback;
    }

    public override void PostClose()
    {
        base.PostClose();
        PhiClient.instance.OnUsable -= OnUsableCallback;
    }

    private void OnUsableCallback()
    {
        wantedNickname = PhiClient.instance.currentUser.name;
    }

    public override void DoWindowContents(Rect inRect)
    {
        var instance = PhiClient.instance;
        var listContainer = new ListContainer { spaceBetween = 10f };
        listContainer.Add(new HeightContainer(DoHeader(), 30f));
        if (instance.IsUsable())
        {
            listContainer.Add(DoConnectedContent());
        }

        listContainer.Draw(inRect);
    }

    public Displayable DoHeader()
    {
        var instance = PhiClient.instance;
        var listContainer = new ListContainer(ListFlow.ROW) { spaceBetween = 10f };
        if (instance.IsUsable())
        {
            listContainer.Add(new TextWidget($"Connected to {instance.serverAddress}", GameFont.Small,
                TextAnchor.MiddleLeft));
            listContainer.Add(
                new WidthContainer(new ButtonWidget("Disconnect", OnDisconnectButtonClick), 140f));
        }
        else
        {
            listContainer.Add(new TextFieldWidget(enteredAddress, delegate(string s) { enteredAddress = s; }));
            listContainer.Add(new WidthContainer(new ButtonWidget("Connect", OnConnectButtonClick),
                140f));
        }

        return listContainer;
    }

    public Displayable DoConnectedContent()
    {
        var client = PhiClient.instance;
        var listContainer = new ListContainer { spaceBetween = 10f };
        var listContainer2 = new ListContainer(ListFlow.ROW) { spaceBetween = 10f };
        listContainer.Add(new HeightContainer(listContainer2, 30f));
        listContainer2.Add(new TextFieldWidget(wantedNickname,
            delegate(string s) { wantedNickname = OnWantedNicknameChange(s); }));
        listContainer2.Add(new WidthContainer(new ButtonWidget("Change nickname", OnChangeNicknameClick), 140f));
        var pref = client.currentUser.preferences;
        var listContainer3 = new ListContainer(ListFlow.ROW) { spaceBetween = 10f };
        listContainer.Add(listContainer3);
        var listContainer4 = new ListContainer();
        listContainer3.Add(listContainer4);
        listContainer4.Add(new CheckboxLabeledWidget("Allow receiving items", pref.receiveItems, delegate(bool b)
        {
            pref.receiveItems = b;
            client.UpdatePreferences();
        }));
        listContainer4.Add(new CheckboxLabeledWidget("Allow receiving colonists (EXPERIMENTAL)",
            pref.receiveColonists, delegate(bool b)
            {
                pref.receiveColonists = b;
                client.UpdatePreferences();
            }));
        listContainer4.Add(new CheckboxLabeledWidget("Allow receiving animals (EXPERIMENTAL)", pref.receiveAnimals,
            delegate(bool b)
            {
                pref.receiveAnimals = b;
                client.UpdatePreferences();
            }));
        var display = new ListContainer();
        listContainer3.Add(display);
        return listContainer;
    }

    public string OnWantedNicknameChange(string newNickname)
    {
        return newNickname;
    }

    public void OnConnectButtonClick()
    {
        var instance = PhiClient.instance;
        instance.SetServerAddress(enteredAddress.Trim());
        instance.TryConnect();
    }

    public void OnDisconnectButtonClick()
    {
        PhiClient.instance.Disconnect();
    }

    private void OnChangeNicknameClick()
    {
        PhiClient.instance.ChangeNickname(wantedNickname);
    }
}