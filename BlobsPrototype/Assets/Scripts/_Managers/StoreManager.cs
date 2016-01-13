using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class StoreManager : MonoBehaviour {
	private static StoreManager _storeManager;
	public static StoreManager storeManager { get {if(_storeManager == null) _storeManager = GameObject.Find("StoreManager").GetComponent<StoreManager>(); return _storeManager; }}

	public List<BaseGene> baseGenes = new List<BaseGene>();
	public GeneManager geneManager { get {return GeneManager.geneManager;} }
	public ItemManager itemManager { get {return ItemManager.itemManager;} }
	public QuestManager questManager { get {return QuestManager.questManager;} }

	public void BuildStoreItems() {
		baseGenes = new List<BaseGene>();
		foreach(BaseGene b in geneManager.genes) {
			if(b.showInStore == false)
				continue;
		
			if(b.activationRequirements.Count > 0) {
				bool found = false;
				int keyItemId = b.activationRequirements[0].itemId;
				List<BaseQuest> quests = questManager.GetQuestsWithPreReqComplete();
				foreach(BaseQuest bq in quests)
					if(questManager.IsItemIdPartOfQuestLoot(keyItemId, bq))
						found = true;
				if(found)
					baseGenes.Add(b);
			}
			else
				baseGenes.Add(b);
		}
	}
}
