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
	OnQuest,
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


public enum Sigil {
	None = -1,
	A,
	B,
	C,
	D,
	E,
	SigilCt,
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
	Right3,
	Left1,
	Left2,
	Max,
};

public enum QuestState {
	NotAvailable,
	Available,
	Embarked,
	Completed,
};

public enum QuestType {
	Gathering, // will give resources
	Combat, // will give items for crafting
	Scouting, // will add rare quests if successful
};


public class GlobalDefines : MonoBehaviour {

	public static float GetQualityPercentage(Quality q) {
		switch (q)	{
		case Quality.Common: return    .700f;
		case Quality.Rare: return      .244f;
		case Quality.Epic: return      .044f;
		case Quality.Legendary: return .012f;
		}
		return 0;
	}

	public static string StringForSigil(Sigil sigil) {
		switch (sigil)	{
		case Sigil.A: return "[sigilA]";
		case Sigil.B: return "[sigilB]";
		case Sigil.C: return "[sigilC]";
		case Sigil.D: return "[sigilD]";
		case Sigil.E: return "[sigilE]";
		}
		return "";
	}
}


