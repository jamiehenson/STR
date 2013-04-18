using UnityEngine;
using System.Collections;

public class Vortex : MonoBehaviour {
    private bool scaleswitch;
    private int i;
	public int leadsToUniverse;
    private float growth = 0.015f;
	public float screenPositionX;
	public float screenPositionY;
	public string label;
	private Texture2D bg;
	private bool labelIsSet = false;
	private Font deco = (Font) Resources.Load ("Belgrad");

    public static IEnumerator grow(GameObject vortex)
    {
        float x = 0;
        vortex.transform.localScale = new Vector3(0, 0, 0);
        while (x <= 4)
        {
            x += 0.05f;
            vortex.transform.localScale = new Vector3(x, 0, x);
            yield return new WaitForSeconds(0.01f);
        }
    }

    public static IEnumerator shrink(GameObject vortex)
    {
        float x = 1;
        while (x > 0)
        {
            x -= 0.01f;
            vortex.transform.localScale = new Vector3(x, x, x);
            yield return new WaitForSeconds(0.01f);
        }
        Destroy(vortex);
    }

	public void setLabel(float x, float y, string lab) {
		screenPositionX = x;
		screenPositionY = y;
		label = lab;
		labelIsSet = true;
	}

	public void OnGUI() {

		if (labelIsSet) {
			Vector3 viewPort = new Vector3(screenPositionX,screenPositionY,0);
			Vector3 screenPoint = Camera.main.ViewportToScreenPoint(viewPort);
			print ("screenPoint = "+screenPoint);
			int bgw = 140; // Vortex note bg width
			int bgh = 30; // Vortex note bg height
			int x = 10;
			int y = 5;
			GUIStyle style = new GUIStyle();
	    	style.font = deco;
			style.normal.textColor = Color.white;
			style.alignment = TextAnchor.MiddleCenter;
			style.fontStyle = FontStyle.Bold;
			style.fontSize = 18;
			label = label.ToUpper();
			GUI.DrawTexture(new Rect(screenPoint.x-(bgw/2-5),screenPoint.y-(bgh/2),bgw,bgh),bg,ScaleMode.StretchToFill, true, 0);
			GUI.Label(new Rect(screenPoint.x,screenPoint.y,x,y), label, style);
		}
	}

    void Start()
    {
        StartCoroutine(grow(gameObject));
		bg = (Texture2D) Resources.Load ("menu/blankfull");
    }

	void Update () {
	    transform.Rotate(Vector3.up * Time.deltaTime * 40f);
        transform.Rotate(Vector3.forward * Time.deltaTime * 10f);

        if (i == 0) scaleswitch = true;
        else if (i == 150) scaleswitch = false;

        if (scaleswitch)
        {
            transform.localScale += new Vector3(growth, 0, growth);
            i++;
        }
        else
        {
            transform.localScale += new Vector3(-growth, 0, -growth);
            i--;
        }
	}

    void OnCollisionEnter(Collision collision)
    {
		GameObject obj = collision.gameObject;
		if (obj.tag == "Player" && obj.GetComponent<PlayerMovement>().myCharacter)
			HudOn.Instance.enteredVortex(leadsToUniverse);
    }
	
	void OnCollisionExit(Collision collision)
    {
		GameObject obj = collision.gameObject;
		if (obj.tag == "Player" && obj.GetComponent<PlayerMovement>().myCharacter)
			HudOn.Instance.leftVortex();
    }
}
