using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class Zone : BaseThing {
	public int unlockingQuestId = -1;
	public List<int> questIds = new List<int>();
	public int scoutingQuestId { 
		get { List<int> ret = GetQuestIdsWithType(QuestType.Scouting); return (ret.Count == 0) ? -1 : ret[0]; }
		set { RemoveQuestsWithType(QuestType.Scouting); questIds.Add(value); }
	}

	public List<int> GetQuestIdsWithType(QuestType t) {
		List<int> ret = new List<int>();
		foreach (int i in questIds) {
			BaseQuest bq = QuestManager.questManager.GetBaseQuestByID(i);
			if(bq == null) continue;
			if(bq.type == t) ret.Add(i);
		}
		return ret;
	}

	public List<BaseQuest> QuestsForZone() {
		List<BaseQuest> baseQuests = new List<BaseQuest>();
		foreach (int i in questIds) {
			BaseQuest bq = QuestManager.questManager.GetBaseQuestByID(i);
			if(bq == null) continue;
			if(bq.type != QuestType.Scouting)
				baseQuests.Add(bq);
		}
		return baseQuests;
	}

	public void RemoveQuestsWithType(QuestType t) {
		List <int> toDelete = new List<int>();
		foreach (int i in questIds) {
			BaseQuest bq = QuestManager.questManager.GetBaseQuestByID(i);
			if(bq == null) continue;
			if(bq.type == t) toDelete.Add(i);
		}
		foreach(int d in toDelete)
			questIds.RemoveAt(d);
	}

	public bool IsUnlocked() {
		if(unlockingQuestId == -1)
			return true;
		if(QuestManager.questManager.completedQuestIds.ContainsKey(unlockingQuestId))
			return true;
		return false;
	}
}
