using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class GeneReq {
	public enum Identifier {
		StatReq,
		LvlReq,
		ConsumeReq,
	};

	public Identifier id;
	public Stat.Identifier statId;
	public int amount;
	public Item item;
	public bool fulfilled = false;
	public int fulfilledAmount = 0;
}

