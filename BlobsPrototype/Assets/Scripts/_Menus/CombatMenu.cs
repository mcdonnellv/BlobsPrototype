using UnityEngine;
using System.Collections;

public class CombatMenu : GenericGameMenu {
	public UIGrid gridLog;
	public UIGrid gridCombatants;
	public UILabel labelPrefab;
	public UIButton dismissButton;
	public int maxLabels;

	CombatManager combatManager  { get { return CombatManager.combatManager; } }
	QuestManager questManager { get { return QuestManager.questManager; } }

	public void Pressed() { base.Show(); }
	public void CombatDone() { dismissButton.gameObject.SetActive(true); }


	public void Dismiss() {	
		base.Hide();
		if(combatManager.lastCombatSuccessFul)
			questManager.CollectRewardsForQuest(combatManager.quest);
		else
			questManager.QuestFinishedCleanup(combatManager.quest);
	}


	public override void Show() {
		dismissButton.gameObject.SetActive(false);
		gridLog.transform.DestroyChildren();
		gridCombatants.transform.DestroyChildren();
		base.Show();
	}


	public override void SetDisplayed() {
		base.SetDisplayed();
		combatManager.StartFight();
	}


	public void PushMessage(string text) {
		if(gridLog.transform.childCount >= maxLabels) {
			Transform child = gridLog.transform.GetChild(0);
			GameObject.DestroyImmediate(child.gameObject);
		}

		UILabel label = (UILabel)GameObject.Instantiate(labelPrefab);
		label.transform.parent = gridLog.transform;
		label.transform.localScale = Vector3.one;
		label.gameObject.SetActive(true);
		label.text = text;
		gridLog.Reposition();
	}

	public void AddCombatant(string name) {
		name = name.ToUpper();
		UILabel label = (UILabel)GameObject.Instantiate(labelPrefab);
		label.transform.parent = gridCombatants.transform;
		label.transform.localScale = Vector3.one;
		label.gameObject.SetActive(true);
		label.text = name + ":   100%";
		gridCombatants.Reposition();
	}


	public void UpdateCombatantHealth(string name, int health) {
		name = name.ToUpper();
		UILabel label = null;
		UILabel[] labels = gridCombatants.GetComponentsInChildren<UILabel>();
		foreach(UILabel l in labels)
			if(l.text.Contains(name))
				label = l;
		label.text = name + ":   ";
		if(health <= 0)
			label.text += ColorDefines.ColorToHexString(Color.red) + "dead[-]";
		else
			label.text += health.ToString() + "%";
	}

}
