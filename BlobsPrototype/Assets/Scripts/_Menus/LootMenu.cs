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
		PopulateSlots();
	}

	
	void RebuildSlots() {
		List<LootEntry> lootList = quest.LootTableA.Union(quest.LootTableB).ToList();
		
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
		int slots = 1;
		bool success = true;
		while (success && slots < 5) {
			slots++;
			success = UnityEngine.Random.Range(0f,1f) < .7f;
		}

		slotCount = slots;
	}

	void PopulateSlots() {
		for(int i = 0; i < slotCount; i++) {
			float roll = UnityEngine.Random.Range(0f,1f);
			//TODO
		}
	}


	public void Claim() {
		Hide();
	}
}
