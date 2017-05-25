namespace MyerList.Common.Brush
{
    public class AcrylicBrush : AcrylicBrushBase
    {
        public AcrylicBrush()
        {

        }

        protected override BackdropBrushType GetBrushType()
        {
            return BackdropBrushType.Backdrop;
        }
    }
}
