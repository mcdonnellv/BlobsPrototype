using UnityEngine;
using System;
using System.Collections.Generic;




[Serializable]
public class Gene : ScriptableObject {

	public enum GeneStrength {
		VeryWeak = 10,
		Weak = 20,
		Normal = 30,
		Strong = 40,
		VeryStrong = 50,
	};

	public enum Type {
		None = -1,
		BodyColor,
		FacialFeature,
		Metabolism,
		Breeding,
		Intellect,
	};

	public enum GeneActivationRequirements {
		None = -1,
		GenderMustBeMale,
		GenderMustBeFemale,
		QualityMustAtLeastBePoor,
		QualityMustAtLeastBeFair,
		QualityMustAtLeastBeGood,
		QualityMustAtLeastBeExcellent,
		QualityMustAtLeastBeOutstanding,
		MustHaveNoActiveGenesOfSameType,
	};

	
	public string geneName = "";
	public string name {get{return geneName;}}
	public List<string> preRequisites = new List<string>();
	public List<GeneActivationRequirements> activationRequirements = new List<GeneActivationRequirements>();
	public string preRequisite = "";
	public string description = "";
	public GeneStrength geneStrength = GeneStrength.Normal;
	public Quality quality = Quality.Common;
	public bool revealed = false;
	public Type type = Type.None;
	public Color bodyColor = Color.white;
	public bool negativeEffect = false;
	public float revealChance { get {return Gene.RevealChanceForQuality(quality);} }
	public float passOnChance { get {return Gene.PassOnChanceForQuality(quality);} }
	public List<Stat> stats = new List<Stat>();


	public static float PassOnChanceForQuality(Quality r) {
		switch (r) {
		case Quality.Standard:  return 0.25f;
		case Quality.Common:    return 0.20f;
		case Quality.Rare:      return 0.10f;
		case Quality.Epic:      return 0.05f;
		case Quality.Legendary: return 0.02f;
		}
		return 0f;
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


	public static string HexColorStringFromRarity(Quality r) {
		return HexStringFromColor(ColorDefines.ColorForQuality(r));
	}


	public static string HexStringFromColor(Color c) {
		return string.Format("[{0}{1}{2}]",
		                     ((int)(c.r * 255)).ToString("X2"),
		                     ((int)(c.g * 255)).ToString("X2"),
		                     ((int)(c.b * 255)).ToString("X2"));
	}

	
	public GameObject CreateGeneGameObject() {
		GameObject geneGameObject = (GameObject)GameObject.Instantiate(Resources.Load("Gene"));
		GenePointer gp = geneGameObject.GetComponent<GenePointer>();
		UISprite s = geneGameObject.GetComponent<UISprite>();
		switch (quality)
		{
		case Quality.Standard:  
		case Quality.Common:    s.spriteName = "cardCommon"; break;
		case Quality.Rare:      s.spriteName = "cardRare"; break;
		case Quality.Epic:      s.spriteName = "cardEpic"; break;
		case Quality.Legendary: s.spriteName = "cardLegendary"; break;
		}
		gp.gene = this;
		return geneGameObject;
	}
}




