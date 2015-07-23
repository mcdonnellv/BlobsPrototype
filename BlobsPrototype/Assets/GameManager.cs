using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour 
{
	public MissionManager mm;
	public NurseryManager nm;
	public VillageManager vm;
	public CastleManager cm;
	public GameObject missionView;
	public GameObject breedingView;
	public List<Blob> blobs;
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
	public GameObject gameOverObject;
	public GameObject winnerObject;
	public UICamera gameCam;
	public float blobHatchDelay;
	public float breedReadyDelay;
	public float yearFillTime;
	public float yearFillDelay;
	public float chanceForMutation;
	public float breedBarFillTime;
	public float tributeGoldPerQuality;
	public float tributeMaxMulitplier;
	public float blobGoldProductionSpeed;
	public float t;
	public int maxBlobs;
	public int gold;
	public int breedCost;
	public int breedBaseCost;
	public int breedCostMax;
	public int sellValue;
	public int breedingAge;
	public int maxBreedcount;
	public int year;


	bool selectMode;


	void Start ()
	{
		t = .1f;

		blobHatchDelay = 40f * t;
		breedReadyDelay = 10f * t;
		breedBarFillTime = 1f * t;
		blobGoldProductionSpeed = 10f;

		year = 0;
		yearFillDelay = 90f * t;
		yearFillTime = Time.time + yearFillDelay;

		chanceForMutation = .8f;
		maxBlobs = 20;
		gold = 100;
		breedCost = 10;
		breedBaseCost = 10;
		breedCostMax = 100;
		sellValue = 3;
		breedingAge = 1;
		maxBreedcount = 3;
		tributeGoldPerQuality = 2f;
		tributeMaxMulitplier = 5f;

		selectMode = false;

		breedingView.SetActive(true);
		missionView.SetActive(false);
		selectModeCover.SetActive(false);

		AddGold(0);

		yearProgressBar.value = 0f;

		Vector3 pos = gameCam.transform.localPosition;
		pos.x = 740f * 2;
		gameCam.transform.localPosition = new Vector3(pos.x, pos.y);
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
		gold += val;
		goldLabel.text = "Gold: " + gold.ToString();
	}

	void AgeAllBlobs()
	{
		if (nm.blobs != null)
		{
			foreach(Blob blob in nm.blobs)
			{
				blob.age++;
				blob.breededThisYear = false;
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



	void CheckGameOver()
	{
		if (nm.MatingPairExists() == false)
			gameOverObject.SetActive(true);

		bool hasBlue = false;
		bool hasRed = false;
		bool hasYellow = false;
		bool hasGreen = false;
		bool hasPurple = false;
		bool hasOrange = false;

		foreach(Blob blob in blobs)
		{
			hasBlue = blob.color == BlobColor.Blue;
			hasRed = blob.color == BlobColor.Red;
			hasYellow = blob.color == BlobColor.Yellow;
			hasGreen  = blob.color == BlobColor.Green;
			hasPurple = blob.color == BlobColor.Purple;
			hasOrange = blob.color == BlobColor.Orange;
		}

		if (hasBlue && hasRed && hasYellow && hasOrange && hasGreen && hasPurple)
		{
			winnerObject.SetActive(true);
		}
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
		if (pos.x <= sw*1)
			leftNavButton.gameObject.SetActive(false);

		gameCam.transform.localPosition = new Vector3(pos.x, pos.y);
	}


	// Update is called once per frame
	void Update () 
	{
		if (yearFillTime > 0f)
		{
			yearProgressBar.value = 1f - ((yearFillTime - Time.time) / yearFillDelay);
			yearProgressBar.value = yearProgressBar.value > 1f ? 1f : yearProgressBar.value;
			
			if (yearFillTime < Time.time)
			{
				year++;
				yearLabel.text = "Year: " + year.ToString();
				AgeAllBlobs();
				yearFillTime = yearFillDelay + Time.time;
			}
		}
	}
}
