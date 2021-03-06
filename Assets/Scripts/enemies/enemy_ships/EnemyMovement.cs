using UnityEngine;
using System.Collections;

public class EnemyMovement : MonoBehaviour {

    public Transform lightWeapon, mediumWeapon, heavyWeapon, superheavyWeapon;
    private Transform bulletPrefab;
    private GameObject explosionPrefab, explosionPrefab2, explosionPrefab3;
    private Universe positions;
    private Commander commander;

    private float stop = 5;
    public Server serv;

    private bool inPlane = false;
    private bool waiting = false;

    EnemyManager eManager;

    public Vector3 randPos;
    public Vector3 startPos;

    // Enemy positioning stats
    private float minX;
    private float maxX;
    private float minY;
    private float maxY;
    private float stopZ;

    // Bullet spawn transform and firing offset
    private Transform spawn;
    private float firingOffset = 1.5f;

    private float typeForceMultiplier;
    private int universeNb;
    // RPC Calls to the Client

    [RPC]
    void modifyName(string name)
    {
        gameObject.name = name;
        universeNb = int.Parse(name.Substring(name.Length - 1, 1));
        gameObject.transform.parent = GameObject.Find("Universe" + universeNb + "Enemies").transform;
        eManager = gameObject.GetComponent<EnemyManager>();
        setUpEnemy();

        int enemyType = eManager.enemyType;
        switch (enemyType)
        {
            case 1: bulletPrefab = lightWeapon; typeForceMultiplier = 2.2f; break;
            case 2: bulletPrefab = mediumWeapon; typeForceMultiplier = 1.5f; break;
            case 3: bulletPrefab = heavyWeapon; typeForceMultiplier = 0.6f; break;
            case 4: bulletPrefab = superheavyWeapon; typeForceMultiplier = 0.3f; break;
            default: break;
        }

		positions = transform.parent.parent.FindChild("Managers/OriginManager").GetComponent<Universe>();
        universeNb = int.Parse(name.Substring(name.Length-1, 1));
        Vector3 forceDir = Vector3.zero;
        eManager = gameObject.GetComponent<EnemyManager>();

        commander = transform.parent.parent.FindChild("Managers/EnemyManager").GetComponent<Commander>();
        setUpEnemy();

        enemyType = eManager.enemyType;
        switch (enemyType)
        {
            case 1: bulletPrefab = lightWeapon; typeForceMultiplier = 2.2f; break;
            case 2: bulletPrefab = mediumWeapon; typeForceMultiplier = 1.5f; break;
            case 3: bulletPrefab = heavyWeapon; typeForceMultiplier = 0.6f; break;
            case 4: bulletPrefab = superheavyWeapon; typeForceMultiplier = 0.3f; break;
            default: break;
        }

        // Set movement variables
        minX = positions.rightMovementLimit + 2.5f;
        maxX = positions.rightXBorder;
        minY = positions.bottomBorder;
        maxY = positions.topBorder;

        stopZ = positions.baseZ;

        // Check direction
        switch (eManager.direction)
        {
            case 1:
                forceDir = Vector3.right;
                stop = Random.Range(minX, maxX);
                break;
            case 2:
                forceDir = Vector3.down;
                stop = Random.Range(minY, maxY);
                break;
            case 3:
                forceDir = Vector3.left;
                stop = Random.Range(minX, maxX);
                break;
            case 4:
                forceDir = Vector3.up;
                stop = Random.Range(minY, maxY);
                break;
            default:
                break;
        }
        gameObject.rigidbody.AddForce(forceDir * eManager.force);
		if (Network.isServer)
        	StartCoroutine(Shoot());
        StartCoroutine(LerpEnemy());
    }

    void Start() {
        if (Network.isServer) {
            networkView.RPC("modifyName", RPCMode.All, gameObject.name);
            serv = GameObject.Find("Network").GetComponent<Server>();
        }
        spawn = transform.Find("BulletSpawn");
		eManager = gameObject.GetComponent<EnemyManager>();
        //eManager.InitStats();
    }

    void setUpEnemy() {     
        gameObject.rigidbody.freezeRotation = true;
    }

    int PickTarget() {
        ArrayList activeChars = new ArrayList();
        for (int i = 1; i < serv.playerLocations.Length; i++) {
            if (serv.playerLocations[i] == universeNb) {
                activeChars.Add(i);
            }
        }
        if (activeChars.Count == 0) return -1;
        int index = Random.Range(0, activeChars.Count);
        return (int) activeChars[index];
    }

	[RPC]
	private void PlayEnemyFire(string wDir) {
		if (Network.isClient) {
			GameObject.Find("Client Scripts").GetComponent<BGMusic>().PlayShot(wDir);
		}
	}

    // Used for firing
    IEnumerator Shoot() {
        while (true) {
            if (inPlane) {
                int targetPlayer = PickTarget();
                if (targetPlayer != -1) {
					GameObject character = GameObject.Find("Character" + targetPlayer);
					NetworkViewID targetID = character.GetComponent<NetworkView>().viewID;
					Vector3 fireDirection = character.transform.position - spawn.position;
					Vector3 force = fireDirection.normalized * eManager.force * 2 * typeForceMultiplier;
        			fireDirection.y = Random.Range(fireDirection.y - firingOffset, fireDirection.y + firingOffset);
					Transform bullet = (Transform)Network.Instantiate(bulletPrefab, spawn.position, gameObject.transform.rotation, 200);
					NetworkViewID bulletID = bullet.networkView.viewID;
					networkView.RPC("fireBullet", RPCMode.All, gameObject.transform.position, gameObject.transform.rotation, targetID, bulletID, fireDirection, force);
					networkView.RPC("PlayEnemyFire", RPCMode.All, "beam/beam6");
                }
                yield return new WaitForSeconds(eManager.firingDelay);
            }
            else yield return new WaitForSeconds(1);
        }
    }

	[RPC]
	void fireBullet(Vector3 startPosition, Quaternion startRotation, NetworkViewID targetID, NetworkViewID bulletID, Vector3 fireDir, Vector3 force) {
		// Get target
		GameObject character = NetworkView.Find (targetID).gameObject;

		GameObject bullet = NetworkView.Find (bulletID).gameObject;
        EnemyBulletSettings ebs = bullet.GetComponent<EnemyBulletSettings>();

        bullet.transform.LookAt(character.transform, Vector3.forward);
        bullet.transform.Rotate(new Vector3(90, 0, 90));
        bullet.name = "EnemyBullet";
        Physics.IgnoreCollision(bullet.collider, gameObject.collider);
        bullet.rigidbody.AddForce(force);
        bullet.rigidbody.freezeRotation = true;
        ebs.damage = eManager.weaponPower;
	}

    IEnumerator LerpEnemy() {
        startPos = gameObject.transform.position;
		float xPos = Random.Range(minX, maxX);
		float yPos = Random.Range(minY, maxY);
		networkView.RPC ("setTargetPosition", RPCMode.Others, xPos, yPos);
		networkView.RPC ("setWaitingVar", RPCMode.Others, true);
        randPos = new Vector3(xPos, yPos, stopZ);

        waiting = true;
        yield return new WaitForSeconds(eManager.moveDelay);
		networkView.RPC ("setWaitingVar", RPCMode.Others, false);
        waiting = false;
    }

	[RPC]
	void setTargetPosition(float x, float y) {
		randPos = new Vector3(x, y, stopZ);
	}

	[RPC]
	void setWaitingVar(bool val) {
		waiting = val;
	}



    void move(float minMove, float maxMove) {
        if (minMove > maxMove && gameObject.transform.position.z > stopZ) {
            gameObject.rigidbody.velocity = new Vector3(0, 0, 0);
            gameObject.rigidbody.AddForce(new Vector3(0, 0, -1) * eManager.force);
        }
        if (gameObject.transform.position.z <= stopZ) {
            gameObject.rigidbody.velocity = new Vector3(0, 0, 0);
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, stopZ);
            inPlane = true;
        }
        if (inPlane) {
            if (waiting) gameObject.transform.position = Vector3.Lerp(transform.position, randPos, Time.deltaTime * eManager.speed);
            else if (Network.isServer) StartCoroutine(LerpEnemy());
        }
    }

    void Update() {
		if (eManager == null)
			return;

        switch (eManager.direction)
        {
            case 1:
                move(gameObject.transform.position.x, stop);
                break;
            case 2:
                move(stop, gameObject.transform.position.y);
                break;
            case 3:
                move(stop, gameObject.transform.position.x);
                break;
            case 4:
                move(gameObject.transform.position.y, stop);
                break;
            default:
                break;
            }
    }
}