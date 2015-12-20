using UnityEngine;

public class PuyoPair
{
    public GameObject Puyo1 { get; set; }

    public GameObject Puyo2 { get; set; }

    public Orientation Orientation { get; set; }

    public PuyoPair(GameObject puyo1, GameObject puyo2)
    {
        Puyo1 = puyo1;
        Puyo2 = puyo2;
        Orientation = Orientation.Vertical;
    }

    // todo
    public void RotateRight()
    {
        //var oldP1Pos = Puyo1.transform.position;
        //var oldP2Pos = Puyo2.transform.position;
        //if (Orientation == Orientation.Vertical)
        //{
        //    Puyo1.transform.position = new Vector3(oldP1Pos.x, oldP1Pos.y);
        //    Puyo2.transform.position = new Vector3(oldP2Pos.x + GameVariable.PuyoSize.x, oldP2Pos.y - GameVariable.PuyoSize.y);
        //    Orientation = Orientation.Horizontal;
        //}
        //else
        //{
        //    Puyo1.transform.position = new Vector3(oldP1Pos.x, oldP1Pos.y);
        //    Puyo2.transform.position = new Vector3(oldP2Pos.x - GameVariable.PuyoSize.x, oldP2Pos.y + GameVariable.PuyoSize.y);
        //    Orientation = Orientation.Vertical;
        //}
    }

    public void RotateLeft()
    {
        var oldP1Pos = Puyo1.transform.position;
        var oldP2Pos = Puyo2.transform.position;
        if (Orientation == Orientation.Vertical)
        {
            Puyo1.transform.position = new Vector3(oldP1Pos.x, oldP1Pos.y);
            Puyo2.transform.position = new Vector3(oldP2Pos.x - GameVariable.PuyoSize.x, oldP2Pos.y - GameVariable.PuyoSize.y);
            Orientation = Orientation.Horizontal;
        }
        else
        {
            Puyo1.transform.position = new Vector3(oldP1Pos.x, oldP1Pos.y);
            Puyo2.transform.position = new Vector3(oldP2Pos.x + GameVariable.PuyoSize.x, oldP2Pos.y + GameVariable.PuyoSize.y);
            Orientation = Orientation.Vertical;
        }
    }
}
