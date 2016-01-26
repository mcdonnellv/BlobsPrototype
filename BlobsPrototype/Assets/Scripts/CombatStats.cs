using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public class CombatStats {
	public List<Stat> allStats = new List<Stat>();
	public Stat attack { get { return allStats[(int)CombatStatType.Attack]; } set { allStats[(int)CombatStatType.Attack] = value; } } 
	public Stat armor { get { return allStats[(int)CombatStatType.Armor]; } set { allStats[(int)CombatStatType.Armor] = value; } } 
	public Stat health { get { return allStats[(int)CombatStatType.Health]; } set { allStats[(int)CombatStatType.Health] = value; } } 
	public Stat stamina { get { return allStats[(int)CombatStatType.Stamina]; } set { allStats[(int)CombatStatType.Stamina] = value; } } 
	public Stat speed { get { return allStats[(int)CombatStatType.Speed]; } set { allStats[(int)CombatStatType.Speed] = value; } } 
	public Element element;
	public static Element defaultElement = Element.None;


	public CombatStats() {
		for(int i=0; i < (int)CombatStatType.CombatStatTypeCt; i++)
			allStats.Add(new Stat((CombatStatType)i, Stat.defaultStartingValue));
	}

	public CombatStats(CombatStats c) { 
		for(int i=0; i < (int)CombatStatType.CombatStatTypeCt; i++)
			allStats.Add(new Stat((CombatStatType)i, c.allStats[i]));
		element = c.element;
	}
	

	public void ResetForCombat() {
		foreach(Stat s in allStats)
			s.ResetCombatValue();
	}

	public void CalculateOtherStats(TraitType t, float v) {
		switch (t) {
		case TraitType.SetElement: element = (Element)v; break;
		}
	}

	public void BirthDone() {
		foreach(Stat s in allStats)
			s.BirthDone();
	}
}
