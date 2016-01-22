using UnityEngine;
using System;
using System.Collections.Generic;

public class StatBiasFunctionality {

	public bool CanExistWithWithBias(TraitType t) { 
		switch(t) {
		case TraitType.AttackMod:
		case TraitType.ArmorMod:
		case TraitType.HealthMod:
		case TraitType.SpeedMod:
		case TraitType.StaminaMod:
			return false;
		}
		return true;
	}

	public void ApplyBiasWithStat(int deviation, Stat s) {
		int deviationRoll = UnityEngine.Random.Range(1, deviation + 1);
		s.ModBirthValue(deviationRoll);
	}
}
