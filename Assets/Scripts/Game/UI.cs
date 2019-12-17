using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour {


    [SerializeField]
    private GameObject pillarPrefab;
    [SerializeField]
    private GameObject bottomPrefab;

    private GameObject[] pillars = new GameObject[4];
    private GameObject bottom;
    // Use this for initialization
    void Start () {
        pillars[0] = Instantiate(pillarPrefab, new Vector3(-0.5f, -0.2f, -0.5f), Quaternion.identity) as GameObject;
        pillars[1] = Instantiate(pillarPrefab, new Vector3(2.5f, -0.2f, -0.5f), Quaternion.identity) as GameObject;
        pillars[2] = Instantiate(pillarPrefab, new Vector3(2.5f, -0.2f, 2.5f), Quaternion.identity) as GameObject;
        pillars[3] = Instantiate(pillarPrefab, new Vector3(-0.5f, -0.2f, 2.5f), Quaternion.identity) as GameObject;
        for (int i = 0; i < pillars.Length; i++) {
            pillars[i].transform.localScale = new Vector3(0.02f, 0.04f, 0.02f);
        }

        bottom = Instantiate(bottomPrefab, new Vector3(-1.5f, -0.3f, -1.5f), Quaternion.identity) as GameObject;
        bottom.transform.localScale = new Vector3(125f, 125f, 125f);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
