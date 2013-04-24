using UnityEngine;
using System.Collections;

public class BossLevelManager : MonoBehaviour {
    // Used to control the sending of a boss
    // Effectively Commander and LevelManager rolled into 1 for bosses

    private int universeN = 0;
    private Object[] bossPrefabs;
    private float leftMoveLimit;
    private Universe positions;

    void Start() {
        if (Network.isServer) {
            bossPrefabs = Resources.LoadAll("enemies/bosses", typeof(GameObject));
            positions = transform.parent.FindChild("OriginManager").GetComponent<Universe>();
            leftMoveLimit = positions.rightMovementLimit + 2.5f;
        }
    }

    // Creates an enemy of the given type
    public void CreateBoss(int type) {
        if (Network.isServer) {
            // ROTATION NEEDS TO GO HERE
            // Directions:
            // 1 - From Left
            // 2 - From Top
            // 3 - From Right
            // 4 - From Bottom
            int dir = 3;
            float x = 0, y = 0, z = 0;
            float genZ = positions.baseZ;
            switch (dir) {
                case 1:
                    x = positions.leftBorder - positions.generationOffset;
                    y = Random.Range(positions.bottomBorder, positions.topBorder);
                    z = genZ + 2;
                    break;
                case 2:
                    x = Random.Range(leftMoveLimit, positions.rightBorder);
                    y = positions.topBorder + positions.generationOffset;
                    z = genZ + 4;
                    break;
                case 3:
                    x = positions.rightBorder + positions.generationOffset;
                    y = Random.Range(positions.bottomBorder, positions.topBorder);
                    z = genZ + 6;
                    break;
                case 4:
                    x = Random.Range(leftMoveLimit, positions.rightBorder);
                    y = positions.bottomBorder - positions.generationOffset;
                    z = genZ + 8;
                    break;
                default:
                    break;
            }
            GameObject enemyPrefab = (GameObject)bossPrefabs[Random.Range(0, bossPrefabs.Length)];
            Transform enemy = (Transform)Network.Instantiate(enemyPrefab.transform, new Vector3(x, y, z), new Quaternion(0, 0, 0, 0), 100 + universeN);
            enemy.name = "Enemy" + universeN;
            enemy.transform.parent = transform.parent.parent.FindChild("Enemies");

            BossManager bMan = enemy.GetComponent<BossManager>();
            bMan.direction = dir;
            bMan.changeType(type);
        }
    }
}
