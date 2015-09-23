using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public enum BlobJob {
	None,
	Farmer,
	Merchant,
	Builder,
	Fighter,
};

public enum BlobTrait {
	None,
	Hardy,
	Wise,
	Smart,
	Charming,
	Strong,
	Nimble,
};

public enum Quality {
	None = -1,
	Standard,
	Common,
	Rare,
	Epic,
	Legendary,
};

[Serializable]
public class Blob : MonoBehaviour {

	public class Stats {
		public List<int> values;

		public void Reset(int val) { 
			if(values == null) {
				values = new List<int>((int)Stat.Identifier.StatIdCount);
			}

			values.Clear();
			for(int i = 0; i < (int)Stat.Identifier.StatIdCount; i++)
				values.Add(val);
		}
	}



	public enum State {
		Idle,
		Dating,
		Breeding,
		Hatching,
		HatchReady,
		GrowingUp,
		Depressed,
		Nugget,
		Working,
		WorkingReady,
	};

	public State state = State.Idle;
	public int id;
	public int momId;
	public int dadId;
	public int spouseId;
	public bool male;
	public bool female { get {return !male;} }
	public int unfertilizedEggs;
	public Quality quality;
	public int level;
	public int goldProduction;
	public int sellValue { get {return  10 + Mathf.FloorToInt(level * 1.5f);} }
	public float levelBoostForOffspring;
	public BlobTrait trait;
	public BlobJob job;
	public bool onMission;
	public DateTime birthday;
	public Blob egg;
	public bool hasHatched;
	public bool isAdult { get {return age > adultAge;} }
	public bool isInfant { get {return (!isAdult && hasHatched);} }
	public TimeSpan blobHatchDelay;
	public TimeSpan breedReadyDelay;
	public TimeSpan depressedDelay;
	public TimeSpan mateFindDelay;
	public TimeSpan workingDelay;
	public TimeSpan adultAge;
	public DateTime heartbrokenRecoverTime;
	public DateTime mateFindTime;
	public List<Gene> genes;
	public Color color;
	public string colorName;
	public int allowedGeneCount { get {return GetGeneCountFromQuality(quality);} }
	public TimeSpan age {get {return DateTime.Now - birthday;}}
	public Dictionary<string, Texture> bodyPartSprites;
	public int tilePosX;
	public int tilePosY;
	public Room room;
	public DateTime actionReadyTime;
	public TimeSpan actionDuration;
	public bool isNugget;
	public Stats stats;

	TimeSpan blobHatchDelayStandard = new TimeSpan(0,0,5);
	TimeSpan breedReadyStandard = new TimeSpan(0,0,1);
	TimeSpan mateFindDelayStandard = new TimeSpan(0,0,1);
	TimeSpan depressedDelayStandard = new TimeSpan(0,0,1);
	TimeSpan adultAgeDelayStandard = new TimeSpan(0,0,10);
	TimeSpan workingDelayStandard = new TimeSpan(0,0,10);
	GameManager2 gameManager;
	BreedManager breedManager;
	BlobFloatingDisplay floatingDisplay;
	HudManager hudManager;

	public Blob () {
		stats = new Stats();
		quality = Quality.Common;
		level = 1;
		onMission = false;
		birthday = DateTime.Now;
		hasHatched = false;
		actionReadyTime  = new DateTime(0); 
		actionDuration = new TimeSpan(0);
		genes = new List<Gene>();
		bodyPartSprites = new Dictionary<string, Texture>();
		momId = -1;
		dadId = -1;
		spouseId = -1;
		unfertilizedEggs = 2;
		color = ColorDefines.defaultBlobColor;
		breedReadyDelay = breedReadyStandard;
		mateFindDelay = mateFindDelayStandard;
		workingDelay = workingDelayStandard;
		isNugget = false;
		tilePosX = 0;
		tilePosY = 0;
		goldProduction = 1;
		stats.Reset(5);
	}


	public void Setup() {
		gameManager = GameObject.Find("GameManager2").GetComponent<GameManager2>();
		breedManager = GameObject.Find("BreedManager").GetComponent<BreedManager>();
		hudManager = GameObject.Find("HudManager").GetComponent<HudManager>();

		id = gameManager.gameVars.blobsSpawned++;
		SetBodyTexture();
		SetEyeTexture();
		List<UISprite> blobsprites = gameObject.GetComponentsInChildren<UISprite>(true).ToList();
		Texture tex = bodyPartSprites["Body"];
		blobsprites[1].spriteName = tex.name;
		tex = bodyPartSprites["Eyes"];
		blobsprites[2].spriteName = tex.name;

		blobsprites[0].gameObject.SetActive(true);
		blobsprites[1].gameObject.SetActive(false);
		blobsprites[2].gameObject.SetActive(false);
		blobsprites[3].gameObject.SetActive(false);

		UIButton button = gameObject.GetComponent<UIButton>();
		EventDelegate ed = new EventDelegate(this, "BlobPressed");
		button.onClick.Add(ed);

		floatingDisplay = gameObject.GetComponentInChildren<BlobFloatingDisplay>();
		floatingDisplay.blob = this;

		blobHatchDelay = TimeSpan.FromTicks(blobHatchDelayStandard.Ticks * level);
		depressedDelay = TimeSpan.FromTicks(depressedDelayStandard.Ticks * level);
		adultAge = TimeSpan.FromTicks(adultAgeDelayStandard.Ticks * level);
	}


	public void SetupNugget() {
		gameManager = GameObject.Find("GameManager2").GetComponent<GameManager2>();
		breedManager = GameObject.Find("BreedManager").GetComponent<BreedManager>();
		hudManager = GameObject.Find("HudManager").GetComponent<HudManager>();

		List<UISprite> blobsprites = gameObject.GetComponentsInChildren<UISprite>(true).ToList();
		blobsprites[0].gameObject.SetActive(true);
		blobsprites[1].gameObject.SetActive(false);
		blobsprites[2].gameObject.SetActive(false);
		blobsprites[3].gameObject.SetActive(false);

		UIButton button = gameObject.GetComponent<UIButton>();
		EventDelegate ed = new EventDelegate(this, "BlobPressed");
		button.onClick.Add(ed);

		floatingDisplay = gameObject.GetComponentInChildren<BlobFloatingDisplay>();
		floatingDisplay.blob = this;

		blobHatchDelay = TimeSpan.FromTicks(blobHatchDelayStandard.Ticks * level);
	}


	public void DisplayBlobInfo() {
		hudManager.blobInfoContextMenu.DisplayWithBlob(this);
	}


	public void UpdateBlobInfoIfDisplayed() {
		HudManager hudManager = GameObject.Find("HudManager").GetComponent<HudManager>();
		if(hudManager.blobInfoContextMenu.IsDisplayed() && hudManager.blobInfoContextMenu.DisplayedBlob() == this)
			hudManager.blobInfoContextMenu.DisplayWithBlob(this);
	}


	public string GetBlobStateString() {
		return state.ToString();
	}
	
	
	public Blob GetSpouse() {
		List<Blob> blobs = room.blobs;
		foreach(Blob b in blobs)
			if(spouseId == b.id)
				return b;
		
		return null;
	}


	void BlobPressed() {
		if(!hasHatched && state == State.HatchReady) {
			Hatch(!isNugget);
			if(hudManager.blobInfoContextMenu.IsDisplayed())
				hudManager.blobInfoContextMenu.Dismiss();
			return;
		}

		if(state == State.WorkingReady) {
			CollectWork();
			return;
		}

		if(state == State.Nugget) {
			gameManager.AddGold(sellValue);
			room.DeleteBlob(this);
			return;
		}

		DisplayBlobInfo();
	}


	public void Hatch(bool displayInfo) {
		if(hasHatched)
			return;
		List<UISprite> blobsprites = gameObject.GetComponentsInChildren<UISprite>(true).ToList();
		blobsprites[0].gameObject.SetActive(false);

		if(!isNugget) {
			blobsprites[1].gameObject.SetActive(true);
			blobsprites[2].gameObject.SetActive(true);
			blobsprites[3].gameObject.SetActive(false);
			state = State.GrowingUp;
			transform.localScale = new Vector3(0.5f, 0.5f, 1f);
			StartActionWithDuration(adultAge);
		}
		else {
			blobsprites[1].gameObject.SetActive(false);
			blobsprites[2].gameObject.SetActive(false);
			blobsprites[3].gameObject.SetActive(true);
			state = State.Nugget;
		}
		hasHatched = true;
		birthday = DateTime.Now;

		floatingDisplay.HideHarvestSprite();
		if(displayInfo)
			DisplayBlobInfo();
	}

	
	public void OrderGenes() {
		genes = genes.OrderByDescending( x => x.quality).ThenBy(x => x.geneName).ToList();
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
		colorName = colorStr;
	}

	public UISprite GetBodySprite() {
		List<UISprite> blobsprites = gameObject.GetComponentsInChildren<UISprite>(true).ToList();
		return blobsprites[1];
	}
	
	
	static public int GetGeneCountFromQuality(Quality q) {
		switch (q) {
		case Quality.Common: return 3;
		case Quality.Rare: return 4;
		case Quality.Epic: return 5;
		case Quality.Legendary: return 6;
		}
		return 3;
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


	static public bool ShouldDisplayBarForState(State blobState) {
		switch(blobState) {
		case State.Dating:
		case State.Breeding:
		case State.Depressed:
		case State.Hatching: 
		case State.Working: 	return true;
		}
		return false;
	}


	static public bool ShouldDisplayHarvestSpriteForState(State blobState) {
		switch(blobState) {
		case State.WorkingReady:
		case State.HatchReady: return true;
		}
		return false;
	}


	public void AddRandomGene(Quality q) {
		Gene g = new Gene();
		genes.Add(g);
		g.quality = (q == Quality.None) ? (Quality)UnityEngine.Random.Range(0,5) : q;
		g.geneName = "Gene";
		Stat s = new Stat();
		s.id = (Stat.Identifier) UnityEngine.Random.Range(0,4);
		s.modifier = (Stat.Modifier) UnityEngine.Random.Range(0,2);
		if(s.modifier == Stat.Modifier.Added)
			s.amount = ((int)g.quality) * 2 + UnityEngine.Random.Range(1,3);
		else
			s.amount = ((int)g.quality) * 2 + UnityEngine.Random.Range(1,3) * 5;
		g.stats.Add(s);
		if(s.modifier == Stat.Modifier.Added)
			g.description = "+" + s.amount.ToString() + " to " + s.id.ToString();
		else
			g.description = s.id.ToString() + " increased by " + s.amount + "%";
	}


	public string GetActionString() {
		switch(state) {
		case State.Dating: return "Pairing";
		case State.Breeding: return "Breeding";
		case State.Depressed: return "Sad";
		case State.Hatching: return "Hatching";
		case State.HatchReady: return "Hatch";
		case State.GrowingUp: return "Growing Up";
		case State.Working: return "Working";
		}
		
		return "Breed";
	}


	void CollectWork() {
		int index = (int)Stat.Identifier.Work;
		int amount = Mathf.CeilToInt(stats.values[index] / 5f);
		gameManager.AddGold(goldProduction * level * amount);
		floatingDisplay.HideHarvestSprite();
		state = State.Idle;
	}


	public void StartActionWithDuration(TimeSpan ts) {
		actionDuration = ts;
		actionReadyTime = System.DateTime.Now + actionDuration;
		UpdateBlobInfoIfDisplayed();
	}


	public void ActionDone() {
		actionDuration = new TimeSpan(0);
		switch (state) {
		case State.Dating: DatingDone(); break;
		case State.Breeding: BreedingDone(); break;
		case State.Depressed: DepressedDone(); break;
		case State.Hatching: HatchingDone(); break;
		case State.GrowingUp: GrowingUpDone(); break;
		case State.Working: WorkingDone(); break;
		}
		UpdateBlobInfoIfDisplayed();
	}


	public void DatingDone() {
		if (female) {
			List<string> keys = ColorDefines.blobColorSet01.Keys.ToList();
			string colorStr = keys[UnityEngine.Random.Range(0,keys.Count)];
			Color color = ColorDefines.HexStringToColor(ColorDefines.blobColorSet01[colorStr]);

			ChangeColor(colorStr, color);
			Blob spouse = GetSpouse();
			spouse.ChangeColor(colorStr, color);
			long ticks1 = breedReadyStandard.Ticks * level;
			long ticks2 = breedReadyStandard.Ticks * spouse.level;
			breedReadyDelay = TimeSpan.FromTicks((ticks1 + ticks2) / 2L);
			spouse.breedReadyDelay = breedReadyDelay;
		}
		state = State.Idle;
	}


	public void DepressedDone() {
		state = State.Idle;
	}

	public void WorkingDone() {
		state = State.WorkingReady;
	}

	
	public void BreedingDone() {
		if(female)
			breedManager.BreedBlobs(this, GetSpouse());
		state = State.Idle;
	}


	public void HatchingDone() {
		state = State.HatchReady;
	}


	public void GrowingUpDone() {
		state = State.Idle;
		transform.localScale = new Vector3(1f, 1f, 1f);
	}


	public void PrepareForDelete() {
		Blob spouse = GetSpouse();
		if(spouse) {
			spouse.spouseId = -1;
			spouse.ChangeColor("Default", ColorDefines.defaultBlobColor);
		}
	}


	public void CalculateStats() {
		stats.Reset(5);
		CalculateAddedStats();
		CalculatePercentStats();
	}


	void CalculateAddedStats() {
		foreach(Gene g in genes) {
			foreach(Stat s in g.stats) {
				if(s.modifier != Stat.Modifier.Added)
					continue;
				int i = (int)s.id;
				stats.values[(int)s.id] += s.amount; break;
			}
		}
	}

	void CalculatePercentStats() {
		Stats st = new Stats();
		st.Reset(0);
		foreach(Gene g in genes) {
			foreach(Stat s in g.stats) {
				if(s.modifier != Stat.Modifier.Percent)
					continue;
				int i = (int)s.id;
				st.values[i] += s.amount;
			}
		}

		for(int i=0; i< stats.values.Count; i++)
			stats.values[i] = (int) (stats.values[i] * (.01f * st.values[i] + 1f));
	}

	void Update() {
		if(actionDuration.TotalSeconds > 0 && actionReadyTime <= System.DateTime.Now)
			ActionDone();

		if(room != null && room.type == Room.RoomType.Workshop && state == State.Idle) {
			state = Blob.State.Working;
			StartActionWithDuration(new TimeSpan(workingDelay.Ticks * level));
		}

	}


}


