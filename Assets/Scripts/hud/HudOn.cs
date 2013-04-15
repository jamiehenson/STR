// HUD generation script
// J. Henson
// 21/11/2012

using UnityEngine;
using System.Collections;

public class HudOn : MonoBehaviour {
	private Texture2D main, speed, universe, flag, wepBox1, wepBox2, wepBox3, crossTex, leaderboard;
	private Font deco;
	private string charName, wepName, gearReady;
	public string[] systemNames = new string[4];
	private float hitPoints, energyLevel, energyBank, startHP, startEnergy;
	public int wepType, bankSize;
	private int hudBarSize = 150, playercount = 4;
	private GameObject toast;
	private GUIStyle health = new GUIStyle();
	private GUIStyle energy = new GUIStyle();
	private GUIStyle bank = new GUIStyle();
    WeaponHandler weaponHandler;
    PlayerManager manager;
	OnlineClient onlineClient;
	
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

    // This seems a logical place to keep track of the score
   // public float score = 0;
   // public static bool gameOver = false;
      public static float score;
      public static bool gameOver;
	
	public static Texture2D fillTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width*height];

        for(int i = 0; i < pix.Length; i++) pix[i] = col; 

        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
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
		
	IEnumerator headOut() {
		EndGame.endIndividualScore = manager.getScore();
		iTween.CameraFadeAdd();
		iTween.CameraFadeTo(1f, 2f);
		yield return new WaitForSeconds(2);
        Application.LoadLevel ("endgame");
	}

    IEnumerator KeepScore() {
        while (!gameOver) {
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
        manager = GameObject.Find("Character" + universeN()).GetComponent<PlayerManager>();
        if (gameOver)
        {
            StartCoroutine("headOut");
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
            Debug.Log("Input Change Weapon");
        }
        else if (Input.GetKeyDown("3")) setWeapon(3);
        //else if (Input.GetAxis("Mouse ScrollWheel") < 0) setWeapon(WeaponHandler.wepType - 1);
        //else if (Input.GetAxis("Mouse ScrollWheel") > 0) setWeapon(WeaponHandler.wepType + 1);

        if (energyBank / (bankSize / hudBarSize) >= hudBarSize || true) // Always true, for testing
        {
            manager.resetEnergyBank(manager.getBankSize());
            gearReady = "WARP DRIVE READY!";
            PlayerManager.bankFull = true;
            if (Input.GetKeyDown("space"))
            {
                manager.resetEnergyBank(0);
                PlayerManager.bankFull = false;
                gearReady = "";

                // Destroy existing vortices
                GameObject[] vortices = GameObject.FindGameObjectsWithTag("vortex");
                foreach (GameObject existingVortex in vortices) StartCoroutine(Vortex.shrink(existingVortex));

                GameObject vortex = (GameObject)Resources.Load("Player/vortex");
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

                // Make n-1 new ones
                for (int i = 0; i < playercount - 1; i++)
                {
                    float x = xvals[i];
                    float y = yvals[i];
                    Vector3 vortpoint = new Vector3(x, y, 15);
                    Vector3 vort = Camera.main.ViewportToWorldPoint(vortpoint);
                    GameObject obj = (GameObject)Instantiate(vortex, vort, Quaternion.identity);
                    vortex.name = "vortex" + (i + 1);
					obj.GetComponentInChildren<Vortex>().leadsToUniverse =
						(i + 1 >= currentUniverse) ? i+2 : i+1;
					print("Just made vortext for "+obj.GetComponentInChildren<Vortex>().leadsToUniverse);
                    vortex.transform.rotation = Quaternion.AngleAxis(270, Vector3.up);
                    vortex.tag = "vortex";
                }
            }
        }
	}

    public void ToastWrapper(string notetext) {
        StartCoroutine("Toast", notetext);
    }

	IEnumerator Toast(string notetext) {
		toast = new GameObject("Toast");
		toast.AddComponent("GUIText");
		toast.guiText.font = (Font) Resources.Load ("Belgrad");
		toast.guiText.fontSize = (Screen.width > 1000) ? 40 : 24;
		toast.transform.position = new Vector3(0.5f,0.5f,0);
		toast.guiText.anchor = TextAnchor.MiddleCenter;
		toast.guiText.text = notetext;
		toast.guiText.material.color = Color.white;
		iTween.FadeTo(toast,0f,0.01f);
		yield return new WaitForSeconds(2);
		iTween.FadeTo(toast,1f,1f);
		yield return new WaitForSeconds(4);
		iTween.FadeTo(toast,0f,1f);
		yield return new WaitForSeconds(1);
		Destroy(toast);
	}

    private int universeN()
    {
        int length = transform.name.Length;
        string num = transform.name.Substring(length - 1, 1);
        if ("0123456789".Contains(num)) return (int.Parse(num));
        else return -1;
    }

    /* Can't use it and moved all its contents to Start() as InitialiseStats() is not static anymore
    void Awake() {
        if (universeN() != -1)
        {
            manager = GameObject.Find("Character" + universeN()).GetComponent<PlayerManager>();
            if (manager.activeChar == null) manager.activeChar = "tester";
            Debug.Log("Hud on" + manager.activeChar);
            manager.InitialiseStats();
            StartScore();
        }
    }
    Not being used. Not sure if necessary*/

	void Start () {
		// Set statics
		score = 0;
		gameOver = false;
		Instance = this;

		for (int i = 0; i < 4; i++) networkView.RPC("setSystemName",RPCMode.AllBuffered,i,generateSystemNames());

        manager = GameObject.Find("Character" + universeN()).GetComponent<PlayerManager>();
		onlineClient = GameObject.Find ("Network").GetComponent<OnlineClient>();

        /* Was in Awake() */
        if (manager.activeCharN == null) manager.activeCharN = "tester";
        Debug.Log("Hud on" + manager.activeCharN);
        manager.InitialiseStats();
        StartScore();
        /* Was in Awake() */

		iTween.CameraFadeAdd();
		iTween.CameraFadeFrom(1.0f, 2.0f);

        StartCoroutine(Toast("SURVIVE THE ENEMY ONSLAUGHT"));

		wepBox1 = (Texture2D) Resources.Load ("hud/wepBox1Off");
		wepBox2 = (Texture2D) Resources.Load ("hud/wepBox2Off");
		wepBox3 = (Texture2D) Resources.Load ("hud/wepBox3Off");
        flag = (Texture2D)Resources.Load("hud/" + PlayerManager.activeChar);
		
        charName = MP.playerName;

        if (Network.isClient)
        {
            int PlayerNumber = int.Parse(GameObject.FindGameObjectWithTag("MainCamera").name.Substring(GameObject.FindGameObjectWithTag("MainCamera").name.Length - 1, 1));
            weaponHandler = GameObject.Find("Character" + PlayerNumber).GetComponent<WeaponHandler>();
            setWeapon(1);
        }

		if (Network.isServer)
		{

		}

        startHP = manager.getStartHP();
        startEnergy = manager.getStartEnergy();
        bankSize = manager.getBankSize();

		health.normal.background = fillTex(1,1,new Color(0.8f,0f,0f,1f));
		energy.normal.background = fillTex(1,1,new Color(0f,0f,0.8f,1f));
		bank.normal.background = fillTex (1,1,new Color(0f,0.8f,0f,1f));

	}

	void OnGUI () {
		main = (Texture2D) Resources.Load ("hud/topleft");
		speed = (Texture2D) Resources.Load ("hud/topright");
		leaderboard = (Texture2D) Resources.Load ("hud/leaderboard");
		universe = (Texture2D) Resources.Load ("hud/bottomleft");
		
		deco = (Font) Resources.Load ("Belgrad");
		
		GUI.Label (new Rect (-130,-20,main.width,main.height), main);
		GUI.Label (new Rect (Screen.width-speed.width+15,-20,speed.width,speed.height), speed);
		GUI.Label (new Rect (Screen.width-leaderboard.width+80,Screen.height/2-leaderboard.height/2,leaderboard.width,leaderboard.height), leaderboard);
		GUI.Label (new Rect (0,0,64,64),flag);

		GUIStyle hudStyle = new GUIStyle();
    	hudStyle.font = deco;
		hudStyle.normal.textColor = Color.white;
		hudStyle.fontStyle = FontStyle.Bold;
		hudStyle.fontSize = 20;
		
		GUIStyle coStyle = new GUIStyle();
    	coStyle.font = deco;
		coStyle.normal.textColor = Color.white;
		coStyle.fontStyle = FontStyle.Italic;
		coStyle.fontSize = 12;
		
		GUIStyle speedStyle = new GUIStyle();
    	speedStyle.font = deco;
		speedStyle.normal.textColor = Color.white;
		//speedStyle.fontSize = 72;
        speedStyle.fontSize = 38;
		
		GUIStyle wepStyle = new GUIStyle();
    	wepStyle.font = deco;
		wepStyle.normal.textColor = Color.white;
		wepStyle.fontSize = 12;
		
		GUIStyle smallStyle = new GUIStyle();
    	smallStyle.font = deco;
		smallStyle.normal.textColor = Color.white;
		smallStyle.fontSize = 11;
		smallStyle.alignment = TextAnchor.MiddleRight;

		// Universe (or rather, star system) name
		int uniNo = manager.universeNumber;
		GUI.Label (new Rect (-5,Screen.height-universe.height/2,universe.width,universe.height),universe);
		GUI.Label (new Rect (5,Screen.height-universe.height/2+15,200,50),"LOCATION:",coStyle);
		GUI.Label (new Rect (5,Screen.height-universe.height/2+30,200,50),systemNames[uniNo],speedStyle);

		GUI.Label (new Rect (70,5,200,50),charName,hudStyle);
		GUI.Label (new Rect (75,21,40,20),hullTitle,smallStyle);
		GUI.Label (new Rect (77,31,40,20),energyTitle,smallStyle);
		GUI.Label (new Rect (75,41,40,20),bankTitle,smallStyle);
		
		// Health bar
		GUI.Label (new Rect (115,25,hitPoints/(startHP/hudBarSize),10),"",health);
		
		// Energy bar
		GUI.Label (new Rect (115,35,energyLevel/(startEnergy/hudBarSize),10),"",energy);
		
		// Power bank
		GUI.Label (new Rect (115,45,energyBank/(bankSize/hudBarSize),10),"",bank);
		
		// Speed and gear indicator
        GUI.Label (new Rect (Screen.width - 160, 10, 200, 50), "" + manager.getScore(), speedStyle);
		GUI.Label (new Rect (Screen.width-240,100,200,40),gearReady,hudStyle);
		
		// Scoreboard indicator
		GUI.Label (new Rect (Screen.width-120,Screen.height/2-leaderboard.height/2+20,200,40),"TEAM SCORES",coStyle);
        for (int i = 1; i <= 4; i++)
        {
            PlayerManager score = GameObject.Find("Character" + i).GetComponent<PlayerManager>();
            GUI.Label(new Rect(Screen.width - 120, Screen.height / 2 - leaderboard.height / 2 + 20 + i*20, 40, 40), score.playerNames[i] + " :" +score.getScore() , coStyle);
        }
		
		// Weapons initialisation
        int wepBoxSize = 48;
		GUI.Label (new Rect (-10,10,wepBoxSize,wepBoxSize), wepBox1);
		GUI.Label (new Rect (7,10,wepBoxSize,wepBoxSize), wepBox2);
        GUI.Label(new Rect(24,10,wepBoxSize,wepBoxSize), wepBox3);
		GUI.Label (new Rect (7,47,200,64), wepName, wepStyle);

        // Add a crosshair
        if (PlayerManager.activeChar == "china") crossTex = (Texture2D)Resources.Load("hud/crossChi");
        else if (PlayerManager.activeChar == "usa") crossTex = (Texture2D)Resources.Load("hud/crossUSA");
        else crossTex = (Texture2D)Resources.Load("hud/crossRus");
        Rect position = new Rect(Input.mousePosition.x - (crossTex.width / 2), (Screen.height - Input.mousePosition.y) - (crossTex.height / 2), crossTex.width, crossTex.height);
        GUI.DrawTexture(position, crossTex);
        Screen.showCursor = false;
	}	
	
	// Vortex logic
	IEnumerator VortexCountdown() {
		// Do GUI magic here!
		while (vortexCountdownNum != 0) {
			print ("In vortex: "+vortexCountdownNum);
			vortexCountdownNum--;
			yield return new WaitForSeconds(1);
		}
		
		manager.movement.changeUniverse(vortexLeadsTo);
	}
	
	public void enteredVortex(int vortexTo) {
		print ("LEADS TO "+vortexTo);
		print("Entered Vortex");
		
		if (inVortexCountdown){
			StopCoroutine("VortexCountdown");
			inVortexCountdown = false;
		}
		
		vortexCountdownNum = 4;
		vortexLeadsTo = vortexTo;
		StartCoroutine("VortexCountdown");		
	}
	
	public void leftVortex() {
		print("Left Vortex");
		
		if (inVortexCountdown){
			StopCoroutine("VortexCountdown");
			inVortexCountdown = false;
		}
	}

	private string generateSystemNames()
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

	[RPC]
	private void setSystemName(int i, string name)
	{
		systemNames[i] = name;
	}
}
