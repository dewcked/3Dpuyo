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
    HorizontalA,
    HorizontalB,
    Vertical
}

/// <summary>
/// Control of Puyo.
/// 뿌요 컨트롤.
/// </summary>
public enum Control
{
    Fall,
    Left,
    Right,
    Drop,
    Back,
    Forth,
    RotateLeft,
    RotateRight
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