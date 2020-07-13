using System;
using PhiClient.UI;
using UnityEngine;
using Verse;

namespace PhiClient
{
	// Token: 0x02000004 RID: 4
	internal class ServerMainMenuWindow : Window
	{
		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000021 RID: 33 RVA: 0x00002841 File Offset: 0x00000A41
		public override Vector2 InitialSize
		{
			get
			{
				return new Vector2(700f, 700f);
			}
		}

		// Token: 0x06000022 RID: 34 RVA: 0x00002852 File Offset: 0x00000A52
		public ServerMainMenuWindow()
		{
			this.doCloseX = true;
			this.closeOnAccept = false;
		}

		// Token: 0x06000023 RID: 35 RVA: 0x00002880 File Offset: 0x00000A80
		public override void PreOpen()
		{
			base.PreOpen();
			PhiClient instance = PhiClient.instance;
			this.enteredAddress = instance.serverAddress;
			if (instance.IsUsable())
			{
				this.OnUsableCallback();
			}
			instance.OnUsable += this.OnUsableCallback;
		}

		// Token: 0x06000024 RID: 36 RVA: 0x000028C5 File Offset: 0x00000AC5
		public override void PostClose()
		{
			base.PostClose();
			PhiClient.instance.OnUsable -= this.OnUsableCallback;
		}

		// Token: 0x06000025 RID: 37 RVA: 0x000028E3 File Offset: 0x00000AE3
		private void OnUsableCallback()
		{
			this.wantedNickname = PhiClient.instance.currentUser.name;
		}

		// Token: 0x06000026 RID: 38 RVA: 0x000028FC File Offset: 0x00000AFC
		public override void DoWindowContents(Rect inRect)
		{
			PhiClient instance = PhiClient.instance;
			ListContainer listContainer = new ListContainer();
			listContainer.spaceBetween = 10f;
			listContainer.Add(new HeightContainer(this.DoHeader(), 30f));
			if (instance.IsUsable())
			{
				listContainer.Add(this.DoConnectedContent());
			}
			listContainer.Draw(inRect);
		}

		// Token: 0x06000027 RID: 39 RVA: 0x00002950 File Offset: 0x00000B50
		public Displayable DoHeader()
		{
			PhiClient instance = PhiClient.instance;
			ListContainer listContainer = new ListContainer(ListFlow.ROW, ListDirection.NORMAL);
			listContainer.spaceBetween = 10f;
			if (instance.IsUsable())
			{
				listContainer.Add(new TextWidget("Connected to " + instance.serverAddress, GameFont.Small, TextAnchor.MiddleLeft));
				listContainer.Add(new WidthContainer(new ButtonWidget("Disconnect", delegate()
				{
					this.OnDisconnectButtonClick();
				}, true), 140f));
			}
			else
			{
				listContainer.Add(new TextFieldWidget(this.enteredAddress, delegate(string s)
				{
					this.enteredAddress = s;
				}));
				listContainer.Add(new WidthContainer(new ButtonWidget("Connect", delegate()
				{
					this.OnConnectButtonClick();
				}, true), 140f));
			}
			return listContainer;
		}

		// Token: 0x06000028 RID: 40 RVA: 0x00002A0C File Offset: 0x00000C0C
		public Displayable DoConnectedContent()
		{
			PhiClient client = PhiClient.instance;
			ListContainer listContainer = new ListContainer();
			listContainer.spaceBetween = 10f;
			ListContainer listContainer2 = new ListContainer(ListFlow.ROW, ListDirection.NORMAL);
			listContainer2.spaceBetween = 10f;
			listContainer.Add(new HeightContainer(listContainer2, 30f));
			listContainer2.Add(new TextFieldWidget(this.wantedNickname, delegate(string s)
			{
				this.wantedNickname = this.OnWantedNicknameChange(s);
			}));
			listContainer2.Add(new WidthContainer(new ButtonWidget("Change nickname", new Action(this.OnChangeNicknameClick), true), 140f));
			UserPreferences pref = client.currentUser.preferences;
			ListContainer listContainer3 = new ListContainer(ListFlow.ROW, ListDirection.NORMAL);
			listContainer3.spaceBetween = 10f;
			listContainer.Add(listContainer3);
			ListContainer listContainer4 = new ListContainer();
			listContainer3.Add(listContainer4);
			listContainer4.Add(new CheckboxLabeledWidget("Allow receiving items", pref.receiveItems, delegate(bool b)
			{
				pref.receiveItems = b;
				client.UpdatePreferences();
			}));
			listContainer4.Add(new CheckboxLabeledWidget("Allow receiving colonists (EXPERIMENTAL)", pref.receiveColonists, delegate(bool b)
			{
				pref.receiveColonists = b;
				client.UpdatePreferences();
			}));
			listContainer4.Add(new CheckboxLabeledWidget("Allow receiving animals (EXPERIMENTAL)", pref.receiveAnimals, delegate(bool b)
			{
				pref.receiveAnimals = b;
				client.UpdatePreferences();
			}));
			ListContainer display = new ListContainer();
			listContainer3.Add(display);
			return listContainer;
		}

		// Token: 0x06000029 RID: 41 RVA: 0x00002B6A File Offset: 0x00000D6A
		public string OnWantedNicknameChange(string newNickname)
		{
			return newNickname;
		}

		// Token: 0x0600002A RID: 42 RVA: 0x00002B6D File Offset: 0x00000D6D
		public void OnConnectButtonClick()
		{
			PhiClient instance = PhiClient.instance;
			instance.SetServerAddress(this.enteredAddress.Trim());
			instance.TryConnect();
		}

		// Token: 0x0600002B RID: 43 RVA: 0x00002B8A File Offset: 0x00000D8A
		public void OnDisconnectButtonClick()
		{
			PhiClient.instance.Disconnect();
		}

		// Token: 0x0600002C RID: 44 RVA: 0x00002B96 File Offset: 0x00000D96
		private void OnChangeNicknameClick()
		{
			PhiClient.instance.ChangeNickname(this.wantedNickname);
		}

		// Token: 0x0400000D RID: 13
		private Vector2 scrollPosition = Vector2.zero;

		// Token: 0x0400000E RID: 14
		private string enteredAddress = "";

		// Token: 0x0400000F RID: 15
		private string wantedNickname;
	}
}
