using UnityEngine;
using System.Collections;

public class PlayerCollisions : MonoBehaviour {

    public AudioSource smack;

    [RPC]
    public void destroyObject()
    {
        Destroy(gameObject);
    }

    public static void Boom(GameObject gameObject) {
        GameObject explosionPrefab = (GameObject)Resources.Load("enemies/ExplosionPrefab");
        GameObject explosionPrefab2 = (GameObject)Resources.Load("enemies/ExplosionPrefab2");
        GameObject explosionPrefab3 = (GameObject)Resources.Load("enemies/ExplosionPrefab3");
        int randExplosion = Random.Range(1, 4);
        if (Network.isServer)
        {
            if (randExplosion == 1) Network.Instantiate(explosionPrefab, gameObject.transform.position, gameObject.transform.rotation, 0);
            else if (randExplosion == 2) Network.Instantiate(explosionPrefab2, gameObject.transform.position, gameObject.transform.rotation, 0);
            else if (randExplosion == 3) Network.Instantiate(explosionPrefab3, gameObject.transform.position, gameObject.transform.rotation, 0);
        }
    }

    public static void WeaponBoom(GameObject gameObject, int wepType)
    {
        GameObject beamCrackle = (GameObject)Resources.Load("weapons/beamCrackle");
        GameObject cannonCrackle = (GameObject)Resources.Load("weapons/cannonCrackle");
       // Vector3 standard = gameObject.transform.position;
        Vector3 asteroid = new Vector3(gameObject.transform.position.x - (0.7f*gameObject.transform.localScale.x), gameObject.transform.position.y, gameObject.transform.position.z);
        if (Network.isServer)
        {
            if (wepType == 1) Network.Instantiate(beamCrackle, asteroid, gameObject.transform.rotation,0);
            if (wepType == 2) Network.Instantiate(cannonCrackle, asteroid, gameObject.transform.rotation,0);
        }
    }

    void OnCollisionEnter(Collision collision) {
        GameObject collided = collision.collider.gameObject;
        //string collidedName = collided.name;
        string tag = collided.tag;

        switch (tag) {
            case "EnemyWeapon":
                // Do what we want for EnemyBullet
                Network.Destroy(collided);
                smack.Play();
                EnemyBulletSettings bulletSettings = collided.GetComponent<EnemyBulletSettings>();
                PlayerManager.hitPoints -= bulletSettings.damage;
                break;           
            default:
                // Do nothing!
                break;
        }
        if (PlayerManager.hitPoints <= 0) {
            Boom(gameObject);
            if (Network.isServer)
            {
                networkView.RPC("destroyObject", RPCMode.All);
                Destroy(gameObject);
            }
        }
    }

    void OnDestroy() {
        HudOn.gameOver = true;
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject enMan = GameObject.Find("EnemyManager");
        GameObject bullMan = GameObject.Find("BulletManager");
        Destroy(enMan);
        Destroy(bullMan);
        PlayerManager.hitPoints = 0;
    }
}