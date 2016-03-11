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
};


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
	public float battleBeat;

	static float actionFixedTime = 2f;
	private float actionProgressTime = 0;

	public delegate void Beat();
	public event Beat onBeat;

	private Vector3 lastBlobAnchorPos;
	private BattleCommand inputCommand = BattleCommand.None;
	private BattleCommand queuedCommand = BattleCommand.None;
	private BattleCommand currentTask = BattleCommand.None;

	[HideInInspector] public Quest quest = null;
	[HideInInspector] public bool lastCombatSuccessFul = false;

	private BlobAnchorHelper blobAnchor;

	public void Start() {
		blobAnchor = root.GetComponentInChildren<BlobAnchorHelper>();
		SetupLevel();
	}

	void SetupLevelSpecifics() {
		AddActor("AiBlob", null, BlobAnchorPosition.Near);
		AddActor("AiBlob", null, BlobAnchorPosition.Near);
		AddActor("AiBlob", null, BlobAnchorPosition.Mid);
		AddActor("AiBlob", null, BlobAnchorPosition.Far);

		AddObject("BattleObjectGoal", new Vector3(50,0,0));
	}


	void SetupLevel() {
		blobAnchor.Reset();
		SetupLevelSpecifics();
		foreach(BattleBlobLifeBar lifeBar in HudManager.hudManager.battleHud.lifeBars)
			lifeBar.gameObject.SetActive(lifeBar.health != null);

		//Transform actors = root.FindChild("Actors");
		//foreach(Transform child in actors)
			//ProCamera2D.Instance.AddCameraTarget(child);
	}

	public void ResetLevel() {
		foreach(BattleBlobLifeBar lifeBar in HudManager.hudManager.battleHud.lifeBars)
			lifeBar.health = null;
		root.FindChild("Actors").DestroyChildren();
		root.FindChild("Objects").DestroyChildren();
		SetupLevel();
	}

	public Actor AddActor(string prefabName, CombatStats cs, BlobAnchorPosition anchorPos) {
		GameObject go = (GameObject)GameObject.Instantiate(Resources.Load(prefabName));
		go.transform.parent = root.FindChild("Actors");
		go.transform.localPosition = Vector3.zero;
		Actor actor = go.GetComponent<Actor>();
		blobAnchor.SetBlobActorPosition(actor, anchorPos);
		actors.Add(actor);
		ActorHealth health = actor.GetComponent<ActorHealth>();
		if(health != null) {
			health.onDeath += BlobDied;
			foreach(BattleBlobLifeBar lifeBar in HudManager.hudManager.battleHud.lifeBars) {
				if(lifeBar.health == null) {
					lifeBar.health = health;
					break;
				}
			}
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


	private void AllowAttacking(float duration) {
		GlobalVariables.Instance.SetVariableValue("gAttackAllowed", true);
		Invoke("BanAttacking", duration);
	}


	private void BanAttacking() {
		GlobalVariables.Instance.SetVariableValue("gAttackAllowed", false);
	}


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
		foreach(Actor actor in actors) {
			if(actor.tag == tag) {
				if(livingOnly) {
					if(actor.health != null && actor.health.IsAlive())
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


	private void ExecuteBattlecommand(BattleCommand cmd) {
		switch(cmd) {
		case BattleCommand.None: 
			return;
		case BattleCommand.Attack: 
			AllowAttacking(actionFixedTime); 
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


	private void InputUpdate() {
		if(Input.GetKey(KeyCode.K)) {
			foreach(Actor actor in actors) {
				ActorHealth ah = actor.GetComponent<ActorHealth>();
				if(ah != null && ah.health > 0)
					ah.health = 0;
			}
		}

//		if (Input.GetButtonDown("Fire1")) {
//			Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
//			pos.z = 0;
//			float radius = 10f;
//			float force = 500f;
//			Collider[] colliders = Physics.OverlapSphere(pos, radius);
//			foreach (Collider hit in colliders) {
//				Rigidbody rb = hit.GetComponent<Rigidbody>();
//				if (rb != null) {
//				ActorHealth health = rb.GetComponent<ActorHealth>();
//					if (health != null) {// && rb.gameObject.tag == "Blob") {
//						rb.AddExplosionForce(force, pos,  radius, 0f);
//						float dist = (rb.transform.position - pos).magnitude;
//						dist =  (radius - dist) / radius;
//						health.TakeDamage(force / 50f * dist);
//					}
//				}
//			}
//		}

		if(queuedCommand != BattleCommand.None || currentTask != BattleCommand.None)
			return;

		if(Input.GetKey(KeyCode.A)) 
			inputCommand = BattleCommand.Attack;
		
		if(Input.GetButton("Jump"))
			inputCommand = BattleCommand.Move;
	}

	void BeatUpdate() {
		if (onBeat != null) 
			onBeat();

		if(inputCommand != BattleCommand.None) {
			queuedCommand = inputCommand;
			inputCommand = BattleCommand.None;
			//actionProgressTime = actionFixedTime;
		}
	}

	void Update() {
		battleBeat += Time.deltaTime;
		if(battleBeat > 1f) {
			BeatUpdate();
			battleBeat = 0f;
		}
			
		actionProgressTime -= Time.deltaTime;
		if(actionProgressTime <= 0){
			if(currentTask != BattleCommand.None)
				currentTask = BattleCommand.None;

			if(queuedCommand != BattleCommand.None) {
				ExecuteBattlecommand(queuedCommand);
				currentTask = queuedCommand;
				queuedCommand = BattleCommand.None;
				actionProgressTime = actionFixedTime;
			}
		}
		
		BattleHud bh = HudManager.hudManager.battleHud;

		bh.timeBar.value = actionProgressTime / actionFixedTime;

		bh.commandLabel.color = queuedCommand == BattleCommand.None ? Color.gray : Color.green;
		bh.commandLabel.text = "command: " + queuedCommand.ToString();

		bh.actionLabel.color = currentTask == BattleCommand.None ? Color.gray : Color.green;
		bh.actionLabel.text = "action: " + currentTask.ToString();

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
