using UnityEngine;
using System;
using System.Collections.Generic;

public class SigilGeneFunctionality : IGeneFunctionality {
	public bool CanExistWithWith(TraitType t, Gene g) { return t != g.traitType;}
	public void OnBirth(Blob blob, Gene gene) { blob.sigil = (Sigil)gene.value; }
	public void OnAdd(Blob blob, Gene gene) {}
	public void OnCombatStart(){}
	public void OnRemove(Blob blob, Gene gene){}
}