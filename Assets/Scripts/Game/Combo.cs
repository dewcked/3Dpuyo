using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combo : MonoBehaviour {

    public GameObject manager;
    private const int COMBO_LENGTH = 7;

    private int lastCombo;
    GameObject[] comboList;
    GameObject currentComboObj;

	// Use this for initialization
	void Start () {
        lastCombo = 0;
        comboList = new GameObject[COMBO_LENGTH];
        for (int i = 0; i < COMBO_LENGTH; i++) {
            comboList[i] = transform.GetChild(i).gameObject;
            comboList[i].transform.position = new Vector3(8f, 5f, 0f);
        }

        currentComboObj = comboList[0];
    }
	
	// Update is called once per frame
	void Update () {
		if (GameVariable.currentCombo == 0) {
            currentComboObj.SetActive(false);
        }
        else {
            currentComboObj.SetActive(false);
            getComboText(GameVariable.currentCombo - 1);
            displayComboText();
        }
	}

    GameObject getComboText(int combo) {
        if (combo < 7) {
            currentComboObj = transform.GetChild(combo).gameObject;
        }
        else {
            currentComboObj = transform.GetChild(6).gameObject;
        }

        return currentComboObj;
    }

    void displayComboText() {
        currentComboObj.SetActive(true);
    }
}
