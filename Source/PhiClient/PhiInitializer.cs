using HugsLib;

namespace PhiClient;

public class PhiInitializer : ModBase
{
    public PhiInitializer()
    {
        new PhiClient().TryConnect();
    }

    public override string ModIdentifier => "Phi";

    public override void Update()
    {
        PhiClient.instance.OnUpdate();
    }
}