using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PuyoManager : MonoBehaviour {

	GameObject[,] puyos = new GameObject[GameVariable.Rows, GameVariable.Columns];

	GameState state = GameState.None;
	Vector2 bottomLeft = new Vector2 (-5f, -4f);

	Vector2 puyoSize = new Vector2(1f, 1f);

	[SerializeField]
	private GameObject[] puyoPrefabs;


	void Awake(){
		InitArray ();

		StartCoroutine (DestroyCoroutine());
	}


	private void InitArray(){
		for (int i = 0; i < GameVariable.Rows; i++) {
			for (int j = 0; j < GameVariable.Columns; j++) {
				var rand = Convert.ToInt32(UnityEngine.Random.Range(0, 4));

				var position = new Vector3((bottomLeft.y + j * puyoSize.y), bottomLeft.x + i * puyoSize.x);

				var go = Instantiate(puyoPrefabs[rand], position, Quaternion.identity) as GameObject;
				var puyoColor = GetPuyoColorFromString(go.tag);
				go.GetComponent<Puyo>().Initialize(i, j, puyoColor);

				puyos[i,j] = go;
			}
		}
	}

	private PuyoColor GetPuyoColorFromString(string str){
		switch (str) {
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
			                                
	private void DestroyAllOfColor(PuyoColor color){
		for (int i = 0; i < GameVariable.Rows; i++) {
			for (int j = 0; j < GameVariable.Columns; j++) {

				if(puyos[i,j].GetComponent<Puyo>().Color == color){
					Destroy(puyos[i,j]);
					puyos[i,j] = null;
				}

			}
		}
	}

	IEnumerator DestroyCoroutine(){
		yield return new WaitForSeconds(5f);

		DestroyAllOfColor (PuyoColor.Red);
	}
    
    public IEnumerable<Puyo> FindChain(Puyo puyo)
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
        
        return currentChain;
    }

    private Puyo findNextPuyoInChain(Puyo puyo, IEnumerable<Puyo> ignoredPuyos)
    {
        var ignoredList = ignoredPuyos.ToList();

        // TOP
        if (puyo.Row < GameVariable.Rows - 1 &&
            (ignoredPuyos == null || !ignoredList.Any(p => p.Row == puyo.Row + 1 && p.Column == puyo.Column)))
        {
            var topPuyo = puyos[puyo.Row + 1, puyo.Column];
            if (topPuyo != null &&
                topPuyo.GetComponent<Puyo>().Color == puyo.Color)
                return topPuyo.GetComponent<Puyo>();
        }
        
        // BOTTOM
        if (puyo.Row > 0 &&
            (ignoredPuyos == null || !ignoredList.Any(p => p.Row == puyo.Row - 1 && p.Column == puyo.Column)))
        {
            var bottomPuyo = puyos[puyo.Row - 1, puyo.Column];
            if (bottomPuyo != null &&
                bottomPuyo.GetComponent<Puyo>().Color == puyo.Color)
                return bottomPuyo.GetComponent<Puyo>();
        }
        
        // RIGHT
        if (puyo.Column < GameVariable.Columns - 1 &&
            (ignoredPuyos == null || !ignoredList.Any(p => p.Row == puyo.Row && p.Column == puyo.Column + 1)))
        {
            var rightPuyo = puyos[puyo.Row, puyo.Column + 1];
            if (rightPuyo != null &&
                rightPuyo.GetComponent<Puyo>().Color == puyo.Color)
                return rightPuyo.GetComponent<Puyo>();
        }
        
        // LEFT
        if (puyo.Column > 0 &&
            (ignoredPuyos == null || !ignoredList.Any(p => p.Row == puyo.Row && p.Column == puyo.Column - 1)))
        {
            var leftPuyo = puyos[puyo.Row, puyo.Column - 1];
            if (leftPuyo != null &&
                leftPuyo.GetComponent<Puyo>().Color == puyo.Color)
                return leftPuyo.GetComponent<Puyo>();
        }
        
        // Nothing is found, return null
        return null;
    }

    public bool HasSameColorNeighbor(Puyo puyo)
    {
        return
                // TOP
                (puyo.Row < GameVariable.Rows - 1 &&
                puyos[puyo.Row + 1, puyo.Column] != null &&
                puyos[puyo.Row + 1, puyo.Column].GetComponent<Puyo>().Color == puyo.Color) ||

               // BOT
               (puyo.Row > 0 &&
                puyos[puyo.Row - 1, puyo.Column] != null &&
                puyos[puyo.Row - 1, puyo.Column].GetComponent<Puyo>().Color == puyo.Color) ||

               // RIGHT
               (puyo.Column < GameVariable.Columns - 1 &&
               puyos[puyo.Row, puyo.Column + 1] != null &&
                puyos[puyo.Row, puyo.Column + 1].GetComponent<Puyo>().Color == puyo.Color) ||

               // LEFT
               (puyo.Column > 0 &&
                puyos[puyo.Row, puyo.Column - 1] != null &&
                puyos[puyo.Row, puyo.Column - 1].GetComponent<Puyo>().Color == puyo.Color);
    }

    void MovePuyosToNewPosition(GameObject[,] newArray){
		// todo
	}
}

public class PuyoChain
{
    public IEnumerable<Vector2> PuyoPositions { get; set; }
}

public enum GameState{
	None,
	Falling,
	Destroying,
	Replacing
}
