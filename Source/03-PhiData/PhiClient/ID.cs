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
            var e = TryFind(list, id);
            if (e == null)
            {
                throw new Exception(string.Concat("Entity with ID ", id, " not found in ", list.ToString()));
            }

            return e;
        }

        // Token: 0x06000014 RID: 20 RVA: 0x000022A8 File Offset: 0x000004A8
        public static E TryFind<E>(List<E> list, int id) where E : IDable
        {
            foreach (var result in list)
            {
                if (result.getID() == id)
                {
                    return result;
                }
            }

            return default;
        }
    }
}