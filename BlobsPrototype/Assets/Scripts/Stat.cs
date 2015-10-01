using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
 public class Stat {
	public enum Identifier {
		Strength = 0,
		Dexterity,
		Intellect,
		Wisdom,
		Vitality,
		Charisma,
		StatIdCount
	};
	
	public enum Modifier {
		Added,
		Percent,
	};
	
	public Identifier id;
	public Modifier modifier;
	public int amount;
	public static List<Identifier> statIds;
	
	static public string GetName (Identifier i) {
		return i.ToString();
	}
	
	static public string GetDescription (Identifier i) {
		switch (i) {
		case Identifier.Strength:		return "Attack damages the enemy";
		case Identifier.Dexterity: 	return "Health allows blobs to take more damage";
		case Identifier.Intellect:		return "Speed makes attacks happen faster";
		case Identifier.Wisdom:	return "Endurance allows blobs to fight longer";
		}
		return null;
	}

	static public Identifier GetStatIdByIndex(int index) {
		if(statIds == null) {
			statIds = new List<Identifier>();
			statIds.Add(Identifier.Strength);
			statIds.Add(Identifier.Dexterity);
			statIds.Add(Identifier.Intellect);
			statIds.Add(Identifier.Wisdom);
			statIds.Add(Identifier.Vitality);
			statIds.Add(Identifier.Charisma);
		}
		return statIds[index];
	}
}