using UnityEngine;
using System.Collections;

public class LootCell : MonoBehaviour {
	public UISprite itemSocketSprite;
	public UILabel itemNameLabel;

	public void Pressed() {
		gameObject.SendMessageUpwards("LootCellPressed", this);
	}

	public void AssignItem(Item item) {
		GameObject itemGameObject = item.CreateItemGameObject(this);
		itemSocketSprite.transform.DestroyChildren();
		itemGameObject.transform.parent = itemSocketSprite.transform;
		itemGameObject.transform.localScale = Vector3.one;
		itemGameObject.transform.localPosition = Vector3.zero;
		UISprite itemSprite = itemGameObject.GetComponent<UISprite>();
		itemSprite.depth = itemSocketSprite.depth + 2;
		itemNameLabel.color = ColorDefines.ColorForQuality(item.quality);
		itemNameLabel.text = item.itemName;
	}
}
