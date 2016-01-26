using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public interface IGeneFunctionality {
	bool CanExistWithWith(TraitType t, Gene g);
	void OnBirth(Blob blob, Gene g);
	void OnAdd(Blob blob, Gene gene);
	void OnCombatStart();
	void OnRemove(Blob blob, Gene g);
}
