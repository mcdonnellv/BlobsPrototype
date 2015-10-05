using UnityEngine;
using System.Collections;

public class ItemPointer : MonoBehaviour {
	public Item item;
	
	public void ItemPressed() {
		HudManager hudManager = GameObject.Find("HudManager").GetComponent<HudManager>();
		hudManager.inventoryMenu.itemsMenu.ShowInfoForItemGameObject(this);
	}
}
