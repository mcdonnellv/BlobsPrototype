using UnityEngine;
using System;
using System.Collections.Generic;

public class StatBiasHealthFunctionality : StatBiasFunctionality, IGeneFunctionality {
	public bool CanExistWithWith(TraitType t, Gene g) { return CanExistWithWithBias(t); }
	public void OnBirth(Blob blob, Gene gene) { ApplyBiasWithStat(gene.value, blob.combatStats.health); }
	public void OnAdd(Blob blob, Gene gene) {}
	public void OnCombatStart(){}
	public void OnRemove(Blob blob, Gene gene){}
}
