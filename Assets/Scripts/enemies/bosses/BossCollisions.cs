using UnityEngine;
using System.Collections;

public class BossCollisions : MonoBehaviour {

    private EyeBossManager eManager;
    private BossMovement eMove;

    private float health = 300;
    private float remainingHealth = 1;

    public AudioSource beamSmack;
    public AudioSource cannonSmack;

    public Transform MineFrag;

    private float screenX, screenY;
    private Texture2D enemyBar;
    private GameObject xp;

    PlayerManager manager;
    private bool showScore;

    void Start() {
        eManager = GetComponent<EyeBossManager>();
        eMove    = GetComponent<BossMovement>();
        health   = eManager.health;
        enemyBar = HudOn.fillTex(60, 10, new Color(1f, 0f, 0f, 1f));
        HudOn.bossOn = true;
    }

    [RPC]
    private void UpdateBossHealth(float rHealth) {
        if (Network.isClient) {
            GameObject.Find("Client Scripts").GetComponent<HudOn>().BossHealthUpdate(rHealth);
        }
    }

    private int universeN() {
        int length = transform.parent.parent.name.Length;
        string num = transform.parent.parent.name.Substring(length - 1, 1);
        if ("0123456789".Contains(num)) return (int.Parse(num));
        else return -1;
    }

    void Update() {
        Vector3 viewPos = Camera.main.WorldToScreenPoint(transform.position);
        screenX = viewPos.x;
        screenY = Screen.height - (viewPos.y + 1);
        remainingHealth = health / eManager.health;
        networkView.RPC("UpdateBossHealth", RPCMode.Others, remainingHealth);
        if (health < eManager.health * 0.25) {
            eManager.rotation    = 80f;
            eManager.firingDelay = 0.2f;
        }
        else if (health < eManager.health * 0.5) {
            eManager.rotation    = 40f;
            eManager.firingDelay = 0.6f;
        }
        else if (health < eManager.health * 0.75) {
            eManager.rotation    = 20f;
            eManager.firingDelay = 1.0f;
        }
    }

    void OnTriggerEnter(Collider other) {
        if (Network.isClient)
            return;
        if (universeN() != -1) {

            GameObject collided = other.gameObject;
            string collidedTag = collided.tag;
            string characterNum = collided.name.Substring(collided.name.Length - 1, 1);
            if ("0123456789".Contains(characterNum)) manager = GameObject.Find("Character" + characterNum).GetComponent<PlayerManager>();
            switch (collidedTag) {
                case "PlayerBeam":
                    // Do what we want for beam
                    Network.Destroy(collided);
                    PlayerCollisions.WeaponBoom(gameObject, 1);
                    //beamSmack.Play();
                    if (eManager.irisOpen) health = health - (WeaponHandler.beamDamage);
                    break;
                case "PlayerCannon":
                    // Do what we want for cannon
                    Network.Destroy(collided);
                    PlayerCollisions.WeaponBoom(gameObject, 2);
                    //cannonSmack.Play();
                    if (eManager.irisOpen) health = health - (WeaponHandler.cannonDamage);
                    break;
                case "PlayerMine":
                    // Do what we want for mine
                    for (int i = 0; i < 40; i++) {
                        Transform fragment = (Transform)Instantiate(MineFrag, gameObject.transform.position, Random.rotation);
                        fragment.name = "Mine Fragment";
                        Physics.IgnoreCollision(fragment.collider, gameObject.collider);
                        fragment.rigidbody.AddForce((Random.insideUnitSphere.normalized * 2) * eManager.forceMultiplier);
                    }
                    for (int i = 0; i < 20; i++) {
                        Transform fragment = (Transform)Instantiate(MineFrag, gameObject.transform.position, Random.rotation);
                        fragment.name = "Mine Fragment";
                        Physics.IgnoreCollision(fragment.collider, gameObject.collider);
                        fragment.rigidbody.AddForce((Random.insideUnitCircle.normalized) * eManager.forceMultiplier);
                    }
                    Network.Destroy(collided);
                    beamSmack.Play();
                    if (eManager.irisOpen) health = health - (WeaponHandler.mineDamage);
                    break;
                case "MineFrag":
                    beamSmack.Play();
                    if (eManager.irisOpen) health = health - (WeaponHandler.mineFragmentDamage);
                    Network.Destroy(collided);
                    break;
                case "Enemy":
                    // Do what we want for hitting anther enemy (not yet perfected)
                    eMove.randPos = eMove.startPos + eMove.randPos;
                    eMove.startPos = eMove.randPos - eMove.startPos;
                    eMove.randPos = eMove.randPos - eMove.startPos;
                    break;
                default:
                    // Do nothing!
                    break;
            }
            if (health <= 0) {
                if ("0123456789".Contains(characterNum)) {
                    networkView.RPC("scoreXP", RPCMode.All, int.Parse(characterNum), eManager.killPoints);
                }
                manager.updateScore(eManager.killPoints);
                GameObject explosionPrefab = (GameObject)Resources.Load("enemies/bosses/BossExplosion");
                if (Network.isServer) {
                    Network.Instantiate(explosionPrefab, transform.position, transform.rotation, 0);
                    Network.Destroy(gameObject);
                } 
            }
        }
    }

    IEnumerator XP(string points) {
        xp = new GameObject("XP");
        xp.AddComponent("GUIText");
        xp.guiText.font = (Font)Resources.Load("Belgrad");
        xp.guiText.fontSize = 28;
        Vector3 placeOfDeath = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        xp.transform.position = new Vector3(placeOfDeath.x / Screen.width, placeOfDeath.y / Screen.height, 0.5f);
        xp.guiText.anchor = TextAnchor.MiddleCenter;
        xp.guiText.text = points;
        xp.guiText.material.color = Color.white;
        iTween.FadeTo(xp, 0f, 2f);
        yield return new WaitForSeconds(2.5f);
        Destroy(xp);
        yield return new WaitForSeconds(5f);
    }

    [RPC]
    void scoreXP(int camNum, int score) {
        if (Network.isClient && GameObject.Find("Camera " + camNum)) {
            StartCoroutine(XP("+" + score));
        }
    }

    void OnDestroy() {
        HudOn.bossOn = false;
        if (Network.isServer) {
            GameObject.Find("Universe" + (0) + "/Managers/LevelManager").GetComponent<BossLevelManager>().BossDestroyed();
        }
    }
}
