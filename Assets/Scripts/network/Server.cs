using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Server : MonoBehaviour {

    public static string gameName = "The Great Adventures of Mort"; // I think this should be STR ?
    public Transform playerUniversePrefab;
    public static int countUniverse;
    public Transform[] universe;
    public Transform characterPrefab;
    public Transform Networkbridge;
    public Dictionary<int, string> playerName;
    public NetworkView[] characterView;
    public Commander commander;
    public LevelManager levMan;
    public bool startGame = false;
    public bool manualGoAhead = false;

    public GUIStyle buttonStyle;
    public static int finalNumberofPlayers;
    public static string serverAddress;
    public string takenNames;
    public int lives;


    public string[] playerNames;
    public string[] playerFlags;

	private string playersJoined = "";
    private static int ID = 1;

    private Dictionary<NetworkViewID, string> viewIDNameMapping;
    private Dictionary<NetworkViewID, string> viewIDChNameMapping;
    private Bridge bridge;
    private int nextPlayerID = 0;

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

    // When server is initalised, set it updoes 
    void OnServerInitialized() {
        Log.Note("Initialized Server" + MasterServer.ipAddress+ MasterServer.port);
        Log.Note("Count Universe: " + countUniverse);
		// Initalise private memeber variables
        playerNames = new string[countUniverse + 1];
		playerFlags = new string[countUniverse + 1];
        finalNumberofPlayers = countUniverse;
        universe = new Transform[countUniverse+2];
        characterView = new NetworkView[countUniverse+1];
        viewIDNameMapping = new Dictionary<NetworkViewID, string>();
        viewIDChNameMapping = new Dictionary<NetworkViewID, string>();
		
		// Instantiate the bridge
        Network.Instantiate(Networkbridge, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0), 99);
		
		// Set up each universe
        for (int i = 1; i <= countUniverse; i++) {
            // Instantiate in correct position
            Vector3 pos = new Vector3(0 + (i * 10000), 0, 0);
            Transform obj = (Transform)Network.Instantiate(playerUniversePrefab, pos, new Quaternion(0, 0, 0, 0), 99);
            universe[i] = obj;
            
			// Rename it to something useful and pass name to clients
            universe[i].transform.Find("Managers/OriginManager").GetComponent<Universe>().origin = pos;
            obj.name = "Universe" + i;
            viewIDNameMapping.Add(obj.networkView.viewID, obj.name);

            // Instantiate charater in universe
            Vector3 position = new Vector3(pos.x - 8, pos.y, pos.z + 15);
   
            Transform characterPlayer = (Transform)Network.Instantiate(characterPrefab, position, new Quaternion(0,0,0,0), i);
			// Rename the character and pass name to clients
            characterPlayer.name = "Character" + i;
			characterPlayer.GetComponent<PlayerManager>().universeNumber = i;
            NetworkView nView = characterPlayer.GetComponent<NetworkView>(); 
            characterView[i] = nView;
            viewIDChNameMapping.Add(characterPlayer.networkView.viewID, characterPlayer.name);
          //  GameObject.Find("Character1/usa_model").SetActive(true);
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

	// When a player is connected, set it up correctly
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

		changeSystemNames(countUniverse);
        if (nextPlayerID == (countUniverse))
        {
            StartCoroutine("SetGameGoing");
        }
    }

	private IEnumerator SetGameGoing() {
		int countdown = 5;
		for (int i = countdown; i > 0; i--) {
			bridge.networkView.RPC("SendCountdownNumber",RPCMode.Others,i);
			yield return new WaitForSeconds(1);
		}
		bridge.networkView.RPC("SendCountdownNumber",RPCMode.Others,-1);
		lives = countUniverse * 3;
        startGame = true;
        EnableGameplayScripts();
        foreach( GameObject g in GameObject.FindGameObjectsWithTag("Player")) g.GetComponent<PlayerMovement>().updateStartGame();
        // Start the game when all players are in the game.
	}

	// When player disconnects, log event
    void OnPlayerDisconnected() {
		Log.Warning("In OnPlayerDisconnected, connects = "+Network.connections.Length);
        //playersJoined = playersJoined + "Character " + nextPlayerID + ": " + playername + " has left the game...\n";
		if (Network.connections.Length == 1) { // Go back to menu
			Misc.CleanStatics();
			Application.LoadLevel("menu");
		}
    }

    void EnableGameplayScripts() {
        Debug.Log("LETS GO");

        startGame = false;
        // Only do for the actual number of players?
        for (int i = 1; i <= countUniverse; i++) {
            //Initialize number of lives
            GameObject.Find("Character" + i).GetComponent<PlayerManager>().initLivesServer(lives);
            // Enable enemy generation
            commander = GameObject.Find("Universe" + i + "/Managers/EnemyManager").GetComponent<Commander>();
            commander.enabled = true;

            // Enable level progression (needs to start AFTER commander)
            LevelManager levMan = GameObject.Find("Universe" + i + "/Managers/LevelManager").GetComponent<LevelManager>();
            levMan.enabled = true;
        }
        // Enable boss level
        BossLevelManager blm = GameObject.Find("Universe" + 0 + "/Managers/LevelManager").GetComponent<BossLevelManager>();
        blm.enabled = true;

        // Enable timing
        GameObject.Find("Main Camera").GetComponent<ServerScoringSystem>().StartTimer();
    }

    void OnGUI()
    {
		if (GameObject.Find("Character" + ID) == null)
			return;
		
        /* Notify server which player has connected. */
        /* GUI part*/
        for(int i = 1; i <ID; i++)
        {
            GUI.Label(new Rect(60, 200 + (20 * i), 300, 100), i + ". Player " + playerNames[i] + "has joined the game.");
        }

        /* Check if a new player has connected. */
        if (!manualGoAhead)
        {
           //if (ID <= countUniverse && GameObject.Find("Character" + ID).GetComponent<PlayerManager>().getPlayerName() != null && ID == nextPlayerID )


            if (GameObject.Find("Character" + ID).GetComponent<PlayerManager>().getPlayerName() != null && !takenNames.Contains(GameObject.Find("Character" + ID).GetComponent<PlayerManager>().getPlayerName()))

            {
                if (Misc.ArrayContains(playerNames, GameObject.Find("Character" + ID).GetComponent<PlayerManager>().getPlayerName()))
                    GameObject.Find("Character" + ID).GetComponent<PlayerManager>().updatePlayerNameS(GameObject.Find("Character" + ID).GetComponent<PlayerManager>().getPlayerName() + "'");
                PlayerManager manager = GameObject.Find("Character" + nextPlayerID).GetComponent<PlayerManager>();
                
             //  Debug.Log("Add " + manager.getPlayerName());
               // playerName.Add(ID, manager.getPlayerName());
                Debug.Log("Player :" + manager.getPlayerName() + "has connected");
                playerNames[ID] = manager.getPlayerName();
				playerFlags[ID] = manager.getFlag();
                GUI.Label(new Rect(60, 60 + (20 * ID), 64, 64), "Player :" + manager.getPlayerName() + "has connected");

                GameObject.Find("Main Camera").GetComponent<ServerScoringSystem>().updatePlayerNames(ID, manager.getPlayerName());
                manager.updatePlayerNames(ID, manager.getPlayerName());
				manager.updatePlayerFlags(ID, manager.getFlag());
                ID++;
                takenNames = takenNames + " " + manager.getPlayerName();
            }
        }
        /* End*/

		GUI.Label(new Rect(Screen.width/4, Screen.height*0.75f, Screen.width/2, 200), playersJoined);
    }

    public static int numberOfPlayers() {
        return finalNumberofPlayers;
    }

	public void moveCamera(int universeNum, NetworkPlayer player){
		bridge.moveCamera(universeNum, player);
	}

	public void changeSystemNames(int countUniverse) {
		bridge.systemNames = new List<string>(countUniverse);


		for (int i=0; i<countUniverse; i++)
			bridge.systemNames.Add(generateSystemNames());

		bridge.sendSystemNames(countUniverse);
	}

	public string generateSystemNames()
	{
		ArrayList greek = new ArrayList();
		greek.Add("alpha");
		greek.Add("beta");
		greek.Add("gamma");
		greek.Add("delta");
		greek.Add("epsilon");
		greek.Add("zeta");
		greek.Add("eta");
		greek.Add("theta");
		greek.Add("iota");
		greek.Add("kappa");
		greek.Add("lambda");
		greek.Add("mu");
		greek.Add("nu");
		greek.Add("xi");
		greek.Add("omicron");
		greek.Add("pi");
		greek.Add("rho");
		greek.Add("sigma");
		greek.Add("tau");
		greek.Add("upsilon");
		greek.Add("phi");
		greek.Add("chi");
		greek.Add("psi");
		greek.Add("omega");
		string system = (string) greek[(int) Random.Range(0,greek.Count)] + "-" + Random.Range(0,20).ToString();
		return system.ToUpper();
	}
}