using UnityEngine;
using System.Collections;

public class SP : MonoBehaviour {
	private bool menuSP = false;
	public static bool singleplayerStart;

    private GameObject header;
    private GameObject subheader;
    private GameObject heading1;
    private GameObject serverNameBox;
    private GameObject[] playerdetails;
    private GameObject proceed;
    private GameObject proceedtext;
    private GameObject refresh;
    private GameObject flagbg;
	
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
            GameObject browserbg = GameObject.Find("SBrowserBG");
            serverNameBox.renderer.enabled = false;
            foreach (GameObject component in playerdetails) component.renderer.enabled = true;
            proceed.renderer.enabled = false;
            proceedtext.renderer.enabled = false;
            refresh.renderer.enabled = true;
            flagbg.renderer.enabled = true;

            iTween.FadeTo(browserbg, 0.75f, 4f);
            iTween.MoveTo(Camera.main.gameObject, new Vector3(75, 0, 0), 4);
            MP.joinScreen = true;
            MP.refresh = true;
		}
	}
}
