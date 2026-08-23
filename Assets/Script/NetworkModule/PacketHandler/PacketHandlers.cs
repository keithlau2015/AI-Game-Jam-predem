using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Network
{
    public class PacketHandlers : PacketHandlerBase
    {
        protected List<PacketHandlerBase> handlers = new List<PacketHandlerBase>();
        public PacketHandlers(params PacketHandlerBase[] para):base()
        {
            handlers.AddRange(handlers);
        }

        public override async Task ReadPacket(NetClient netClient, Packet packet)
        {
            if(netClient == null || packet == null)
            {
                Debug.LogError($"Params Null[NetClient => {netClient == null}, packet => {packet == null}]");
                return;
            }
            if(!netClient.IsAlive)
            {
                Debug.LogError($"NetClient not alive");
                return;
            }
            if(packet.UnreadLength() == 0)
            {
                Debug.LogError($"Packet unreadLength is 0");
                return;
            }
            foreach (PacketHandlerBase handler in handlers)
            {
                await handler.ReadPacket(netClient, packet);
            }
        }
    }
}
