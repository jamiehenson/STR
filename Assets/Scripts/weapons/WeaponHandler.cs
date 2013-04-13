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
	
	private static WeaponHandler Instance;

    // Base weapon damages
    public static float beamDamage = 1;
    public static float cannonDamage = 3;
    public static float mineDamage = 7;
    public static float mineFragmentDamage = 0.2f;
	
	public void Start() {
		Instance = this;
	}

    public static void ScaleDamages(float scale) {
        beamDamage *= scale;
        cannonDamage *= scale;
        mineDamage *= scale;
        mineFragmentDamage *= scale;
    }
	
	public static WeaponStats GetWeaponStats(string player, int wep)
	{
		WeaponStats stats = null;
		
		switch(wep) {
			case 1:
				if (player == "china")	
					stats = new BeamStats(Instance.beamPrefabChina, 4000, 0.10f);
			
	            if (player == "usa" || player == "tester")	
					stats = new BeamStats(Instance.beamPrefabUSA, 1000, 0.10f);
			
	            if (player == "russia")
					stats = new BeamStats(Instance.beamPrefabRussia,2000,0.10f);
				break;
			case 2:
				if (player == "china") 
					stats = new CannonStats(Instance.cannonPrefabChina,800,0.2f);
	            
				if (PlayerManager.activeChar == "usa")
					stats = new CannonStats(Instance.cannonPrefabUSA,800,0.2f);
	            
	            if (player == "russia" || player == "tester") 
					stats = new CannonStats(Instance.cannonPrefabRussia,800,0.2f);
				break;
			case 3: 
				if (player == "china" || player == "tester")
					stats = new MineStats(Instance.minePrefabChina,400,3f);
	            
	            if (player == "usa")
					stats = new MineStats(Instance.minePrefabUSA,400,3f);
	
	            if (player == "russia")
					stats = new MineStats(Instance.minePrefabRussia,400,3f);
				break;
			default:
				break;
		}
		
		if (stats ==  null)
			Log.Error("WeaponHandler Error", "stats == null! Gosh!");
		
		return stats;
	}

	public void Update() {
		WeaponStats stats = GetWeaponStats(PlayerManager.activeChar, wepType);
		
		wepPrefab = stats.wepPrefab;
		wepSpeed = stats.wepSpeed;
		wepRate = stats.wepRate;
		wepName = stats.wepName;
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