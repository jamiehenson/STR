using UnityEngine;
using System.Collections;

public class MediumEnemyManager : EnemyManager {

    public override void InitStats() {
        health = 8;
        weaponPower = 15;
        speed = 1;
        firingDelay = 2;
        moveDelay = 1.5f;
        killPoints = 2000;
        enemyType = 2;
        reward = 200;
    }
}