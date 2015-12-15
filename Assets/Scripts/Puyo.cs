using System.Linq;
using UnityEditor;
using UnityEngine;

public class Puyo : MonoBehaviour
{
    public int Row { get; set; }

    public int Column { get; set; }

    public PuyoColor Color { get; set; }

    public void Initialize(int row, int column, PuyoColor color)
    {
        Row = row;
        Column = column;
        Color = color;
    }

    private void OnMouseDown()
    {
        //EditorUtility.DisplayDialog("Puyo Position", string.Format("Row {0}, Column {1}", this.Row, this.Column), "ok");


        var manager = GameObject.Find("PuyoManager");
        var managerScript = manager.GetComponent<PuyoManager>();
        //var hasSameColorNeighbor = managerScript.HasSameColorNeighbor(this);


        managerScript.UpdatePuyosPosition();

        //var chainCount = managerScript.FindChain(this);

        //EditorUtility.DisplayDialog("Toast", chainCount.Puyos.Count().ToString(), "ok");
    }
}