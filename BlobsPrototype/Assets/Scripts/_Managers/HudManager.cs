using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HudManager : MonoBehaviour {

	public UILabel averageQualityLabel;
	public UILabel goldLabel;
	public UILabel chocolateLabel;
	public InventoryMenu inventoryMenu;
	public BlobInfoContextMenu blobInfoContextMenu;
	public RoomOptionsContextMenu roomOptionsContextMenu;
	public CheatMenu cheatMenu;
	public BuildRoomMenu buildRoomMenu;
	public Popup popup;
	public ItemInfoPopup itemInfoPopup;
	public BlobInteractPopup blobInteractPopup;
	public bool dragToUi = false;
	public GameObject dragObjectHelper;

	public Camera popupCameraParam;
	static Camera popupCamera;
	static int popupRefCount = 0;

	void EnablePopupCam(bool enable) { popupCamera.enabled = enable; }
	public void UpdateGold(int gold) {goldLabel.text = gold.ToString() + "[gold]";}
	public void UpdateChocolate(int chocolate) {chocolateLabel.text = chocolate.ToString() + "[token]";}
	public void UpdateAverageQuality(string qualityStr) {averageQualityLabel.text = "Average Quality: " + qualityStr;}

	// Use this for initialization
	void Start () {
		HudManager.popupCamera = popupCameraParam;
		foreach(Transform child in popupCamera.transform.parent) {
			if(child.GetComponent<GenericGameMenu>()) 
				child.gameObject.SetActive(false);
		}
		popupCamera.enabled = false;
	}

	public static void IncrementPopupRefCount() {
		popupRefCount++;
		popupCamera.enabled = popupRefCount > 0;
	}

	public static  void DecrementPopupRefCount() {
		popupRefCount--;
		popupCamera.enabled = popupRefCount > 0;
	}


	// Update is called once per frame
	void Update () {
	}
}

