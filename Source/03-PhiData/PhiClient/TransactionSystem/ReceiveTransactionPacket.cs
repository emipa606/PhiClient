using System;

namespace PhiClient.TransactionSystem
{
	// Token: 0x02000027 RID: 39
	[Serializable]
	public class ReceiveTransactionPacket : Packet
	{
		// Token: 0x0600006A RID: 106 RVA: 0x00004534 File Offset: 0x00002734
		public override void Apply(User user, RealmData realmData)
		{
			if (realmData.TryFindTransaction(this.transaction.id, this.transaction.sender.id) == null)
			{
				realmData.transactions.Add(this.transaction);
			}
			if (user == this.transaction.receiver)
			{
				this.transaction.OnStartReceiver(realmData);
			}
		}

		// Token: 0x04000084 RID: 132
		public Transaction transaction;
	}
}
