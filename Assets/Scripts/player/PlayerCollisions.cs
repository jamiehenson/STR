using UnityEngine;
using System.Collections;

public class PlayerCollisions : MonoBehaviour {

    public AudioSource smack;
    PlayerManager manager;

    [RPC]
    public void destroyObject()
    {
        Destroy(gameObject);
    }

    private int universeN()
    {
        int length = transform.name.Length;
        string num = transform.name.Substring(length - 1, 1);
        if ("0123456789".Contains(num)) return (int.Parse(num));
        else return -1;
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
		if (Network.isClient)
			return;
        manager = GameObject.Find("Character" + universeN()).GetComponent<PlayerManager>();
        GameObject collided = collision.collider.gameObject;
        //string collidedName = collided.name;
        string tag = collided.tag;

        switch (tag) {
            case "EnemyWeapon":
                // Do what we want for EnemyBullet
                Network.Destroy(collided);
               // smack.Play();
                EnemyBulletSettings bulletSettings = collided.GetComponent<EnemyBulletSettings>();
                manager.updateHitPoints(-bulletSettings.damage);
                break;           
            default:
                // Do nothing!
                break;
        }
      /*  if (manager.getHitPoints() <= 0)
        {
            Boom(gameObject);
            if (Network.isServer)
            {
                networkView.RPC("destroyObject", RPCMode.All);
                Network.Destroy(gameObject);
            }
        }*/
    }

    void OnDestroy() {
        HudOn.gameOver = true;
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject enMan = GameObject.Find("EnemyManager");
        GameObject bullMan = GameObject.Find("BulletManager");
        Destroy(enMan);
        Destroy(bullMan);
        // Last line throws errors when server shuts down. Don't panic.
        //manager.resetHitPoints(0);
    }
}