using UnityEngine;

public class Puyo : MonoBehaviour
{
    public int Row { get; set; }

    public int Column { get; set; }

    public PuyoColor Color { get; set; }

    public void Initialize(PuyoColor color, int row = 50, int column = 50)
    {
        Row = row;
        Column = column;
        Color = color;
    }
}