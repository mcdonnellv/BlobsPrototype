using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GeneStoreItem {
	public int baseGeneId = -1;
	public DateTime actionReadyTime;
	public TimeSpan actionDuration = new TimeSpan(0);
	public bool researched = false;
	public bool researching = false;
	public bool alreadySeen = false;
	public int researchCost = 1000;
}

public class StoreManager : MonoBehaviour {
	private static StoreManager _storeManager;
	public static StoreManager storeManager { get {if(_storeManager == null) _storeManager = GameObject.Find("StoreManager").GetComponent<StoreManager>(); return _storeManager; }}
	
	public List<GeneStoreItem> baseGenes = new List<GeneStoreItem>();
	public GeneManager geneManager { get {return GeneManager.geneManager;} }
	public ItemManager itemManager { get {return ItemManager.itemManager;} }
	public QuestManager questManager { get {return QuestManager.questManager;} }
	public HudManager hudManager { get {return HudManager.hudManager;} }

	public static TimeSpan reseachTime = new TimeSpan(0,0,5);

	public void BuildStoreItems() {
		foreach(BaseGene b in geneManager.genes) {
			if(b.showInStore == false)
				continue;
			bool alreadyThere = false;
			foreach(GeneStoreItem g in baseGenes)
				if(b.id == g.baseGeneId) 
					alreadyThere = true;
			if(alreadyThere)
				continue;

			GeneStoreItem gti = new GeneStoreItem();
			gti.baseGeneId = b.id;
			if(b.activationRequirements.Count > 0) {
				bool found = false;
				int keyItemId = b.activationRequirements[0].itemId;
				List<BaseQuest> quests = questManager.GetQuestsWithPreReqComplete();
				foreach(BaseQuest bq in quests)
					if(questManager.IsItemIdPartOfQuestLoot(keyItemId, bq))
						found = true;
				if(found)
					baseGenes.Add(gti);
			}
			else
				baseGenes.Add(gti);
		}

		baseGenes = baseGenes.OrderBy(x => x.baseGeneId).ToList();
	}


	public GeneStoreItem GetGeneStoreItemWithId(int id) {
		foreach(GeneStoreItem gsi in baseGenes)
			if(gsi.baseGeneId == id)
				return gsi;
		return null;
	}
	

	void Update() {
		foreach(GeneStoreItem g in baseGenes) {
			if(g.researched == false && g.actionDuration.TotalSeconds > 0 && g.actionReadyTime <= System.DateTime.Now) {
				g.researched = true;
				g.researching = false;
				hudManager.storeMenu.UpdateGeneStoreItem(g);
				hudManager.ShowNotice("Research complete");

			}
		}
	}
}
