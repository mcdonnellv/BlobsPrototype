using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public static class GeneFunctionalityFactory {
	private static Dictionary<TraitType, Func<IGeneFunctionality>> map = new Dictionary<TraitType, Func<IGeneFunctionality>>() {
		{TraitType.SetElement, () => { return new ElementGeneFunctionality(); }},
		{TraitType.SetSigil, () => { return new SigilGeneFunctionality(); }},
		{TraitType.SetGeneSlots, () => { return new GeneSlotsFunctionality(); }},
		{TraitType.StatBiasAttack, () => { return new StatBiasAttackFunctionality(); }},
		{TraitType.StatBiasArmor, () => { return new StatBiasArmorFunctionality(); }},
		{TraitType.StatBiasHealth, () => { return new StatBiasHealthFunctionality(); }},
		{TraitType.StatBiasStamina, () => { return new StatBiasStaminaFunctionality(); }},
		{TraitType.StatBiasSpeed, () => { return new StatBiasSpeedFunctionality(); }},
		{TraitType.AttackMod, () => { return new StatModAttackFunctionality(); }}
	};
	public static IGeneFunctionality GeneFunctionalityFromTraitType(TraitType t) { 
		Func<IGeneFunctionality> v;
		if(map.TryGetValue(t, out v))
			return v();
		else return null;
	}
}
