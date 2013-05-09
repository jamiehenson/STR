using UnityEngine;
using System.Collections;

public class PlayerCollisions : MonoBehaviour {

    public AudioSource smack;
    PlayerManager manager;

    void Start() {
        if (Network.isServer) {
            manager = GetComponent<PlayerManager>();
        }
    }

    [RPC]
    public void destroyObject()
    {
        Destroy(gameObject);
    }

    // This function actually returns the player's number
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
        if (Network.isServer) {
            GameObject collided = collision.collider.gameObject;
            string tag = collided.tag;

            switch (tag) {
                case "EnemyWeapon":
                    Network.Destroy(collided);
                    // smack.Play();
                    EnemyBulletSettings bulletSettings = collided.GetComponent<EnemyBulletSettings>();
                    manager.updateHitPoints(-bulletSettings.damage);
                    break;
                case "BossBeam":
                    Debug.Log("BossBeam collided!");
                    EnemyBulletSettings ebs = collided.GetComponent<EnemyBulletSettings>();
                    manager.updateHitPoints(manager.getHitPoints() * -ebs.damage);
                    break;
                default:
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
            /*if (manager.getHitPoints() <= 0) {
                Boom(gameObject);
                gameObject.SetActive(false);
                networkView.RPC("ChangePlayerActiveState", RPCMode.Others, false);
                StartCoroutine("DeathTimeout");
            }*/
        }
    }

    void OnCollisionStay(Collision collision) {
        //if (Network.isServer) {
            GameObject collided = collision.collider.gameObject;

            switch (collided.tag) {
                case "BossBeam":
                    EnemyBulletSettings ebs = collided.GetComponent<EnemyBulletSettings>();
                    manager.updateHitPoints(manager.getHitPoints() * -ebs.damage);
                    break;
                default:
                    break;
            }
            //if (manager.getHitPoints() <= 0) {
            //    Boom(gameObject);
            //    Network.Destroy(gameObject);
            //}
        //}
    }

    IEnumerator DeathTimeout() {
        yield return new WaitForSeconds(3);
        gameObject.SetActive(true);
        networkView.RPC("ChangePlayerActiveState", RPCMode.Others, true);
    }

    [RPC]
    void ChangePlayerActiveState(bool active) {
        if (Network.isClient) {
            gameObject.SetActive(active);
        }
    }

    void OnDestroy() {
        HudOn.gameOver = true;
        GameObject enMan = GameObject.Find("EnemyManager");
        GameObject bullMan = GameObject.Find("BulletManager");
        Destroy(enMan);
        Destroy(bullMan);
        // Last line throws errors when server shuts down. Don't panic.
        //manager.resetHitPoints(0);
    }
}