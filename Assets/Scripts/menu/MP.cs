using UnityEngine;
using System.Collections;

public class MP : MonoBehaviour
{
    public GameObject SBrowserDetails;
    public GameObject SNewServer;
    public GameObject SAvailableServers;
    public static bool joinScreen = false, hostScreen = false;
    public static string playerName = "WOOF", serverName = "GAME";
    private Font deco;
    private Texture2D bg;
    private int butWidth = 400, butHeight = 60, margin = 5;
    public static bool refresh;
    private bool startServer;
    public static HostData[] hostData;
    public static int hostnb;
    public GUIStyle serverButton;

    private float btnX = Screen.width * 0.1f;
    private float btnY = Screen.height * 0.1f;
    private float btnW = Screen.width * 0.4f;
    private float btnH = Screen.width * 0.05f;

    void Start()
    {
        MasterServer.ipAddress = "54.243.193.180";
        MasterServer.port = 23466;
        Network.natFacilitatorIP = "54.243.193.180";
        Network.natFacilitatorPort = 50005;
        iTween.FadeTo(gameObject, 0.3f, 0.1f);
        GameObject browserbg = GameObject.Find("SBrowserBG");
        iTween.FadeTo(browserbg, 0, 0.1f);
        bg = (Texture2D) Resources.Load("menu/blank");
        PlayerManager.activeChar = "usa";

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
        field.normal.textColor = Color.black;
        field.alignment = TextAnchor.MiddleCenter;
        field.font = deco;
        field.fontSize = (int)(Screen.height/100)*6;

        if (joinScreen && PilotName.showBox)
        {
            playerName = GUI.TextField(new Rect((Screen.width / 2), (Screen.height / 4) + 30, (Screen.width / 3), (Screen.width / 16)), playerName, 10, field);
            playerName = playerName.ToUpper();
        }
        if (hostScreen && PilotName.showBox2)
        {
            serverName = GUI.TextField(new Rect((Screen.width / 8), (Screen.height / 4) + 30, (Screen.width / 3), (Screen.width / 16)), serverName, 10, field);
            serverName = serverName.ToUpper();
        }
        if (hostScreen)
        {
            GUIStyle centeredStyle = GUI.skin.GetStyle("Label");
            centeredStyle.alignment = TextAnchor.UpperCenter;
            int credWidth = (Screen.width >= bg.width) ? bg.width : Screen.width;
            int credHeight = (Screen.height >= bg.height) ? bg.height : Screen.height;
            float charbgAlpha = 0.75f;

            GameObject header = GameObject.Find("SBrowserHeader");
            GameObject subheader = GameObject.Find("SBrowserSelect");
            GameObject heading1 = GameObject.Find("SGameName");
            GameObject serverNameBox = GameObject.Find("SDetailsNameBox2");
            GameObject[] playerdetails = GameObject.FindGameObjectsWithTag("Player Details");
            GameObject proceed = GameObject.Find("SBrowser - proceed");
            GameObject proceedtext = GameObject.Find("Proceed text");
            GameObject refreshbutton = GameObject.Find("Refresh");
            GameObject flagbg = GameObject.Find("SBrowserBG");

            // HOST ONLINE GAME
            header.GetComponent<TextMesh>().text = "HOST ONLINE GAME";
            subheader.GetComponent<TextMesh>().text = "CREATE A SERVER";
            heading1.GetComponent<TextMesh>().text = "NAME YOUR GAME";
            serverNameBox.renderer.enabled = true;
            foreach (GameObject component in playerdetails) component.renderer.enabled = false;
            proceed.renderer.enabled = true;
            proceedtext.renderer.enabled = true;
            refreshbutton.renderer.enabled = false;
            flagbg.renderer.enabled = false;

            GameObject browserbg = GameObject.Find("SBrowserBG");
            iTween.FadeTo(browserbg, charbgAlpha, 4f);
            iTween.MoveTo(Camera.main.gameObject, new Vector3(75, 0, 0), 4);
            hostScreen = true;
        }

        if (hostData != null && joinScreen)
        {
            float btnYtotal;
            if (hostScreen)  {
                GUI.Label(new Rect(btnX - (btnX * 4.5f), btnY * 5, btnW * 3, btnH), "AVAILABLE SERVERS:", exitStyle);
            }
            for (int i = 0; i < hostData.Length; i++)
            {
                btnYtotal = btnY * 2 + (btnH * i) + (btnH / 2) + 30;
                if (GUI.Button(new Rect(btnX - (btnX*3f) + btnW, btnYtotal, btnW*0.6f, btnH*1.4f), hostData[i].gameName,serverButton))
                {
                    hostnb = i;
                    PlayerManager.playername = playerName;
                    Application.LoadLevel("OnlineClient");
                }
            }
        }	
    }
}
