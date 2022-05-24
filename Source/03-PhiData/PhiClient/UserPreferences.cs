using System;

namespace PhiClient;

[Serializable]
public class UserPreferences
{
    public bool receiveAnimals;

    public bool receiveColonists;

    public bool receiveItems = true;
}