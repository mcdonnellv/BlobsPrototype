using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class MissionManager : MonoBehaviour 
{
	public GameManager gm;
	public List<Mission> missions;
	public GameObject missionPanel;
	public UISlider progressBar;
	public float missionSpawnTimeMins = 3f;

	int curMissionIndex = 0;
	int maxMissions = 6;
	public float missionSpawnTimer = 0f;

	// Use this for initialization
	void Start () 
	{
		missions = new List<Mission>();

		Mission mission = new Mission();
		mission.duration = 2f * 60f;
		mission.reward = 10;
		mission.ageRequirement = 1;
		missions.Add(mission);

		missionSpawnTimer = missionSpawnTimeMins * 60f;
		UpdateMissionList();
		gm.missionButtonLabel.text = "Missions (" + missions.Count.ToString() + ")";
	}


	void SpawnMission()
	{
		Mission mission = new Mission();
		mission.SetRandomMissionValues();
		missions.Add(mission);
		UpdateMissionList();
		gm.missionButtonLabel.text = "Missions (" + missions.Count.ToString() + ")";
	}


	MissionCell GetMissionCell(Mission mission)
	{
		int index = missions.IndexOf(mission);
		GameObject cell = missionPanel.transform.GetChild(index).gameObject;
		return cell.GetComponent<MissionCell>();
	}


	public void UpdateMissionList()
	{
		if (gm.missionView.activeSelf == false)
			return;

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
				string requirements = "";
				if (mission.ageRequirement > 0)
					requirements = requirements + "Age: " + mission.ageRequirement.ToString();
				mc.requirementLabel.text = requirements;
				mc.chanceLabel.text = "";

				if(mission.blob == null)
				{
					mc.blobContainer.SetActive(false);
					mc.startButton.isEnabled = false;
					mc.blobButton.isEnabled = true;
					UISprite bg = mc.blobButton.GetComponent<UISprite>();
					bg.color = new Color(0.584f, 0.471f, 0.349f, 1f);
					mc.blobButton.defaultColor = bg.color;
					mc.blobButton.hover = bg.color;
					mc.chanceLabel.text = "";
					UILabel startButtonLabel = mc.startButton.GetComponentInChildren<UILabel>();
					startButtonLabel.text = "Start";
				}
				else
				{
					mc.blobContainer.SetActive(true);
					mc.startButton.isEnabled = true;

					UISprite[] sprites = cell.GetComponentsInChildren<UISprite>();
					sprites[1].color = Blob.GetColorFromEnum(mission.blob.color);
					sprites[2].color = Color.white;
					sprites[3].enabled = !mission.blob.male;

					float a = (float)(mission.blob.age > 3 ? 3 : mission.blob.age);
					float s = .3f + (.7f * (a / 3f));
					if(s > 1f)
						s = 1f;
					int pixels = (int)(s * 50f);
					sprites[1].SetDimensions(pixels, pixels);
					sprites[2].SetDimensions(pixels, pixels);
					sprites[3].SetDimensions(pixels, pixels);

					UISprite bg = mc.blobButton.GetComponent<UISprite>();
					bg.color = (mission.blob.male) ? new Color(0.62f, 0.714f, 0.941f,1f) : new Color(0.933f, 0.604f, 0.604f, 1f);
					mc.blobButton.defaultColor = bg.color;
					mc.blobButton.hover = bg.color;

					float successChance = mission.GetSuccessChance(mission.blob);
					mc.chanceLabel.text = (successChance * 100f).ToString() + "%\n Chance";
					if (successChance >= 1f)
						mc.chanceLabel.color = Color.green;
					else if(successChance >= .5f)
						mc.chanceLabel.color = Color.yellow;
					else
						mc.chanceLabel.color = Color.red;

					if(mission.active == true && mission.durationCounting == mission.duration)
					{
						mc.startButton.isEnabled = true;
						mc.startButton.GetComponent<UISprite>().depth = 8;
						UILabel startButtonLabel = mc.startButton.GetComponentInChildren<UILabel>();
						if(mission.successful)
							startButtonLabel.text = "Collect";
						else
							startButtonLabel.text = "Failed";
					}
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
		UpdateMissionList();
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
			UpdateMissionList();
		}
	}

	
	public void PressMissionItemStart(int index)
	{
		Mission mission = missions[index];

		if (mission.active)
		{
			mission.active = false;
			mission.blob.onMission = false;
			if(mission.successful)
				gm.AddGold(mission.reward);
			missions.Remove(mission);
			UpdateMissionList();
			gm.missionButtonLabel.text = "Missions (" + missions.Count.ToString() + ")";
		}
		else
		{
			mission.active = true;
			MissionCell mc = GetMissionCell(mission);
			mc.startButton.isEnabled = false;
			mc.blobButton.isEnabled = false;
			mc.startButton.GetComponent<UISprite>().depth = 0;

		}
	}


	void MissionReady(Mission mission)
	{
		MissionCell mc = GetMissionCell(mission);
		float missionResult = Random.Range(0f,1f);
		float successChance = mission.GetSuccessChance(mission.blob);
		if (missionResult > successChance)
			mission.successful = false;
		UpdateMissionList();
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
		if (missions != null && missions.Count < maxMissions)
		{
			missionSpawnTimer -= Time.deltaTime;
			progressBar.value = 1f - (missionSpawnTimer / (missionSpawnTimeMins * 60f));
			if (missionSpawnTimer <= 0f)
			{
				missionSpawnTimer = missionSpawnTimeMins * 60;
				SpawnMission();
			}
		}

		foreach(Mission mission in missions)
		{
			if (!mission.active)
				continue;

			if (mission.durationCounting == mission.duration)
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
