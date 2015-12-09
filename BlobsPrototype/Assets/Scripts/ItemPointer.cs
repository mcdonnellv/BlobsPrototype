using UnityEngine;
using System.Collections;

public class ItemPointer : MonoBehaviour {
	public Item item;
	public MonoBehaviour owningMenu = null;
	
	public void ItemPressed() {
		if(owningMenu != null)
			owningMenu.SendMessage("ItemPressed", this);
	}
}
