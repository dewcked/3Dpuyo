using UnityEngine;

/// <summary>
/// Game variable.
/// 게임 변수.
/// </summary>
public static class GameVariable {
    /// <summary>
    /// Total rows of game screen.
    /// 게임 화면의 행의 갯수.
    /// </summary>
	public static int Rows = 15;
    /// <summary>
    /// Total columns of game screen.
    /// 게임 화면의 열 A의 갯수.
    /// </summary>
	public static int ColumnsA = 3;
    /// <summary>
    /// Total columns of game screen.
    /// 게임 화면의 열 B의 갯수.
    /// </summary>
    public static int ColumnsB = 3;
    /// <summary>
    /// Mininum matches to break Puyo group.
    /// 연쇄에 필요한 최소 뿌요 갯수.
    /// </summary>
	public static int MinimumMatches = 4;
    /// <summary>
    /// Puyo repositioning speed.
    /// 뿌요를 터트린 후 위치 변경 속도.
    /// </summary>
    public static float PuyoRepositioningSpeed = 0.10f;
    /// <summary>
    /// Number of Puyo pairs to generate.
    /// 이 게임에서 생성할 뿌요의 갯수.
    /// </summary>
    public static int NumberOfPairToGenerate = 1000;
    /// <summary>
    /// Base point of game screen.
    /// 게임 화면외 왼쪽 아래 기준점.
    /// </summary>
    public static Vector3 BasePoint = new Vector3(0f, 0f, 0f);
    /// <summary>
    /// Size of Puyo.
    /// 뿌요의 가로 세로 크기.
    /// </summary>
    public static Vector3 PuyoSize = new Vector3(1f, 1f, 1f);

    public static Control Scene = Control.Screen1;

    public static Vector3 MidPuyo = new Vector3(GameVariable.ColumnsA / 2 - GameVariable.BasePoint.x, 0f, GameVariable.ColumnsB / 2 - GameVariable.BasePoint.z);
}
