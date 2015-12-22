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

	// Use this for initialization
	void Start () {
		bool firstTime = (PlayerPrefs.GetInt("FirstTimeSetup") == 0);
		if (true) {
			PlayerPrefs.SetInt("FirstTimeSetup", 1);;
			gameVars = new GameVariables();
			gameVars.nurseryBlobs = new List<Blob>();
			gameVars.villageBlobs = new List<Blob>();
			gameVars.castleBlobs = new List<Blob>();
			gameVars.year = 0;
			gameVars.gold = 80;
			gameVars.chocolate = 5;
			gameVars.inventoryItemSlots = 20;
			gameVars.inventoryGeneSlots = 20;
			geneManager.FirstTimeSetup();
			itemManager.FirstTimeSetup();
			questManager.FirstTimeSetup();
		}
		else {
			gameVars = GenericDeSerialize<GameVariables>("GameVariables.dat");
		}

		roomMan.maxSize = 5;
		roomMan.minSize = 2;
		hudMan.UpdateGold(gameVars.gold);
		hudMan.UpdateChocolate(gameVars.chocolate);
		Room room = roomMan.CreateRoom(roomMan.minSize, Room.RoomType.Field);

		Blob blob = null;

		for(int i=0; i<2; i++) {
			if(room.IsRoomFull())
				break;

			GameObject go = (GameObject)GameObject.Instantiate(Resources.Load("BlobGameObject"));
			BlobGameObject blobGameObject = go.GetComponent<BlobGameObject>();
			blobGameObject.Setup();
			blob = blobGameObject.blob;
			blob.gender = (i % 2 == 0 ? Gender.Male : Gender.Female);
			blob.SetNativeElement(Element.Black);//(Element)UnityEngine.Random.Range(0, (int)Element.ElementCt);
			blob.sigil = (Sigil)UnityEngine.Random.Range(0, (int)Sigil.SigilCt);
			blob.Hatch();
			blob.birthday = DateTime.Now - new TimeSpan(1,0,0);
			blob.actionDuration = new TimeSpan(0);
			blob.state = BlobState.Idle;
			blob.missionCount = 3;
			blob.gameObject.UpdateGrowth();
			room.AddBlob(blob);

			blob.genes.Add(new Gene(geneManager.genes[0]));
			blob.genes[0].state = GeneState.Available;
		}
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
