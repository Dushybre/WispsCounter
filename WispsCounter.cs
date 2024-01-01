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
            var gameUi = GameController.Game.IngameState.IngameUi;

            if (GameController.Area.CurrentArea == null || (GameController.Area.CurrentArea.IsHideout && Settings.DisableInHideout) || gameUi.InventoryPanel.IsVisible || gameUi.SyndicatePanel.IsVisibleLocal)
            {
                CanRender = false;
                return;
            }

            var UIHover = GameController.Game.IngameState.UIHover;

            if (UIHover.Tooltip != null && UIHover.Tooltip.IsVisibleLocal &&
                UIHover.Tooltip.GetClientRectCache.Intersects(leftPanelStartDrawRect))
            {
                CanRender = false;
                return;
            }

            CanRender = true;
			
			purpleWisps = $"{GameController.IngameState.Data.ServerData.CurrentWildWisps} Wild";
			yellowWisps = $"{GameController.IngameState.Data.ServerData.CurrentVividWisps} Vivid";
			blueWisps = $"{GameController.IngameState.Data.ServerData.CurrentPrimalWisps} Primal";
            totalWisps = $"{GameController.IngameState.Data.ServerData.CurrentWildWisps + GameController.IngameState.Data.ServerData.CurrentVividWisps + GameController.IngameState.Data.ServerData.CurrentPrimalWisps} Wisps";
        }

        public override void Render()
        {
            if (!CanRender)
                return;
            var origStartPoint = GameController.LeftPanel.StartDrawPoint;

            var rightHalfDrawPoint = origStartPoint.Translate(Settings.DrawXOffset.Value - GameController.IngameState.IngameUi.MapSideUI.Width);
            leftPanelStartDrawRect = new RectangleF(rightHalfDrawPoint.X, rightHalfDrawPoint.Y, 1, 1);

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
			
            var rightTextBounds = wispsItems.Select(x => Graphics.MeasureText(x.Item1)).ToList()
                switch { var s => new Vector2N(s.DefaultIfEmpty(Vector2N.Zero).Max(x => x.X), s.Sum(x => x.Y)) };
            var leftTextBounds = wispsItems.Select(x => Graphics.MeasureText(x.Item1)).ToList()
                switch { var s => new Vector2N(s.DefaultIfEmpty(Vector2N.Zero).Max(x => x.X), s.Sum(x => x.Y)) };

            var sumX = rightTextBounds.X + leftTextBounds.X + 5;
            var maxY = Math.Max(rightTextBounds.Y, leftTextBounds.Y);
            var leftHalfDrawPoint = rightHalfDrawPoint with { X = rightHalfDrawPoint.X - sumX };
            startY = leftHalfDrawPoint.Y;
            var bounds = new RectangleF(leftHalfDrawPoint.X, startY - 2, sumX, maxY);
            Graphics.DrawImage("preload-new.png", bounds, Settings.BackgroundColor);

            foreach (var (text, color) in wispsItems)
            {
                drawTextVector2 = Graphics.DrawText(text, leftHalfDrawPoint, color);
                leftHalfDrawPoint.Y += drawTextVector2.Y;
            }

            GameController.LeftPanel.StartDrawPoint = new Vector2(origStartPoint.X, origStartPoint.Y + maxY + 10);
        }
    }
}
