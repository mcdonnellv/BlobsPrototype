using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class HudManager : MonoBehaviour {
	private static HudManager _hudManager;
	public static HudManager hudManager { get {if(_hudManager == null) _hudManager = GameObject.Find("HudManager").GetComponent<HudManager>(); return _hudManager; } }

	public UIAtlas sigilAtlas;
	public UILabel averageQualityLabel;
	public UILabel goldLabel;
	public UILabel chocolateLabel;
	public InventoryMenu inventoryMenu;
	public BlobInfoContextMenu blobInfoContextMenu;
	public RoomOptionsContextMenu roomOptionsContextMenu;
	public CheatMenu cheatMenu;
	public BuildRoomMenu buildRoomMenu;
	public QuestListMenu questListMenu;
	public QuestPrepMenu questPrepMenu;
	public Popup popup;
	public ItemInfoPopup itemInfoPopup;
	public BlobInteractPopup blobInteractPopup;
	public LootMenu lootMenu;
	public GameObject hudRoot;
	public GameObject mainHudObject;
	public NotificationIndicator notificationIndicator;
	public CombatMenu combatMenu;
	public StoreMenu storeMenu;
	public BattleHud battleHud;

	public bool dragToUi = false;
	public GameObject dragObjectHelper;
	public GameObject worldObject;

	public Camera popupCamera;
	static int popupRefCount = 0;
	static int showHudRefCount = 0;

	void EnablePopupCam(bool enable) { popupCamera.enabled = enable; }
	public void UpdateGold(int gold) {goldLabel.text = gold.ToString() + "[gold]";}
	public void UpdateChocolate(int chocolate) {chocolateLabel.text = chocolate.ToString() + "[token]";}
	public void UpdateAverageQuality(string qualityStr) {averageQualityLabel.text = "Average Quality: " + qualityStr;}

	// Use this for initialization
	void Start () {
		ShowOnlyBattleHud();
		return;



		foreach(Transform child in popupCamera.transform.parent) {
			if(child.GetComponentInChildren<GenericGameMenu>()) 
				child.gameObject.SetActive(false);
		}
		popupCamera.enabled = false;
		ShowHud(true);
	}

	void ShowOnlyBattleHud() {
		GameObject battleHud = GameObject.Find("Battle Hud");
		foreach(Transform child in hudRoot.transform)
			child.gameObject.SetActive(false);
		battleHud.SetActive(true);
	}


	public void IncrementPopupRefCount() {
		HudManager.popupRefCount++;
		popupCamera.enabled = popupRefCount > 0;
	}


	public  void DecrementPopupRefCount() {
		HudManager.popupRefCount--;
		popupCamera.enabled = popupRefCount > 0;
	}


	public void ShowHud(bool show) {
		if(show)
			showHudRefCount++;
		else
			showHudRefCount--;
		mainHudObject.SetActive(showHudRefCount > 0);
	}

	public void Broadcast(string functionName, System.Object message) { hudRoot.BroadcastMessage(functionName, message); }
	public void ShowNotice(string text) { notificationIndicator.AddNoticeToQueue(text); }
	public void ShowError(string text) { notificationIndicator.AddErrorToQueue(text); }
	public void ShowPersistentNotice(string text) { notificationIndicator.DisplayPersistentNotice(text); }
	public void HidePersistentNotice() { notificationIndicator.HidePersistentNotice(); }

	// Update is called once per frame
	void Update () {
	}
}

