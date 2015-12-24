using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class LootEntry {
	public int quantity = 1;
	public int itemId = -1;
	public int probability;
}