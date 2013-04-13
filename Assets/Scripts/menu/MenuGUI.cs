using UnityEngine;
using System.Collections;

public class MenuGUI : MonoBehaviour {
	
	private Texture2D credz;
	
	void Start () {
		iTween.CameraFadeAdd();
		iTween.CameraFadeFrom(1.0f, 2.0f);
		credz = (Texture2D) Resources.Load ("menu/credz");
		HudOn.gameOver = false;
		//HudOn.score = 0;
	}
	
	void OnGUI() {
		GUIStyle centeredStyle = GUI.skin.GetStyle("Label");
        Screen.showCursor = true;
    	centeredStyle.alignment = TextAnchor.UpperCenter;
		float credWidth = Screen.width;
		float credHeight = Screen.height;
		
		if (ShowCredits.credOn) {
			GUI.Label (new Rect ((Screen.width-credWidth)/2,(Screen.height-credHeight)/4,credWidth,credHeight), credz, centeredStyle);
            float yButPos = ((((Screen.height)/4)*3)+15);
            float xButPos = (Screen.width/2)-50;
			if (GUI.Button(new Rect(xButPos,yButPos, 100, 30), "Close")) ShowCredits.credOn = false;
		}

        if (Input.GetKeyDown("escape") && Exit.goingOut == false){
            Exit.goingOut = true;
        }
	}
}
