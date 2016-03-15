using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Com.LuisPedroFonseca.ProCamera2D;
using BehaviorDesigner.Runtime;

public enum BattleCommand {
	None,
	Move,
	Attack,
	Defend,
};

public enum BattleState {
	Setup,
	Running,
	Victory,
	Defeat
};


public class CombatManager : MonoBehaviour {
	private static CombatManager _combatManager;
	public static CombatManager combatManager { get {if(_combatManager == null) _combatManager = GameObject.Find("CombatManager").GetComponent<CombatManager>(); return _combatManager; } }
	HudManager hudManager { get { return HudManager.hudManager; } }
	RoomManager roomManager { get { return RoomManager.roomManager; } }
	QuestManager questManager { get { return QuestManager.questManager; } }

	public BattleState battleState { get; private set;}
	public Transform root;
	public List <Transform> blobSpawner;
	public Transform enemySpawner;
	public Camera battleCam;
	public const float battleBeatTimeConst = 1f;
	public const float battleDurationTimeConst = 180f;
	public const float actionTimeConst = 2f;

	public float battleStartTime;
	public float actionStartTime;
	public bool pauseBattleTimer = false;

	public delegate void Beat();
	public event Beat onBeat;

	private Vector3 lastBlobAnchorPos;
	public BattleCommand inputCommand = BattleCommand.None;
	private BattleCommand currentTask = BattleCommand.None;

	[HideInInspector] public Quest quest = null;
	[HideInInspector] public bool lastCombatSuccessFul = false;

	private BlobAnchorHelper blobAnchor;

	public void Start() {
		blobAnchor = root.GetComponentInChildren<BlobAnchorHelper>();
		SetupLevel();
		BeatUpdate();
	}

	void SetupLevelSpecifics() {
		AddActor("AiBlob", BlobAnchorPosition.Near);
		AddActor("AiBlob", BlobAnchorPosition.Near);
		AddActor("AiBlob", BlobAnchorPosition.Mid);
		AddActor("AiBlob", BlobAnchorPosition.Mid);
		AddActor("AiBlob", BlobAnchorPosition.Far);
		AddActor("AiBlob", BlobAnchorPosition.Far);

		AddActor("Wolf", new Vector3(30f,0f,0f));

		AddObject("BattleObjectGoal", new Vector3(35,0,0));
	}


	void SetupLevel() {
		blobAnchor.Reset();
		SetupLevelSpecifics();
		foreach(BattleBlobLifeBar lifeBar in HudManager.hudManager.battleHud.lifeBars)
			lifeBar.gameObject.SetActive(lifeBar.health != null);

		ProCamera2D.Instance.AddCameraTarget(blobAnchor.transform, 1, 1, 0, new Vector2(10,4));
		battleStartTime = Time.time;
		battleState = BattleState.Running;
		pauseBattleTimer = false;
	}

	public void ResetLevel() {
		battleState = BattleState.Setup;
		foreach(BattleBlobLifeBar lifeBar in HudManager.hudManager.battleHud.lifeBars)
			lifeBar.health = null;
		root.FindChild("Actors").DestroyChildren();
		root.FindChild("Objects").DestroyChildren();
		Transform anchors = root.FindChild("Anchors");
		foreach(Transform t in anchors) {
			if(t == blobAnchor.transform)
				continue;
			Destroy(t.gameObject);
		}
		SetupLevel();
	}

	public Actor AddActor(string prefabName, BlobAnchorPosition anchorPos) {
		Actor a = AddActor(prefabName);
		ActorHealth health = a.GetComponent<ActorHealth>();
		blobAnchor.SetBlobActorPosition(a, anchorPos);
		if(health != null) {
			foreach(BattleBlobLifeBar lifeBar in HudManager.hudManager.battleHud.lifeBars) {
				if(lifeBar.health == null) {
					lifeBar.health = health;
					break;
				}
			}
		}
		return a;
	}

	public Actor AddActor(string prefabName, Vector3 pos) {
		Actor a = AddActor(prefabName);
		a.transform.position = pos;
		ActorHealth health = a.GetComponent<ActorHealth>();
		if(health != null)
			health.onDeath += ActorDied;
		
		return a;
	}
		
	public Actor AddActor(string prefabName) {
		GameObject go = (GameObject)GameObject.Instantiate(Resources.Load(prefabName));
		go.transform.parent = root.FindChild("Actors");
		go.transform.localPosition = Vector3.zero;
		Actor actor = go.GetComponent<Actor>();
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
		battleCam.gameObject.SetActive(true);
	}


	public void EndFight() {
		battleCam.gameObject.SetActive(false);
		hudManager.combatMenu.CombatDone();
	}


	public int GetFactionCount(string tag, bool livingOnly) {
		return GetActorsOfTag(tag,livingOnly).Count;
	}


	public List<Actor> GetActorsOfTag(string tag, bool livingOnly) {
		List<Actor> ret = new List<Actor>();
		Transform actors = root.FindChild("Actors");
		foreach(Transform child in actors) {
			if(child.tag != tag)
				continue;
			Actor actor = child.GetComponent<Actor>();
			if(actor == null)
				continue;
			ActorHealth ah = child.GetComponent<ActorHealth>();
			if(!livingOnly) 
				ret.Add(actor);
			else if(ah != null && ah.health > 0)
				ret.Add(actor);
		}
		return ret;
	}


	public void ActorDied() {
		List<Actor> blobs = GetActorsOfTag("Blob", true);
		if(blobs.Count == 0 && battleState == BattleState.Running)
			Defeat();

		List<Actor> enemies = GetActorsOfTag("Enemy", true);
		if(enemies.Count == 0 && battleState == BattleState.Running)
			pauseBattleTimer = true;
	}



	void Defeat() {
		battleState = BattleState.Defeat;
		BattleHud bh = HudManager.hudManager.battleHud;
		bh.defeatObject.SetActive(true);
		Invoke("ResetLevel", 3f);
		ProCamera2D.Instance.RemoveAllCameraTargets(0);
	}

	public void Victory() {
		battleState = BattleState.Victory;
		BattleHud bh = HudManager.hudManager.battleHud;
		bh.victoryObject.SetActive(true);
		Invoke("ResetLevel", 3f);
		ProCamera2D.Instance.RemoveAllCameraTargets(0);
		blobAnchor.TranslateBlobAnchorPosition(new Vector3(50f,0,0)); // walk offscreen
	}


	private bool CanInterruptCurrnetBattlecommand(BattleCommand newCommand) {

		if(newCommand == currentTask)
			return false; // no sense in interrupting self
		
		switch(currentTask) {
		case BattleCommand.Defend: return true; // defending can be interrupted
		}

		return false; //default to no interruption
	}


	private void ExecuteBattlecommand(BattleCommand cmd) {
		object[] battleCommandParams = {cmd, actionTimeConst};
		HudManager.hudManager.battleHud.BroadcastMessage("BattleCommandExecuted", battleCommandParams);

		if(currentTask != BattleCommand.None) // if we are interrupting, this will be true
			ConlcudeBattleCommand(currentTask);
		
		currentTask = inputCommand;
		inputCommand = BattleCommand.None;
		actionStartTime = Time.time;

		switch(cmd) {
		case BattleCommand.None: 
			return;
		case BattleCommand.Attack: 
			GlobalVariables.Instance.SetVariableValue("gAttackAllowed", true);
			return;
		case BattleCommand.Defend: 
			GlobalVariables.Instance.SetVariableValue("gDefendAllowed", true);
			List<Actor> blobs = GetActorsOfTag("Blob", true);
			foreach(Actor actor in blobs) {
				actor.health.GrantImmunityDuration(actionTimeConst);
			}
			return;

		case BattleCommand.Move:
			//cast ray to see if the path is clear
			Vector3 pos = blobAnchor.anchorTargetPos;
			pos.y = .5f;
			pos.z = 0f;
			float checkDistance = blobAnchor.marchDistance;
			Ray ray = new Ray(pos, Vector3.right);
			RaycastHit[] hit = Physics.RaycastAll(ray.origin, ray.direction, checkDistance);
			Debug.DrawLine(ray.origin, ray.origin + (ray.direction * checkDistance), Color.red);
			for(int i = 0; i < hit.Length; i++)
				if( hit[i].collider.tag == "Enemy")
					return; // the way is blocked		
			blobAnchor.AdvanceBlobAnchorPosition();
			return;
		}
	}

	private void ConlcudeBattleCommand(BattleCommand cmd) {
		object[] battleCommandParams = {cmd};
		HudManager.hudManager.battleHud.BroadcastMessage("BattleCommandCompleted", battleCommandParams, SendMessageOptions.DontRequireReceiver);
		currentTask = BattleCommand.None;
		switch(cmd) {
		case BattleCommand.None: 
			return;
		case BattleCommand.Attack: 
			GlobalVariables.Instance.SetVariableValue("gAttackAllowed", false);
			return;
		case BattleCommand.Defend: 
			if(inputCommand != BattleCommand.Defend)
				GlobalVariables.Instance.SetVariableValue("gDefendAllowed", false);
			return;
		}
	}


	void KillAllBlobs() {
		List<Actor> blobs = GetActorsOfTag("Blob", true);
		foreach(Actor actor in blobs) {
			ActorHealth ah = actor.GetComponent<ActorHealth>();
			if(ah != null && ah.health > 0)
				ah.health = 0;
		}
	}

	private void InputUpdate() {
		if(Input.GetKey(KeyCode.K))
			KillAllBlobs();
	}

	void BeatUpdate() {
		Invoke("BeatUpdate", battleBeatTimeConst);
		if (onBeat != null) 
			onBeat();

		if(inputCommand == BattleCommand.None)
			return; // no input
		
		if(currentTask == BattleCommand.None || CanInterruptCurrnetBattlecommand(inputCommand)) {
			ExecuteBattlecommand(inputCommand);
			inputCommand = BattleCommand.None;
		}
	}

	void Update() {
		if((Time.time + 0.01f) > actionStartTime + actionTimeConst)  //advance time a bit so it conlcudes faster before Beat() updates, to avoid race condition
			if(currentTask != BattleCommand.None)
				ConlcudeBattleCommand(currentTask);

		if(Time.time > battleStartTime + battleDurationTimeConst && pauseBattleTimer == false)
			KillAllBlobs();
		
		BattleHud bh = HudManager.hudManager.battleHud;

		bh.timeBar.value = (Time.time - actionStartTime) / actionTimeConst;

		bh.commandLabel.color = inputCommand == BattleCommand.None ? Color.gray : Color.green;
		bh.commandLabel.text = "next command: " + inputCommand.ToString();

		bh.actionLabel.color = currentTask == BattleCommand.None ? Color.gray : Color.green;
		bh.actionLabel.text = "action: " + currentTask.ToString();

		if(pauseBattleTimer == false) {
			float battleTimeLeft = battleDurationTimeConst - (Time.time - battleStartTime);
			bh.battleTimerLabel.text = "Time Left\n" + GlobalDefines.TimeToString(battleTimeLeft, false, 2);
		}

		blobAnchor.Update();
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
