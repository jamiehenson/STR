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
    private Vector3 pivot;
    private Vector3 forceDir;
    public Vector3 startPos;
    public Vector3 randPos;
   
    // Helper variables
    public bool inPlane = false;
    private bool waiting = false;
    private float typeForceMultiplier;
    private int universeNb;

    // RPC Calls to the Client
    [RPC]
    void modifyName(string name) {
        gameObject.name = name;
        universeNb = int.Parse(name.Substring(name.Length - 1, 1));
        gameObject.transform.parent = GameObject.Find("Universe" + universeNb + "Enemies").transform;
        bossManager = GetComponent<EyeBossManager>();
    }

    void Start() {
        if (Network.isServer) {
            networkView.RPC("modifyName", RPCMode.All, gameObject.name);
            positions = transform.parent.parent.FindChild("Managers/OriginManager").GetComponent<Universe>();
            universeNb = int.Parse(gameObject.name.Substring(gameObject.name.Length - 1, 1));
            bossManager = GetComponent<EyeBossManager>();

            // Set movement variables
            minZ = -15f;
            maxZ = 15f;
            minY = -5f;
            maxY = 5f;
            stopY = 0f;
            pivot = new Vector3(transform.position.x, stopY, transform.position.z);

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
            iTween.MoveTo(gameObject, new Vector3(transform.position.x, stopY+10.0f, transform.position.z), 8.0f);
            inPlane = true;
            bossManager.inPlane = true;
            bossManager.rotation = 10f;
            StartCoroutine(Spin());
            //StartCoroutine(Move());
        }
    }

    IEnumerator Spin() {
        while (true) {
            transform.RotateAround(pivot, Vector3.left, Time.deltaTime * Random.Range(bossManager.speed/2, bossManager.speed*2));
            transform.RotateAround(transform.position, transform.forward, Time.deltaTime * bossManager.rotation);
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator Move() {
        while (true) {
            randPos = new Vector3(transform.position.x, Random.Range(minY, maxY), Random.Range(minZ, maxZ));
            iTween.MoveTo(gameObject, randPos, 3.0f);
            //waiting = true;
            yield return new WaitForSeconds(bossManager.moveDelay);
            //waiting = false;
        }
    }

    void move(float startPos, float endPos, float time) {
        //if (minMove > maxMove && transform.position.y > stopY) {
        //    rigidbody.velocity = new Vector3(0, 0, 0);
        //    rigidbody.AddForce(new Vector3(0, -1, 0) * bossManager.force);
        //}
        if (!inPlane) {
            if (transform.position.y < stopY) {

                float i = 0.0f;
                float rate = 1.0f / 5.0f;
                Vector3 stopPos = new Vector3(transform.position.x, stopY, transform.position.y);
                while (i < 1.0f) {
                    i += Time.deltaTime * rate;
                    transform.position = Vector3.Lerp(transform.position, stopPos, i);
                }
                //rigidbody.velocity = new Vector3(0, 0, 0);
                //rigidbody.AddForce(forceDir * 1000);
                
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
}