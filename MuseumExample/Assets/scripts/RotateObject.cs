using UnityEngine;
using System.Collections;

public class RotateObject : MonoBehaviour {

    bool rotating = false;
    Quaternion origRotation;
    Vector3 origPosition;
    Vector3 origScale;

	// Use this for initialization
	void Start () {
        origPosition = transform.position;
        origRotation = transform.rotation;
        origScale = transform.localScale;
	}
	
	// Update is called once per frame
	void Update () {
	    if (rotating)
        {
            this.transform.Rotate(Vector3.up, 1, Space.World);
        }
	}

    private void StartRotation()
    {
        transform.localScale = origScale * 2;
        Vector3 translation = new Vector3(0f, 0.25f, 0f);
        this.transform.position = origPosition + translation;
        rotating = true;
    }

    private void StopRotation()
    {
        this.transform.position = origPosition;
        this.transform.rotation = origRotation;
        this.transform.localScale = origScale;
        
    }

    public void ToggleRotation()
    {
        if (rotating)
        {
            rotating = false;
            StopRotation();
        }
        else
        {
            StartRotation();
        }
    }
}
