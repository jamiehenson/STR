using UnityEngine;
using System.Collections;

public class LightEnemyManager : EnemyManager {

    public void Start() {
        health = 4;
        weaponPower = 7;
        speed = 1.5f;
        firingDelay = 1f;
        moveDelay = 1f;
        killPoints = 1000;
        enemyType = 1;
    }
}