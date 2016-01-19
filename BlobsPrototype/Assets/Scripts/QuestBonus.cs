using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public enum QuestBonusType {
	Health,
	Attack,
	Armor,
	Speed,
	IncreasedRewards,
	IncreasedRewardsScouting,
	QuestBonusTypeCt,
};


public class QuestBonus { 
	public string description;
	public float value;
	public QuestBonusType type;
	public int weight;

	public QuestBonus (float val, QuestBonusType t, int w) {
		value = val;
		type = t;
		weight = w;
		description = BuildDescription();
	}

	public string BuildDescription() { 
		if(type == QuestBonusType.IncreasedRewards) {
			string str = "Increased Rewards";
			for( int i = 0; i < value; i++)
				str += "+";
			return str;
		}

		if(type == QuestBonusType.IncreasedRewardsScouting) {
			string str = "More Quests Scouted";
			for( int i = 0; i < value; i++)
				str += "+";
			return str;
		}

		return "+" + (Mathf.FloorToInt(value * 100f)).ToString() + "% to " + type.ToString(); }
}