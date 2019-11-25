using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        SceneChange();
        AdaptChange();
        //this.transform.RotateAround(new Vector3(1f, 1f, 1f), Vector3.left, 30 * Time.deltaTime);
    }

    /// <summary>
    /// 장면 전환 메서드
    /// </summary>
    void SceneChange()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            transform.RotateAround(new Vector3(1f, 1f, 1f), Vector3.up, 90f);
            switch (GameVariable.Scene)
            {
                case Control.Screen1:
                    GameVariable.Scene = Control.Screen4;
                    break;
                case Control.Screen2:
                    GameVariable.Scene = Control.Screen1;
                    break;
                case Control.Screen3:
                    GameVariable.Scene = Control.Screen2;
                    break;
                case Control.Screen4:
                    GameVariable.Scene = Control.Screen3;
                    break;
            }
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            transform.RotateAround(new Vector3(1f, 1f, 1f), Vector3.down, 90f);
            switch (GameVariable.Scene)
            {
                case Control.Screen1:
                    GameVariable.Scene = Control.Screen2;
                    break;
                case Control.Screen2:
                    GameVariable.Scene = Control.Screen3;
                    break;
                case Control.Screen3:
                    GameVariable.Scene = Control.Screen4;
                    break;
                case Control.Screen4:
                    GameVariable.Scene = Control.Screen1;
                    break;
            }
        }
    }

    void AdaptChange()
    {

    }
}
