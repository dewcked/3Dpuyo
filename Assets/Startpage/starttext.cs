using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class starttext : MonoBehaviour {
    public GameObject readText;
    // Use this for initialization
    void Start()
    {
        readText.SetActive(false);
        StartCoroutine(ShowReady());
    }

    IEnumerator ShowReady()
    {
        int count = 0;
        while (true)
        {
            readText.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            readText.SetActive(false);
            yield return new WaitForSeconds(0.5f);
            count++;
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
}
