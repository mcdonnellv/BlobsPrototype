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
	public Stat attack;
	public Stat armor;
	public Stat health;
	public Stat stamina;
	public Stat speed;
	public Element element;
	public List<StatBias> statBias = new List<StatBias>();


	public static int defaultAttack = 100;
	public static int defaultArmor = 100;
	public static int defaultHealth = 100;
	public static int defaultStamina = 100;
	public static int defaultSpeed = 100;
	public static Element defaultElement = Element.None;

	public CombatStats() { SetDefaultValues(); }
	public CombatStats(CombatStats c) { 
		attack = new Stat(CombatStatType.Attack, c.attack);
		armor = new Stat(CombatStatType.Armor, c.armor);
		health = new Stat(CombatStatType.Health, c.health);
		stamina = new Stat(CombatStatType.Stamina, c.stamina);
		speed = new Stat(CombatStatType.Speed, c.speed);
		element = c.element;
	}


	public void SetDefaultValues() {
		attack = new Stat(CombatStatType.Attack, defaultAttack);
		armor = new Stat(CombatStatType.Armor, defaultArmor);
		health = new Stat(CombatStatType.Health, defaultHealth);
		stamina = new Stat(CombatStatType.Stamina, defaultStamina);
		speed = new Stat(CombatStatType.Speed, defaultSpeed);
		element = defaultElement;
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
		return StatFromType(resultStatType);
	}


	public Stat StatFromType(CombatStatType t) {
		switch(t) {
		case CombatStatType.Attack: return attack;
		case CombatStatType.Armor: return armor;
		case CombatStatType.Health: return health;
		case CombatStatType.Stamina: return stamina;
		case CombatStatType.Speed: return speed;
		}
		return attack;
	}

}
