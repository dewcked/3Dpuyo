using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class start : MonoBehaviour {
    AudioSource audio;
	// Use this for initialization

	void Start () {
        audio = GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.anyKeyDown)
        {
            audio.Play();
            Invoke("nextScean", 1);
        }

	}
    void nextScean()
    {
        Application.LoadLevel("Gameplay");
    }
}
