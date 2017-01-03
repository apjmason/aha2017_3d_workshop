using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PieceInfoDisplay : MonoBehaviour{

    Text text;
    Dictionary<char, float> dict;
    bool showingText;

	// Use this for initialization
	void Start () {
        text = GetComponent<Text>();
        dict = new Dictionary<char, float>();
        showingText = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    private void ShowText()
    {
        GameObject textObj = GameObject.FindGameObjectWithTag("TextDisplay");
        TextMesh textMesh = textObj.GetComponent<TextMesh>();
        MeshRenderer render = textObj.GetComponentInChildren<MeshRenderer>();
        textMesh.text = text.text;
        FitToWidth(textObj, textMesh, 30);

        Bounds sculptureBound = this.GetComponentInChildren<MeshRenderer>().bounds;
        Vector3 textPos = transform.position + new Vector3(sculptureBound.extents.x + 0.2f, render.bounds.extents.y, 0f);
        textObj.transform.position = textPos;
        render.enabled = true;
    }

    private void HideText()
    {
        showingText = false;
        GameObject textObj = GameObject.FindGameObjectWithTag("TextDisplay");
        MeshRenderer render = textObj.GetComponentInChildren<MeshRenderer>();
        render.enabled = false;
    }

    public void ToggleText()
    {
        if (showingText)
        {
            showingText = false;
            HideText();
        }
        else
        {
            showingText = true;
            ShowText();
        }
    }

    private void FitToWidth(GameObject textObj, TextMesh textMesh, float wantedWidth)
    {
        string oldText = textMesh.text;
        textMesh.text = "";

        string[] lines = oldText.Split('\n');

        foreach (string line in lines)
        {
            textMesh.text += wrapLine(textObj, textMesh, line, wantedWidth);
            textMesh.text += "\n";
        }
    }

    private string wrapLine(GameObject textObj, TextMesh textMesh, string s, float w)
    {
        // need to check if smaller than maximum character length, really...
        if (w == 0 || s.Length <= 0) return s;

        char c;
        char[] charList = s.ToCharArray();

        float charWidth = 0;
        float wordWidth = 0;
        float currentWidth = 0;

        string word = "";
        string newText = "";
        string oldText = textMesh.text;

        for (int i = 0; i < charList.Length; i++)
        {
            c = charList[i];

            if (dict.ContainsKey(c))
            {
                charWidth = (float)dict[c];
            }
            else
            {
                textMesh.text = "" + c;
                charWidth = textObj.GetComponent<Renderer>().bounds.size.x;
                dict.Add(c, charWidth);
                //here check if max char length
            }

            if (c == ' ' || i == charList.Length - 1)
            {
                if (c != ' ')
                {
                    word += c.ToString();
                    wordWidth += charWidth;
                }

                if (currentWidth + wordWidth < w)
                {
                    currentWidth += wordWidth;
                    newText += word;
                }
                else
                {
                    currentWidth = wordWidth;
                    newText += word.Replace(" ", "\n");
                }

                word = "";
                wordWidth = 0;
            }

            word += c.ToString();
            wordWidth += charWidth;
        }

        textMesh.text = oldText;
        return newText;
    }

}
