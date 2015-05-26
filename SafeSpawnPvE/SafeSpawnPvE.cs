using System;
using UnityEngine;
using System.IO;
using System.Timers;
using System.Threading;
using System.Diagnostics;
using Rocket.API;
using System.Collections.Generic;
using Rocket.Unturned;
using Rocket.Unturned.Logging;
using Rocket.Unturned.Plugins;
using SDG;
using Rocket.Unturned.Player;

namespace SafeSpawnPvE
{
    public class PluginSSPE : RocketPlugin
    {
        public string Name
        {
            get { return "safespawnpve"; }
        }

        private void RocketServerEvents_OnPlayerConnected(RocketPlayer player)
        {
            new Thread(new ThreadStart(() =>
            {
                Thread.Sleep(3000); //Allow player API to setup before changing their position.
                float newDist, smallestDist = -1;
                Vector3 loc = new Vector3(); SDG.Node n = new SDG.Node();
                Vector3 from = new Vector3(player.Position.x, player.Position.y, player.Position.z);

                foreach (SDG.NodeLocation node in SDG.LevelNodes.Nodes)
                {
                    Vector3 to = new Vector3(node.Position.x, node.Position.y, node.Position.z);
                    newDist = Vector3.Distance(from, to);

                    if (smallestDist == -1 || newDist < smallestDist)
                    {
                        smallestDist = newDist; n = node;
                        loc.Set(node.Position.x, (node.Position.y + 3), node.Position.z); //Prevent being stuck in ground so + y by 3.
                    }
                }
                NodeLocation location = (NodeLocation)n; player.Teleport(loc, player.Rotation);
                RocketChat.Say(player, "You've been teleported to " + location.Name + " to prevent spawn deaths!");
            }))
            {
                IsBackground = true
            }.Start();
        }

        protected override void Load()
        {
            Rocket.Unturned.Events.RocketServerEvents.OnPlayerConnected += RocketServerEvents_OnPlayerConnected;
        }
    }
}
