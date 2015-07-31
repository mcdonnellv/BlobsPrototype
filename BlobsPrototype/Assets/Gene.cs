using UnityEngine;
using System;
using System.Collections.Generic;




[Serializable]
public class Gene : ScriptableObject 
{
	public enum Rarity
	{
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
		BodyColor,
	};
	
	public string geneName;
	public string preRequisite;
	public GeneStrength geneStrength;
	public Rarity rarity;
	public bool revealed = false;
	public Type type;
	public Color bodyColor;
	public float revealChance { get {return Gene.RevealChanceForRarity(rarity);} }
	public string name {get{return geneName;}}

	static float RevealChanceForRarity(Rarity r)
	{
		switch (r)
		{
		case Rarity.Common:    return 0.07000f;
		case Rarity.Rare:      return 0.02140f;
		case Rarity.Epic:      return 0.00428f;
		case Rarity.Legendary: return 0.00108f;
		}
		return 0f;
	}
}