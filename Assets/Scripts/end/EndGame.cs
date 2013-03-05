using UnityEngine;
using System.Collections;

public class EndGame : MonoBehaviour {
	
	private Texture2D scoreBG;
	public static int endIndividualScore;
	
	void Start() {
		scoreBG = (Texture2D) Resources.Load ("end/endgameBG");
	}
	
	void OnGUI() {
		
		Font deco = (Font) Resources.Load ("Belgrad");
		
		GUIStyle endBigStyle = new GUIStyle();
    	endBigStyle.font = deco;
		endBigStyle.normal.textColor = Color.white;
		endBigStyle.fontSize = 120;
		
		GUIStyle endSmallStyle = new GUIStyle();
    	endSmallStyle.font = deco;
		endSmallStyle.normal.textColor = Color.white;
		endSmallStyle.fontSize = 60;
		
		GUI.DrawTexture (new Rect (0,0,Screen.width,Screen.height), scoreBG);
		GUI.Label (new Rect((Screen.width/2)-360,80,200,60),"GAME OVER",endBigStyle);
		GUI.Label (new Rect((Screen.width/2)-360,300,200,60),"YOU GOT " + endIndividualScore + "...",endSmallStyle);
		
		if (GUI.Button(new Rect(100,(Screen.height/1.15f), 200, 50), "PLAY AGAIN")) {
			HudOn.gameOver = false;
			Application.LoadLevel("plane");
		}
		if (GUI.Button(new Rect(Screen.width-300,(Screen.height/1.15f), 200, 50), "BACK TO MENU")) {
			HudOn.gameOver = false;
			Application.LoadLevel("menu");
		}
	}
}
