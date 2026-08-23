using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Network
{
    public class GenericPacketHandler<T> : PacketHandlerBase
    {
        private Action<NetClient, T> cb;
        public GenericPacketHandler(Action<NetClient, T> cb)
        {
            this.cb = cb;
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
            T obj = (T)packet.ReadObject<T>();
            if (obj == null)
            {
                Debug.LogError($"obj is null");
                return;
            }
            await Task.Run(() => { this.cb?.Invoke(netClient, obj); });
        }
    }
}
