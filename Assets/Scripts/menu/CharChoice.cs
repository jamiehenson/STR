using UnityEngine;
using System.Collections;

public class CharChoice : MonoBehaviour {
	private bool fading;

    private int inPos, outPos;
	
	void Start () {
        if (gameObject.name.Contains("Details"))
        {
            inPos = 20; outPos = 19;
        }
        else
        {
            inPos = 29; outPos = 28;
        }
		iTween.FadeTo(gameObject, 0.6f, 0.1f);
        iTween.MoveTo(gameObject, new Vector3(transform.position.x, transform.position.y, inPos), 0.1f);
        SetIndicators("usa");
	}

    void Update()
    {
        if (gameObject.name.Contains("Details"))
        {
            inPos = 20; outPos = 19;
        }
        else
        {
            inPos = 29; outPos = 28;
        }
    }

	void OnMouseEnter() {
		iTween.FadeTo(gameObject, 1.0f, 0.5f);
        iTween.MoveTo(gameObject, new Vector3(transform.position.x, transform.position.y, outPos), 1f);
	}
	
	void OnMouseExit() {
		iTween.FadeTo(gameObject, 0.6f, 0.5f);
        iTween.MoveTo(gameObject, new Vector3(transform.position.x, transform.position.y, inPos), 1f);
	}
	
	IEnumerator LoadGame () {
	    iTween.CameraFadeAdd();
		iTween.CameraFadeTo(1.0f, 2.0f);
		yield return new WaitForSeconds(2.0f);
	    Application.LoadLevel("plane");
	}
	
    IEnumerator changeBGTex (string character)
    {
        GameObject bg = GameObject.Find("SBrowserBG");
        Shader trans = Shader.Find("Transparent/Diffuse");
        bg.renderer.material.shader = trans;

        iTween.FadeTo(bg, 0, 0.5f);
        yield return new WaitForSeconds(0.5f);
        bg.renderer.material.SetTexture("_MainTex", (Texture2D) Resources.Load("menu/" + character));
        iTween.FadeTo(bg, 0.7f, 0.5f);
        yield return new WaitForSeconds(0.1f);
    }

    void SetIndicators(string character)
    {
        GameObject combatSphere = GameObject.Find("SDetailsCombatSphere");
        GameObject combatSphere2 = GameObject.Find("SDetailsCombatSphere2");
        GameObject combatSphere3 = GameObject.Find("SDetailsCombatSphere3");
        GameObject energySphere = GameObject.Find("SDetailsEnergySphere");
        GameObject energySphere2 = GameObject.Find("SDetailsEnergySphere2");
        GameObject energySphere3 = GameObject.Find("SDetailsEnergySphere3");
        GameObject warpingSphere = GameObject.Find("SDetailsWarpingSphere");
        GameObject warpingSphere2 = GameObject.Find("SDetailsWarpingSphere2");
        GameObject warpingSphere3 = GameObject.Find("SDetailsWarpingSphere3");

        switch (character)
        {
            case "china":
                combatSphere.renderer.material.color = Color.red;
                energySphere.renderer.material.color = Color.green;
                energySphere2.renderer.material.color = Color.green;
                energySphere3.renderer.material.color = Color.green;
                warpingSphere.renderer.material.color = Color.yellow;
                warpingSphere2.renderer.material.color = Color.yellow;

                combatSphere.renderer.enabled = true;
                combatSphere2.renderer.enabled = false;
                combatSphere3.renderer.enabled = false;
                energySphere.renderer.enabled = true;
                energySphere2.renderer.enabled = true;
                energySphere3.renderer.enabled = true;
                warpingSphere.renderer.enabled = true;
                warpingSphere2.renderer.enabled = true;
                warpingSphere3.renderer.enabled = false;
                break;
            case "usa":
                energySphere.renderer.material.color = Color.red;
                combatSphere.renderer.material.color = Color.green;
                combatSphere2.renderer.material.color = Color.green;
                combatSphere3.renderer.material.color = Color.green;
                warpingSphere.renderer.material.color = Color.yellow;
                warpingSphere2.renderer.material.color = Color.yellow;

                combatSphere.renderer.enabled = true;
                combatSphere2.renderer.enabled = true;
                combatSphere3.renderer.enabled = true;
                energySphere.renderer.enabled = true;
                energySphere2.renderer.enabled = false;
                energySphere3.renderer.enabled = false;
                warpingSphere.renderer.enabled = true;
                warpingSphere2.renderer.enabled = true;
                warpingSphere3.renderer.enabled = false;
                break;
            case "russia":
                combatSphere.renderer.material.color = Color.yellow;
                combatSphere2.renderer.material.color = Color.yellow;
                energySphere.renderer.material.color = Color.yellow;
                energySphere2.renderer.material.color = Color.yellow;
                warpingSphere.renderer.material.color = Color.green;
                warpingSphere2.renderer.material.color = Color.green;
                warpingSphere3.renderer.material.color = Color.green;

                combatSphere.renderer.enabled = true;
                combatSphere2.renderer.enabled = true;
                combatSphere3.renderer.enabled = false;
                energySphere.renderer.enabled = true;
                energySphere2.renderer.enabled = true;
                energySphere3.renderer.enabled = false;
                warpingSphere.renderer.enabled = true;
                warpingSphere2.renderer.enabled = true;
                warpingSphere3.renderer.enabled = true;
                break;
            default: break;
        }
    }

	void OnMouseUp() {
        GameObject header = GameObject.Find("SDetailsSelected");
        

        if (gameObject.name.Contains("China")) {
            PlayerManager.activeChar = "china";
            if (MP.joinScreen || MP.hostScreen)
            {
                StartCoroutine("changeBGTex", "china");
                SetIndicators("china");
                header.GetComponent<TextMesh>().text = "ZHANG";
            }
        }
        else if (gameObject.name.Contains("USA")) {
            PlayerManager.activeChar = "usa";
            if (MP.joinScreen || MP.hostScreen)
            {
                StartCoroutine("changeBGTex", "usa");
                SetIndicators("usa");
                header.GetComponent<TextMesh>().text = "JOHNSON";
            }
        }
        else if (gameObject.name.Contains("Russia")) {
            PlayerManager.activeChar = "russia";
            if (MP.joinScreen || MP.hostScreen)
            {
                StartCoroutine("changeBGTex", "russia");
                SetIndicators("russia");
                header.GetComponent<TextMesh>().text = "MARKOV";
            }
        }
		
        if (SP.singleplayerStart) StartCoroutine(LoadGame());
	}
}
