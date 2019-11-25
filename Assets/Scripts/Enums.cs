/// <summary>
/// Puyo Color.
/// 뿌요의 색.
/// </summary>
public enum PuyoColor {
	Blue,
	Red,
	Yellow,
	Green
}

/// <summary>
/// Puyo Orientation.
/// 뿌요의 방향.
/// </summary>
public enum Orientation
{
    PosA,
    NegA,
    PosB,
    NegB,
    VerticalA,
    VerticalB
}

/// <summary>
/// Control of Puyo.
/// 뿌요 컨트롤. Screen은 오른쪽으로 회전하면 1씩 증가
/// </summary>
public enum Control
{
    MovePosA = 0,
    MoveNegB = 1,
    MoveNegA = 2,
    MovePosB = 3,
    RotateRightA = 4,
    RotateBack = 5,
    RotateLeftA = 6,
    RotateForth = 7,
    Drop,
    Fall,
    RotateHorizontalLeft,
    RotateHorizontalRight,
    Screen1 = 0,
    Screen2 = 1,
    Screen3 = 2,
    Screen4 = 3
}
/// <summary>
/// 게임상태
/// </summary>
public enum GameState
{
    Busy,
    Spawning,
    Falling,
    CheckAndDestroy,
    Repositioning,
    GameOver,
    Generate
}