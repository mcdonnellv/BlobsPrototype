using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class BaseThing {
	public int id;
	public string itemName = "";
	public string description = "";
	public Quality quality = Quality.Common;
	public string iconName;
	public UIAtlas iconAtlas;
	public int iconTintIndex = 0;
	public int sellValue = 0;
}

[Serializable]
public class BaseItem : BaseThing {
	public int maxStack = 99;
}


[Serializable]
public class Item : BaseItem {

	public int count = 1;

	public Item(BaseItem b) {
		id = b.id;
		itemName = b.itemName;
		description = b.description;
		iconName = b.iconName;
		iconAtlas = b.iconAtlas;
		quality = b.quality;
		maxStack = b.maxStack;
		sellValue = b.sellValue;
		iconTintIndex = b.iconTintIndex;
	}

	public GameObject CreateItemGameObject(MonoBehaviour owner) {
		GameObject itemGameObject = (GameObject)GameObject.Instantiate(Resources.Load("Item"));
		ItemPointer ip = itemGameObject.GetComponent<ItemPointer>();
		ip.owningMenu = owner;
		UISprite s = itemGameObject.GetComponent<UISprite>();
		s.atlas = iconAtlas;
		s.spriteName = iconName;
		s.color = ColorDefines.IconColorFromIndex(iconTintIndex);
		ip.item = this;
		return itemGameObject;
	}
}
