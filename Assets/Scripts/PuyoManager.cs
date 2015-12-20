using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;

public class PuyoManager : MonoBehaviour {

    readonly GameObject[,] puyos = new GameObject[GameVariable.Rows, GameVariable.Columns];

    private PuyoPair fallingPair;

	GameState gameState;

	Vector2 bottomLeft = new Vector2 (-5f, -4f);

	Vector2 puyoSize = new Vector2(1f, 1f);

    List<PuyoPair> pairs = new List<PuyoPair>();

	[SerializeField]
	private GameObject[] puyoPrefabs;
        
	void Awake(){
        InitRandomArray();

        InitPairs();

        gameState = GameState.CheckAndDestroy;
	}

    void InitPairs()
    {
        for (int i = 0; i < GameVariable.NumberOfPairToGenerate; i++)
        {
            pairs.Add(GenerateNewPuyoPair());
        }
    }

    void Update()
    {
        switch (gameState)
        {
            case GameState.Busy:
                break;
            case GameState.Spawning:
                SpawnNewPair();
                gameState = GameState.Falling;
                break;
            case GameState.Falling:
                HandleFall();
                break;
            case GameState.CheckAndDestroy:
                gameState = GameState.Busy;
                DestroyAllChains();
                break;
            case GameState.Repositioning:
                gameState = GameState.Busy;
                UpdatePuyosPosition();
                break;
            case GameState.GameOver:
                Time.timeScale = 0f; // todo
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private PuyoPair GenerateNewPuyoPair()
    {
        var rand1 = Convert.ToInt32(UnityEngine.Random.Range(0, 4));

        var go1 = Instantiate(puyoPrefabs[rand1], new Vector3(10f, 10f), Quaternion.identity) as GameObject;
        var puyoColor = PuyoHelper.GetPuyoColorFromString(go1.tag);
        go1.GetComponent<Puyo>().Initialize(puyoColor);

        var rand2 = Convert.ToInt32(UnityEngine.Random.Range(0, 4));

        var go2 = Instantiate(puyoPrefabs[rand2], new Vector3(10f, 10f), Quaternion.identity) as GameObject;
        var puyoColor2 = PuyoHelper.GetPuyoColorFromString(go2.tag);
        go2.GetComponent<Puyo>().Initialize(puyoColor2);

        return new PuyoPair(go1, go2);
    }
    
    private Vector3 fallingSpeed = new Vector3(0f, 3f);

    private bool canPairFall()
    {
        var p1pos = fallingPair.Puyo1.transform.position;
        var p2pos = fallingPair.Puyo1.transform.position;

        for (int i = 0; i < GameVariable.Rows; i++)
        {
            for (int j = 0; j < GameVariable.Columns; j++)
            {
                if (puyos[i, j] != null && j == getColumnFromXPosition(p1pos.x))
                {
                    var pPos = puyos[i, j].transform.position;
                    if (pPos.y >= p1pos.y - puyoSize.y || pPos.y >= p2pos.y - puyoSize.y) return false;
                }

                if (p1pos.y <= -6f || p2pos.y <= -6f) return false;
            }
        }

        return true;
    }

    private bool canGoLeft()
    {
        var p1pos = fallingPair.Puyo1.transform.position;
        var p2pos = fallingPair.Puyo1.transform.position;

        for (int i = 0; i < GameVariable.Rows; i++)
        {
            for (int j = 0; j < GameVariable.Columns; j++)
            {
                if (puyos[i, j] != null && (i == getRowFromYPosition(p1pos.y) || i == getRowFromYPosition(p2pos.y) ))
                {
                    var pPos = puyos[i, j].transform.position;
                    if (pPos.x <= p1pos.x || pPos.x <= p2pos.x) return false;
                }

                if (p1pos.x <= -4f || p2pos.x <= -4f) return false;
            }
        }

        return true;
    }

    private bool canGoRight()
    {
        var p1pos = fallingPair.Puyo1.transform.position;
        var p2pos = fallingPair.Puyo1.transform.position;

        for (int i = 0; i < GameVariable.Rows; i++)
        {
            for (int j = 0; j < GameVariable.Columns; j++)
            {
                if (puyos[i, j] != null && (i == getRowFromYPosition(p1pos.y) || i == getRowFromYPosition(p2pos.y)))
                {
                    var pPos = puyos[i, j].transform.position;
                    if (pPos.x <= p1pos.x || pPos.x <= p2pos.x) return false;
                }

                if (p1pos.x >= 1f || p2pos.x >= 1f) return false;
            }
        }

        return true;
    }

    private void HandleFall()
    {
        if (fallingPair == null)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow) && canGoLeft())
        {
            fallingPair.Puyo1.transform.position -= new Vector3(1f, 0f);
            fallingPair.Puyo2.transform.position -= new Vector3(1f, 0f);   
        }

        if (Input.GetKeyDown(KeyCode.RightArrow) && canGoRight())
        {
            fallingPair.Puyo1.transform.position += new Vector3(1f, 0f);
            fallingPair.Puyo2.transform.position += new Vector3(1f, 0f);
        }

        bool canFall = canPairFall();
        if (canFall)
        {
            fallingPair.Puyo1.transform.position -= fallingSpeed*Time.deltaTime;
            fallingPair.Puyo2.transform.position -= fallingSpeed*Time.deltaTime;
        }
        else
        {
            FixPair();
            gameState = GameState.CheckAndDestroy;
        }
        
    }

    private void FixPair()
    {
        // todo correct puyo position and add to array and add column and row properties

        // Round Pos
        var p1pos = fallingPair.Puyo1.transform.position;
        var roundedP1PosY = (float)Math.Round(p1pos.y);
        fallingPair.Puyo1.transform.position = new Vector3(p1pos.x, roundedP1PosY);

        var p2pos = fallingPair.Puyo2.transform.position;
        var roundedP2PosY = (float)Math.Round(p2pos.y);
        fallingPair.Puyo2.transform.position = new Vector3(p2pos.x, roundedP2PosY);


        // Find Column and Row
        var p1col = getColumnFromXPosition(fallingPair.Puyo1.transform.position.x);
        var p1row = getRowFromYPosition(fallingPair.Puyo1.transform.position.y);

        var p2col = getColumnFromXPosition(fallingPair.Puyo2.transform.position.x);
        var p2row = getRowFromYPosition(fallingPair.Puyo2.transform.position.y);

        Debug.Log(string.Format("p1 col : {0}, p1 row : {1}\np2col : {2}, p2row : {3}", p1col, p1row, p2col, p2row));

        // Update array
        puyos[p1row, p1col] = fallingPair.Puyo1;
        puyos[p1row, p1col].GetComponent<Puyo>().Column = p1col;
        puyos[p1row, p1col].GetComponent<Puyo>().Row = p1row;

        puyos[p2row, p2col] = fallingPair.Puyo2;
        puyos[p2row, p2col].GetComponent<Puyo>().Column = p2col;
        puyos[p2row, p2col].GetComponent<Puyo>().Row = p2row;

        // ResetFallingPair
        fallingPair = null;
    }

    private int getColumnFromXPosition(float x)
    {
        var col = x + 4;
        return (int)col;
    }

    private int getRowFromYPosition(float y)
    {
        var row = y + 5;
        return (int)row;
    }

    public void SpawnNewPair()
    {
        if (pairs.Any())
        {
            fallingPair = pairs.First();
            fallingPair.Puyo1.transform.position = new Vector3(-2f, 7f);
            fallingPair.Puyo2.transform.position = new Vector3(-2f, 8f);
            
            pairs.Remove(pairs.First());
        }
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
            for (int j = 0; j < GameVariable.Columns; j++)
            {
                if (i < 0 && puyos[i - 1, j] != null)
                {
                    var pos = puyos[i, j].transform.position;
                    pos.y -= puyoSize.y;
                    puyos[i, j].transform.positionTo(2f, pos);
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
                for (int j = 0; j < GameVariable.Columns; j++)
                {
                    var puyoGo = puyos[i, j];
                    if (puyoGo != null)
                    {
                        // Puyo under is null
                        if (puyos[i - 1, j] == null)
                        {
                            // Shift all top puyos
                            for (int k = i; k < GameVariable.Rows; k++)
                            {
                                var topPuyo = puyos[k, j];
                                if (topPuyo != null)
                                {
                                    puyos[k - 1, j] = topPuyo;
                                    puyos[k - 1, j].GetComponent<Puyo>().Row--;
                                    puyos[k, j] = null;
                                }
                            }
                            puyoHasToMove = true;
                        }
                    }
                }
            }
        }
    }

    private Vector3 FindPuyoScreenPosition(int row, int column)
    {
        return new Vector3((bottomLeft.y + column * puyoSize.y), bottomLeft.x + row * puyoSize.x);
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
            for (int j = 0; j < GameVariable.Columns; j++)
            {
                var puyo = puyos[i, j];
                if (puyo != null)
                {
                    var oldPosition = puyo.transform.position;
                    var newPosition = FindPuyoScreenPosition(i, j);

                    // Position is correct, skip and continue with the next puyo
                    if (oldPosition.y == newPosition.y) continue;

                    var ratio = (oldPosition.y - newPosition.y) /puyoSize.y;
                    if (ratio > biggestRatio) biggestRatio = ratio;
                    
                    puyo.transform.positionTo(GameVariable.PuyoRepositioningSpeed * ratio, newPosition);
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
            for (int j = 0; j < GameVariable.Columns; j++)
            {
                var rand = Convert.ToInt32(UnityEngine.Random.Range(0, 4));

                var position = FindPuyoScreenPosition(i, j);

                var go = Instantiate(puyoPrefabs[rand], position, Quaternion.identity) as GameObject;
                var puyoColor = PuyoHelper.GetPuyoColorFromString(go.tag);
                go.GetComponent<Puyo>().Initialize(puyoColor, i, j);

                puyos[i, j] = go;
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
                foreach (var puyo in chain.Puyos)
                {
                    StartCoroutine(AnimateAndDestroy(puyo));
                }
            }
        }
        else
        {
            gameState = GameState.Spawning;
        }
        
    }
    
    private IEnumerator AnimateAndDestroy(Puyo puyo)
    {
        var animationComponent = puyos[puyo.Row, puyo.Column].GetComponent<Animation>();
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
        Destroy(puyos[puyo.Row, puyo.Column]);
        puyos[puyo.Row, puyo.Column] = null;
    }

    private List<PuyoGroup> FindAllChains()
    {
        var chains = new List<PuyoGroup>();

        for (int i = 0; i < GameVariable.Rows; i++)
        {
            for (int j = 0; j < GameVariable.Columns; j++)
            {
                var p = puyos[i, j];

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

        return chains;
    }

    private PuyoGroup FindChain(Puyo puyo)
    {
        var currentChain = new List<Puyo> {puyo};
        var nextPuyosToCheck = new List<Puyo> {puyo};

        while (nextPuyosToCheck.Any())
        {
            var pi = nextPuyosToCheck.First();
            var nextInChain = findNextPuyoInChain(pi, currentChain);
            while (nextInChain != null)
            {
                currentChain.Add(nextInChain);
                nextPuyosToCheck.Add(nextInChain);
                nextInChain = findNextPuyoInChain(pi, currentChain);
            }
            nextPuyosToCheck.Remove(pi);
        }

        return currentChain.Count >= GameVariable.MinimumMatches ? new PuyoGroup(currentChain) : null;
    }

    private Puyo findNextPuyoInChain(Puyo puyo, IEnumerable<Puyo> ignoredPuyos)
    {
        var ignoredList = ignoredPuyos.ToList();

        // Top
        if (puyo.Row < GameVariable.Rows - 1 && (ignoredPuyos == null || !ignoredList.Any(p => p.Row == puyo.Row + 1 && p.Column == puyo.Column)))
        {
            var topPuyo = puyos[puyo.Row + 1, puyo.Column];
            if (topPuyo != null && topPuyo.GetComponent<Puyo>().Color == puyo.Color)
                return topPuyo.GetComponent<Puyo>();
        }

        // Bottom
        if (puyo.Row > 0 && (ignoredPuyos == null || !ignoredList.Any(p => p.Row == puyo.Row - 1 && p.Column == puyo.Column)))
        {
            var bottomPuyo = puyos[puyo.Row - 1, puyo.Column];
            if (bottomPuyo != null && bottomPuyo.GetComponent<Puyo>().Color == puyo.Color)
                return bottomPuyo.GetComponent<Puyo>();
        }

        // Right
        if (puyo.Column < GameVariable.Columns - 1 && (ignoredPuyos == null || !ignoredList.Any(p => p.Row == puyo.Row && p.Column == puyo.Column + 1)))
        {
            var rightPuyo = puyos[puyo.Row, puyo.Column + 1];
            if (rightPuyo != null && rightPuyo.GetComponent<Puyo>().Color == puyo.Color)
                return rightPuyo.GetComponent<Puyo>();
        }

        // Left
        if (puyo.Column > 0 && (ignoredPuyos == null || !ignoredList.Any(p => p.Row == puyo.Row && p.Column == puyo.Column - 1)))
        {
            var leftPuyo = puyos[puyo.Row, puyo.Column - 1];
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

public enum GameState
{
    Busy,
    Spawning,
    Falling,
    CheckAndDestroy,
    Repositioning,
    GameOver
}
