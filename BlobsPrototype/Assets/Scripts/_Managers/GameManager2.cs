using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Linq;


public class GameManager2 : MonoBehaviour {
	private static GameManager2 _gameManager;
	public static GameManager2 gameManager { get {if(_gameManager == null) _gameManager = GameObject.Find("GameManager2").GetComponent<GameManager2>(); return _gameManager; } }

	public GameVariables gameVars;
	public HudManager hudMan;
	public RoomManager roomMan;
	public GeneManager geneManager;
	public ItemManager itemManager;
	public QuestManager questManager;
	public List<GameObject> cameraObjects;


	void SelectCamera(int index) {
		foreach(GameObject camObject in cameraObjects) {
			if(index == cameraObjects.IndexOf(camObject))
				camObject.SetActive(true);
			else
				camObject.SetActive(false);
		}
	}


	void CombatMode (bool enable) {
		if(enable) {
			// set camera
			SelectCamera(0);
			// enable combatmanager
			CombatManager.combatManager.gameObject.SetActive(true);
			CombatManager.combatManager.aiSimulator.gameObject.SetActive(true);
			// enable combatHud
			HudManager.hudManager.battleHud.gameObject.SetActive(true);
		}
		else {
			SelectCamera(1);
			CombatManager.combatManager.gameObject.SetActive(false);
			CombatManager.combatManager.aiSimulator.gameObject.SetActive(false);
			HudManager.hudManager.battleHud.gameObject.SetActive(false);
		}
	}


	// Use this for initialization
	void Awake () {
		CombatMode(false);
		bool firstTime = (PlayerPrefs.GetInt("FirstTimeSetup") == 0);
		if (true) {
			PlayerPrefs.SetInt("FirstTimeSetup", 1);;
			gameVars = new GameVariables();
			gameVars.nurseryBlobs = new List<Blob>();
			gameVars.villageBlobs = new List<Blob>();
			gameVars.castleBlobs = new List<Blob>();
			gameVars.year = 0;
			gameVars.gold = 80;
			gameVars.chocolate = 20;
			gameVars.inventoryItemSlots = 20;
			gameVars.inventoryGeneSlots = 20;
			geneManager.FirstTimeSetup();
			itemManager.FirstTimeSetup();
			questManager.FirstTimeSetup();
		}
		else {
			gameVars = GenericDeSerialize<GameVariables>("GameVariables.dat");
		}


		hudMan.UpdateGold(gameVars.gold);
		hudMan.UpdateChocolate(gameVars.chocolate);
		List<Blob> blobs = CreateInitalBlobs(5);
		//CreateRoom(blobs);
		foreach(Blob blob in blobs) 
			BlobManager.blobManager.blobs.Add(blob);
	}

	void CreateRoom(List<Blob> blobs) {
		roomMan.maxSize = 5;
		roomMan.minSize = 2;
		Room room = roomMan.CreateRoom(roomMan.minSize + 1, Room.RoomType.Field);
		foreach(Blob blob in blobs)
			room.AddBlob(blob);
	}


	List<Blob> CreateInitalBlobs(int num) {
		List<Blob> blobs = new List<Blob>();
		for(int i=0; i<num; i++) {
			Blob blob = null;
			GameObject go = (GameObject)GameObject.Instantiate(Resources.Load("BlobGameObject"));
			BlobGameObject blobGameObject = go.GetComponent<BlobGameObject>();
			blobGameObject.Setup();
			blob = blobGameObject.blob;
			blob.gender = (i % 2 == 0 ? Gender.Male : Gender.Female);
			blob.Hatch();
			blob.birthday = DateTime.Now - new TimeSpan(1,0,0);
			blob.actionDuration = new TimeSpan(0);
			blob.state = BlobState.Idle;
			blob.missionCount = 3;
			blob.gameObject.UpdateGrowth();
			blob.hiddenGenes.Add(new Gene(geneManager.GetBaseGeneByID(2))); //white
			blob.hiddenGenes.Add(new Gene(geneManager.GetBaseGeneByID(5))); //sigilA
			blob.hiddenGenes.Add(new Gene(geneManager.GetBaseGeneByID(10))); //Attack Bias I
			blob.hiddenGenes.Add(new Gene(geneManager.GetBaseGeneByID(30))); //2 Gene Slots

			blob.genes.Add(new Gene(geneManager.GetBaseGeneByID(100))); //test
			blob.dormantGenes.Add(new Gene(geneManager.GetBaseGeneByID(101))); //test
			blob.dormantGenes.Add(new Gene(geneManager.GetBaseGeneByID(102))); //test
			blob.OnBirth();
			blobs.Add(blob);
		}
		return blobs;
	}


	public void AddGold(int value) {
		gameVars.AddGold(value);
		hudMan.UpdateGold(gameVars.gold);
	}


	public void AddChocolate(int value) {
		gameVars.AddChocolate(value);
		hudMan.UpdateChocolate(gameVars.chocolate);
	}


	private void Serialize<T>(T thing, string filename) {
		XmlSerializer serializer = new XmlSerializer(typeof(T));
		TextWriter tw = new StreamWriter(filename);
		serializer.Serialize(tw, thing);
		tw.Close();
	}
	
	
	private T GenericDeSerialize<T>(string filename)
	{
		XmlSerializer serializer = new XmlSerializer(typeof(T));
		TextReader tr = new StreamReader(filename);
		T b = (T)serializer.Deserialize(tr);
		tr.Close();
		return b;
	}


	// Update is called once per frame
	void Update () {
	
	}
}
