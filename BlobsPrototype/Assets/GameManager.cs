using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour 
{
	public MissionManager mm;
	public NurseryManager nm;
	public GameObject missionView;
	public GameObject breedingView;
	public List<Blob> blobs;
	public GameObject grid;
	public GameObject selectModeCover;
	public UILabel genderLabel;
	public UILabel colorLabel;
	public UILabel genotypeLabel;
	public UILabel countLabel;
	public UILabel qualityLabel;
	public UILabel averageQualityLabel;
	public UILabel goldLabel;
	public UILabel ageLabel;
	public UILabel breedButtonLabel;
	public UILabel yearLabel;
	public UILabel missionButtonLabel;
	public UISlider breedProgressBar;
	public UISlider yearProgressBar;
	public UIButton breedButton;
	public UIButton sellButton;
	public UIButton missionButton;
	public GameObject gameOverObject;
	public GameObject winnerObject;

	public float yearFillTime;
	public float yearProgress;
	public float breedBarFillTime;
	public float chanceForMutation;
	public int maxBlobs;
	public int gold;
	public int breedCost;
	public int breedBaseCost;
	public int breedCostMax;
	public int sellValue;
	public int curSelectedIndex;
	public int breedingAge;
	public int breedTimesPerYear;
	public int maxBreedcount;

	Blob recentFather;
	Blob recentMother;
	Blob recentBaby;
	bool selectMode;
	List<Blob> maleBlobs;
	List<Blob> femaleBlobs;
	int year;
	public float blobHatchTime;

	
	// Use this for initialization
	void Start ()
	{
		blobHatchTime = 3f;// * 60f;
		yearProgress = 0f;
		chanceForMutation = .8f;
		maxBlobs = 20;
		gold = 100;
		breedCost = 10;
		breedBaseCost = 10;
		breedCostMax = 100;
		sellValue = 3;
		curSelectedIndex = 0;
		breedingAge = 2;
		breedTimesPerYear = 4;
		maxBreedcount = 3;
		year = 0;
		selectMode = false;

		maleBlobs = new List<Blob>();
		femaleBlobs = new List<Blob>();
		blobs = new List<Blob>();

		breedingView.SetActive(true);
		missionView.SetActive(false);
		selectModeCover.SetActive(false);

		AddGold(0);

		breedProgressBar.value = 1f;
		yearProgressBar.value = 0f;

		yearFillTime = 30f;
		breedBarFillTime = yearFillTime / breedTimesPerYear;
	}


	public void EnableSelectMode(bool enable)
	{
		selectMode = enable;
		selectModeCover.SetActive(enable);
	}


	public void UpdateAverageQuality()
	{

		float cummulativeQuality = 0;

		// iterate thru blobs

		float averageQuality = cummulativeQuality / blobs.Count;
		averageQuality = Mathf.Round(averageQuality * 10f) / 10f;
		averageQualityLabel.text = "Average Quality: " + Blob.GetQualityStringFromValue(averageQuality);

		float mod = (float)Blob.GetQualityFromValue(averageQuality) - 1f;
		breedCost = breedBaseCost + (int)((mod / 4f) * (breedCostMax - breedBaseCost));
		breedButtonLabel.text = "Breed (" + breedCost.ToString() + "g)";
	}


	public void  AddGold(int val)
	{
		gold += val;
	}

	void AgeAllBlobs()
	{
		foreach(Blob blob in nm.blobs)
		{
			blob.age++;
			blob.breededThisYear = false;
		}

		nm.UpdateAllBlobCells();
		nm.infoPanel.UpdateWithBlob(nm.blobs[nm.curSelectedIndex]);
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
	

	// Update is called once per frame
	void Update () 
	{
		if(yearProgress < 1f)
		{
			yearProgress += Time.deltaTime * (1f / yearFillTime);

			if(yearProgress >= 1f)
			{
				yearProgress = 0f;
				year++;
				AgeAllBlobs();
			}
		}
	}
}
