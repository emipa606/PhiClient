using System;
using System.Collections.Generic;

namespace PhiClient;

public class ID
{
    public static E Find<E>(List<E> list, int id) where E : IDable
    {
        var e = TryFind(list, id);
        if (e == null)
        {
            throw new Exception(string.Concat("Entity with ID ", id, " not found in ", list.ToString()));
        }

        return e;
    }

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