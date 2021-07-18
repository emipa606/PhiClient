using System;

namespace PhiClient
{
    // Token: 0x02000020 RID: 32
    [Serializable]
    public class User : IDable
    {
        // Token: 0x04000067 RID: 103
        public const int MIN_NAME_LENGTH = 4;

        // Token: 0x04000068 RID: 104
        public const int MAX_NAME_LENGTH = 32;

        // Token: 0x0400006B RID: 107
        public bool connected;

        // Token: 0x04000069 RID: 105
        public int id;

        // Token: 0x0400006C RID: 108
        public bool inGame;

        // Token: 0x0400006E RID: 110
        public int lastTransactionId;

        // Token: 0x0400006F RID: 111
        public DateTime lastTransactionTime = DateTime.MinValue;

        // Token: 0x0400006A RID: 106
        public string name;

        // Token: 0x0400006D RID: 109
        public UserPreferences preferences = new UserPreferences();

        // Token: 0x06000053 RID: 83 RVA: 0x00003B57 File Offset: 0x00001D57
        public int getID()
        {
            return id;
        }
    }
}