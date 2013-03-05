using UnityEngine;
using System.Collections;

public class CivilianManager : MonoBehaviour {
	
	public Transform civPrefab;
	public Transform busPrefab;
    public GameObject lookTarget1, lookTarget2, lookTarget3, lookTarget4;
	public int density; // How busy the scene is
	private int force; 

	void Start () {
		for (int i = 0; i < density; i++) {
            CreateCiv();
        }
	}
	
	void CreateCiv() {
		Vector3 forceDir = Vector3.zero;
        int dir = Random.Range(1, 5);
        float x = 0, y = 0, z = 0;
		Transform civ;
		Transform shipPrefab;
		
		int shipType = Random.Range (1,5);
        if (shipType == 2) shipPrefab = busPrefab;
        else shipPrefab = civPrefab;
		
        switch (dir) {
            case 1: // Coming past the planet on main screen going right
                x = Random.Range(-110,-60);
                y = Random.Range(-10, 10);
                z = Random.Range(40,50);
				forceDir = new Vector3(1.2f,0f,0.4f);
				force = Random.Range (80,150);
				civ = (Transform)Instantiate(shipPrefab, new Vector3(x, y, z), new Quaternion(0, 0, 0, 0));
				civ.rigidbody.AddForce(forceDir * force);
				civ.transform.rotation = Quaternion.LookRotation(new Vector3(-x,y,z));
				civ.name = "Civilian (right)";
                break;
            case 2: // Going away from the camera on the right
                x = Random.Range(94,144);
                y = Random.Range(-20, 20);
                z = Random.Range(40,50);
				forceDir = new Vector3(-1f,0f,1.7f);
				force = Random.Range (80,100);
				civ = (Transform)Instantiate(shipPrefab, new Vector3(x, y, z), new Quaternion(0, 0, 0, 0));
				civ.rigidbody.AddForce(forceDir * force);
				civ.transform.rotation = Quaternion.LookRotation(new Vector3(-x,y,z+150));
				civ.name = "Civilian (left)";
                break;
            case 3: // Coming diagonally down
                x = Random.Range(-90,-40);
                y = Random.Range(-30, -50);
                z = Random.Range(70,60);
				forceDir = new Vector3(2f,-0.6f,-0.4f);
				force = Random.Range (80,150);
				civ = (Transform)Instantiate(shipPrefab, new Vector3(x, y, z), new Quaternion(0, 0, 0, 0));
				civ.rigidbody.AddForce(forceDir * force);
				civ.transform.rotation = Quaternion.LookRotation(new Vector3(65,-40,-30));
				civ.name = "Civilian (diag down)";
                break;
            case 4: // Coming diagonally up
                x = Random.Range(100,150);
                y = Random.Range(-80, -100);
                z = Random.Range(40,50);
				forceDir = new Vector3(-1.4f,0.7f,0f);
				force = Random.Range (50,250);
				civ = (Transform)Instantiate(shipPrefab, new Vector3(x, y, z), new Quaternion(0, 0, 0, 0));
				civ.rigidbody.AddForce(forceDir * force);
                civ.transform.rotation = Quaternion.LookRotation(lookTarget1.transform.position, Vector3.up);
				civ.name = "Civilian (diag up)";
                break;
            default:
                break;
        }
	}

	void Update () {
		int civCount = GameObject.FindGameObjectsWithTag("Civilian").Length;

        if (civCount < density) {
            for (int i = 0; i < (density - civCount); i++) {
                CreateCiv();
            }
        }
	}

}