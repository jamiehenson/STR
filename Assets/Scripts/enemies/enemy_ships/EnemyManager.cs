using UnityEngine;
using System.Collections;

public class EnemyManager : MonoBehaviour {

    // Enemy stats
    // Enemy type is set ahead of instantiation for next enemy type
    public int enemyType = 1;

    // As these change on the fly, can't be static (access through GetComponent)
    public float health = 4;
    public float weaponPower = 1;
    public int killPoints = 100;
    public float speed = 10;
    public float firingDelay = 0.5f;
    public float moveDelay = 0.5f;
    public Color color = Color.blue;
    public float force = 500;

    // Randomised, doesn't depend on type
    // Directions:
    // 1 - From Left
    // 2 - From Top
    // 3 - From Right
    // 4 - From Bottom
    public int direction = 1;

    public void changeType(int type) {
        enemyType = type;
        switch (enemyType) {
            // light
            case 1:
                health = 4;
                weaponPower = 7;
                speed = 1.5f;
                firingDelay = 0.2f;
                moveDelay = 1f;
                color = Color.white;
                killPoints = 1000;
                break;
            // standard
            case 2:
                health = 8;
                weaponPower = 15;
                speed = 1;
                firingDelay = 1;
                moveDelay = 1.5f;
                color = Color.yellow;
                killPoints = 2000;
                break;
            // heavy
            case 3:
                health = 16;
                weaponPower = 22;
                speed = 0.7f;
                firingDelay = 3f;
                moveDelay = 2f;
                color = Color.blue;
                killPoints = 4000;
                break;
            // superheavy
            case 4:
                health = 30;
                weaponPower = 30;
                speed = 0.3f;
                firingDelay = 6f;
                moveDelay = 5f;
                color = Color.red;
                killPoints = 8000;
                break;
            default:
                break;
        }
    }
}