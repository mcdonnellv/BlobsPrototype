using UnityEngine;
using System;
using System.Collections.Generic;

public class StatModAttackFunctionality : StatModFunctionality, IGeneFunctionality {
	public bool CanExistWithWith(TraitType t, Gene g) { return CanExistWithWithMod(t); }
	public void OnBirth(Blob blob, Gene gene) { }
	public void OnAdd(Blob blob, Gene gene) { ModStat(gene.value, blob.combatStats.attack, gene.modifier); }
	public void OnCombatStart(){}
	public void OnRemove(Blob blob, Gene gene) { ModStat(-gene.value, blob.combatStats.attack, gene.modifier); }
}
