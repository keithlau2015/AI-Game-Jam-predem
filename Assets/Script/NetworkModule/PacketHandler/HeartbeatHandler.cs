using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Network
{
    public class HeartbeatHandler : PacketHandlerBase
    {
        public override async Task ReadPacket(NetClient netClient, Packet packet)
        {
            if (netClient == null || packet == null)
            {
                Debug.LogError($"Params Null[NetClient => {netClient == null}, packet => {packet == null}]");
                return;
            }
            if (!netClient.IsAlive)
            {
                Debug.LogError($"NetClient not alive");
                return;
            }

            await Task.Delay(0);
        }
    }
}