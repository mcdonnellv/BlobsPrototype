using UnityEngine;
using System;

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
