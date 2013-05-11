using UnityEngine;
using System.Collections;

public class BossLevelManager : MonoBehaviour {
    // Used to control the sending of a boss
    // Effectively Commander and LevelManager rolled into 1 for bosses

    private Object[] bossPrefabs;
    private Universe positions;
    private int universeN = 0; 
    private float leftMoveLimit;
    

    void Start() {
        if (Network.isServer) {
            bossPrefabs = Resources.LoadAll("enemies/bosses/Prefabs", typeof(GameObject));
            positions = transform.parent.FindChild("OriginManager").GetComponent<Universe>();
            leftMoveLimit = positions.rightMovementLimit + 2.5f;
        }
    }

    // Creates an enemy of the given type
    public void CreateBoss() {
        if (Network.isServer) {
            // Directions:
            // 1 - From Left
            // 2 - From Top
            // 3 - From Right
            // 4 - From Bottom
            int dir = 4;
            float x = 0, y = 0, z = 0;

            switch (dir) {
                case 1:
                    x = positions.origin.x + 60;
                    y = positions.origin.y; //Random.Range(positions.bottomBorder, positions.topBorder);
                    z = positions.baseZ; //positions.leftBorder - positions.generationOffset;;
                    break;
                case 2:
                    x = positions.origin.x + 60;
                    y = positions.origin.y + 100;
                    z = positions.baseZ; //Random.Range(leftMoveLimit, positions.rightBorder);
                    break;
                case 3:
                    x = positions.origin.x + 60;
                    y = Random.Range(positions.bottomBorder, positions.topBorder);
                    z = positions.baseZ; //positions.rightBorder + positions.generationOffset;
                    break;
                case 4:
                    x = positions.origin.x + 60;
                    y = positions.origin.y - 100;
                    z = positions.baseZ; //Random.Range(leftMoveLimit, positions.rightBorder);
                    break;
                default:
                    break;
            }
            GameObject bossPrefab = (GameObject)bossPrefabs[Random.Range(0, bossPrefabs.Length)];
            Transform boss = (Transform)Network.Instantiate(bossPrefab.transform, new Vector3(x, y, z), Quaternion.Euler(0, 270, 0), 100 + universeN);
            boss.transform.parent = transform.parent.parent.FindChild("Enemies");

            boss.name = "boss0";
            EyeBossManager bMan = boss.GetComponent<EyeBossManager>();  
            bMan.direction = dir;
        }
    }

    public void BossDestroyed() {
        if (Network.isServer) {
            GameObject.Find("Main Camera").GetComponent<ServerScoringSystem>().BossCleared();
        }
    }
}
