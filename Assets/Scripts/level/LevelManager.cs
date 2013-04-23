using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour {
    // Level stats
    private int stage = 0;
    private float changeTime = 10;
    private Commander enemyGen;
    private HudOn hudOn;
    private int universeNum;

    private List<string> levelNames = new List<string>();
    private int stagesBeforeBoss = 3;

    // ******Determine by which prefab is the script called***** 
    private int universeN() {
        int length = transform.parent.parent.name.Length;
        string name = transform.parent.parent.name;
        if (name == "Boss Universe") return -1;
        string num = name.Substring(length - 1, 1);
        return (int.Parse(num));
    }

    public void Start() {
        if (Network.isServer) {
            universeNum = universeN();
            if (universeNum == -1) return;
            stage = 0;
            enemyGen = transform.parent.FindChild("EnemyManager").GetComponent<Commander>();
            //hudOn = GameObject.Find("Camera " + universeNum).GetComponent<HudOn>();
            InitializeLevelNames();
            //StartCoroutine("StageProgression");     
        }
    }

    // PLACEHOLDER NAMES
    private void InitializeLevelNames() {
        levelNames.Add("AARGONAR");
        levelNames.Add("ABREGADO");
        levelNames.Add("ADUBA");
        levelNames.Add("ADEGA");
        levelNames.Add("ALDERAAN");
        levelNames.Add("AL'HAR");
        levelNames.Add("ALZOC");
        levelNames.Add("ANOAT");
        levelNames.Add("AXUM");
        levelNames.Add("BAKURA");
        levelNames.Add("BESH GORGON");
        levelNames.Add("BESPIN");
        levelNames.Add("BILBRINGI");
        levelNames.Add("BITH");
        levelNames.Add("BODI");
        levelNames.Add("BONADAN");
        levelNames.Add("BOTHAWUI");
        levelNames.Add("BRENTAAL");
        levelNames.Add("CHANDRILA");
        levelNames.Add("CHORAX");
        levelNames.Add("CHORIOS");
        levelNames.Add("CIRIUS");
        levelNames.Add("CIRCARPOUS MAJOR");
        levelNames.Add("COLU");
        levelNames.Add("CORELLIAN");
        levelNames.Add("CORULUS");
        levelNames.Add("CORUSCANT");
        levelNames.Add("CULARIN");
        levelNames.Add("CYPRIX");
        levelNames.Add("DAGOBAH");
        levelNames.Add("DANTOOINE");
        levelNames.Add("DOMINUS");
        levelNames.Add("DORVALA");
        levelNames.Add("ELROOD");
        levelNames.Add("EMPRESS TETA");
        levelNames.Add("EVONA/ARDOS");
        levelNames.Add("ENDOR");
        levelNames.Add("FALLEEN");
        levelNames.Add("FEST");
        levelNames.Add("GAMORR");
        levelNames.Add("GORSH");
        levelNames.Add("HELSKA");
        levelNames.Add("HORUSET");
        levelNames.Add("HUTTA");
        levelNames.Add("HOTH");
        levelNames.Add("IRODONIA");
        levelNames.Add("JAPREAL");
        levelNames.Add("KAMINO");
        levelNames.Add("KORRIBAN");
        levelNames.Add("KARTHAKK");
        levelNames.Add("KASHYYYK");
        levelNames.Add("KHUIUMIN");
        levelNames.Add("KESSEL");
        levelNames.Add("KOROS");
        levelNames.Add("LYBEYA");
        levelNames.Add("MUSTAFAR");
        levelNames.Add("MUUN");
        levelNames.Add("NABOO");
        levelNames.Add("ONDERON");
        levelNames.Add("PAKUNNI SYSTEM");
        levelNames.Add("POLITH");
        levelNames.Add("POLIS MASSA");
        levelNames.Add("PYRIA");
        levelNames.Add("PYRSHAK");
        levelNames.Add("RAFA");
        levelNames.Add("RIFLORII");
        levelNames.Add("RISHI");
        levelNames.Add("RODIA");
        levelNames.Add("ROSP");
        levelNames.Add("SARTINAYNIAN");
        levelNames.Add("SCARL");
        levelNames.Add("SERIANAN");
        levelNames.Add("TINGEL ARM");
        levelNames.Add("TARIS");
        levelNames.Add("TAROON");
        levelNames.Add("TATOO");
        levelNames.Add("TELOS");
        levelNames.Add("TETH");
        levelNames.Add("UTAPAU");
        levelNames.Add("UTEGETU NEBULA");
        levelNames.Add("VELUS");
        levelNames.Add("VERON");
        levelNames.Add("XCORPON");
        levelNames.Add("YAVIN");
        levelNames.Add("Y'TOUB");
        levelNames.Add("ZUG");
    }

    public void WarpAnimation() {
        enemyGen.WarpAnimation();
    }

    public void LevelIncrease() {
        stage++;
        int level = Random.Range(0, levelNames.Count);
        string thisLevelName = levelNames[level];
        // Difficulty increases ahoy!
        int[] changedVars = enemyGen.IncreaseDifficulty();
        levelNames.Remove(thisLevelName);
        if (levelNames.Count == 0) InitializeLevelNames();
        // Put a toast informing about level/difficulty increase here?
    }

    IEnumerator BossComingIE(int wait) {
        // Push everyone to boss universe
        enemyGen.SendToBoss();
        // Pause for a set amount of time
        yield return new WaitForSeconds(wait + 2);
        // Tell the commander to stop sending enemies (& clear screen)
        enemyGen.ClearScreen();
    }

    public void BossComing(int wait) {
        // Wrapper - needs to run on a coroutine
        if (Network.isServer) {
            StartCoroutine("BossComingIE", wait);
        }
    }

    IEnumerator BossClearedIE(int wait) {
        // Pull everyone back to their own universe
        enemyGen.BringBackFromBoss();
        // Pause for a set amount of time
        yield return new WaitForSeconds(wait+5);
        // Tell the commander to resume sending enemies
        enemyGen.ResumeGame();
    }

    public void BossCleared(int wait) {
        // Wrapper - needs to run on a coroutine
        if (Network.isServer) {
            StartCoroutine("BossClearedIE", wait);
        }
    }
}