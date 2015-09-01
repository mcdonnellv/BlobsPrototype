using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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

	public void AddGold(int val)
	{
		gold += val;
	}
	
	
	public void AddChocolate(int val)
	{
		chocolate += val;
	}
}