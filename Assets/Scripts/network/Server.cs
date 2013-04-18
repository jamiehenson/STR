using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Server : MonoBehaviour {

    private int playerCount = 0;
    public static string gameName = "STD"; // I think this should be STR ?
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
    public LevelManager levMan;
    public bool startGame, manualGoAhead;
    public static string serverAddress;
    public GUIStyle buttonStyle;
	private string playersJoined;
    int ID = 1;
    public static int finalNumberofPlayers;
    public string takenNames;
    public string[] playerNames;

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
        Log.Note("Count Universe: " + countUniverse);
		// Initalise private memeber variables
        playerNames = new string[countUniverse + 1];
        finalNumberofPlayers = countUniverse;
        universe = new Transform[countUniverse+2];
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
			//characterPlayer.Rotate(new Vector3(0,180,0));
			
			// Rename the character and pass name to clients
            characterPlayer.name = "Character" + i;
			characterPlayer.GetComponent<PlayerManager>().universeNumber = i;
            NetworkView nView = characterPlayer.GetComponent<NetworkView>(); 
            characterView[i] = nView;
            viewIDChNameMapping.Add(characterPlayer.networkView.viewID, characterPlayer.name);
        }

        // Set up boss universe
        Vector3 pos_b = new Vector3(0 + ((countUniverse+1) * 10000), 0, 0);
        Transform obj_b = (Transform)Network.Instantiate(playerUniversePrefab, pos_b, new Quaternion(0, 0, 0, 0), 99);
        universe[countUniverse+1] = obj_b;

        // Rename it to something useful and pass name to clients
        universe[countUniverse+1].transform.Find("Managers/OriginManager").GetComponent<Universe>().origin = pos_b;
        obj_b.name = "Universe0";
        viewIDNameMapping.Add(obj_b.networkView.viewID, obj_b.name);
			
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
		
    }

	// When player disconnects, log event
    void OnPlayerDisconnected() {
		Log.Warning("In OnPlayerDisconnected, connects = "+Network.connections.Length);
		//GameObject activeChar = GameObject.Find("Character" + nextPlayerID);
		//PlayerManager pm = activeChar.GetComponent<PlayerManager>();
		//string playername = pm.returnName();
        //playersJoined = playersJoined + "Character " + nextPlayerID + ": " + playername + " has left the game...\n";
		if (Network.connections.Length == 1) { // Go back to menu
			Misc.CleanStatics();
			Application.LoadLevel("menu");
		}
    }

    void OnGUI()
    {
        int x = 400;
        /* Notify server which player has connected. */
        /* GUI part*/
        for(int i = 1; i <ID; i++)
        {
            GUI.Label(new Rect(60, 200 + (20 * i), 300, 100), i + ". Player " + playerNames[i] + "has joined the game.");
        }
        /* Check if a new player has connected. */
        if (!manualGoAhead)
        {
            if (ID<= countUniverse && GameObject.Find("Character" + ID).GetComponent<PlayerManager>().getPlayerName() != null)
            {
                /*if (takenNames.Contains(GameObject.Find("Character" + ID).GetComponent<PlayerManager>().getPlayerName()))
                    GameObject.Find("Character" + ID).GetComponent<PlayerManager>().updatePlayerNameS(GameObject.Find("Character" + ID).GetComponent<PlayerManager>().getPlayerName() + "'");*/
                PlayerManager manager = GameObject.Find("Character" + nextPlayerID).GetComponent<PlayerManager>();
                playerNames[ID] = manager.getPlayerName();

                GameObject.Find("Main Camera").GetComponent<ServerScoringSystem>().updatePlayerNames(ID, manager.getPlayerName());
                manager.updatePlayerNames(ID, manager.getPlayerName());
                ID++;
                takenNames = takenNames + ", " + manager.getPlayerName();
            }

        }
        /* End*/

        /* Start the AI part of the game.*/
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

                    // Enable level progression
                    LevelManager levMan = GameObject.Find("Universe" + i + "/Managers/LevelManager").GetComponent<LevelManager>();
                    levMan.enabled = true;
                }
            }
        }

        if (!manualGoAhead) if (GUI.Button(new Rect((Screen.width / 2) - x/2, (Screen.height / 2) - x/4, x, x/2), "START GAME", buttonStyle)) manualGoAhead = true;
		GUI.Label(new Rect(Screen.width/4, Screen.height*0.75f, Screen.width/2, 200), playersJoined);
    }

    public static int numberOfPlayers()
    {
        return finalNumberofPlayers;
    }

	public void moveCamera(int universeNum, NetworkPlayer player){
		bridge.moveCamera(universeNum, player);
	}
}