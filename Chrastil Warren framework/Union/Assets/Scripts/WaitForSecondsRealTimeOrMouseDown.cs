using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRStandardAssets.Utils;

// Ends WaitForSecondsRealtime on mouse press
public class WaitForSecondsRealTimeOrMouseDown : WaitForSecondsRealtime {

	private bool hasDoubleClick;
	private VRInput myVRInput;

	public override bool keepWaiting {
		get { 
			return base.keepWaiting && !hasDoubleClick;
		}
	}

	public WaitForSecondsRealTimeOrMouseDown(float time, VRInput rhsVRInput) 
		: base(time) {
		hasDoubleClick = false;

		this.myVRInput = rhsVRInput;
		this.myVRInput.OnDoubleClick += setDoubleClick;
	}

	private void setDoubleClick() {
		Debug.Log ("received doubleclick");
		hasDoubleClick = true;
		myVRInput.OnDoubleClick -= setDoubleClick;
	}
}
