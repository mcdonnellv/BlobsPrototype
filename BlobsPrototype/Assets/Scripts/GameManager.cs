using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Linq;

[System.Serializable]
public class GameVariables 
{
	public int blobsSpawned = 0;
	public int visitorsSpawned = 0;
	public int gold;
	public int chocolate;
	public int year;
	public List<Blob> nurseryBlobs;
	public List<Blob> villageBlobs;
	public List<Blob> castleBlobs;
	public List<Blob> allBlobs {get {return (nurseryBlobs.Union(villageBlobs.Union(castleBlobs))).ToList();} }
}

public class GameManager : MonoBehaviour 
{
	public MissionManager mm;
	public NurseryManager nm;
	public VillageManager vm;
	public CastleManager cm;
	public GeneManager mum;
	public BodyPartManager bpm;
	public GameObject missionView;
	public GameObject breedingView;
	public GameObject grid;
	public GameObject selectModeCover;
	public UILabel averageQualityLabel;
	public UILabel goldLabel;
	public UILabel chocolateLabel;
	public UILabel missionButtonLabel;
	public UIButton missionButton;
	public UIButton rightNavButton;
	public UIButton leftNavButton;
	public UIButton buildButton;
	public List<UIButton> visitorButtons;
	public GameObject gameOverObject;
	public GameObject winnerObject;
	public UICamera gameCam;
	public Popup popup;
	public BlobPopup blobPopup;
	public BlobPopupChoice blobPopupChoice;
	public GeneAddPopup geneAddPopup;
	public GameVariables gameVars;

	public TimeSpan blobHatchDelay;
	public TimeSpan breedReadyDelay;
	public TimeSpan breedBarFillDelay;
	public TimeSpan blobGoldProductionDelay;
	public TimeSpan yearFillDelay;
	public DateTime yearFillTime;


	public float tributeGoldPerQuality;
	public float tributeMaxMulitplier;

	public int maxBlobs;

	public int breedCost;
	public int breedBaseCost;
	public int breedCostMax;
	public int sellValue;
	public TimeSpan breedingAge;
	public int maxBreedcount;
	public int villageCost;
	public int castleCost;
	public float timeScale;
	public int gold {get{return gameVars.gold;}}
	public int chocolate {get{return gameVars.chocolate;}}

	float timeScaleOld;
	bool selectMode;

	TimeSpan blobHatchDelayOriginal;
	TimeSpan breedReadyDelayOriginal;
	TimeSpan breedBarFillDelayOriginal;
	TimeSpan blobGoldProductionDelayOriginal;
	TimeSpan yearFillDelayOriginal;



	void Start ()
	{
		timeScaleOld = 0f;
		timeScale = .3f;

		blobHatchDelay = new TimeSpan(0,0,30);
		breedReadyDelay = new TimeSpan(0,0,10);
		breedBarFillDelay = new TimeSpan(0,0,1);
		blobGoldProductionDelay = new TimeSpan(0,0,10);
		yearFillDelay = new TimeSpan(1,0,0);
		yearFillTime = DateTime.Now + yearFillDelay;

		blobHatchDelayOriginal = blobHatchDelay;
		breedReadyDelayOriginal = breedReadyDelay;
		yearFillDelayOriginal = yearFillDelay;
		blobGoldProductionDelayOriginal = blobGoldProductionDelay;
		breedBarFillDelayOriginal = breedBarFillDelay;

		maxBlobs = 20;
		breedCost = 10;
		breedBaseCost = 10;
		breedCostMax = 100;
		sellValue = 15;
		breedingAge = new TimeSpan(0,0,0,2);
		maxBreedcount = 3;
		tributeGoldPerQuality = 2f;
		tributeMaxMulitplier = 5f;

		villageCost = 300;
		castleCost = 1000;

		selectMode = false;

		breedingView.SetActive(true);
		missionView.SetActive(false);
		selectModeCover.SetActive(false);

		Vector3 pos = gameCam.transform.localPosition;
		pos.x = 1334f * 1;
		gameCam.transform.localPosition = new Vector3(pos.x, pos.y);
		rightNavButton.gameObject.SetActive(false);
		leftNavButton.gameObject.SetActive(false);

		Update();

		bool firstTime = (PlayerPrefs.GetInt("FirstTimeSetup") == 0);
		if (true)//firstTime)
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

		foreach(UIButton visitorbutton in visitorButtons)
			visitorbutton.gameObject.SetActive(false);

		nm.PressGridItem(0);
		AddGold(0);
		AddChocolate(0);
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
		gameVars.chocolate = 5;
		nm.FirstTimeSetup();
		mum.FirstTimeSetup();
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
		List <Blob> allBlobs = gameVars.allBlobs;

		foreach (Blob blob in allBlobs)
		{
			if(blob.hasHatched)
			{
				cummulativeQuality += blob.quality;
				totalBlobs++;
			}
		}

		float averageQuality = cummulativeQuality / totalBlobs;
		averageQuality = Mathf.Round(averageQuality * 10f) / 10f;

		return averageQuality;
	}


	public void AddGold(int val)
	{
		gameVars.gold += val;
		goldLabel.text = "Gold: [FFD700]" + gold.ToString() + "g[-]";

		UILabel label = buildButton.GetComponentInChildren<UILabel>();
		if (!vm.villageExists)
		{
			label.text = "Build Village     [FFD700]" + villageCost.ToString() + "g[-]";
			buildButton.isEnabled = (gold >= villageCost);
		}
		else if (!cm.castleExists)
		{
			label.text = "Build Castle     [FFD700]" + castleCost.ToString() + "g[-]";
			buildButton.isEnabled = (gold >= castleCost);
		}
	}


	public void AddChocolate(int val)
	{
		gameVars.chocolate += val;
		chocolateLabel.text = "Chocolate: [C59F76]" + chocolate.ToString() + "c[-]";
	}

	
	public void TrySellBlob(Blob blob, MonoBehaviour target)
	{
		if (blob.onMission) 
		{blobPopup.Show(blob, "Cannot Sell", "Blob is on a mission.");return;}
		
		if (blob.hasHatched == false)
		{blobPopup.Show(blob, "Cannot Sell", "Blob has not been hatched."); return;}
		
		if (blob.breedReadyTime > System.DateTime.Now)
		{blobPopup.Show(blob, "Cannot Sell", "Blob is still breeding.");return;}
		
		bool lastOfGender = true;
		List<Blob> allBlobs = gameVars.allBlobs;
		foreach(Blob b in allBlobs)
			if (b != blob && blob.male == b.male && b.hasHatched)
				lastOfGender = false;
		
		if (lastOfGender)
		{blobPopup.Show(blob, "Cannot Sell", "Cannot sell your last " + ((blob.male == true) ? "male" : "female") +" blob."); return;}
		
		Gene lastGene = null;
		foreach(Gene g1 in blob.genes)
		{
			bool geneDupeFound = false;
			foreach(Blob b in allBlobs)
			{
				if(b == blob)
					continue;
				
				foreach(Gene g2 in b.genes)
				{
					if(g1 == g2)
					{
						geneDupeFound = true;
						break;
					}
				}
				
				if(geneDupeFound)
					break;
			}
			
			if(!geneDupeFound)
			{
				lastGene = g1;
				break;
			}
		}
		
		if (lastGene != null)
		{
			blobPopupChoice.ShowChoice(blob, "Warning!", 
			                    "This is your last blob with the [9BFF9B]" + lastGene.geneName + " gene[-]. Are you sure you want to sell this blob?", 
			                 target, "SellBlobFinal", null, null); 
			return;
		}
		
		blobPopupChoice.ShowChoice(blob, "Sell Blob", "Are you sure you want to sell this blob?", target, "SellBlobFinal", null, null);
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
		float sw = 1334f;
		Vector3 pos = gameCam.transform.localPosition;
		pos.x += (pos.x >= sw*2) ? 0f : sw;

		if (pos.x >= sw*1 && vm.villageExists == false)
			rightNavButton.gameObject.SetActive(false);

		if (pos.x >= sw*2)
			rightNavButton.gameObject.SetActive(false);

		gameCam.transform.localPosition = new Vector3(pos.x, pos.y);
	}


	public void LeftNavButtonPressed()
	{
		rightNavButton.gameObject.SetActive(true);
		float sw = 1334f;
		Vector3 pos = gameCam.transform.localPosition;
		pos.x -= (pos.x <= 0) ? 0f : sw;

		if (pos.x <= sw*1 && cm.castleExists == false)
			leftNavButton.gameObject.SetActive(false);

		if (pos.x <= sw*0)
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
		if (yearFillTime <= DateTime.Now)
		{
			gameVars.year++;
			yearFillTime = DateTime.Now + yearFillDelay;
		}



		//make time go faster
		if(timeScale != timeScaleOld)
		{
			timeScaleOld = timeScale;

			//visitorDelay = new TimeSpan(0,0,(int)(visitorDelayOriginal.TotalSeconds * timeScale));
			blobHatchDelay = new TimeSpan(0,0,(int)(blobHatchDelayOriginal.TotalSeconds * timeScale));
			breedReadyDelay = new TimeSpan(0,0,(int)(breedReadyDelayOriginal.TotalSeconds * timeScale));
			breedBarFillDelay = new TimeSpan(0,0,(int)(breedBarFillDelayOriginal.TotalSeconds * timeScale));
			blobGoldProductionDelay = new TimeSpan(0,0,(int)(blobGoldProductionDelayOriginal.TotalSeconds * timeScale));
			yearFillDelay = new TimeSpan(0,0,(int)(yearFillDelayOriginal.TotalSeconds * timeScale));
		}
	}
}
