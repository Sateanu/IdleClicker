﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urho;
using Urho.Gui;
using Urho.Resources;

namespace IdleClicker
{
    public static class Helpers
    {
        public static Vector2 GetNormalizedScreenPosition(Graphics graphics, IntVector2 position)
        {
            return new Vector2(position.X / (float)graphics.Width, position.Y / (float)graphics.Height);
        }

        public static Vector2 GetNormalizedScreenPosition(Graphics graphics, Vector2 position)
        {
            return new Vector2(position.X / graphics.Width, position.Y / graphics.Height);
        }

        public static Ray GetScreenRay(this Camera camera, Vector2 pos)
        {
            return camera.GetScreenRay(pos.X, pos.Y);
        }

        public static UIElement CreateBuildingCreationUIFromProperties(UI UI, XmlFile buildingStyleXml, BuildingProperties properties)
        {
            var buildingWindow = UI.LoadLayout(buildingStyleXml);

            var BuildingName = buildingWindow.GetChild("BuildingName", true) as Text;
            BuildingName.Value = properties.Name;

            var BuildingPrice = buildingWindow.GetChild("BuildingPrice", true) as Text;
            BuildingPrice.Value = properties.Cost.ToString();

            var BuildingType = buildingWindow.GetChild("BuildingType", true) as Text;
            BuildingType.Value = properties.ResourceType.ToString();

            return buildingWindow;
        }
    }
}
