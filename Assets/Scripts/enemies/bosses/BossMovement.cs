using UnityEngine;
using System.Collections;

public class BossMovement : MonoBehaviour {

    private Universe positions;
    private EyeBossManager bossManager;     

    // Enemy positioning stats
    private float minZ;
    private float maxZ;
    private float minY;
    private float maxY;
    private float stop = 5;
    private float stopY;
    private Vector3 forceDir;
    public Vector3 startPos;
    public Vector3 randPos;
   
    // Helper variables
    public bool inPlane = false;
    private bool waiting = false;
    private float firingOffset = 1.5f;
    private float typeForceMultiplier;
    private int universeNb;

    // RPC Calls to the Client
    [RPC]
    void modifyName(string name) {
        gameObject.name = name;
        universeNb = int.Parse(name.Substring(name.Length - 1, 1));
        gameObject.transform.parent = GameObject.Find("Universe" + universeNb + "Enemies").transform;
        bossManager = GetComponent<EyeBossManager>();

        //int enemyType = bossManager.enemyType;
        //switch (enemyType) {
        //    case 1: bulletPrefab = lightWeapon; typeForceMultiplier = 2.2f; break;
        //    case 2: bulletPrefab = mediumWeapon; typeForceMultiplier = 1.5f; break;
        //    case 3: bulletPrefab = heavyWeapon; typeForceMultiplier = 0.6f; break;
        //    case 4: bulletPrefab = superheavyWeapon; typeForceMultiplier = 0.3f; break;
        //    default: break;
        //}
    }

    void Start() {
        if (Network.isServer) {
            networkView.RPC("modifyName", RPCMode.All, gameObject.name);
            positions = transform.parent.parent.FindChild("Managers/OriginManager").GetComponent<Universe>();
            universeNb = int.Parse(gameObject.name.Substring(gameObject.name.Length - 1, 1));
            bossManager = GetComponent<EyeBossManager>();

            // Set movement variables
            minZ = positions.origin.z - 25;
            maxZ = positions.origin.z + 25;
            minY = positions.origin.y - 25;
            maxY = positions.origin.y + 25;
            
            stopY = 0;

            // NEED to do this not based on position, but on a FIXED stopZ (due to rotation issues)
            /*GameObject character = GameObject.Find("Character"+universeNb);
            stopZ = character.transform.position.z;*/

            // Check direction
            switch (bossManager.direction) {
                case 1:
                    forceDir = Vector3.right;
                    stop = positions.origin.z;
                    break;
                case 2:
                    forceDir = Vector3.down;
                    stop = positions.origin.y;
                    break;
                case 3:
                    forceDir = Vector3.left;
                    stop = positions.origin.z;
                    break;
                case 4:
                    forceDir = Vector3.up;
                    stop = positions.origin.y;
                    break;
                default:
                    break;
            }
            //StartCoroutine(LerpEnemy());
        }
    }

    IEnumerator LerpEnemy() {
        startPos = transform.position;
        randPos = new Vector3(positions.origin.x + 100, Random.Range(minY, maxY), Random.Range(minZ, maxZ));
        waiting = true;
        yield return new WaitForSeconds(bossManager.moveDelay);
        waiting = false;
    }

    void move(float startMarker, float endMarker) {
        //if (minMove > maxMove && transform.position.y > stopY) {
        //    rigidbody.velocity = new Vector3(0, 0, 0);
        //    rigidbody.AddForce(new Vector3(0, -1, 0) * bossManager.force);
        //}
        if (!inPlane) {
            if (transform.position.y < stopY) {
                rigidbody.velocity = new Vector3(0, 0, 0);
                rigidbody.AddForce(forceDir * bossManager.forceMultiplier);
                //transform.position = Vector3.Lerp(transform.position, transform.position + new Vector3(0, stopY, 0), Time.deltaTime * bossManager.speed);
                //transform.position = new Vector3(transform.position.x, stopY, transform.position.z);
            }
            else {
                rigidbody.velocity = new Vector3(0, 0, 0);
                inPlane = true;
                bossManager.inPlane = true;
                bossManager.rotation = 10f;
            }       
        }
        else {
            transform.RotateAround(transform.position, transform.forward, Time.deltaTime * bossManager.rotation);
        //    if (waiting) transform.position = Vector3.Lerp(transform.position, randPos, Time.deltaTime * bossManager.speed);
        //    else StartCoroutine(LerpEnemy());
        }
    }

    void Update() {
        if (Network.isServer) {
            switch (bossManager.direction) {
                case 1:
                    move(transform.position.z, stop);
                    break;
                case 2:
                    move(stop, transform.position.y);
                    break;
                case 3:
                    move(stop, transform.position.z);
                    break;
                case 4:
                    move(transform.position.y, stop);
                    break;
                default:
                    break;
            }
        }
    }
}