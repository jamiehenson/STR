using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour {
    // Player stats
    private int score;
    private float hitPoints;
    private float energyLevel;
    private float startHP, startEnergy;
    private float energyBank = 0;
    private int selectedWepDrain;
    private int bankSize = 10000;
    public static bool bankFull;
    public static float speed;
    public static string playername;
    private GameObject xp;

    // Character-centric player stats
    public string activeCharN;
    public static string activeChar;
    public int wepType;
	public WeaponStats wepStats;
    public static float damageMultiplier;
    public static float energyMultiplier;

    //Scoring System variables
    private bool myCharacter;
    private int characterNum;
	
	public void changeWeapon(int type){
		wepStats = WeaponHandler.GetWeaponStats(activeChar, type);
		wepType = type;
		
		if (Network.isClient)
			networkView.RPC("changeWeaponRPC", RPCMode.Server, type);
	}
	
	[RPC]
	public void changeWeaponRPC(int type){
		changeWeapon(type);
	}

    public float getEnergyLevel()
    {
        return energyLevel;
    }

    /* Score functions*/
    public int getScore()
    {
        return score;
    }

    public void updateScore(int s)
    {
        score = score + s;
    }

    /* End of core functions*/
    public void updateEnergyLevel(float val)
    {
        energyLevel = energyLevel + val;

    }

    public float getStartHP()
    {
        return startHP;
    }

    public float getStartEnergy()
    {
        return startEnergy;
    }

    public int getSelectedWepDrain()
    {
        return selectedWepDrain;
    }
    public int getBankSize()
    {
        return bankSize;
    }


    /*Hit Points call functions*/
    public float getHitPoints()
    {
        return hitPoints;
    }

    public void updateHitPoints(float val)
    {
        hitPoints = hitPoints + val;
    }

    public void resetHitPoints(float val)
    {
        hitPoints = val;
    }
    /*End of Hit Points call functions*/

    /* Energy Bank call functions*/
    public float getEnergyBank()
    {
        return energyBank;
    }

    public void resetEnergyBank(float val)
    {
        energyBank = val;
    }
    /* Energy Bank call functions*/

    public string getPlayerName()
    {
        return playername;
    }

    public string getActiveChar()
    {
        return activeChar;
    }

    public void activateCharacter(int charNum)
    {
        myCharacter = true;
        characterNum = charNum;
    }

    /* Called in HudOn class, Start() */
    public void InitialiseStats()
    {
		changeWeapon(1);
        if (Network.isClient)
        {
            networkView.RPC("updatePlayerName", RPCMode.Server, playername);
            switch (activeChar)
            {
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
            Debug.Log("Active " + activeCharN);
            hitPoints = startHP;
            energyLevel = startEnergy;
            score = 0;
            WeaponHandler.ScaleDamages(damageMultiplier);
            networkView.RPC("updateStartEnergy", RPCMode.Server, startEnergy);
            networkView.RPC("updateEnergy", RPCMode.All, energyLevel);
            networkView.RPC("updateHitP", RPCMode.All, hitPoints);
        }
    }

    void Update()
    {

        if (Network.isClient && myCharacter)
        {
            WeaponHandler weaponHandler = GameObject.Find("Character" + characterNum).GetComponent<WeaponHandler>();
 
            switch (wepType)
            {
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
            networkView.RPC("updateWepDrain", RPCMode.Server, selectedWepDrain);
        }

        // Recharge power supply
        if (Network.isServer)
        {
            if (energyLevel > 0 && energyLevel <= startEnergy && Time.timeScale != 0) energyLevel += (startEnergy / 800);
            if (energyLevel <= 0) energyLevel = 1;
            if (hitPoints < 0) hitPoints = 0;
            if (!bankFull) energyBank += (startEnergy / 1500);
            networkView.RPC("updateEnergy", RPCMode.All, energyLevel);
            networkView.RPC("updateHitP", RPCMode.All, hitPoints);
            networkView.RPC("updatePlayerScore", RPCMode.All, score);
        }
    }

    [RPC]
    void updateEnergy(float e)
    {
        energyLevel = e;
    }

    [RPC]
    void updateWepDrain(int e)
    {
        selectedWepDrain = e;
    }

    [RPC]
    void updateHitP(float e)
    {
        hitPoints = e;
    }

    [RPC]
    void updateStartEnergy(float e)
    {
        startEnergy = e;
    }

    [RPC]
    void updatePlayerName(string name)
    {
        playername = name;
    }

    [RPC]
    void updatePlayerScore(int s)
    {
        if (Network.isClient)
        {
            score = s;
        }
    }

}