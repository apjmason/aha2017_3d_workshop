using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PieceInfoDisplay : MonoBehaviour{

    Text text;

	// Use this for initialization
	void Start () {
        text = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void ShowText()
    {
        GameObject textMesh = GameObject.FindGameObjectWithTag("TextDisplay");
        textMesh.GetComponent<TextMesh>().text = text.text;
        textMesh.transform.position = transform.position + new Vector3(0f, 0.5f, 0f);
        MeshRenderer render = textMesh.GetComponentInChildren<MeshRenderer>();
        Debug.Log(render.enabled);
        render.enabled = true;
    }

    public void hideText()
    {
        GameObject textMesh = GameObject.FindGameObjectWithTag("TextDisplay");
        MeshRenderer render = textMesh.GetComponentInChildren<MeshRenderer>();
        render.enabled = false;
    }

}
