using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PotentialLootMenu : GenericGameMenu {
	public UIGrid grid;
	public Quest quest;
	private ItemManager _im;
	ItemManager itemManager { get{ if(_im == null) _im = GameObject.Find("ItemManager").GetComponent<ItemManager>(); return _im; } }
	int defaultWindowHeight = 228;


	public void Show(Quest questParam) {
		quest = questParam;
		base.Show();
		Setup();
	}


	void Setup() {
		RebuildSlots();
		ResizeWindow();
	}


	void RebuildSlots() {
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

		grid.transform.DestroyChildren();
		foreach (LootEntry loot in lootList) {
			GameObject slot = (GameObject)GameObject.Instantiate(Resources.Load("Possible Reward Slot"));
			slot.transform.parent = grid.transform;
			slot.transform.localScale = new Vector3(1f,1f,1f);
			UISprite sprite = slot.GetComponentInChildren<UISprite>();
			sprite.depth = 1;
			SetupItemInSocket(loot, slot);
		}
		grid.Reposition();
	}


	void SetupItemInSocket(LootEntry loot, GameObject parentSocket){
		Item item = new Item(itemManager.GetBaseItemByID(loot.itemId));
		GameObject go = item.CreateItemGameObject(this);
		go.transform.parent = parentSocket.transform;
		go.transform.localScale = Vector3.one;
		go.transform.localPosition = Vector3.zero;
		UISprite itemSprite = go.GetComponent<UISprite>();
		UISprite socketSprite = parentSocket.GetComponentInChildren<UISprite>();
		itemSprite.depth = socketSprite.depth + 2;
		socketSprite.color = ColorDefines.ColorForQuality(item.quality);
	}

	public void ItemPressed(ItemPointer itemPointer) {
		HudManager hudManager = HudManager.hudManager;
		ItemInfoPopup itemInfoPopup = hudManager.itemInfoPopup;
		if(itemPointer == null) 
			return;
		itemInfoPopup.defaultStartPosition = PopupPosition.Right2;
		itemInfoPopup.Show(this, itemPointer.item);
	}


	public void ResizeWindow() {
		int height = defaultWindowHeight;
		int slotCount = grid.transform.childCount;
		int additionalRows = slotCount / 4;
		height += 60 * additionalRows;
		window.GetComponent<UISprite>().height = height;
	}
}
