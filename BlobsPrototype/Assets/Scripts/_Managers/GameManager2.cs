using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Linq;


public class GameManager2 : MonoBehaviour {

	public GameVariables gameVars;
	public HudManager hudMan;
	public RoomManager roomMan;
	public GeneManager geneManager;
	public ItemManager itemManager;

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
		}
		else {
			gameVars = GenericDeSerialize<GameVariables>("GameVariables.dat");
		}

		roomMan.maxSize = 5;
		roomMan.minSize = 2;
		hudMan.UpdateGold(gameVars.gold);
		hudMan.UpdateChocolate(gameVars.chocolate);
		Room room = roomMan.CreateRoom(roomMan.minSize, Room.RoomType.Field);

		Blob blob;

		for(int i=0; i<2; i++) {
			if(room.IsRoomFull())
				break;

			GameObject blobGameObject = (GameObject)GameObject.Instantiate(Resources.Load("BlobSprites"));
			blob = blobGameObject.AddComponent<Blob>();
			blob.male = (i % 2 == 0);
			blob.Setup();
			blob.Hatch(false);
			blob.birthday = DateTime.Now - new TimeSpan(1,0,0);
			blob.actionDuration = new TimeSpan(0);
			blob.state = Blob.State.Idle;
			room.AddBlob(blob);

			//blob.AddRandomGene(Quality.Standard);
			blob.genes.Add(geneManager.genes[0]);
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


	public int GetAverageLevel() {
		float cummulativeLevel = 0f;
		int totalBlobs = 0;
		List <Blob> allBlobs = gameVars.allBlobs;
		foreach (Room room in roomMan.rooms) {
			foreach (Blob blob in room.blobs) {
				if(blob.hasHatched){
					cummulativeLevel += blob.level;
					totalBlobs++;
				}
			}
		}
		
		float averageLevel = cummulativeLevel / totalBlobs;
		return Mathf.RoundToInt(averageLevel);
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
