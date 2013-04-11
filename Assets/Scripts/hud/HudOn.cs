// HUD generation script
// J. Henson
// 21/11/2012

using UnityEngine;
using System.Collections;

public class HudOn : MonoBehaviour {
	private Texture2D main, speed, flag, wepBox1, wepBox2, wepBox3, crossTex, leaderboard, coFlag;
	private Font deco;
	private string charName, coName, wepName, gearReady;
	private float hitPoints, energyLevel, energyBank, startHP, startEnergy;
	public int wepType, bankSize;
	private int hudBarSize = 150, playercount = 4;
	private GameObject toast;
	private GUIStyle health = new GUIStyle();
	private GUIStyle energy = new GUIStyle();
	private GUIStyle bank = new GUIStyle();
    WeaponHandler weaponHandler;
	
	private string beamTitle = "BEAM", 
		cannonTitle = "CANNON", 
		mineTitle = "SPECIAL", 
		hullTitle = "HULL", 
		energyTitle = "ENERGY", 
		bankTitle = "WARP";

    // This seems a logical place to keep track of the score
    public static float score = 0;
    public static bool gameOver = false;
	
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
			weaponHandler.wepType = 1;
		}
		
		else if (type == 2) 
		{
            Debug.Log("Change weapon");
			wepBox1 = (Texture2D) Resources.Load ("hud/wepBox1Off");
			wepBox2 = (Texture2D) Resources.Load ("hud/wepBox2On");
			wepBox3 = (Texture2D) Resources.Load ("hud/wepBox3Off");
			wepName = cannonTitle;
			weaponHandler.wepType = 2;
		}
		
		else if (type == 3 || type == 0) 
		{
			wepBox1 = (Texture2D) Resources.Load ("hud/wepBox1Off");
			wepBox2 = (Texture2D) Resources.Load ("hud/wepBox2Off");
			wepBox3 = (Texture2D) Resources.Load ("hud/wepBox3On");
			wepName = mineTitle;
			weaponHandler.wepType = 3;
		}	
	}
		
	IEnumerator headOut() {
		EndGame.endIndividualScore = (int) score;
		iTween.CameraFadeAdd();
		iTween.CameraFadeTo(1f, 2f);
		yield return new WaitForSeconds(2);
        Application.LoadLevel ("endgame");
	}

    IEnumerator KeepScore() {
        while (!gameOver) {
            score += 1;
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
		if (gameOver) {
			StartCoroutine("headOut");
		}

		// Update player stats
		hitPoints = PlayerManager.hitPoints;
		energyLevel = PlayerManager.energyLevel;
		energyBank = PlayerManager.energyBank;
		
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

        if (energyBank / (bankSize / hudBarSize) >= hudBarSize)
		{
            PlayerManager.energyBank = PlayerManager.bankSize;
			gearReady = "WARP DRIVE READY!";
			PlayerManager.bankFull = true;
			if (Input.GetKeyDown ("space"))
			{
                PlayerManager.energyBank = 0;
                PlayerManager.bankFull = false;
                gearReady = "";

                // Destroy existing vortices
                GameObject[] vortices = GameObject.FindGameObjectsWithTag("vortex");
                foreach (GameObject existingVortex in vortices) StartCoroutine(Vortex.shrink(existingVortex));

                GameObject vortex = (GameObject)Resources.Load("Player/vortex");
                float[] xvals = new float[playercount - 1];
                float[] yvals = new float[playercount - 1];
                float chunkX = (float) 0.5f / (playercount - 1);
                float chunkY = (float) 0.8f / (playercount - 1);
                for (int i = 0; i < playercount - 1; i++)
                {
                    xvals[i] = Random.Range(0 + (i * chunkX), (i + 1) * chunkX);
                    yvals[i] = Random.Range(0 + (i * chunkY), (i + 1) * chunkY);
                }

                for (var i = (playercount-2); i > 0; i--)
                {
                    int t = Random.Range(0, i);
                    float temp = yvals[i];
                    yvals[i] = yvals[t];
                    yvals[t] = temp;
                }

                // Make n-1 new ones
                for (int i = 0; i < playercount - 1; i++)
                {
                    float x = xvals[i];
                    float y = yvals[i];
                    Vector3 vortpoint = new Vector3(x, y, 25);
                    Vector3 vort = Camera.main.ViewportToWorldPoint(vortpoint);
                    Instantiate(vortex, vort, Quaternion.identity);
                    vortex.name = "vortex" + (i + 1);
                    vortex.transform.rotation = Quaternion.AngleAxis(270, Vector3.up);
                    vortex.tag = "vortex";
                }
			}
		}
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

    void Awake() {
        if (PlayerManager.activeChar == null) PlayerManager.activeChar = "tester";
        Debug.Log("Hud on" + PlayerManager.activeChar);
        PlayerManager.InitialiseStats();
        StartScore();
    }

	
	void Start () {
		iTween.CameraFadeAdd();
		iTween.CameraFadeFrom(1.0f, 2.0f);

        StartCoroutine(Toast("SURVIVE THE ENEMY ONSLAUGHT"));

		wepBox1 = (Texture2D) Resources.Load ("hud/wepBox1Off");
		wepBox2 = (Texture2D) Resources.Load ("hud/wepBox2Off");
		wepBox3 = (Texture2D) Resources.Load ("hud/wepBox3Off");
        flag = (Texture2D)Resources.Load("hud/" + PlayerManager.activeChar);
		
        charName = MP.playerName;
		coName = SP.coPilotName;
		coFlag = SP.coPilotFlag;

        if (Network.isClient)
        {
            int PlayerNumber = int.Parse(GameObject.FindGameObjectWithTag("MainCamera").name.Substring(GameObject.FindGameObjectWithTag("MainCamera").name.Length - 1, 1));
            weaponHandler = GameObject.Find("Character" + PlayerNumber).GetComponent<WeaponHandler>();
            setWeapon(1);
        }
		startHP = PlayerManager.startHP;
		startEnergy = PlayerManager.startEnergy;
        bankSize = PlayerManager.bankSize;

		health.normal.background = fillTex(1,1,new Color(0.8f,0f,0f,1f));
		energy.normal.background = fillTex(1,1,new Color(0f,0f,0.8f,1f));
		bank.normal.background = fillTex (1,1,new Color(0f,0.8f,0f,1f));

	}
	
	void OnGUI () {
		main = (Texture2D) Resources.Load ("hud/topleft");
		speed = (Texture2D) Resources.Load ("hud/topright");
		leaderboard = (Texture2D) Resources.Load ("hud/leaderboard");
		
		deco = (Font) Resources.Load ("Belgrad");
		
		GUI.Label (new Rect (-15,-20,main.width,main.height), main);
		GUI.Label (new Rect (Screen.width-speed.width+15,-20,speed.width,speed.height), speed);
		GUI.Label (new Rect (Screen.width-leaderboard.width+80,Screen.height/2-leaderboard.height/2,leaderboard.width,leaderboard.height), leaderboard);
		GUI.Label (new Rect (5,0,64,64),flag);
		
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
        speedStyle.fontSize = 40;
		
		GUIStyle wepStyle = new GUIStyle();
    	wepStyle.font = deco;
		wepStyle.normal.textColor = Color.white;
		wepStyle.fontSize = 14;
		
		GUIStyle smallStyle = new GUIStyle();
    	smallStyle.font = deco;
		smallStyle.normal.textColor = Color.white;
		smallStyle.fontSize = 11;
		smallStyle.alignment = TextAnchor.MiddleRight;

		GUI.Label (new Rect (70,5,200,50),charName,hudStyle);
		GUI.Label (new Rect (92,25,200,50),coName,coStyle);
		GUI.Label (new Rect (70,20,20,20),coFlag,coStyle);
		
		GUI.Label (new Rect (200,1,40,20),hullTitle,smallStyle);
		GUI.Label (new Rect (202,11,40,20),energyTitle,smallStyle);
		GUI.Label (new Rect (200,21,40,20),bankTitle,smallStyle);
		
		// Health bar
		GUI.Label (new Rect (241,5,hitPoints/(startHP/hudBarSize),10),"",health);
		
		// Energy bar
		GUI.Label (new Rect (240,15,energyLevel/(startEnergy/hudBarSize),10),"",energy);
		
		// Power bank
		GUI.Label (new Rect (240,25,energyBank/(bankSize/hudBarSize),10),"",bank);
		
		// Speed and gear indicator
        GUI.Label (new Rect (Screen.width - 160, 10, 200, 50), "" + score, speedStyle);
		GUI.Label (new Rect (Screen.width-240,100,200,40),gearReady,hudStyle);
		
		// Scoreboard indicator
		GUI.Label (new Rect (Screen.width-120,Screen.height/2-leaderboard.height/2+20,200,40),"TEAM SCORES",coStyle);
		
		// Weapons initialisation
        int wepBoxSize = 48;
		GUI.Label (new Rect (-5,25,wepBoxSize,wepBoxSize), wepBox1);
		GUI.Label (new Rect (12,25,wepBoxSize,wepBoxSize), wepBox2);
        GUI.Label(new Rect(29,25,wepBoxSize,wepBoxSize), wepBox3);
		GUI.Label (new Rect (70,42,200,64), wepName, wepStyle);

        // Add a crosshair
        if (PlayerManager.activeChar == "china") crossTex = (Texture2D)Resources.Load("hud/crossChi");
        else if (PlayerManager.activeChar == "usa") crossTex = (Texture2D)Resources.Load("hud/crossUSA");
        else crossTex = (Texture2D)Resources.Load("hud/crossRus");
        Rect position = new Rect(Input.mousePosition.x - (crossTex.width / 2), (Screen.height - Input.mousePosition.y) - (crossTex.height / 2), crossTex.width, crossTex.height);
        GUI.DrawTexture(position, crossTex);
        Screen.showCursor = false;
	}	
}
