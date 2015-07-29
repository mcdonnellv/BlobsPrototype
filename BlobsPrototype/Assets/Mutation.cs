using UnityEngine;
using System;
using System.Collections.Generic;




[Serializable]
public class Mutation : ScriptableObject 
{
	public enum Rarity
	{
		Common,
		Rare,
		Epic,
		Legendary,
	};

	public enum Type
	{
		BodyColor,
	};
	
	public string mutationName;
	public string preRequisite;
	public Rarity rarity;
	public bool revealed = false;
	public Type type;
	public Color bodyColor;
	public float revealChance { get {return RevealChanceForRarity(rarity);} }


	float RevealChanceForRarity(Rarity r)
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