using UnityEngine;

/// <summary>
/// Puyo.
/// 뿌요.
/// </summary>
public class Puyo : MonoBehaviour
{
    /// <summary>
    /// Current row of puyo.
    /// 현재 뿌요가 있는 행.
    /// </summary>
    public int Row { get; set; }
    /// <summary>
    /// Current column of puyo.
    /// 현재 뿌요가 있는 열 A.
    /// </summary>
    public int ColumnA { get; set; }
    /// <summary>
    /// Current column of puyo.
    /// 현재 뿌요가 있는 열 B.
    /// </summary>
    public int ColumnB { get; set; }
    /// <summary>
    /// Current color of puyo.
    /// 현재 뿌요의 색.
    /// </summary>
    public PuyoColor Color { get; set; }
    /// <summary>
    /// Initialize puyo.
    /// 뿌요 초기화.
    /// </summary>
    /// <param name="color">뿌요의 색</param>
    /// <param name="row">뿌요의 행</param>
    /// <param name="columnA">뿌요의 열 A</param>
    /// <param name="columnB">뿌요의 열 B</param>
    public void Initialize(PuyoColor color, int row = 50, int columnA = 50, int columnB = 50)
    {
        Row = row;
        ColumnA = columnA;
        ColumnB = columnB;
        Color = color;
    }
}