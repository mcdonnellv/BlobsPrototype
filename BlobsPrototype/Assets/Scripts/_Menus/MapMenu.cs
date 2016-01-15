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
		foreach(Transform zoneGameObjectTransform in mapContainer.transform) {
			ZoneMapItem zoneMapItem = zoneGameObjectTransform.GetComponent<ZoneMapItem>();
			Zone zone = zoneManager.GetZoneByID(zoneMapItem.zoneId);
			if(zone == null)
				continue;
			UIButton zoneButton = zoneGameObjectTransform.GetComponent<UIButton>();
			zoneButton.isEnabled = true;
			if(zone.unlockingQuestId != -1 && questManager.completedQuestIds.ContainsKey(zone.unlockingQuestId) == false)
				zoneButton.isEnabled = false;
		}
	}

	public void ZonePressed(GameObject zoneButton) {
		ZoneMapItem zoneMapItem = zoneButton.GetComponent<ZoneMapItem>();
		selectedZone = zoneManager.GetZoneByID(zoneMapItem.zoneId);
		hudManager.popup.Show("Scout", "Add a scouting mission for the " + selectedZone.itemName + "s?", this, "AddScoutingQuestConfirmed" );
	}


	public void AddScoutingQuestConfirmed() {
		if(questManager.availableQuests.Count >= QuestManager.maxAvailableQuests) {
			hudManager.ShowError("Quest Log Full");
			return;
		}

		bool found = false;
		foreach(Quest quest in questManager.availableQuests)
			if(quest.zoneId == selectedZone.id && quest.type == QuestType.Scouting)
				found = true;
		if(found) {
			hudManager.ShowError("There is already a scouting mission for the "  + selectedZone.itemName + "s");
			return;
		}

		BaseQuest bq = questManager.GetBaseQuestByID(selectedZone.scoutingQuestId);
		Quest q = questManager.AddQuestToList(bq);
		if(q == null) {
			hudManager.ShowError("Error: Scouting quest is missing for this zone");
			return;
		}

		hudManager.ShowNotice("Scouting Quest Added");
		if(hudManager.questListMenu.IsDisplayed()){
			hudManager.Broadcast("QuestsAdded", new List<Quest>{q});
		}
		else {
			hudManager.questDetailsMenu.Show(q);
		}
		Hide();

		//TODO: automatically select quest

	}


	public void Dismiss() {
		Hide();
	}

	public override void Hide() {
		//hudManager.HidePersistentNotice();
		base.Hide();
	}
}
