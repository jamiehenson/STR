using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour {
    // Player stats
    private int score;
	
	// Lives
	public static int lives = -1;
    public static string loser = "";
    public bool[] remainingUni;

	//Stats
    private float hitPoints;
    private float energyLevel;
    private float startHP, startEnergy;
    private float energyBank = 0;
    private int selectedWepDrain;
    private int bankSize = 10000;
    public static bool bankFull;
    public static float speed;
    public static string playername, flagname;
    private GameObject xp;
    public string[] playerNames, playerFlags;
	public PlayerMovement movement;
    public FiringHandler firingHandler;
    private string myPlayerName;
	
	public int universeNumber;

    // Character-centric player stats
    public string activeCharN;
    private string activeModel;
    public static string activeChar;
    public int wepType;
	public WeaponStats wepStats;
    public static float damageMultiplier;
    public static float energyMultiplier;
	public static PlayerManager Instance;
    private bool setModel= false;
    //Scoring System variables
    private bool myCharacter;
    public int characterNum;

	
	public void changeWeapon(int type){
		wepStats = WeaponHandler.GetWeaponStats(activeChar, type);
		wepType = type;
		
		if (Network.isClient)
			networkView.RPC("changeWeaponRPC", RPCMode.Server, type);
	}

    public void updatePlayerNames(int pos, string s)
    {
        playerNames[pos] = s;
        networkView.RPC("updatePlayerNameC", RPCMode.AllBuffered, pos, s);
    }

	public void updatePlayerFlags(int pos, string s)
    {
        playerFlags[pos] = s;
        networkView.RPC("updatePlayerFlagC", RPCMode.AllBuffered, pos, s);
    }

	[RPC]
	public void changeWeaponRPC(int type){
		changeWeapon(type);
	}

	public void Awake() {
		Instance = this;
	}

	public void Start()
	{
		movement = gameObject.GetComponent<PlayerMovement>();
        myPlayerName = playername;
        movement.playerModel(activeChar);
        firingHandler = gameObject.GetComponent<FiringHandler>();
        firingHandler.playerModel(activeChar);
		if (Network.isServer)
        {
            playerNames = new string[Server.numberOfPlayers() + 1];
            networkView.RPC("instantiatePlayerNames", RPCMode.AllBuffered, Server.numberOfPlayers() + 1);
			playerFlags = new string[Server.numberOfPlayers() + 1];
            networkView.RPC("instantiatePlayerFlags", RPCMode.AllBuffered, Server.numberOfPlayers() + 1);
        }

	}

    public float getEnergyLevel()
    {
        return energyLevel;
    }

    public int getLives()
    {
        return lives;
    }

    /* Score functions*/
    public int getScore()
    {
        return score;
    }

    public void updateScore(int s)
    {
        score = score + s;
    }

    /* End of Score functions*/
    public void updateEnergyLevel(float val)
    {
        energyLevel = energyLevel + val;

    }

    public float getStartHP()
    {
        return startHP;
    }

    public float getStartEnergy()
    {
        return startEnergy;
    }

    public int getSelectedWepDrain()
    {
        return selectedWepDrain;
    }
    public int getBankSize()
    {
        return bankSize;
    }

    /*Hit Points call functions*/
    public float getHitPoints()
    {
        return hitPoints;
    }

    public void updateHitPoints(float val)
    {
        hitPoints = hitPoints + val;
    }

    public void resetHitPoints(float val)
    {
        hitPoints = val;
    }
    /*End of Hit Points call functions*/

    /* Energy Bank call functions*/
    public float getEnergyBank()
    {
        return energyBank;
    }

    public void resetEnergyBank(float val)
    {
        energyBank = val;
    }
    /* Energy Bank call functions*/

    public string getPlayerName()
    {
        return myPlayerName;
        
    }

    public void updatePlayerNameS(string name)
    {
        myPlayerName = name;
        
        networkView.RPC("updatePlayerName", RPCMode.Others, myPlayerName);
    }

	public string getFlag()
    {
        return flagname;
    }

    public string getActiveChar()
    {
        return activeChar;
    }

    public void activateCharacter(int charNum)
    {
        myCharacter = true;
        universeNumber = charNum;
        characterNum = charNum;
        networkView.RPC("updateCharacterNum", RPCMode.OthersBuffered, charNum);
		HudOn.Instance.setManager(this);
		Instance = this;
    }

    /* Called in HudOn class, Start() */
    public void InitialiseStats()
    {
		changeWeapon(1);
        if (Network.isClient)
        {
            networkView.RPC("updatePlayerName", RPCMode.Server, playername);
			networkView.RPC("updatePlayerFlag", RPCMode.Server, flagname);
            switch (activeChar)
            {
                case "china":
                    startHP = 500;
                    startEnergy = 1000;
                    speed = 15.0f;
                    damageMultiplier = 0.5f;
                    energyMultiplier = 1f;
                    bankSize = 7000;
                    energyBank = 0;
                    break;
                case "usa":
                    startHP = 1000;
                    startEnergy = 500;
                    speed = 5.0f;
                    damageMultiplier = 1.5f;
                    energyMultiplier = 1.5f;
                    bankSize = 7000;
                    energyBank = 0;
                    break;
                case "russia":
                    startHP = 750;
                    startEnergy = 750;
                    speed = 10.0f;
                    damageMultiplier = 1.0f;
                    energyMultiplier = 1.25f;
                    bankSize = 5000;
                    energyBank = 0;
                    break;
                default:
                    startHP = 1000;
                    startEnergy = 1000;
                    speed = 10.0f;
                    damageMultiplier = 1f;
                    energyMultiplier = 1f;
                    bankSize = 7000;
                    energyBank = 0;
                    break;
            }
            if (activeChar == null) activeChar = "tester";
            Debug.Log("Active " + activeCharN);
            activeModel = activeChar;
            networkView.RPC("updateActiveChar", RPCMode.OthersBuffered, activeModel);
            hitPoints = startHP;
            energyLevel = startEnergy;
            score = 0;
            WeaponHandler.ScaleDamages(damageMultiplier);
            networkView.RPC("updateStartEnergy", RPCMode.Server, startEnergy);
			networkView.RPC("updateStartHP", RPCMode.Server, startHP);
            networkView.RPC("updateEnergy", RPCMode.All, energyLevel);
            networkView.RPC("updateHitP", RPCMode.All, hitPoints);
          
        }
    }

    [RPC]
    void updateActiveChar(string model)
    {
        activeModel = model;
        Debug.Log("Active character " + characterNum + " " + activeModel);
    }
 
    [RPC]
    void updateCharacterModel(string activeC, int characterN)
    {
        Debug.Log("Activate " + "Character" + characterN + "/" + activeC);
        if(activeC == "china") GameObject.Find("Character" + characterN + "/" + activeC).SetActive(true);
        else GameObject.Find("Character" + characterN + "/china").SetActive(false);

        if (activeC == "usa") GameObject.Find("Character" + characterN + "/" + activeC).SetActive(true);
        else GameObject.Find("Character" + characterN + "/usa").SetActive(false);

        if (activeC == "russia") GameObject.Find("Character" + characterN + "/" + activeC).SetActive(true);
        else GameObject.Find("Character" + characterN + "/russia").SetActive(false);
    }

    void updateCharacterModelClient(string activeC, int characterN)
    {
        Debug.Log("Activate " + "Character" + characterN + "/" + activeC);
        if (activeC == "china") GameObject.Find("Character" + characterN + "/" + activeC).SetActive(true);
        else GameObject.Find("Character" + characterN + "/china").SetActive(false);

        if (activeC == "usa") GameObject.Find("Character" + characterN + "/" + activeC).SetActive(true);
        else GameObject.Find("Character" + characterN + "/usa").SetActive(false);

        if (activeC == "russia") GameObject.Find("Character" + characterN + "/" + activeC).SetActive(true);
        else GameObject.Find("Character" + characterN + "/russia").SetActive(false);
    }

    void Update()
    {

        
        if (movement.startGame && !setModel && !myCharacter)
        {
            Debug.Log("Active character model " + characterNum + " " + activeModel);
            updateCharacterModelClient(activeModel, characterNum);
            setModel = true;
        }
		if (Network.isClient && myCharacter)
        {
            if (!setModel)
            {
                updateCharacterModelClient(activeModel, characterNum);
                setModel = true;
            }
            //networkView.RPC("updateCharacterModel", RPCMode.Server, activeChar, characterNum);  
            switch (wepType)
            {
                case 1:
                    selectedWepDrain = 6;
                    break;
                case 2:
                    selectedWepDrain = 15;
                    break;
                case 3:
                    selectedWepDrain = 70;
                    break;
                default:
                    break;
            }
            networkView.RPC("updateWepDrain", RPCMode.Server, selectedWepDrain);
        }
        //Debug.Log("Available lives:" + lives);
        // Recharge power supply
        if (Network.isServer)
        {
            if (energyLevel > 0 && energyLevel <= startEnergy && Time.timeScale != 0) energyLevel += (startEnergy / 800);
            if (energyLevel <= 0) energyLevel = 1;
			// Update number of lives
            if (lives == 1 && hitPoints<0)
            {
                Debug.Log("GAME END");
                networkView.RPC("endGame", RPCMode.Others);
            }
            if (hitPoints < 0 && lives>1)
				{
                    loser = playerNames[characterNum];
					hitPoints = startHP;
					lives--;
                    Debug.Log("Remaining lives " + lives);
                    networkView.RPC("updateLives", RPCMode.Others, lives, loser);
                    if (lives < playerNames.Length-1)
                    {
                        Debug.Log("Restrict universe. Remaining lives : " + lives + " " + playerNames.Length);
                        remainingUni[characterNum] = false;
                        for (int i = 1; i <= GameObject.FindGameObjectsWithTag("Player").Length; i++)
                        {
                            if (remainingUni[i])
                            {
                                Debug.Log("Warp to universe " + i + ".Lives " + lives);
                                networkView.RPC("restrictUniverses", RPCMode.Others, i, characterNum);
                                break;
                            }
                                
                        }
                    }
				}
            
            if (!bankFull) energyBank += (startEnergy / 1500);
            networkView.RPC("updateEnergy", RPCMode.All, energyLevel);
            networkView.RPC("updateHitP", RPCMode.All, hitPoints);
            networkView.RPC("updatePlayerScore", RPCMode.All, score);
        }
    }

    [RPC]
    void endGame()
    {
        Network.Disconnect();
        MasterServer.UnregisterHost();
        GameObject.Find("Client Scripts").GetComponent<HudOn>().updateGameOver();
    }

    [RPC]
    public void updateCharacterNum(int c)
    {
        characterNum = c;

    }

    public void initLivesServer(int count)
    {
        lives = count;
        int c = GameObject.FindGameObjectsWithTag("Player").Length+1;
        remainingUni = new bool[c];

		// CHANGED TO 1 TO REDUCE GAME TIME FROM A BILLION YEARS TO ONE YEAR
        for (int i = 1; i < 2; i++) remainingUni[i] = true;
            Debug.Log("Start lives " + lives);
        networkView.RPC("initLivesClient", RPCMode.Others, lives);

    }

    [RPC]
    void restrictUniverses(int universe, int player)
    {
        if(player == characterNum)
        {
            PlayerMovement move = transform.GetComponent<PlayerMovement>();
            Debug.Log("Change to uni " + universe);
            move.changeUniverse(universe);
            move.networkView.RPC("AnimateWarp", RPCMode.All, player);

        }
    }

    [RPC]
    void initLivesClient(int count)
    {
        lives = count;
        int c = GameObject.FindGameObjectsWithTag("Player").Length + 1;
        remainingUni = new bool[c];
        for (int i = 1; i < c; i++) remainingUni[i] = true;
        GameObject.Find("Client Scripts").GetComponent<HudOn>().startLives(lives);
    }

	[RPC]
    void updateLives(int count, string s)
    {
        lives = count;
        loser = s;
        Debug.Log("Current lives " + lives);
        GameObject.Find("Client Scripts").GetComponent<HudOn>().updateLives(lives, loser);
    }
	
    [RPC]
    void instantiatePlayerNames(int count)
    {
        playerNames = new string[count];
    }

	[RPC]
	void instantiatePlayerFlags(int count)
	{
		playerFlags = new string[count];
	}

    [RPC]
    void updatePlayerNameC(int pos, string s)
    {
        playerNames[pos] = s;
    }

	[RPC]
    void updatePlayerFlagC(int pos, string s)
    {
        playerFlags[pos] = s;
    }

    [RPC]
    void updateEnergy(float e)
    {
        energyLevel = e;
    }

    [RPC]
    void updateWepDrain(int e)
    {
        selectedWepDrain = e;
    }

    [RPC]
    void updateHitP(float e)
    {
        hitPoints = e;
    }

    [RPC]
    void updateStartEnergy(float e)
    {
        startEnergy = e;
    }
	
	[RPC]
    void updateStartHP(float e)
    {
        startHP = e;
    }

    [RPC]
    void updatePlayerName(string name)
    {
        myPlayerName = name;
        if (Network.isClient)
        {
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<HudOn>().updateName(myPlayerName);
        }
    }

	[RPC]
    void updatePlayerFlag(string pflag)
    {
        flagname = pflag;
    }

    [RPC]
    void updatePlayerScore(int s)
    {
        if (Network.isClient)
        {
            score = s;
        }
    }
}
