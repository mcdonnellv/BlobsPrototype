using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class LootMenu : GenericGameMenu {

	public UIGrid grid;
	public Quest quest;
	private ItemManager _im;
	ItemManager itemManager { get{ if(_im == null) _im = GameObject.Find("ItemManager").GetComponent<ItemManager>(); return _im; } }
	int defaultWindowHeight = 180;
	int slotCount = 1;
	
	
	public void Show(Quest questParam) {
		quest = questParam;
		base.Show();
		Setup();
	}


	void Setup() {
		RollForSlotCount();
		RebuildSlots();
		int slotCountForTableA = slotCount / 2;
		if(slotCount % 2 != 0) //odd number
			if (UnityEngine.Random.Range(0f, 1f) < .5f)
				slotCountForTableA++;

		PopulateSlots(quest.LootTableA, 0, slotCountForTableA);
		PopulateSlots(quest.LootTableB, slotCountForTableA, slotCount);
		//ConsolidateSlots();
	}

	
	void RebuildSlots() {
		grid.transform.DestroyChildren();
		for(int i = 0; i < slotCount; i++) {
			GameObject slot = (GameObject)GameObject.Instantiate(Resources.Load("Possible Reward Slot"));
			slot.transform.parent = grid.transform;
			slot.transform.localScale = new Vector3(1f,1f,1f);
			UISprite sprite = slot.GetComponentInChildren<UISprite>();
			sprite.depth = 1;
		}
		grid.Reposition();
	}


	void RollForSlotCount() {
		bool highYield = quest.IsHighYield();
		int minSlots = highYield ? 2 : 1;
		int maxSlots = highYield ? 5 : 3;
		bool success = true;
		slotCount = minSlots;
		while (success && slotCount < maxSlots) {
			slotCount++;
			success = UnityEngine.Random.Range(0, 100) < 70;
		}
	}


	void PopulateSlots(List<LootEntry> lootList, int startingSlotIndex, int totalSlots) {
		lootList = lootList.OrderBy(x => x.probability).ToList();
		for(int i = startingSlotIndex; i < totalSlots; i++) {
			LootEntry loot = null;
			int roll = UnityEngine.Random.Range(0, 100);
			int cumProbability = 0;
			foreach(LootEntry lootEntry in lootList) {
				int probabilityComp = cumProbability + lootEntry.probability;
				if(roll < probabilityComp) {
					loot = lootEntry;
					break;
				}
				cumProbability += lootEntry.probability;
			}

			GameObject parentSocket = grid.transform.GetChild(i).gameObject;
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
	}


	void ConsolidateSlots() {
		for(int i = 0; i < slotCount; i++) {
			GameObject parentSocket = grid.transform.GetChild(i).gameObject;
			Item item = parentSocket.GetComponentInChildren<ItemPointer>().item;
			for(int j = i + 1; j < slotCount; j++) {
				parentSocket = grid.transform.GetChild(j).gameObject;
				Item itemComp = parentSocket.GetComponentInChildren<ItemPointer>().item;
				if(item.id == itemComp.id) {
					item.count += itemComp.count;
					GameObject.Destroy(parentSocket);
					slotCount--;
				}
			}
		}
		grid.Reposition();
	}


	public void ItemPressed(ItemPointer itemPointer) {
		HudManager hudManager = HudManager.hudManager;
		ItemInfoPopup itemInfoPopup = hudManager.itemInfoPopup;
		if(itemPointer == null) 
			return;
		itemInfoPopup.defaultStartPosition = PopupPosition.Right2;
		itemInfoPopup.Show(this, itemPointer.item);
	}



	public void Claim() {
		Hide();
		foreach(Transform child in grid.transform) {
			ItemPointer ip = child.GetComponentInChildren<ItemPointer>();
			itemManager.AddItemToStorage(itemManager.GetBaseItemByID(ip.item.id));
		}
	}
}
