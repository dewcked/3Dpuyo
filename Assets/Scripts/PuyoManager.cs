using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Animations;

public class PuyoManager : MonoBehaviour {

	GameObject[,] puyos = new GameObject[GameVariable.Rows, GameVariable.Columns];

	GameState state = GameState.None;
	Vector2 bottomLeft = new Vector2 (-5f, -4f);

	Vector2 puyoSize = new Vector2(1f, 1f);

	[SerializeField]
	private GameObject[] puyoPrefabs;


	void Awake(){
		InitArray ();

		//StartCoroutine (DestroyCoroutine());
        state = GameState.CheckAndDestroy;
	}

    private bool isBusy = false;

    void FixedUpdate()
    {
        if (isBusy) return;

        switch (state)
        {
            case GameState.None:
                break;
            case GameState.Falling:
                break;
            case GameState.CheckAndDestroy:
                isBusy = true;
                DestroyAllChains();
                state = GameState.Repositioning;
                isBusy = false;
                break;
            case GameState.Repositioning:
                isBusy = true;
                //StartCoroutine(ComputeNewPuyosPosition());
                //StartCoroutine(MovePuyosToNewPosition());
                state = GameState.None;
                isBusy = false;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void UpdatePuyosPosition()
    {
        ComputeNewPuyosPosition();
        MovePuyosToNewPosition();
    }

    private void MovePuyos()
    {
        //yield return new WaitForSeconds(10f);

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

    private void ComputeNewPuyosPosition()
    {
        //yield return new WaitForSeconds(10f);

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
                        // PUYO UNDER IS NULL
                        if (puyos[i - 1, j] == null)
                        {
                            // SHIFT ALL TOP PUYOS
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
    
    void MovePuyosToNewPosition()
    {
        //yield return new WaitForSeconds(15f);

        for (int i = 0; i < GameVariable.Rows; i++)
        {
            for (int j = 0; j < GameVariable.Columns; j++)
            {
                var puyo = puyos[i, j];
                if (puyo != null)
                {
                    var oldPosition = puyo.transform.position;
                    var newPosition = new Vector3((bottomLeft.y + j * puyoSize.y), bottomLeft.x + i * puyoSize.x); // todo externalize (used in Init and here)

                    var ratio = (oldPosition.y - newPosition.y) /puyoSize.y;
                    var speed = 0.3f; // todo externalize

                    // Without tween
                    //puyo.transform.position = newPosition;

                    // With tween
                    puyo.transform.positionTo(speed * ratio, newPosition);
                }
                
            }
        }
    }

    private void InitArray()
    {
        for (int i = 0; i < GameVariable.Rows; i++)
        {
            for (int j = 0; j < GameVariable.Columns; j++)
            {
                var rand = Convert.ToInt32(UnityEngine.Random.Range(0, 4));

                var position = new Vector3((bottomLeft.y + j*puyoSize.y), bottomLeft.x + i*puyoSize.x);

                var go = Instantiate(puyoPrefabs[rand], position, Quaternion.identity) as GameObject;
                var puyoColor = GetPuyoColorFromString(go.tag);
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

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(5f);
    }

    IEnumerator AnimateAndDestroy(Puyo puyo)
    {
        var animationComponent = puyos[puyo.Row, puyo.Column].GetComponent<Animation>();
        animationComponent.Play();

        do
        {
            yield return null;
        } while (animationComponent.isPlaying);

        Destroy(puyos[puyo.Row, puyo.Column]);
        puyos[puyo.Row, puyo.Column] = null;
    }

    private PuyoColor GetPuyoColorFromString(string str)
    {
        switch (str)
        {
            case "Blue":
                return PuyoColor.Blue;
            case "Green":
                return PuyoColor.Green;
            case "Yellow":
                return PuyoColor.Yellow;
            case "Red":
                return PuyoColor.Red;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void DestroyAllOfColor(PuyoColor color)
    {
        for (int i = 0; i < GameVariable.Rows; i++)
        {
            for (int j = 0; j < GameVariable.Columns; j++)
            {
                if (puyos[i, j].GetComponent<Puyo>().Color == color)
                {
                    Destroy(puyos[i, j]);
                    puyos[i, j] = null;
                }
            }
        }
    }

    private IEnumerator DestroyCoroutine()
    {
        yield return new WaitForSeconds(5f);

        //DestroyAllOfColor (PuyoColor.Red);
        DestroyAllChains();
    }

    public List<PuyoGroup> FindAllChains()
    {
        var chains = new List<PuyoGroup>();

        for (int i = 0; i < GameVariable.Rows; i++)
        {
            for (int j = 0; j < GameVariable.Columns; j++)
            {
                var p = puyos[i, j];

                // Is null ?
                if (p == null) continue;

                var pscript = p.GetComponent<Puyo>();

                // Is already in a group ?
                bool alreadyInGroup = false;
                foreach (var group in chains)
                {
                    if (group.ContainPuyo(pscript))
                    {
                        alreadyInGroup = true;
                        continue;
                    }
                }
                if (alreadyInGroup) continue;

                var newGroup = FindChain(pscript);

                if (newGroup != null) chains.Add(newGroup);
            }
        }

        return chains;
    }

    public PuyoGroup FindChain(Puyo puyo)
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

        return currentChain.Count >= 4 ? new PuyoGroup(currentChain) : null;
    }

    private Puyo findNextPuyoInChain(Puyo puyo, IEnumerable<Puyo> ignoredPuyos)
    {
        var ignoredList = ignoredPuyos.ToList();

        // TOP
        if (puyo.Row < GameVariable.Rows - 1 && (ignoredPuyos == null || !ignoredList.Any(p => p.Row == puyo.Row + 1 && p.Column == puyo.Column)))
        {
            var topPuyo = puyos[puyo.Row + 1, puyo.Column];
            if (topPuyo != null && topPuyo.GetComponent<Puyo>().Color == puyo.Color)
                return topPuyo.GetComponent<Puyo>();
        }

        // BOTTOM
        if (puyo.Row > 0 && (ignoredPuyos == null || !ignoredList.Any(p => p.Row == puyo.Row - 1 && p.Column == puyo.Column)))
        {
            var bottomPuyo = puyos[puyo.Row - 1, puyo.Column];
            if (bottomPuyo != null && bottomPuyo.GetComponent<Puyo>().Color == puyo.Color)
                return bottomPuyo.GetComponent<Puyo>();
        }

        // RIGHT
        if (puyo.Column < GameVariable.Columns - 1 && (ignoredPuyos == null || !ignoredList.Any(p => p.Row == puyo.Row && p.Column == puyo.Column + 1)))
        {
            var rightPuyo = puyos[puyo.Row, puyo.Column + 1];
            if (rightPuyo != null && rightPuyo.GetComponent<Puyo>().Color == puyo.Color)
                return rightPuyo.GetComponent<Puyo>();
        }

        // LEFT
        if (puyo.Column > 0 && (ignoredPuyos == null || !ignoredList.Any(p => p.Row == puyo.Row && p.Column == puyo.Column - 1)))
        {
            var leftPuyo = puyos[puyo.Row, puyo.Column - 1];
            if (leftPuyo != null && leftPuyo.GetComponent<Puyo>().Color == puyo.Color)
                return leftPuyo.GetComponent<Puyo>();
        }

        // Nothing is found, return null
        return null;
    }

    public bool HasSameColorNeighbor(Puyo puyo)
    {
        return
            // TOP
            (puyo.Row < GameVariable.Rows - 1 && puyos[puyo.Row + 1, puyo.Column] != null && puyos[puyo.Row + 1, puyo.Column].GetComponent<Puyo>().Color == puyo.Color) ||

            // BOT
            (puyo.Row > 0 && puyos[puyo.Row - 1, puyo.Column] != null && puyos[puyo.Row - 1, puyo.Column].GetComponent<Puyo>().Color == puyo.Color) ||

            // RIGHT
            (puyo.Column < GameVariable.Columns - 1 && puyos[puyo.Row, puyo.Column + 1] != null && puyos[puyo.Row, puyo.Column + 1].GetComponent<Puyo>().Color == puyo.Color) ||

            // LEFT
            (puyo.Column > 0 && puyos[puyo.Row, puyo.Column - 1] != null && puyos[puyo.Row, puyo.Column - 1].GetComponent<Puyo>().Color == puyo.Color);
    }
    
    private void MovePuyosToNewPosition(GameObject[,] newArray)
    {
        // todo
    }
}

public class PuyoGroup
{
    public PuyoGroup(IEnumerable<Puyo> puyos)
    {
        this.Puyos = puyos;
    }

    public IEnumerable<Puyo> Puyos { get; set; }

    public bool ContainPuyo(Puyo puyo)
    {
        return Puyos.Any(p => p.Row == puyo.Row && p.Column == puyo.Column);
    }
}

public enum GameState
{
    None,
    Falling,
    CheckAndDestroy,
    Repositioning
}
