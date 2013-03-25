using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour {
	// Player stats
	public static float hitPoints;
	public static float energyLevel;
	public static float startHP, startEnergy;
	public static float energyBank = 0;
	public static int selectedWepDrain;
    public static int bankSize = 10000;
    public static bool bankFull;
    public static float speed;
    public static string playername;
	
	// Character-centric player stats
	public static string activeChar;
	public static int wepType;
	public static float damageMultiplier;
	public static float energyMultiplier;

	public static void InitialiseStats()
	{
		switch(activeChar) {              
			case "china":
				startHP = 500;
				startEnergy = 1000;
                speed = 15.0f;
				damageMultiplier = 0.5f;
				energyMultiplier = 1f;
                bankSize = 7000;
				energyBank = 0;
				break;
			case "usa":
				startHP = 1000;
				startEnergy = 500;
                speed = 5.0f;
				damageMultiplier = 1.5f;
				energyMultiplier = 1.5f;
                bankSize = 7000;
				energyBank = 0;
				break;
			case "russia":
				startHP = 750;
				startEnergy = 750;
                speed = 10.0f;
				damageMultiplier = 1.0f;
				energyMultiplier = 1.25f;
                bankSize = 5000;
				energyBank = 0;
				break;
			default: 
			    startHP = 1000;
			    startEnergy = 1000;
                speed = 10.0f;
			    damageMultiplier = 1f;
			    energyMultiplier = 1f;
                bankSize = 7000;
				energyBank = 0;
				break;
		}
		if (activeChar == null) activeChar = "tester";
        Debug.Log("Active " + activeChar);
		hitPoints = startHP;
		energyLevel = startEnergy;
        WeaponHandler.ScaleDamages(damageMultiplier);
	}
	
	void Update() {
		/*
		wepType = WeaponHandler.wepType;
		switch(wepType) {
		case 1: 
			selectedWepDrain = 6;
			break;
		case 2: 
			selectedWepDrain = 15;
			break;
		case 3: 
			selectedWepDrain = 70;
			break;
		default:
			break;
		}
		*/
		// Recharge power supply
		if (energyLevel > 0 && energyLevel <= startEnergy && Time.timeScale != 0) energyLevel += (startEnergy/800);
        if (energyLevel <= 0) energyLevel = 1;
		if (hitPoints < 0) hitPoints = 0;
        if (!bankFull) energyBank += (startEnergy / 1500);
	}
}
