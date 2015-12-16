using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class QuestDetailsMenu : GenericGameMenu {
	public QuestListMenu questListMenu;
	public UILabel descriptionLabel;
	public UISprite icon;
	public UILabel durationLabel;
	public UILabel rarityLabel;
	public UIButton departButton;
	public UIButton cancelButton;
	public UIGrid blobGrid;
	public PotentialLootMenu potentialLootMenu;
	public UILabel blobInfoLabel;
	public UISprite blobInfoBG;
	public UILabel rewardInfoLabel;
	public UISprite rewardInfoBG;
	public UIWidget requirementsContainer;
	public UIWidget rewardsContainer;


	[HideInInspector] public Quest quest;
	[HideInInspector] public GenericGameMenu owner = null;
	int defaultWindowHeight = 554;//742;
	HudManager hudManager { get { return HudManager.hudManager; } }
	QuestManager questManager { get { return QuestManager.questManager; } }
	RoomManager roomManager  { get { return RoomManager.roomManager; } }
	public void Pressed() {	Show(); }
	public bool IsSelected()  {return (owner == null || !owner.IsDisplayed()); }


	public void ResizeWindow() { 
		int height = defaultWindowHeight;
		requirementsContainer.transform.localPosition = new Vector3(0,-278,0);
		rewardsContainer.transform.localPosition = GetRewardsContainerPosition();
		requirementsContainer.height = 117;
		if(quest.blobsRequired > 3) {
			requirementsContainer.transform.localPosition = new Vector3(0,-260,0);
			requirementsContainer.height = 180;
		}

		if(IsSelected()) 
			height += 110;
		window.GetComponent<UISprite>().height = height;
	}


	public void Show(GenericGameMenu caller, Quest questParam, bool showButtons) {
		gameObject.SetActive(true);
		if(IsDisplayed() && owner == caller && quest != questParam) // We are just changing the displayed info
			FlashChangeAnim();
		else 
			base.Show(); 
		owner = caller;
		quest = questParam;
		ShowButtons(showButtons);
		Setup();
	}
	

	void Setup() {
		headerLabel.text = quest.itemName;
		icon.spriteName = quest.iconName;
		icon.atlas = quest.iconAtlas;
		descriptionLabel.text = quest.description;
		rarityLabel.gameObject.SetActive(quest.quality > Quality.Common);
		string timeString = "";
		if(quest.days > 0)
			timeString += quest.days.ToString() + " day";
		if(quest.hrs > 0)
			timeString += quest.hrs.ToString() + " hr";
		if(quest.mins > 0)
			timeString += quest.mins.ToString() + " min";
		durationLabel.text = ColorDefines.ColorToHexString(ColorDefines.goldenTextColor) + timeString + "[-]";
		int blobsRequired = quest.blobsRequired;


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
		UpdateRewardInfoLabel();
		ResizeWindow();
		potentialLootMenu.RebuildSlots(quest);
	}


	public void SelectQuest() {
		animationWindow.PlayReverse();
		Invoke("ChangePositionToRight", GetAnimationDelay());
		hudManager.ShowHud(false);
		hudManager.dragToUi = true;
	}
	

	public void ChangePositionToRight() {
		hudManager.ShowPersistentNotice("Drag Blobs to the quest");
		transform.parent.parent.BroadcastMessage("GameMenuClosing", this);
		ShowButtons(true);
		ChangePosition(PopupPosition.Right1);
		owner = null;
		ResizeWindow();
		animationWindow.PlayForward();

		foreach(Transform child in blobGrid.transform) {
			BlobDragDropItem dragDropItem = child.GetComponentInChildren<BlobDragDropItem>();
			if(dragDropItem != null) {
				dragDropItem.interactable = true;
			}
		}
	}


	public void ChangePositionToLeft() {
		ChangePosition(defaultStartPosition);
		ResizeWindow();
		animationWindow.PlayForward();
	}


	public void UnSelectQuest() {
		animationWindow.PlayReverse();
		owner.Invoke("Show", GetAnimationDelay()/2);
		hudManager.ShowHud(true);
		hudManager.dragToUi = false;
		hudManager.HidePersistentNotice();
		Hide();
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

			GameObject blobGameObject = blob.CreateDuplicateForUi(blobSlot.socket.transform, true);
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
			if(child.GetComponentInChildren<BlobDragDropContainer>() == blobDragDropContainer || !child.gameObject.activeSelf)
				continue;
			Blob blob = child.GetComponentInChildren<Blob>();
			if(blob != null && blob.id == blobToAdd.id) {
				BlobRemovedFromContainer(blob.id);
				GameObject.Destroy(blob.gameObject);
			}
		}
		quest.AddBlob(blobToAdd.id, containerIndex);

		// now check to see if elements and sigils match
		BlobQuestSlot blobSlot = blobDragDropContainer.GetComponentInParent<BlobQuestSlot>();
		bool match = questManager.DoesBlobMatchSlot(quest, blobToAdd, containerIndex);
		blobSlot.fulfilledSprite.gameObject.SetActive(match);
		blobSlot.socketSprite.alpha = 1f;
		blobSlot.sigilSprite.alpha = .15f;
		UpdateRewardInfoLabel();
		departButton.isEnabled = questManager.IsPartyFull(quest);
	}
	

	public void BlobRemovedFromContainer(int blobId) {
		int index = quest.blobIds.IndexOf(blobId);
		Transform slot = blobGrid.transform.GetChild(index);
		BlobQuestSlot blobSlot = slot.GetComponentInParent<BlobQuestSlot>();
		blobSlot.fulfilledSprite.gameObject.SetActive(false);
		blobSlot.socketSprite.alpha = .5f;
		blobSlot.sigilSprite.alpha = .5f;
		quest.RemoveBlob(blobId);
		UpdateRewardInfoLabel();
		departButton.isEnabled = questManager.IsPartyFull(quest);
	}


	void ShowButtons(bool show) {
		departButton.gameObject.SetActive(show);
		cancelButton.gameObject.SetActive(show);
		if(show) {
			departButton.isEnabled = questManager.IsPartyFull(quest);
			//rewardsContainer.transform.localPosition = new Vector3(0,-462,0);
			blobInfoBG.gameObject.SetActive(true);
		}
		else {
			rewardInfoBG.gameObject.SetActive(false);
			//rewardsContainer.transform.localPosition = new Vector3(0,-443,0);
			blobInfoBG.gameObject.SetActive(false);
		}
	}


	Vector3 GetRewardsContainerPosition() {
		Vector3 pos = new Vector3(0,-462,0);
		if(quest.blobsRequired > 3) 
			pos.y -= 30;
		if(!IsSelected())
			pos.y += 19;
		return pos;
	}

	void UpdateRewardInfoLabel() {
		if(IsSelected() ==  false)
			return;

		bool full = questManager.IsPartyFull(quest);
		if(!full) {
			rewardInfoLabel.text = "";
			rewardsContainer.transform.localPosition = GetRewardsContainerPosition();
		}
		else {
			RewardRange range = questManager.GetRewardRange(quest);
			rewardInfoLabel.text = "YOU WILL GET [00FF00]" + range.min.ToString() +  " TO " + range.max.ToString() + "[-] ITEMS";
			Vector3 pos = GetRewardsContainerPosition();
			pos.y += blobInfoBG.height;
			rewardsContainer.transform.localPosition = pos;
		}

		blobInfoLabel.text = "DRAG BLOBS HERE";
		blobInfoBG.gameObject.SetActive(!full);
		rewardInfoBG.gameObject.SetActive(full);
	}


	List<int> GetChosenBlobs() {
		List<int> blobs = new List<int>();
		foreach(Transform child in blobGrid.transform) {
			Blob blob = child.GetComponentInChildren<Blob>();
			if(blob != null)
				blobs.Add(blob.id);
		}
		return blobs;
	}


	public void ClearBlobs() {
		//quest.RemoveAllBlobs();
		foreach(Transform child in blobGrid.transform) {
			Blob blob = child.GetComponentInChildren<Blob>();
			if(blob != null)
				GameObject.Destroy(blob.gameObject);
		}
	}


	public void Depart() {
		List<int> blobs = GetChosenBlobs();
		if(blobs.Count != quest.blobsRequired) {
			hudManager.popup.Show("Quest", "Add more blobs to start this quest");
			return;
		}
		quest.Start(GetChosenBlobs());
		Hide();
	}


	public void FlashChangeAnim() {
		transform.parent.parent.BroadcastMessage("GameMenuClosing", this);
		base.FlashChangeAnim();
	}


	public override void Cleanup() {
		hudManager.ShowHud(true);
		hudManager.dragToUi = false;
		hudManager.HidePersistentNotice();
		base.Cleanup();
	}

}
