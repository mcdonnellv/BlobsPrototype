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
	int slotCount = 1;
	
	
	public void Show(Quest questParam) {
		quest = questParam;
		base.Show();
		Setup();
	}


	void Setup() {
		RewardRange range = questManager.GetRewardRange(quest);
		slotCount = UnityEngine.Random.Range(range.min, range.max + 1);
		RebuildSlots();
		if(quest.LootTableB.Count == 0) {
			PopulateSlots(quest.LootTableA, 0, slotCount);
		}
		else {
			int slotCountForTableA = slotCount / 2;
			if(slotCount % 2 != 0) //odd number
				if (UnityEngine.Random.Range(0f, 1f) < .5f)
					slotCountForTableA++;
			PopulateSlots(quest.LootTableA, 0, slotCountForTableA);
			PopulateSlots(quest.LootTableB, slotCountForTableA, slotCount);
		}
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
		List<Item> rewardEntries = new List<Item>();
		int maxRange = 0;
		foreach(LootEntry l in lootList)
			maxRange += l.probability; // commonly 100

		for(int i = startingSlotIndex; i < totalSlots; i++) {
			LootEntry loot = null;
			int roll = UnityEngine.Random.Range(0, maxRange);
			int cumProbability = 0;
			foreach(LootEntry lootEntry in lootList) {
				int probabilityComp = cumProbability + lootEntry.probability;
				if(roll < probabilityComp) {
					loot = lootEntry;
					break;
				}
				cumProbability += lootEntry.probability;
			}

			if(loot != null)
				rewardEntries.Add(new Item(itemManager.GetBaseItemByID(loot.itemId)));
		}

		rewardEntries = rewardEntries.OrderBy(x => x.itemName).ThenBy(x => x.quality).ToList();
		foreach(Item item in rewardEntries) {
			int index = startingSlotIndex + rewardEntries.IndexOf(item);
			LootCell lootCell = grid.transform.GetChild(index).GetComponent<LootCell>();
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
