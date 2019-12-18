using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour {
    readonly GameObject[] planes = new GameObject[16];
    readonly GameObject[] planesb = new GameObject[11];
    bool isSceneMoving = false;
    // Use this for initialization
    void Awake () {
        for(int i = 1; i <= 15; i++)
        {
            planes[i] = transform.FindChild("Plane" + i).gameObject;
        }
        for (int i = 1; i <= 10; i++)
        {
            planesb[i] = transform.FindChild("Planeb" + i).gameObject;
        }
    }
	
	// Update is called once per frame
	void Update () {
        for(int i = 1; i <= 15; i++)
        {
            if (planes[i].transform.localPosition.x <= -30)
                planes[i].transform.localPosition += new Vector3(50f, 0, 0);
            else if (planes[i].transform.localPosition.x >= 30)
                planes[i].transform.localPosition += new Vector3(-50f, 0, 0);
        }
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
            StartCoroutine(AdaptiveRotate(Vector3.up));
            StartCoroutine(AdaptiveMove(Vector3.left));
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
            StartCoroutine(AdaptiveRotate(Vector3.down));
            StartCoroutine(AdaptiveMove(Vector3.right));
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
    private IEnumerator AdaptiveRotate(Vector3 axis)
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
    private IEnumerator AdaptiveMove(Vector3 direction)
    {
        var target = 5f;
        var target2 = 2f;
        while (target > 0.1f)
        {
            target /= 2;
            target2 /= 2;
            for(int i = 1; i <= 15; i++)
            {
                planes[i].transform.localPositionTo(0.05f, new Vector3(planes[i].transform.localPosition.x + direction.x * target, planes[i].transform.localPosition.y, planes[i].transform.localPosition.z));
            }
            for (int i = 1; i <= 10; i++)
            {
                planesb[i].transform.localPositionTo(0.05f, new Vector3(planesb[i].transform.localPosition.x + direction.x * target2, planesb[i].transform.localPosition.y, planesb[i].transform.localPosition.z));
            }
            yield return new WaitForSeconds(0.05f);
        }
        for (int i = 1; i <= 15; i++)
        {
            planes[i].transform.localPositionTo(0.05f, new Vector3(planes[i].transform.localPosition.x + direction.x * target, planes[i].transform.localPosition.y, planes[i].transform.localPosition.z));
        }
        for (int i = 1; i <= 10; i++)
        {
            planesb[i].transform.localPositionTo(0.05f, new Vector3(planesb[i].transform.localPosition.x + direction.x * target2, planesb[i].transform.localPosition.y, planesb[i].transform.localPosition.z));
        }
        //isSceneMoving = false;
        //yield return null;
        yield return null;
    }
}
