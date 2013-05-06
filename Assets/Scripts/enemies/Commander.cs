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

    private int currType = 0;
    public GameObject[,] enemyTypes = new GameObject[3, 4];
	
	private Object[] enemyPrefabs;
    public Transform asteroidPrefab;
    private bool levelStarted = false;
    public bool bossDeployed = false;

    private float minAstScale = 0.0f;
    private float maxAstScale = 1.5f;
    private int fadeWait = 2;
    private float beltGap = 1f;
    private int astProb = 3;

    private float leftMoveLimit;

    // Enemy generation positioning stats
    private float astXOffsetRange = 2.5f;

    // Difficulty stats (numbered for selection, some merged into 1 "setting")
    private int numDiffVars = 4;
    /*0*/
    //private int beltLevels = 3;
    private int beltLevels = 1;
    /*1a*/
    private int minAstsInBelt = 1;
    /*1b*/
    //private int maxAstsInBelt = 3;
    private int maxAstsInBelt = 1;
    /*2*/
    private int enemyTotalStrength = 5;
    /*3*/
    private float minEnemyClearanceTime = 30;
    /*3*/
    private float maxEnemyClearanceTime = 30;

    // Hardest Poss Difficulty Stats
    /*1a*/
    //private int hardestMinAstsInBelt = 6;
    private int hardestMinAstsInBelt = 1;
    /*1b*/
    //private int hardestMaxAstsInBelt = 10;
    private int hardestMaxAstsInBelt = 1;
    /*3a*/
    private float hardestMinEnemyClearanceTime = 10;
    /*3b*/
    private float hardestMaxEnemyClearanceTime = 10;

    public Universe positions;

    /* Keep track of characters in universe. <-- Easter present for Ben !!*/
    /* How it works: For each Commander script in each Universe, there is an activeCharacters bool array that
     * determines which character is active in the respective universe. The count is started for simplicity
     * from 1 (activeCharacters[1] refers to the status of the first character in the current universe).*/
    public bool[] activeCharacters;

    List<int> masterDiffStats = new List<int>();
	
	public static void SetupStatics() {
		asteroidCount = new int[5];
    	enemyCount = new int[5];	
	}

    // ******Used by enemies to pick a target player******
    /*public static int pickTarget() {
        // Pick randomly from the array
        return 1;
    }*/

    // ******Determine by which prefab is the script called***** 
    private int universeN()
    {
        int length = transform.parent.parent.name.Length;
        string num = transform.parent.parent.name.Substring(length - 1, 1);
        return(int.Parse(num));
    }

    private int cameraN()
    {
        if (Network.isClient)
        {
            int length = GameObject.FindGameObjectWithTag("MainCamera").name.Length;
            string num = GameObject.FindGameObjectWithTag("MainCamera").name.Substring(length - 1, 1);
            Debug.Log("Camera name " + GameObject.FindGameObjectWithTag("MainCamera").name + " " + num);
            return (int.Parse(num));
        }
        else return 0;
    }

    // ******Asteroid Belt Functions******
    IEnumerator SendAsteroidBelt() {
        RotatePlayers(true, universeN());
        GameObject[] asteroidBelts = GameObject.FindGameObjectsWithTag("AsteroidBelt");
        
        foreach (GameObject asteroidBelt in asteroidBelts) {
            asteroidBelt.GetComponent<ParticleSystem>().Play();
            asteroidBelt.GetComponent<ParticleSystem>().enableEmission = true;
        }

        for (int i = 0; i < beltLevels; i++) {
            // First asteroid is always at the character's current position (stops player from sitting in 1 spot)
            CreateAsteroid();
            for (int j = 1; j < Random.Range(minAstsInBelt, maxAstsInBelt + 1); j++) {
                CreateAsteroid();
            }
            yield return new WaitForSeconds(beltGap);
        }

        foreach (GameObject asteroidBelt in asteroidBelts) {
            asteroidBelt.GetComponent<ParticleSystem>().enableEmission = false;
        }
    }

    void CreateAsteroid() {
        float y = Random.Range(positions.bottomBorder, positions.topBorder);
        Vector3 astPosition = new Vector3(positions.rightBorder + positions.generationOffset + Random.Range(-astXOffsetRange, astXOffsetRange), y, Random.Range(positions.baseZ + 5, positions.baseZ - 5));
        Transform asteroid = (Transform)Network.Instantiate(asteroidPrefab, astPosition, new Quaternion(0, 0, 0, 0), 100+universeN());
        asteroid.name = "Asteroid" + universeN();
        asteroid.transform.parent = transform.parent.parent.FindChild("Enemies");
        float sf = Random.Range(minAstScale, maxAstScale);
        asteroid.localScale += new Vector3(sf, sf, sf);
        asteroidCount[universeN()]++;
    }

    // ******Enemy Wave Functions******

    // Sends a wave of enemies, adding to the level of enemyTotalStrength
    void SendEnemyWave() {
        RotatePlayers(false, universeN());
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

		//GameObject enemyPrefab = (GameObject) enemyPrefabs[Random.Range (0,enemyPrefabs.Length)];
        GameObject enemyPrefab = enemyTypes[currType, type-1];
        Transform enemy = (Transform)Network.Instantiate(enemyPrefab.transform, new Vector3(x, y, z), Quaternion.Euler(new Vector3 (0, 180, 0)),100+universeN());

		enemy.name = "Enemy" + universeN();
        enemy.transform.parent = transform.parent.parent.FindChild("Enemies");
		
        EnemyManager eMan = enemy.GetComponent<EnemyManager>();
        eMan.direction = dir;
        //eMan.changeType(type);
        enemyCount[universeN()]++;
    }

    // Gives the user set seconds to clear enemies, or starts a new decision
    IEnumerator EnemyWaveCountdown() {
        yield return new WaitForSeconds(Random.Range(minEnemyClearanceTime, maxEnemyClearanceTime));
        MakeDeploymentDecision();
    }

    // ******General Functions******
    void Start() {
		//enemyPrefabs = Resources.LoadAll("enemies/enemytypes/test", typeof(GameObject));
        // Hard coded. Can't be arsed to mess around with a more flexible solution
        enemyTypes[0,0] = (GameObject) Resources.Load("enemies/enemytypes/vox/vox_light", typeof(GameObject));
        enemyTypes[0,1] = (GameObject) Resources.Load("enemies/enemytypes/vox/vox_medium", typeof(GameObject));
        enemyTypes[0,2] = (GameObject) Resources.Load("enemies/enemytypes/vox/vox_heavy", typeof(GameObject));
        enemyTypes[0,3] = (GameObject) Resources.Load("enemies/enemytypes/vox/vox_superheavy", typeof(GameObject));
        enemyTypes[1,0] = (GameObject) Resources.Load("enemies/enemytypes/crim/crim_light", typeof(GameObject));
        enemyTypes[1,1] = (GameObject) Resources.Load("enemies/enemytypes/crim/crim_medium", typeof(GameObject));
        enemyTypes[1,2] = (GameObject) Resources.Load("enemies/enemytypes/crim/crim_heavy", typeof(GameObject));
        enemyTypes[1,3] = (GameObject) Resources.Load("enemies/enemytypes/crim/crim_superheavy", typeof(GameObject));
        enemyTypes[2,0] = (GameObject) Resources.Load("enemies/enemytypes/merc/merc_light", typeof(GameObject));
        enemyTypes[2,1] = (GameObject) Resources.Load("enemies/enemytypes/merc/merc_medium", typeof(GameObject));
        enemyTypes[2,2] = (GameObject) Resources.Load("enemies/enemytypes/merc/merc_heavy", typeof(GameObject));
        enemyTypes[2,3] = (GameObject) Resources.Load("enemies/enemytypes/merc/merc_superheavy", typeof(GameObject));
        currType = Random.Range(0, 3);
        if (Network.isServer)
        {
            int countUniverse = GameObject.FindGameObjectsWithTag("Universe").Length + 1;
            activeCharacters = new bool[countUniverse+1];
            activeCharacters[universeN()] = true;
            asteroidCount = new int[countUniverse];
            enemyCount = new int[countUniverse];
            positions = transform.parent.FindChild("OriginManager").GetComponent<Universe>();
            leftMoveLimit = positions.rightMovementLimit + 2.5f;
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

    [RPC]
    public void moveBossUniverse()
    {
        if (Network.isClient)
        {
            Debug.Log("Move to Boss universe " + cameraN());
            PlayerManager manager = GameObject.Find("Character" + cameraN()).GetComponent<PlayerManager>();
            manager.movement.changeUniverse(0);
            manager.movement.SetCamForBoss();
        }
    }

    [RPC]
    public void moveInitialUniverse()
    {
        if (Network.isClient) {
            Debug.Log("Move back to universe " + cameraN());
            PlayerManager manager = GameObject.Find("Character" + cameraN()).GetComponent<PlayerManager>();
            manager.movement.changeUniverse(cameraN());
            manager.movement.SetCamAfterBoss();
        }
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
        if (Network.isServer) {
            if (levelStarted) {
                if (asteroidCount[universeN()] == 0 && enemyCount[universeN()] < 2 && !bossDeployed) {
                    StopCoroutine("EnemyWaveCountdown");
                    MakeDeploymentDecision();
                }
            }
        }
    }

    void RotatePlayers(bool toBehind, int rotUniverse) {
        for (int i = 0; i < activeCharacters.Length; i++) {
            if (activeCharacters[i]) {
                GameObject character = GameObject.Find("Character" + i);
                PlayerMovement move = character.GetComponent<PlayerMovement>();
                move.networkView.RPC("RotateCamera", RPCMode.Others, toBehind, i, rotUniverse);
            }
        }
    }

    // ADJUSTMENTS SUBJECT TO BALANCING
    private void AlterDifficultyVariable(int varNum) {
        switch (varNum) {
            case 0:
                // Alter beltLevels
                //beltLevels++;
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
            //if (universeN() == 1) Debug.Log(varToChange);
            // Ensure we can't change the same one twice in the same difficulty increase
            availableDiffStats.Remove(varToChange);
        }
        // Send a message to the screen for the wanted clients (via Universe)
        return changed;
    }

    /*public void DeployBoss() {
        // Clear all enemies/asteroids from the screen
        // Pick a boss and send them out!
        // Resume usual enemy generation techniques
        RotatePlayers(true);
        bossDeployed = true;
        ClearScreen();
        CreateBoss(4);
    }*/

    public void WarpAnimation() {
        if (Network.isServer) {
            bossDeployed = true;
            for (int i = 1; i < 5; i++) {
                if (activeCharacters[i]) {
                    GameObject character = GameObject.Find("Character" + i);
                    PlayerMovement move = character.GetComponent<PlayerMovement>();
                    move.networkView.RPC("AnimateWarp", RPCMode.All, i);
                }
            }
        }
    }

    public void SendToBoss() {
        if (Network.isServer) {
            networkView.RPC("moveBossUniverse", RPCMode.All);
            ClearScreen();   
        }
    }

    public void BringBackFromBoss() {
        if (Network.isServer) {
            networkView.RPC("moveInitialUniverse", RPCMode.All);
            /*for (int i = 1; i < 5; i++) {
                if (activeCharacters[i]) {
                    GameObject character = GameObject.Find("Character" + i);
                    PlayerMovement move = character.GetComponent<PlayerMovement>();
                    move.SetCamAfterBoss();
                }
            }*/
        }
    }

    public void ClearScreen() { 
        Transform enDirectory = transform.parent.parent.FindChild("Enemies");
        List<GameObject> children = new List<GameObject>();
        // Stops countdown timer
        StopAllCoroutines();
        foreach (Transform child in enDirectory) children.Add(child.gameObject);
        children.ForEach(child => Network.Destroy(child));     
    }

    public void ResumeGame() {
        bossDeployed = false;
        // Pick a new enemy type
        currType = Random.Range(0, 3);
    }

    /* Notify Server Commander script about post warp positions*/
    public void updateActiveChar(int characterNum, bool val)
    {
        if (activeCharacters != null) activeCharacters[characterNum] = val;
    }
}