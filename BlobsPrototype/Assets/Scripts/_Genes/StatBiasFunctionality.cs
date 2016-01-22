using UnityEngine;
using System;
using System.Collections.Generic;

public class StatBiasFunctionality {
	public void ApplyBiasWithStat(int deviation, Stat s) {
		int deviationRoll = UnityEngine.Random.Range(1, deviation + 1);
		s.ModBirthValue(deviationRoll);
	}
}
