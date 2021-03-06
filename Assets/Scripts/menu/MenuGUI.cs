using UnityEngine;
using System.Collections;

public class MenuGUI : MonoBehaviour {
	
	private Texture2D credz;
	
	void Start () {
		iTween.CameraFadeAdd();
		iTween.CameraFadeFrom(1.0f, 2.0f);
		credz = (Texture2D) Resources.Load ("menu/credz");
		HudOn.gameOver = false;
	}
	
	void OnGUI() {
		GUIStyle centeredStyle = GUI.skin.GetStyle("Label");
        Screen.showCursor = true;
    	centeredStyle.alignment = TextAnchor.UpperCenter;
		
		if (ShowCredits.credOn) {
			GUI.Label (new Rect (0,0,Screen.width,Screen.height), credz);
            float yButPos = (((Screen.height)/4)-10);
            float xButPos = (Screen.width*0.65f)-100;
			if (GUI.Button(new Rect(xButPos,yButPos, 200, 30), "Close")) ShowCredits.credOn = false;
		}

        /*if (Input.GetKeyDown("escape") && Exit.goingOut == false){
            Exit.goingOut = true;
        }*/
	}
}
