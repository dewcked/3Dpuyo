﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class End : MonoBehaviour {

    public void onclickrestart()
    {
        SceneManager.LoadScene("Gameplay");
    }

    public void onclickending()
    {
        Application.Quit();
    }
}
