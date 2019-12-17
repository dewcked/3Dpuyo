using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour {

    GameObject _plane1;
    GameObject _plane2;
    bool isSceneMoving = false;
    // Use this for initialization
    void Awake () {
        _plane1 = transform.FindChild("Plane").gameObject;
        _plane2 = transform.FindChild("Plane2").gameObject;
    }
	
	// Update is called once per frame
	void Update () {
        switch (isSceneMoving)
        {
            case true:
                break;
            case false:
                SceneChange();
                break;
        }
    }

    /// <summary>
    /// 장면 전환 메서드
    /// </summary>
    void SceneChange()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            isSceneMoving = true;
            StartCoroutine(AdaptiveChange(Vector3.up));
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
        else if (Input.GetKey(KeyCode.E))
        {
            isSceneMoving = true;
            StartCoroutine(AdaptiveChange(Vector3.down));
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
    //1, 1, 1을 기준으로 axis를 축으로 앵글이 점차적으로 줄어들면서 무한급수 수렴하도록 회전하는 함수
    private IEnumerator AdaptiveChange(Vector3 axis)
    {
        var leftangle = 90f;
        while (leftangle > 0.5f)
        {
            leftangle /= 2;
            transform.RotateAround(new Vector3(1f, 1f, 1f), axis, leftangle);
            yield return new WaitForSeconds(0.05f);
        }
        transform.RotateAround(new Vector3(1f, 1f, 1f), axis, leftangle);
        isSceneMoving = false;
        yield return null;
    }
}
