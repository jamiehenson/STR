using UnityEngine;
using System.Collections;

public class IntroController : MonoBehaviour {
	
	public GameObject text; 
	public GameObject logo;
	public GameObject bg;

	IEnumerator Intro() {
		iTween.CameraFadeAdd();
		iTween.CameraFadeFrom(1.0f, 2f);
		
		iTween.FadeTo(bg, 0f, 0.1f);	
		iTween.FadeTo(text, 0f, 0.1f);	
		iTween.FadeTo(logo, 0f, 0.1f);
		
		iTween.MoveTo(logo,new Vector3(1.44f,40,44.06f), 0.1f);
		iTween.MoveTo(text,new Vector3(1.6f,-30,40f), 0.1f);
		yield return new WaitForSeconds(2.8f);
		
		iTween.MoveTo(text,new Vector3(1.6f,-16f,40f), 4);	
		iTween.FadeTo(text,1f,1f);		
		yield return new WaitForSeconds(1.7f);
		
		iTween.MoveTo(logo,new Vector3(1.44f,4.4f,44.06f), 5);	
		iTween.FadeTo(logo,1f,1f);		
		yield return new WaitForSeconds(5);
		
		iTween.CameraFadeAdd();
		iTween.CameraFadeTo(1f, 2f);
		yield return new WaitForSeconds(1.5f);
		Application.LoadLevel("menu");
	}
	
	void Start () {
		StartCoroutine(Intro());
	}
}
