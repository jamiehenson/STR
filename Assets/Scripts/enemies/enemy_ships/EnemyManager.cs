using UnityEngine;
using System.Collections;

public class EnemyManager : MonoBehaviour {

    // As these change on the fly, can't be static (access through GetComponent)
    public float health = 4;
    public float weaponPower = 1;
    public int killPoints = 100;
    public float speed = 10;
    public float firingDelay = 0.5f;
    public float moveDelay = 0.5f;
    public float force = 500;
    public int enemyType = 1;

    // Randomised, doesn't depend on type
    // Directions:
    // 1 - From Left
    // 2 - From Top
    // 3 - From Right
    // 4 - From Bottom
    public int direction = 1;

    public virtual void Start() { }
    public virtual void InitStats() { }
}