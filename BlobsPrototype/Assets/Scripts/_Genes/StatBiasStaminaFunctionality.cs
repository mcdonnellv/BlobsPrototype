using UnityEngine;
using System;
using System.Collections.Generic;

public class StatBiasStaminaFunctionality : StatBiasFunctionality, IGeneFunctionality {
	public bool CanExistWithWith(TraitType t, Gene g) { return t != g.traitType;}
	public void OnBirth(Blob blob, Gene gene) { ApplyBiasWithStat(gene.value, blob.combatStats.stamina); }
	public void OnCombatStart(){}
}
