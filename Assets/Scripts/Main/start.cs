using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        SceneManager.LoadScene("Gameplay");
    }
}
