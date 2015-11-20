using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class BaseGene {
	public string geneName = "";
	public string description = "";
	public TraitType traitType = TraitType.None;
	public int value = 0;
	public AbilityModifier modifier = AbilityModifier.Added;
	public Quality quality = Quality.Common;
	public List<GeneActivationRequirement> activationRequirements = new List<GeneActivationRequirement>();
}

[Serializable]
public class Gene : BaseGene {

	public GeneState state;
	public string name { get{return geneName;} }
	public bool active { get{return state == GeneState.Active;} }


	public Gene() {
		geneName = "";
		description = "";
		quality = Quality.Common;
		traitType = TraitType.None;
		value = 0;
		activationRequirements = new List<GeneActivationRequirement>();
		state = GeneState.Passive;
	}

	public Gene(BaseGene b) {
		geneName = b.geneName;
		description = b.description;
		quality = b.quality;
		traitType = b.traitType;
		value = b.value;
		activationRequirements = new List<GeneActivationRequirement>();
		foreach (GeneActivationRequirement req in b.activationRequirements)
			activationRequirements.Add(new GeneActivationRequirement(req));
	
		state = GeneState.Passive;
	}

	public GameObject CreateGeneGameObject() {
		GameObject geneGameObject = (GameObject)GameObject.Instantiate(Resources.Load("Gene"));
		GenePointer gp = geneGameObject.GetComponent<GenePointer>();
		UISprite s = geneGameObject.GetComponent<UISprite>();
		s.spriteName = GetSpriteNameWithQuality(quality);
		gp.gene = this;
		return geneGameObject;
	}

	static public string GetSpriteNameWithQuality(Quality q) {
		string spriteName = "";
		switch (q) {
		case Quality.Standard:  
		case Quality.Common:    spriteName = "cardCommon"; break;
		case Quality.Rare:      spriteName = "cardRare"; break;
		case Quality.Epic:      spriteName = "cardEpic"; break;
		case Quality.Legendary: spriteName = "cardLegendary"; break;
		}

		return spriteName;
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
		state = GeneState.Active;
		HudManager hudManager = GameObject.Find("HudManager").GetComponent<HudManager>();
		hudManager.popup.Show("Gene", "The " + geneName + " gene has been activated!");
	}

}




