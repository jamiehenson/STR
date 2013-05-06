using UnityEngine;
using System.Collections;

public class BossManager : MonoBehaviour {

    private Commander commander;

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
    public int direction = 4;

    public virtual void Start() {
        if (Network.isServer) {
            gameObject.name = "Boss0";
            commander = transform.parent.parent.FindChild("Managers/EnemyManager").GetComponent<Commander>();
        }
    }

    public int PickTarget() {
        int activeChars = 0;
        print(commander.activeCharacters.Length);
        for (int i = 0; i < commander.activeCharacters.Length; i++) {
            if (commander.activeCharacters[i] == true) activeChars++;
        }
        if (activeChars == 0) return -1;
        int index = Random.Range(0, activeChars);
        return index;
    }

    // Used for firing
    public virtual IEnumerator Shoot() {
        while (true) {
            //if (inPlane) {
            //    //targetPlayer = comman
            //    int targetPlayer = PickTarget();
            //    if (targetPlayer != -1) {
            //        Transform bullet = (Transform)Network.Instantiate(bulletPrefab, gameObject.transform.position, gameObject.transform.rotation, 100 + universeNb);
            //        GameObject character = GameObject.Find("Character" + targetPlayer);
            //        EnemyBulletSettings ebs = bullet.GetComponent<EnemyBulletSettings>();
            //        Vector3 fireDirection = character.transform.position - gameObject.transform.position;
            //        fireDirection.y = Random.Range(fireDirection.y - firingOffset, fireDirection.y + firingOffset);
            //        bullet.transform.LookAt(character.transform, Vector3.forward);
            //        bullet.transform.Rotate(new Vector3(90, 0, 90));
            //        bullet.name = "EnemyBullet";
            //        Physics.IgnoreCollision(bullet.collider, gameObject.collider);
            //        bullet.rigidbody.AddForce(fireDirection.normalized * bossManager.force * 2 * typeForceMultiplier);
            //        bullet.rigidbody.freezeRotation = true;
            //        ebs.damage = bossManager.weaponPower;
            //    }
            //    yield return new WaitForSeconds(bossManager.firingDelay);
            //}
            //else yield return new WaitForSeconds(1);
            yield return new WaitForSeconds(5f);
        }
    }

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