using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms; 
using ExileCore;
using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared;
using ExileCore.Shared.Cache;
using ExileCore.Shared.Enums;
using ExileCore.Shared.Helpers;
using SharpDX;
using Input = ExileCore.Input;
using Vector2N = System.Numerics.Vector2;

namespace  WispsCounter
{
    public class  WispsCounter : BaseSettingsPlugin<WispsCounterSettings>
    {
        private string areaName = "";
		
		private string purpleWisps = "";
		private string yellowWisps = "";
		private string blueWisps = "";
        private string totalWisps = "";

        private float startY;
        private bool CanRender;
        private int tickCounter = 0;
        private DebugInformation debugInformation;
        private Vector2N drawTextVector2;
        private RectangleF leftPanelStartDrawRect = RectangleF.Empty;

        public override void OnLoad()
        {
            Order = -45;
            Graphics.InitImage("preload-start.png");
            Graphics.InitImage("preload-end.png");
            Graphics.InitImage("preload-new.png");
        }

        public override bool Initialise()
        {
            GameController.LeftPanel.WantUse(() => Settings.Enable);

            debugInformation = new DebugInformation("Game FPS", "Collect game fps", false);
            return true;
        }

        public override Job Tick()
        {
            TickLogic();
            return null;
        }

        private void TickLogic()
        {
            if (tickCounter <= Settings.TickDelay) {
                tickCounter++;
                return;
            } else {
                tickCounter = 0;
                var gameUi = GameController.Game.IngameState.IngameUi;

                if (GameController.Area.CurrentArea == null || (GameController.Area.CurrentArea.IsHideout && Settings.DisableInHideout) || gameUi.InventoryPanel.IsVisible || gameUi.SyndicatePanel.IsVisibleLocal) {
                    CanRender = false;
                    return;
                }

                var UIHover = GameController.Game.IngameState.UIHover;

                if (UIHover.Tooltip != null && UIHover.Tooltip.IsVisibleLocal && UIHover.Tooltip.GetClientRectCache.Intersects(leftPanelStartDrawRect)) {
                    CanRender = false;
                    return;
                }

                CanRender = true;
                
                purpleWisps = $"{GameController.IngameState.Data.ServerData.CurrentWildWisps} Wild";
                yellowWisps = $"{GameController.IngameState.Data.ServerData.CurrentVividWisps} Vivid";
                blueWisps = $"{GameController.IngameState.Data.ServerData.CurrentPrimalWisps} Primal";
                totalWisps = $"{GameController.IngameState.Data.ServerData.CurrentWildWisps + GameController.IngameState.Data.ServerData.CurrentVividWisps + GameController.IngameState.Data.ServerData.CurrentPrimalWisps} Wisps";
            }
        }

        public override void Render()
{
    if (!CanRender)
        return;

    var wispsItems = new[] {
        (purpleWisps, Settings.PurpleWispsTextColor),
        (yellowWisps, Settings.YellowWispsTextColor),
        (blueWisps, Settings.BlueWispsTextColor)    
    };

    if(Settings.EnableTotalWisps) {
        wispsItems = new[] {
            (purpleWisps, Settings.PurpleWispsTextColor),
            (yellowWisps, Settings.YellowWispsTextColor),
            (blueWisps, Settings.BlueWispsTextColor),
            (totalWisps, Settings.TotalWispsTextColor)  
        };
    }
    
    var width = wispsItems.Max(x => Graphics.MeasureText(x.Item1).X);
    var height = wispsItems.Sum(x => Graphics.MeasureText(x.Item1).Y);

    var backgroundRectangleF = new RectangleF(Settings.PositionX - 20, Settings.PositionY, width + 40, height + 10);
    Graphics.DrawImage("preload-new.png", backgroundRectangleF, Settings.BackgroundColor);

    var currentHeight = 0f;
    foreach (var (text, color) in wispsItems)
    {
        var textSize = Graphics.MeasureText(text);
        var drawPosition = new Vector2N(Settings.PositionX + width - textSize.X + 15, Settings.PositionY + currentHeight + 5);
        Graphics.DrawText(text, drawPosition, color);
        currentHeight += textSize.Y;
    }
}

    }
}
