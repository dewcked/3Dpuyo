using UnityEngine;
using System;
using System.Collections;


public class PuyoManager : MonoBehaviour {

	GameObject[,] puyos = new GameObject[GameVariable.Rows, GameVariable.Columns];

	GameState state = GameState.None;
	Vector2 bottomLeft = new Vector2 (-5f, -4f);

	Vector2 puyoSize = new Vector2(1f, 1f);

	[SerializeField]
	private GameObject[] puyoPrefabs;


	void Awake(){
		initArray ();

		StartCoroutine (DestroyCoroutine());
	}


	private void initArray(){
		for (int i = 0; i < GameVariable.Rows; i++) {
			for (int j = 0; j < GameVariable.Columns; j++) {
				var rand = Convert.ToInt32(UnityEngine.Random.Range(0, 4));

				var position = new Vector3(-(bottomLeft.y + j * puyoSize.y), bottomLeft.x + i * puyoSize.x);

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
				break;
			case "Green":
				return PuyoColor.Green;
				break;
			case "Yellow":
				return PuyoColor.Yellow;
				break;
			case "Red":
				return PuyoColor.Red;
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
		return PuyoColor.Blue;
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

	//GameObject[,] ComputeNewPositions(){
		// todo
	//}

	void MovePuyosToNewPosition(GameObject[,] newArray){
		// todo
	}
}

public enum GameState{
	None,
	Falling,
	Destroying,
	Replacing
}
