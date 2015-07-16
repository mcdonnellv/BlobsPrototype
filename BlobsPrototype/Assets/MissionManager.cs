using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class MissionManager : MonoBehaviour 
{
	public GameManager gm;
	public List<Mission> missions;
	public GameObject missionPanel;
	public UISlider progressBar;

	int curMissionIndex = 0;
	int maxMissions = 6;
	float missionSpawnTime = 1f;//60f * 3f;
	float missionSpawnTimer = 0f;

	// Use this for initialization
	void Start () 
	{
		missions = new List<Mission>();
	}


	void SpawnMission()
	{
		Mission mission = new Mission();
		mission.SetRandomMissionValues();
		missions.Add(mission);
		updateMissionList();
	}


	MissionCell GetMissionCell(Mission mission)
	{
		int index = missions.IndexOf(mission);
		GameObject cell = missionPanel.transform.GetChild(index).gameObject;
		return cell.GetComponent<MissionCell>();
	}


	void updateMissionList()
	{
		foreach(Transform missionCell in missionPanel.transform)
		{
			GameObject cell = missionCell.gameObject;

			if (missionCell.GetSiblingIndex() < missions.Count)
			{
				cell.gameObject.SetActive(true);
				Mission mission = missions[missionCell.GetSiblingIndex()];
				MissionCell mc = cell.GetComponent<MissionCell>();
				int timeMin = (int)((mission.duration - mission.durationCounting) / 60f);
				int timeSec = (int)((mission.duration - mission.durationCounting) % 60f);
				mc.durationLabel.text = ((timeMin > 0) ? timeMin.ToString() + "m " : "") + ((mission.active) ? (timeSec.ToString() + "s") : "");
				mc.rewardLabel.text = "Reward: " + ((int)mission.reward).ToString() + "g";
				mc.requirementLabel.text = "Requirements: none";

				if(mission.blob == null)
				{
					mc.blobContainer.SetActive(false);
					mc.startButton.isEnabled = false;
					UISprite bg = mc.blobButton.GetComponent<UISprite>();
					bg.color = new Color(0.584f, 0.471f, 0.349f, 1f);
					mc.blobButton.defaultColor = bg.color;
					mc.blobButton.hover = bg.color;
				}
				else
				{
					mc.blobContainer.SetActive(true);
					mc.startButton.isEnabled = true;

					UISprite[] sprites = cell.GetComponentsInChildren<UISprite>();
					sprites[1].color = Blob.GetColorFromEnum(mission.blob.color);
					sprites[2].color = Color.white;
					sprites[3].enabled = !mission.blob.male;
					UISprite bg = mc.blobButton.GetComponent<UISprite>();
					bg.color = (mission.blob.male) ? new Color(0.62f, 0.714f, 0.941f,1f) : new Color(0.933f, 0.604f, 0.604f, 1f);
					mc.blobButton.defaultColor = bg.color;
					mc.blobButton.hover = bg.color;
				}
			}
			else
			{
				cell.gameObject.SetActive(false);
			}
		}
	}


	public void PressSelectButton()
	{
		Mission mission = missions[curMissionIndex];
		mission.blob = gm.blobs[gm.curSelectedIndex];
		mission.blob.onMission = true;
		gm.EnableSelectMode(false);
		gm.breedingView.SetActive(false);
		gm.missionView.SetActive(true);
		updateMissionList();
	}


	public void PressMissionItemBlob(int index)
	{
		curMissionIndex = index;
		Mission mission = missions[index];

		if (mission.blob == null)
		{
			gm.EnableSelectMode(true);
			gm.breedingView.SetActive(true);
			gm.missionView.SetActive(false);
			gm.UpdateGrid();
		}
		else
		{
			mission.blob.onMission = false;
			mission.blob = null;
			updateMissionList();
		}
	}

	
	public void PressMissionItemStart(int index)
	{
		Mission mission = missions[index];

		if(!mission.active) //start
		{
			mission.active = true;
			
			MissionCell mc = GetMissionCell(mission);
			UILabel startButtonLabel = mc.startButton.GetComponentInChildren<UILabel>();
			startButtonLabel.text = "";
			mc.startButton.isEnabled = false;
			mc.blobButton.isEnabled = false;
		}
		else //collect
		{
			mission.blob.onMission = false;
			if(mission.successful)
				gm.AddGold(mission.reward);
			missions.Remove(mission);
			updateMissionList();
		}
	}


	void MissionReady(Mission mission)
	{
		MissionCell mc = GetMissionCell(mission);
		UILabel startButtonLabel = mc.startButton.GetComponentInChildren<UILabel>();

		float missionResult = Random.Range(0f,1f);
		float successChance = mission.GetSuccessChance(mission.blob);
		if (missionResult > successChance)
			mission.successful = false;

		if(mission.successful)
			startButtonLabel.text = "Collect";
		else
			startButtonLabel.text = "Failed";

		mc.startButton.isEnabled = true;
	}


	public void BreedingButtonPressed()
	{
		gm.breedingView.SetActive(true);
		gm.missionView.SetActive(false);
		gm.UpdateGrid();
	}


	// Update is called once per frame
	void Update () 
	{
		if (missions.Count < maxMissions)
		{
			missionSpawnTimer -= Time.deltaTime;
			progressBar.value = 1f - (missionSpawnTimer / missionSpawnTime);
			if (missionSpawnTimer <= 0f)
			{
				missionSpawnTimer = missionSpawnTime;
				SpawnMission();
			}
		}

		foreach(Mission mission in missions)
		{
			if (!mission.active)
				continue;

			mission.durationCounting += Time.deltaTime;
			MissionCell mc = GetMissionCell(mission);
			mc.slider.value = 1f - (mission.durationCounting / mission.duration);
			int timeMin = (int)((mission.duration - mission.durationCounting) / 60f);
			int timeSec = (int)((mission.duration - mission.durationCounting) % 60f);
			mc.durationLabel.text = ((timeMin > 0) ? timeMin.ToString() + "m " : "") + ((mission.active) ? (timeSec.ToString() + "s") : "");

			if (mission.durationCounting >= mission.duration)
			{
				mission.durationCounting = mission.duration;
				MissionReady(mission);
			}
		}
	}


}
