namespace PhiClient.UI;

internal class WidthContainer : Container
{
    public WidthContainer(Displayable child, float width) : base(child, width, -1f)
    {
    }

    public override bool IsFluidWidth()
    {
        return false;
    }
}