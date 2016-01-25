using UnityEngine;
using System;
using System.Collections.Generic;

public class GeneSlotsFunctionality : IGeneFunctionality {
	public bool CanExistWithWith(TraitType t, Gene g) { return t != g.traitType;}
	public void OnBirth(Blob blob, Gene gene) { blob.geneSlots = gene.value; }
	public void OnCombatStart(){}
}