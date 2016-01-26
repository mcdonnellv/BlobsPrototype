using UnityEngine;
using System;
using System.Collections.Generic;

public class StatModFunctionality {
	public bool CanExistWithWithMod(TraitType t) { 
		switch(t) {
		case TraitType.ArmorMod:
		case TraitType.AttackMod:
		case TraitType.HealthMod:
		case TraitType.SpeedMod:
		case TraitType.StaminaMod:
			return false;
		}
		return true;
	}
	
	public void ModStat(int v, Stat s, AbilityModifier m) { 
		if(m == AbilityModifier.Added) {
			s.geneModdedValue += v;
		}
		else {
			if(v < 0) v = -1/v;
			s.geneModdedValue *= v;
		}
	}
}
