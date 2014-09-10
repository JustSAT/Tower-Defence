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
        public MyNetType playerNetType;
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

                if (playerName.Length < 3)
                {
                    playerName = "Player";
                }
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
        foreach (NewNetworkPlayer nnp in connectedPlayers)
        {
            if (nnp.isEmptySlot)
            {
                nnp.isEmptySlot = false;
                nnp.netPlayer = player;
                nnp.playerNetType = MyNetType.client;
            }
        }
    }

    void OnServerInitialized()
    {
        connectedPlayers[0].playerName = "Server";
        connectedPlayers[0].playerNetType = MyNetType.server;
    }


    public void BroadcastNickName()
    {
        networkView.RPC("UpdateNickName", RPCMode.All, playerName);
    }

    [RPC]
    void UpdateNickName(string aNewNickName)
    {
        if (networkView.isMine)
            return; // We don't want others to change our nickname
        playerName = aNewNickName;
    }

}



/*
var remoteIP = "127.0.0.1";
var remotePort = 25000;
var listenPort = 25000;
var useNAT = false;
var yourIP = "";
var yourPort = "";

function OnGUI () {
// Проверка подключены ли вы к Серверу или нет
if (Network.peerType == NetworkPeerType.Disconnected)
{
// Если вы подключены
if (GUI.Button (new Rect(10,10,100,30),"Connect"))
{
Network.useNat = useNAT;
// Подключение к Серверу
Network.Connect(remoteIP, remotePort);
}
if (GUI.Button (new Rect(10,50,100,30),"Start Server"))
{
Network.useNat = useNAT;
// Создание Сервера
Network.InitializeServer(32, listenPort);
// Сказать нашим объектам, что уровень и сеть готова к работе
for (var go : GameObject in FindObjectsOfType(GameObject))
{
go.SendMessage("OnNetworkLoadedLevel",
SendMessageOptions.DontRequireReceiver);
}
}
// Создаем поля  ip адрес и port
remoteIP = GUI.TextField(new Rect(120,10,100,20),remoteIP);
remotePort = parseInt(GUI.TextField(new
Rect(230,10,40,20),remotePort.ToString()));
}
else
{
// Получаем твой  ip адрес и port
ipaddress = Network.player.ipAddress;
port = Network.player.port.ToString();
GUI.Label(new Rect(140,20,250,40),"IP Adress: "+ipaddress+":"+port);
if (GUI.Button (new Rect(10,10,100,50),"Disconnect"))
{
// Отключение от Сервера
Network.Disconnect(200);
}
}
}

function OnConnectedToServer () {
// Сказать всем объектам что сцена и сеть готовы
for (var go : GameObject in FindObjectsOfType(GameObject))
go.SendMessage("OnNetworkLoadedLevel",
SendMessageOptions.DontRequireReceiver);
}


*/