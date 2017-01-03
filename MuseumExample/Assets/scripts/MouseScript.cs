using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseScript : MonoBehaviour {

	void Start () {
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	void Update()
	{
		var fwd = transform.TransformDirection (Vector3.forward);
		RaycastHit hit;
		Debug.DrawRay(transform.position, fwd * 2, Color.green);

		if (Physics.Raycast (transform.position, fwd, out hit, 10)) {  // Perform Raycast
			
			if (hit.collider.tag == "Artifact") {  // check tag
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
			}


		}
	}


	//	// press Escape to toggle the cursor on and off
//	void Start()
//	{
//		Cursor.lockState = CursorLockMode.Locked;
//		Cursor.visible = false;
//	}
//
//	void Update()
//	{
//		Cursor.lockState = CursorLockMode.Locked;
//		Cursor.visible = false;
//	}

}