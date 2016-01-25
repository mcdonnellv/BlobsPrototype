using UnityEngine;
using System;
using System.Collections.Generic;


[Serializable]
public class BaseGene : BaseThing {
	public TraitType traitType = TraitType.None;
	public TraitCondition traitCondition = TraitCondition.None;
	public int value = 0;
	public AbilityModifier modifier = AbilityModifier.NA;
	public List<GeneActivationRequirement> activationRequirements = new List<GeneActivationRequirement>();

	// can this gene be bought in the store
	public bool showInStore = false; 

	// genes that do things behind the scene like assign a blob's color/sigil, or change starting stats
	public bool hiddenGene = false; 

	// when passing the gene, this gene can be replaced by any gene in this list if conditions are met
	public List<GeneMorph> morphList = new List<GeneMorph>(); 

	// when passing the gene, this is the chance the gene will morph into another
	public float morphChance = 0f; 
}