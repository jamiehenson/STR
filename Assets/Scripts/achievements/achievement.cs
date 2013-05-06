using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Achievement {
	private List<Requirement> requirements = new List<Requirement>();
	private bool beenAchieved = false;
	string message;
	int score;

	public Achievement(string m, int s) {
		message = m;
		score = s;
	}

	public void Add(string metric, string op, int v) {
		Requirement newRequirement = Requirement.newRequirement(metric, op, v);
		requirements.Add (newRequirement);
	}

	public void updateMetrics(Metrics m) {
		if (beenAchieved)
			return;

		bool metAllSoFar = true;
		foreach(Requirement req in requirements) {
			if (!req.compare(m))
				metAllSoFar = false;
		}

		if (metAllSoFar) {
			// Completed
			beenAchieved = true;
			Achieved();
		}
	}

	private void Achieved() {
		AchievementMessage am = new AchievementMessage();
		message = message.ToUpper();
		am.message = "ACHIEVEMENT UNLOCKED!\n"+message+"\n+"+score;
		AchievementSystem.newAchievementMessage(am);
	}
}
