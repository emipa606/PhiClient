using System;

namespace WebSocketSharp.Server
{
	// Token: 0x0200004E RID: 78
	internal class WebSocketServiceHost<TBehavior> : WebSocketServiceHost where TBehavior : WebSocketBehavior
	{
		// Token: 0x06000585 RID: 1413 RVA: 0x0001ECD4 File Offset: 0x0001CED4
		internal WebSocketServiceHost(string path, Func<TBehavior> creator, Logger log) : this(path, creator, null, log)
		{
		}

		// Token: 0x06000586 RID: 1414 RVA: 0x0001ECE2 File Offset: 0x0001CEE2
		internal WebSocketServiceHost(string path, Func<TBehavior> creator, Action<TBehavior> initializer, Logger log) : base(path, log)
		{
			this._creator = this.createCreator(creator, initializer);
		}

		// Token: 0x1700019B RID: 411
		// (get) Token: 0x06000587 RID: 1415 RVA: 0x0001ED00 File Offset: 0x0001CF00
		public override Type BehaviorType
		{
			get
			{
				return typeof(TBehavior);
			}
		}

		// Token: 0x06000588 RID: 1416 RVA: 0x0001ED1C File Offset: 0x0001CF1C
		private Func<TBehavior> createCreator(Func<TBehavior> creator, Action<TBehavior> initializer)
		{
			bool flag = initializer == null;
			Func<TBehavior> result;
			if (flag)
			{
				result = creator;
			}
			else
			{
				result = delegate()
				{
					TBehavior tbehavior = creator();
					initializer(tbehavior);
					return tbehavior;
				};
			}
			return result;
		}

		// Token: 0x06000589 RID: 1417 RVA: 0x0001ED64 File Offset: 0x0001CF64
		protected override WebSocketBehavior CreateSession()
		{
			return this._creator();
		}

		// Token: 0x04000260 RID: 608
		private Func<TBehavior> _creator;
	}
}
