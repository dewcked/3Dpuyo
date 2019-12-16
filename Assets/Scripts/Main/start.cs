using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class start : MonoBehaviour {
#pragma warning disable CS0108 // 'start.audio' hides inherited member 'Component.audio'. Use the new keyword if hiding was intended.
    AudioSource audio;
#pragma warning restore CS0108 // 'start.audio' hides inherited member 'Component.audio'. Use the new keyword if hiding was intended.
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
#pragma warning disable CS0618 // 'Application.LoadLevel(string)' is obsolete: 'Use SceneManager.LoadScene'
        Application.LoadLevel("Gameplay");
#pragma warning restore CS0618 // 'Application.LoadLevel(string)' is obsolete: 'Use SceneManager.LoadScene'
    }
}
