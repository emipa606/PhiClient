using System;
using System.Collections.Generic;

namespace PhiClient
{
	// Token: 0x0200000C RID: 12
	public class ID
	{
		// Token: 0x06000013 RID: 19 RVA: 0x00002254 File Offset: 0x00000454
		public static E Find<E>(List<E> list, int id) where E : IDable
		{
			E e = ID.TryFind<E>(list, id);
			if (e == null)
			{
				throw new Exception(string.Concat(new object[]
				{
					"Entity with ID ",
					id,
					" not found in ",
					list.ToString()
				}));
			}
			return e;
		}

		// Token: 0x06000014 RID: 20 RVA: 0x000022A8 File Offset: 0x000004A8
		public static E TryFind<E>(List<E> list, int id) where E : IDable
		{
			foreach (E result in list)
			{
				if (result.getID() == id)
				{
					return result;
				}
			}
			return default(E);
		}
	}
}
