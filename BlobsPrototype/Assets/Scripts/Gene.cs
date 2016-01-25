using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public interface IGeneFunctionality {
	bool CanExistWithWith(TraitType t, Gene g);
	void OnBirth(Blob blob, Gene g);
	void OnCombatStart();
}

public static class GeneFunctionalityFactory {
	private static Dictionary<TraitType, Func<IGeneFunctionality>> map = new Dictionary<TraitType, Func<IGeneFunctionality>>() {
		{TraitType.SetElement, () => { return new ElementGeneFunctionality(); }},
		{TraitType.SetSigil, () => { return new SigilGeneFunctionality(); }},
		{TraitType.SetGeneSlots, () => { return new GeneSlotsFunctionality(); }},
		{TraitType.StatBiasAttack, () => { return new StatBiasAttackFunctionality(); }},
		{TraitType.StatBiasArmor, () => { return new StatBiasArmorFunctionality(); }},
		{TraitType.StatBiasHealth, () => { return new StatBiasHealthFunctionality(); }},
		{TraitType.StatBiasStamina, () => { return new StatBiasStaminaFunctionality(); }},
		{TraitType.StatBiasSpeed, () => { return new StatBiasSpeedFunctionality(); }}
	};
	public static IGeneFunctionality GeneFunctionalityFromTraitType(TraitType t) { return map[t](); }
}


[Serializable]
public class Gene : BaseGene {

	public GeneState state;
	public string name { get{return itemName;} }
	public bool active { get{return state == GeneState.Active;} }
	public IGeneFunctionality functionality;


	public Gene() {
		itemName = "";
		description = "";
		quality = Quality.Common;
		traitType = TraitType.None;
		value = 0;
		activationRequirements = new List<GeneActivationRequirement>();
		state = GeneState.Passive;
	}


	public Gene(BaseGene b) {
		id = b.id;
		itemName = b.itemName;
		description = b.description;
		iconName = b.iconName;
		iconAtlas = b.iconAtlas;
		quality = b.quality;
		traitType = b.traitType;
		traitCondition = b.traitCondition;
		value = b.value;
		modifier = b.modifier;
		showInStore = b.showInStore;
		hiddenGene = b.hiddenGene;
		morphChance = b.morphChance;
		morphList = new List<GeneMorph>();
		foreach (GeneMorph m in b.morphList)
			morphList.Add(new GeneMorph(m));
		activationRequirements = new List<GeneActivationRequirement>();
		foreach (GeneActivationRequirement req in b.activationRequirements)
			activationRequirements.Add(new GeneActivationRequirement(req));
		state = GeneState.Passive;
		functionality = GeneFunctionalityFactory.GeneFunctionalityFromTraitType(traitType);
	}


	public GameObject CreateGeneGameObject(MonoBehaviour owner) {
		GameObject geneGameObject = (GameObject)GameObject.Instantiate(Resources.Load("Gene"));
		GenePointer gp = geneGameObject.GetComponent<GenePointer>();
		gp.owningMenu = owner;
		UISprite s = geneGameObject.GetComponent<UISprite>();
		s.atlas = iconAtlas;
		s.spriteName = iconName;
		gp.gene = this;
		return geneGameObject;
	}


	public void CheckActivationStatus() {
		if (state != GeneState.Available)
			return;

		foreach(GeneActivationRequirement req in activationRequirements)
			if (req.fulfilled == false)
				return;
		Activate();

	}


	void Activate() {
		HudManager hudManager = HudManager.hudManager;
		state = GeneState.Active;
		hudManager.ShowNotice("The " + itemName + " gene is now active");
		if(hudManager.inventoryMenu.IsDisplayed())
			hudManager.inventoryMenu.Hide();
		hudManager.blobInfoContextMenu.Show(hudManager.blobInfoContextMenu.blob.id);
	}
	

	public Gene MorphIfNeeded() {
		if(morphChance <= 0f || morphList == null || morphList.Count == 0)
			return this;
		if(UnityEngine.Random.Range(0f,1f) >= morphChance)
			return this;
		GeneMorph gm = GeneMorph.RollForMorph(morphList);
		BaseGene newBaseGene = GeneManager.geneManager.GetBaseGeneByID(gm.geneId);
		return new Gene(newBaseGene);
	}

}




