﻿using Dalamud.Game.ClientState.Objects.Enums;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using MiniMappingway.Manager;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Types = Dalamud.Game.ClientState.Objects.Types;

namespace MiniMappingway.Service
{
    public sealed class FinderService : IDisposable
    {
        public List<Types.GameObject> friends = new List<Types.GameObject>();
        public List<Types.GameObject> fcMembers = new List<Types.GameObject>();

        public Vector2 playerPos = new Vector2();
        public bool inCombat = false;

        public unsafe void LookFor()
        {
            if (!ServiceManager.Configuration.showFcMembers && !ServiceManager.Configuration.showFriends)
            {
                return;
            }

            friends.Clear();
            fcMembers.Clear();



            byte* FC = null;
            if (ServiceManager.ObjectTable == null || ServiceManager.ObjectTable.Length <= 0)
            {
                return;
            }
            try
            {

                unsafe
                {
                    if(ServiceManager.ObjectTable[0] is null) {
                        return;
                    }
                    var player = (Character*)ServiceManager.ObjectTable[0].Address;
                    playerPos = new Vector2(player->GameObject.Position.X, player->GameObject.Position.Z);
                    Dalamud.Logging.PluginLog.Verbose($"player {player->GameObject.Position.X}, {player->GameObject.Position.Z}");


                    if (((StatusFlags)player->StatusFlags).HasFlag(StatusFlags.InCombat))
                    {
                        return;
                    }
                    FC = player->FreeCompanyTag;

                }


            }
            catch
            {

            }

            Parallel.For(1, ServiceManager.ObjectTable.Length, (i, state) =>
            {
                var obj = ServiceManager.ObjectTable[i];

                if (obj == null) { return; }
                unsafe
                {
                    var ptr = obj.Address;
                    var charPointer = (FFXIVClientStructs.FFXIV.Client.Game.Character.Character*)ptr;
                    if (charPointer->GameObject.ObjectKind != (byte)ObjectKind.Player)
                    {
                        return;
                    }
                    if (((StatusFlags)charPointer->StatusFlags).HasFlag(StatusFlags.AllianceMember))
                    {
                        return;
                    }

                    //iscasting currently means friend
                    if (ServiceManager.Configuration.showFriends)
                    {


                        if (((StatusFlags)charPointer->StatusFlags).HasFlag(StatusFlags.IsCasting))
                        {
                            lock (friends)
                            {
                                friends.Add(obj);

                            }
                        }
                    }

                    if (ServiceManager.Configuration.showFcMembers)
                    {
                        if (FC == null)
                        {
                            return;
                        }
                        var tempFc = new ReadOnlySpan<byte>(charPointer->FreeCompanyTag, 7);
                        var playerFC = new ReadOnlySpan<byte>(FC, 7);
                        ServiceManager.NaviMapManager.debugValue = (FC->CompareTo(0) == 0).ToString();
                        if (FC->CompareTo(0) == 0)
                        {
                            return;
                        }
                        if (playerFC.SequenceEqual(tempFc))
                        {
                            lock (fcMembers)
                            {
                                fcMembers.Add(obj);
                            }
                        }
                    }
                }
            });

        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

    }
}
