using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class MouseRelease : MonoBehaviour {

	void OnTriggerEnter(Collider other) {
		GameObject.Find ("FPSController").GetComponent<FirstPersonController> ().MouseLook.LockCursor = false;
	}
}
