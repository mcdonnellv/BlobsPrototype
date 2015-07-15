using UnityEngine;
using System.Collections;

public class MissionManager : MonoBehaviour 
{
	public GameManager gm;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	public void BreedingButtonPressed()
	{
		gm.breedingView.SetActive(true);
		gm.missionView.SetActive(false);
	}
}
