using UnityEngine;
using System.Collections;
using System;

public class NetworkingLobby : MonoBehaviour
{
    public enum MyNetType : int
    {
        server = 0,
        client = 1
    };

    [System.Serializable]
    public class NewNetworkPlayer
    {
        public string playerName;
        public NetworkPlayer netPlayer;
        public bool isEmptySlot = true;
    }

    public NewNetworkPlayer[] connectedPlayers;

    public string playerName = "";



    public MyNetType myType = MyNetType.client;

    public string gameName = "";
    public string gameComment = "";
    public int remotePort = 25000;
    public bool useNat = false;
    public Vector2 scrollPosition = Vector2.zero;
    private string myIP = "";
    private string myPort = "";


    void Awake()
    {
        MasterServer.ClearHostList();
        MasterServer.RequestHostList("TowerDefence");
    }

    // Use this for initialization
    void Start()
    {
        connectedPlayers = new NewNetworkPlayer[4];
        connectedPlayers[0] = new NewNetworkPlayer();
        connectedPlayers[1] = new NewNetworkPlayer();
        connectedPlayers[2] = new NewNetworkPlayer();
        connectedPlayers[3] = new NewNetworkPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            scrollPosition = new Vector2(0, scrollPosition.y + Input.GetAxis("Mouse ScrollWheel"));
        }
    }

    void OnGUI()
    {
        if (Network.peerType == NetworkPeerType.Disconnected)
        {
            MasterServer.RequestHostList("TowerDefence");
            HostData[] hostData = MasterServer.PollHostList();
            GUI.Box(new Rect(10, 195, Screen.width - 35, Screen.height - 210), "");

            GUI.BeginScrollView(new Rect(15, 200, Screen.width - 15, Screen.height - 210), scrollPosition, new Rect(0, 0, 0, Screen.height - 211));
            GUILayout.BeginHorizontal();

            GUILayout.Box("Game name:", GUILayout.Width(Screen.width * 0.234375f));
            //GUILayout.Space(5);
            //GUILayout.FlexibleSpace();
            GUILayout.Box("Connected players:", GUILayout.Width(Screen.width * 0.1171875f));
            GUILayout.Box("IP and Port:", GUILayout.Width(Screen.width * 0.234375f));
            GUILayout.Box("Commentary:", GUILayout.Width(Screen.width * 0.234375f));

            GUILayout.EndHorizontal();
            GUILayout.Space(15);
            GUI.EndScrollView();
            foreach (HostData element in hostData)
            {
                scrollPosition = GUI.BeginScrollView(new Rect(15, 200, Screen.width - 15, Screen.height - 210), scrollPosition, new Rect(0, 0, 0, Screen.height - 211));

                GUILayout.BeginHorizontal();
                var name = element.gameName;
                GUILayout.Box(name, GUILayout.Width(Screen.width * 0.234375f));

                name = element.connectedPlayers + " / " + element.playerLimit;
                GUILayout.Box(name, GUILayout.Width(Screen.width * 0.1171875f));


                string hostInfo;
                hostInfo = "";
                foreach (string host in element.ip)
                {
                    if (hostInfo != "")
                        hostInfo += ", ";
                    hostInfo = hostInfo + host + ":" + element.port;
                }
                GUILayout.Box(hostInfo, GUILayout.Width(Screen.width * 0.234375f));


                GUILayout.Box(element.comment, GUILayout.Width(Screen.width * 0.234375f));
                float minus = 0.0f;
                if (Screen.width < 1024)
                {
                    minus = 1024 - Screen.width;
                    minus *= 0.05208333333333333333333333333333f;
                }
                if (GUILayout.Button("Connect", GUILayout.Width(Screen.width * 0.1171875f - minus)))
                {
                    // Connect to HostData struct, internally the correct method is used (GUID when using NAT).
                    if (playerName.Length < 3)
                    {
                        playerName = "Player";
                    }
                    Network.Connect(element);
                }
                GUILayout.EndHorizontal();
                GUI.EndScrollView();
            }

            Vector2 currentSize = new Vector2(100, 20);

            currentSize = new Vector2(100, 40);
            Rect myRect = new Rect(Screen.width / 2 - currentSize.x / 2, 30, currentSize.x, currentSize.y);
            if (GUI.Button(myRect, "Start Lobby"))
            {
                // Создание Сервера
                myType = MyNetType.server;
                Network.InitializeServer(3, remotePort, useNat);
                if (gameName.Length < 5)
                    gameName = "Tower Deffence Game";
                MasterServer.RegisterHost("TowerDefence", gameName, gameComment);
            }

            currentSize = new Vector2(100, 22);
            myRect = new Rect(Screen.width / 2 - 300 / 2, 99, currentSize.x, currentSize.y);
            GUI.Box(myRect, "Game name:");

            currentSize = new Vector2(200, 23);
            myRect = new Rect(Screen.width / 2 - 100 / 2, 99, currentSize.x, currentSize.y);
            gameName = GUI.TextField(myRect, gameName);

            currentSize = new Vector2(125, 22);
            myRect = new Rect(Screen.width / 2 - 350 / 2, 126, currentSize.x, currentSize.y);
            GUI.Box(myRect, "Game Commentary:");

            currentSize = new Vector2(200, 23);
            myRect = new Rect(Screen.width / 2 - 100 / 2, 126, currentSize.x, currentSize.y);
            gameComment = GUI.TextField(myRect, gameComment);

            currentSize = new Vector2(125, 22);
            myRect = new Rect(Screen.width / 2 - 350 / 2, 170, currentSize.x, currentSize.y);
            GUI.Box(myRect, "Nickname:");

            currentSize = new Vector2(200, 23);
            myRect = new Rect(Screen.width / 2 - 100 / 2, 170, currentSize.x, currentSize.y);
            playerName = GUI.TextField(myRect, playerName);

        }
        if (Network.peerType != NetworkPeerType.Disconnected)
        {
            Vector2 currentSize = new Vector2(100, 40);
            Rect myRect = new Rect(Screen.width / 2 - currentSize.x / 2, 30, currentSize.x, currentSize.y);
            var disconnectText = "Stop Server";
            if (Network.peerType == NetworkPeerType.Client)
                disconnectText = "Disconnect";
            if (GUI.Button(myRect, disconnectText))
            {
                myType = MyNetType.client;
                Network.Disconnect();
                MasterServer.UnregisterHost();
            }

            currentSize = new Vector2(125, 22);
            myRect = new Rect(Screen.width / 2 - 350 / 2, 170, currentSize.x, currentSize.y);
            GUI.Box(myRect, "Your name: " + playerName);
            
        }
    }

    void OnPlayerConnected(NetworkPlayer player)
    {
        Debug.Log("Player " + " connected from " + player.ipAddress + ":" + player.port);
    }

    void OnConnectedToServer()
    {
        networkView.RPC("TakeSlot", RPCMode.Server, new object[] { Network.player, playerName });
    }

    void OnPlayerDisconnected(NetworkPlayer player)
    {
        Debug.Log("Clean up after player " + player);
        networkView.RPC("CleanUpSlot", RPCMode.Server, new object[] { player });
        Network.RemoveRPCs(player);
        Network.DestroyPlayerObjects(player);
    }

    void OnServerInitialized()
    {
        if (playerName.Length < 3)
        {
            playerName = "Server";
        }
        connectedPlayers[0].playerName = playerName;
        connectedPlayers[0].isEmptySlot = false;
        connectedPlayers[0].netPlayer = Network.player;
    }

    [RPC]
    void TakeSlot(NetworkPlayer player, string aNewNickName)
    {
        if (Network.isServer)
        {
            foreach (NewNetworkPlayer nnp in connectedPlayers)
            {
                if (nnp.isEmptySlot)
                {
                    nnp.isEmptySlot = false;
                    nnp.netPlayer = player;
                    nnp.playerName = aNewNickName;
                    break;
                }
            }
        }
    }
    [RPC]
    void CleanUpSlot(NetworkPlayer player)
    {
        foreach (NewNetworkPlayer nnp in connectedPlayers)
        {
            if (nnp.netPlayer == player)
            {
                nnp.isEmptySlot = true;
                nnp.netPlayer = new NetworkPlayer();
                nnp.playerName = "";
                break;
            }
        }
    }
}