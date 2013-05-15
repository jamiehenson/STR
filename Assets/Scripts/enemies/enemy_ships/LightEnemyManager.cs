using UnityEngine;
using System.Collections;

public class LightEnemyManager : EnemyManager {

    public override void InitStats() {
        health = 4;
        weaponPower = 7;
        speed = 1.5f;
        firingDelay = 1f;
        moveDelay = 1f;
        killPoints = 1000;
        enemyType = 1;
        reward = 100;
    }
}