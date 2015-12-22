using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapMenu : GenericGameMenu {
	public GameObject mapContainer;
	Zone selectedZone = null;
	QuestManager questManager { get { return QuestManager.questManager; } }
	ZoneManager zoneManager { get { return ZoneManager.zoneManager; } }
	HudManager hudManager { get { return HudManager.hudManager; } }

	public void Pressed() {	Show(); }

	public override void Show() { 
		base.Show(); 
		//hudManager.ShowPersistentNotice("Select a Zone to Scout");
	}

	public void ZonePressed(GameObject zoneButton) {
		ZoneMapItem zoneMapItem = zoneButton.GetComponent<ZoneMapItem>();
		selectedZone = zoneManager.GetZoneByID(zoneMapItem.zoneId);
		hudManager.popup.Show("Scout", "Add a scouting mission for the " + selectedZone.itemName + "s?", this, "AddScoutingQuestConfirmed" );
	}


	public void AddScoutingQuestConfirmed() {
		bool found = false;
		foreach(Quest quest in questManager.availableQuests)
			if(quest.zoneId == selectedZone.id && quest.type == QuestType.Scouting)
				found = true;
		if(found) {
			hudManager.ShowError("There is already a scouting mission for the "  + selectedZone.itemName + "s");
			return;
		}

		Quest q = null;
		foreach(int zoneQuestId in selectedZone.questIds) {
			BaseQuest bq = questManager.GetBaseQuestByID(zoneQuestId);
			if(bq.type == QuestType.Scouting) {
				q = questManager.AddQuestToList(bq);
				break;
			}
		}

		if(q == null) {
			hudManager.ShowError("Error: Scouting quest is missing for this zone");
			return;
		}

		List<Quest> quests = new List<Quest>();
		quests.Add(q);

		hudManager.ShowNotice("Scouting Quest Added");
		if(hudManager.questListMenu.IsDisplayed())
			hudManager.Broadcast("QuestsAdded", quests);
		else
			hudManager.questListMenu.Show();
		Hide();
	}


	public void Dismiss() {
		Hide();
	}

	public override void Hide() {
		//hudManager.HidePersistentNotice();
		base.Hide();
	}
}
