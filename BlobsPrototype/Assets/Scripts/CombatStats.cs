using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Stat {
	public CombatStatType type;
	int _defaultValue;
	int _birthValue;
	int _geneModdedValue;
	int _combatValue;

	public int defaultValue { get { return _defaultValue; } }
	public int birthValue { get { return _birthValue; } }
	public int geneModdedValue { get { return _geneModdedValue; } }
	public int combatValue { get { return _combatValue; } }
	public static int defaultStartingValue = 100;

	public Stat(CombatStatType t, Stat s) {
		type = t;
		_defaultValue = s.defaultValue;
		_birthValue = s.birthValue;
		_geneModdedValue = s.geneModdedValue;
		_combatValue = s.combatValue;
	}
	
	public Stat(CombatStatType t, int v) {
		type = t;
		_defaultValue = v;
		_birthValue = v;
		_geneModdedValue = v;
		_combatValue = v;
	}

	public void ResetCombatValue() { _combatValue = _geneModdedValue; }
	public void ResetGeneModdedValue() { _geneModdedValue = _birthValue; }
	public void ModBirthValue(int v) { _birthValue = _defaultValue + v; }
	public void ModGeneModdedValue(int v) { _geneModdedValue = _birthValue + v; }
	public void ModGeneModdedValue(AbilityModifier m, int v) { _geneModdedValue = (m == AbilityModifier.Added) ? _birthValue + v : _birthValue * v; }
	public void ModCombatValue(AbilityModifier m, int v) { _combatValue = (m == AbilityModifier.Added) ? _combatValue + v : _combatValue * v; }
}


[System.Serializable]
public class StatBias {
	public CombatStatType type;
	public bool positive;
	public StatBias(CombatStatType t, bool p) { type = t; positive = p; }
}


[System.Serializable]
public class CombatStats {
	public List<Stat> allStats;
	public Stat attack { get { return allStats[(int)CombatStatType.Attack]; } set { allStats[(int)CombatStatType.Attack] = value; } } 
	public Stat armor { get { return allStats[(int)CombatStatType.Armor]; } set { allStats[(int)CombatStatType.Armor] = value; } } 
	public Stat health { get { return allStats[(int)CombatStatType.Health]; } set { allStats[(int)CombatStatType.Health] = value; } } 
	public Stat stamina { get { return allStats[(int)CombatStatType.Stamina]; } set { allStats[(int)CombatStatType.Stamina] = value; } } 
	public Stat speed { get { return allStats[(int)CombatStatType.Speed]; } set { allStats[(int)CombatStatType.Speed] = value; } } 
	public Element element;
	public List<StatBias> statBias = new List<StatBias>();
	public static Element defaultElement = Element.None;


	public CombatStats() {
		allStats = new List<Stat>();
		for(int i=0; i < (int)CombatStatType.CombatStatTypeCt; i++)
			allStats.Add(new Stat((CombatStatType)i, Stat.defaultStartingValue));
	}

	public CombatStats(CombatStats c) { 
		allStats = new List<Stat>();
		for(int i=0; i < (int)CombatStatType.CombatStatTypeCt; i++)
			allStats.Add(new Stat((CombatStatType)i, c.allStats[i]));
		element = c.element;
	}
	

	public void ResetForCombat() {
		for(int i=0; i < (int)CombatStatType.CombatStatTypeCt; i++)
			allStats[i].ResetCombatValue();
	}

	public void CalculateOtherStats(TraitType t, float v) {
		switch (t) {
		case TraitType.SetElement: element = (Element)v; break;
		}
	}


	public void RandomizeBirthStats(int numPositive, int numNegative, int deviation, List<StatBias> bias) {
		for(int i=0; i < numPositive; i++) {
			int deviationRoll = UnityEngine.Random.Range(1, deviation + 1);
			Stat s = GetRandomStat(GetPosOrNegBias(bias, true));
			s.ModBirthValue(deviationRoll);
			statBias.Add(new StatBias(s.type, true));
		}

		for(int i=0; i < numNegative; i++) {
			int deviationRoll = UnityEngine.Random.Range(-1, -(deviation + 1));
			Stat s = GetRandomStat(GetPosOrNegBias(bias, false));
			s.ModBirthValue(deviationRoll);
			statBias.Add(new StatBias(s.type, false));
		}
	}


	List<StatBias> GetPosOrNegBias(List<StatBias> b, bool positive) {
		List<StatBias> r = new List<StatBias>();
		foreach(StatBias s in b)
			if((positive && s.positive) || (!positive && !s.positive))
				r.Add(s);
		return r;
	}


	Stat GetRandomStat(List<StatBias> biasList) {
		int biasWeight = 2;
		int numStats = 5;
		CombatStatType[] possibleStats = new CombatStatType[numStats + biasList.Count * biasWeight];
		possibleStats[0] = CombatStatType.Attack;
		possibleStats[1] = CombatStatType.Armor;
		possibleStats[2] = CombatStatType.Health;
		possibleStats[3] = CombatStatType.Stamina;
		possibleStats[4] = CombatStatType.Speed;
		int i = numStats;
		foreach(StatBias s in biasList)
			for(int j = 0; j < biasWeight; j++)
				possibleStats[i++] = s.type;

		int statTypeRoll = UnityEngine.Random.Range(0, possibleStats.Length);
		CombatStatType resultStatType = possibleStats[statTypeRoll];
		return allStats[(int)resultStatType];
	}

}
