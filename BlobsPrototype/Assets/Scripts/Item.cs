using UnityEngine;
using System;

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
		consumable = b.consumable;
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
