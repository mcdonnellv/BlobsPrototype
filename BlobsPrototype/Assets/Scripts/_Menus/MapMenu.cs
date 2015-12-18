using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapMenu : GenericGameMenu {
	public UIButton zoneButton1;
	public UIButton zoneButton2;
	public UIButton zoneButton3;
	public UIButton zoneButton4;
	public UIButton zoneButton5;
	public UIButton zoneButton6;
	MapZone selectedZone;
	QuestManager questManager { get { return QuestManager.questManager; } }
	HudManager hudManager { get { return HudManager.hudManager; } }

	public void Pressed() {	Show(); }

	public void Show() { 
		base.Show(); 
		//hudManager.ShowPersistentNotice("Select a Zone to Scout");

		zoneButton1.isEnabled = true;
		zoneButton2.isEnabled = true;
		zoneButton3.isEnabled = true;
		zoneButton4.isEnabled = true;
		zoneButton5.isEnabled = true;
		zoneButton6.isEnabled = true;
	}

	public void ZonePressed(GameObject zoneButton) {
		hudManager.popup.Show("Scout", "Add a scouting mission for this zone?", this, "AddScoutingQuestConfirmed" );
		selectedZone = MapZone.Meadows;
	}


	public void AddScoutingQuestConfirmed() {
		bool found = false;
		foreach(Quest quest in questManager.availableQuests)
			if(quest.id == 5)
				found = true;
		if(found) {
			hudManager.popup.Show("Scout", "There is alrady a scouting mission available for that zone");
			return;
		}

		Quest q = questManager.AddQuestToList(questManager.GetBaseQuestByID(5));
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

	public void Hide() {
		//hudManager.HidePersistentNotice();
		base.Hide();
	}
}
