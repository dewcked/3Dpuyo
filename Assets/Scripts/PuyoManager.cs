using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;

public class PuyoManager : MonoBehaviour {

    readonly GameObject[,,] puyos = new GameObject[GameVariable.Rows, GameVariable.ColumnsA, GameVariable.ColumnsB];

    private PuyoPair fallingPair;
    //private Vector3[] fallingPairposition;

    AudioSource mainBGM;
    AudioSource Sound1;
    AudioSource Sound2;
    GameState gameState;

    List<PuyoPair> pairs = new List<PuyoPair>();

	int combo = 0;
    
    private float fallingSpeed = 0.3f;
    private float time = 0f;
    private float MoveKeytime = 0f;
    private float DownKeytime = 0.02f;
    private float RotateKeytime = 0.08f;
    private bool DoubleRotationChk = false;

    private float MoveKeySpeed = 0.05f;
    private float DownKeySpeed = 0.02f;
    private float RotateKeySpeed = 0.08f;

    private float Delaytime = 0.8f;
    private float CurrentDelay = 0f;

    [SerializeField]
	private GameObject[] puyoPrefabs;
    private bool firstMove;

    void Awake(){
        Sound1 = gameObject.AddComponent<AudioSource>();
        Sound2 = gameObject.AddComponent<AudioSource>();
        mainBGM = gameObject.AddComponent<AudioSource>();
        mainBGM.clip = Resources.Load<AudioClip>("Game1");
        mainBGM.loop = true;
        mainBGM.Play();
        gameState = GameState.Generate;
	}

    void Update()
    {
        time += Time.deltaTime;
        MoveKeytime += Time.deltaTime;
        DownKeytime += Time.deltaTime;
        switch (gameState)
        {
            case GameState.Generate:
                //Debug.Log("gen");
                gameState = GameState.Busy;
                while (pairs.Count < 3)
                    pairs.Add(GenerateNewPuyoPair());
                gameState = GameState.Spawning;
                break;
            case GameState.Spawning:
                //Debug.Log("spn");
                gameState = GameState.Busy;
                SpawnNewPair();
                gameState = GameState.Falling;
                break;
            case GameState.Busy:
                //Debug.Log("bsy");
                break;
            case GameState.CheckAndDestroy:
                //Debug.Log("cad");
                gameState = GameState.Busy;
                DestroyAllChains();
                ifDanger();
                ifgameOver();
                break;
            case GameState.Falling:
                //Debug.Log("fal");
                if (fallingPair == null)
                    break;
                if (time >= fallingSpeed)
                {
                    controlBlock(Control.Fall);
                    time = 0;
                }
                if (fallingPair == null)
                    break;
                rotatePuyo();
                moveABPuyo();
                dropPuyo();
                break;
            case GameState.Repositioning:
                //Debug.Log("rep");
                gameState = GameState.Busy;
                UpdatePuyosPosition();
                break;
            case GameState.GameOver:
                //Debug.Log("gov");
                gameOverAction();
                //Time.timeScale = 0f; // todo
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    void FixedUpdate() { }
    ///<summary>
    ///Move Puyo by ColumnA
    ///뿌요를 좌우로 움직이는 메서드.
    ///</summary>
    private void moveABPuyo()
    {
        var pressedLR = Input.GetAxisRaw("HorizontalA");

        if (pressedLR == 0)
        {
            firstMove = true;
            return;
        }
        if (MoveKeytime >= MoveKeySpeed || firstMove == true)
        {
            if (pressedLR == -1)
            {
                controlBlock(Control.Left);
            }
            else if (pressedLR == 1)
            {
                controlBlock(Control.Right);
            }
        }
        if (firstMove == true)
        {
            firstMove = false;
            MoveKeytime = -0.1f;
        }
    }

    /// <summary>
    /// Move Puyo down.
    /// 뿌요를 아래로 움직이는 메서드.
    /// </summary>
    private void dropPuyo()
    {
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.V) || Input.GetKeyDown(KeyCode.B) || Input.GetKeyDown(KeyCode.Space))
        {
            if (DownKeytime >= DownKeySpeed)
            {
                controlBlock(Control.Drop);
                DownKeytime = 0;
            }
        }
    }
    /// <summary>
    /// Rotate Puyo.
    /// 뿌요를 회전시키는 메서드.
    /// </summary>
    private void rotatePuyo()
    {
        if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.C))
        {
            controlBlock(Control.RotateLeft);
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            controlBlock(Control.RotateRight);
        }
    }

    private PuyoPair GenerateNewPuyoPair()
    {
        var rand1 = Convert.ToInt32(UnityEngine.Random.Range(0, 4));

        var go1 = Instantiate(puyoPrefabs[rand1], new Vector3(0f, -100f), Quaternion.identity) as GameObject;
        var puyoColor = PuyoHelper.GetPuyoColorFromString(go1.tag);
        go1.GetComponent<Puyo>().Initialize(puyoColor);

        var rand2 = Convert.ToInt32(UnityEngine.Random.Range(0, 4));

        var go2 = Instantiate(puyoPrefabs[rand2], new Vector3(0f, -100f), Quaternion.identity) as GameObject;
        var puyoColor2 = PuyoHelper.GetPuyoColorFromString(go2.tag);
        go2.GetComponent<Puyo>().Initialize(puyoColor2);

        return new PuyoPair(go1, go2);
    }

    private bool canPairFall()
    {
        var p1pos = fallingPair.Puyo1.transform.position;
        var p2pos = fallingPair.Puyo2.transform.position;
        
        if (Math.Ceiling(p1pos.y) != Math.Truncate(p1pos.y)) return true;

        if (p1pos.y == GameVariable.BasePoint.y || p2pos.y == GameVariable.BasePoint.y) return false;

        for (int i = 0; i < GameVariable.Rows; i++)
        {
            for (int j = 0; j < GameVariable.ColumnsA; j++)
            {
                for (int k = 0; k < GameVariable.ColumnsB; k++)
                {
                    if (puyos[i, j, k] != null)
                    {
                        var pPos = puyos[i, j, k].transform.position;
                        if ((pPos.x == (int)p1pos.x && pPos.y == (int)p1pos.y - 1) || (pPos.x == (int)p2pos.x && pPos.y == (int)p2pos.y - 1))
                            return false;
                    }
                }
            }
        }

        return true;
    }

    private bool canGoLeft()
    {
        var p1pos = fallingPair.Puyo1.transform.position;
        var p2pos = fallingPair.Puyo2.transform.position;

        if (p1pos.x <= GameVariable.BasePoint.x || p2pos.x <= GameVariable.BasePoint.x)
            return false;

        for (int i = 0; i < GameVariable.Rows; i++)
        {
            for (int j = 0; j < GameVariable.ColumnsA; j++)
            {
                for (int k = 0; k < GameVariable.ColumnsB; k++)
                {
                    if (puyos[i, j, k] != null && (i == getRowFromYPosition(p1pos.y) || i == getRowFromYPosition(p2pos.y)))
                    {
                        var pPos = puyos[i, j, k].transform.position;
                        if (p1pos.x - 1 == pPos.x || p2pos.x - 1 == pPos.x)
                            return false;
                    }
                }
            }
        }

        return true;
    }

    private bool canGoRight()
    {
        var p1pos = fallingPair.Puyo1.transform.position;
        var p2pos = fallingPair.Puyo2.transform.position;

        if (p1pos.x >= GameVariable.BasePoint.x + GameVariable.ColumnsA - 1 || p2pos.x >= GameVariable.BasePoint.x + GameVariable.ColumnsA - 1) return false;

        for (int i = 0; i < GameVariable.Rows; i++)
        {
            for (int j = 0; j < GameVariable.ColumnsA; j++)
            {
                for (int k = 0; k < GameVariable.ColumnsB; k++)
                {
                    if (puyos[i, j, k] != null && (i == getRowFromYPosition(p1pos.y) || i == getRowFromYPosition(p2pos.y)))
                    {
                        var pPos = puyos[i, j, k].transform.position;
                        if (p1pos.x + 1 == pPos.x || p2pos.x + 1 == pPos.x)
                            return false;
                    }
                }
            }
        }
        return true;
    }

    private void controlBlock(Control PuyoControl)
    {
        gameState = GameState.Busy;
        bool canFall = canPairFall();
        Vector3[] changeVector = { Vector3.zero, Vector3.zero };
        switch (PuyoControl)
        {
            case Control.Fall:
                if (canFall)
                {
                    fallingPair.Puyo1.transform.position -= new Vector3(0f, 0.5f);
                    fallingPair.Puyo2.transform.position -= new Vector3(0f, 0.5f);
                }
                else
                {
                    gameState = GameState.Busy;
                    StartCoroutine(FixPair());
                }
                break;
            case Control.Drop:
                if (canFall)
                {
                    fallingPair.Puyo1.transform.position -= new Vector3(0f, 0.5f);
                    fallingPair.Puyo2.transform.position -= new Vector3(0f, 0.5f);
                    Sound1.clip = Resources.Load<AudioClip>("movePuyo");
                    Sound1.Play();
                }
                else
                {
                    gameState = GameState.Busy;
                    StartCoroutine(FixPair());
                }
                break;
            case Control.Left:
                if (canGoLeft())
                {
                    fallingPair.Puyo1.transform.position -= new Vector3(1f, 0f);
                    fallingPair.Puyo2.transform.position -= new Vector3(1f, 0f);
                    Sound1.clip = Resources.Load<AudioClip>("movePuyo");
                    Sound1.Play();
                }
                break;
            case Control.Right:
                if (canGoRight())
                {
                    fallingPair.Puyo1.transform.position += new Vector3(1f, 0f);
                    fallingPair.Puyo2.transform.position += new Vector3(1f, 0f);
                    Sound1.clip = Resources.Load<AudioClip>("movePuyo");
                    Sound1.Play();
                }
                break;
            case Control.RotateLeft:
                changeVector = canRotateLeft();
                if (!changeVector[0].Equals(Vector3.zero) || !changeVector[1].Equals(Vector3.zero))
                {
                    fallingPair.Puyo1.transform.position += changeVector[0];
                    fallingPair.Puyo2.transform.position += changeVector[1];
                    Sound2.clip = Resources.Load<AudioClip>("rotatePuyo");
                    Sound2.Play();
                }
                break;
            case Control.RotateRight:
                changeVector = canRotateRight();
                if (!changeVector[0].Equals(Vector3.zero) || !changeVector[1].Equals(Vector3.zero))
                {
                    fallingPair.Puyo1.transform.position += changeVector[0];
                    fallingPair.Puyo2.transform.position += changeVector[1];
                    //fallingPair.Puyo1.transform.position = Vector3.Lerp(fallingPair.Puyo1.transform.position, fallingPair.Puyo1.transform.position + changeVector[0], 1);
                    //fallingPair.Puyo2.transform.position = Vector3.Lerp(fallingPair.Puyo2.transform.position, fallingPair.Puyo2.transform.position + changeVector[1], 1);
                    Sound2.clip = Resources.Load<AudioClip>("rotatePuyo");
                    Sound2.Play();
                }
                break;
        }
        gameState = GameState.Falling;
    }

    private Vector3[] canRotateRight()
    {
        var p1pos = fallingPair.Puyo1.transform.position;
        var p2pos = fallingPair.Puyo2.transform.position;
        var lpad = false;
        var rpad = false;
        Vector3 bpad = Vector3.zero;
        Vector3[] result = { Vector3.zero, Vector3.zero };

        if (fallingPair.Orientation == Orientation.HorizontalA)
        {
            if (Math.Truncate(p1pos.y) == GameVariable.BasePoint.y)
                bpad = new Vector3(0f, 1 - (p1pos.y - GameVariable.BasePoint.y));
            else
            {
                for (int i = 0; i < GameVariable.Rows; i++)
                {
                    for (int j = 0; j < GameVariable.ColumnsA; j++)
                    {
                        for (int k = 0; k < GameVariable.ColumnsB; k++)
                        {
                            if (puyos[i, j, k] != null)
                            {
                                var pPos = puyos[i, j, k].transform.position;

                                if (pPos.x == p1pos.x && pPos.y == (float)Math.Truncate(p1pos.y) - 1)
                                    bpad = new Vector3(0, 1 - (p1pos.y - (float)Math.Truncate(p1pos.y)));
                            }
                        }
                    }
                }
            }
            if (p1pos.x > p2pos.x)
                result[1] = new Vector3(1f, 1f);
            else
            {
                //Debug.Log(fallingPair.Puyo1.transform.position.y + " " + fallingPair.Puyo2.transform.position.y + " " + bpad.y + "s");
                result[0] = new Vector3(0f, bpad.y);
                result[1] = new Vector3(-1f, -1f + bpad.y);
                //Debug.Log(fallingPair.Puyo1.transform.position.y + " " + fallingPair.Puyo2.transform.position.y + " " + bpad.y + "e");
            }
            fallingPair.Orientation = Orientation.Vertical;
            DoubleRotationChk = false;
            return result;
        }
        else
        {
            if (Math.Truncate(p1pos.y) == GameVariable.BasePoint.y)
                bpad = new Vector3(0f, 1 - (p1pos.y - GameVariable.BasePoint.y));
            if (p1pos.x == GameVariable.BasePoint.x)
                lpad = true;
            else if (p1pos.x == GameVariable.BasePoint.x + GameVariable.ColumnsA - 1)
                rpad = true;
            for (int i = 0; i < GameVariable.Rows; i++)
            {
                for (int j = 0; j < GameVariable.ColumnsA; j++)
                {
                    for (int k = 0; k < GameVariable.ColumnsB; k++)
                    {
                        if (puyos[i, j, k] != null)
                        {
                            var pPos = puyos[i, j, k].transform.position;

                            if (p1pos.x - 1 == pPos.x && (float)Math.Truncate(p1pos.y) == pPos.y)
                            {
                                lpad = true;
                            }
                            else if (p1pos.x + 1 == pPos.x && (float)Math.Truncate(p1pos.y) == pPos.y)
                            {
                                rpad = true;
                            }
                            else if (p1pos.x == pPos.x && (float)Math.Truncate(p1pos.y) - 1 == pPos.y)
                            {
                                bpad = new Vector3(0, 1 - (p1pos.y - (float)Math.Truncate(p1pos.y)));
                            }
                        }
                    }
                }
            }
            if (lpad == true && rpad == true)
            {
                if (DoubleRotationChk == false)
                {
                    DoubleRotationChk = true;
                    return result;
                }
                else
                {
                    DoubleRotationChk = false;
                    if (p1pos.y > p2pos.y)
                        result[1] = new Vector3(0f, 2f);
                    else
                    {
                        result[0] = new Vector3(0f, bpad.y);
                        result[1] = new Vector3(0f, bpad.y - 2f);
                    }
                    fallingPair.Orientation = Orientation.Vertical;
                    return result;
                }
            }
            else if (lpad == false && rpad == false)
            {
                if (p1pos.y > p2pos.y)
                    result[1] = new Vector3(-1f, 1f);
                else
                    result[1] = new Vector3(1f, -1f);
            }
            else if (lpad == true && rpad == false)
            {
                if (p1pos.y > p2pos.y)
                {
                    result[0] = new Vector3(1f, 0f);
                    result[1] = new Vector3(0f, 1f);
                }
                else
                {
                    result[1] = new Vector3(1f, -1f);
                }
            }
            else if (lpad == false && rpad == true)
            {
                if (p1pos.y > p2pos.y)
                {
                    result[1] = new Vector3(-1f, 1f);
                }
                else
                {
                    result[0] = new Vector3(-1f, 0f);
                    result[1] = new Vector3(0f, -1f);
                }
            }
            fallingPair.Orientation = Orientation.HorizontalA;
            return result;
        }
    }

    private Vector3[] canRotateLeft()
    {
        var p1pos = fallingPair.Puyo1.transform.position;
        var p2pos = fallingPair.Puyo2.transform.position;
        var lpad = false;
        var rpad = false;
        Vector3 bpad = Vector3.zero;
        Vector3[] result = { Vector3.zero, Vector3.zero };

        if (fallingPair.Orientation == Orientation.HorizontalA)
        {
            if (Math.Truncate(p1pos.y) == GameVariable.BasePoint.y)
                bpad = new Vector3(0f, 1 - (p1pos.y - GameVariable.BasePoint.y));
            else
            {
                for (int i = 0; i < GameVariable.Rows; i++)
                {
                    for (int j = 0; j < GameVariable.ColumnsA; j++)
                    {
                        for (int k = 0; k < GameVariable.ColumnsB; k++)
                        {
                            if (puyos[i, j, k] != null)
                            {
                                var pPos = puyos[i, j, k].transform.position;
                                if (pPos.x == p1pos.x && pPos.y == (float)Math.Truncate(p1pos.y) - 1)
                                    bpad = new Vector3(0, 1 - (p1pos.y - (float)Math.Truncate(p1pos.y)));
                            }
                        }
                    }
                }
            }
            if (p1pos.x < p2pos.x)
                result[1] = new Vector3(-1f, 1f);
            else
            {
                result[0] = new Vector3(0f, bpad.y);
                result[1] = new Vector3(1f, -1f + bpad.y);
            }
            fallingPair.Orientation = Orientation.Vertical;
            DoubleRotationChk = false;
            return result;
        }
        else
        {
            if (Math.Truncate(p1pos.y) == GameVariable.BasePoint.y)
                bpad = new Vector3(0f, 1 - (p1pos.y - GameVariable.BasePoint.y));
            if (p1pos.x == GameVariable.BasePoint.x)
                lpad = true;
            else if (p1pos.x == GameVariable.BasePoint.x + GameVariable.ColumnsA - 1)
                rpad = true;
            for (int i = 0; i < GameVariable.Rows; i++)
            {
                for (int j = 0; j < GameVariable.ColumnsA; j++)
                {
                    for (int k = 0; k < GameVariable.ColumnsB; k++)
                    {
                        if (puyos[i, j, k] != null)
                        {
                            var pPos = puyos[i, j, k].transform.position;

                            if (p1pos.x - 1 == pPos.x && (float)Math.Truncate(p1pos.y) == pPos.y)
                            {
                                lpad = true;
                            }
                            else if (p1pos.x + 1 == pPos.x && (float)Math.Truncate(p1pos.y) == pPos.y)
                            {
                                rpad = true;
                            }
                            else if (p1pos.x == pPos.x && (float)Math.Truncate(p1pos.y) - 1 == pPos.y)
                            {
                                bpad = new Vector3(0, 1 - (p1pos.y - (float)Math.Truncate(p1pos.y)));
                            }
                        }
                    }
                }
            }
            if (lpad == true && rpad == true)
            {
                if (DoubleRotationChk == false)
                {
                    DoubleRotationChk = true;
                    return result;
                }
                else
                    DoubleRotationChk = false;
                if (p1pos.y > p2pos.y)
                    result[1] = new Vector3(0f, 2f);
                else
                {
                    result[0] = new Vector3(0f, bpad.y);
                    result[1] = new Vector3(0f, bpad.y - 2f);
                }
                fallingPair.Orientation = Orientation.Vertical;
                return result;
            }
            else if (lpad == false && rpad == false)
            {
                if (p1pos.y > p2pos.y)
                    result[1] = new Vector3(1f, 1f);
                else
                    result[1] = new Vector3(-1f, -1f);
            }
            else if (lpad == false && rpad == true)
            {
                if (p1pos.y > p2pos.y)
                {
                    result[0] = new Vector3(-1f, 0f);
                    result[1] = new Vector3(0f, 1f);
                }
                else
                {
                    result[1] = new Vector3(-1f, -1f);
                }
            }
            else if (lpad == true && rpad == false)
            {
                if (p1pos.y > p2pos.y)
                {
                    result[1] = new Vector3(1f, 1f);
                }
                else
                {
                    result[0] = new Vector3(1f, 0f);
                    result[1] = new Vector3(0f, -1f);
                }
            }
            fallingPair.Orientation = Orientation.HorizontalA;
            return result;
        }
    }

    private IEnumerator FixPair()
    {
        // todo correct puyo position and add to array and add column and row properties

        // Round Pos
        //var p1pos = fallingPair.Puyo1.transform.position;
        //var roundedP1PosY = (float)Math.Round(p1pos.y);
        //fallingPair.Puyo1.transform.position = new Vector3(p1pos.x, roundedP1PosY);

        //var p2pos = fallingPair.Puyo2.transform.position;
        //var roundedP2PosY = (float)Math.Round(p2pos.y);
        //fallingPair.Puyo2.transform.position = new Vector3(p2pos.x, roundedP2PosY);


        // Find Column and Row
        var p1colA = getColumnFromXPosition(fallingPair.Puyo1.transform.position.x);
        var p1colB = getColumnFromXPosition(fallingPair.Puyo1.transform.position.z);
        var p1row = getRowFromYPosition(fallingPair.Puyo1.transform.position.y);

        var p2colA = getColumnFromXPosition(fallingPair.Puyo2.transform.position.x);
        var p2colB = getColumnFromXPosition(fallingPair.Puyo2.transform.position.z);
        var p2row = getRowFromYPosition(fallingPair.Puyo2.transform.position.y);

        //Debug.Log(string.Format("p1 col : {0}, p1 row : {1}\np2col : {2}, p2row : {3}", p1col, p1row, p2col, p2row));

        // Update array
        puyos[p1row, p1colA, p1colB] = fallingPair.Puyo1;
        puyos[p1row, p1colA, p1colB].GetComponent<Puyo>().ColumnA = p1colA;
        puyos[p1row, p1colA, p1colB].GetComponent<Puyo>().ColumnB = p1colB;
        puyos[p1row, p1colA, p1colB].GetComponent<Puyo>().Row = p1row;

        puyos[p2row, p2colA, p2colB] = fallingPair.Puyo2;
        puyos[p2row, p2colA, p2colB].GetComponent<Puyo>().ColumnA = p2colA;
        puyos[p2row, p2colA, p2colB].GetComponent<Puyo>().ColumnB = p2colB;
        puyos[p2row, p2colA, p2colB].GetComponent<Puyo>().Row = p2row;

        // ResetFallingPair
        fallingPair = null;
        combo = 0;
        yield return new WaitForSeconds(3 * GameVariable.PuyoRepositioningSpeed);
        gameState = GameState.Repositioning;
    }
    private void ifgameOver()
    {
        for (int i = 0; i < GameVariable.ColumnsA; i++)
            for (int j = 0; j < GameVariable.ColumnsB; j++)
            {
                if (puyos[12, i, j] != null)
                {
                    mainBGM.Stop();
                    gameState = GameState.GameOver;
                }
            }
    }
    private void ifDanger()
    {
        for (int i = 0; i < GameVariable.ColumnsA; i++)
            for (int j = 0; j < GameVariable.ColumnsB; j++)
            {
                if (puyos[9, i, j] != null)
                {
                    if (mainBGM.clip.name != "Warn")
                    {
                        mainBGM.clip = Resources.Load<AudioClip>("Warn");
                        mainBGM.Play();
                    }
                    return;
                }
            }
        if(mainBGM.clip.name == "Warn")
        {
            mainBGM.clip = Resources.Load<AudioClip>("Game1");
            mainBGM.Play();
        }

    }
    private void gameOverAction()
    {
        var biggestRatio = 0f;

        for (int i = 0; i < GameVariable.Rows; i++)
        {
            for (int j = 0; j < GameVariable.ColumnsA; j++)
            {
                for (int k = 0; k < GameVariable.ColumnsB; k++)
                {
                    var puyo = puyos[i, j, k];
                    if (puyo != null)
                    {
                        var oldPosition = puyo.transform.position;
                        var newPosition = FindPuyoScreenPosition(i, j);

                        // Position is correct, skip and continue with the next puyo
                        if (oldPosition.y == newPosition.y) continue;

                        var ratio = (oldPosition.y - newPosition.y) / GameVariable.PuyoSize.y;
                        if (ratio > biggestRatio) biggestRatio = ratio;

                        puyo.transform.positionTo(GameVariable.PuyoRepositioningSpeed * ratio, newPosition);
                    }
                }
            }
        }
    }

    private int getColumnFromXPosition(float x)
    {
        return (int)x;
    }

    private int getRowFromYPosition(float y)
    {
		return (int)y;
    }

    public void SpawnNewPair()
    {
        if (pairs.Any())
        {
            pairs.First().Puyo1.transform.position = new Vector3(2f, 12f, 2f);
            pairs.First().Puyo2.transform.position = new Vector3(2f, 13f, 2f);
            fallingPair = pairs.First();
            pairs.Remove(pairs.First());
        }
        pairs.ElementAt(1).Puyo1.transform.position = new Vector3(7.5f, 7f, 2f);
        pairs.ElementAt(1).Puyo2.transform.position = new Vector3(7.5f, 8f, 2f);

        pairs.First().Puyo1.transform.position = new Vector3(6.5f, 8f, 2f);
        pairs.First().Puyo2.transform.position = new Vector3(6.5f, 9f, 2f);
    }

    public void UpdatePuyosPosition()
    {
        ComputeNewPuyosPosition();
        StartCoroutine(MovePuyosToNewPosition());
    }

    private void MovePuyos()
    {
        for (int i = 0; i < GameVariable.Rows; i++)
        {
            for (int j = 0; j < GameVariable.ColumnsA; j++)
            {
                for (int k = 0; k < GameVariable.ColumnsB; k++)
                {
                    if (i < 0 && puyos[i - 1, j, k] != null)
                    {
                        var pos = puyos[i, j, k].transform.position;
                        pos.y -= GameVariable.PuyoSize.y;
                        puyos[i, j, k].transform.positionTo(2f, pos);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Used to compute new puyos position after destroying
    /// </summary>
    private void ComputeNewPuyosPosition()
    {
        bool puyoHasToMove = true;
        while (puyoHasToMove)
        {
            puyoHasToMove = false;
            for (int i = 1; i < GameVariable.Rows; i++)
            {
                for (int j = 0; j < GameVariable.ColumnsA; j++)
                {
                    for (int k = 0; k < GameVariable.ColumnsB; k++)
                    {
                        var puyoGo = puyos[i, j, k];
                        if (puyoGo != null)
                        {
                            // Puyo under is null
                            if (puyos[i - 1, j, k] == null)
                            {
                                // Shift all top puyos
                                for (int l = i; l < GameVariable.Rows; l++)
                                {
                                    var topPuyo = puyos[l, j, k];
                                    if (topPuyo != null)
                                    {
                                        puyos[l - 1, j, k] = topPuyo;
                                        puyos[l - 1, j, k].GetComponent<Puyo>().Row--;
                                        puyos[l, j, k] = null;
                                    }
                                }
                                puyoHasToMove = true;
                            }
                        }
                    }
                }
            }
        }
    }

    private Vector3 FindPuyoScreenPosition(int row, int column)
    {
        return new Vector3((GameVariable.BasePoint.y + column * GameVariable.PuyoSize.y), GameVariable.BasePoint.x + row * GameVariable.PuyoSize.x);
    }

    /// <summary>
    /// Tween Puyos to new positions
    /// </summary>
    /// <returns></returns>
    private IEnumerator MovePuyosToNewPosition()
    {
        // We use this variable to store the biggest ratio, it is used to know how many time we have to wait for the tween to complete
        var biggestRatio = 0f;
        
        for (int i = 0; i < GameVariable.Rows; i++)
        {
            for (int j = 0; j < GameVariable.ColumnsA; j++)
            {
                for (int k = 0; k < GameVariable.ColumnsB; k++)
                {
                    var puyo = puyos[i, j, k];
                    if (puyo != null)
                    {
                        var oldPosition = puyo.transform.position;
                        var newPosition = FindPuyoScreenPosition(i, j);

                        // Position is correct, skip and continue with the next puyo
                        if (oldPosition.y == newPosition.y) continue;

                        var ratio = (oldPosition.y - newPosition.y) / GameVariable.PuyoSize.y;
                        if (ratio > biggestRatio) biggestRatio = ratio;

                        puyo.transform.positionTo(GameVariable.PuyoRepositioningSpeed * ratio, newPosition);
                    }
                }
            }
        }

        yield return new WaitForSeconds(biggestRatio * GameVariable.PuyoRepositioningSpeed);

        gameState = GameState.CheckAndDestroy;
    }
    
    private void InitRandomArray()
    {
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < GameVariable.ColumnsA; j++)
            {
                for (int k = 0; k < GameVariable.ColumnsB; k++)
                {
                    var rand = Convert.ToInt32(UnityEngine.Random.Range(0, 4));

                    var position = FindPuyoScreenPosition(i, j);
                    var go = Instantiate(puyoPrefabs[rand], position, Quaternion.identity) as GameObject;
                    var puyoColor = PuyoHelper.GetPuyoColorFromString(go.tag);
                    go.GetComponent<Puyo>().Initialize(puyoColor, i, j);

                    puyos[i, j, k] = go;
                }
            }
        }
    }

    private void DestroyAllChains()
    {
        var chains = FindAllChains();
        if (chains.Any())
        {
            foreach (var chain in chains)
            {
                ComboSoundPlay();
                foreach (var puyo in chain.Puyos)
                {
                    StartCoroutine(AnimateAndDestroy(puyo));
                }
            }
        }
        else
        {
            gameState = GameState.Generate;
        }
        
    }
    private void ComboSoundPlay()
    {
        ++combo;
        Debug.Log(combo);
        switch(combo)
        {
            case 1:
                Sound1.clip = Resources.Load<AudioClip>("combo1");
                break;
            case 2:
                Sound1.clip = Resources.Load<AudioClip>("combo2");
                break;
            case 3:
                Sound1.clip = Resources.Load<AudioClip>("combo3");
                break;
            case 4:
                Sound1.clip = Resources.Load<AudioClip>("combo4");
                break;
            case 5:
                Sound1.clip = Resources.Load<AudioClip>("combo5");
                break;
            case 6:
                Sound1.clip = Resources.Load<AudioClip>("combo6");
                break;
            case 7:
                Sound1.clip = Resources.Load<AudioClip>("combo7");
                break;
        }
        Sound1.Play();
    }
    private IEnumerator AnimateAndDestroy(Puyo puyo)
    {
        var animationComponent = puyos[puyo.Row, puyo.ColumnA, puyo.ColumnB].GetComponent<Animation>();
        animationComponent.Play();

        do
        {
            yield return null;
        } while (animationComponent.isPlaying);

        DestroyPuyo(puyo);
        
        gameState = GameState.Repositioning;
    }

    private void DestroyPuyo(Puyo puyo)
    {
        Destroy(puyos[puyo.Row, puyo.ColumnA, puyo.ColumnB]);
        puyos[puyo.Row, puyo.ColumnA, puyo.ColumnB] = null;
    }

    private List<PuyoGroup> FindAllChains()
    {
        var chains = new List<PuyoGroup>();

        for (int i = 0; i < GameVariable.Rows; i++)
        {
            for (int j = 0; j < GameVariable.ColumnsA; j++)
            {
                for (int k = 0; k < GameVariable.ColumnsB; k++)
                {
                    var p = puyos[i, j, k];

                    if (p == null) continue;

                    var pscript = p.GetComponent<Puyo>();

                    bool alreadyInGroup = false;
                    foreach (var group in chains)
                    {
                        if (group.ContainPuyo(pscript))
                        {
                            alreadyInGroup = true;
                        }
                    }
                    if (alreadyInGroup) continue;

                    var newGroup = FindChain(pscript);

                    if (newGroup != null) chains.Add(newGroup);
                }
            }
        }

        return chains;
    }

    private PuyoGroup FindChain(Puyo puyo)
    {
        var currentChain = new List<Puyo> {puyo};
        var nextPuyosToCheck = new List<Puyo> {puyo};

        while (nextPuyosToCheck.Any())
        {
            var pi = nextPuyosToCheck.First();
            var nextInChain = FindNextPuyoInChain(pi, currentChain);
            while (nextInChain != null)
            {
                currentChain.Add(nextInChain);
                nextPuyosToCheck.Add(nextInChain);
                nextInChain = FindNextPuyoInChain(pi, currentChain);
            }
            nextPuyosToCheck.Remove(pi);
        }

        return currentChain.Count >= GameVariable.MinimumMatches ? new PuyoGroup(currentChain) : null;
    }

    private Puyo FindNextPuyoInChain(Puyo puyo, IEnumerable<Puyo> ignoredPuyos)
    {
        var ignoredList = ignoredPuyos.ToList();

        // Top
        if (puyo.Row < GameVariable.Rows - 1 && (ignoredPuyos == null || !ignoredList.Any(p => p.Row == puyo.Row + 1 && p.ColumnA == puyo.ColumnA)))
        {
            var topPuyo = puyos[puyo.Row + 1, puyo.ColumnA, puyo.ColumnB];
            if (topPuyo != null && topPuyo.GetComponent<Puyo>().Color == puyo.Color)
                return topPuyo.GetComponent<Puyo>();
        }

        // Bottom
        if (puyo.Row > 0 && (ignoredPuyos == null || !ignoredList.Any(p => p.Row == puyo.Row - 1 && p.ColumnA == puyo.ColumnA)))
        {
            var bottomPuyo = puyos[puyo.Row - 1, puyo.ColumnA, puyo.ColumnB];
            if (bottomPuyo != null && bottomPuyo.GetComponent<Puyo>().Color == puyo.Color)
                return bottomPuyo.GetComponent<Puyo>();
        }

        // Right
        if (puyo.ColumnA < GameVariable.ColumnsA - 1 && (ignoredPuyos == null || !ignoredList.Any(p => p.Row == puyo.Row && p.ColumnA == puyo.ColumnA + 1)))
        {
            var rightPuyo = puyos[puyo.Row, puyo.ColumnA + 1, puyo.ColumnB];
            if (rightPuyo != null && rightPuyo.GetComponent<Puyo>().Color == puyo.Color)
                return rightPuyo.GetComponent<Puyo>();
        }

        // Left
        if (puyo.ColumnA > 0 && (ignoredPuyos == null || !ignoredList.Any(p => p.Row == puyo.Row && p.ColumnA == puyo.ColumnA - 1)))
        {
            var leftPuyo = puyos[puyo.Row, puyo.ColumnA - 1, puyo.ColumnB];
            if (leftPuyo != null && leftPuyo.GetComponent<Puyo>().Color == puyo.Color)
                return leftPuyo.GetComponent<Puyo>();
        }
        
        // Nothing is found, return null
        return null;
    }
    
    //public bool HasSameColorNeighbor(Puyo puyo)
    //{
    //    return
    //        // TOP
    //        (puyo.Row < GameVariable.Rows - 1 && puyos[puyo.Row + 1, puyo.Column] != null && puyos[puyo.Row + 1, puyo.Column].GetComponent<Puyo>().Color == puyo.Color) ||

    //        // BOT
    //        (puyo.Row > 0 && puyos[puyo.Row - 1, puyo.Column] != null && puyos[puyo.Row - 1, puyo.Column].GetComponent<Puyo>().Color == puyo.Color) ||

    //        // RIGHT
    //        (puyo.Column < GameVariable.Columns - 1 && puyos[puyo.Row, puyo.Column + 1] != null && puyos[puyo.Row, puyo.Column + 1].GetComponent<Puyo>().Color == puyo.Color) ||

    //        // LEFT
    //        (puyo.Column > 0 && puyos[puyo.Row, puyo.Column - 1] != null && puyos[puyo.Row, puyo.Column - 1].GetComponent<Puyo>().Color == puyo.Color);
    //}
    
    //private bool CheckIfAllPuyosAreWellPositioned()
    //{
    //    for (int i = 0; i < GameVariable.Rows; i++)
    //    {
    //        for (int j = 0; j < GameVariable.Columns; j++)
    //        {
    //            if (!IsPuyoPositionCorrect(puyos[i, j])) return false;
    //        }
    //    }
    //    return true;
    //}

    //private bool IsPuyoPositionCorrect(GameObject puyoGo)
    //{
    //    if(puyoGo == null) throw new ArgumentNullException();

    //    var puyoScript = puyoGo.GetComponent<Puyo>();
    //    var puyoPosition = puyoGo.transform.position;
    //    var correctPosition = FindPuyoScreenPosition(puyoScript.Row, puyoScript.Column);
    //    return puyoPosition.x == correctPosition.x && puyoPosition.y == correctPosition.y;
    //}
}

internal interface IEnumerarator
{
}