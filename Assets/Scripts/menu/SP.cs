using UnityEngine;
using System.Collections;

public class SP : MonoBehaviour {
	private bool menuSP = false;
	public static bool singleplayerStart;
	private string coPilotName;

    private GameObject header,subheader,heading1,serverNameBox,proceed,proceedtext,refresh,flagbg,serverLimitBox,heading2,coPilot,flagBox;
	private GameObject[] playerdetails;
	private Object[] flags;
	
	void Start () {
		GameObject PopSP = GameObject.Find ("PopSP");
		GameObject SP = GameObject.Find ("SP Button");
		iTween.FadeTo(SP, 0.3f, 0.1f);	
		iTween.FadeTo(PopSP,0.5f,0.1f);
		singleplayerStart = false;

        header = GameObject.Find("SBrowserHeader");
        subheader = GameObject.Find("SBrowserSelect");
        heading1 = GameObject.Find("SGameName");
        serverNameBox = GameObject.Find("SDetailsNameBox2");
        playerdetails = GameObject.FindGameObjectsWithTag("Player Details");
        proceed = GameObject.Find("SBrowser - proceed");
        proceedtext = GameObject.Find("Proceed text");
        refresh = GameObject.Find("Refresh");
        flagbg = GameObject.Find("SBrowserBG");
		serverLimitBox = GameObject.Find("SDetailsLimitBox");
		heading2 = GameObject.Find("SPlayerLimit");
		coPilot = GameObject.Find("SCoPilot");
		flagBox = GameObject.Find("SFlag");
		flagBox.GetComponent<Renderer>().material.shader = Shader.Find("Transparent/Diffuse");
		flags = Resources.LoadAll("menu/flags", typeof(Texture2D));
	}

	void OnMouseEnter() {
		GameObject PopSP = GameObject.Find ("PopSP");
		GameObject SP = GameObject.Find ("SP Button");
		iTween.FadeTo(SP, 1.0f, 0.5f);
		iTween.FadeTo(PopSP,1.0f,0.5f);
		iTween.MoveTo(PopSP,new Vector3(PopSP.transform.position.x,PopSP.transform.position.y,28),0.5f);
	}
	
	void OnMouseExit() {
		GameObject PopSP = GameObject.Find ("PopSP");
		GameObject SP = GameObject.Find ("SP Button");
		iTween.FadeTo(SP, 0.3f, 0.5f);
		iTween.FadeTo(PopSP,0.5f,0.5f);
		iTween.MoveTo(PopSP,new Vector3(PopSP.transform.position.x,PopSP.transform.position.y,30),0.5f);
	}
	
	void OnMouseUp ()
	{
		menuSP = true;
	}
	
	void Update()
	{
		if (menuSP) 
		{
			iTween.MoveTo(Camera.main.gameObject,new Vector3(75,-75,0), 4);	
			menuSP = false;

            header.GetComponent<TextMesh>().text = "JOIN ONLINE GAME";
            subheader.GetComponent<TextMesh>().text = "CHOOSE AN ONLINE SERVER TO JOIN";
            heading1.GetComponent<TextMesh>().text = "AVAILABLE SERVERS";
			coPilot.GetComponent<TextMesh>().text = FetchPlayerName();
			flagBox.GetComponent<Renderer>().material.mainTexture = (Texture2D) flags[Random.Range(0,flags.Length)];
			
            GameObject browserbg = GameObject.Find("SBrowserBG");
            serverNameBox.renderer.enabled = false;
            foreach (GameObject component in playerdetails) component.renderer.enabled = true;
            proceed.renderer.enabled = false;
            proceedtext.renderer.enabled = false;
            refresh.renderer.enabled = true;
            flagbg.renderer.enabled = true;
			heading2.renderer.enabled = false;
			serverLimitBox.renderer.enabled = false;
			coPilot.renderer.enabled = true;
			flagBox.renderer.enabled = true;

            iTween.FadeTo(browserbg, 0.75f, 4f);
            iTween.MoveTo(Camera.main.gameObject, new Vector3(75, 0, 0), 4);
            MP.joinScreen = true;
            MP.refresh = true;
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
            player.Add("Cobbe-Warburton");
            player.Add("Collins");
            player.Add("Davies");
            player.Add("Dennis");
            player.Add("Diop");
            player.Add("Domenge");
            player.Add("Edwards-Jones");
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
