using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;
using SharpDX;

namespace  WispsCounter
{
    public class WispsCounterSettings : ISettings
    {
        public  WispsCounterSettings()
        {
            BackgroundColor = new ColorBGRA(0, 0, 0, 255);
            
            PurpleWispsTextColor = new ColorBGRA(255, 0, 255, 255);
            YellowWispsTextColor = new ColorBGRA(255, 255, 0, 255);
            BlueWispsTextColor = new ColorBGRA(0, 208, 255, 255);

            TotalWispsTextColor = new ColorBGRA(255, 255, 255, 255);
        }

        public ToggleNode Enable { get; set; } = new ToggleNode(true);
        public RangeNode<int> DrawXOffset { get; set; } = new RangeNode<int>(0, -150, 150);
        public ColorNode BackgroundColor { get; set; }
        public ColorNode PurpleWispsTextColor { get; set; }
        public ColorNode YellowWispsTextColor { get; set; }
        public ColorNode BlueWispsTextColor { get; set; }
        public ColorNode TotalWispsTextColor { get; set; }
        public ToggleNode EnableTotalWisps { get; set; } = new ToggleNode(true);
        public ToggleNode DisableInHideout { get; set; } = new ToggleNode(true);
    }
}
