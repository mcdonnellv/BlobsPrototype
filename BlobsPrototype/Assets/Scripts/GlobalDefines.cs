using UnityEngine;
using System;
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
	Questing,
	QuestComplete,
};


public enum CombatStatType {
	Attack = 0,
	Armor,
	Health,
	Stamina,
	Speed,
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
	SpeedMod,
	SetElement,
	TraitTypeCt,
};

public enum TraitCondition {
	None,
	Alone,
	ForEachOtherBlob,
	ColorBlack,
	ColorBlue,
	ColorWhite,
	ColorGreen,
	ColorRed,
};


public enum Element {
	None = -1,
	Black = 0,
	Blue,
	White,
	Green,
	Red,
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
	Popup1,
	Popup2,
	QuestDetails1,
	QuestDetails2,
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

public enum MapZone {
	None,
	Meadows,
	Mountains,
	Swamp,
	Forest,
	Coast,
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


	public static string SpriteNameForSigil(Sigil sigil) {
		switch (sigil)	{
		case Sigil.A: return "sigil_skull";
		case Sigil.B: return "sigil_tornado";
		case Sigil.C: return "sigil_omega";
		case Sigil.D: return "sigil_fire";
		case Sigil.E: return "sigil_thunder";
		}
		return "";
	}


	public static string TimeToString(TimeSpan ts) {
		string timeString = "";
		if(ts.Days > 0)
			timeString += ts.Days.ToString() + " day";
		if(ts.Days == 0 && ts.Hours > 0)
			timeString += (timeString == "" ? "" : "  " ) + ts.Hours.ToString() + " hr";
		if(ts.Days == 0 && ts.Hours == 0 && ts.Minutes > 0)
			timeString += (timeString == "" ? "" : "  " ) + ts.Minutes.ToString() + " min";
		if(ts.Days == 0 && ts.Hours == 0 && ts.Minutes == 0 && ts.Seconds > 0)
			timeString += (timeString == "" ? "" : "  " ) + ts.Seconds.ToString() + " sec";
		if(timeString == "")
			timeString = "0 sec";
		return timeString;
	}



}


