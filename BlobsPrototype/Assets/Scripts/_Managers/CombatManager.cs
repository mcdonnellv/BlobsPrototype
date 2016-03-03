using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Com.LuisPedroFonseca.ProCamera2D;
using BehaviorDesigner.Runtime;

public class CombatManager : MonoBehaviour {
	private static CombatManager _combatManager;
	public static CombatManager combatManager { get {if(_combatManager == null) _combatManager = GameObject.Find("CombatManager").GetComponent<CombatManager>(); return _combatManager; } }
	HudManager hudManager { get { return HudManager.hudManager; } }
	RoomManager roomManager { get { return RoomManager.roomManager; } }
	QuestManager questManager { get { return QuestManager.questManager; } }

	public List<Actor> actors = new List<Actor>();
	public Transform root;
	public List <Transform> blobSpawner;
	public Transform enemySpawner;
	public Camera battleCam;

	private float inputBlockTime = 4f;
	private float lastInputTime = -4f;
	private Vector3 lastBlobAnchorPos;

	static int spawnerIndex = 0;
	bool fighting = false;
	float fightSpeed = 1f;
	[HideInInspector] public Quest quest = null;
	[HideInInspector] public bool lastCombatSuccessFul = false;

	public void Start() {
		SetupLevel();
	}

	public void SetupLevel() {
		lastInputTime = -4f;
		GlobalVariables.Instance.SetVariableValue("gBlobAnchor",  new Vector3(0.01f,0,0));
		Transform a = GameObject.Find("BlobAnchorMarker").transform;
		a.position = new Vector3(0.01f, a.position.y, a.position.z);

		Actor mainBlob = AddActor("AiBlob", null, new Vector3(0,0,0));
		AddObject("BattleObjectGoal", new Vector3(50,0,0));
		ProCamera2D.Instance.AddCameraTarget(mainBlob.transform);
	}

	public void ResetLevel() {
		root.FindChild("Actors").DestroyChildren();
		root.FindChild("Objects").DestroyChildren();
		SetupLevel();
	}

	public Actor AddActor(string prefabName, CombatStats cs, Vector3 spawnPos) {
		GameObject go = (GameObject)GameObject.Instantiate(Resources.Load(prefabName));
		go.transform.parent = root.FindChild("Actors");
		go.transform.localPosition = spawnPos;
		Actor actor = go.GetComponent<Actor>();
		actors.Add(actor);
		ActorHealth health = actor.GetComponent<ActorHealth>();
		if(health != null) {
			health.onDeath += BlobDied;
		}
		return actor;
	}

	public GameObject AddObject(string prefabName, Vector3 spawnPos) {
		GameObject go = (GameObject)GameObject.Instantiate(Resources.Load(prefabName));
		go.transform.parent = root.FindChild("Objects");
		go.transform.localPosition = spawnPos;
		return go;
	}


	public Actor AddActor(BaseMonster monster) { return null;}


	public Actor AddActor(Blob blob) { return null; }


	public void StartFight() {
		fighting = true;
		battleCam.gameObject.SetActive(true);
	}


	public void EndFight() {
		fighting = false;
		battleCam.gameObject.SetActive(false);
		hudManager.combatMenu.CombatDone();
	}


	public int GetFactionCount(string tag, bool livingOnly) {
		return GetActorsOfTag(tag,livingOnly).Count;
	}


	public List<Actor> GetActorsOfTag(string tag, bool livingOnly) {
		List<Actor> ret = new List<Actor>();
		foreach(Actor actor in actors) {
			if(actor.tag == tag) {
				if(livingOnly) {
					if(actor.IsAlive())
						ret.Add(actor);
				}
				else
					ret.Add(actor);
			}
		}
		return ret;
	}


	public void BlobDied() {
		bool aBlobIsStillAlive = false;
		List<Actor> actorsToRemove = new List<Actor>();
		foreach(Actor actor in actors) {
			ActorHealth ah = actor.GetComponent<ActorHealth>();
			if(ah != null && ah.health > 0)
				aBlobIsStillAlive = true;
			else
				actorsToRemove.Add(actor);
		}

		foreach(Actor actor in actorsToRemove)
			actors.Remove(actor);

		if(aBlobIsStillAlive == false)
			ResetLevel();
	}

	public void AdvanceBlobAnchorPosition() {
		TransformBlobAnchorPosition(new Vector3(10f,0,0));
	}

	public void RevertBlobAnchorPosition() {
		TransformBlobAnchorPosition(new Vector3(-10f,0,0));
	}

	private void TransformBlobAnchorPosition(Vector3 offset) {
		SharedVector3 sharedVar = (SharedVector3)GlobalVariables.Instance.GetVariable("gBlobAnchor");
		Vector3 anchor = sharedVar.Value + offset;
		anchor.x = Mathf.Max(0f, anchor.x);
		GlobalVariables.Instance.SetVariableValue("gBlobAnchor",  anchor);
		Transform a = GameObject.Find("BlobAnchorMarker").transform;
		a.position = new Vector3(anchor.x, a.position.y, a.position.z);
	}

	private void InputUpdate() {
		if(Input.GetKeyDown(KeyCode.K)) {
			foreach(Actor actor in actors) {
				ActorHealth ah = actor.GetComponent<ActorHealth>();
				if(ah != null && ah.health > 0)
					ah.health = 0;
			}
		}

		if(lastInputTime + inputBlockTime > Time.time)
			return;
		
		if(Input.GetButton("Jump")) {
			AdvanceBlobAnchorPosition();
			lastInputTime = Time.time;
		}
			

	}

	void Update() {
		

		 
	}

	void FixedUpdate () {
		InputUpdate();
	}


	//		combatant.monster = monster;
//		combatant.id = curId++;
//		combatant.combatStats = monster.combatStats;
//		combatant.combatStats.attack.combatValue *= monster.level;
//		combatant.combatStats.armor.combatValue *= monster.level;
//		combatant.combatStats.speed.combatValue *= monster.level;
//		combatant.name = combatant.monster.itemName + (++monsterCt).ToString();
//		combatant.SetInitialRandomCombatSpeed();
//		combatants.Add(combatant);
				


//	public void SpeedUpPressed() {
//		fightSpeed = 10f;
//		foreach(Combatant combatant in combatants) {
//			TimeSpan timeLeft = combatant.turnTime - System.DateTime.Now;
//			float secs = ((float)timeLeft.TotalSeconds) / fightSpeed;
//			combatant.turnTime = System.DateTime.Now + new TimeSpan(0, 0, Mathf.FloorToInt(secs));
//		}
//	}
//
//
//	public void AddCombatant(Monster monster) {
//		Combatant combatant = new Combatant();
//		combatant.monster = monster;
//		combatant.id = curId++;
//		combatant.combatStats = monster.combatStats;
//		combatant.combatStats.attack.combatValue *= monster.level;
//		combatant.combatStats.armor.combatValue *= monster.level;
//		combatant.combatStats.speed.combatValue *= monster.level;
//		combatant.name = combatant.monster.itemName + (++monsterCt).ToString();
//		combatant.SetInitialRandomCombatSpeed();
//		combatants.Add(combatant);
//	}
//
//
//	public void AddCombatant(Blob blob) {
//		Combatant combatant = new Combatant();
//		combatant.blob = blob;
//		combatant.id = curId++;
//		combatant.combatStats = blob.combatStats;
//		combatant.combatStats.ResetForCombat();
//		combatant.name = "Blob" + (++blobCt).ToString();
//		combatant.SetInitialRandomCombatSpeed();
//		combatants.Add(combatant);
//	}
//
//
//	List<Combatant> GetAliveMonsters() {
//		List<Combatant> ret = new List<Combatant>();
//		foreach(Combatant combatant in combatants) 
//			if(combatant.monster != null && !combatant.IsZeroHalth()) 
//				ret.Add(combatant);
//		return ret;
//	}
//
//
//	List<Combatant> GetAliveBlobs() {
//		List<Combatant> ret = new List<Combatant>();
//		foreach(Combatant combatant in combatants) 
//			if(combatant.isBlob && !combatant.IsZeroHalth()) 
//				ret.Add(combatant);
//		return ret;
//	}


//	public int GetMonsterCount() {
//		int ret = 0;
//		foreach(Combatant combatant in combatants) 
//			if(combatant.isBlob) 
//				ret++;
//		return ret;
//	}
//
//
//	public void StartFight() {
//		battleFieldManager.Show();
//		foreach(Combatant combatant in combatants) {
//			if(combatant.isBlob)
//				battleFieldManager.SpawnBlob(combatant.blob);
//			else 
//				battleFieldManager.SpawnMonster(combatant.monster);
//		}
//
//
//
//		fightSpeed = 1f;
//		combatMenu = hudManager.combatMenu;
//		if (!combatMenu.IsDisplayed())
//			return;
//
//		combatants = combatants.OrderBy(x => x.name).ToList();
//		foreach(Combatant combatant in combatants)
//			combatMenu.AddCombatant(combatant.name);
//
//		// TODO: Activate buffs or trigger gene abilities that activate on combat start
//		foreach(Combatant combatant in combatants) {
//			combatant.CalculatePreCombatStats(); //genes
//			combatant.CalculatePreCombatStats(quest.combatBonuses); //quest bonus
//		}
//
//		// fastest combatants act first
//		combatants = combatants.OrderByDescending(x => x.combatStats.speed.combatValue).ToList();
//
//		// Get initial targets
//		foreach(Combatant combatant in combatants)
//			AcquireTarget(combatant);
//			
//
//		// Schedule when they take their turn based on ther speed (stagger by 1s)
//		float s = 0f;
//		foreach(Combatant combatant in combatants) {
//			float secs = s + (2f * (100f / combatant.combatStats.speed.combatValue));
//			combatant.turnTime = System.DateTime.Now + new TimeSpan(0, 0, Mathf.FloorToInt(secs));
//			s += 1f; //stagger by 1s
//		}
//
//		combatIsRunning = true;
//	}
//
//
//	public bool CheckGameEnd() {
//		if(GetAliveBlobs().Count == 0) {
//			GameLost();
//			return true;
//		}
//		if(GetAliveMonsters().Count == 0) {
//			GameWon();
//			return true;
//		}
//		return false;
//	}
//
//
//	void PerformTurn(Combatant combatant) {
//		combatant.turnTime = DateTime.MaxValue;
//		if(combatant.target == null || combatant.target.IsZeroHalth()) {
//			AcquireTarget(combatant);
//			SetTurnDelayQuick(combatant);
//			return;
//		}
//
//		int baseAtk = Mathf.FloorToInt(combatant.combatStats.attack.combatValue / 10f);
//		float variance = 0;//UnityEngine.Random.Range(-0.2f ,0.2f);
//		int damage = baseAtk + Mathf.FloorToInt(baseAtk * variance);
//		damage = Mathf.Max(damage, 1);
//		AttackCombatant(combatant, combatant.target, damage);
//	}
//
//
//	void AcquireTarget(Combatant combatant) {
//		combatant.target = null;
//		List<Combatant> monsters = GetAliveMonsters();
//		List<Combatant> blobs = GetAliveBlobs();
//		if(combatant.isBlob && monsters.Count > 0)
//			combatant.target = monsters[UnityEngine.Random.Range(0, monsters.Count)];
//		else if(combatant.monster != null && blobs.Count > 0)
//			combatant.target = blobs[UnityEngine.Random.Range(0, blobs.Count)];
//
//		if(combatant.target != null)
//			combatMenu.PushMessage(combatant.name + " targets " + combatant.target.name);
//	}
//
//
//	void SetTurnDelayQuick(Combatant combatant) {
//		float secs = .25f + (.75f * (100f / combatant.combatStats.speed.combatValue));
//		combatant.turnTime = System.DateTime.Now + new TimeSpan(0, 0, Mathf.FloorToInt(secs/ fightSpeed));
//	}
//
//
//	void SetTurnDelayNormal(Combatant combatant) {
//		float secs = 2f + (5f * (100f / combatant.combatStats.speed.combatValue));
//		combatant.turnTime = System.DateTime.Now + new TimeSpan(0, 0, Mathf.FloorToInt(secs/ fightSpeed));
//	}
//
//
//	void AttackCombatant(Combatant attacker, Combatant receiver, int damage) {
//		bool blobTakingDamage = receiver.isBlob;
//		string colorstr = ColorDefines.ColorToHexString( blobTakingDamage ? Color.red : Color.green );
//		float defense = 100f / receiver.combatStats.armor.combatValue;
//		damage = Mathf.FloorToInt(damage * defense);
//		receiver.combatStats.health.combatValue -= damage;
//		combatMenu.PushMessage(colorstr + attacker.name + " attacks " + receiver.name + " for " + damage.ToString() + " damage[-]");
//
//		if(receiver.IsZeroHalth()) 
//			combatMenu.PushMessage( receiver.name + (blobTakingDamage ? " has fainted" : " has died"));
//
//
//		float healthTotal = (blobTakingDamage ? receiver.blob.combatStats.health.geneModdedValue : receiver.monster.combatStats.health.geneModdedValue);
//		float healthPercentF = receiver.combatStats.health.combatValue / healthTotal;
//		int healthPercent = Mathf.FloorToInt(healthPercentF * 100);
//		combatMenu.UpdateCombatantHealth(receiver.name, healthPercent);
//	}
//
//
//	public void GameLost() { 
//		combatMenu.PushMessage( "All blobs have fainted");
//		lastCombatSuccessFul = false;
//	}
//	
//	
//	public void GameWon() { 
//		combatMenu.PushMessage( "All monsters have been defeated");
//		lastCombatSuccessFul = true;
//	}
//
//
//	public void CombatDone() {
//		battleFieldManager.Hide();
//		combatIsRunning = false;
//		combatMenu.CombatDone();
//		Cleanup();
//	}
//
//
//	void Cleanup() {
//		combatants.Clear();
//		curId = 0;
//		blobCt = 0;
//		monsterCt = 0;
//	}
//
//
//	// Update is called once per frame
//	void Update () {
//		if(combatIsRunning == false)
//			return;
//
//		if (CheckGameEnd()) {
//			CombatDone();
//			return;
//		}
//
//		foreach(Combatant combatant in combatants) {
//			if(combatant.IsZeroHalth())
//				continue;
//
//			if(combatant.turnTime == DateTime.MaxValue)
//				SetTurnDelayNormal(combatant);
//			
//			if(combatant.turnTime <= System.DateTime.Now) 
//				PerformTurn(combatant);
//		}
//	}
}
