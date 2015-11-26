using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class BaseThing {
	public string itemName = "";
	public string description = "";
	public Quality quality = Quality.Common;
}

[Serializable]
public class BaseItem : BaseThing {
	public string iconName;
	public UIAtlas iconAtlas;
	public int maxStack = 99;
}


[Serializable]
public class Item : BaseItem {

	public int count = 0;

	public Item(BaseItem b) {
		itemName = b.itemName;
		description = b.description;
		iconName = b.iconName;
		iconAtlas = b.iconAtlas;
		quality = b.quality;
		maxStack = b.maxStack;
	}

	public GameObject CreateItemGameObject() {
		GameObject itemGameObject = (GameObject)GameObject.Instantiate(Resources.Load("Item"));
		ItemPointer ip = itemGameObject.GetComponent<ItemPointer>();
		UISprite s = itemGameObject.GetComponent<UISprite>();
		s.atlas = iconAtlas;
		s.spriteName = iconName;
		
		ip.item = this;
		return itemGameObject;
	}
}
