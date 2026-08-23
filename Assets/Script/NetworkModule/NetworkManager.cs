using Network;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : Singleton<NetworkManager>
{
    [SerializeField]
    private string serverIp = "127.0.0.1";

    [SerializeField]
    private int serverPort = 45000;

    [SerializeField]
    private bool connectOnStart = false;

    private NetClient _netClient;
    public NetClient netClient
    {
        get
        {
            if (_netClient == null)
                _netClient = new NetClient();
            return _netClient;
        }
    }

    public Dictionary<string, PacketHandlerBase> packetHandlers = new Dictionary<string, PacketHandlerBase>();
    public Queue<Packet> pendingRequestPacket = new Queue<Packet>();
    public ServerStatus CurServerStatus { get; private set; }

    protected async void Start()
    {
        packetHandlers["ResponseHeartbeat"] = new HeartbeatHandler();
        packetHandlers[typeof(ServerStatus).ToString()] = new GenericPacketHandler<ServerStatus>(UpdateServerStatus);

        if (connectOnStart)
            await ConnectAsync();
    }

    public async Cysharp.Threading.Tasks.UniTask<bool> ConnectAsync(string ip = null, int? port = null)
    {
        string host = string.IsNullOrEmpty(ip) ? serverIp : ip;
        int p = port ?? serverPort;
        bool connected = await netClient.TryConnect(host, p);
        Debug.Log($"[Network] connected={connected} host={host}:{p}");
        return connected;
    }

    private async void Update()
    {
        if (!netClient.IsAlive)
            return;

        await netClient.Read();
        netClient.PreformHeartbeat();

        if (pendingRequestPacket == null || pendingRequestPacket.Count == 0)
            return;

        if (pendingRequestPacket.TryDequeue(out Packet packet) && packet != null)
            netClient.Send(packet);
    }

    protected override void OnDestroy()
    {
        netClient.Disconnect();
        base.OnDestroy();
    }

    public void Request(Packet packet)
    {
        if (packet == null || pendingRequestPacket == null)
            return;

        pendingRequestPacket.Enqueue(packet);
    }

    private void UpdateServerStatus(NetClient client, ServerStatus newServerStatus)
    {
        CurServerStatus = newServerStatus;
    }
}
