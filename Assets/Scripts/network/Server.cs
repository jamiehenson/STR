using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Server : MonoBehaviour {

    private int playerCount = 0;
    public static string gameName = "Spektor0307"; // I think this should be STR ?
    public Transform playerUniversePrefab;
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
    public bool startGame, manualGoAhead;
    public static string serverAddress;
    public GUIStyle buttonStyle;
	private string playersJoined;

    // Use this for initialization
    void Start() {
		// Set MasterServer
        MasterServer.ipAddress = NetworkConstants.masterServerAddress;
        Network.natFacilitatorIP = NetworkConstants.masterServerAddress;
		
		// Initialise as a Server
		Network.InitializeServer(NetworkConstants.connectionsAllowed, NetworkConstants.serverPort, true);
		
		// Register with MasterSever
	    MasterServer.RegisterHost(gameName, MP.serverName, "This is a test game");
		
		// Set bridge
		bridge = GameObject.Find("Networkbridge(Clone)").GetComponent<Bridge>();
		
        //Online Server Code - Do not Delete
        // Network.InitializeServer(NetworkConstants.connectionsAllowed, NetworkConstants.serverPort, false);
    }
	
	// When connected to MasterServer LogNote
    void OnMasterServerEvent(MasterServerEvent msEvent) {
        if (msEvent == MasterServerEvent.RegistrationSucceeded)
            Log.Note("Registered MasterServer");
    }

    // When sever is initalise, set it up
    void OnServerInitialized() {
        Log.Note("Initialized Server" + MasterServer.ipAddress+ MasterServer.port);
		
		// Initalise private memeber variables
        countUniverse = 4;
        universe = new Transform[countUniverse+1];
        characterView = new NetworkView[countUniverse+1];
        viewIDNameMapping = new Dictionary<NetworkViewID, string>();
        viewIDChNameMapping = new Dictionary<NetworkViewID, string>();
		
		// Initanitate the bridge
        Network.Instantiate(Networkbridge, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0), 99);
		
		// Set up each universe
        for (int i = 1; i <= countUniverse; i++) {
			// Instaniate in correct position
            Vector3 pos = new Vector3(0 + (i * 10000), 0, 0);
            Transform obj = (Transform)Network.Instantiate(playerUniversePrefab, pos, new Quaternion(0, 0, 0, 0), 99);
            universe[i] = obj;
			
			// Rename it to something useful and pass name to clients
            universe[i].transform.Find("Managers/OriginManager").GetComponent<Universe>().origin = pos;
            obj.name = "Universe" + i;
            viewIDNameMapping.Add(obj.networkView.viewID, obj.name);
			
			// Instaniate charater in universe
            Vector3 position = new Vector3(pos.x - 8, pos.y, pos.z + 15);
            Transform characterPlayer = (Transform)Network.Instantiate(characterPrefab, position, new Quaternion(0,0,0,0), i);
			
			// Rename the character and pass name to clients
            characterPlayer.name = "Character" + i;
            NetworkView nView = characterPlayer.GetComponent<NetworkView>(); 
            characterView[i] = nView;
            viewIDChNameMapping.Add(characterPlayer.networkView.viewID, characterPlayer.name);
        }
    }

	// When a player is connected , set it up correctly
    void OnPlayerConnected(NetworkPlayer player) {
        nextPlayerID = nextPlayerID + 1;
		
		// Allow it to work with "command" RPCs
        Network.SetReceivingEnabled(player, 99, true);
        Network.SetSendingEnabled(player, 99, true);
		
		// Set it to only work with RPC/sync objects in the universe it currently is in
        for (int i = 1; i < countUniverse; i++)
        {
            Network.SetReceivingEnabled(player, i, i == nextPlayerID);
            Network.SetSendingEnabled(player, i, i == nextPlayerID);
        }
		
		// Send bridge data to send to client when its ready
        bridge.addPlayer(player);
        bridge.setUniverse(nextPlayerID, player);
        bridge.updateUniverseNames(viewIDNameMapping, player);
        bridge.updateCharacterNames(viewIDChNameMapping, player);
		
		GameObject activeChar = GameObject.Find("Character" + nextPlayerID);
		PlayerManager pm = activeChar.GetComponent<PlayerManager>();
		string playerName = pm.returnName();
		int characterNum = 0;
		
		playersJoined = playersJoined + "Character " + characterNum + ": " + playerName + " has joined the game...\n";
		Log.Note("Player has connected" + playerCount++ + "connected from" + player.ipAddress + ":" + player.port);
		
        if (!startGame)
        {
            if (manualGoAhead)
            {
                print("LETS GO");
                startGame = true;
                for (int i = 1; i <= countUniverse; i++)
                {
                    // Enable enemy generation
                    commander = GameObject.Find("Universe" + i + "/Managers/EnemyManager").GetComponent<Commander>();
                    commander.enabled = true;
                }
            }
        }
    }

	// When player disconnects, log event
    void OnPlayerDisconnected() {
		//GameObject activeChar = GameObject.Find("Character" + nextPlayerID);
		//PlayerManager pm = activeChar.GetComponent<PlayerManager>();
		//string playername = pm.returnName();
        //playersJoined = playersJoined + "Character " + nextPlayerID + ": " + playername + " has left the game...\n";
    }

    void OnGUI()
    {
		Texture2D svrbg = HudOn.fillTex(Screen.width,Screen.height,Color.black);
		Texture2D svrTitle = (Texture2D) Resources.Load ("hud/svrTitle");
		GUI.Box(new Rect(0,0,Screen.width,Screen.height),svrbg);
		GUI.Label (new Rect(Screen.width/2-200, 0, 400, 200),svrTitle);
        int x = 400;
        if (!manualGoAhead) if (GUI.Button(new Rect((Screen.width / 2) - x/2, (Screen.height / 2) - x/8, x, x/4), "START GAME", buttonStyle)) manualGoAhead = true;
		GUI.Label(new Rect(Screen.width/4, Screen.height*0.75f, Screen.width/2, 200), playersJoined);
		if (GUI.Button(new Rect(Screen.width/4,Screen.height*0.9f, Screen.width/2, 50), "QUIT SERVER", buttonStyle)) Application.LoadLevel("menu");
    }
}