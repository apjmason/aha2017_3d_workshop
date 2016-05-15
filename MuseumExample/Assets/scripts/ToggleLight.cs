using UnityEngine;
using System.Collections;

public class ToggleLight : MonoBehaviour {

	public void Toggle()
    {
        Light light = GameObject.FindGameObjectWithTag("Light").GetComponent<Light>();
        light.enabled = !light.enabled;
    }
}
