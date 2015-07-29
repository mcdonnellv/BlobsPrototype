using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

[System.Serializable]
public class GameVariables 
{
	public int gold;
	public int year;
	public List<Blob> nurseryBlobs;
	public List<Blob> villageBlobs;
	public List<Blob> castleBlobs;
}

public class GameManager : MonoBehaviour 
{
	public MissionManager mm;
	public NurseryManager nm;
	public VillageManager vm;
	public CastleManager cm;
	public MutationManager mum;
	public GameObject missionView;
	public GameObject breedingView;
	public GameObject grid;
	public GameObject selectModeCover;
	public UILabel averageQualityLabel;
	public UILabel goldLabel;
	public UILabel yearLabel;
	public UILabel missionButtonLabel;
	public UISlider yearProgressBar;
	public UIButton missionButton;
	public UIButton rightNavButton;
	public UIButton leftNavButton;
	public UIButton buildButton;
	public GameObject gameOverObject;
	public GameObject winnerObject;
	public UICamera gameCam;
	public Popup popup;
	public GameVariables gameVars;
	public System.TimeSpan blobHatchDelay;
	public System.TimeSpan breedReadyDelay;
	public System.TimeSpan yearFillDelay;
	public System.TimeSpan breedBarFillTime;
	public System.DateTime yearFillTime;


	public float tributeGoldPerQuality;
	public float tributeMaxMulitplier;
	public System.TimeSpan blobGoldProductionDelay;
	public int maxBlobs;

	public int breedCost;
	public int breedBaseCost;
	public int breedCostMax;
	public int sellValue;
	public int breedingAge;
	public int maxBreedcount;
	public int villageCost;
	public int castleCost;
	public float timeScale;
	public int gold {get{return gameVars.gold;}}

	float timeScaleOld;
	bool selectMode;



	void Start ()
	{
		timeScaleOld = 0f;
		timeScale = 1f;
		blobHatchDelay = new System.TimeSpan(0,0,30);
		breedReadyDelay = new System.TimeSpan(0,0,10);
		breedBarFillTime = new System.TimeSpan(0,0,1);
		yearFillDelay = new System.TimeSpan(0,1,0);
		yearFillTime = System.DateTime.Now + yearFillDelay;

		maxBlobs = 20;
		breedCost = 10;
		breedBaseCost = 10;
		breedCostMax = 100;
		sellValue = 15;
		breedingAge = 1;
		maxBreedcount = 3;
		tributeGoldPerQuality = 2f;
		tributeMaxMulitplier = 5f;

		villageCost = 150;
		castleCost = 800;

		selectMode = false;

		breedingView.SetActive(true);
		missionView.SetActive(false);
		selectModeCover.SetActive(false);



		yearProgressBar.value = 0f;

		Vector3 pos = gameCam.transform.localPosition;
		pos.x = 740f * 2;
		gameCam.transform.localPosition = new Vector3(pos.x, pos.y);
		rightNavButton.gameObject.SetActive(false);
		leftNavButton.gameObject.SetActive(false);

		bool firstTime = (PlayerPrefs.GetInt("FirstTimeSetup") == 0);
		if (firstTime)
		{
			PlayerPrefs.SetInt("FirstTimeSetup", 1);;
			gameVars = new GameVariables();
			gameVars.nurseryBlobs = new List<Blob>();
			gameVars.villageBlobs = new List<Blob>();
			gameVars.castleBlobs = new List<Blob>();
			FirstTimeSetup();

			gameVars.year = 0;
		}
		else
		{
			gameVars = GenericDeSerialize<GameVariables>("GameVariables.dat");
		}

		foreach(Blob b in nm.blobs) 
			nm.blobPanel.UpdateBlobCellWithBlob(nm.blobs.IndexOf(b), b);

		nm.PressGridItem(0);
		AddGold(0);
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
	
	void FirstTimeSetup()
	{
		gameVars.year = 0;
		gameVars.gold = 100;
		nm.FirstTimeSetup();
	}

	void OnApplicationQuit()
	{
		Serialize<GameVariables>(gameVars, "GameVariables.dat");
		PlayerPrefs.Save();
	}


	public void EnableSelectMode(bool enable)
	{
		selectMode = enable;
		selectModeCover.SetActive(enable);
	}


	public void UpdateAverageQuality()
	{
		float averageQuality = GetAverageQuality();
		averageQualityLabel.text = "Average Quality: " + Blob.GetQualityStringFromValue(averageQuality);
	}

	public float GetAverageQuality()
	{
		float cummulativeQuality = 0f;
		int totalBlobs = 0;
		
		// iterate thru nursery blobs
		if(nm.blobs != null)
		{
			totalBlobs += nm.blobs.Count;
			foreach (Blob blob in nm.blobs)
				if(blob.hasHatched)
					cummulativeQuality += blob.quality;
		}

		// iterate thru village blobs
		if(vm.blobs != null)
		{
			totalBlobs += vm.blobs.Count;
			foreach (Blob blob in vm.blobs)
				cummulativeQuality += blob.quality;
		}

		float averageQuality = cummulativeQuality / totalBlobs;
		averageQuality = Mathf.Round(averageQuality * 10f) / 10f;

		return averageQuality;

	}

	public void  AddGold(int val)
	{
		gameVars.gold += val;
		goldLabel.text = "Gold: " + gold.ToString();

		UILabel label = buildButton.GetComponentInChildren<UILabel>();
		if (!vm.villageExists)
		{
			label.text = "Build Village     " + villageCost.ToString() + "g";
			buildButton.isEnabled = (gold >= villageCost);
		}
		else if (!cm.castleExists)
		{
			label.text = "Build Castle     " + castleCost.ToString() + "g";
			buildButton.isEnabled = (gold >= castleCost);
		}
	}

	void AgeAllBlobs()
	{
		if (nm.blobs != null)
		{
			foreach(Blob blob in nm.blobs)
			{
				if(blob.hasHatched ==  false)
					continue;
				blob.age++;
			}
		}

		if (vm.blobs != null)
		{
			foreach(Blob blob in vm.blobs)
			{
				blob.age++;
			}
		}

		nm.UpdateAllBlobCells();
		vm.UpdateAllBlobCells();
	}

	
	public void MissionsButtonPressed()
	{
		breedingView.SetActive(false);
		missionView.SetActive(true);
		mm.UpdateMissionList();
	}

	
	public void RightNavButtonPressed()
	{
		leftNavButton.gameObject.SetActive(true);
		float sw = 740f;
		Vector3 pos = gameCam.transform.localPosition;
		pos.x += (pos.x >= sw*3) ? 0f : sw;

		if (pos.x >= sw*2 && vm.villageExists == false)
			rightNavButton.gameObject.SetActive(false);

		if (pos.x >= sw*3)
			rightNavButton.gameObject.SetActive(false);

		gameCam.transform.localPosition = new Vector3(pos.x, pos.y);
	}


	public void LeftNavButtonPressed()
	{
		rightNavButton.gameObject.SetActive(true);
		float sw = 740f;
		Vector3 pos = gameCam.transform.localPosition;
		pos.x -= (pos.x <= sw*1) ? 0f : sw;

		if (pos.x <= sw*2 && cm.castleExists == false)
			leftNavButton.gameObject.SetActive(false);

		if (pos.x <= sw*1)
			leftNavButton.gameObject.SetActive(false);

		gameCam.transform.localPosition = new Vector3(pos.x, pos.y);
	}


	public void BuildButtonPressed()
	{
		if (!vm.villageExists)
		{
			vm.villageExists = true;
			AddGold(-villageCost);
			nm.toVillageButton.gameObject.SetActive(true);
			rightNavButton.gameObject.SetActive(true);
			popup.Show("Village Now Available", "You can now move Blobs to the new village to work and give you tribute.");
		}
		else if (!cm.castleExists)
		{
			cm.castleExists = true;
			AddGold(-castleCost);
			buildButton.gameObject.SetActive(false);
			nm.toCastleButton.gameObject.SetActive(true);
			leftNavButton.gameObject.SetActive(true);
			popup.Show("Castle Now Available", "You can now move Blobs to the new castle to perform missions.");
		}
	}


	// Update is called once per frame
	void Update () 
	{
		if (yearFillTime > System.DateTime.Now)
		{
			System.TimeSpan ts = (yearFillTime - System.DateTime.Now);
			yearProgressBar.value = 1f - (float)(ts.TotalSeconds / yearFillDelay.TotalSeconds);
		}	
		else
		{
			gameVars.year++;
			yearLabel.text = "Year: " + gameVars.year.ToString();
			AgeAllBlobs();
			yearFillTime = System.DateTime.Now + yearFillDelay;
		}

		if(timeScale != timeScaleOld)
		{
			timeScaleOld = timeScale;
			blobHatchDelay = new System.TimeSpan(0,0,(int)(blobHatchDelay.TotalSeconds * timeScale));
			breedReadyDelay = new System.TimeSpan(0,0,(int)(breedReadyDelay.TotalSeconds * timeScale));
			breedBarFillTime = new System.TimeSpan(0,0,(int)(breedBarFillTime.TotalSeconds * timeScale));
			yearFillDelay = new System.TimeSpan(0,0,(int)(yearFillDelay.TotalSeconds * timeScale));
		}
	}
}
