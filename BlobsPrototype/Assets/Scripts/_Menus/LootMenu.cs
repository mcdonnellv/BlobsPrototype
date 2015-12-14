using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class LootMenu : GenericGameMenu {

	public UIGrid grid;
	public Quest quest;
	private ItemManager _im;
	ItemManager itemManager { get { return ItemManager.itemManager; } }
	QuestManager questManager { get { return QuestManager.questManager; } }
	int defaultWindowHeight = 180;
	int slotCount = 1;
	
	
	public void Show(Quest questParam) {
		quest = questParam;
		base.Show();
		Setup();
	}


	void Setup() {
		slotCount = questManager.GetRewardCount(quest);
		RebuildSlots();
		int slotCountForTableA = slotCount / 2;
		if(slotCount % 2 != 0) //odd number
			if (UnityEngine.Random.Range(0f, 1f) < .5f)
				slotCountForTableA++;

		PopulateSlots(quest.LootTableA, 0, slotCountForTableA);
		PopulateSlots(quest.LootTableB, slotCountForTableA, slotCount);
	}

	
	void RebuildSlots() {
		grid.transform.DestroyChildren();
		for(int i = 0; i < slotCount; i++) {
			GameObject lootCellGameObject = (GameObject)GameObject.Instantiate(Resources.Load("LootCell"));
			LootCell lootCell = lootCellGameObject.GetComponent<LootCell>();
			lootCell.transform.parent = grid.transform;
			lootCell.transform.localScale = new Vector3(1f,1f,1f);
		}
		grid.Reposition();
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

			LootCell lootCell = grid.transform.GetChild(i).GetComponent<LootCell>();
			Item item = new Item(itemManager.GetBaseItemByID(loot.itemId));
			lootCell.AssignItem(item);
		}
	}


	public void LootCellPressed(LootCell lootCell) {
		ItemPointer ip = lootCell.GetComponentInChildren<ItemPointer>();
		itemManager.AddItemToStorage(itemManager.GetBaseItemByID(ip.item.id));
		lootCell.transform.parent = null;
		GameObject.Destroy(lootCell.gameObject);
		if(grid.transform.childCount == 0) {
			HudManager.hudManager.questListMenu.RewardsCollected();
			Hide();
		}
		else
			grid.Reposition();
	}
}
