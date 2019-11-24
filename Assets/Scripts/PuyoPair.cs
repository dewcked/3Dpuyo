using UnityEngine;

/// <summary>
/// Puyo pair.
/// 뿌요 쌍.
/// </summary>
public class PuyoPair
{
    /// <summary>
    /// First(Base) Puyo.
    /// 첫 번째(기준) 뿌요.
    /// </summary>
    public GameObject Puyo1 { get; set; }
    /// <summary>
    /// Second Puyo.
    /// 두 번째 뿌요.
    /// </summary>
    public GameObject Puyo2 { get; set; }
    /// <summary>
    /// Orientation of Puyo pair.
    /// 뿌요 쌍의 방향.
    /// </summary>
    public Orientation Orientation { get; set; }
    /// <summary>
    /// Initializer.
    /// 생성자.
    /// </summary>
    /// <param name="puyo1">첫 번째 뿌요</param>
    /// <param name="puyo2">두 번째 뿌요</param>
    public PuyoPair(GameObject puyo1, GameObject puyo2)
    {
        Puyo1 = puyo1;
        Puyo2 = puyo2;
        Orientation = Orientation.Vertical;
    }
}