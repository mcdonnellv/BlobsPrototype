using UnityEngine;
using System.Collections;

public enum Quality {
	None = -1,
	Bad,
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
	Element,
};


public enum AbilityModifier {
	NA,
	Added,
	Percent,
};


public enum TraitType {
	None,
	AttackMod,
	ArmorMod,
	HealthMod,
	StaminaMod,
	SetElement,
	TraitTypeCt,
};


public enum Element {
	None = -1,
	Black = 0,
	Blue,
	White,
	Red,
	Green,
	ElementCt,
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


public enum PopupPosition {
	DontSet = -1,
	Center = 0,
	Right1,
	Right2,
	Left1,
	Max,
};



public class GlobalDefines : MonoBehaviour {
}
