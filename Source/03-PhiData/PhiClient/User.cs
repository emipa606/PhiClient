using System;

namespace PhiClient;

[Serializable]
public class User : IDable
{
    public const int MIN_NAME_LENGTH = 4;

    public const int MAX_NAME_LENGTH = 32;

    public bool connected;

    public int id;

    public bool inGame;

    public int lastTransactionId;

    public DateTime lastTransactionTime = DateTime.MinValue;

    public string name;

    public UserPreferences preferences = new UserPreferences();

    public int getID()
    {
        return id;
    }
}