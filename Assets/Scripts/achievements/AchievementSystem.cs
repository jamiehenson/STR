using UnityEngine;
using System.Collections;
using System.Xml;
using System.IO;
using System.Collections.Generic;

// This is static, and that is fine.

public class AchievementSystem : MonoBehaviour {
	private static List<Achievement> achievements = new List<Achievement>();
	private static Metrics metrics;
	private static Time startedShowingMessage;
	private static bool beenSetup = false;

	public static void MenuStarted() {
		if (!beenSetup)
			Setup();

		metrics = new Metrics();
	}

	private static void Setup() {
		/*DirectoryInfo dirInfo = new DirectoryInfo("Assets/Resources/Acheivments");

		foreach(System.IO.FileSystemInfo fileInfo in dirInfo.GetFiles()) {
			string endOfPath = fileInfo.FullName.Substring(fileInfo.FullName.Length - 5);

			if (!endOfPath.Equals(".meta"))
				AddFile(fileInfo);
		}

		beenSetup = true;*/
	}


	private static void AddFile(System.IO.FileSystemInfo file) {
		XmlDocument root = new XmlDocument();
		root.Load(file.FullName);
		XmlElement doc = root.DocumentElement;

		//string name = doc.SelectSingleNode("name").InnerXml;
		string message = doc.SelectSingleNode("message").InnerXml;
		int score = int.Parse(doc.SelectSingleNode("score").InnerXml);

		Achievement newAchievement = new Achievement(message, score);

		XmlNode requirments = doc.SelectSingleNode("requirments");
		XmlNodeList list = requirments.ChildNodes;

		foreach(XmlNode node in list) {
			string metric = node.Attributes.GetNamedItem("metric").InnerXml;
			string comparision = node.Attributes.GetNamedItem("comparision").InnerXml;
			int val = int.Parse(node.InnerXml);
			newAchievement.Add (metric, comparision, val);
		}

		achievements.Add(newAchievement);
	}

	private static void checkAchievements() {
		foreach (Achievement achievement in achievements) {
			achievement.updateMetrics(metrics);
		}
	}

	public static void playerFired() {
		metrics.shots++;
		checkAchievements();
	}

	public static void killedEnemy() {
		metrics.kills++;
		checkAchievements();
	}

	public static void nextLevel() {
		metrics.levels++;
		checkAchievements();
	}

	public static void newAchievementMessage(AchievementMessage message) {
		HudOn.Instance.ToastWrapper(message.message);
	}
	/*
	public void OnGUI()
	{
		/*
			Vector3 screenPoint = Camera.main.ViewportToScreenPoint(vortPos);
			screenPoint.y = (Screen.height/2 - (screenPoint.y - Screen.height/2)); // Flip y about center line (lord knows why)
			int bgw = 140; // Vortex note bg width
			int bgh = 30; // Vortex note bg height
			int x = 10;
			int y = 5;
			GUIStyle style = new GUIStyle();
	    	style.font = deco;
			style.normal.textColor = Color.white;
			style.alignment = TextAnchor.MiddleCenter;
			style.fontStyle = FontStyle.Bold;
			style.fontSize = 18;
			label = label.ToUpper();
			GUI.DrawTexture(new Rect(screenPoint.x-(bgw/2-5),screenPoint.y-(bgh/2)-40,bgw,bgh),bg,ScaleMode.StretchToFill, true, 0);
			GUI.Label(new Rect(screenPoint.x,screenPoint.y-40,x,y), label, style);*//*
	}*/
}
