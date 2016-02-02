using UnityEngine;
using System;
using System.Collections;

public class Selector : Composite {

	protected int _selector;

	public Selector() {
		Update = () => {
			for(;;) {
				Status s = GetChild(_selector).Tick();
				if(s != Status.BhFailure){
					if(s == Status.BhSuccess)
						_selector = 0; // reset on a success
					return s;
				}

				if(++_selector == ChildCount) {
					_selector = 0; // reset on a failure
					return Status.BhFailure;
				}
			}
		};

		Initialize = () => { 
			_selector = 0; 
			Status = Status.BhInvalid;
		};
	}

	public override void Reset() {
		Initialize();
	}
}