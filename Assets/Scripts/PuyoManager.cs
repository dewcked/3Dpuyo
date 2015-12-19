using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;

public class PuyoManager : MonoBehaviour {

    readonly GameObject[,] puyos = new GameObject[GameVariable.Rows, GameVariable.Columns];

	GameState gameState;

	Vector2 bottomLeft = new Vector2 (-5f, -4f);

	Vector2 puyoSize = new Vector2(1f, 1f);

	[SerializeField]
	private GameObject[] puyoPrefabs;
    
	void Awake(){
		InitRandomArray ();

        gameState = GameState.CheckAndDestroy;
	}

    void Update()
    {
        switch (gameState)
        {
            case GameState.Busy:
                break;
            case GameState.Falling:
                break;
            case GameState.CheckAndDestroy:
                gameState = GameState.Busy;
                DestroyAllChains();
                break;
            case GameState.Repositioning:
                gameState = GameState.Busy;
                UpdatePuyosPosition();
                break;
            default:
                throw new ArgumentOutOfRangeException();
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
        for (int i = 0; i < GameVariable.Rows; i++)
        {
            for (int j = 0; j < GameVariable.Columns; j++)
            {
                var rand = Convert.ToInt32(UnityEngine.Random.Range(0, 4));

                var position = FindPuyoScreenPosition(i, j);

                var go = Instantiate(puyoPrefabs[rand], position, Quaternion.identity) as GameObject;
                var puyoColor = PuyoHelper.GetPuyoColorFromString(go.tag);
                go.GetComponent<Puyo>().Initialize(i, j, puyoColor);

                puyos[i, j] = go;
            }
        }
    }

    private void DestroyAllChains()
    {
        var chains = FindAllChains();
        foreach (var chain in chains)
        {
            foreach (var puyo in chain.Puyos)
            {
                StartCoroutine(AnimateAndDestroy(puyo));
            }
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
    Falling,
    CheckAndDestroy,
    Repositioning
}
