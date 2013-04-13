using UnityEngine;
using System.Collections;

public class ServerScoringSystem : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnGUI()
    {
        int players = Server.numberOfPlayers();
        if (players > 0)
        {
            for (int i = 1; i <= players; i++)
            {
                GUI.Label(new Rect(60 * i, 0, 64, 64), "Player" + i);

                PlayerManager manager = GameObject.Find("Character" + i).GetComponent<PlayerManager>();
                GUI.Label(new Rect(60 * i, 60, 64, 64), "Energy level: " + manager.getEnergyLevel());
                GUI.Label(new Rect(60 * i, 100, 64, 64), "HitPoints: " + manager.getHitPoints());
                GUI.Label(new Rect(60 * i, 140, 64, 64), "Score: " + manager.getScore());
            }
        }
    }
}
