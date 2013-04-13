using UnityEngine;
using System.Collections;

public class Exit : MonoBehaviour {
	public static bool goingOut;
	private Font deco;
    private Texture2D bg;
	
	void Start () {
		goingOut = false;
		
		GameObject PopExit = GameObject.Find ("PopExit");
		GameObject Exit = GameObject.Find ("Exit");
		iTween.FadeTo(Exit, 0.3f, 0.1f);
        bg = (Texture2D)Resources.Load("menu/blank");
		iTween.FadeTo(PopExit,0.5f,0.1f);
	}

	void OnMouseEnter() {
		GameObject Exit = GameObject.Find ("Exit");
		GameObject PopExit = GameObject.Find ("PopExit");
		iTween.FadeTo(Exit, 1.0f, 0.5f);
		iTween.FadeTo(PopExit,1.0f,0.5f);
		iTween.MoveTo(PopExit,new Vector3(PopExit.transform.position.x,PopExit.transform.position.y,28),0.5f);
	}
	
	void OnMouseExit() {
		GameObject Exit = GameObject.Find ("Exit");
		GameObject PopExit = GameObject.Find ("PopExit");
		iTween.FadeTo(Exit, 0.3f, 0.5f);
		iTween.FadeTo(PopExit,0.5f,0.5f);
		iTween.MoveTo(PopExit,new Vector3(PopExit.transform.position.x,PopExit.transform.position.y,30),0.5f);
	}
	
	void OnMouseUp () {
		goingOut = true;
	}
	
	void Update() {
		if (goingOut) {
            /*
            if (Input.GetKeyDown("escape") && goingOut == true) goingOut = false;
            GUIStyle centeredStyle = GUI.skin.GetStyle("Label");
            centeredStyle.alignment = TextAnchor.UpperCenter;
            int credWidth = (Screen.width >= bg.width) ? bg.width : Screen.width;
            int credHeight = (Screen.height >= bg.height) ? bg.height : Screen.height;

            GUI.Label(new Rect((Screen.width - credWidth) / 2, (Screen.height - credHeight) / 4, credWidth, credHeight), bg, centeredStyle);
			GUI.Box (new Rect (Screen.width/2-95,Screen.height/2-130,200,140), "QUIT STR?",exitStyleBig);
           
            if (GUI.Button (new Rect (Screen.width/2-80,Screen.height/2,200,60), "YES, ABANDON SHIP!",exitStyle)) {
            	
            }
            if (GUI.Button (new Rect (Screen.width/2-80,Screen.height/2+80,200,60), "NO, GIVE ME MORE!",exitStyle)) {
                goingOut = false;
            }*/
            Application.Quit();
		}
	}
}
