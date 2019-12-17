using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public PuyoManager PuyoManager;

    private float fallingSpeed = 0.3f;
    private float time = 0f;
    private float MoveKeytime = 0f;
    private float DownKeytime = 0.02f;
#pragma warning disable CS0414 // The field 'PuyoManager.RotateKeytime' is assigned but its value is never used
    private float RotateKeytime = 0.08f;
#pragma warning restore CS0414 // The field 'PuyoManager.RotateKeytime' is assigned but its value is never used

    private float MoveKeySpeed = 0.05f;
    private float DownKeySpeed = 0.02f;
#pragma warning disable CS0414 // The field 'PuyoManager.RotateKeySpeed' is assigned but its value is never used
    private float RotateKeySpeed = 0.08f;
#pragma warning restore CS0414 // The field 'PuyoManager.RotateKeySpeed' is assigned but its value is never used

#pragma warning disable CS0414 // The field 'PuyoManager.Delaytime' is assigned but its value is never used
    private float Delaytime = 0.8f;
#pragma warning restore CS0414 // The field 'PuyoManager.Delaytime' is assigned but its value is never used
#pragma warning disable CS0414 // The field 'PuyoManager.CurrentDelay' is assigned but its value is never used
    private float CurrentDelay = 0f;
#pragma warning restore CS0414 // The field 'PuyoManager.CurrentDelay' is assigned but its value is never used
    private bool firstMove = true;

    private GameState gameState;

    // Use this for initialization
    void Awake () {
        PuyoManager = transform.FindChild("PuyoManager").gameObject.GetComponent<PuyoManager>();
        gameState = GameState.Generate;
    }
	
	// Update is called once per frame
	void Update () {
		time += Time.deltaTime;
        MoveKeytime += Time.deltaTime;
        DownKeytime += Time.deltaTime;
        
        switch (gameState)
        {
            case GameState.Busy:
                //Debug.Log("bsy");
                break;
            case GameState.Generate:
                //Debug.Log("gen");
                gameState = GameState.Busy;
                Generate();
                break;
            case GameState.Spawning:
                //Debug.Log("spn");
                gameState = GameState.Busy;
                Spawning();
                break;
            case GameState.Falling:
                //Debug.Log("fal");
                gameState = GameState.Busy;
                Falling();
                break;
            case GameState.Fixing:
                //Debug.Log("fix");
                gameState = GameState.Busy;
                Fixing();
                break;
            case GameState.CheckAndDestroy:
                //Debug.Log("cad");
                gameState = GameState.Busy;
                CheckAndDestroy();
                break;
            case GameState.Repositioning:
                //Debug.Log("rep");
                gameState = GameState.Busy;
                StartCoroutine(RepositePuyo());
                break;
            case GameState.GameOver:
                //Debug.Log("gov");
                gameState = GameState.Busy;
                GameOver();
                //Time.timeScale = 0f; // todo
                break;
            case GameState.WaitingForInput:
                if (Input.anyKey)
                    SceneManager.LoadScene("End");
                break;
            default:
                throw new System.ArgumentOutOfRangeException();
        }
	}
    private void Generate()
    {
        StartCoroutine(PuyoManager.GenerateNewPuyoPairs(callBack =>
        {
            gameState = GameState.Spawning;
        }));
    }
    private void Spawning()
    {
        StartCoroutine(PuyoManager.SpawnNewPair(callBack =>
        {
            gameState = GameState.Falling;
        }));
    }
    private void Falling()
    {
        if (!fallPuyo())
        {
            gameState = GameState.Fixing;
            return;
        }
        rotatePuyo();
        moveABPuyo();
        if (!dropPuyo())
            gameState = GameState.Fixing;
    }
    private void Fixing()
    {
        // 0 = Game over, 1 = Danger, 2 = Normal
        StartCoroutine(PuyoManager.FixPair(callBack =>
        {
            if (callBack == 1)
                gameState = GameState.GameOver;
            else
                gameState = GameState.Repositioning;
        }));
    }

    private IEnumerator RepositePuyo()
    {
        PuyoManager.UpdatePuyosPosition();
        StartCoroutine(PuyoManager.MovePuyosToNewPosition(callBack =>
        {
            gameState = GameState.CheckAndDestroy;
        }));
        yield return null;
    }
    private void GameOver()
    {
        StartCoroutine(PuyoManager.GameOverAction(callBack =>
        {
            gameState = GameState.WaitingForInput;
        }));
    }

    /// <summary>
    /// Auto Fall Puyo down. 뿌요를 자동으로 아래로 움직이는 메서드.
    /// </summary>
    /// <returns></returns>
    private bool fallPuyo()
    {
        if (time >= fallingSpeed)
        {
            time = 0;
            if (PuyoManager.canPairFall())
                PuyoManager.controlBlock(Control.Fall);
            else
                return false;
        }
        return true;
    }
    /// <summary>
    /// Move Puyo down.
    /// 뿌요를 아래로 움직이는 메서드.
    /// </summary>
    private bool dropPuyo()
    {
        if (Input.GetKey(KeyCode.Space) == true && DownKeytime >= DownKeySpeed)
        {
            DownKeytime = 0;
            if (PuyoManager.canPairFall())
            {
                PuyoManager.controlBlock(Control.Drop);
                gameState = GameState.Falling;
            }
            else
                return false;
        }
        return true;
    }
    ///<summary>
    ///Move Puyo by ColumnA
    ///뿌요를 전후좌우로 움직이는 메서드.
    ///</summary>
    private void moveABPuyo()
    {
        var pressedA = Input.GetAxisRaw("HorizontalA");
        var pressedB = Input.GetAxisRaw("HorizontalB");

        if (pressedA == 0 && pressedB == 0)
            firstMove = true;
        else if (MoveKeytime >= MoveKeySpeed || firstMove == true)
        {
            if (pressedA == -1)
            {
                switch (GameVariable.Scene)
                {
                    case Control.Screen1:
                        PuyoManager.controlBlock(Control.MoveNegA);
                        break;
                    case Control.Screen2:
                        PuyoManager.controlBlock(Control.MoveNegB);
                        break;
                    case Control.Screen3:
                        PuyoManager.controlBlock(Control.MovePosA);
                        break;
                    case Control.Screen4:
                        PuyoManager.controlBlock(Control.MovePosB);
                        break;
                }
            }
            else if (pressedA == 1)
            {
                switch (GameVariable.Scene)
                {
                    case Control.Screen1:
                        PuyoManager.controlBlock(Control.MovePosA);
                        break;
                    case Control.Screen2:
                        PuyoManager.controlBlock(Control.MovePosB);
                        break;
                    case Control.Screen3:
                        PuyoManager.controlBlock(Control.MoveNegA);
                        break;
                    case Control.Screen4:
                        PuyoManager.controlBlock(Control.MoveNegB);
                        break;
                }
            }
            else if (pressedB == 1)
            {
                switch (GameVariable.Scene)
                {
                    case Control.Screen1:
                        PuyoManager.controlBlock(Control.MovePosB);
                        break;
                    case Control.Screen2:
                        PuyoManager.controlBlock(Control.MoveNegA);
                        break;
                    case Control.Screen3:
                        PuyoManager.controlBlock(Control.MoveNegB);
                        break;
                    case Control.Screen4:
                        PuyoManager.controlBlock(Control.MovePosA);
                        break;
                }
            }
            else if (pressedB == -1)
            {
                switch (GameVariable.Scene)
                {
                    case Control.Screen1:
                        PuyoManager.controlBlock(Control.MoveNegB);
                        break;
                    case Control.Screen2:
                        PuyoManager.controlBlock(Control.MovePosA);
                        break;
                    case Control.Screen3:
                        PuyoManager.controlBlock(Control.MovePosB);
                        break;
                    case Control.Screen4:
                        PuyoManager.controlBlock(Control.MoveNegA);
                        break;
                }
            }
            if (firstMove == true)
            {
                firstMove = false;
                MoveKeytime = -0.1f;
            }
        }
        gameState = GameState.Falling;
    }
    /// <summary>
    /// Rotate Puyo.
    /// 뿌요를 회전시키는 메서드.
    /// </summary>
    private void rotatePuyo()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            switch (GameVariable.Scene)
            {
                case Control.Screen1:
                    PuyoManager.controlBlock(Control.RotateLeftA);
                    break;
                case Control.Screen2:
                    PuyoManager.controlBlock(Control.RotateBack);
                    break;
                case Control.Screen3:
                    PuyoManager.controlBlock(Control.RotateRightA);
                    break;
                case Control.Screen4:
                    PuyoManager.controlBlock(Control.RotateForth);
                    break;
            }
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            switch (GameVariable.Scene)
            {
                case Control.Screen1:
                    PuyoManager.controlBlock(Control.RotateRightA);
                    break;
                case Control.Screen2:
                    PuyoManager.controlBlock(Control.RotateBack);
                    break;
                case Control.Screen3:
                    PuyoManager.controlBlock(Control.RotateLeftA);
                    break;
                case Control.Screen4:
                    PuyoManager.controlBlock(Control.RotateForth);
                    break;
            }
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            switch (GameVariable.Scene)
            {
                case Control.Screen1:
                    PuyoManager.controlBlock(Control.RotateBack);
                    break;
                case Control.Screen2:
                    PuyoManager.controlBlock(Control.RotateLeftA);
                    break;
                case Control.Screen3:
                    PuyoManager.controlBlock(Control.RotateForth);
                    break;
                case Control.Screen4:
                    PuyoManager.controlBlock(Control.RotateRightA);
                    break;
            }
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            switch (GameVariable.Scene)
            {
                case Control.Screen1:
                    PuyoManager.controlBlock(Control.RotateForth);
                    break;
                case Control.Screen2:
                    PuyoManager.controlBlock(Control.RotateRightA);
                    break;
                case Control.Screen3:
                    PuyoManager.controlBlock(Control.RotateBack);
                    break;
                case Control.Screen4:
                    PuyoManager.controlBlock(Control.RotateLeftA);
                    break;
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            PuyoManager.controlBlock(Control.RotateHorizontalLeft);
        }
        else if (Input.GetKeyDown(KeyCode.RightShift))
        {
            PuyoManager.controlBlock(Control.RotateHorizontalRight);
        }
        gameState = GameState.Falling;
    }

    private void CheckAndDestroy()
    {
        StartCoroutine(PuyoManager.DestroyAllChains(callBack =>
        {
            if (callBack == false)
                gameState = GameState.Repositioning;
            else
                gameState = GameState.Generate;
        }));
    }
}
