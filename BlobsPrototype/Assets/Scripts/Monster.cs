using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class BaseMonster : BaseThing {
	public CombatStats combatStats = new CombatStats();
}

[Serializable]
public class Monster : BaseMonster {
	public int level = 1;

	public Monster(BaseMonster b) {
		id = b.id;
		itemName = b.itemName;
		description = b.description;
		iconName = b.iconName;
		iconAtlas = b.iconAtlas;
		iconTintIndex = b.iconTintIndex;
		quality = b.quality;
	}

}
