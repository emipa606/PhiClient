using System;
using System.Runtime.Serialization;

namespace PhiClient
{
	// Token: 0x02000009 RID: 9
	[Serializable]
	public class ChatMessage
	{
		// Token: 0x0600000C RID: 12 RVA: 0x000021A3 File Offset: 0x000003A3
		[OnSerializing]
		internal void OnSerializingCallback(StreamingContext c)
		{
			this.userId = this.user.id;
		}

		// Token: 0x0600000D RID: 13 RVA: 0x000021B8 File Offset: 0x000003B8
		[OnDeserialized]
		internal void OnDeserializedCallback(StreamingContext c)
		{
			RealmContext realmContext = (RealmContext)c.Context;
			if (realmContext.realmData != null)
			{
				this.user = ID.Find<User>(realmContext.realmData.users, this.userId);
			}
		}

		// Token: 0x04000013 RID: 19
		public User user;

		// Token: 0x04000014 RID: 20
		public int userId;

		// Token: 0x04000015 RID: 21
		public string message;
	}
}
