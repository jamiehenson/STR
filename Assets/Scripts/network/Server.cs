using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Server : MonoBehaviour {

    private int playerCount = 0;
    public static string gameName = "Spektor0307";
    public Transform playerUniversePrefab;
    //public Transform enemyHolderprefab;
    public static int countUniverse;
    public Transform[] universe;
    private Bridge bridge;
    public Transform characterPrefab;
    public Transform Networkbridge;
    private int nextPlayerID = 0;
    private Dictionary<NetworkViewID, string> viewIDNameMapping;
    private Dictionary<NetworkViewID, string> viewIDChNameMapping;
    public NetworkView[] characterView;
    public Commander commander;
    public bool startGame;
    public static string serverAddress;

    // Use this for initialization
    void Start() {

        MasterServer.ipAddress = "54.243.193.180";
        MasterServer.port = 23466;
        Network.natFacilitatorIP = "54.243.193.180";
        Network.natFacilitatorPort = 50005;
        
		Network.InitializeServer(NetworkConstants.connectionsAllowed, 23467, true);

		// If using the MasterServer, then connect to it
		//if (NetworkConstants.usingMasterServer)
		//{
	        //Online Server Code - Do Not Delete
	        MasterServer.ipAddress = "54.243.193.180";
	        MasterServer.port = 23466;
	        Network.natFacilitatorIP = "54.243.193.180";
	        Network.natFacilitatorPort = 50005;
	        Debug.Log(MP.serverName);
	        MasterServer.RegisterHost(gameName, MP.serverName, "This is a test game");
		//}
        //Online Server Code - Do not Delete

        // LAN Server
       // Network.InitializeServer(NetworkConstants.connectionsAllowed, NetworkConstants.serverPort, false);
        // LAN Server
        bridge = GameObject.Find("Networkbridge(Clone)").GetComponent<Bridge>();
    }

    void OnMasterServerEvent(MasterServerEvent msEvent) {
        if (msEvent == MasterServerEvent.RegistrationSucceeded)
            Debug.Log("Registered MasterServer");
    }

    // Messages
    void OnServerInitialized() {
        //Debug.Log("Initialized Server" + MasterServer.ipAddress+ MasterServer.port);
        countUniverse = 4;
        universe = new Transform[countUniverse+1];
        characterView = new NetworkView[countUniverse+1];
        viewIDNameMapping = new Dictionary<NetworkViewID, string>();
        viewIDChNameMapping = new Dictionary<NetworkViewID, string>();
        Network.Instantiate(Networkbridge, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0), 99);
        for (int i = 1; i <= countUniverse; i++) {
            Vector3 pos = new Vector3(0 + (i * 10000), 0, 0);
            Transform obj = (Transform)Network.Instantiate(playerUniversePrefab, pos, new Quaternion(0, 0, 0, 0), 99);
            universe[i] = obj;
            universe[i].transform.Find("Managers/OriginManager").GetComponent<Universe>().origin = pos;
            obj.name = "Universe" + i;
            viewIDNameMapping.Add(obj.networkView.viewID, obj.name);
            Vector3 position = new Vector3(pos.x - 8, pos.y, pos.z + 15);
            Transform characterPlayer = (Transform)Network.Instantiate(characterPrefab, position, new Quaternion(0,0,0,0), i);
            characterPlayer.name = "Character" + i;
         //   Transform enemyHolder = (Transform)Network.Instantiate(enemyHolderprefab, position, new Quaternion(0, 0, 0, 0), 99);
          //  enemyHolder.name = "EnemyHolder" + i;
            NetworkView nView = characterPlayer.GetComponent<NetworkView>(); 
            characterView[i] = nView;
            viewIDChNameMapping.Add(characterPlayer.networkView.viewID, characterPlayer.name);

        }
    }


    void OnPlayerConnected(NetworkPlayer player) {
        nextPlayerID = nextPlayerID + 1;
        Network.SetReceivingEnabled(player, 99, true);
        Network.SetSendingEnabled(player, 99, true);
        for (int i = 1; i < countUniverse; i++)
        {
            Network.SetReceivingEnabled(player, i, i == nextPlayerID);
            Network.SetSendingEnabled(player, i, i == nextPlayerID);
        }

        bridge.addPlayer(player);
        Debug.Log("Player has connected" + playerCount++ + "connected from" + player.ipAddress + ":" + player.port);
        bridge.setUniverse(nextPlayerID, player);
        bridge.updateUniverseNames(viewIDNameMapping, player);
        bridge.updateCharacterNames(viewIDChNameMapping, player);
        if (!startGame)
            if (Network.connections.Length == 1)
            {
                startGame = true;
                for (int i = 1; i <= countUniverse; i++)
                {
                    // Enable enemy generation
                    commander = GameObject.Find("Universe" + i+ "/Managers/EnemyManager").GetComponent<Commander>();
                    commander.enabled = true;
                }
                
            }
    }


    void OnPlayerDisconnected() {
        Debug.Log("Clean up after player");

    }

    // Update is called once per frame
    void Update() {
    }
}