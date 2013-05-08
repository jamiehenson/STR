using UnityEngine;
using System.Collections;

public class ServerScoringSystem : MonoBehaviour {

    public string[] playerNames;
    public bool initialized;

    private float levelTime = 10; // in seconds
    private int pauseDelay = 2; // in seconds
    private int stagesBeforeBoss = 3;
    private LevelManager[] levelManagers = new LevelManager[4];
    private BossLevelManager bossLevelManager;

	// Use this for initialization
	void Start () {
        initialized = false;   
	}

    public void updatePlayerNames(int pos, string s) {
        playerNames[pos] = s;
    }

    void OnGUI() {
        int players = Server.numberOfPlayers();
        if (!initialized) {
            playerNames = new string[players + 1];
            initialized = true;
        }

        if (players > 0) {
            for (int i = 1; i <= players; i++) {
                PlayerManager manager = GameObject.Find("Character" + i).GetComponent<PlayerManager>();
                GUI.Label(new Rect(60 * i, 0, 64, 64), playerNames[i] + " :");
                GUI.Label(new Rect(60 * i, 60, 64, 64), "Energy level: " + manager.getEnergyLevel());
                GUI.Label(new Rect(60 * i, 100, 64, 64), "HitPoints: " + manager.getHitPoints());
                GUI.Label(new Rect(60 * i, 140, 64, 64), "Score: " + manager.getScore());
            }
        }
    }

    // Used to time different stages and increment difficulty progression
    IEnumerator TimeLevels() {
        yield return new WaitForSeconds(pauseDelay);
        for (int j = 0; j < stagesBeforeBoss; j++) {
            Debug.Log("New Stage");
            // Level up
            foreach (LevelManager levMan in levelManagers) {
                levMan.LevelIncrease();
            }
            // Wait a certain amount of time
            yield return new WaitForSeconds(levelTime);
        }
        foreach (LevelManager levMan in levelManagers) {
            levMan.WarpAnimation();
        }
        yield return new WaitForSeconds(pauseDelay);
        // Inform LevelManagers about incoming boss
        foreach (LevelManager levMan in levelManagers) {
            levMan.BossComing(pauseDelay);
        }
        yield return new WaitForSeconds(pauseDelay);
        // SEND BOSS HERE
        bossLevelManager.CreateBoss();
    }

    IEnumerator BossClearedIE() {
        // Inform all LevelManagers that the boss is complete
        foreach (LevelManager levMan in levelManagers) {
            levMan.WarpAnimation();
        }
        yield return new WaitForSeconds(pauseDelay);
        foreach (LevelManager levMan in levelManagers) {
            levMan.BossCleared(pauseDelay);
        }
        // Start timing again
        StartCoroutine("TimeLevels");
    }

    // Used to control when a boss has been cleared (wrapper)
    public void BossCleared() {
        if (Network.isServer) {
            StartCoroutine("BossClearedIE");   
        }
    }

    // Global timer for controlling game progression
    public void StartTimer() {
        if (Network.isServer) {
            // Store level managers (for quicker repeated access)
            for (int i = 0; i < Server.numberOfPlayers(); i++) {
                levelManagers[i] = GameObject.Find("Universe" + (i + 1) + "/Managers/LevelManager").GetComponent<LevelManager>();
            }
            bossLevelManager = GameObject.Find("Universe" + (0) + "/Managers/LevelManager").GetComponent<BossLevelManager>();
            StartCoroutine("TimeLevels");
        }
    }
}
