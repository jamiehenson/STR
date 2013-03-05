using UnityEngine;
using System.Collections;

public class MP : MonoBehaviour
{
    public GameObject SBrowserDetails;
    public GameObject SNewServer;
    public GameObject SAvailableServers;
    public static bool goingOn = false, joinScreen = false, hostScreen = false;
    public static string playerName = "WOOF", serverName = "GAME";
    private Font deco;
    private Texture2D bg;
    private int butWidth = 400, butHeight = 60, margin = 5;
    public static bool refresh;
    private bool startServer;
    public static HostData[] hostData;
    public static int hostnb;

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
    }

    void OnMouseEnter()
    {
        iTween.FadeTo(gameObject, 1.0f, 0.5f);
    }

    void OnMouseExit()
    {
        iTween.FadeTo(gameObject, 0.3f, 0.5f);
    }

    void OnMouseUp()
    {
        goingOn = true;
    }
    
    void Update () {
        if (refresh)
        {
            MasterServer.RequestHostList(Server.gameName);
            if (MasterServer.PollHostList().Length > 0) refresh = false;
            hostData = MasterServer.PollHostList();
        }
    }

    void OnGUI()
    {
        deco = (Font)Resources.Load("Belgrad");

        GUIStyle exitStyle = new GUIStyle();
        exitStyle.font = deco;
        exitStyle.normal.textColor = Color.white;
        exitStyle.fontSize = 28;
        exitStyle.alignment = TextAnchor.UpperCenter;

        GUIStyle field = new GUIStyle();
        field.normal.background = HudOn.fillTex(10,10,Color.black);
        field.padding = new RectOffset(15, 15, 15, 15);
        field.normal.textColor = Color.white;
        field.alignment = TextAnchor.MiddleCenter;
        field.font = deco;
        field.fontSize = 32;

        if (joinScreen && Camera.main.transform.position.x >= 75)
        {
            playerName = GUI.TextField(new Rect((Screen.width / 2) + 50, (Screen.height / 4) + 40, 300, 50), playerName, 10, field);
            playerName = playerName.ToUpper();
        }

        if (hostScreen && Camera.main.transform.position.x >= 75)
        {
            playerName = GUI.TextField(new Rect((Screen.width / 2) + 50, (Screen.height / 4) + 40, 300, 50), playerName, 10, field);
            playerName = playerName.ToUpper();
            serverName = GUI.TextField(new Rect((Screen.width / 10), (Screen.height / 4) + 40, 300, 50), serverName, 10, field);
            serverName = serverName.ToUpper();
        }

        if (goingOn)
        {
            GUIStyle centeredStyle = GUI.skin.GetStyle("Label");
            centeredStyle.alignment = TextAnchor.UpperCenter;
            int credWidth = (Screen.width >= bg.width) ? bg.width : Screen.width;
            int credHeight = (Screen.height >= bg.height) ? bg.height : Screen.height;
            float charbgAlpha = 0.75f;

            GUI.Label(new Rect((Screen.width - credWidth) / 2, (Screen.height - credHeight) / 4, credWidth, credHeight), bg, centeredStyle);
            GUI.Box(new Rect(Screen.width / 2 - 95, Screen.height / 2 - 130, 200, 140), "WHAT WOULD YOU LIKE TO DO?", exitStyle);

            GameObject header = GameObject.Find("SBrowserHeader");
            GameObject subheader = GameObject.Find("SBrowserSelect");
            GameObject heading1 = GameObject.Find("SGameName");

            // HOST ONLINE GAME
            if (GUI.Button(new Rect(Screen.width / 2 - (butWidth/2), Screen.height / 2 - 60, butWidth, butHeight), "HOST ONLINE GAME"))
            {
                
                header.GetComponent<TextMesh>().text = "HOST ONLINE GAME";
                subheader.GetComponent<TextMesh>().text = "CREATE A SERVER";
                heading1.GetComponent<TextMesh>().text = "NAME YOUR GAME";
                GameObject browserbg = GameObject.Find("SBrowserBG");
                iTween.FadeTo(browserbg, charbgAlpha, 4f);
                iTween.MoveTo(Camera.main.gameObject, new Vector3(75, 0, 0), 4);
                goingOn = false;
                hostScreen = true;
            }

            //JOIN ONLINE GAME
            if (GUI.Button(new Rect(Screen.width / 2 - (butWidth/2), Screen.height / 2 - 60 + butHeight + margin, butWidth, butHeight), "JOIN ONLINE GAME"))
            {
                header.GetComponent<TextMesh>().text = "JOIN ONLINE GAME";
                subheader.GetComponent<TextMesh>().text = "CHOOSE AN ONLINE SERVER TO JOIN";
                heading1.GetComponent<TextMesh>().text = "AVAILABLE SERVERS";
                GameObject browserbg = GameObject.Find("SBrowserBG");
                iTween.FadeTo(browserbg, charbgAlpha, 4f);
                iTween.MoveTo(Camera.main.gameObject, new Vector3(75, 0, 0), 4);
                goingOn = false;
                joinScreen = true;
            }

            if (GUI.Button(new Rect(Screen.width / 2 - (butWidth / 2), Screen.height / 2 + 40 + butHeight, butWidth, butHeight/2), "CLOSE"))
            {
                goingOn = false;
            }
        }

        if (hostData != null)
        {
            float btnYtotal;
            if (hostScreen)  {
                GUI.Label(new Rect(btnX - (btnX * 4.5f), btnY * 5, btnW * 3, btnH), "AVAILABLE SERVERS:", exitStyle);
            }
            for (int i = 0; i < hostData.Length; i++)
            {
                if (hostScreen) btnYtotal = btnY * 4 + (btnH * i) + (btnH / 2);
                else btnYtotal = btnY * 2 + (btnH * i) + (btnH / 2);
                if (GUI.Button(new Rect(btnX - (btnX*4f) + btnW, btnYtotal, btnW*0.8f, btnH), hostData[i].gameName))
                {
                    hostnb = i;
                    Application.LoadLevel("OnlineClient");
                }
            }
        }
    }
}
