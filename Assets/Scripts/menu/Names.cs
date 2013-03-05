using UnityEngine;
using System.Collections;

public class Names : MonoBehaviour
{
	public static string FetchSTRName() {
		ArrayList str = new ArrayList();

		str.Add("Space Turd Revolution");
		str.Add("Superintendent Tony Robinson");
		str.Add("Spanish Tour Guide");
		str.Add("Swedish Techno Remix");
		str.Add("Suddenly, Thomas Realised...");
		str.Add("Stylish Turkish Relaxation");
		str.Add("Somewhat Totally Random");
		str.Add("Sobbing To Radiohead");
		str.Add("Shetlands To Reykjavik");
		str.Add("Spirited Tequila Rampage");
		str.Add("Sensible Tail Recursion");
		str.Add("Salsa Training, Revisited");
		str.Add("Sublime Tympani Recording");
		string winner = (string) str[(int) Random.Range(0,str.Count)];
		return winner.ToUpper();
	}
}