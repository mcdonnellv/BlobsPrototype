using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


[Serializable]
public class Blob : MonoBehaviour {

	// internal
	public BlobState state = BlobState.Idle;
	public int id;
	public int spouseId;
	public int missionId;
	public int missionCount;
	public int tilePosX;
	public int tilePosY;
	public Gender gender;
	public Quality quality;
	public DateTime birthday;
	public List<Gene> genes;
	public CombatStats combatStats;
	public Element nativeElement;
	public Sigil sigil;
	public Element element { get { return combatStats.element; } }
	public Dictionary<string, int> itemsConsumed;
	public Room room;
	public DateTime actionReadyTime;
	public TimeSpan actionDuration;
	public Dictionary<string,string> blobColor;

	// visual
	public Color color;
	public Dictionary<string, Texture> bodyPartSprites;
	public GameObject blobSpriteContainer;
	
	// helpers
	public bool male { get {return gender == Gender.Male;} }
	public bool female { get {return gender == Gender.Female;} }
	public bool isInfant { get {return (!isAdult && hasHatched);} }
	public bool isAdult { get {return missionCount >= 3;} }
	public bool hasHatched { get {return birthday != DateTime.MinValue;} }
	public bool canBreed { get {return isAdult && state == BlobState.Idle;} }
	public bool canMerge { get {return hasHatched && state == BlobState.Idle;} }
	public TimeSpan age {get {return DateTime.Now - birthday;} }
	public int allowedGeneCount { get {return GetGeneCountFromQuality(quality);} }
	public TimeSpan blobHatchDelay {get {return TimeSpan.FromTicks(blobHatchDelayStandard.Ticks * (1L + (long)quality + (long)genes.Count));} }
	public TimeSpan breedReadyDelay {get {return TimeSpan.FromTicks(breedReadyStandard.Ticks);} }
	public TimeSpan workingDelay {get {return TimeSpan.FromTicks(workingDelayStandard.Ticks);} }
	Animator animator { get { return GetComponentInChildren<Animator>(); } }
	BlobDragDropItem blobDragDropItem { get { return GetComponent<BlobDragDropItem>(); } }
	
	// managers
	GameManager2 gameManager;
	BreedManager breedManager;
	public BlobFloatingDisplay floatingDisplay;
	HudManager hudManager { get { return HudManager.hudManager; } }
	GeneManager geneManager;
	RoomManager roomManager  { get { return RoomManager.roomManager; } }
	
	// time delays
	TimeSpan blobHatchDelayStandard = new TimeSpan(0,0,1);
	TimeSpan workingDelayStandard = new TimeSpan(0,0,10);
	TimeSpan breedReadyStandard = new TimeSpan(0,0,1);

	

	public void DisplayBlobInfo() { 
		if(hasHatched == false)
			return;
		hudManager.blobInfoContextMenu.Show(id); 
	}
	public string GetBlobStateString() { return state.ToString(); }

	public Blob () {
		combatStats = new CombatStats();
		genes = new List<Gene>();
		itemsConsumed = new Dictionary<string, int>();
		quality = Quality.Common;
		birthday = DateTime.MinValue;
		actionReadyTime  = new DateTime(0); 
		actionDuration = new TimeSpan(0);
		bodyPartSprites = new Dictionary<string, Texture>();
		spouseId = -1;
		color = ColorDefines.defaultBlobColor;
		tilePosX = 0;
		tilePosY = 0;
		missionCount = 0;
	}


	public void Setup() {
		gameManager = GameObject.Find("GameManager2").GetComponent<GameManager2>();
		breedManager = GameObject.Find("BreedManager").GetComponent<BreedManager>();
		geneManager = GameObject.Find("GeneManager").GetComponent<GeneManager>();
		id = gameManager.gameVars.blobsSpawned++;
		SetBodyTexture();
		SetEyeTexture();

		blobSpriteContainer = gameObject.GetComponentInChildren<UIWidget>().gameObject;
		List<UISprite> blobsprites = blobSpriteContainer.GetComponentsInChildren<UISprite>(true).ToList();
		Texture tex = bodyPartSprites["Body"];
		blobsprites[1].spriteName = tex.name;
		tex = bodyPartSprites["Eyes"];
		blobsprites[2].spriteName = tex.name;
		blobsprites[0].gameObject.SetActive(true);
		blobsprites[1].gameObject.SetActive(false);
		blobsprites[2].gameObject.SetActive(false);
		UIButton button = gameObject.GetComponent<UIButton>();
		button.onClick.Add(new EventDelegate(this, "BlobPressed"));
		floatingDisplay = gameObject.GetComponentInChildren<BlobFloatingDisplay>();
		floatingDisplay.blob = this;
		combatStats.element = nativeElement;
		SetColorFromElement(nativeElement);
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

	public void UpdateBlobInfoIfDisplayed() {
		if(hudManager.blobInfoContextMenu.IsDisplayed() && hudManager.blobInfoContextMenu.DisplayedBlob() == this)
			hudManager.blobInfoContextMenu.Show(id);
	}

	
	public Blob GetSpouse() {
		List<Blob> blobs = room.blobs;
		foreach(Blob b in blobs)
			if(spouseId == b.id)
				return b;
		
		return null;
	}


	void BlobPressed() {
		if(!hasHatched && state == BlobState.HatchReady) {
			Hatch(true);
			if(hudManager.blobInfoContextMenu.IsDisplayed())
				hudManager.blobInfoContextMenu.Hide();
			return;
		}

		if(state == BlobState.WorkingReady) {
			CollectWork();
			return;
		}

		DisplayBlobInfo();
	}


	public void Hatch(bool displayInfo) {
		if(hasHatched)
			return;
		List<UISprite> blobsprites = blobSpriteContainer.GetComponentsInChildren<UISprite>(true).ToList();
		blobsprites[0].gameObject.SetActive(false);
		blobsprites[1].gameObject.SetActive(true);
		blobsprites[2].gameObject.SetActive(true);
		blobSpriteContainer.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
		birthday = DateTime.Now;
		state = BlobState.Idle;

		floatingDisplay.HideHarvestSprite();
		if(displayInfo)
			DisplayBlobInfo();
	}

	
	public void OrderGenes() {
		genes = genes.OrderByDescending( x => x.quality).ThenBy(x => x.itemName).ToList();
	}
	
	
	public void SetRandomTextures() {
		SetBodyTexture();
		SetEyeTexture();
	}
	
	
	public void SetBodyTexture() {
		BodyPartManager bpm = (BodyPartManager)(GameObject.Find("BodyPartDatabase")).GetComponent<BodyPartManager>();
		bodyPartSprites.Add("Body", bpm.bodyTextures[UnityEngine.Random.Range(0, bpm.bodyTextures.Count)]);
	}

	
	public void SetEyeTexture() {
		BodyPartManager bpm = (BodyPartManager)(GameObject.Find("BodyPartDatabase")).GetComponent<BodyPartManager>();
		bodyPartSprites.Add("Eyes", bpm.eyeTextures[UnityEngine.Random.Range(0, bpm.eyeTextures.Count)]);
	}


	public void ChangeColor(string colorStr, Color c) {
		UISprite body = GetBodySprite();
		TweenColor tc = body.gameObject.GetComponent<TweenColor>();
		if (tc == null)
			tc = body.gameObject.AddComponent<TweenColor>();
		tc.duration = 1f;
		tc.from = color;
		tc.to = c;
		tc.PlayForward();
		color = c;
	}


	public UISprite GetBodySprite() {
		List<UISprite> blobsprites = gameObject.GetComponentsInChildren<UISprite>(true).ToList();
		return blobsprites[1];
	}
	
	
	static public int GetGeneCountFromQuality(Quality q) {
		switch (q) {
		case Quality.Common: return 2;
		case Quality.Rare: return 3;
		case Quality.Epic: return 4;
		case Quality.Legendary: return 5;
		}
		return 2;
	}
	
	
	static public Quality GetRandomQuality()	{	
		Quality q = Quality.Common;
		float r = UnityEngine.Random.Range(0f,1f);
		if(r <= 0.02140f)
			q = Quality.Rare;
		
		if(r <= 0.00428f)
			q = Quality.Epic;
		
		if(r <= 0.00108f)
			q = Quality.Legendary;
		
		return q;
	}


	static public bool ShouldDisplayBarForState(BlobState blobState) {
		switch(blobState) {
		case BlobState.Breeding:
		case BlobState.Hatching: 
		case BlobState.Working: 	return true;
		}
		return false;
	}


	static public bool ShouldDisplayHarvestSpriteForState(BlobState blobState) {
		switch(blobState) {
		case BlobState.WorkingReady:
		case BlobState.HatchReady: return true;
		}
		return false;
	}


	public void AddRandomGene(Quality q) {
		BaseGene g = geneManager.genes[UnityEngine.Random.Range(0, geneManager.genes.Count)];
		genes.Add(new Gene(g));
	}


	public string GetActionString() {
		switch(state) {
		case BlobState.Breeding: return "Breeding";
		case BlobState.Hatching: return "Hatching";
		case BlobState.HatchReady: return "Hatch";
		case BlobState.Working: return "Working";
		case BlobState.OnQuest: return "AWAY";
		}
		
		return "Breed";
	}


	void CollectWork() {
		gameManager.AddGold(1);
		floatingDisplay.HideHarvestSprite();
		state = BlobState.Idle;
	}


	public void StartActionWithDuration(TimeSpan ts) {
		actionDuration = ts;
		actionReadyTime = System.DateTime.Now + actionDuration;
		UpdateBlobInfoIfDisplayed();
	}


	public void ActionDone() {
		actionDuration = new TimeSpan(0);
		switch (state) {
		case BlobState.Breeding: BreedingDone(); break;
		case BlobState.Hatching: HatchingDone(); break;
		case BlobState.Working: WorkingDone(); break;
		case BlobState.OnQuest: QuestDone(); break;
		}
		UpdateBlobInfoIfDisplayed();
	}
	

	public void WorkingDone() {
		state = BlobState.WorkingReady;
	}

	
	public void BreedingDone() {
		if(female)
			breedManager.BreedBlobs(GetSpouse(), this);
		state = BlobState.Idle;
	}


	public void HatchingDone() {
		state = BlobState.HatchReady;
	}

	public void QuestDone() {
		ReturnFromQuest();
		state = BlobState.Idle;
	}

	public void PrepareForDelete() {
		Blob spouse = GetSpouse();
		if(spouse) {
			spouse.spouseId = -1;
			spouse.ChangeColor("Default", ColorDefines.defaultBlobColor);
		}
	}


	public void CalculateStats() {
		Element preCalcElement = (element == Element.None) ? nativeElement : element;
		combatStats.SetDefaultValues();

		foreach(Gene g in genes)
			if(g.active && g.modifier == AbilityModifier.NA)
				combatStats.CalculateOtherStats(g.traitType, g.value);

		foreach(Gene g in genes)
			if(g.active && g.modifier == AbilityModifier.Added)
				combatStats.CalculateAddedStats(g.traitType, g.value);

		foreach(Gene g in genes) 
			if(g.active && g.modifier == AbilityModifier.Percent)
				combatStats.CalculatePercentStats(g.traitType, g.value);
	
		if(combatStats.element == Element.None)
			combatStats.element = nativeElement;

		if(preCalcElement != element)
			SetColorFromElement(combatStats.element);
	}

	
	public void EatItem(Item item) {
		// assume the item has been deleted already from inventory
		if(itemsConsumed.ContainsKey(item.itemName) ==  false)
			itemsConsumed.Add(item.itemName, 1);
		else 
			itemsConsumed[item.itemName]++;

		bool updateDisplay = false;
		//check all genes. see if item consumed was a requirement for activation.
		foreach(Gene gene in genes) {
			if(gene.active)
				continue;
			foreach(GeneActivationRequirement req in gene.activationRequirements) {
				if(req.itemId == item.id) {
					Mathf.Clamp(++req.amountConsumed, 0, req.amountNeeded);
					gene.CheckActivationStatus();
					if(gene.active)
						updateDisplay = true;
				}
			}
		}

		if (updateDisplay)
			UpdateBlobInfoIfDisplayed();
	}


	public void ReturnFromQuest() {
		missionCount++;
		missionId = -1;
		UpdateGrowth();
		floatingDisplay.stateLabel.gameObject.SetActive(false);
		animator.SetBool("away", false);
		blobDragDropItem.interactable = true;
	}

	public void UpdateGrowth() {
		if(isAdult)
			blobSpriteContainer.transform.localScale = new Vector3(1f, 1f, 1f);
	}


	public void DepartForQuest(Quest quest) {
		missionId = quest.id;
		floatingDisplay.stateLabel.gameObject.SetActive(true);
		floatingDisplay.stateLabel.text = "AWAY";
		state = BlobState.OnQuest;
		animator.SetBool("away", true);
		blobDragDropItem.interactable = false;
	}


	public GameObject CreateDuplicateForUi(Transform newParent, bool canInteract) {
		GameObject newBlobGameObject = (GameObject)GameObject.Instantiate(gameObject);
		newBlobGameObject.transform.parent = newParent;
		newBlobGameObject.transform.localPosition = new Vector3(0f, -18f, 0f);
		newBlobGameObject.transform.localScale = transform.localScale;
		BlobFloatingDisplay floatingDisplay = newBlobGameObject.GetComponentInChildren<BlobFloatingDisplay>();
		GameObject.Destroy(floatingDisplay.gameObject);

		if(canInteract == false) {
			GameObject.Destroy(newBlobGameObject.GetComponentInChildren<Blob>());
			GameObject.Destroy(newBlobGameObject.GetComponentInChildren<BoxCollider>());
			GameObject.Destroy(newBlobGameObject.GetComponentInChildren<BlobDragDropItem>());
			GameObject.Destroy(newBlobGameObject.GetComponentInChildren<UIButton>());
		}
		return newBlobGameObject;
	}

	void Update() {
		if(actionDuration.TotalSeconds > 0 && actionReadyTime <= System.DateTime.Now)
			ActionDone();

		if(room != null && room.type == Room.RoomType.Workshop && state == BlobState.Idle) {
			state = BlobState.Working;
			StartActionWithDuration(new TimeSpan(workingDelay.Ticks));
		}
	}


}


