using UnityEngine;
using System.Collections;

public class WeaponHandler : MonoBehaviour {
	public int wepType = 1;
	public float wepSpeed;
	public Transform wepPrefab;
	public float wepRate;
    public string wepName;
	public Transform beamPrefabChina;
	public Transform cannonPrefabChina;
	public Transform minePrefabChina;
	public Transform beamPrefabUSA;
	public Transform cannonPrefabUSA;
	public Transform minePrefabUSA;
	public Transform beamPrefabRussia;
	public Transform cannonPrefabRussia;
	public Transform minePrefabRussia;

    // Base weapon damages
    public static float beamDamage = 1;
    public static float cannonDamage = 3;
    public static float mineDamage = 7;
    public static float mineFragmentDamage = 0.2f;

    public static void ScaleDamages(float scale) {
        beamDamage *= scale;
        cannonDamage *= scale;
        mineDamage *= scale;
        mineFragmentDamage *= scale;
    }

	public void Update() {

		switch(wepType) {
			case 1: 
				if (PlayerManager.activeChar == "china") {	
					wepPrefab = beamPrefabChina;
					wepSpeed = 4000;
	                wepRate = 0.10f;
	            }
	            if (PlayerManager.activeChar == "usa" || PlayerManager.activeChar == "tester") {	
					wepPrefab = beamPrefabUSA;
					wepSpeed = 1000;
	                wepRate = 0.10f;
	            }
	            if (PlayerManager.activeChar == "russia") {	
					wepPrefab = beamPrefabRussia;
					wepSpeed = 2000;
	                wepRate = 0.10f;
	            }
                wepName = "Beam";
				break;
			case 2:
				if (PlayerManager.activeChar == "china") {	
					wepPrefab = cannonPrefabChina;
					wepSpeed = 800;
	                wepRate = 0.2f;
	            }
				if (PlayerManager.activeChar == "usa") {
					wepPrefab = cannonPrefabUSA;
					wepSpeed = 800;
	                wepRate = 0.2f;
	            }
	            if (PlayerManager.activeChar == "russia" || PlayerManager.activeChar == "tester") {	
					wepPrefab = cannonPrefabRussia;
					wepSpeed = 800;
	                wepRate = 0.2f;
	            }
                wepName = "Cannon";
				break;
			case 3: 
				if (PlayerManager.activeChar == "china" || PlayerManager.activeChar == "tester") {	
					wepPrefab = minePrefabChina;
					wepSpeed = 400;
	                wepRate = 3f;
	            }
	            if (PlayerManager.activeChar == "usa") {	
	            	wepPrefab = minePrefabUSA;
					wepSpeed = 400;
	                wepRate = 3f;
	            }
	            if (PlayerManager.activeChar == "russia") {	
	            	wepPrefab = minePrefabRussia;
					wepSpeed = 400;
	                wepRate = 3f;
	            }
                wepName = "Mine";
				break;
			default: break;
		}
	}
}

public class WeaponStats {
	public readonly int wepType;
	public readonly float wepSpeed;
	public readonly Transform wepPrefab;
	public readonly float wepRate;
    public readonly string wepName;
	
	public WeaponStats(int _wepType, Transform _wepPrefab, float _wepSpeed, float _wepRate, string _wepName){
		wepType = _wepType;
		wepSpeed = _wepSpeed;
		wepPrefab = _wepPrefab;
		wepRate = _wepRate;
		wepName = _wepName;
	}
}

public class BeamStats : WeaponStats {
	public BeamStats(Transform wepPrefab, float wepSpeed, float wepRate)
		: base(1, wepPrefab, wepSpeed, wepRate, "Beam") {}
}

public class CannonStats : WeaponStats {
	public CannonStats(Transform wepPrefab, float wepSpeed, float wepRate)
		: base(2, wepPrefab, wepSpeed, wepRate, "Cannon") {}
}

public class MineStats : WeaponStats {
	public MineStats(Transform wepPrefab, float wepSpeed, float wepRate)
		: base(3, wepPrefab, wepSpeed, wepRate, "Mine") {}
}