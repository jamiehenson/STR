using UnityEngine;
using System.Collections;

public class Exit : MonoBehaviour {
	public static bool goingOut = false;
	private Font deco;
    private Texture2D bg;
	
	void Start () {
		iTween.FadeTo(gameObject, 0.3f, 0.1f);
        bg = (Texture2D)Resources.Load("menu/blank");
	}

	void OnMouseEnter() {
		iTween.FadeTo(gameObject, 1.0f, 0.5f);
	}
	
	void OnMouseExit() {
		iTween.FadeTo(gameObject, 0.3f, 0.5f);
	}
	
	void OnMouseUp () {
		goingOut = true;
	}
	
	void OnGUI() {
		deco = (Font) Resources.Load ("Belgrad");
		
		GUIStyle exitStyle = new GUIStyle();
    	exitStyle.font = deco;
		exitStyle.normal.textColor = Color.white;
		exitStyle.fontSize = 40;
        exitStyle.alignment = TextAnchor.UpperCenter;


        GUIStyle exitStyleBig = new GUIStyle();
        exitStyleBig.font = deco;
        exitStyleBig.normal.textColor = Color.white;
        exitStyleBig.fontSize = 80;
        exitStyleBig.alignment = TextAnchor.UpperCenter;

		
		if (goingOut && Camera.main.gameObject.transform.position.x == 0 && Camera.main.gameObject.transform.position.y == 0) {
            if (Input.GetKeyDown("escape") && goingOut == true) goingOut = false;
            GUIStyle centeredStyle = GUI.skin.GetStyle("Label");
            centeredStyle.alignment = TextAnchor.UpperCenter;
            int credWidth = (Screen.width >= bg.width) ? bg.width : Screen.width;
            int credHeight = (Screen.height >= bg.height) ? bg.height : Screen.height;

            GUI.Label(new Rect((Screen.width - credWidth) / 2, (Screen.height - credHeight) / 4, credWidth, credHeight), bg, centeredStyle);
			GUI.Box (new Rect (Screen.width/2-95,Screen.height/2-130,200,140), "QUIT STR?",exitStyleBig);
           
            if (GUI.Button (new Rect (Screen.width/2-80,Screen.height/2,200,60), "YES, ABANDON SHIP!",exitStyle)) {
            	Application.Quit();
            }
            if (GUI.Button (new Rect (Screen.width/2-80,Screen.height/2+80,200,60), "NO, GIVE ME MORE!",exitStyle)) {
                goingOut = false;
            }
		}
	}
}
