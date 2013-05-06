using UnityEngine;
using System.Collections;

public class SuperheavyEnemyManager : EnemyManager {

    public void Start() {
        health = 30;
        weaponPower = 30;
        speed = 0.3f;
        firingDelay = 6f;
        moveDelay = 5f;
        killPoints = 8000;
        enemyType = 4;
    }
}