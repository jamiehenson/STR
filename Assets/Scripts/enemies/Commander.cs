/* HOW GENERATION SHOULD WORK:
 * 
 * 1) Pick a number from 1 to 5
 * 2) If 5, send asteroid belt, otherwise send enemy wave
 * 3) Wait until asteroid belt cleared, or if enemies 1 left (or set time elapsed)
 * 4) Repeat from Step 1
 * 
 * Script enabled from Universe.cs
 * */


using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Commander : MonoBehaviour {
    
    // For keeping track of numbers
    public static int[] asteroidCount;
    public static int[] enemyCount;

    public Transform enemyPrefab;
    public Transform asteroidPrefab;
    public Transform bossPrefab;
    private bool levelStarted = false;
    public bool bossDeployed = false;

    private float minAstScale = 0.0f;
    private float maxAstScale = 1.5f;
    private int fadeWait = 2;
    private float beltGap = 1f;
    private int astProb = 8;

    private float leftMoveLimit;

    // Enemy generation positioning stats
    private float astXOffsetRange = 2.5f;

    // Difficulty stats (numbered for selection, some merged into 1 "setting")
    private int numDiffVars = 4;
    /*0*/
    private int beltLevels = 3;
    /*1a*/
    private int minAstsInBelt = 1;
    /*1b*/
    private int maxAstsInBelt = 3;
    /*2*/
    private int enemyTotalStrength = 5;
    /*3*/
    private float minEnemyClearanceTime = 30;
    /*3*/
    private float maxEnemyClearanceTime = 30;

    // Hardest Poss Difficulty Stats
    /*1a*/
    private int hardestMinAstsInBelt = 6;
    /*1b*/
    private int hardestMaxAstsInBelt = 10;
    /*3a*/
    private float hardestMinEnemyClearanceTime = 10;
    /*3b*/
    private float hardestMaxEnemyClearanceTime = 10;

    public Universe Positions;

    List<int> masterDiffStats = new List<int>();

    // ******Determine by which prefab is the script called***** 
    private int universeN()
    {
        int length = transform.parent.parent.name.Length;
        string num = transform.parent.parent.name.Substring(length - 1, 1);
        return(int.Parse(num));
    }

    // ******Asteroid Belt Functions******
    IEnumerator SendAsteroidBelt() {
        
        GameObject[] asteroidBelts = GameObject.FindGameObjectsWithTag("AsteroidBelt");
        
        foreach (GameObject asteroidBelt in asteroidBelts) {
            asteroidBelt.GetComponent<ParticleSystem>().Play();
            asteroidBelt.GetComponent<ParticleSystem>().enableEmission = true;
        }

        for (int i = 0; i < beltLevels; i++) {
            // First asteroid is always at the character's current position (stops player from sitting in 1 spot)
            CreateAsteroid(false);
            for (int j = 1; j < Random.Range(minAstsInBelt, maxAstsInBelt + 1); j++) {
                
                CreateAsteroid(true);
            }
            yield return new WaitForSeconds(beltGap);
        }

        foreach (GameObject asteroidBelt in asteroidBelts) {
            asteroidBelt.GetComponent<ParticleSystem>().enableEmission = false;
        }
    }

    void CreateAsteroid(bool randY) {
        GameObject character = GameObject.Find("Character" + universeN());
        float y;
        if (randY) y = Random.Range(Positions.bottomBorder, Positions.topBorder);
        else y = character.transform.position.y;
        Vector3 astPosition = new Vector3(Positions.rightBorder + Positions.generationOffset + Random.Range(-astXOffsetRange, astXOffsetRange), y, Positions.baseZ);
        Transform asteroid = (Transform)Network.Instantiate(asteroidPrefab, astPosition, new Quaternion(0, 0, 0, 0), NetworkGroups.Universe[universeN()]);
        asteroid.name = "Asteroid" + universeN();
        asteroid.transform.parent = transform.parent.parent.FindChild("Enemies");
        float sf = Random.Range(minAstScale, maxAstScale);
        asteroid.localScale += new Vector3(sf, sf, sf);
        asteroidCount[universeN()]++;
    }

    // ******Enemy Wave Functions******

    // Sends a wave of enemies, adding to the level of enemyTotalStrength
    void SendEnemyWave() {
        int deployedStrength = enemyTotalStrength;
        while (deployedStrength != 0) {
            // Send a random enemy from the possibles still permitted
            int enemyType = Random.Range(1, Mathf.Min(deployedStrength, 5));
            CreateEnemy(enemyType);
            deployedStrength -= enemyType;
        }
        StartCoroutine("EnemyWaveCountdown");
    }

    // Creates an enemy of the given type
    void CreateEnemy(int type) {
        // Directions:
        // 1 - From Left
        // 2 - From Top
        // 3 - From Right
        // 4 - From Bottom
        int dir = Random.Range(1, 5);
        float x = 0, y = 0, z = 0;
        GameObject character = GameObject.Find("Character" + universeN());
        float genZ = (float)character.transform.position.z;
        switch (dir) {
            case 1:
                x = Positions.leftBorder - Positions.generationOffset;
                y = Random.Range(Positions.bottomBorder, Positions.topBorder);
                z = genZ + 2;
                break;
            case 2:
                x = Random.Range(leftMoveLimit, Positions.rightBorder);
                y = Positions.topBorder + Positions.generationOffset;
                z = genZ + 4;
                break;
            case 3:
                x = Positions.rightBorder + Positions.generationOffset;
                y = Random.Range(Positions.bottomBorder, Positions.topBorder);
                z = genZ + 6;
                break;
            case 4:
                x = Random.Range(leftMoveLimit, Positions.rightBorder);
                y = Positions.bottomBorder - Positions.generationOffset;
                z = genZ + 8;
                break;
            default:
                break;
        }
        Transform enemy = (Transform)Network.Instantiate(enemyPrefab, new Vector3(x, y, z), new Quaternion(0, 0, 0, 0),NetworkGroups.Universe[universeN()]);
        enemy.name = "Enemy" + universeN();
        enemy.transform.parent = transform.parent.parent.FindChild("Enemies");
        EnemyManager eMan = enemy.GetComponent<EnemyManager>();
        eMan.direction = dir;
        eMan.changeType(type);
        enemyCount[universeN()]++;
    }

    // Currently a near-copy of CreateEnemy
    void CreateBoss(int type) {
        // Directions:
        // 1 - From Left
        // 2 - From Top
        // 3 - From Right
        // 4 - From Bottom
        int dir = Random.Range(1, 5);
        float x = 0, y = 0, z = 0;
        GameObject character = GameObject.Find("Character" + universeN());
        float genZ = character.transform.position.z;
        switch (dir) {
            case 1:
                x = Positions.leftBorder - Positions.generationOffset;
                y = Random.Range(Positions.bottomBorder, Positions.topBorder);
                z = genZ + 2;
                break;
            case 2:
                x = Random.Range(leftMoveLimit, Positions.rightBorder);
                y = Positions.topBorder + Positions.generationOffset;
                z = genZ + 4;
                break;
            case 3:
                x = Positions.rightBorder + Positions.generationOffset;
                y = Random.Range(Positions.bottomBorder, Positions.topBorder);
                z = genZ + 6;
                break;
            case 4:
                x = Random.Range(leftMoveLimit, Positions.rightBorder);
                y = Positions.bottomBorder - Positions.generationOffset;
                z = genZ + 8;
                break;
            default:
                break;
        }
        Transform enemy = (Transform)Instantiate(bossPrefab, new Vector3(x, y, z), new Quaternion(0, 0, 0, 0));
        BossManager eMan = enemy.GetComponent<BossManager>();
        eMan.direction = dir;
        eMan.changeType(type);
    }

    // Gives the user set seconds to clear enemies, or starts a new decision
    IEnumerator EnemyWaveCountdown() {
        yield return new WaitForSeconds(Random.Range(minEnemyClearanceTime, maxEnemyClearanceTime));
        MakeDeploymentDecision();
    }

    // ******General Functions******
    void Start() {
        if (Network.isServer)
        {
            int countUniverse = GameObject.FindGameObjectsWithTag("Universe").Length + 1;
            asteroidCount = new int[countUniverse];
            enemyCount = new int[countUniverse];
            Positions = transform.parent.FindChild("OriginManager").GetComponent<Universe>();
            leftMoveLimit = Positions.rightMovementLimit + 2.5f;
            FillMasterDiffStats();
            asteroidCount[universeN()] = 0;
            enemyCount[universeN()] = 0;
            StartCoroutine("StartGame");
        }
    }

    IEnumerator StartGame() {
        // Wait for fade
        yield return new WaitForSeconds(fadeWait);
        levelStarted = true;
    }

    void MakeDeploymentDecision() {
        int choice = Random.Range(0, astProb);
        if (choice == (astProb - 1)) {
            // Deploy asteroid belt
            StartCoroutine("SendAsteroidBelt");
        }
        else {
            // Deploy enemy wave
            SendEnemyWave();
        }
    }

    // Update is called once per frame
    void Update() {
        if (levelStarted) {
            if (asteroidCount[universeN()] == 0 && enemyCount[universeN()] < 2 && !bossDeployed) {
                StopCoroutine("EnemyWaveCountdown");
                MakeDeploymentDecision();
            }
        }
    }

    // ADJUSTMENTS SUBJECT TO BALANCING
    private void AlterDifficultyVariable(int varNum) {
        switch (varNum) {
            case 0:
                // Alter beltLevels
                beltLevels++;
                break;
            case 1:
                // Alter minAstsInBelt or maxAstsInBelt
                int toChangeAsts = Random.Range(0, 2);
                if (toChangeAsts == 0) {
                    // Change min asts (unless can't)
                    if (minAstsInBelt == maxAstsInBelt || minAstsInBelt == hardestMinAstsInBelt) {
                        // Can't increment, so change max instead
                        maxAstsInBelt++;
                    }
                    else minAstsInBelt++;
                }
                else {
                    // Change max asts (unless can't)
                    if (maxAstsInBelt == hardestMaxAstsInBelt) {
                        // Can't increment, so change min instead
                        minAstsInBelt++;
                    }
                    else maxAstsInBelt++;
                }
                // Check if needs to be removed from master list (both hit hardest)
                if (minAstsInBelt == hardestMinAstsInBelt && maxAstsInBelt == hardestMaxAstsInBelt) masterDiffStats.Remove(varNum);
                break;
            case 2:
                // Alter enemyTotalStrength
                enemyTotalStrength++;
                break;
            case 3:
                // Alter minEnemyClearanceTime or maxEnemyClearanceTime
                int toChangeTime = Random.Range(0, 2);
                if (toChangeTime == 0) {
                    // Change min time (unless can't)
                    if (minEnemyClearanceTime == hardestMinEnemyClearanceTime) {
                        // Can't change, so change max instead
                        maxEnemyClearanceTime = maxEnemyClearanceTime - 2;
                    }
                    else minEnemyClearanceTime = minEnemyClearanceTime - 2;
                }
                else {
                    // Change max time (unless can't)
                    if (maxEnemyClearanceTime == minEnemyClearanceTime || maxEnemyClearanceTime == hardestMaxEnemyClearanceTime) {
                        // Can't change, so change min instead
                        minEnemyClearanceTime = minEnemyClearanceTime - 2;
                    }
                    else maxEnemyClearanceTime = maxEnemyClearanceTime - 2;
                }
                // Check if needs to be removed from master list (both hit hardest)
                if (minEnemyClearanceTime == hardestMinEnemyClearanceTime && maxEnemyClearanceTime == hardestMaxEnemyClearanceTime) masterDiffStats.Remove(varNum);
                break;
            default:
                break;
        }
    }

    private void FillMasterDiffStats() {
        for (int i = 0; i < numDiffVars; i++) masterDiffStats.Add(i);
    }

    public string GetDiffVarFromInt(int diffVar) {
        switch (diffVar) {
            case 0:
                return "LONGER ASTEROID BELTS";
            case 1:
                return "HARDER ASTEROID BELTS";
            case 2:
                return "MORE ENEMIES";
            case 3:
                return "LESS TIME TO KILL ENEMIES";
            default:
                return "SOMETHING'S WRONG!";
        }
    }

    public int[] IncreaseDifficulty() {
        // Pick 2 variables to change at random (ensure different!)
        // Change these accordingly
        // Number of changes fixed to 2 to ensure this never freezes
        List<int> availableDiffStats = new List<int>(masterDiffStats);
        int[] changed = new int[2];
        for (int i = 0; i < 2; i++) {
            // Pick an int from the number of poss DiffStats, then retrieve the value at that index
            int varToChange = Random.Range(0, availableDiffStats.Count);
            varToChange = availableDiffStats[varToChange];
            AlterDifficultyVariable(varToChange);
            changed[i] = varToChange;
            // Ensure we can't change the same one twice in the same difficulty increase
            availableDiffStats.Remove(varToChange);
        }
        return changed;
    }

    public void DeployBoss() {
        // Clear all enemies/asteroids from the screen
        // Pick a boss and send them out!
        // Resume usual enemy generation techniques
        bossDeployed = true;
        ClearScreen();
        CreateBoss(4);
    }

    private void ClearScreen() {      
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] asteroids = GameObject.FindGameObjectsWithTag("Asteroid");
        foreach (GameObject e in enemies) Destroy(e);
        foreach (GameObject a in asteroids) Destroy(a);
        StopAllCoroutines();
    }

    public void BossDestroyed() {
        bossDeployed = false;
    }
}