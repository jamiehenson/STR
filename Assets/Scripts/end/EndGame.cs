using UnityEngine;
using System.Collections;

public class EndGame : MonoBehaviour {
	
	private string heading, subheading;
	public static int endIndividualScore;
	
	void Start() {
		heading = GenerateEndHeader();
		subheading = GenerateEndSubHeader();
		GameObject header = GameObject.Find ("Header");
		header.guiText.text = heading;
		GameObject subheader = GameObject.Find ("Subheader");
		subheader.guiText.text = subheading;
		GameObject finalscore = GameObject.Find ("Team Score");
		finalscore.guiText.text = "TEAM: " + endIndividualScore + "...";
		GameObject youscore = GameObject.Find ("Your Score");
		youscore.guiText.text = "YOU: " + endIndividualScore + "...";
		GameObject teammate1score = GameObject.Find ("T1 Score");
		teammate1score.guiText.text = "TEAMMATE1: " + endIndividualScore + "...";
		GameObject teammate2score = GameObject.Find ("T2 Score");
		teammate2score.guiText.text = "TEAMMATE2: " + endIndividualScore + "...";
		
		//StartCoroutine(HSController.PostScores,name,score,kills,deaths);
	}
	
	string GenerateEndHeader() {
		ArrayList endHeader = new ArrayList();
		endHeader.Add("Game Over!");
		endHeader.Add("Unlucky!");
		endHeader.Add("Strike out!");
		endHeader.Add("You're toast!");
		endHeader.Add("Chin up, mate.");
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
		
		GUIStyle rightButton = new GUIStyle();
    	rightButton.font = deco;
		rightButton.normal.textColor = Color.white;
		rightButton.alignment = TextAnchor.MiddleRight;
		
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
