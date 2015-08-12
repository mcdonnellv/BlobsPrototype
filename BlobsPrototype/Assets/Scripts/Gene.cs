using UnityEngine;
using System;
using System.Collections.Generic;




[Serializable]
public class Gene : ScriptableObject 
{
	public enum Rarity
	{
		Standard,
		Common,
		Rare,
		Epic,
		Legendary,
	};

	public enum GeneStrength
	{
		VeryWeak = 10,
		Weak = 20,
		Normal = 30,
		Strong = 40,
		VeryStrong = 50,
	};

	public enum Type
	{
		None = -1,
		BodyColor,
		FacialFeature,
		Metabolism,
		Breeding,
		Intellect,
	};

	public enum GeneActivationRequirements
	{
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
	public List<string> preRequisites = new List<string>();
	public List<GeneActivationRequirements> activationRequirements = new List<GeneActivationRequirements>();

	public string preRequisite = "";
	public string description = "";
	public GeneStrength geneStrength = GeneStrength.Normal;
	public Rarity rarity = Rarity.Common;
	public bool revealed = false;
	public Type type = Type.None;
	public Color bodyColor = Color.white;
	public bool negativeEffect = false;
	public float revealChance { get {return Gene.RevealChanceForRarity(rarity);} }
	public float passOnChance { get {return Gene.PassOnChanceForRarity(rarity);} }
	public string name {get{return geneName;}}

	static float PassOnChanceForRarity(Rarity r)
	{
		if(r == Rarity.Standard)
			return 1f;
		return RevealChanceForRarity(r) * 14.28f;
	}


	static float RevealChanceForRarity(Rarity r)
	{
		switch (r)
		{
		case Rarity.Standard:  return 0f;
		case Rarity.Common:    return 0.07000f;
		case Rarity.Rare:      return 0.02140f;
		case Rarity.Epic:      return 0.00428f;
		case Rarity.Legendary: return 0.00108f;
		}
		return 0f;
	}


	public static Color ColorForRarity(Rarity r)
	{
		switch (r)
		{
		case Rarity.Standard:  return new Color(.7f, .7f, .7f, 1f);
		case Rarity.Common:    return Color.white;
		case Rarity.Rare:      return new Color(0.255f, 0.616f, 1f, 1f);
		case Rarity.Epic:      return new Color(0.957f, 0.294f, 1f, 1f);
		case Rarity.Legendary: return new Color(1f, 0.773f, 0.082f, 1f);
		}

		return Color.white;
	}


	public static string HexColorStringFromRarity(Rarity r)
	{
		return HexStringFromColor(Gene.ColorForRarity(r));
	}


	public static string HexStringFromColor(Color c)
	{
		return string.Format("[{0}{1}{2}]",
		                     ((int)(c.r * 255)).ToString("X2"),
		                     ((int)(c.g * 255)).ToString("X2"),
		                     ((int)(c.b * 255)).ToString("X2"));
	}

	public static string RarityStringFromrarity(Rarity r)
	{
		switch (r)
		{
		case Rarity.Standard:  return "Standard";
		case Rarity.Common:    return "Common";
		case Rarity.Rare:      return "Rare";
		case Rarity.Epic:      return "Epic";
		case Rarity.Legendary: return "Legendary";
		}

		return "?";
	}
}