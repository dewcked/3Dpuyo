using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Puyo Group.
/// 뿌요 그룹.
/// </summary>
public class PuyoGroup
{
    /// <summary>
    /// Initializer.
    /// 생성자.
    /// </summary>
    /// <param name="puyos">뿌요 그룹 목록</param>
    public PuyoGroup(IEnumerable<Puyo> puyos)
    {
        this.Puyos = puyos;
    }

    /// <summary>
    /// Puyo group list.
    /// 뿌요 그룹 목록.
    /// </summary>
    public IEnumerable<Puyo> Puyos { get; set; }

    /// <summary>
    /// Check if contains Puyo.
    /// 뿌요 포함 여부 조사.
    /// </summary>
    /// <param name="puyo">검사할 뿌요</param>
    /// <returns>Any Puyo in the group that given Puyo is in. 해당 뿌요가 속한 그룹의 모든 뿌요 목록.</returns>
    public bool ContainPuyo(Puyo puyo)
    {
        if (puyo == null)
            throw new ArgumentNullException("puyo");

        return Puyos.Any(p => p.Row == puyo.Row && p.ColumnA == puyo.ColumnA && p.ColumnB == puyo.ColumnB);
    }
}