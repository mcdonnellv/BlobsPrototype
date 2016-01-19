using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class QuestPrepMenu : GenericGameMenu {
	public UISprite icon;
	public UILabel durationLabel;
	public UILabel rarityLabel;
	public UIButton departButton;
	public UIGrid blobGrid;
	public UILabel blobInfoLabel;
	public UISprite blobInfoBG;
	public UIWidget requirementsContainer;
	public UILabel bonusLabel;
	public UILabel bonusCountLabel;
	
	[HideInInspector] public Quest quest;
	[HideInInspector] public GenericGameMenu owner = null;
	HudManager hudManager { get { return HudManager.hudManager; } }
	QuestManager questManager { get { return QuestManager.questManager; } }
	RoomManager roomManager  { get { return RoomManager.roomManager; } }

	public void Pressed() {	Show(); }

	public void Show(Quest questParam) {
		gameObject.SetActive(true);
		base.Show(); 
		quest = questParam;
		Populate();
		hudManager.ShowPersistentNotice("Drag Blobs to the quest");
		EnableBlobBoxes();
		PopulateWithBlobs();
		UpdateBonuses();
		hudManager.ShowHud(false); 
		hudManager.dragToUi = true;
		roomManager.ToggleAllFloatingSprites(true);
	}


	void Populate() {
		headerLabel.text = quest.itemName.ToUpper();
		durationLabel.text = GetTimeString();
		icon.spriteName = quest.iconName;
		icon.atlas = quest.iconAtlas;
		PopulateBlobSlots();
	}

	
	void PopulateBlobSlots() {
		foreach(Transform child in blobGrid.transform) {
			bool active = (child.GetSiblingIndex() < quest.blobsRequired);
			child.gameObject.SetActive(active);
			if(!active)
				continue;
			
			BlobQuestSlot blobSlot = child.GetComponent<BlobQuestSlot>();
			blobSlot.fulfilledSprite.gameObject.SetActive(false);
			blobSlot.sigilSprite.alpha = .5f;
			blobSlot.socketSprite.alpha = .5f;
			Element element = quest.elementRequirements[child.GetSiblingIndex()];
			Color colorRequired = ColorDefines.ColorForElement(element);
			blobSlot.colorBgSprite.color = (element == Element.None) ? ColorDefines.defaultBlobSocketColor: colorRequired;
			Sigil sigil = quest.sigilRequirements[child.GetSiblingIndex()];
			blobSlot.sigilSprite.gameObject.SetActive(sigil != Sigil.None);
			if(sigil != Sigil.None)
				blobSlot.sigilSprite.spriteName = GlobalDefines.SpriteNameForSigil(sigil);
		}
		
		ClearBlobs();
		blobGrid.Reposition();
	}


	void EnableBlobBoxes() {
		foreach(Transform child in blobGrid.transform) {
			BlobDragDropItem dragDropItem = child.GetComponentInChildren<BlobDragDropItem>();
			if(dragDropItem != null) {
				dragDropItem.interactable = true;
			}
		}
	}
	

	public void PopulateWithBlobs() {
		foreach(Transform child in blobGrid.transform) {
			int index = child.GetSiblingIndex();
			if(index >= quest.blobsRequired)
				break;
			int blobID = quest.blobIds[index];
			if(blobID == -1)
				continue;
			Blob blob = roomManager.GetBlobByID(quest.blobIds[index]);
			if(blob == null)
				continue;
			BlobQuestSlot blobSlot = child.GetComponent<BlobQuestSlot>();
			blobSlot.socket.transform.DestroyChildren();

			GameObject blobGameObject = blob.gameObject.CreateDuplicateForUi(blobSlot.socket.transform, true);
			blobGameObject.transform.localPosition = new Vector3(0f, -18f, 1f);
			blobGameObject.transform.localScale = new Vector3(.6f, .6f, .6f);

			BlobDragDropItem dragDropItem = blobGameObject.GetComponent<BlobDragDropItem>();
			dragDropItem.uiClone = true;
			dragDropItem.interactable = false;
			blobSlot.socketSprite.alpha = 1f;
			blobSlot.sigilSprite.alpha = .15f;
		}
	}


	public void BlobAddedToContainer(BlobContainerPackage package) {
		Blob blobToAdd = package.blob;
		BlobDragDropContainer blobDragDropContainer = package.container;
		int containerIndex = blobDragDropContainer.transform.parent.GetSiblingIndex();

		//if this blob is also in other containers remove it from those containers
		foreach(Transform child in blobGrid.transform) {
			BlobDragDropContainer bddc = child.GetComponentInChildren<BlobDragDropContainer>();
			if(bddc == blobDragDropContainer || !child.gameObject.activeSelf)
				continue;
			BlobGameObject blobObj = child.GetComponentInChildren<BlobGameObject>();
			if(blobObj != null && blobObj.blob.id == blobToAdd.id) {
				BlobRemovedFromContainer(blobObj.blob.id);
				BlobQuestSlot slot = bddc.GetComponentInParent<BlobQuestSlot>();
				slot.socket.transform.DestroyChildren();
			}
		}
		quest.AddBlob(blobToAdd.id, containerIndex);

		// now check to see if elements and sigils match
		BlobQuestSlot blobSlot = blobDragDropContainer.GetComponentInParent<BlobQuestSlot>();
		bool match = questManager.DoesBlobMatchSlot(quest, blobToAdd, containerIndex);
		blobSlot.fulfilledSprite.gameObject.SetActive(match);
		blobSlot.socketSprite.alpha = 1f;
		blobSlot.sigilSprite.alpha = .15f;
		departButton.isEnabled = questManager.IsPartyFull(quest);
		UpdateBonuses();
	}


	public void BlobRemovedFromContainer(int blobId) {
		int index = quest.blobIds.IndexOf(blobId);
		Transform slot = blobGrid.transform.GetChild(index);
		BlobQuestSlot blobSlot = slot.GetComponentInParent<BlobQuestSlot>();
		blobSlot.fulfilledSprite.gameObject.SetActive(false);
		blobSlot.socketSprite.alpha = .5f;
		blobSlot.sigilSprite.alpha = .5f;
		quest.RemoveBlob(blobId);
		departButton.isEnabled = questManager.IsPartyFull(quest);
		UpdateBonuses();
	}


	public int GetMatchCt() {
		int i=0;
		int matchCt = 0;
		foreach(int blobId in quest.blobIds) {
			Blob blob = roomManager.GetBlobByID(blobId);
			if(questManager.DoesBlobMatchSlot(quest, blob, i))
				matchCt++;
			i++;
		}
		return matchCt;
	}


	void UpdateBonuses() {
		List<QuestBonus> bonusList = quest.GetAppropriateBonusList();
		int bonusTotalCt = bonusList.Count;
		string inactiveColorStr = ColorDefines.ColorToHexString(ColorDefines.inactiveTextColor);
		bonusLabel.alignment = bonusTotalCt > 1 ? NGUIText.Alignment.Left : NGUIText.Alignment.Center;
		if(bonusTotalCt == 0) {
			bonusCountLabel.text = "Bonuses";
			bonusLabel.text = "None";
			return;
		}

		int activeBonusCt = GetActiveBonusCount();
		bonusCountLabel.text = "BONUSES   (" + activeBonusCt.ToString() + "/" + bonusTotalCt.ToString() + ")" ;
		bonusLabel.text = "";
		for(int i=0; i < bonusTotalCt; i++) {
			bool active = (i < activeBonusCt);
			if(active)
				bonusLabel.text += "[FFFFFF]Bonus: " + bonusList[i].description + "[-]";
			else {
				int reqMatches = quest.GetNumMatchesRequiredForBonus(bonusList[i]);
				bonusLabel.text += inactiveColorStr + reqMatches.ToString() + " Match: " + bonusList[i].description + "[-]";
			}
			bonusLabel.text += ((i < (bonusTotalCt - 1)) ? "\n" : "");
		}
	}


	int GetActiveBonusCount() {
		List<QuestBonus> bonusList = quest.GetAppropriateBonusList();
		int matchCt = GetMatchCt();
		int retVal = 0;
		foreach(QuestBonus qb in bonusList) {
			int reqMatch = quest.GetNumMatchesRequiredForBonus(qb);
			if(matchCt >= reqMatch)
				retVal++;
		}
		return retVal;
	}


	List<int> GetChosenBlobs() {
		List<int> blobs = new List<int>();
		foreach(Transform child in blobGrid.transform) {
			BlobGameObject blobObj = child.GetComponentInChildren<BlobGameObject>();
			if(blobObj != null)
				blobs.Add(blobObj.blob.id);
		}
		return blobs;
	}


	public void ClearBlobs() {
		foreach(Transform child in blobGrid.transform) {
			BlobDragDropItem blobDragDropItem = child.GetComponentInChildren<BlobDragDropItem>();
			if(blobDragDropItem != null)
				GameObject.Destroy(blobDragDropItem.gameObject);
		}
	}


	public void Depart() {
		List<int> blobs = GetChosenBlobs();
		if(blobs.Count != quest.blobsRequired) {
			hudManager.popup.Show("Quest", "Add more blobs to start this quest");
			return;
		}
		questManager.StartQuest(quest, GetChosenBlobs());
		Hide();
	}


	string GetTimeString() {
		string timeString = "";
		if(quest.days > 0)
			timeString += quest.days.ToString() + " day";
		if(quest.hrs > 0)
			timeString += quest.hrs.ToString() + " hr";
		if(quest.mins > 0)
			timeString += quest.mins.ToString() + " min";
		return ColorDefines.ColorToHexString(ColorDefines.goldenTextColor) + timeString + "[-]";
	}


	public override void Cleanup() {
		roomManager.ToggleAllFloatingSprites(false);
		hudManager.ShowHud(true);
		hudManager.dragToUi = false;
		hudManager.HidePersistentNotice();
		base.Cleanup();
	}

}
