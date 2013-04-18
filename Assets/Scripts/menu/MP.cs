using UnityEngine;
using System.Collections;

public class MP : MonoBehaviour
{
    private GameObject SBrowserDetails;
    private GameObject SNewServer;
    private GameObject SAvailableServers;
    public static bool joinScreen, hostScreen, openBox;
    public static string playerName, serverName, playerLimit, coPilotName;
    private Font deco;
    private Texture2D coPilotFlag;
    public static bool refresh;
    private bool startServer;
    public static HostData[] hostData;
    public static int hostnb;
    public GUIStyle serverButton;
	private Object[] flags;

    private float btnX = Screen.width * 0.1f;
    private float btnY = Screen.height * 0.1f;
    private float btnW = Screen.width * 0.4f;
    private float btnH = Screen.width * 0.05f;
	
	private GameObject header, subheader, heading1, heading2, playerNameText, serverNameBox, serverNameBoxText, serverLimitBox, serverLimitBoxText, proceed, proceedtext, refreshbutton, flagbg, coPilot, flagBox;
	private GameObject[] playerdetails, playerballs;

    void Start()
    {
		// Set static vars
		joinScreen = false;
		hostScreen = false;
		playerName = "HOLYER";
		playerLimit="4";
		hostData = null;
		playerdetails = null;
		playerballs = null;
		
		header = GameObject.Find("SBrowserHeader");
        subheader = GameObject.Find("SBrowserSelect");
        heading1 = GameObject.Find("SGameName");
        heading2 = GameObject.Find("SPlayerLimit");
        serverNameBox = GameObject.Find("SDetailsNameBox2");
		serverNameBoxText = GameObject.Find("SDetailsNameBox2Text");
        serverLimitBox = GameObject.Find("SDetailsLimitBox");
		serverLimitBoxText = GameObject.Find("SPlayerLimitText");
        playerdetails = GameObject.FindGameObjectsWithTag("Player Details");
		playerballs = GameObject.FindGameObjectsWithTag("Player Balls");
        proceed = GameObject.Find("SBrowser - proceed");
        proceedtext = GameObject.Find("Proceed text");
        refreshbutton = GameObject.Find("Refresh");
        flagbg = GameObject.Find("SBrowserBG");
		coPilot = GameObject.Find ("SCoPilot");
		flagBox = GameObject.Find("SFlag");
		playerNameText = GameObject.Find ("SPlayerNameText");
		flags = Resources.LoadAll("menu/flags", typeof(Texture2D));

		coPilotName = FetchPlayerName();
		coPilot.GetComponent<TextMesh>().text = coPilotName;
		coPilotFlag = (Texture2D) flags[Random.Range(0,flags.Length)];
		flagBox.GetComponent<Renderer>().material.mainTexture = coPilotFlag;

        MasterServer.ipAddress = "54.243.193.180";
        MasterServer.port = 23466;
        Network.natFacilitatorIP = "54.243.193.180";
        Network.natFacilitatorPort = 50005;
        iTween.FadeTo(gameObject, 0.3f, 0.1f);
        GameObject browserbg = GameObject.Find("SBrowserBG");
        iTween.FadeTo(browserbg, 0.6f, 0.1f);
        PlayerManager.activeChar = "usa";
        serverName = System.Environment.UserName;

        GameObject nameTitle = GameObject.Find("STRName");
        nameTitle.GetComponent<TextMesh>().text = Names.FetchSTRName();
		
		GameObject PopMP = GameObject.Find ("PopMP");
		iTween.FadeTo(PopMP,0.5f,0.1f);
    }

    void OnMouseEnter()
    {
        GameObject PopMP = GameObject.Find ("PopMP");
		GameObject MP = GameObject.Find ("Multiplayer button");
		iTween.FadeTo(MP, 1.0f, 0.5f);
		iTween.FadeTo(PopMP,1.0f,0.5f);
		iTween.MoveTo(PopMP,new Vector3(PopMP.transform.position.x,PopMP.transform.position.y,28),0.5f);
    }

    void OnMouseExit()
    {
        GameObject PopMP = GameObject.Find ("PopMP");
		GameObject MP = GameObject.Find ("Multiplayer button");
		iTween.FadeTo(MP, 0.3f, 0.5f);
		iTween.FadeTo(PopMP,0.5f,0.5f);
		iTween.MoveTo(PopMP,new Vector3(PopMP.transform.position.x,PopMP.transform.position.y,30),0.5f);
    }

    void OnMouseUp()
    {
        hostScreen = true;
		openBox = true;
		GameObject browserbg = GameObject.Find("SBrowserBG");
        iTween.FadeTo(browserbg, 0.6f, 1);
    }
    
    void Update ()
	{
        if (refresh)
        {
            MasterServer.RequestHostList(Server.gameName);
            if (MasterServer.PollHostList().Length > 0)
            {
                refresh = false;
            }
            hostData = MasterServer.PollHostList();
		}
    }

    void OnGUI()
    {
        deco = (Font)Resources.Load("Belgrad");
		Texture2D bgTex = (Texture2D) Resources.Load ("menu/blank");
		Texture2D bgTexFull = (Texture2D) Resources.Load ("menu/blankfull");

        GUIStyle exitStyle = new GUIStyle();
        exitStyle.font = deco;
        exitStyle.normal.textColor = Color.white;
        exitStyle.fontSize = Screen.height/16;
        exitStyle.alignment = TextAnchor.MiddleLeft;

		GUIStyle exitStyleR = new GUIStyle();
        exitStyleR.font = deco;
        exitStyleR.normal.textColor = Color.white;
        exitStyleR.fontSize = Screen.height/12;
        exitStyleR.alignment = TextAnchor.MiddleRight;
		
		GUIStyle exitStyleBig = new GUIStyle();
        exitStyleBig.font = deco;
        exitStyleBig.normal.textColor = Color.white;
        exitStyleBig.fontSize = Screen.height/16;
        exitStyleBig.alignment = TextAnchor.UpperCenter;

        GUIStyle field = new GUIStyle();
        field.padding = new RectOffset(15, 15, 15, 15);
        field.normal.textColor = Color.white;
		field.normal.background = bgTexFull;
        field.alignment = TextAnchor.MiddleCenter;
        field.font = deco;
        field.fontSize = (int)(Screen.height/100)*10;

		GUIStyle field2 = new GUIStyle();
        field2.padding = new RectOffset(15, 15, 15, 15);
        field2.normal.textColor = Color.white;
		field2.normal.background = bgTexFull;
        field2.alignment = TextAnchor.MiddleCenter;
        field2.font = deco;
        field2.fontSize = (int)(Screen.height/100)*6;

        if (joinScreen)
        {
			if (openBox)
			{
				GUI.Label (new Rect(0,0,Screen.width,Screen.height),bgTex);
				GUI.Label (new Rect(Screen.width/2-90,Screen.height/4+10,200,40),"WHAT'S YOUR NAME?",exitStyleBig);
				playerName = GUI.TextField(new Rect((Screen.width / 2)-(Screen.width / 6), Screen.height / 4 + (Screen.width / 18), Screen.width/3, Screen.width/12), playerName, 10, field);
				playerName = playerName.ToUpper();
				if (GUI.Button(new Rect(Screen.width/2-Screen.width/8,Screen.height*0.65f,Screen.width/4,Screen.height/10),"DONE",field2)) openBox = false;
			}

			header.GetComponent<TextMesh>().text = "JOIN ONLINE GAME";
            subheader.GetComponent<TextMesh>().text = "CHOOSE AN ONLINE SERVER TO JOIN";
            heading1.GetComponent<TextMesh>().text = "AVAILABLE SERVERS";

			GameObject browserbg = GameObject.Find("SBrowserBG");
            serverNameBox.renderer.enabled = false;
            foreach (GameObject component in playerdetails) component.renderer.enabled = true;
            proceed.renderer.enabled = false;
            proceedtext.renderer.enabled = false;
            refreshbutton.renderer.enabled = true;
            flagbg.renderer.enabled = true;
			heading2.renderer.enabled = false;
			serverLimitBox.renderer.enabled = false;
			coPilot.renderer.enabled = true;
			flagBox.renderer.enabled = true;
			serverNameBoxText.renderer.enabled = false;
			serverLimitBoxText.renderer.enabled = false;
			playerNameText.renderer.enabled = true;

            iTween.MoveTo(Camera.main.gameObject, new Vector3(75, 0, 0), 4);
            joinScreen = true;
            refresh = true;

			playerNameText.GetComponent<TextMesh>().text = playerName;
        }

        if (hostScreen)
        {
            GUIStyle centeredStyle = GUI.skin.GetStyle("Label");
            centeredStyle.alignment = TextAnchor.UpperCenter;
            float charbgAlpha = 0.75f;

			if (openBox)
			{
				GUI.Label (new Rect(0,0,Screen.width,Screen.height),bgTex);

				GUI.Label (new Rect(Screen.width/2-90,Screen.height/4+10,200,40),"SET UP YOUR GAME!",exitStyleBig);

				GUI.Label (new Rect(Screen.width/4 + Screen.width/18, Screen.height/4 + Screen.height/8, Screen.width/6, Screen.height/10),"NAME:",exitStyle);
				serverName = GUI.TextField(new Rect((Screen.width/2 - Screen.height/8), Screen.height/4 + Screen.height/8 + 2, Screen.width/4, Screen.height/10), serverName, 10, field);

				GUI.Label (new Rect(Screen.width/4 + Screen.width/18, Screen.height/3 + Screen.height/6, Screen.width/6, Screen.height/10),"PLAYERS:",exitStyle);
				playerLimit = GUI.TextField(new Rect((Screen.width/2 - Screen.height/8), Screen.height/3 + Screen.height/6, Screen.width/4, Screen.height/10), playerLimit, 2, field);

				serverName = serverName.ToUpper();
				playerLimit = playerLimit.ToUpper();

				if (GUI.Button(new Rect(Screen.width/2-Screen.width/8,Screen.height*0.65f,Screen.width/4,Screen.height/10),"DONE",field2)) openBox = false;
			}

            // HOST ONLINE GAME
            header.GetComponent<TextMesh>().text = "HOST ONLINE GAME";
            subheader.GetComponent<TextMesh>().text = "CREATE A SERVER";
            heading1.GetComponent<TextMesh>().text = "SERVER NAME";
            serverNameBox.renderer.enabled = true;
			serverLimitBox.renderer.enabled = true;
			serverNameBoxText.renderer.enabled = true;
			serverLimitBoxText.renderer.enabled = true;
			serverNameBoxText.GetComponent<TextMesh>().text = serverName;
			serverLimitBoxText.GetComponent<TextMesh>().text = playerLimit;
            foreach (GameObject component in playerdetails) component.renderer.enabled = false;
			foreach (GameObject component in playerballs) component.renderer.enabled = false;
            proceed.renderer.enabled = true;
            proceedtext.renderer.enabled = true;
            refreshbutton.renderer.enabled = false;
            flagbg.renderer.enabled = false;
			heading2.renderer.enabled = true;
			coPilot.renderer.enabled = false;
			flagBox.renderer.enabled = false;
			playerNameText.renderer.enabled = false;

            GameObject browserbg = GameObject.Find("SBrowserBG");
            iTween.FadeTo(browserbg, charbgAlpha, 4f);
            iTween.MoveTo(Camera.main.gameObject, new Vector3(75, 0, 0), 4);
            hostScreen = true;
        }

        if (hostData != null && joinScreen && Camera.main.transform.position.x >= 74 && openBox == false)
        {
            float btnYtotal;
            for (int i = 0; i < hostData.Length; i++)
            {
                btnYtotal = btnY * 2 + (btnH * i) + (btnH / 2) + 30;
                if (GUI.Button(new Rect(btnX - (btnX*3f) + btnW, btnYtotal, btnW*0.6f, btnH*1.4f), hostData[i].gameName,serverButton))
                {
                    hostnb = i;	
                    joinScreen = false;
                    PlayerManager.playername = playerName;
                    Application.LoadLevel("OnlineClient");
                }
            }
        }	
    }

	private string FetchPlayerName() {
        ArrayList rank = new ArrayList();
            rank.Add("Pvt. ");
            rank.Add("Sgt. ");
            rank.Add("Lt. ");
            rank.Add("Cpt. ");
            rank.Add("Maj. ");

		ArrayList player = new ArrayList();
            player.Add("Andrew");
            player.Add("Bergmann");
            player.Add("Bessey");
            player.Add("Blackmore");
            player.Add("Briggs");
            player.Add("Brown");
            player.Add("Collins");
            player.Add("Clark");
            player.Add("Coales");
            player.Add("Collins");
            player.Add("Davies");
            player.Add("Dennis");
            player.Add("Diop");
            player.Add("Domenge");
            player.Add("Ford");
            player.Add("Ghumran");
            player.Add("Grant");
            player.Add("Gulliver");
            player.Add("Hart");
            player.Add("Henson");
            player.Add("Jeufo");
            player.Add("Jones");
            player.Add("Joyce");
            player.Add("Lamb");
            player.Add("Latimer");
            player.Add("Lewis");
            player.Add("Lisle");
            player.Add("Livingston");
            player.Add("Lloyd");
            player.Add("Lockyer");
            player.Add("Matusiewicz");
            player.Add("McIntosh");
			player.Add("Mortensson");
            player.Add("Muir");
            player.Add("Nelson");
            player.Add("Ogilvie");
            player.Add("Otrocol");
            player.Add("Patrichi");
            player.Add("Reed");
            player.Add("Simeria");
            player.Add("Stewart");
            player.Add("Timms");
            player.Add("Uttamchandani");
            player.Add("Webb");
            player.Add("Whiteley");
            player.Add("Wybourn");
		string winner = (string)  rank[(int) Random.Range(0,rank.Count)] + player[(int) Random.Range(0,player.Count)];
		return winner.ToUpper();
	}
}
