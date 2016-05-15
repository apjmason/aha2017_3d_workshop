/*****************************************************************\
* =============================================================== *
* Author: Keenan Cole
* Date : 6/18/2012
*
* Purpose : If in rotate mode, use the Mouse X and Y axis to 
*           change the rotation of the object.
*
*
* =============================================================== *
\*****************************************************************/

//Global variables
//  You can adjust these in the Inspector
var xSpeed = 250.0;
var ySpeed = 120.0;


// Private variables
private var startRot : Quaternion;
private var x = 0.0;
private var y = 0.0;
private var canRotate : boolean = false;


// Store the initial rotation. We'll use this 
// to reset the rotation when we exit GUI mode
function Start()
{
	startRot = transform.rotation;	
	
}

// Toggle off or on rotate mode. 
// Set the rotation back to its initial value
// whenever we do so
function toggleControl(b : boolean)
{
	canRotate = b;
	transform.rotation = startRot;
	
}
// LateUpdate function is part of the UnityAPI. 
//  It is called every frame, but only after the 
//  Update is called. 

// If Rotate Mode is enabled, and LMB is pressed
//   use values from Mouse X and Y axis to 
//   change the rotation of the object
function LateUpdate () {
    if (canRotate) {
    	
    	if(Input.GetButton("Fire1"))
    	{
    	    x -= Input.GetAxis("Mouse X") * xSpeed * 0.02;
       		y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02;
 		
 		       
     	    var rotation = Quaternion.Euler(y, x, 0);
        
      		transform.rotation = rotation;
    	}
    }
}