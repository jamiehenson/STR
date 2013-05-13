using UnityEngine;
using System.Collections;

public class BossManager : MonoBehaviour {

    // Enemy stats
    // Enemy type is set ahead of instantiation for next enemy type
    public int enemyType = 1;

    // As these change on the fly, can't be static (access through GetComponent)
    public float health              = 4;
    public float weaponPower         = 1;
    public int killPoints            = 100;
    public float speed               = 20;
    public float firingDelay         = 0.5f;
    public float moveDelay           = 0.5f;
    public Color color               = Color.blue;
    public float forceMultiplier     = 500;
    public float rotation            = 0.0f;
    public float typeForceMultiplier = 0.0f;

    // Randomised, doesn't depend on type
    // Directions:
    // 1 - From Left
    // 2 - From Top
    // 3 - From Right
    // 4 - From Bottom
    public int direction = 4;

    public virtual void Start() {
        if (Network.isServer) {
            gameObject.name = "Boss0";
        }
    }

    public int PickTarget() {
        GameObject[] activeChars = GameObject.FindGameObjectsWithTag("Player");
        if (activeChars.Length == 0) return -1;
        int index = Random.Range(1, activeChars.Length+1);
        return index;
    }

    public virtual IEnumerator Shoot() {
        while (true) {
            yield return new WaitForSeconds(5f);
        }
    }
}