using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class Gene : BaseGene {

	public GeneState state;
	public string name { get{return itemName;} }
	public bool active { get{return state == GeneState.Active;} }


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
		value = b.value;
		modifier = b.modifier;
		activationRequirements = new List<GeneActivationRequirement>();
		foreach (GeneActivationRequirement req in b.activationRequirements)
			activationRequirements.Add(new GeneActivationRequirement(req));
	
		state = GeneState.Passive;
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
		hudManager.popup.Show("Gene", "The " + itemName + " gene has been activated!");
		if(hudManager.inventoryMenu.IsDisplayed())
			hudManager.inventoryMenu.Hide();
		hudManager.blobInfoContextMenu.Show(hudManager.blobInfoContextMenu.blob.id);
	}

}




