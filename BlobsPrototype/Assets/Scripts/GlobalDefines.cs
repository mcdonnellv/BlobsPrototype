using UnityEngine;
using System.Collections;

public enum Quality {
	None = -1,
	Standard,
	Common,
	Rare,
	Epic,
	Legendary,
};

public enum Gender {
	Male,
	Female,
}

public enum BlobState {
	Idle,
	Breeding,
	Hatching,
	HatchReady,
	Working,
	WorkingReady,
};

public enum CombatStatType {
	Attack = 0,
	Armor,
	Health,
	Stamina,
};

public enum AbilityModifier {
	Added,
	Percent,
};


public enum TraitType {
	None,
	AttackMod,
	ArmorMod,
	HealthMod,
	StaminaMod,
	TraitTypeCt,
};

public enum GeneState {
	Passive,
	Available,
	Active,
};

public enum BlobInteractAction {
	Breed,
	Merge,
};


public class GlobalDefines : MonoBehaviour {
}
