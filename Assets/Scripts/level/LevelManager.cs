using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour {
    // Level stats
    private int stage = 0;
    private Commander enemyGen;
    private HudOn hudOn;
    private int universeNum;

    // ******Determine by which prefab is the script called***** 
    private int universeN() {
        int length = transform.parent.parent.name.Length;
        string name = transform.parent.parent.name;
        if (name == "Boss Universe") return -1;
        string num = name.Substring(length - 1, 1);
        return (int.Parse(num));
    }

    public void Start() {
        if (Network.isServer) {
            universeNum = universeN();
            if (universeNum == -1) return;
            stage = 0;
            enemyGen = transform.parent.FindChild("EnemyManager").GetComponent<Commander>();
        }
    }

    public void WarpAnimation() {
        enemyGen.WarpAnimation();
    }

    public void LevelIncrease() {
        stage++;
        // Difficulty increases ahoy!
        enemyGen.IncreaseDifficulty();
    }

    IEnumerator BossComingIE(int wait) {
        // Push everyone to boss universe
        enemyGen.SendToBoss();
        // Pause for a set amount of time
        yield return new WaitForSeconds(wait + 2);
        // Tell the commander to stop sending enemies (& clear screen)
        enemyGen.ClearScreen();
    }

    public void BossComing(int wait) {
        // Wrapper - needs to run on a coroutine
        if (Network.isServer) {
            StartCoroutine("BossComingIE", wait);
        }
    }

    IEnumerator BossClearedIE(int wait) {
        // Pull everyone back to their own universe
        enemyGen.BringBackFromBoss();
        // Pause for a set amount of time
        yield return new WaitForSeconds(wait+5);
        // Tell the commander to resume sending enemies
        enemyGen.ResumeGame();
    }

    public void BossCleared(int wait) {
        // Wrapper - needs to run on a coroutine
        if (Network.isServer) {
            StartCoroutine("BossClearedIE", wait);
        }
    }
}