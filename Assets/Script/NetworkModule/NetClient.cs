using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

namespace Network
{
    public class NetClient
    {
        private const int DATA_BUFFER_SIZE = 4096;
        private const int PING_INTERVAL = 2;
        private const int CONNECTION_TIMEOUT = 10;
        private DateTime lastPingTime = DateTime.Now;
        public long Ping { get; private set; }
        public Guid UID { get; private set; }
        private TcpClient tcpClient;
        private byte[] receiveBuffer;

        public bool IsAlive {
            get
            {
                return
                    tcpClient != null &&
                    tcpClient.Connected &&
                    tcpClient.GetStream() != null;
            }
        }
        public NetClient()
        {
            UID = Guid.NewGuid();
            this.tcpClient = new TcpClient();
            this.receiveBuffer = new byte[DATA_BUFFER_SIZE];
            this.tcpClient.ReceiveBufferSize = DATA_BUFFER_SIZE;
            this.tcpClient.SendBufferSize = DATA_BUFFER_SIZE;
        }

        public async void Send(Packet packet)
        {
            if (!IsAlive)
                return;

            if (packet == null)
                return;

            packet.WriteLength();
            try
            {
                await tcpClient.GetStream().WriteAsync(packet.ToBytes(), 0, packet.ToBytes().Length);
            }
            catch (Exception e)
            {
                Debug.LogError($"SendMsgAsync: {e}");
            }
        }

        public async Task Read()
        {
            if (!IsAlive || tcpClient.Available == 0)
                return;
            try
            {
                int byteLength = tcpClient.GetStream().Read(receiveBuffer, 0, DATA_BUFFER_SIZE);
                if (byteLength <= 0)
                    return;

                byte[] receiveData = new byte[byteLength];
                Array.Copy(receiveBuffer, receiveData, byteLength);

                //Clear After Copy from the buffer
                Array.Clear(receiveBuffer, 0, DATA_BUFFER_SIZE);

                Packet packet = new Packet(receiveData);
                //Get Packet Lenght
                int packetLength = packet.ReadInt();
                //Get Packet ID
                string packet_id = packet.ReadString();
                //Retrieve Packet Handler
                PacketHandlerBase packetHandler = null;
                if (!NetworkManager.singleton.packetHandlers.TryGetValue(packet_id, out packetHandler))
                {
                    Debug.LogError($"Invaild Packet ID[{packet_id}]");
                    return;
                }

                UpdatePing(null, null);
                await packetHandler.ReadPacket(this, packet);
            }
            catch (Exception e)
            {
                Debug.LogError($"ReadMsg: {e}");
            }
        }

        public async Task<bool> TryConnect(string ip, int port)
        {
            if (IsAlive)
                return true;

            try
            {
                Task connectionTask = tcpClient.ConnectAsync(ip, port);
                Task timeoutTask = Task.Delay((int)TimeSpan.FromSeconds(CONNECTION_TIMEOUT).TotalMilliseconds);
                if (connectionTask == await Task.WhenAny(connectionTask, timeoutTask))
                {
                    await connectionTask;
                    return true;
                }

                throw new Exception("Time out");
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Establish connection failed: {e}");
                return false;
            }
        }
        
        public void Disconnect()
        {
            if (!IsAlive)
                return;

            if (tcpClient != null)
            {
                tcpClient.GetStream().Close();
                tcpClient.Close();
                tcpClient = null;
            }
            Debug.Log($"NetClient[{UID.ToString()}] Disconnected");
        }

        public void PreformHeartbeat()
        {
            //Debug.Log($"{TimerManager.GetCurrentUnixTimeStampTimestamp() - lastRequestTime}");
            if (!IsAlive || DateTime.Now.Subtract(lastPingTime).TotalSeconds <= PING_INTERVAL)
                return;

            Packet packet = new Packet("Heartbeat");
            Send(packet);
            lastPingTime = DateTime.Now;
        }

        public void UpdatePing(NetClient netClient, Packet packet)
        {
            DateTime preResponseTime = lastPingTime;
            lastPingTime = DateTime.Now;
            Ping = (int)lastPingTime.Subtract(preResponseTime).TotalMilliseconds;
            if (Ping > 9999)
                Ping = 9999;
            else if (Ping < 0)
                Ping = 0;

            Debug.Log($"Ping: {Ping}");
        }
    }
}