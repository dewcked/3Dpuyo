using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // Use this for initialization
    void Awake () {
        PuyoManager = transform.FindChild("PuyoManager").gameObject.GetComponent<PuyoManager>();
        GameVariable.gameState = GameState.Generate;
    }
	
	// Update is called once per frame
	void Update () {
		time += Time.deltaTime;
        MoveKeytime += Time.deltaTime;
        DownKeytime += Time.deltaTime;
        if(PuyoManager.pairs.Count >= 2)
        {
            PuyoManager.ShowNextPuyo();
        }
        switch (GameVariable.gameState)
        {
            case GameState.Generate:
                //Debug.Log("gen");
                GameVariable.gameState = GameState.Busy;
                while (PuyoManager.pairs.Count < 3)
                    PuyoManager.pairs.Add(PuyoManager.GenerateNewPuyoPair());
                GameVariable.gameState = GameState.Spawning;
                break;
            case GameState.Spawning:
                //Debug.Log("spn");
                GameVariable.gameState = GameState.Busy;
                PuyoManager.SpawnNewPair();
                GameVariable.gameState = GameState.Falling;
                break;
            case GameState.Busy:
                //Debug.Log("bsy");
                break;
            case GameState.CheckAndDestroy:
                //Debug.Log("cad");
                GameVariable.gameState = GameState.Busy;
                PuyoManager.DestroyAllChains();
                PuyoManager.ifDanger();
                PuyoManager.ifgameOver();
                break;
            case GameState.Falling:
                //Debug.Log("fal");
                if (PuyoManager.fallingPair == null)
                    break;
                if (time >= fallingSpeed)
                {
                    PuyoManager.controlBlock(Control.Fall);
                    time = 0;
                }
                if (PuyoManager.fallingPair == null)
                    break;
                rotatePuyo();
                moveABPuyo();
                dropPuyo();
                break;
            case GameState.Repositioning:
                //Debug.Log("rep");
                GameVariable.gameState = GameState.Busy;
                PuyoManager.UpdatePuyosPosition();
                break;
            case GameState.GameOver:
                //Debug.Log("gov");
                PuyoManager.gameOverAction();
                //Time.timeScale = 0f; // todo
                break;
            default:
                throw new System.ArgumentOutOfRangeException();
        }
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
    }
    /// <summary>
    /// Move Puyo down.
    /// 뿌요를 아래로 움직이는 메서드.
    /// </summary>
    private void dropPuyo()
    {
        if (Input.GetKey(KeyCode.Space) == true && DownKeytime >= DownKeySpeed)
        {
            PuyoManager.controlBlock(Control.Drop);
            DownKeytime = 0;
        }
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
    }
}
