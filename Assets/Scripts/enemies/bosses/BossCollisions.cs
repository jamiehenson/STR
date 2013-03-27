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

    void Start() {
        eManager = gameObject.GetComponent<BossManager>();
        eMove = gameObject.GetComponent<BossMovement>();
        health = eManager.health;
        enemyBar = HudOn.fillTex(60, 10, new Color(1f, 0f, 0f, 1f));
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
		
        GameObject collided = other.gameObject;
        string collidedName = collided.name;
        switch (collidedName) {
            case "Beam":
                // Do what we want for beam
                Destroy(collided);
                PlayerCollisions.WeaponBoom(gameObject, 1);
                beamSmack.Play();
                health = health - (WeaponHandler.beamDamage);
                break;
            case "Cannon":
                // Do what we want for cannon
                Destroy(collided);
                PlayerCollisions.WeaponBoom(gameObject, 2);
                cannonSmack.Play();
                iTween.MoveBy(gameObject, eManager.speed * (collided.rigidbody.velocity / 7), 1f);
                health = health - (WeaponHandler.cannonDamage);
                break;
            case "Mine":
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
                Destroy(collided);
                beamSmack.Play();
                health = health - (WeaponHandler.mineDamage);
                break;
            case "Mine Fragment":
                beamSmack.Play();
                health = health - (WeaponHandler.mineFragmentDamage);
                Destroy(collided);
                break;
            case "Player":
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
            int points = eManager.killPoints;
            Destroy(gameObject);
            PlayerCollisions.Boom(gameObject);
            HudOn.score += points;
            StartCoroutine(XP("+" + points));
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

    void OnDestroy() {
        GameObject levManObj = GameObject.Find("LevelManager");
        LevelManager levMan = levManObj.GetComponent<LevelManager>();
        levMan.BossCleared();
    }
}
