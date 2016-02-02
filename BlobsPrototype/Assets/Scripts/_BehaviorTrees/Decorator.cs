using UnityEngine;
using System;
using System.Collections;

public class Decorator : Composite {
	
	public Func<bool> _canRun { protected get; set; }
	public Status ReturnStatus { protected get; set; }

	public Decorator() {
		ReturnStatus = Status.BhFailure;
		Update = () => {
			if(_canRun != null && _canRun() && Children!= null && Children.Count > 0)
				return Children[0].Tick();
			return ReturnStatus;
		};
	}

	public Decorator CanRun(Func<bool> canRun) {
		_canRun = canRun;
		return this;
	}
}
