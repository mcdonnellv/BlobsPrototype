using UnityEngine;
using System;
using System.Collections;

public class Decorator : Composite {
	
	public Func<bool> CanRun { protected get; set; }
	public Status ReturnStatus { protected get; set; }
	public Decorator() {
		Update = () => {
			if(CanRun != null && CanRun() && Children!= null && Children.Count > 0)
				return Children[0].Tick();
			return ReturnStatus;
		};
	}

	public void Add(Behavior behavior) { Children.Add(behavior); }
}
