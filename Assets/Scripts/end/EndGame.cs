using UnityEngine;
using System.Collections;

public class EndGame : MonoBehaviour {
	
	private string heading, subheading;
	public static int endIndividualScore;
	
	void Start() {
		heading = GenerateEndHeader();
		subheading = GenerateEndSubHeader();
	}
	
	string GenerateEndHeader() {
		ArrayList endHeader = new ArrayList();
		endHeader.Add("Game Over!");
		endHeader.Add("Unlucky!");
		endHeader.Add("Strike out!");
		endHeader.Add("You're toast!");
		endHeader.Add("Chin up, mate");
		string winner = (string) endHeader[(int) Random.Range(0,endHeader.Count)];
		return winner.ToUpper();
	}
	
	string GenerateEndSubHeader() {
		ArrayList endSub = new ArrayList();
		endSub.Add("He died as he lived.");
		endSub.Add("Another one bites the dust.");
		endSub.Add("A solid effort.");
		string winner = (string) endSub[(int) Random.Range(0,endSub.Count)];
		return winner.ToUpper();
	}
	
	void OnGUI() {
		
		Font deco = (Font) Resources.Load ("Belgrad");
		
		GUIStyle endBigStyle = new GUIStyle();
    	endBigStyle.font = deco;
		endBigStyle.normal.textColor = Color.white;
		endBigStyle.fontSize = 120;
		endBigStyle.alignment = TextAnchor.MiddleCenter;
		
		GUIStyle endSubHeading = new GUIStyle();
    	endSubHeading.font = deco;
		endSubHeading.normal.textColor = Color.white;
		endSubHeading.alignment = TextAnchor.MiddleCenter;
		endSubHeading.fontStyle = FontStyle.Italic;
		endSubHeading.fontSize = 30;
		
		GUIStyle endSmallStyle = new GUIStyle();
    	endSmallStyle.font = deco;
		endSmallStyle.normal.textColor = Color.white;
		endSmallStyle.fontSize = 60;
		
		GUIStyle rightButton = new GUIStyle();
    	rightButton.font = deco;
		rightButton.normal.textColor = Color.white;
		rightButton.fontSize = 60;
		rightButton.alignment = TextAnchor.MiddleRight;
		
		GUI.Label (new Rect((Screen.width/2)-150,100,300,60),heading,endBigStyle);
		GUI.Label (new Rect((Screen.width/2)-150,170,300,60),subheading,endSubHeading);
		GUI.Label (new Rect((Screen.width/4)-360,Screen.height/4,200,60),"YOU GOT " + endIndividualScore + "...",endSmallStyle);
		
		if (GUI.Button(new Rect(100,(Screen.height/1.15f), 200, 50), "PLAY AGAIN",endSmallStyle)) {
			HudOn.gameOver = false;
			Application.LoadLevel("plane");
		}
		if (GUI.Button(new Rect(Screen.width-300,(Screen.height/1.15f), 200, 50), "BACK TO MENU",rightButton)) {
			HudOn.gameOver = false;
			Application.LoadLevel("menu");
		}
	}
}
