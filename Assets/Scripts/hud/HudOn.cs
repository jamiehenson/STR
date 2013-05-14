// HUD generation script
// J. Henson
// 21/11/2012

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HudOn : MonoBehaviour {
    public Rect position;

	private Texture2D main, speed, universe, flag, wepBox1, wepBox2, wepBox3, crossTex, leaderboard, bossbar, bossthumb, helpPic;
	private Font deco;
	private string charName, wepName, gearReady, bossname;

	public string[] systemNames;

	private float hitPoints, energyLevel, energyBank, startHP, startEnergy, bossHealth;
    public static int lives, currentLives;
	public int wepType, bankSize;
	private int hudBarSize = 150, playercount;

    public bool canWarp = true;

	private string[] allNames;
	private int[] allScores;

	private GameObject[] vortexRegister;
	private GUIStyle health = new GUIStyle();
	private GUIStyle energy = new GUIStyle();
	private GUIStyle bank = new GUIStyle();
    public int startLivesNb;

	private int countdownVal = 6;

	public static Vector3 vortpointOut;
	private bool showCountdown, showHelp;
	public static float score;
	public static bool gameOver, bossOn;
	private static bool gameOverBeenDetected;

    public static int countUniverse;
    PlayerManager manager = null;

	// Toasts
	private GameObject toast, helppic;//, charModel;
	private Queue<string> queuedToastMessages;
	private int toastCountdown;
	
	public static HudOn Instance; // Singleton var so vortex can access (Is there a better method?)
	
	bool inVortexCountdown = false;
	int vortexCountdownNum;
	int vortexLeadsTo;

	private string beamTitle = "BEAM", 
		cannonTitle = "CANNON", 
		mineTitle = "SPECIAL", 
		hullTitle = "HULL", 
		energyTitle = "ENERGY", 
		bankTitle = "WARP";

	public static Texture2D fillTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width*height];

        for(int i = 0; i < pix.Length; i++) pix[i] = col; 

        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }

    public void updateGameOver()
    {
        gameOver = true;
    }

    public void updateLives(int c, string s)
    {
        lives = c;
		string[] dieToast = {" K.I.A. "," TOOK IT LIKE A MAN. "," BITES THE DUST. "," DIED LIKE A SPARTAN. "," FAILED AT LIFE "," IS A GONER "," SEGFAULTED "," IS A HPC CADET "};
		string dieText = dieToast[Random.Range (0,dieToast.Length)];
        ToastWrapper(s + dieText + "\n" + lives + " LIVES REMAINING");
    }

    public void startLives(int c)
    {
        lives = c;
        startLivesNb = lives;
        ToastWrapper("YOUR TEAM HAS " + lives + " LIVES LEFT");
    }
    public void updateName(string s)
    {
        charName = s;
    }

	void setWeapon(int type)
	{
		if (type == 1 || type == 4) 
		{
			wepBox1 = (Texture2D) Resources.Load ("hud/wepBox1On");
			wepBox2 = (Texture2D) Resources.Load ("hud/wepBox2Off");
			wepBox3 = (Texture2D) Resources.Load ("hud/wepBox3Off");
			wepName = beamTitle;
			manager.changeWeapon(1);
		}
		
		else if (type == 2) 
		{
            Debug.Log("Change weapon");
			wepBox1 = (Texture2D) Resources.Load ("hud/wepBox1Off");
			wepBox2 = (Texture2D) Resources.Load ("hud/wepBox2On");
			wepBox3 = (Texture2D) Resources.Load ("hud/wepBox3Off");
			wepName = cannonTitle;
			manager.changeWeapon(2);
		}
		
		else if (type == 3 || type == 0) 
		{
			wepBox1 = (Texture2D) Resources.Load ("hud/wepBox1Off");
			wepBox2 = (Texture2D) Resources.Load ("hud/wepBox2Off");
			wepBox3 = (Texture2D) Resources.Load ("hud/wepBox3On");
			wepName = mineTitle;
			manager.changeWeapon(3);
		}	
	}
		
	IEnumerator headOut()
	{
		EndGame.scores = allScores;
		EndGame.names = allNames;
		iTween.CameraFadeAdd();
		iTween.CameraFadeTo(1f, 2f);
		yield return new WaitForSeconds(2);
        Application.LoadLevel ("endgame");
	}

    IEnumerator KeepScore()
	{
        while (!gameOver)
		{
            if(Network.isServer) manager.updateScore(1);
            //score += 1;
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void StopScore() {
        StopCoroutine("KeepScore");
    }

    public void StartScore() {
        StartCoroutine("KeepScore");
    }
	
	void Update() {
		// Check if names have been sent from the server. If not then we canot
		// determin our character, so stop.
		if (manager == null)
			return;

		// The headOut coroutine should only be called once, so check if it
		// needs to be called and hasn't already
        if (gameOver && !gameOverBeenDetected){
            StartCoroutine("headOut");
			gameOverBeenDetected = true;
        }

        // Update player stats
        hitPoints = manager.getHitPoints();
        energyLevel = manager.getEnergyLevel();
        energyBank = manager.getEnergyBank();

        // Weapon changing
        if (Input.GetKeyDown("1")) setWeapon(1);
        else if (Input.GetKeyDown("2"))
        {
            setWeapon(2);
        }
        else if (Input.GetKeyDown("3")) setWeapon(3);

        if (energyBank / (bankSize / hudBarSize) >= hudBarSize || true) // Always true, for testing
        {
			manager.resetEnergyBank(manager.getBankSize());
            gearReady = "WARP DRIVE READY!";
            PlayerManager.bankFull = true;
            if (Input.GetKeyDown("space") && canWarp)
            {
				stopVortices(); // Kill the current vortices

                manager.resetEnergyBank(0);
                PlayerManager.bankFull = false;
                gearReady = "";

                // Destroy existing vortices
                GameObject[] vortices = GameObject.FindGameObjectsWithTag("vortex");
                foreach (GameObject existingVortex in vortices)
				{
					Vortex.labelIsSet = false;
					StartCoroutine(Vortex.shrink(existingVortex));
				}

                GameObject vortex = (GameObject)Resources.Load("Player/vortex");

                if (playercount != 1) {
                    VortexLaunch(vortex);
                }
            }
        }
	}

    private void VortexLaunch(GameObject vortex) {
        float[] xvals = new float[playercount - 1];
        float[] yvals = new float[playercount - 1];

        float chunkX = (float)0.5f / (playercount - 1);
        float chunkY = (float)0.8f / (playercount - 1);
        for (int i = 0; i < playercount - 1; i++)
        {
            xvals[i] = Random.Range(0 + (i * chunkX), (i + 1) * chunkX);
            yvals[i] = Random.Range(0 + (i * chunkY), (i + 1) * chunkY);
        }

        for (var i = (playercount - 2); i > 0; i--)
        {
            int t = Random.Range(0, i);
            float temp = yvals[i];
            yvals[i] = yvals[t];
            yvals[t] = temp;
        }
				
		int currentUniverse = manager.universeNumber;
		print("Manager says current universe is: "+currentUniverse);
        // Make n-1 new ones
        for (int i = 0; i < playercount - 1; i++)
        {
            float x = xvals[i];
            float y = yvals[i];
            Vector3 vortpoint = new Vector3(x, y, 15);
            Vector3 vort = Camera.main.ViewportToWorldPoint(vortpoint);
            GameObject obj = (GameObject)Instantiate(vortex, vort, Quaternion.identity);
            vortex.name = "vortex" + (i + 1);
			Vortex vortexScript = obj.GetComponentInChildren<Vortex>();
			vortexScript.leadsToUniverse = (i + 1 >= currentUniverse) ? i+2 : i+1;
			vortexScript.inUniverse = currentUniverse;
			vortexScript.setLabel(vortpoint,systemNames[vortexScript.leadsToUniverse-1]);
			print("Just made vortext for "+obj.GetComponentInChildren<Vortex>().leadsToUniverse);
            vortex.transform.rotation = Quaternion.AngleAxis(270, Vector3.up);
            vortex.tag = "vortex";
        }
    }

	public void DisplayCountdown(int num) {
		// Switch here, show desired image
		countdownVal = num;
	}

    public void ToastWrapper(string notetext) {
        //StartCoroutine("Toast", notetext);
		queuedToastMessages.Enqueue(notetext);
    }

	IEnumerator Toast() {
		while (true) {
			if (queuedToastMessages.Count == 0)
				yield return new WaitForSeconds(1/15);
			else {
				// Display it
				string notetext = queuedToastMessages.Dequeue();
				toast = new GameObject("Toast");
				toast.AddComponent("GUIText");
				toast.guiText.font = (Font) Resources.Load ("Belgrad");
				toast.guiText.fontSize = (Screen.width > 1000) ? 40 : 24;
				toast.transform.position = new Vector3(0.5f,0.5f,0);
				toast.guiText.anchor = TextAnchor.MiddleCenter;
				toast.guiText.text = notetext;
				toast.guiText.material.color = Color.white;
				yield return new WaitForSeconds(4);
				iTween.FadeTo(toast,0f,1f);
				yield return new WaitForSeconds(1);
				Destroy(toast);
			}
		}
	}

    private int universeN()
    {
		PlayerManager manager = PlayerManager.Instance;

		if (manager == null)
			return -1;
		else
			return manager.universeNumber;
    }


	void Start () {
		// Set statics
		score = 0;
		gameOver = false;
		Instance = this;

		generateBossName();

		gameOverBeenDetected = false;
		main = (Texture2D) Resources.Load ("hud/topleft");
		speed = (Texture2D) Resources.Load ("hud/topright");
		leaderboard = (Texture2D) Resources.Load ("hud/leaderboard");
		universe = (Texture2D) Resources.Load ("hud/bottomleft");
		deco = (Font) Resources.Load ("Belgrad");
		bossbar = (Texture2D) Resources.Load ("hud/bossbar");

		health.normal.background = fillTex(1,1,new Color(0.8f,0f,0f,1f));
		energy.normal.background = fillTex(1,1,new Color(0f,0f,0.8f,1f));
		bank.normal.background = fillTex (1,1,new Color(0f,0.8f,0f,1f));

		gameOverBeenDetected = false;

		helpPic = (Texture2D) Resources.Load ("menu/howtocut");

		queuedToastMessages = new Queue<string>();
		StartCoroutine("Toast");

		showHelp = true;
	}

	void startWithManager() {		
        playercount = GameObject.FindGameObjectsWithTag("Player").Length;

		allNames = new string[playercount];
		allScores = new int[playercount];

		print ("I think universeN() = "+universeN() );
        manager = GameObject.Find("Character" + manager.universeNumber).GetComponent<PlayerManager>();
      
        if (manager.activeCharN == null) manager.activeCharN = "tester";
        Debug.Log("Hud on" + manager.activeCharN);
        manager.InitialiseStats();
        StartScore();

		iTween.CameraFadeAdd();
		iTween.CameraFadeFrom(1.0f, 2.0f);

        ToastWrapper("SURVIVE THE ENEMY ONSLAUGHT");

        if (PlayerManager.activeChar == "china") crossTex = (Texture2D)Resources.Load("hud/crossChi");
        else if (PlayerManager.activeChar == "usa") crossTex = (Texture2D)Resources.Load("hud/crossUSA");
        else crossTex = (Texture2D)Resources.Load("hud/crossRus");

		wepBox1 = (Texture2D) Resources.Load ("hud/wepBox1Off");
		wepBox2 = (Texture2D) Resources.Load ("hud/wepBox2Off");
		wepBox3 = (Texture2D) Resources.Load ("hud/wepBox3Off");
		flag = (Texture2D) Resources.Load ("menu/flags/"+manager.getFlag());
        charName = MP.playerName;

        if (Network.isClient)
        {
            setWeapon(1);
        }

        startHP = manager.getStartHP();
        startEnergy = manager.getStartEnergy();
        bankSize = manager.getBankSize();
        lives = manager.getLives();

        charName = manager.getPlayerName();
	}

	void OnGUI () {
		main = (Texture2D) Resources.Load ("hud/topleft");
		speed = (Texture2D) Resources.Load ("hud/topright");
		leaderboard = (Texture2D) Resources.Load ("hud/leaderboard");
		universe = (Texture2D) Resources.Load ("hud/bottomleft");
		
		deco = (Font) Resources.Load ("Belgrad");

		GUI.Label (new Rect (-130,-20,main.width,main.height), main);
		GUI.Label (new Rect (Screen.width-speed.width+15,-10,speed.width,speed.height), speed);
		GUI.Label (new Rect (Screen.width-leaderboard.width+90,Screen.height/2-leaderboard.height/2,leaderboard.width,leaderboard.height), leaderboard);
		GUI.DrawTexture (new Rect (2,-2,64,48),flag,ScaleMode.StretchToFill);

		GUIStyle hudStyle = new GUIStyle();
    	hudStyle.font = deco;
		hudStyle.normal.textColor = Color.white;
		hudStyle.fontStyle = FontStyle.Bold;
		hudStyle.fontSize = 20;

		GUIStyle barStyle = new GUIStyle();
    	barStyle.font = deco;
		barStyle.normal.textColor = Color.white;
		barStyle.fontStyle = FontStyle.Bold;
		//barStyle.alignment = TextAnchor.MiddleRight;
		barStyle.fontSize = Screen.height/26;
		barStyle.fixedWidth = Screen.width/12;
		
		GUIStyle coStyle = new GUIStyle();
    	coStyle.font = deco;
		coStyle.normal.textColor = Color.white;
		coStyle.fontStyle = FontStyle.Italic;
		coStyle.fontSize = 14;

		GUIStyle coStyle2 = new GUIStyle();
    	coStyle2.font = deco;
		coStyle2.normal.textColor = Color.white;
		coStyle2.fontStyle = FontStyle.Italic;
		coStyle2.fontSize = 12;
		
		GUIStyle speedStyle = new GUIStyle();
    	speedStyle.font = deco;
		speedStyle.normal.textColor = Color.white;
        speedStyle.fontSize = 34;

		GUIStyle largeStyle = new GUIStyle();
    	largeStyle.font = deco;
		largeStyle.normal.textColor = Color.white;
        largeStyle.fontSize = 40;

		GUIStyle wepStyle = new GUIStyle();
    	wepStyle.font = deco;
		wepStyle.normal.textColor = Color.white;
		wepStyle.fontSize = 12;
		
		GUIStyle smallStyle = new GUIStyle();
    	smallStyle.font = deco;
		smallStyle.normal.textColor = Color.white;
		smallStyle.fontSize = 11;
		smallStyle.alignment = TextAnchor.MiddleRight;

		// In Boss section

		// THIS IS FOR WHEN A BOSS ARRIVES
		if (bossOn)
		{
			float thumbSize = Screen.width/28;
            GUI.DrawTexture(new Rect(Screen.width / 4 + 22, 0, Screen.width / 2, Screen.height / 8), bossbar);
            GUI.DrawTexture(new Rect(Screen.width / 4 + 36, Screen.height / 38, thumbSize, thumbSize), bossthumb);
            GUI.Label(new Rect(Screen.width / 4 + 36 + thumbSize, Screen.height / 24, Screen.width / 2, Screen.height / 8), bossname, barStyle);
            GUI.Label(new Rect(Screen.width / 2.75f + 18 + thumbSize, Screen.height / 30, (bossHealth * Screen.width / 3), Screen.height / 16), "", health);
            //GUI.DrawTexture(new Rect(Screen.width/4, 20, Screen.width/2, Screen.height/8), bossbar);
            //GUI.DrawTexture(new Rect(Screen.width/4 + 14, 18 + Screen.height/38, thumbSize, thumbSize), bossthumb);
            //GUI.Label(new Rect(Screen.width/4 + 18 + thumbSize, 22 + Screen.height/24, Screen.width/2, Screen.height/8), bossname, barStyle);
            //GUI.Label (new Rect (Screen.width/2.75f + thumbSize, 20 + Screen.height/30, (bossHealth*Screen.width/3),Screen.height/16),"", health);
		}

		// Universe (or rather, star system) name
		int uniNo = manager.universeNumber;
		
		GUI.Label (new Rect (-5,Screen.height-universe.height/2,universe.width,universe.height),universe);
		GUI.Label (new Rect (6,Screen.height-universe.height/2+14,200,50),"LOCATION:",coStyle);

		if (manager != null && uniNo != 0)
			GUI.Label (new Rect (10,Screen.height-universe.height/2+30,200,50),systemNames[uniNo-1],speedStyle);
		else
			GUI.Label (new Rect (10,Screen.height-universe.height/2+30,200,50),"MORT",speedStyle);

		GUI.Label (new Rect (70,5,200,50),charName,hudStyle);
		GUI.Label (new Rect (75,21,40,20),hullTitle,smallStyle);
		GUI.Label (new Rect (77,31,40,20),energyTitle,smallStyle);
		GUI.Label (new Rect (75,41,40,20),bankTitle,smallStyle);
		
		// Health bar
		GUI.Label (new Rect (115,25,hitPoints/(startHP/hudBarSize),10),"",health);
		
		// Energy bar
		GUI.Label (new Rect (115,35,energyLevel/(startEnergy/hudBarSize),10),"",energy);
		
		// Power bank
        GUI.Label(new Rect(115, 45, energyBank / (bankSize / hudBarSize), 10), "", bank);

		// Score, lives and warp indicator
        GUI.Label (new Rect (Screen.width - 165, 10, 200, 50), "" + manager.getScore(), largeStyle);
		GUI.Label (new Rect (Screen.width - 155, 50, 200, 50), "TEAM LIVES:" +  lives.ToString(), coStyle);
		GUI.Label (new Rect (Screen.width - 240,100,200,40),gearReady,hudStyle);
		
		// Scoreboard indicator
		GUI.Label (new Rect (Screen.width-155,Screen.height/2-leaderboard.height/2+25,200,40),"TEAM SCORES",hudStyle);
		
        for (int i = 1; i <= playercount; i++)
        {
			int spacer = 35;
			PlayerManager score = GameObject.Find("Character" + i).GetComponent<PlayerManager>();
			OnlineClient cli = GameObject.Find ("Client Scripts").GetComponent< OnlineClient>();
			Texture2D playerFlag = (Texture2D) Resources.Load ("menu/flags/"+score.playerFlags[i]);
            GUI.Label(new Rect(Screen.width - 145, Screen.height / 2 - leaderboard.height / 2 + 16 + i*spacer, 50, 30), score.playerNames[i] + ":"  + score.getScore(), coStyle);
            GUI.Label(new Rect(Screen.width - 180, Screen.height / 2 - leaderboard.height / 2 + 10 + i*spacer, 35, 35), playerFlag);
			GUI.Label(new Rect(Screen.width - 145, Screen.height / 2 - leaderboard.height / 2 + 30 + i*spacer, 100, 35), systemNames[cli.universeNum-1], coStyle2);
			allScores[i-1] = score.getScore();
			allNames[i-1] = score.playerNames[i];
        }
		
		// Weapons initialisation
        int wepBoxSize = 48;
		GUI.Label (new Rect (-10,10,wepBoxSize,wepBoxSize), wepBox1);
		GUI.Label (new Rect (10,10,wepBoxSize,wepBoxSize), wepBox2);
        GUI.Label(new Rect(30,10,wepBoxSize,wepBoxSize), wepBox3);
		GUI.Label (new Rect (7,47,200,64), wepName, wepStyle);

        // Add a crosshair
        position = new Rect(Input.mousePosition.x - (crossTex.width / 2), (Screen.height - Input.mousePosition.y) - (crossTex.height / 2), crossTex.width, crossTex.height);
        GUI.DrawTexture(position, crossTex);
        Screen.showCursor = false;

		// Show vortex timer
		if (showCountdown)
		{
			Vector3 screenPoint = Camera.main.WorldToScreenPoint(vortpointOut);
			screenPoint.y = (Screen.height/2 - (screenPoint.y - Screen.height/2)); // Flip y about center line (lord knows why)
			//int x = 100;
			//int y = 100;

			// Counter style - yes, I included another style here, shoot me
			GUIStyle style = new GUIStyle();
	    	style.font = deco;
			style.normal.textColor = Color.white;
			style.alignment = TextAnchor.MiddleCenter;
			style.fontStyle = FontStyle.Bold;
			style.fontSize = 70;

			GUI.Label(new Rect(screenPoint.x,screenPoint.y,10,10), vortexCountdownNum.ToString(), style);
		}

		if (showHelp) GUI.DrawTexture(new Rect(Screen.width*0.05f,Screen.height*0.05f,Screen.height*0.9f*1.72f,Screen.height*0.9f),helpPic);

		if (countdownVal > 0)
		{
			Texture2D number = (Texture2D) Resources.Load ("hud/numbers/"+countdownVal);
			GUI.DrawTexture(new Rect(Screen.width/2-128,Screen.height/2-128,256,256),number);
		}
		else
		{
			showHelp = false;
		}
	}
	
	// Vortex logic
	IEnumerator VortexCountdown()
	{
		showCountdown = true;
		while (vortexCountdownNum != 0)
		{
			print ("In vortex: "+vortexCountdownNum);
			vortexCountdownNum--;
            // ANIMATE HERE AT 2
			yield return new WaitForSeconds(1);
		}
		stopVortices();
		manager.movement.changeUniverse(vortexLeadsTo);
		showCountdown = false;
	}

	public void enteredVortex(int vortexTo)
	{
		print ("LEADS TO "+vortexTo);
		print("Entered Vortex");
		
		if (inVortexCountdown)
		{
			StopCoroutine("VortexCountdown");
			StopCoroutine ("Vortex.playerGrow");
			inVortexCountdown = false;
		}
		
		vortexCountdownNum = 4;
		vortexLeadsTo = vortexTo;
		StartCoroutine("VortexCountdown");
	}

	public void leftVortex() {
		print("In leftVortex");
		StopCoroutine("VortexCountdown");

		Vortex.labelIsSet = false;
		inVortexCountdown = false;
		showCountdown = false;
	}

	public void setManager(PlayerManager m) {
		manager = m;
		startWithManager();
	}

	public void stopVortices() {
		leftVortex(); // Stop countdown

		GameObject[] vorties = GameObject.FindGameObjectsWithTag("vortex");
		foreach (GameObject vortex in vorties) {
			Destroy(vortex);
		}
	}

	public void generateBossName() {
		string[] bossnames = {"iBOSS","ADMIRAL","BIG DADDY","EYEBALL\nPAUL","MORTO","KEVIN","MORTEYE","iMORT","McBALL","POPEYE"};
		bossname = bossnames[Random.Range (0,bossnames.Length)];
		bossthumb = (Texture2D) Resources.Load ("hud/boss");
	}

    public void BossHealthUpdate(float bHealth) {
        bossHealth = bHealth;
    }
}
