using UnityEngine;
using System.Collections;

public class BossCollisions : MonoBehaviour {

    private BossManager eManager;
    private BossMovement eMove;

    private float health;
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
        eManager = gameObject.GetComponent<BossManager>();
        eMove = gameObject.GetComponent<BossMovement>();
        health = eManager.health;
        enemyBar = HudOn.fillTex(60, 10, new Color(1f, 0f, 0f, 1f));
    }

    private int universeN() {
        int length = transform.parent.parent.name.Length;
        string num = transform.parent.parent.name.Substring(length - 1, 1);
        if ("0123456789".Contains(num)) return (int.Parse(num));
        else return -1;
    }

    void Update() {
        Vector3 viewPos = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        screenX = viewPos.x;
        screenY = Screen.height - (viewPos.y + 1);
        remainingHealth = health / eManager.health;
    }

    void OnTriggerEnter(Collider other) {
        if (Network.isClient)
            return;
        bool hit = false;
        if (universeN() != -1) {

            GameObject collided = other.gameObject;
            // Need to switch from name-based system to tag-based
            string collidedTag = collided.tag;
            string characterNum = collided.name.Substring(collided.name.Length - 1, 1);
            if ("0123456789".Contains(characterNum)) manager = GameObject.Find("Character" + characterNum).GetComponent<PlayerManager>();
            switch (collidedTag) {
                case "PlayerBeam":
                    // Do what we want for beam
                    Network.Destroy(collided);
                    PlayerCollisions.WeaponBoom(gameObject, 1);
                    //beamSmack.Play();
                    health = health - (WeaponHandler.beamDamage);
                    hit = true;
                    break;
                case "PlayerCannon":
                    // Do what we want for cannon
                    Network.Destroy(collided);
                    PlayerCollisions.WeaponBoom(gameObject, 2);
                    //cannonSmack.Play();
                    iTween.MoveBy(gameObject, eManager.speed * (collided.rigidbody.velocity / 7), 1f);
                    health = health - (WeaponHandler.cannonDamage);
                    hit = true;
                    break;
                case "PlayerMine":
                    // Do what we want for mine
                    for (int i = 0; i < 40; i++) {
                        Transform fragment = (Transform)Instantiate(MineFrag, gameObject.transform.position, Random.rotation);
                        fragment.name = "Mine Fragment";
                        Physics.IgnoreCollision(fragment.collider, gameObject.collider);
                        fragment.rigidbody.AddForce((Random.insideUnitSphere.normalized * 2) * eManager.force);
                    }
                    for (int i = 0; i < 20; i++) {
                        Transform fragment = (Transform)Instantiate(MineFrag, gameObject.transform.position, Random.rotation);
                        fragment.name = "Mine Fragment";
                        Physics.IgnoreCollision(fragment.collider, gameObject.collider);
                        fragment.rigidbody.AddForce((Random.insideUnitCircle.normalized) * eManager.force);
                    }
                    Network.Destroy(collided);
                    beamSmack.Play();
                    health = health - (WeaponHandler.mineDamage);
                    hit = true;
                    break;
                case "MineFrag":
                    beamSmack.Play();
                    health = health - (WeaponHandler.mineFragmentDamage);
                    Network.Destroy(collided);
                    hit = true;
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
                int scoreAddition = (int)(100 * transform.localScale.x);
                if ("0123456789".Contains(characterNum)) {
                    networkView.RPC("scoreXP", RPCMode.All, int.Parse(characterNum), scoreAddition);
                }
                manager.updateScore(scoreAddition);
                int points = eManager.killPoints;
                Network.Destroy(gameObject);
                PlayerCollisions.Boom(gameObject);
                //HudOn.score += points;
                // StartCoroutine(XP("+" + points));
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
        yield return new WaitForSeconds(2f);
    }

    void OnGUI() {
        if (remainingHealth != 1) GUI.Label(new Rect(screenX - 30, screenY - 30, remainingHealth * 60, 30), enemyBar);
    }

    [RPC]
    void scoreXP(int camNum, int score) {
        if (Network.isClient && GameObject.Find("Camera " + camNum)) {
            StartCoroutine(XP("+" + score));

        }

    }

    void OnDestroy() {
        if (Network.isServer) {
            GameObject.Find("Main Camera").GetComponent<ServerScoringSystem>().BossCleared();
        }
    }
}
