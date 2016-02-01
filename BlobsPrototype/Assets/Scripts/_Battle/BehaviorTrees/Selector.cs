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
						_selector = 0;
					return s;
				}

				if(++_selector == ChildCount) {
					_selector = 0;
					return Status.BhFailure;
				}
			}
		};

		Initialize = () => { _selector = 0; };
	}
}


public class PrioritySelector : Selector {

	private int _lastSelector;

	public PrioritySelector() {
		Update = () => {
			_selector = 0;
			for(;;) {
				Status s = GetChild(_selector).Tick();
				if(s != Status.BhFailure){
					for(int i = _selector; i <= _lastSelector; i++) {
						GetChild(i).Reset();
					}
				}
			}
		}; 
		
		Initialize = () => { _lastSelector = 0; };
	}

}