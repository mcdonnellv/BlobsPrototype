using UnityEngine;
using System;

[Serializable]
public class GeneActivationRequirement {
	public int amountNeeded;
	public int amountConsumed;
	public BaseItem item;
	public bool fulfilled { get{return amountConsumed >= amountNeeded;} }

	public GeneActivationRequirement() {}

	public GeneActivationRequirement(GeneActivationRequirement r) {
		amountNeeded = r.amountNeeded;
		amountConsumed = 0;
		item = r.item;
	}
}
