﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Game.ClientState.Objects.Enums;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using ImGuiNET;
using MiniMappingway.Manager;
using MiniMappingway.Model;
using MiniMappingway.Utility;

namespace MiniMappingway.Window
{
    internal class NaviMapWindow : Dalamud.Interface.Windowing.Window
    {
        


        public NaviMapWindow() : base("NaviMapWindow")
        {
            Size = new Vector2(200, 200);
            Position = new Vector2(200, 200);

            Flags |= ImGuiWindowFlags.NoInputs | ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoBackground
                | ImGuiWindowFlags.NoBringToFrontOnFocus | ImGuiWindowFlags.NoFocusOnAppearing | ImGuiWindowFlags.NoNavFocus;

            ForceMainWindow = true;
            IsOpen = true;
        }

        public override void Draw()
        {
            var drawList = ImGui.GetWindowDrawList();

            while (ServiceManager.NaviMapManager.CircleData.Any())
            {
                ServiceManager.NaviMapManager.CircleData.TryDequeue(out var circle);
                if (circle == null)
                {
                    continue;
                }
                drawList.AddCircleFilled(circle.Position, ServiceManager.Configuration.CircleSize, ServiceManager.NaviMapManager.SourceDataDict[circle.SourceName]);
            }
            if (ServiceManager.NaviMapManager.DebugMode)
            {
                ImGui.Text($"zoom {ServiceManager.NaviMapManager.Zoom}");
                ImGui.Text($"naviScale {ServiceManager.NaviMapManager.NaviScale}");
                ImGui.Text($"zoneScale {ServiceManager.NaviMapManager.ZoneScale}");
                ImGui.Text($"offsetX {ServiceManager.NaviMapManager.OffsetX}");
                ImGui.Text($"offsetY {ServiceManager.NaviMapManager.OffsetY}");
                ImGui.Text($"x {ServiceManager.NaviMapManager.X}");
                ImGui.Text($"y {ServiceManager.NaviMapManager.Y}");
                ImGui.Text($"islocked {ServiceManager.NaviMapManager.IsLocked}");
                ImGui.Text($"showfc {ServiceManager.Configuration.ShowFcMembers}");
                ImGui.Text($"showfriend {ServiceManager.Configuration.ShowFriends}");
                ImGui.Text($"circleCount {ServiceManager.NaviMapManager.CircleData.Count}");
                ImGui.Text($"personDict {ServiceManager.NaviMapManager.PersonDict.Count}");
            }
        }

        public override bool DrawConditions()
        {
            if (!MarkerUtility.RunChecks())
            {
                return false;
            }
            return true;
        }

        public override void PreDraw()
        {
            MarkerUtility.PrepareDrawOnMinimap();

        }



        public void Dispose()
        {
        }





        

        
    }
}
