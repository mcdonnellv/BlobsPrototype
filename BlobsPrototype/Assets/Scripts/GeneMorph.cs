using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class GeneMorph {
	public int geneId = 0;
	public int morphWeight = 100;
	public GeneMorph() {}
	public GeneMorph(GeneMorph m) {
		geneId = m.geneId;
		morphWeight = m.morphWeight;
	}
	
	static int GetTotalWeight(List<GeneMorph> ml) {
		if(ml == null || ml.Count == 0)
			return -1;
		int t = 0;
		foreach(GeneMorph g in ml)
			t += g.morphWeight;
		return t;
	}
	
	public static GeneMorph RollForMorph(List<GeneMorph> ml) {
		if(ml == null || ml.Count == 0)
			return null;
		int totalWeight = GeneMorph.GetTotalWeight(ml);
		int cummulativeWeight = 0;
		int roll = UnityEngine.Random.Range(0,totalWeight);
		foreach(GeneMorph g in ml) {
			cummulativeWeight += g.morphWeight;
			if(roll < cummulativeWeight)
				return g;
		}
		return null;
	}
}
