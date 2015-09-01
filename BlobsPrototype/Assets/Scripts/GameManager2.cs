using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.IO;
using System.Linq;


public class GameManager2 : MonoBehaviour {

	public GameVariables gameVars;
	public HudManager hudMan;
	public RoomManager roomMan;

	// Use this for initialization
	void Start () {
		bool firstTime = (PlayerPrefs.GetInt("FirstTimeSetup") == 0);
		if (true)//firstTime)
		{
			PlayerPrefs.SetInt("FirstTimeSetup", 1);;
			gameVars = new GameVariables();
			gameVars.nurseryBlobs = new List<Blob>();
			gameVars.villageBlobs = new List<Blob>();
			gameVars.castleBlobs = new List<Blob>();
			gameVars.year = 0;
			gameVars.gold = 100;
			gameVars.chocolate = 5;
		}
		else
		{
			gameVars = GenericDeSerialize<GameVariables>("GameVariables.dat");
		}

		hudMan.UpdateGold(gameVars.gold);
		hudMan.UpdateChocolate(gameVars.chocolate);
		Room room = roomMan.CreateRoom(3,3);
		Blob blob;

		for(int i=0; i<2; i++) {
			if(room.IsRoomFull())
				break;
			blob = new Blob();
			blob.Setup();
			room.AddBlob(blob);
		}


	}

	private void Serialize<T>(T thing, string filename)
	{
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
