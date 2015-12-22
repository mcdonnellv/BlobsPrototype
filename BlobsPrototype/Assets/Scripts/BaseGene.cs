using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class BaseGene : BaseThing {
	public TraitType traitType = TraitType.None;
	public int value = 0;
	public AbilityModifier modifier = AbilityModifier.NA;
	public List<GeneActivationRequirement> activationRequirements = new List<GeneActivationRequirement>();
}