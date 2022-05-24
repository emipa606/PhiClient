namespace PhiClient.UI;

internal class HeightContainer : Container
{
    public HeightContainer(Displayable child, float height) : base(child, -1f, height)
    {
    }

    public override bool IsFluidHeight()
    {
        return false;
    }
}