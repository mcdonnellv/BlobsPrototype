using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class Item {
	public string itemName = "";
	public string description = "";
	public string iconName;
	public UIAtlas iconAtlas;
	public Quality quality = Quality.Common;


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
