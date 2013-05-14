using UnityEngine;
using System.Collections;

public class HeavyEnemyManager : EnemyManager {

    public override void InitStats() {
        health = 16;
        weaponPower = 22;
        speed = 0.7f;
        firingDelay = 3f;
        moveDelay = 2f;
        killPoints = 4000;
        enemyType = 3;
        reward = 300;
    }
}