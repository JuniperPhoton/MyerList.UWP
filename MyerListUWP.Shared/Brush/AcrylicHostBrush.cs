namespace MyerList.Common.Brush
{
    public class AcrylicHostBrush : AcrylicBrushBase
    {
        public AcrylicHostBrush()
        {

        }

        protected override BackdropBrushType GetBrushType()
        {
            return BackdropBrushType.HostBackdrop;
        }
    }
}
