using System;

namespace PhiClient.TransactionSystem
{
	// Token: 0x02000028 RID: 40
	[Serializable]
	public class StartTransactionPacket : Packet
	{
		// Token: 0x0600006C RID: 108 RVA: 0x00004590 File Offset: 0x00002790
		public override void Apply(User user, RealmData realmData)
		{
			realmData.transactions.Add(this.transaction);
			user.lastTransactionId = this.transaction.id;
			user.lastTransactionTime = DateTime.Now;
			realmData.NotifyPacket(this.transaction.receiver, new ReceiveTransactionPacket
			{
				transaction = this.transaction
			});
		}

		// Token: 0x04000085 RID: 133
		public Transaction transaction;
	}
}
