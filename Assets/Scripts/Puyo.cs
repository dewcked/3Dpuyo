using UnityEngine;
using System.Collections;

public class Puyo : MonoBehaviour {

	public int Row {
		get;
		set;
	}

	public int Column {
		get;
		set;
	}

	public PuyoColor Color {
		get;
		set;
	}

	public void Initialize (int row, int column, PuyoColor color)
	{
		this.Row = row;
		this.Column = column;
		this.Color = color;
	}

}
