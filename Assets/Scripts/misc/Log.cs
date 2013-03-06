using UnityEngine;
using System.Collections;

// A log class to handle fatal errors, warnings and event notes
public class Log : MonoBehaviour {
	private static int printCount = 1;
	
	
	// Turn on to get a flood of information
	public static bool printNotes = false;

	// Report Fatal Error
	public static void Error(string header, string message){
		// TODO: SCREAM!
	}
	
	// Report Warning
	public static void Warning(string message){
		Debug.Log("[#"+printCount+" Warning] "+message);
	}
	
	// Report Event
	public static void Note(string message){
		if (printNotes)
			Debug.Log ("[#"+printCount+" Note   ] "+message);
		else
		{
			// Not today Maddie!
		}
	}
}
