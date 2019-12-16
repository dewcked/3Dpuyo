using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour {


    [SerializeField]
    private GameObject uiPrefabs;

    private GameObject[] pillars = new GameObject[4];
    // Use this for initialization
    void Start () {
        pillars[0] = Instantiate(uiPrefabs, new Vector3(-0.5f, 0f, -0.5f), Quaternion.identity) as GameObject;
        pillars[1] = Instantiate(uiPrefabs, new Vector3(2.5f, 0f, -0.5f), Quaternion.identity) as GameObject;
        pillars[2] = Instantiate(uiPrefabs, new Vector3(2.5f, 0f, 2.5f), Quaternion.identity) as GameObject;
        pillars[3] = Instantiate(uiPrefabs, new Vector3(-0.5f, 0f, 2.5f), Quaternion.identity) as GameObject;
        for (int i = 0; i < pillars.Length; i++) {
            pillars[i].transform.localScale = new Vector3(0.02f, 0.04f, 0.02f);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
