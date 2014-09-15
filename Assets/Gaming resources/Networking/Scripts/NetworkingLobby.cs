using UnityEngine;
using System.Collections;
using System;

public class NetworkingLobby : MonoBehaviour
{
    [System.Serializable]
    public class NewNetworkPlayer
    {
        public string playerName;
        public NetworkPlayer netPlayer;
        public bool isEmptySlot = true;
        public int money;
    }

    public NewNetworkPlayer[] connectedPlayers;

    public string playerName = "";


    public AudioClip connectedPlayerSong;
    public AudioClip disconnectedPlayerSong;
    public GameObject mainSongObject;
    public AudioClip mainSong;

    private bool showMasterServerIpConf = false;
    public string masterServerIP = "";

    public string gameName = "";
    public string gameComment = "";
    public int remotePort = 25000;
    public bool useNat = false;
    public Vector2 scrollPosition = Vector2.zero;
    private string myIP = "";
    private string myPort = "";

    public bool levelLoaded = false;

    void Awake()
    {
        DontDestroyOnLoad(this);
        networkView.group = 1;

        masterServerIP = "192.168.0.104";
        MasterServer.ipAddress = masterServerIP;

        MasterServer.ClearHostList();
        MasterServer.RequestHostList("TowerDefence");
        
    }

    // Use this for initialization
    void Start()
    {
        
        if (mainSong)
        {
            mainSongObject.audio.clip = mainSong;
            mainSongObject.audio.Play();
        }
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

        if (Network.peerType == NetworkPeerType.Disconnected && Input.GetKeyUp(KeyCode.F1))
        {
            if (showMasterServerIpConf)
            {
                Network.Disconnect();
                MasterServer.ClearHostList();
                MasterServer.ipAddress = masterServerIP;

                Network.Connect(MasterServer.ipAddress, MasterServer.port);
                MasterServer.RequestHostList("TowerDefence");
            }
            showMasterServerIpConf = !showMasterServerIpConf;
        }
    }

    void OnGUI()
    {
        if (!levelLoaded)
        {
            if (Network.peerType == NetworkPeerType.Disconnected)
            {

                if (showMasterServerIpConf)
                {
                    GUI.Box(new Rect(10, 30, 100, 20), "MS IP:");
                    masterServerIP = GUI.TextField(new Rect(110, 30, 150, 20), masterServerIP);
                    GUI.Box(new Rect(10, 51, 100, 20), "Your local IP:");
                    GUI.TextField(new Rect(110, 51, 150, 20), Network.player.ipAddress);
                    GUI.Box(new Rect(10, 72, 100, 20), "Your external IP:");
                    GUI.TextField(new Rect(110, 72, 150, 20), Network.player.externalIP);
                }

                MasterServer.RequestHostList("TowerDefence");
                HostData[] hostData = MasterServer.PollHostList();
                    
                GUI.Box(new Rect(10, 195, Screen.width - 35, Screen.height - 210), "");

                GUI.BeginScrollView(new Rect(15, 200, Screen.width - 15, Screen.height - 210), scrollPosition, new Rect(0, 0, 0, Screen.height - 211));

                GUILayout.BeginHorizontal();
                GUILayout.Box("Game name:", GUILayout.Width(Screen.width * 0.234375f));
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
                Rect myRect = new Rect(Screen.width / 2 - currentSize.x / 2, 31, currentSize.x, currentSize.y);
                if (GUI.Button(myRect, "Start Lobby"))
                {
                    // Создание Сервера
                    Network.InitializeServer(3, remotePort, useNat);
                    if (gameName.Length < 5)
                        gameName = "Tower Deffence Game";
                    MasterServer.RegisterHost("TowerDefence", gameName, gameComment);
                }
                GUI.Box(new Rect(Screen.width / 2 - currentSize.x / 2, 70, currentSize.x, 20), "");
                currentSize = new Vector2(70, 20);
                useNat = GUI.Toggle(new Rect(Screen.width / 2 - currentSize.x / 2, 70, currentSize.x, currentSize.y), useNat, "Use Nat");

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
                    Network.Disconnect();
                    MasterServer.UnregisterHost();
                }
            }

            if (Network.isServer || Network.isClient)
            {
                string text;
                Vector2 size = new Vector2(200, 30);
                GUI.Box(new Rect(50, 100, size.x, size.y), "Server");
                if (!connectedPlayers[0].isEmptySlot)
                    GUI.Box(new Rect(50, 131, size.x, size.y), connectedPlayers[0].playerName);

                text = connectedPlayers[1].playerName;
                if (connectedPlayers[1].isEmptySlot)
                {
                    text = "Empty_Slot";
                }
                GUI.Box(new Rect(Screen.width - size.x - 50, 100, size.x, size.y), "Slot 1");
                if (!connectedPlayers[1].isEmptySlot)
                    GUI.Box(new Rect(Screen.width - size.x - 50, 131, size.x, size.y), text);
                text = connectedPlayers[2].playerName;
                if (connectedPlayers[2].isEmptySlot)
                {
                    text = "Empty_Slot";
                }
                GUI.Box(new Rect(50, Screen.height - 160, size.x, size.y), "Slot 2");
                if (!connectedPlayers[2].isEmptySlot)
                    GUI.Box(new Rect(50, Screen.height - 129, size.x, size.y), text);
                text = connectedPlayers[3].playerName;
                if (connectedPlayers[3].isEmptySlot)
                {
                    text = "Empty_Slot";
                }
                GUI.Box(new Rect(Screen.width - size.x - 50, Screen.height - 160, size.x, size.y), "Slot 3");
                if (!connectedPlayers[3].isEmptySlot)
                    GUI.Box(new Rect(Screen.width - size.x - 50, Screen.height - 129, size.x, size.y), text);

                Vector2 currentSize = new Vector2(300, 40);
                Rect myRect = new Rect(Screen.width / 2 - currentSize.x / 2, Screen.height - 30 - currentSize.y, currentSize.x, currentSize.y);

                if (Network.isServer && GUI.Button(myRect, "Start Game"))
                {
                    MasterServer.UnregisterHost();
                    networkView.RPC("LoadLevel", RPCMode.AllBuffered, null);
                }
            }
        }
    }

    void OnFailedToConnectToMasterServer(NetworkConnectionError info)
    {
        Debug.Log("Could not connect to master server: " + info + ". MasterServerIP: " + MasterServer.ipAddress);
    }
    void OnMasterServerEvent(MasterServerEvent msEvent)
    {
    }

    void OnPlayerConnected(NetworkPlayer player)
    {
        Debug.Log("Player " + " connected from " + player.ipAddress + ":" + player.port);
        Invoke("StartUpdateConnectedPlayers", 0.25f);
        networkView.RPC("PlayAudio", RPCMode.AllBuffered, new object[] { 1 });
    }
    void OnPlayerDisconnected(NetworkPlayer player)
    {
        CleanUpSlot(player);
        Invoke("StartUpdateConnectedPlayers", 0.25f);
        Network.RemoveRPCs(player);
        Network.DestroyPlayerObjects(player);
        networkView.RPC("PlayAudio", RPCMode.AllBuffered, new object[] { 2 });
    }

    void OnConnectedToServer()
    {
        networkView.RPC("TakeSlot", RPCMode.Server, new object[] { Network.player, playerName });
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

    void CleanUpSlot(NetworkPlayer player)
    {
        if (Network.isServer)
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

    void StartUpdateConnectedPlayers()
    {
        for (int i = 0; i < 4; i++)
        {
            if (connectedPlayers[i].isEmptySlot == false)
                networkView.RPC("UpdateConnectedPlayer", RPCMode.Others, new object[] { i, connectedPlayers[i].playerName, connectedPlayers[i].netPlayer, connectedPlayers[i].isEmptySlot });
            else
                networkView.RPC("UpdateConnectedPlayer", RPCMode.Others, new object[] { i, "Empty_Slot", new NetworkPlayer(), true });
        }
    }

	[RPC]
	void LoadLevel()
	{
        levelLoaded = false;
        Network.SetSendingEnabled(0, false);
        Network.isMessageQueueRunning = false;
		Application.LoadLevel("devScene");

        Network.SetSendingEnabled(0, true);
        Network.isMessageQueueRunning = true;
        this.GetComponent<Camera>().enabled = false;
        this.GetComponent<GUILayer>().enabled = false;
        levelLoaded = true;
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
    void UpdateConnectedPlayer(int id, string name, NetworkPlayer player, bool slotStatus)
    {
        connectedPlayers[id].playerName = name;
        connectedPlayers[id].netPlayer = player;
        connectedPlayers[id].isEmptySlot = slotStatus;
    }

    [RPC]
    void PlayAudio(int id)
    {
        switch (id)
        {
            case 1:
                if (connectedPlayerSong)
                {
                    audio.clip = connectedPlayerSong;
                    audio.Play();
                }
                break;
            case 2:
                if (disconnectedPlayerSong)
                {
                    audio.clip = disconnectedPlayerSong;
                    audio.Play();
                }
                break;
        }
    }
}