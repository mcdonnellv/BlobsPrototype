using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


[Serializable]
public class Blob : BaseThing {

	// internal
	public BlobState state = BlobState.Idle;
	public int spouseId;
	public int missionId;
	public int missionCount;
	public int tilePosX;
	public int tilePosY;
	public Gender gender;
	public DateTime birthday;
	public List<Gene> genes;
	public List<Gene> hiddenGenes;
	public List<Gene> dormantGenes;
	public int geneSlots;
	public CombatStats combatStats;
	public Element nativeElement;
	public Sigil sigil;
	public Element element { get { return combatStats.element; } }
	public Room room;
	public DateTime actionReadyTime;
	public TimeSpan actionDuration;
	public Dictionary<string,string> blobColor;
	public BlobGameObject gameObject;

	public static int maxDormantGenes = 2;
	public static float dormantSurfaceChance = .1f;

	// helpers
	public bool male { get {return gender == Gender.Male;} }
	public bool female { get {return gender == Gender.Female;} }
	public bool isInfant { get {return (!isAdult && hasHatched);} }
	public bool isAdult { get {return missionCount >= 3;} }
	public bool hasHatched { get {return birthday != DateTime.MinValue;} }
	public bool canBreed { get {return isAdult && state == BlobState.Idle;} }
	public bool canMerge { get {return hasHatched && state == BlobState.Idle;} }
	public TimeSpan age {get {return DateTime.Now - birthday;} }
	public TimeSpan blobHatchDelay {get {return TimeSpan.FromTicks(blobHatchDelayStandard.Ticks);} }
	public TimeSpan breedReadyDelay {get {return TimeSpan.FromTicks(breedReadyStandard.Ticks);} }
	public TimeSpan workingDelay {get {return TimeSpan.FromTicks(workingDelayStandard.Ticks);} }
	public TimeSpan recoveryDelay {get {return TimeSpan.FromTicks(recoveryDelayStandard.Ticks);} }
	public BlobFloatingDisplay floatingDisplay;

	// managers
	GameManager2 gameManager { get { return GameManager2.gameManager; } }
	BreedManager breedManager { get { return BreedManager.breedManager; } }
	HudManager hudManager { get { return HudManager.hudManager; } }
	GeneManager geneManager { get { return GeneManager.geneManager; } }
	RoomManager roomManager  { get { return RoomManager.roomManager; } }
	
	// time delays
	TimeSpan blobHatchDelayStandard = new TimeSpan(0,0,1);
	TimeSpan workingDelayStandard = new TimeSpan(0,0,10);
	TimeSpan breedReadyStandard = new TimeSpan(0,0,3);
	TimeSpan recoveryDelayStandard = new TimeSpan(0,1,0);

	public void OrderGenes() {	genes = genes.OrderByDescending( x => x.quality).ThenBy(x => x.itemName).ToList();	}


	public Blob () {
		combatStats = new CombatStats();
		genes = new List<Gene>();
		hiddenGenes = new List<Gene>();
		dormantGenes = new List<Gene>(); 
		birthday = DateTime.MinValue;
		actionReadyTime  = new DateTime(0); 
		actionDuration = new TimeSpan(0);
		spouseId = -1;
		tilePosX = 0;
		tilePosY = 0;
		missionCount = 0;
		geneSlots = 1;
	}


	public void SetNativeElement(Element e) {
		nativeElement = e;
		combatStats.element = nativeElement;
		gameObject.SetColorFromElement(nativeElement);
	}

	
	public Blob GetSpouse() {
		List<Blob> blobs = room.blobs;
		foreach(Blob b in blobs)
			if(spouseId == b.id)
				return b;
		return null;
	}
	

	public void Hatch() {
		if(hasHatched)
			return;
		birthday = DateTime.Now;
		state = BlobState.Idle;
		gameObject.Hatch();
	}
	

	public string GetActionString() {
		string retString = "";
		switch(state) {
		case BlobState.Idle: retString = "Idle"; break;
		case BlobState.Breeding: retString = "Breeding"; break;
		case BlobState.Hatching: retString = "Hatching"; break;
		case BlobState.HatchReady: retString = "Hatch"; break;
		case BlobState.Working: retString = "Working"; break;
		case BlobState.Recovering: retString = "Recovering"; break;
		case BlobState.Questing: 
			if(QuestManager.questManager.GetBaseQuestByID(missionId).type == QuestType.Scouting)
				retString = "Scouting";
			else
				retString = "Questing"; 
			break;
		case BlobState.QuestComplete: retString = "Complete"; break;
		}
		return retString.ToUpper();
	}


	public void ActionDone() {
		actionDuration = new TimeSpan(0);
		switch (state) {
		case BlobState.Breeding: 
			if(female)
				breedManager.BreedBlobs(GetSpouse(), this);
			state = BlobState.Idle;
			break;
		case BlobState.Hatching: 
			state = BlobState.HatchReady;
			break;
		case BlobState.Questing: 
			state = BlobState.QuestComplete;
			break;
		case BlobState.Recovering:
			gameObject.animator.SetBool("recovering", false);
			state = BlobState.Idle;
			break;
		}
		gameObject.UpdateBlobInfoIfDisplayed();
	}


	public void CleanUp() {
		Blob spouse = GetSpouse();
		if(spouse != null && spouse.spouseId == id)
			spouse.spouseId = -1;
	}

	
	public void EatItem(Item item) {
		// assume the item has been deleted already from inventory
		// TODO: Do Stuff
	}


	public void OnBirth() {
		foreach(Gene g in hiddenGenes)
			if(g.functionality != null)
				g.functionality.OnBirth(this, g);

		foreach(Gene g in genes)
			if(g.functionality != null)
				g.functionality.OnBirth(this, g);

		combatStats.BirthDone();
		OnAdd(); //run only once whne born
	}


	public void OnAdd() {
		foreach(Gene g in hiddenGenes)
			if(g.functionality != null)
				g.functionality.OnAdd(this, g);
		
		foreach(Gene g in genes)
			if(g.functionality != null)
				g.functionality.OnAdd(this, g);
	}


	public void AddGene(Gene g, bool trigger) {
		genes.Add(g);
		if(trigger && g.functionality != null)
			g.functionality.OnAdd(this, g);
		Gene newGene = new Gene(geneManager.GetBaseGeneByID(g.id));
		dormantGenes.Add(newGene);
		dormantGenes = GeneManager.LimitGenes(dormantGenes, Blob.maxDormantGenes);
	}

	public void RemoveGene(Gene g) {
		if(g.functionality != null)
			g.functionality.OnRemove(this, g);
		genes.Remove(g);
	}
}


