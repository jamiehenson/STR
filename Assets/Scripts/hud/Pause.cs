using UnityEngine;
using System.Collections;

// Pause menu for STR levels
// J. Henson, using foundational code from Phil Chu
// Free script here: http://wiki.unity3d.com/index.php?title=PauseMenu

public class Pause : MonoBehaviour
{
	private float savedTimeScale;
	public enum Page {
	    None,Main,Options,Exit
	}
	private Page currentPage;
	private int toolbarInt = 0;
	private string[]  toolbarstrings = {"Audio","Graphics","System"};
 	private Texture2D pause_bg, pause_back, pause_continue, pause_settings;
 	private GameObject backImg;
 
	void Start() {
	    Time.timeScale = 1;
	    pause_bg = (Texture2D) Resources.Load("hud/pause_bg");
	    pause_back = (Texture2D) Resources.Load("hud/pause_back");
		pause_continue = (Texture2D) Resources.Load("hud/pause_continue");
	    pause_settings = (Texture2D) Resources.Load("hud/pause_settings");
	}
 
	void LateUpdate () {
		if (Input.GetKeyDown("escape")) 
		{
	        switch (currentPage) 
			{
	            case Page.None: 
					PauseGame(); 
					break;
				case Page.Main: 
					UnPauseGame(); 
					break;
				default: 
					currentPage = Page.Main;
					break;
	        }
	    }
	}
 
	void OnGUI () {
	    if (Time.timeScale == 0) {
	        switch (currentPage) {
	            case Page.Main: MainPauseMenu(); break;
	            case Page.Options: ShowToolbar(); break;
	        }
	    }   
	}
 
	void ShowToolbar() {
	    GUILayout.BeginArea( new Rect((Screen.width - 300) / 2, (Screen.height - 300 + 200) / 2, 300, 300));

	    toolbarInt = GUILayout.Toolbar (toolbarInt, toolbarstrings);
	    switch (toolbarInt) {
	        case 0: VolumeController(); break;
	        case 1: QualityLevels(); QualityController(); break;
	        case 2: ShowInfo(); break;
	    }

	    GUILayout.EndArea();
	    if (currentPage != Page.Main) {
	        ShowBackButton();
	    }
	}
 
	void ShowBackButton() {
	    if (GUI.Button(new Rect(Screen.width/2 - 25, Screen.height/2 + 140, 50, 20),"Back")) {
	        currentPage = Page.Main;
	    }
	}
 
	void ShowInfo() {
	    GUILayout.Label("Unity player version "+Application.unityVersion);
	    GUILayout.Label("Graphics: "+SystemInfo.graphicsDeviceName+" "+
	    SystemInfo.graphicsMemorySize+"MB\n"+
	    SystemInfo.graphicsDeviceVersion+"\n"+
	    SystemInfo.graphicsDeviceVendor);
	    GUILayout.Label("Shadows: "+SystemInfo.supportsShadows);
	    GUILayout.Label("Image Effects: "+SystemInfo.supportsImageEffects);
	    GUILayout.Label("Render Textures: "+SystemInfo.supportsRenderTextures);
	}

	void QualityLevels() {
	    switch (QualitySettings.GetQualityLevel()) 
		{
	        case 0:
	        	GUILayout.Label("Fastest");
	        	break;
	        case 1:
	        	GUILayout.Label("Fast");
	        	break;
	        case 2:
		        GUILayout.Label("Simple");
		        break;
	        case 3:
		        GUILayout.Label("Good");
		        break;
	        case 4:
		        GUILayout.Label("Beautiful");
		        break;
	        case 5:
	        	GUILayout.Label("Fantastic");
	        	break;
	    }
	}
 
	void QualityController() {
	    GUILayout.BeginHorizontal();
	    if (GUILayout.Button("Decrease")) {
	        QualitySettings.DecreaseLevel(true);
	    }
	    if (GUILayout.Button("Increase")) {
	        QualitySettings.IncreaseLevel(true);
	    }
	    GUILayout.EndHorizontal();
	}
 
	void VolumeController() {
	    GUILayout.Label("Master volume:");
	    AudioListener.volume = GUILayout.HorizontalSlider(AudioListener.volume, 0, 1);
	}
 
	void MainPauseMenu() {
	    GUILayout.BeginArea( new Rect((Screen.width - 300) / 2, (Screen.height - 300 + 200) / 2, 300, 300));

	    if (GUILayout.Button (pause_continue)) {
	        UnPauseGame();
	    }
	    if (GUILayout.Button (pause_settings)) {
	        currentPage = Page.Options;
	    }
	    if (GUILayout.Button (pause_back)) {
	    	Time.timeScale = 1;
	        Application.LoadLevel("menu");
	    }
	    
		GUILayout.EndArea();
	    if (currentPage != Page.Main) {
	        ShowBackButton();
	    }
	}

	void PauseGame() {
		backImg = new GameObject("Back Image");
		backImg.AddComponent("GUITexture");
		backImg.guiTexture.texture = pause_bg;
		backImg.transform.localScale = new Vector3(0, 0, 0);
		backImg.guiTexture.pixelInset = new Rect((Screen.width - 512) / 2, (Screen.height - 512) / 2, 512, 512);
	    savedTimeScale = Time.timeScale;
	    Time.timeScale = 0;
	    AudioListener.pause = true;
	    currentPage = Page.Main;	
	}
 
	void UnPauseGame() {
	    Time.timeScale = savedTimeScale;
	    AudioListener.pause = false;
		currentPage = Page.None;
		Destroy(backImg);
	}
 
	void OnApplicationPause(bool pause) {
	    if (Time.timeScale == 0) {
	        AudioListener.pause = true;
	    }
	}
}