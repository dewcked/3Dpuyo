using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEditor;

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

    void OnMouseDown()
    {

        //EditorUtility.DisplayDialog("Puyo Position", string.Format("Row {0}, Column {1}", this.Row, this.Column), "ok");


        var manager = GameObject.Find("PuyoManager");
        var managerScript = (PuyoManager)manager.GetComponent<PuyoManager>();
        //var hasSameColorNeighbor = managerScript.HasSameColorNeighbor(this);


        var chainCount = managerScript.FindChain(this);

        EditorUtility.DisplayDialog("Toast", chainCount.Count().ToString(), "ok");
    }


}
