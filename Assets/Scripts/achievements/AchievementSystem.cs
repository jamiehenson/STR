using UnityEngine;
using System.Collections;
using System.Xml;
using System.IO;
using System.Collections.Generic;

// This is static, and that is fine.

public class AchievementSystem : MonoBehaviour {
	private static List<Achievement> achievements = new List<Achievement>();
	private static bool beenSetup = false;

	public static void MenuStarted() {
		if (!beenSetup)
			Setup();
	}

	private static void Setup() {
		DirectoryInfo dirInfo = new DirectoryInfo("Assets/Resources/Acheivments");

		foreach(System.IO.FileSystemInfo fileInfo in dirInfo.GetFiles()) {
			string endOfPath = fileInfo.FullName.Substring(fileInfo.FullName.Length - 5);

			if (!endOfPath.Equals(".meta"))
				AddFile(fileInfo);
		}

		beenSetup = true;
	}


	private static void AddFile(System.IO.FileSystemInfo file) {
		XmlDocument root = new XmlDocument();
		root.Load(file.FullName);
		XmlElement doc = root.DocumentElement;

		string name = doc.SelectSingleNode("name").InnerXml;
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
}
