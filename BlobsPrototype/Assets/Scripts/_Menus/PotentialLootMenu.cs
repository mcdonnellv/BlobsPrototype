using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PotentialLootMenu : UIGrid {
	private ItemManager _im;
	ItemManager itemManager { get{ if(_im == null) _im = GameObject.Find("ItemManager").GetComponent<ItemManager>(); return _im; } }


	public void RebuildSlots(Quest quest) {
		List<LootEntry> lootList = quest.LootTableA.Union(quest.LootTableB).ToList();

		// prune dupes for simple display
		for(int i = 0; i < lootList.Count; i++) {
			LootEntry lootEntryBase = lootList[i];
			for(int j = i + 1; j < lootList.Count; j++) {
				LootEntry lootEntryComp = lootList[j];
				if(lootEntryBase.itemId == lootEntryComp.itemId)
					lootList.RemoveAt(j);
			}
		}

		transform.DestroyChildren();
		foreach (LootEntry loot in lootList) {
			GameObject slot = (GameObject)GameObject.Instantiate(Resources.Load("Possible Reward Slot"));
			slot.transform.parent = transform;
			slot.transform.localScale = new Vector3(1f,1f,1f);
			UISprite sprite = slot.GetComponentInChildren<UISprite>();
			sprite.depth = 1;
			SetupItemInSocket(loot, slot);
		}
		Reposition();
	}


	void SetupItemInSocket(LootEntry loot, GameObject parentSocket){
		Item item = new Item(itemManager.GetBaseItemByID(loot.itemId));
		GameObject go = item.CreateItemGameObject(this);
		go.transform.parent = parentSocket.transform;
		go.transform.localScale = Vector3.one;
		go.transform.localPosition = Vector3.zero;
		UISprite itemSprite = go.GetComponent<UISprite>();
		UISprite[] socketsprites = parentSocket.GetComponentsInChildren<UISprite>();
		itemSprite.depth = socketsprites[0].depth + 2;
		socketsprites[1].color = ColorDefines.ColorForQuality(item.quality);
	}


	public void ItemPressed(ItemPointer itemPointer) {
		HudManager hudManager = HudManager.hudManager;
		ItemInfoPopup itemInfoPopup = hudManager.itemInfoPopup;
		if(itemPointer == null) 
			return;
		QuestDetailsMenu questMenu = gameObject.GetComponentInParent<QuestDetailsMenu>();
		itemInfoPopup.defaultStartPosition = questMenu.IsSelected() ? PopupPosition.Popup2 : PopupPosition.Popup1;
		itemInfoPopup.Show(questMenu, itemPointer.item);
	}
}
