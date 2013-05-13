using UnityEngine;
using System.Collections;

public class Names : MonoBehaviour
{
	public static string FetchSTRName() {
		ArrayList str = new ArrayList();

		str.Add("Space Turd Revolution");
		str.Add("Superintendent Tony Robinson");
		str.Add("Spanish Tour Rep");
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
        str.Add("Stoke Taste Relegation");
		str.Add("Sunday's Tragic Roast");
		str.Add("Speedos, Thongs, Rabbits");
		str.Add("Spaniels Terminating Robots");
		str.Add("Southern Turkey Regiment");
		str.Add("Star Tries Radiotherapy");
		str.Add("Sleeping Through Rammstein");
		str.Add("Shepherd Turned Rapper");
		str.Add("Secretly, Timothy Rages");
		str.Add("Swindon's Totalitarian Regime");
		str.Add("Silver Tongued Retiree");
		str.Add("Sober Times, Rejoice!");
		str.Add("Shania Twain Reunion");
		str.Add("Surf Turtles: Rehab");
		str.Add("Swindon's Totalitarian Regime");
		string winner = (string) str[(int) Random.Range(0,str.Count)];
		return winner.ToUpper();
	}
}