using System;

namespace PhiClient
{
    // Token: 0x02000014 RID: 20
    [Serializable]
    public class RealmHediff
    {
        // Token: 0x04000030 RID: 48
        public int ageTicks;

        // Token: 0x0400002D RID: 45
        public int bodyPartIndex;

        // Token: 0x0400002C RID: 44
        public string hediffDefName;

        // Token: 0x0400002E RID: 46
        public float immunity = float.NaN;

        // Token: 0x04000031 RID: 49
        public float severity;

        // Token: 0x0400002F RID: 47
        public string sourceDefName;
    }
}