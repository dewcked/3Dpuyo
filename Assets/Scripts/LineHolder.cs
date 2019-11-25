using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineHolder : MonoBehaviour {

    [SerializeField]
    private GameObject LineGeneratorPrefab;

	// Use this for initialization
	void Start () {
        DrawGuideLine();
	}
    void DrawGuideLine()
    {
        for (int i = 0; i <= GameVariable.Rows; i++)
            for (int j = 0; j <= GameVariable.ColumnsA; j++)
            {
                Debug.Log("not that easy");
                GameObject newline = Instantiate(LineGeneratorPrefab);
                LineRenderer lRend = newline.GetComponent<LineRenderer>();

                lRend.SetPosition(0, new Vector3(j - 0.5f, i - 0.5f, 0 - 0.5f));
                lRend.SetPosition(1, new Vector3(j - 0.5f, i - 0.5f, GameVariable.ColumnsB - 0.5f));
                lRend.SetPosition(0, new Vector3(j - 0.5f, i - 0.5f, 0 - 0.5f));

            }
        for (int i = 0; i <= GameVariable.ColumnsB; i++)
            for (int j = 0; j <= GameVariable.ColumnsA; j++)
            {
                GameObject newline = Instantiate(LineGeneratorPrefab);
                LineRenderer lRend = newline.GetComponent<LineRenderer>();

                lRend.SetPosition(0, new Vector3(j - 0.5f, 0 - 0.5f, i - 0.5f));
                lRend.SetPosition(1, new Vector3(j - 0.5f, GameVariable.Rows - 0.5f, i - 0.5f));
                lRend.SetPosition(0, new Vector3(j - 0.5f, 0 - 0.5f, i - 0.5f));

            }
        for (int i = 0; i <= GameVariable.Rows; i++)
            for (int j = 0; j <= GameVariable.ColumnsB; j++)
            {
                GameObject newline = Instantiate(LineGeneratorPrefab);
                LineRenderer lRend = newline.GetComponent<LineRenderer>();

                lRend.SetPosition(0, new Vector3(0 - 0.5f, i - 0.5f, j - 0.5f));
                lRend.SetPosition(1, new Vector3(GameVariable.ColumnsA - 0.5f, i - 0.5f, j - 0.5f));
                lRend.SetPosition(0, new Vector3(0 - 0.5f, i - 0.5f, j - 0.5f));
            }
    }
    // Update is called once per frame
    void Update () {
		
	}
}
