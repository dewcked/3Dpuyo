using UnityEngine;

/// <summary>
/// Puyo pair.
/// �ѿ� ��.
/// </summary>
public class PuyoPair
{
    /// <summary>
    /// First(Base) Puyo.
    /// ù ��°(����) �ѿ�.
    /// </summary>
    public GameObject Puyo1 { get; set; }
    /// <summary>
    /// Second Puyo.
    /// �� ��° �ѿ�.
    /// </summary>
    public GameObject Puyo2 { get; set; }
    /// <summary>
    /// Orientation of Puyo pair.
    /// �ѿ� ���� ����.
    /// </summary>
    public Orientation Orientation { get; set; }
    /// <summary>
    /// Initializer.
    /// ������.
    /// </summary>
    /// <param name="puyo1">ù ��° �ѿ�</param>
    /// <param name="puyo2">�� ��° �ѿ�</param>
    public PuyoPair(GameObject puyo1, GameObject puyo2)
    {
        Puyo1 = puyo1;
        Puyo2 = puyo2;
        Orientation = Orientation.Vertical;
    }
}