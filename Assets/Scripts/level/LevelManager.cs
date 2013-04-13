using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour {
    // Level stats
    public static float stage;
    private float changeTime = 10;
    private Commander enemyGen;
    private HudOn hudOn;

    private List<string> levelNames = new List<string>();
    private int stagesBeforeBoss = 3;

    public void Awake() {
		stage = 0;
		
        GameObject enMan = GameObject.Find("EnemyManager");
        //GameObject cam = GameObject.Find("Main Camera");
        enemyGen = enMan.GetComponent<Commander>();
        //hudOn = cam.GetComponent<HudOn>();
        InitializeLevelNames();
       // StartCoroutine("StageProgression");     
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

    // Used to time different stages and increment difficulty progression
    IEnumerator StageProgression() {
        yield return new WaitForSeconds(2);
        for (int j = 0; j < stagesBeforeBoss; j++) {
            
            stage++;
            // Pick a random level name
            if (stage != 1) {
                int level = Random.Range(0, levelNames.Count);
                string thisLevelName = levelNames[level];
                string toastText = "NOW ENTERING " + thisLevelName;
                // Make more difficult
                int[] changedVars = enemyGen.IncreaseDifficulty();
                for (int i = 0; i < changedVars.Length; i++) {
                    toastText = toastText + "\n" + enemyGen.GetDiffVarFromInt(changedVars[i]);
                }
                StartCoroutine(Toast(toastText));
                levelNames.Remove(thisLevelName);
                if (levelNames.Count == 0) InitializeLevelNames();
            }
            // Wait a certain amount of time          
            yield return new WaitForSeconds(changeTime);
        }
        // Deploy boss here and halt all enemy distribution
        StartCoroutine(Toast("BOSS INCOMING"));
        yield return new WaitForSeconds(3);
        hudOn.StopScore();
        enemyGen.DeployBoss();
    }

    IEnumerator Toast(string notetext) {
        GameObject toast = new GameObject("Toast");
        toast.AddComponent("GUIText");
        toast.guiText.font = (Font)Resources.Load("Belgrad");
        toast.guiText.fontSize = (Screen.width > 1000) ? 40 : 24;
        toast.transform.position = new Vector3(0.5f, 0.5f, 0);
        toast.guiText.anchor = TextAnchor.MiddleCenter;
        toast.guiText.text = notetext;
        toast.guiText.material.color = Color.white;
        toast.guiText.alignment = TextAlignment.Center;
        iTween.FadeTo(toast, 1f, 1f);
        yield return new WaitForSeconds(1.5f);
        iTween.FadeTo(toast, 0f, 1f);
        yield return new WaitForSeconds(1f);
        Destroy(toast);
    }

    IEnumerator BossClearedEnumerator() {     
        StartCoroutine("Toast", "CONGRATULATIONS");
        yield return new WaitForSeconds(5);
        hudOn.StartScore();
        StartCoroutine("StageProgression");
        enemyGen.BossDestroyed();
    }

    public void BossCleared() {
        StartCoroutine("BossClearedEnumerator");
    }
}