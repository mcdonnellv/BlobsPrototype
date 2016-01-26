using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class Gene : BaseGene {
	public IGeneFunctionality functionality;

	public Gene() {
		itemName = "";
		description = "";
		quality = Quality.Common;
		traitType = TraitType.None;
		value = 0;
		activationRequirements = new List<GeneActivationRequirement>();
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




