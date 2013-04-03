using UnityEngine;
using System.Collections;

public class MP : MonoBehaviour
{
    public GameObject SBrowserDetails;
    public GameObject SNewServer;
    public GameObject SAvailableServers;
    public static bool joinScreen = false, hostScreen = false;
    public static string playerName = "HOLYER", serverName, playerLimit="4";
    private Font deco;
    private Texture2D bg;
    public static bool refresh;
    private bool startServer;
    public static HostData[] hostData;
    public static int hostnb;
    public GUIStyle serverButton;

    private float btnX = Screen.width * 0.1f;
    private float btnY = Screen.height * 0.1f;
    private float btnW = Screen.width * 0.4f;
    private float btnH = Screen.width * 0.05f;
	
	private GameObject header, subheader, heading1, heading2, serverNameBox, serverLimitBox, proceed, proceedtext, refreshbutton, flagbg, coPilot;
	private GameObject[] playerdetails;

    void Start()
    {
		header = GameObject.Find("SBrowserHeader");
        subheader = GameObject.Find("SBrowserSelect");
        heading1 = GameObject.Find("SGameName");
        heading2 = GameObject.Find("SPlayerLimit");
        serverNameBox = GameObject.Find("SDetailsNameBox2");
        serverLimitBox = GameObject.Find("SDetailsLimitBox");
        playerdetails = GameObject.FindGameObjectsWithTag("Player Details");
        proceed = GameObject.Find("SBrowser - proceed");
        proceedtext = GameObject.Find("Proceed text");
        refreshbutton = GameObject.Find("Refresh");
        flagbg = GameObject.Find("SBrowserBG");
		coPilot = GameObject.Find ("SCoPilot");
			
        MasterServer.ipAddress = "54.243.193.180";
        MasterServer.port = 23466;
        Network.natFacilitatorIP = "54.243.193.180";
        Network.natFacilitatorPort = 50005;
        iTween.FadeTo(gameObject, 0.3f, 0.1f);
        GameObject browserbg = GameObject.Find("SBrowserBG");
        iTween.FadeTo(browserbg, 0, 0.1f);
        bg = (Texture2D) Resources.Load("menu/blank");
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
    }
    
    void Update () {
        if (refresh)
        {
            MasterServer.RequestHostList(Server.gameName);
            if (MasterServer.PollHostList().Length > 0)
            {
                refresh = false;
            }
            hostData = MasterServer.PollHostList();
        }
		
		if (Camera.main.transform.position.x >= 72)
		{
			PilotName.showBox = true;
			PilotName.showBox2 = true;
			PilotName.showLimitBox = true;
		}
		else
		{
			PilotName.showBox = false;
			PilotName.showBox2 = false;
			PilotName.showLimitBox = false;
		}
    }

    void OnGUI()
    {
        deco = (Font)Resources.Load("Belgrad");

        GUIStyle exitStyle = new GUIStyle();
        exitStyle.font = deco;
        exitStyle.normal.textColor = Color.white;
        exitStyle.fontSize = 32;
        exitStyle.alignment = TextAnchor.UpperCenter;
		
		GUIStyle exitStyleBig = new GUIStyle();
        exitStyleBig.font = deco;
        exitStyleBig.normal.textColor = Color.white;
        exitStyleBig.fontSize = 40;
        exitStyleBig.alignment = TextAnchor.UpperCenter;

        GUIStyle field = new GUIStyle();
        field.padding = new RectOffset(15, 15, 15, 15);
        field.normal.textColor = Color.white;
        field.alignment = TextAnchor.MiddleCenter;
        field.font = deco;
        field.fontSize = (int)(Screen.height/100)*6;

        if (joinScreen && PilotName.showBox)
        {
            playerName = GUI.TextField(new Rect((Screen.width / 2), (Screen.height / 4) + 30, (Screen.width / 3), (Screen.width / 16)), playerName, 20, field);
            playerName = playerName.ToUpper();
        }
        if (hostScreen && PilotName.showBox2)
        {
            serverName = GUI.TextField(new Rect((Screen.width / 6), (Screen.height / 4) + 30, (Screen.width / 3), (Screen.width / 16)), serverName, 10, field);
            serverName = serverName.ToUpper();
        }
		if (hostScreen && PilotName.showLimitBox)
        {
            playerLimit = GUI.TextField(new Rect((Screen.width * 0.225f), (Screen.height / 4) + (Screen.width / 16) + 10, (Screen.width / 3), (Screen.width / 16)), playerLimit, 2, field);
            playerLimit = playerLimit.ToUpper();
        }
        if (hostScreen)
        {
            GUIStyle centeredStyle = GUI.skin.GetStyle("Label");
            centeredStyle.alignment = TextAnchor.UpperCenter;
            float charbgAlpha = 0.75f;

            // HOST ONLINE GAME
            header.GetComponent<TextMesh>().text = "HOST ONLINE GAME";
            subheader.GetComponent<TextMesh>().text = "CREATE A SERVER";
            heading1.GetComponent<TextMesh>().text = "NAME YOUR GAME";
            serverNameBox.renderer.enabled = true;
			serverLimitBox.renderer.enabled = true;
            foreach (GameObject component in playerdetails) component.renderer.enabled = false;
            proceed.renderer.enabled = true;
            proceedtext.renderer.enabled = true;
            refreshbutton.renderer.enabled = false;
            flagbg.renderer.enabled = false;
			heading2.renderer.enabled = true;
			coPilot.renderer.enabled = false;

            GameObject browserbg = GameObject.Find("SBrowserBG");
            iTween.FadeTo(browserbg, charbgAlpha, 4f);
            iTween.MoveTo(Camera.main.gameObject, new Vector3(75, 0, 0), 4);
            hostScreen = true;
        }

        if (hostData != null && joinScreen)
        {
            float btnYtotal;
            for (int i = 0; i < hostData.Length; i++)
            {
                btnYtotal = btnY * 2 + (btnH * i) + (btnH / 2) + 30;
                if (GUI.Button(new Rect(btnX - (btnX*3f) + btnW, btnYtotal, btnW*0.6f, btnH*1.4f), hostData[i].gameName,serverButton))
                {
                    hostnb = i;	
                    joinScreen = false;
					PlayerManager.setName(playerName);
                    Application.LoadLevel("OnlineClient");
                }
            }
        }	
    }
}
