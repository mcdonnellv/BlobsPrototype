using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
 public class Stat {
	public enum Identifier {
		Attack = 0,
		Work,
		Fertility,
		Speed,
		StatIdCount,
	}
	
	public enum Modifier {
		Added,
		Percent,
	}
	
	public Identifier id;
	public Modifier modifier;
	public int amount;
	public static List<Identifier> statIds;
	
	static public string GetName (Identifier i) {
		return i.ToString();
	}
	
	static public string GetDescription (Identifier i) {
		switch (i) {
		case Identifier.Attack:		return "Attack damages the enemy";
		case Identifier.Work: 	return "Health allows blobs to take more damage";
		case Identifier.Fertility:		return "Speed makes attacks happen faster";
		case Identifier.Speed:	return "Endurance allows blobs to fight longer";
		}
		return null;
	}

	static public Identifier GetStatIdByIndex(int index) {
		if(statIds == null) {
			statIds = new List<Identifier>();
			statIds.Add(Identifier.Attack);
			statIds.Add(Identifier.Work);
			statIds.Add(Identifier.Fertility);
			statIds.Add(Identifier.Speed);
		}
		return statIds[index];
	}
}