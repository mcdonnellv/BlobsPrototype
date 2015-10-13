using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class Gene {

	public enum GeneType {
		None = -1,
		StatGene,
		MonsterGene,
	};

	public string itemName = "";
	public string description = "";
	public string name {get{return itemName;}}
	public Quality quality = Quality.Common;
	public bool revealed = false;
	public GeneType type = GeneType.None;
	public float revealChance { get {return Gene.RevealChanceForQuality(quality);} }
	public float passOnChance { get {return Gene.PassOnChanceForQuality(quality);} }
	public List<Stat> stats = new List<Stat>();
	public List<GeneReq> activationReq = new List<GeneReq>();
	public bool active = true;


	public static float PassOnChanceForQuality(Quality r) {
		switch (r) {
		case Quality.Standard:  return 0.25f;
		case Quality.Common:    return 0.20f;
		case Quality.Rare:      return 0.10f;
		case Quality.Epic:      return 0.05f;
		case Quality.Legendary: return 0.02f;
		}
		return 1f;
	}


	public static float RevealChanceForQuality(Quality r) {
		switch (r) {
		case Quality.Standard:  return 0f;
		case Quality.Common:    return 0.07000f;
		case Quality.Rare:      return 0.02140f;
		case Quality.Epic:      return 0.00428f;
		case Quality.Legendary: return 0.00108f;
		}
		return 0f;
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
}




