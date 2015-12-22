using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BlobGameObject : MonoBehaviour {

	[HideInInspector] public Blob blob = null;
	public UISprite eyesSprite;
	public UISprite bodySprite;
	public UISprite eggSprite;
	public Dictionary<string, Texture> bodyPartSprites = new Dictionary<string, Texture>();
	public BlobFloatingDisplay floatingDisplay;
	public Color color;

	Animator animator { get { return GetComponentInChildren<Animator>(); } }
	BlobDragDropItem blobDragDropItem { get { return GetComponent<BlobDragDropItem>(); } }

	// managers
	GameManager2 gameManager { get { return GameManager2.gameManager; } }
	BreedManager breedManager { get { return BreedManager.breedManager; } }
	HudManager hudManager { get { return HudManager.hudManager; } }
	GeneManager geneManager { get { return GeneManager.geneManager; } }
	RoomManager roomManager  { get { return RoomManager.roomManager; } }
	BodyPartManager bodyPartManager  { get { return BodyPartManager.bodyPartManager; } }

	// Use this for initialization
	void Start () {}


	public void Setup() {
		blob = new Blob();
		blob.gameObject = this;
		blob.id = gameManager.gameVars.blobsSpawned++;
		blob.quality = Blob.GetRandomQuality();
		blob.gender = (UnityEngine.Random.Range(0, 2) == 0) ? Gender.Male : Gender.Female;
		bodyPartSprites.Add("Body", bodyPartManager.bodyTextures[UnityEngine.Random.Range(0, bodyPartManager.bodyTextures.Count)]);
		bodyPartSprites.Add("Eyes", bodyPartManager.eyeTextures[UnityEngine.Random.Range(0, bodyPartManager.eyeTextures.Count)]);
		bodySprite.spriteName = bodyPartSprites["Body"].name;
		eyesSprite.spriteName = bodyPartSprites["Eyes"].name;
		eggSprite.gameObject.SetActive(true);
		eyesSprite.gameObject.SetActive(false);
		bodySprite.gameObject.SetActive(false);
		color = ColorDefines.defaultBlobColor;
		floatingDisplay.blob = blob;
	}
	

	public void BlobPressed() {
		if(!blob.hasHatched && blob.state == BlobState.HatchReady) 
			blob.Hatch();
		else
			DisplayBlobInfo();
	}


	public void DisplayBlobInfo() { 
		if(blob.hasHatched == false)
			return;
		hudManager.blobInfoContextMenu.Show(blob.id); 
	}


	public void SetColorFromElement(Element e) {
		if(ColorDefines.elementColorTables.Count == 0)
			ColorDefines.BuildColorDefines();
		Dictionary<string,string> elementColors = ColorDefines.elementColorTables[e];
		int randIndex = UnityEngine.Random.Range(0, elementColors.Keys.Count);
		string key = elementColors.Keys.ElementAt(randIndex);
		string colorHexStr = elementColors[key];
		ChangeColor(key, ColorDefines.HexStringToColor(colorHexStr));
	}


	public void ChangeColor(string colorStr, Color c) {
		TweenColor tc = bodySprite.gameObject.GetComponent<TweenColor>();
		if (tc == null)
			tc = bodySprite.gameObject.AddComponent<TweenColor>();
		tc.duration = 1f;
		tc.from = color;
		tc.to = c;
		tc.PlayForward();
		color = c;
	}


	public void Hatch() {
		eggSprite.gameObject.SetActive(false);
		bodySprite.gameObject.SetActive(true);
		eyesSprite.gameObject.SetActive(true);
		transform.localScale = new Vector3(0.5f, 0.5f, 1f);
		floatingDisplay.HideHarvestSprite();
	}


	public void DepartForQuest(Quest quest) {
		floatingDisplay.stateLabel.gameObject.SetActive(true);
		floatingDisplay.stateLabel.text = "AWAY";
		animator.SetBool("away", true);
		blobDragDropItem.interactable = false;

		blob.missionId = quest.id;
		StartActionWithDuration(BlobState.Questing, quest.GetActionReadyDuration());
	}


	public void ReturnFromQuest() {
		blob.state = BlobState.Idle;
		blob.missionCount++;
		blob.missionId = -1;
		floatingDisplay.stateLabel.gameObject.SetActive(false);
		animator.SetBool("away", false);
		blobDragDropItem.interactable = true;
		UpdateGrowth();
	}


	public void UpdateGrowth() {
		if(blob.isAdult)
			transform.localScale = new Vector3(1f, 1f, 1f);
	}


	public void UpdateBlobInfoIfDisplayed() {
		if(hudManager.blobInfoContextMenu.IsDisplayed() && hudManager.blobInfoContextMenu.DisplayedBlob().id == blob.id)
			hudManager.blobInfoContextMenu.Show(blob.id);
	}


	public GameObject CreateDuplicateForUi(Transform newParent, bool canInteract) {
		GameObject newBlobGameObject = (GameObject)GameObject.Instantiate(this.gameObject);
		newBlobGameObject.transform.parent = newParent;
		newBlobGameObject.transform.localPosition = new Vector3(0f, -18f, 0f);
		newBlobGameObject.transform.localScale = transform.localScale;
		BlobFloatingDisplay floatingDisplay = newBlobGameObject.GetComponentInChildren<BlobFloatingDisplay>();
		GameObject.Destroy(floatingDisplay.gameObject);
		
		if(canInteract == false) {
			GameObject.Destroy(newBlobGameObject.GetComponentInChildren<BoxCollider>());
			GameObject.Destroy(newBlobGameObject.GetComponentInChildren<BlobDragDropItem>());
			GameObject.Destroy(newBlobGameObject.GetComponentInChildren<UIButton>());
		}
		return newBlobGameObject;
	}


	public void StartActionWithDuration(BlobState state, TimeSpan ts) {
		blob.state = state;
		blob.actionDuration = ts;
		blob.actionReadyTime = System.DateTime.Now + blob.actionDuration;
		UpdateBlobInfoIfDisplayed();
	}


	// Update is called once per frame
	void Update() {
		if(blob.actionDuration.TotalSeconds > 0 && blob.actionReadyTime <= System.DateTime.Now)
			blob.ActionDone();
		
		if(blob.room != null && blob.room.type == Room.RoomType.Workshop && blob.state == BlobState.Idle) {
			StartActionWithDuration(BlobState.Working, new TimeSpan(blob.workingDelay.Ticks));
		}
	}


}
