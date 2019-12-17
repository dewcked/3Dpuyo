using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using UnityEngine.Events;

public class PuyoManager : MonoBehaviour
{
    readonly GameObject[,,] puyos = new GameObject[GameVariable.Rows, GameVariable.ColumnsA, GameVariable.ColumnsB];
    public PuyoPair fallingPair;
    public List<PuyoPair> pairs = new List<PuyoPair>();

    private bool firstRotate = true;

    [SerializeField]
    private GameObject[] puyoPrefabs;

    public SoundManager SoundManager;

    void Awake()
    {
        SoundManager = transform.FindChild("SoundManager").gameObject.GetComponent<SoundManager>();
    }

    private void Update()
    {
        if (pairs.Count >= 2)
            ShowNextPuyo();
    }

    // Generate
    public IEnumerator GenerateNewPuyoPairs(System.Action<bool> callBack)
    {
        while (pairs.Count < 3)
            pairs.Add(GenerateNewPuyoPair());
        yield return null;
        callBack(true);
    }
    public PuyoPair GenerateNewPuyoPair()
    {
        var rand1 = Convert.ToInt32(UnityEngine.Random.Range(0, 4));

        var go1 = Instantiate(puyoPrefabs[rand1], new Vector3(0f, 0f, 10f), Quaternion.identity) as GameObject;
        var puyoColor = PuyoHelper.GetPuyoColorFromString(go1.tag);
        go1.GetComponent<Puyo>().Initialize(puyoColor);
        go1.transform.Rotate(new Vector3(180f, 0f, 0f));

        var rand2 = Convert.ToInt32(UnityEngine.Random.Range(0, 4));

        var go2 = Instantiate(puyoPrefabs[rand2], new Vector3(0f, 0f, 10f), Quaternion.identity) as GameObject;
        var puyoColor2 = PuyoHelper.GetPuyoColorFromString(go2.tag);
        go2.GetComponent<Puyo>().Initialize(puyoColor2);
        go2.transform.Rotate(new Vector3(180f, 0f, 0f));

        return new PuyoPair(go1, go2);
    }
    // Spawn
    public IEnumerator SpawnNewPair(System.Action<bool> callBack)
    {
        if (pairs.Any())
        {
            pairs.First().Puyo1.transform.position = new Vector3(GameVariable.MidPuyo.x, 12f, GameVariable.MidPuyo.z);
            pairs.First().Puyo2.transform.position = new Vector3(GameVariable.MidPuyo.x, 13f, GameVariable.MidPuyo.z);
            fallingPair = pairs.First();
            pairs.Remove(pairs.First());
        }
        pairs.ElementAt(1).Puyo1.transform.position = new Vector3(7.5f, 7f, GameVariable.MidPuyo.z);
        pairs.ElementAt(1).Puyo2.transform.position = new Vector3(7.5f, 8f, GameVariable.MidPuyo.z);

        pairs.First().Puyo1.transform.position = new Vector3(6.5f, 8f, GameVariable.MidPuyo.z);
        pairs.First().Puyo2.transform.position = new Vector3(6.5f, 9f, GameVariable.MidPuyo.z);

        yield return null;
        callBack(true);
    }
    public void ShowNextPuyo()
    {
        switch (GameVariable.Scene)
        {
            case Control.Screen1:
                pairs.ElementAt(1).Puyo1.transform.position = new Vector3(7.5f, 7f, GameVariable.MidPuyo.z);
                pairs.ElementAt(1).Puyo2.transform.position = new Vector3(7.5f, 8f, GameVariable.MidPuyo.z);
                pairs.First().Puyo1.transform.position = new Vector3(6.5f, 8f, GameVariable.MidPuyo.z);
                pairs.First().Puyo2.transform.position = new Vector3(6.5f, 9f, GameVariable.MidPuyo.z);
                break;
            case Control.Screen2:
                pairs.ElementAt(1).Puyo1.transform.position = new Vector3(GameVariable.MidPuyo.x, 7f, 7.5f);
                pairs.ElementAt(1).Puyo2.transform.position = new Vector3(GameVariable.MidPuyo.x, 8f, 7.5f);
                pairs.First().Puyo1.transform.position = new Vector3(GameVariable.MidPuyo.x, 8f, 6.5f);
                pairs.First().Puyo2.transform.position = new Vector3(GameVariable.MidPuyo.x, 9f, 6.5f);
                break;
            case Control.Screen3:
                pairs.ElementAt(1).Puyo1.transform.position = new Vector3(-5.5f, 7f, GameVariable.MidPuyo.z);
                pairs.ElementAt(1).Puyo2.transform.position = new Vector3(-5.5f, 8f, GameVariable.MidPuyo.z);
                pairs.First().Puyo1.transform.position = new Vector3(-4.5f, 8f, GameVariable.MidPuyo.z);
                pairs.First().Puyo2.transform.position = new Vector3(-4.5f, 9f, GameVariable.MidPuyo.z);
                break;
            case Control.Screen4:
                pairs.ElementAt(1).Puyo1.transform.position = new Vector3(GameVariable.MidPuyo.x, 7f, -5.5f);
                pairs.ElementAt(1).Puyo2.transform.position = new Vector3(GameVariable.MidPuyo.x, 8f, -5.5f);
                pairs.First().Puyo1.transform.position = new Vector3(GameVariable.MidPuyo.x, 8f, -4.5f);
                pairs.First().Puyo2.transform.position = new Vector3(GameVariable.MidPuyo.x, 9f, -4.5f);
                break;
        }
    }
    // Fall

    // Fix
    public IEnumerator FixPair(System.Action<int> callBack)
    {
        var p1 = fallingPair.Puyo1.transform.position;
        var p2 = fallingPair.Puyo2.transform.position;
        // Find Column and Row
        var p1colA = (int)p1.x;
        var p1colB = (int)p1.z;
        var p1row = (int)p1.y;

        var p2colA = (int)p2.x;
        var p2colB = (int)p2.z;
        var p2row = (int)p2.y;

        puyos[p1row, p1colA, p1colB] = fallingPair.Puyo1;
        puyos[p1row, p1colA, p1colB].GetComponent<Puyo>().ColumnA = p1colA;
        puyos[p1row, p1colA, p1colB].GetComponent<Puyo>().ColumnB = p1colB;
        puyos[p1row, p1colA, p1colB].GetComponent<Puyo>().Row = p1row;

        puyos[p2row, p2colA, p2colB] = fallingPair.Puyo2;
        puyos[p2row, p2colA, p2colB].GetComponent<Puyo>().ColumnA = p2colA;
        puyos[p2row, p2colA, p2colB].GetComponent<Puyo>().ColumnB = p2colB;
        puyos[p2row, p2colA, p2colB].GetComponent<Puyo>().Row = p2row;

        // ResetFallingPair
        fallingPair = null;
        GameVariable.currentCombo = 0;
        yield return new WaitForSeconds(3 * GameVariable.PuyoRepositioningSpeed);
        if (ifGameOver())
            callBack(0);
        else if (ifDanger())
            callBack(1);
        else
            callBack(2);
    }
    public bool ifGameOver()
    {
        for (int i = 0; i < GameVariable.ColumnsA; i++)
            for (int j = 0; j < GameVariable.ColumnsB; j++)
            {
                if (puyos[12, i, j] != null)
                {
                    SoundManager.StopSound();
                    return true;
                }
            }
        return false;
    }
    public bool ifDanger()
    {
        for (int i = 0; i < GameVariable.ColumnsA; i++)
            for (int j = 0; j < GameVariable.ColumnsB; j++)
            {
                if (puyos[9, i, j] != null)
                {
                    GameVariable.isDanger = true;
                    return true;
                }
            }
        GameVariable.isDanger = false;
        return false;
    }

    public bool isPairFalling()
    {
        if (fallingPair == null)
            return false;
        return true;
    }
    /// <summary>
    /// Check if can fall
    /// </summary>
    /// <returns></returns>
    public bool canPairFall()
    {
        var p1pos = fallingPair.Puyo1.transform.position;
        var p2pos = fallingPair.Puyo2.transform.position;

        if (Math.Ceiling(p1pos.y) != p1pos.y) return true;

        if (p1pos.y == GameVariable.BasePoint.y || p2pos.y == GameVariable.BasePoint.y) return false;

        for (int i = 0; i < GameVariable.Rows; i++)
            for (int j = 0; j < GameVariable.ColumnsA; j++)
                for (int k = 0; k < GameVariable.ColumnsB; k++)
                    if (puyos[i, j, k] != null)
                    {
                        var pPos = puyos[i, j, k].transform.position;
                        if (pPos.x == p1pos.x && pPos.z == p1pos.z
                            && (pPos.y == Math.Ceiling(p1pos.y) - 1 || pPos.y == Math.Ceiling(p2pos.y) - 1))
                            return false;
                        if (pPos.x == p2pos.x && pPos.z == p2pos.z
                            && (pPos.y == Math.Ceiling(p1pos.y) - 1 || pPos.y == Math.Ceiling(p2pos.y) - 1))
                            return false;
                    }
        return true;
    }
    /// <summary>
    /// Check if can go negative A axis
    /// </summary>
    /// <returns></returns>
    private bool canGoNegA()
    {
        var p1pos = fallingPair.Puyo1.transform.position;
        var p2pos = fallingPair.Puyo2.transform.position;

        if (p1pos.x <= GameVariable.BasePoint.x || p2pos.x <= GameVariable.BasePoint.x)
            return false;

        for (int i = 0; i < GameVariable.Rows; i++)
            for (int j = 0; j < GameVariable.ColumnsA; j++)
                for (int k = 0; k < GameVariable.ColumnsB; k++)
                    if (puyos[i, j, k] != null)
                    {
                        var pPos = puyos[i, j, k].transform.position;
                        if (pPos.z == p1pos.z && pPos.y == Math.Truncate(p1pos.y) && p1pos.x - 1 == pPos.x)
                            return false;
                        if (pPos.z == p2pos.z && pPos.y == Math.Truncate(p2pos.y) && p2pos.x - 1 == pPos.x)
                            return false;
                    }
        return true;
    }
    /// <summary>
    /// Check if can go positive A axis
    /// </summary>
    /// <returns></returns>
    private bool canGoPosA()
    {
        var p1pos = fallingPair.Puyo1.transform.position;
        var p2pos = fallingPair.Puyo2.transform.position;

        if (p1pos.x >= GameVariable.BasePoint.x + GameVariable.ColumnsA - 1 || p2pos.x >= GameVariable.BasePoint.x + GameVariable.ColumnsA - 1) return false;

        for (int i = 0; i < GameVariable.Rows; i++)
            for (int j = 0; j < GameVariable.ColumnsA; j++)
                for (int k = 0; k < GameVariable.ColumnsB; k++)
                    if (puyos[i, j, k] != null)
                    {
                        var pPos = puyos[i, j, k].transform.position;
                        if (pPos.z == p1pos.z && pPos.y == Math.Truncate(p1pos.y) && p1pos.x + 1 == pPos.x)
                            return false;
                        if (pPos.z == p2pos.z && pPos.y == Math.Truncate(p2pos.y) && p2pos.x + 1 == pPos.x)
                            return false;
                    }
        return true;
    }
    /// <summary>
    /// Check if can go negative B axis
    /// </summary>
    /// <returns></returns>
    private bool canGoNegB()
    {
        var p1pos = fallingPair.Puyo1.transform.position;
        var p2pos = fallingPair.Puyo2.transform.position;

        if (p1pos.z <= GameVariable.BasePoint.z || p2pos.z <= GameVariable.BasePoint.z)
            return false;

        for (int i = 0; i < GameVariable.Rows; i++)
            for (int j = 0; j < GameVariable.ColumnsA; j++)
                for (int k = 0; k < GameVariable.ColumnsB; k++)
                    if (puyos[i, j, k] != null)
                    {
                        var pPos = puyos[i, j, k].transform.position;
                        if (pPos.x == p1pos.x && pPos.y == Math.Truncate(p1pos.y) && p1pos.z - 1 == pPos.z)
                            return false;
                        if (pPos.x == p2pos.x && pPos.y == Math.Truncate(p2pos.y) && p2pos.z - 1 == pPos.z)
                            return false;
                    }
        return true;
    }
    /// <summary>
    /// Check if can go positive B axis
    /// </summary>
    /// <returns></returns>
    private bool canGoPosB()
    {
        var p1pos = fallingPair.Puyo1.transform.position;
        var p2pos = fallingPair.Puyo2.transform.position;

        if (p1pos.z >= GameVariable.BasePoint.z + GameVariable.ColumnsB - 1 || p2pos.z >= GameVariable.BasePoint.z + GameVariable.ColumnsB - 1) return false;

        for (int i = 0; i < GameVariable.Rows; i++)
            for (int j = 0; j < GameVariable.ColumnsA; j++)
                for (int k = 0; k < GameVariable.ColumnsB; k++)
                    if (puyos[i, j, k] != null)
                    {
                        var pPos = puyos[i, j, k].transform.position;
                        if (pPos.x == p1pos.x && pPos.y == Math.Truncate(p1pos.y) && p1pos.z + 1 == pPos.z)
                            return false;
                        if (pPos.x == p2pos.x && pPos.y == Math.Truncate(p2pos.y) && p2pos.z + 1 == pPos.z)
                            return false;
                    }
        return true;
    }

    public void controlBlock(Control PuyoControl)
    {
        Vector3[] changeVector = { Vector3.zero, Vector3.zero, Vector3.zero };
        switch (PuyoControl)
        {
            case Control.Drop:
                fallingPair.Puyo1.transform.position -= new Vector3(0f, 0.5f, 0f);
                fallingPair.Puyo2.transform.position -= new Vector3(0f, 0.5f, 0f);
                SoundManager.PlaySound(FX.Move);
                break;
            case Control.Fall:
                fallingPair.Puyo1.transform.position -= new Vector3(0f, 0.5f, 0f);
                fallingPair.Puyo2.transform.position -= new Vector3(0f, 0.5f, 0f);
                break;
            case Control.MovePosA:
                if (canGoPosA())
                {
                    fallingPair.Puyo1.transform.position += new Vector3(1f, 0f, 0f);
                    fallingPair.Puyo2.transform.position += new Vector3(1f, 0f, 0f);
                    SoundManager.PlaySound(FX.Move);
                    firstRotate = true;
                }
                break;
            case Control.MoveNegA:
                if (canGoNegA())
                {
                    fallingPair.Puyo1.transform.position -= new Vector3(1f, 0f, 0f);
                    fallingPair.Puyo2.transform.position -= new Vector3(1f, 0f, 0f);
                    SoundManager.PlaySound(FX.Move);
                    firstRotate = true;
                }
                break;
            case Control.MovePosB:
                if (canGoPosB())
                {
                    fallingPair.Puyo1.transform.position += new Vector3(0f, 0f, 1f);
                    fallingPair.Puyo2.transform.position += new Vector3(0f, 0f, 1f);
                    SoundManager.PlaySound(FX.Move);
                    firstRotate = true;
                }
                break;
            case Control.MoveNegB:
                if (canGoNegB())
                {
                    fallingPair.Puyo1.transform.position -= new Vector3(0f, 0f, 1f);
                    fallingPair.Puyo2.transform.position -= new Vector3(0f, 0f, 1f);
                    SoundManager.PlaySound(FX.Move);
                    firstRotate = true;
                }
                break;
            case Control.RotateRightA:
                changeVector = canRotate(Control.RotateRightA);
                if (!changeVector[0].Equals(Vector3.zero) || !changeVector[1].Equals(Vector3.zero))
                {
                    fallingPair.Puyo1.transform.position += changeVector[0];
                    fallingPair.Puyo2.transform.position += changeVector[1];
                    SoundManager.PlaySound(FX.Rotate);
                    firstRotate = true;
                }
                else
                {
                    firstRotate = false;
                }
                break;
            case Control.RotateLeftA:
                changeVector = canRotate(Control.RotateLeftA);
                if (!changeVector[0].Equals(Vector3.zero) || !changeVector[1].Equals(Vector3.zero))
                {
                    fallingPair.Puyo1.transform.position += changeVector[0];
                    fallingPair.Puyo2.transform.position += changeVector[1];
                    SoundManager.PlaySound(FX.Rotate);
                    firstRotate = true;
                }
                else
                {
                    firstRotate = false;
                }
                break;
            case Control.RotateBack:
                changeVector = canRotate(Control.RotateBack);
                if (!changeVector[0].Equals(Vector3.zero) || !changeVector[1].Equals(Vector3.zero))
                {
                    fallingPair.Puyo1.transform.position += changeVector[0];
                    fallingPair.Puyo2.transform.position += changeVector[1];
                    SoundManager.PlaySound(FX.Rotate);
                    firstRotate = true;
                }
                else
                {
                    firstRotate = false;
                }
                break;
            case Control.RotateForth:
                changeVector = canRotate(Control.RotateForth);
                if (!changeVector[0].Equals(Vector3.zero) || !changeVector[1].Equals(Vector3.zero))
                {
                    fallingPair.Puyo1.transform.position += changeVector[0];
                    fallingPair.Puyo2.transform.position += changeVector[1];
                    SoundManager.PlaySound(FX.Rotate);
                    firstRotate = true;
                }
                else
                {
                    firstRotate = false;
                }
                break;
            case Control.RotateHorizontalRight:
                changeVector = canRotate(Control.RotateHorizontalRight);
                if (!changeVector[0].Equals(Vector3.zero) || !changeVector[1].Equals(Vector3.zero))
                {
                    fallingPair.Puyo1.transform.position += changeVector[0];
                    fallingPair.Puyo2.transform.position += changeVector[1];
                    SoundManager.PlaySound(FX.Rotate);
                    firstRotate = true;
                }
                else
                {
                    firstRotate = false;
                }
                break;
            case Control.RotateHorizontalLeft:
                changeVector = canRotate(Control.RotateHorizontalLeft);
                if (!changeVector[0].Equals(Vector3.zero) || !changeVector[1].Equals(Vector3.zero))
                {
                    fallingPair.Puyo1.transform.position += changeVector[0];
                    fallingPair.Puyo2.transform.position += changeVector[1];
                    SoundManager.PlaySound(FX.Rotate);
                    firstRotate = true;
                }
                else
                {
                    firstRotate = false;
                }
                break;
        }
    }

    private Vector3[] canRotate(Control Direction)
    {
        Vector3[] momentum = { Vector3.zero, Vector3.zero};
        var p1pos = fallingPair.Puyo1.transform.position;
        var p2pos = fallingPair.Puyo2.transform.position;
        var nApad = false;
        var pApad = false;
        var bBpad = false;
        var fBpad = false;
        Vector3 vpad = Vector3.zero;

        if (p1pos.x == GameVariable.BasePoint.x)
            nApad = true;
        else if (p1pos.x == GameVariable.BasePoint.x + GameVariable.ColumnsA - 1)
            pApad = true;

        if (p1pos.z == GameVariable.BasePoint.z)
            fBpad = true;
        else if (p1pos.z == GameVariable.BasePoint.z + GameVariable.ColumnsB - 1)
            bBpad = true;

        if (Math.Truncate(p1pos.y) == GameVariable.BasePoint.y)
            vpad = new Vector3(0, 1 - (p1pos.y - (float)Math.Truncate(p1pos.y)));

        for (int i = 0; i < GameVariable.Rows; i++)
            for (int j = 0; j < GameVariable.ColumnsA; j++)
                for (int k = 0; k < GameVariable.ColumnsB; k++)
                    if (puyos[i, j, k] != null)
                    {
                        var pPos = puyos[i, j, k].transform.position;

                        if (p1pos.x == pPos.x)
                        {
                            if (p1pos.z == pPos.z && Math.Truncate(p1pos.y) - 1 == pPos.y)
                                vpad = new Vector3(0, 1 - (p1pos.y - (float)Math.Truncate(p1pos.y)));
                            else if (Math.Truncate(p1pos.y) == pPos.y)
                            {
                                if (p1pos.z - 1 == pPos.z)
                                    fBpad = true;
                                else if (p1pos.z + 1 == pPos.z)
                                    bBpad = true;
                            }
                        }
                        if (p1pos.z == pPos.z)
                        {
                            if (Math.Truncate(p1pos.y) == pPos.y)
                            {
                                if (p1pos.x - 1 == pPos.x)
                                    nApad = true;
                                else if (p1pos.x + 1 == pPos.x)
                                    pApad = true;
                            }
                        }

                    }

        switch (fallingPair.Orientation)
        {
            case Orientation.NegA:
                switch (Direction)
                {
                    case Control.RotateForth:
                    case Control.RotateBack:
                        return momentum;
                    case Control.RotateLeftA:
                        momentum[0] += vpad;
                        momentum[1] += new Vector3(1f, -1f, 0f) + vpad;
                        fallingPair.Orientation = Orientation.VerticalB;
                        return momentum;
                    case Control.RotateRightA:
                        momentum[1] += new Vector3(1f, 1f, 0f);
                        fallingPair.Orientation = Orientation.VerticalA;
                        return momentum;
                    case Control.RotateHorizontalLeft:
                        if (bBpad)
                        {
                            if (fBpad)
                            {
                                if (firstRotate == true)
                                    return momentum;
                                else
                                {
                                    if (pApad)
                                    {
                                        momentum[0] += new Vector3(1f, 0f, 0f);
                                        momentum[1] += new Vector3(-1f, 0f, 0f);
                                    }
                                    else
                                    {
                                        momentum[1] += new Vector3(2f, 0f, 0f);
                                    }
                                    fallingPair.Orientation = Orientation.PosA;
                                    return momentum;
                                }
                            }
                            else
                            {
                                momentum[0] += new Vector3(0f, 0f, -1f);
                                momentum[1] += new Vector3(0f, 0f, -1f);
                            }
                        }
                        momentum[1] += new Vector3(1f, 0f, 1f);
                        fallingPair.Orientation = Orientation.PosB;
                        return momentum;
                    case Control.RotateHorizontalRight:
                        if (fBpad)
                        {
                            if (bBpad)
                            {
                                if (firstRotate == true)
                                    return momentum;
                                else
                                {
                                    if (pApad)
                                    {
                                        momentum[0] += new Vector3(1f, 0f, 0f);
                                        momentum[1] += new Vector3(-1f, 0f, 0f);
                                    }
                                    else
                                    {
                                        momentum[1] += new Vector3(2f, 0f, 0f);
                                    }
                                    fallingPair.Orientation = Orientation.PosA;
                                    return momentum;
                                }
                            }
                            else
                            {
                                momentum[0] += new Vector3(0f, 0f, 1f);
                                momentum[1] += new Vector3(0f, 0f, 1f);
                            }
                        }
                        momentum[1] += new Vector3(1f, 0f, -1f);
                        fallingPair.Orientation = Orientation.NegB;
                        return momentum;
                }
                break;
            case Orientation.PosA:
                switch (Direction)
                {
                    case Control.RotateForth:
                    case Control.RotateBack:
                        return momentum;
                    case Control.RotateLeftA:
                        momentum[1] += new Vector3(-1f, 1f, 0f);
                        fallingPair.Orientation = Orientation.VerticalA;
                        return momentum;
                    case Control.RotateRightA:
                        momentum[0] += vpad;
                        momentum[1] += new Vector3(-1f, -1f, 0f) + vpad;
                        fallingPair.Orientation = Orientation.VerticalB;
                        return momentum;
                    case Control.RotateHorizontalLeft:
                        if (fBpad)
                        {
                            if (bBpad)
                            {
                                if (firstRotate == true)
                                    return momentum;
                                else
                                {
                                    if (nApad)
                                    {
                                        momentum[0] += new Vector3(-1f, 0f, 0f);
                                        momentum[1] += new Vector3(1f, 0f, 0f);
                                    }
                                    else
                                    {
                                        momentum[1] += new Vector3(-2f, 0f, 0f);
                                    }
                                    fallingPair.Orientation = Orientation.NegA;
                                    return momentum;
                                }
                            }
                            else
                            {
                                momentum[0] += new Vector3(0f, 0f, 1f);
                                momentum[1] += new Vector3(0f, 0f, 1f);
                            }
                        }
                        momentum[1] += new Vector3(-1f, 0f, -1f);
                        fallingPair.Orientation = Orientation.NegB;
                        return momentum;
                    case Control.RotateHorizontalRight:
                        if (bBpad)
                        {
                            if (fBpad)
                            {
                                if (firstRotate == true)
                                    return momentum;
                                else
                                {
                                    if (nApad)
                                    {
                                        momentum[0] += new Vector3(-1f, 0f, 0f);
                                        momentum[1] += new Vector3(1f, 0f, 0f);
                                    }
                                    else
                                    {
                                        momentum[1] += new Vector3(-2f, 0f, 0f);
                                    }
                                    fallingPair.Orientation = Orientation.PosA;
                                    return momentum;
                                }
                            }
                            else
                            {
                                momentum[0] += new Vector3(0f, 0f, -1f);
                                momentum[1] += new Vector3(0f, 0f, -1f);
                            }
                        }
                        momentum[1] += new Vector3(-1f, 0f, 1f);
                        fallingPair.Orientation = Orientation.PosB;
                        return momentum;
                }
                break;
            case Orientation.NegB:
                switch (Direction)
                {
                    case Control.RotateLeftA:
                    case Control.RotateRightA:
                        return momentum;
                    case Control.RotateForth:
                        momentum[0] += vpad;
                        momentum[1] += new Vector3(0f, -1f, 1f) + vpad;
                        fallingPair.Orientation = Orientation.VerticalB;
                        return momentum;
                    case Control.RotateBack:
                        momentum[1] += new Vector3(0f, 1f, 1f);
                        fallingPair.Orientation = Orientation.VerticalA;
                        return momentum;
                    case Control.RotateHorizontalLeft:
                        if (nApad)
                        {
                            if (pApad)
                            {
                                if (firstRotate == true)
                                    return momentum;
                                else
                                {
                                    if (bBpad)
                                    {
                                        momentum[0] += new Vector3(0f, 0f, 1f);
                                        momentum[1] += new Vector3(0f, 0f, -1f);
                                    }
                                    else
                                    {
                                        momentum[1] += new Vector3(0f, 0f, -2f);
                                    }
                                    fallingPair.Orientation = Orientation.PosB;
                                    return momentum;
                                }
                            }
                            else
                            {
                                momentum[0] += new Vector3(1f, 0f, 0f);
                                momentum[1] += new Vector3(1f, 0f, 0f);
                            }
                        }
                        momentum[1] += new Vector3(-1f, 0f, 1f);
                        fallingPair.Orientation = Orientation.NegA;
                        return momentum;
                    case Control.RotateHorizontalRight:
                        if (pApad)
                        {
                            if (nApad)
                            {
                                if (firstRotate == true)
                                    return momentum;
                                else
                                {
                                    if (bBpad)
                                    {
                                        momentum[0] += new Vector3(0f, 0f, 1f);
                                        momentum[1] += new Vector3(0f, 0f, -1f);
                                    }
                                    else
                                    {
                                        momentum[1] += new Vector3(0f, 0f, -2f);
                                    }
                                    fallingPair.Orientation = Orientation.PosB;
                                    return momentum;
                                }
                            }
                            else
                            {
                                momentum[0] += new Vector3(-1f, 0f, 0f);
                                momentum[1] += new Vector3(-1f, 0f, 0f);
                            }
                        }
                        momentum[1] += new Vector3(1f, 0f, 1f);
                        fallingPair.Orientation = Orientation.PosA;
                        return momentum;
                }
                break;
            case Orientation.PosB:
                switch (Direction)
                {
                    case Control.RotateLeftA:
                    case Control.RotateRightA:
                        return momentum;
                    case Control.RotateForth:
                        momentum[1] += new Vector3(0f, 1f, -1f);
                        fallingPair.Orientation = Orientation.VerticalA;
                        return momentum;
                    case Control.RotateBack:
                        momentum[0] += vpad;
                        momentum[1] += new Vector3(0f, -1f, -1f) + vpad;
                        fallingPair.Orientation = Orientation.VerticalB;
                        return momentum;
                    case Control.RotateHorizontalLeft:
                        if (pApad)
                        {
                            if (nApad)
                            {
                                if (firstRotate == true)
                                    return momentum;
                                else
                                {
                                    if (fBpad)
                                    {
                                        momentum[0] += new Vector3(0f, 0f, -1f);
                                        momentum[1] += new Vector3(0f, 0f, 1f);
                                    }
                                    else
                                    {
                                        momentum[1] += new Vector3(0f, 0f, -2f);
                                    }
                                    fallingPair.Orientation = Orientation.NegB;
                                    return momentum;
                                }
                            }
                            else
                            {
                                momentum[0] += new Vector3(-1f, 0f, 0f);
                                momentum[1] += new Vector3(-1f, 0f, 0f);
                            }
                        }
                        momentum[1] += new Vector3(1f, 0f, -1f);
                        fallingPair.Orientation = Orientation.PosA;
                        return momentum;
                    case Control.RotateHorizontalRight:
                        if (nApad)
                        {
                            if (pApad)
                            {
                                if (firstRotate == true)
                                    return momentum;
                                else
                                {
                                    if (fBpad)
                                    {
                                        momentum[0] += new Vector3(0f, 0f, -1f);
                                        momentum[1] += new Vector3(0f, 0f, 1f);
                                    }
                                    else
                                    {
                                        momentum[1] += new Vector3(0f, 0f, -2f);
                                    }
                                    fallingPair.Orientation = Orientation.NegB;
                                    return momentum;
                                }
                            }
                            else
                            {
                                momentum[0] += new Vector3(1f, 0f, 0f);
                                momentum[1] += new Vector3(1f, 0f, 0f);
                            }
                        }
                        momentum[1] += new Vector3(-1f, 0f, -1f);
                        fallingPair.Orientation = Orientation.NegA;
                        return momentum;
                }
                break;
            case Orientation.VerticalA:
                switch (Direction)
                {
                    case Control.RotateHorizontalLeft:
                    case Control.RotateHorizontalRight:
                        return momentum;
                    case Control.RotateLeftA:
                        if (nApad)
                        {
                            if (pApad)
                            {
                                if (firstRotate == true)
                                    return momentum;
                                else
                                {
                                    momentum[0] += vpad;
                                    momentum[1] += new Vector3(0f, -2f, 0f) + vpad;
                                    fallingPair.Orientation = Orientation.VerticalB;
                                    return momentum;
                                }
                            }
                            else
                            {
                                momentum[0] += new Vector3(1f, 0f, 0f);
                                momentum[1] += new Vector3(1f, 0f, 0f);
                            }
                        }
                        momentum[1] += new Vector3(-1f, -1f, 0f);
                        fallingPair.Orientation = Orientation.NegA;
                        return momentum;
                    case Control.RotateRightA:
                        if (pApad)
                        {
                            if (nApad)
                            {
                                if (firstRotate == true)
                                    return momentum;
                                else
                                {
                                    momentum[0] += vpad;
                                    momentum[1] += new Vector3(0f, -2f, 0f) + vpad;
                                    fallingPair.Orientation = Orientation.VerticalB;
                                    return momentum;
                                }
                            }
                            else
                            {
                                momentum[0] += new Vector3(-1f, 0f, 0f);
                                momentum[1] += new Vector3(-1f, 0f, 0f);
                            }
                        }
                        momentum[1] += new Vector3(1f, -1f, 0f);
                        fallingPair.Orientation = Orientation.PosA;
                        return momentum;
                    case Control.RotateForth:
                        if (fBpad)
                        {
                            if (bBpad)
                            {
                                if (firstRotate == true)
                                    return momentum;
                                else
                                {
                                    momentum[0] += vpad;
                                    momentum[1] += new Vector3(0f, -2f, 0f) + vpad;
                                    fallingPair.Orientation = Orientation.VerticalB;
                                    return momentum;
                                }
                            }
                            else
                            {
                                momentum[0] += new Vector3(0f, 0f, 1f);
                                momentum[1] += new Vector3(0f, 0f, 1f);
                            }
                        }
                        momentum[1] += new Vector3(0f, -1f, -1f);
                        fallingPair.Orientation = Orientation.NegB;
                        return momentum;
                    case Control.RotateBack:
                        if (bBpad)
                        {
                            if (fBpad)
                            {
                                if (firstRotate == true)
                                    return momentum;
                                else
                                {
                                    momentum[0] += vpad;
                                    momentum[1] += new Vector3(0f, -2f, 0f) + vpad;
                                    fallingPair.Orientation = Orientation.VerticalB;
                                    return momentum;
                                }
                            }
                            else
                            {
                                momentum[0] += new Vector3(0f, 0f, -1f);
                                momentum[1] += new Vector3(0f, 0f, -1f);
                            }
                        }
                        momentum[1] += new Vector3(0f, -1f, 1f);
                        fallingPair.Orientation = Orientation.PosB;
                        return momentum;
                }
                break;
            case Orientation.VerticalB:
                switch (Direction)
                {
                    case Control.RotateHorizontalLeft:
                    case Control.RotateHorizontalRight:
                        return momentum;
                    case Control.RotateLeftA:
                        if (pApad)
                        {
                            if (nApad)
                            {
                                if (firstRotate == true)
                                    return momentum;
                                else
                                {
                                    momentum[0] += vpad;
                                    momentum[1] += new Vector3(0f, -2f, 0f) + vpad;
                                    fallingPair.Orientation = Orientation.VerticalB;
                                    return momentum;
                                }
                            }
                            else
                            {
                                momentum[0] += new Vector3(-1f, 0f, 0f);
                                momentum[1] += new Vector3(-1f, 0f, 0f);
                            }
                        }
                        momentum[1] += new Vector3(1f, 1f, 0f);
                        fallingPair.Orientation = Orientation.PosA;
                        return momentum;
                    case Control.RotateRightA:
                        if (nApad)
                        {
                            if (pApad)
                            {
                                if (firstRotate == true)
                                    return momentum;
                                else
                                {
                                    momentum[0] += vpad;
                                    momentum[1] += new Vector3(0f, -2f, 0f) + vpad;
                                    fallingPair.Orientation = Orientation.VerticalB;
                                    return momentum;
                                }
                            }
                            else
                            {
                                momentum[0] += new Vector3(1f, 0f, 0f);
                                momentum[1] += new Vector3(1f, 0f, 0f);
                            }
                        }
                        momentum[1] += new Vector3(-1f, 1f, 0f);
                        fallingPair.Orientation = Orientation.NegA;
                        return momentum;
                    case Control.RotateForth:
                        if (bBpad)
                        {
                            if (fBpad)
                            {
                                if (firstRotate == true)
                                    return momentum;
                                else
                                {
                                    momentum[0] += vpad;
                                    momentum[1] += new Vector3(0f, -2f, 0f) + vpad;
                                    fallingPair.Orientation = Orientation.VerticalB;
                                    return momentum;
                                }
                            }
                            else
                            {
                                momentum[0] += new Vector3(0f, 0f, -1f);
                                momentum[1] += new Vector3(0f, 0f, -1f);
                            }
                        }
                        momentum[1] += new Vector3(0f, 1f, 1f);
                        fallingPair.Orientation = Orientation.PosB;
                        return momentum;
                    case Control.RotateBack:
                        if (fBpad)
                        {
                            if (bBpad)
                            {
                                if (firstRotate == true)
                                    return momentum;
                                else
                                {
                                    momentum[0] += vpad;
                                    momentum[1] += new Vector3(0f, -2f, 0f) + vpad;
                                    fallingPair.Orientation = Orientation.VerticalB;
                                    return momentum;
                                }
                            }
                            else
                            {
                                momentum[0] += new Vector3(0f, 0f, 1f);
                                momentum[1] += new Vector3(0f, 0f, 1f);
                            }
                        }
                        momentum[1] += new Vector3(0f, 1f, -1f);
                        fallingPair.Orientation = Orientation.NegB;
                        return momentum;
                }
                break;
        }
        return momentum;
    }

    public void UpdatePuyosPosition()
    {
        ComputeNewPuyosPosition();
    }
    private void MovePuyos()
    {
        for (int i = 0; i < GameVariable.Rows; i++)
        {
            for (int j = 0; j < GameVariable.ColumnsA; j++)
            {
                for (int k = 0; k < GameVariable.ColumnsB; k++)
                {
                    if (i < 0 && puyos[i - 1, j, k] != null)
                    {
                        var pos = puyos[i, j, k].transform.position;
                        pos.y -= GameVariable.PuyoSize.y;
                        puyos[i, j, k].transform.positionTo(2f, pos);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Used to compute new puyos position after destroying
    /// </summary>
    private void ComputeNewPuyosPosition()
    {
        bool puyoHasToMove = true;
        while (puyoHasToMove)
        {
            puyoHasToMove = false;
            for (int i = 1; i < GameVariable.Rows; i++)
            {
                for (int j = 0; j < GameVariable.ColumnsA; j++)
                {
                    for (int k = 0; k < GameVariable.ColumnsB; k++)
                    {
                        var puyoGo = puyos[i, j, k];
                        if (puyoGo != null)
                        {
                            // Puyo under is null
                            if (puyos[i - 1, j, k] == null)
                            {
                                // Shift all top puyos
                                for (int l = i; l < GameVariable.Rows; l++)
                                {
                                    var topPuyo = puyos[l, j, k];
                                    if (topPuyo != null)
                                    {
                                        puyos[l - 1, j, k] = topPuyo;
                                        puyos[l - 1, j, k].GetComponent<Puyo>().Row--;
                                        puyos[l, j, k] = null;
                                    }
                                }
                                puyoHasToMove = true;
                            }
                        }
                    }
                }
            }
        }
    }

    private Vector3 FindPuyoScreenPosition(int row, int columnA, int columnB)
    {
        return new Vector3(GameVariable.BasePoint.x + columnA * GameVariable.PuyoSize.y, GameVariable.BasePoint.y + row * GameVariable.PuyoSize.y, GameVariable.BasePoint.z + columnB * GameVariable.PuyoSize.z);
    }

    /// <summary>
    /// Tween Puyos to new positions
    /// </summary>
    /// <returns></returns>
    public IEnumerator MovePuyosToNewPosition(System.Action<bool> callBack)
    {
        // We use this variable to store the biggest ratio, it is used to know how many time we have to wait for the tween to complete
        var biggestRatio = 0f;

        for (int i = 0; i < GameVariable.Rows; i++)
        {
            for (int j = 0; j < GameVariable.ColumnsA; j++)
            {
                for (int k = 0; k < GameVariable.ColumnsB; k++)
                {
                    var puyo = puyos[i, j, k];
                    if (puyo != null)
                    {
                        var oldPosition = puyo.transform.position;
                        var newPosition = FindPuyoScreenPosition(i, j, k);

                        // Position is correct, skip and continue with the next puyo
                        if (oldPosition.y == newPosition.y) continue;

                        var ratio = (oldPosition.y - newPosition.y) / GameVariable.PuyoSize.y;
                        if (ratio > biggestRatio) biggestRatio = ratio;

                        puyo.transform.positionTo(GameVariable.PuyoRepositioningSpeed * ratio, newPosition);
                    }
                }
            }
        }
        yield return new WaitForSeconds(biggestRatio * GameVariable.PuyoRepositioningSpeed);
        callBack(true);
    }

    private void InitRandomArray()
    {
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < GameVariable.ColumnsA; j++)
            {
                for (int k = 0; k < GameVariable.ColumnsB; k++)
                {
                    var rand = Convert.ToInt32(UnityEngine.Random.Range(0, 4));

                    var position = FindPuyoScreenPosition(i, j, k);
                    var go = Instantiate(puyoPrefabs[rand], position, Quaternion.identity) as GameObject;
                    var puyoColor = PuyoHelper.GetPuyoColorFromString(go.tag);
                    go.GetComponent<Puyo>().Initialize(puyoColor, i, j, k);

                    puyos[i, j, k] = go;
                }
            }
        }
    }
    
    public void ProcessComboEffect()
    {
        GameVariable.currentCombo++;
        SoundManager.PlaySound(FX.Combo);
    }

    public IEnumerator DestroyAllChains(System.Action<bool> callBack)
    {
        var chains = FindAllChains();
        if (chains.Any())
        {
            ProcessComboEffect();
            StartCoroutine(AnimateAndDestroyChain(chains, secondCallBack =>
            {
                callBack(false);
            }));
        }
        else
            callBack(true);
        yield return null;
    }

    public IEnumerator AnimateAndDestroyChain(List<PuyoGroup> chains, System.Action<bool> secondCallBack)
    {
        foreach (var chain in chains)
        {
            foreach (var puyo in chain.Puyos)
            {
                StartCoroutine(AnimateAndDestroyPuyo(puyo, thirdcallBack =>
                {
                    secondCallBack(true);
                }));
            }
        }
        yield return null;
    }

    public IEnumerator AnimateAndDestroyPuyo(Puyo puyo, System.Action<bool> thirdCallBack)
    {
        puyos[puyo.Row, puyo.ColumnA, puyo.ColumnB].SetActive(false);
        yield return new WaitForSeconds(0.25f);
        puyos[puyo.Row, puyo.ColumnA, puyo.ColumnB].SetActive(true);
        yield return new WaitForSeconds(0.25f);
        puyos[puyo.Row, puyo.ColumnA, puyo.ColumnB].SetActive(false);
        yield return new WaitForSeconds(0.25f);

        DestroyPuyo(puyo);
        thirdCallBack(true);
    }

    private void DestroyPuyo(Puyo puyo)
    {
        Destroy(puyos[puyo.Row, puyo.ColumnA, puyo.ColumnB]);
        puyos[puyo.Row, puyo.ColumnA, puyo.ColumnB] = null;
    }

    public List<PuyoGroup> FindAllChains()
    {
        var chains = new List<PuyoGroup>();

        for (int i = 0; i < GameVariable.Rows; i++)
        {
            for (int j = 0; j < GameVariable.ColumnsA; j++)
            {
                for (int k = 0; k < GameVariable.ColumnsB; k++)
                {
                    var p = puyos[i, j, k];

                    if (p == null) continue;

                    var pscript = p.GetComponent<Puyo>();

                    bool alreadyInGroup = false;
                    foreach (var group in chains)
                    {
                        if (group.ContainPuyo(pscript))
                        {
                            alreadyInGroup = true;
                        }
                    }
                    if (alreadyInGroup) continue;

                    var newGroup = FindChain(pscript);

                    if (newGroup != null) chains.Add(newGroup);
                }
            }
        }

        return chains;
    }

    private PuyoGroup FindChain(Puyo puyo)
    {
        var currentChain = new List<Puyo> { puyo };
        var nextPuyosToCheck = new List<Puyo> { puyo };

        while (nextPuyosToCheck.Any())
        {
            var pi = nextPuyosToCheck.First();
            var nextInChain = FindNextPuyoInChain(pi, currentChain);
            while (nextInChain != null)
            {
                currentChain.Add(nextInChain);
                nextPuyosToCheck.Add(nextInChain);
                nextInChain = FindNextPuyoInChain(pi, currentChain);
            }
            nextPuyosToCheck.Remove(pi);
        }

        return currentChain.Count >= GameVariable.MinimumMatches ? new PuyoGroup(currentChain) : null;
    }

    private Puyo FindNextPuyoInChain(Puyo puyo, IEnumerable<Puyo> ignoredPuyos)
    {
        var ignoredList = ignoredPuyos.ToList();

        // Top
        if (puyo.Row < GameVariable.Rows - 1 && (ignoredPuyos == null || !ignoredList.Any(p => p.Row == puyo.Row + 1 && p.ColumnA == puyo.ColumnA)))
        {
            var topPuyo = puyos[puyo.Row + 1, puyo.ColumnA, puyo.ColumnB];
            if (topPuyo != null && topPuyo.GetComponent<Puyo>().Color == puyo.Color)
                return topPuyo.GetComponent<Puyo>();
        }

        // Bottom
        if (puyo.Row > 0 && (ignoredPuyos == null || !ignoredList.Any(p => p.Row == puyo.Row - 1 && p.ColumnA == puyo.ColumnA)))
        {
            var bottomPuyo = puyos[puyo.Row - 1, puyo.ColumnA, puyo.ColumnB];
            if (bottomPuyo != null && bottomPuyo.GetComponent<Puyo>().Color == puyo.Color)
                return bottomPuyo.GetComponent<Puyo>();
        }

        // Right
        if (puyo.ColumnA < GameVariable.ColumnsA - 1 && (ignoredPuyos == null || !ignoredList.Any(p => p.Row == puyo.Row && p.ColumnA == puyo.ColumnA + 1)))
        {
            var rightPuyo = puyos[puyo.Row, puyo.ColumnA + 1, puyo.ColumnB];
            if (rightPuyo != null && rightPuyo.GetComponent<Puyo>().Color == puyo.Color)
                return rightPuyo.GetComponent<Puyo>();
        }

        // Left
        if (puyo.ColumnA > 0 && (ignoredPuyos == null || !ignoredList.Any(p => p.Row == puyo.Row && p.ColumnA == puyo.ColumnA - 1)))
        {
            var leftPuyo = puyos[puyo.Row, puyo.ColumnA - 1, puyo.ColumnB];
            if (leftPuyo != null && leftPuyo.GetComponent<Puyo>().Color == puyo.Color)
                return leftPuyo.GetComponent<Puyo>();
        }

        // Back
        if (puyo.ColumnB < GameVariable.ColumnsB - 1 && (ignoredPuyos == null || !ignoredList.Any(p => p.Row == puyo.Row && p.ColumnB == puyo.ColumnB + 1)))
        {
            var rightPuyo = puyos[puyo.Row, puyo.ColumnA, puyo.ColumnB + 1];
            if (rightPuyo != null && rightPuyo.GetComponent<Puyo>().Color == puyo.Color)
                return rightPuyo.GetComponent<Puyo>();
        }

        // Forth
        if (puyo.ColumnB > 0 && (ignoredPuyos == null || !ignoredList.Any(p => p.Row == puyo.Row && p.ColumnB == puyo.ColumnB - 1)))
        {
            var leftPuyo = puyos[puyo.Row, puyo.ColumnA, puyo.ColumnB - 1];
            if (leftPuyo != null && leftPuyo.GetComponent<Puyo>().Color == puyo.Color)
                return leftPuyo.GetComponent<Puyo>();
        }

        // Nothing is found, return null
        return null;
    }

    public IEnumerator SwagPuyo(int direction, int i, int j, int k, int count)
    {
        //direction -> 0 -x 1 +x 2 -z 3 +z
        switch (direction)
        {
            case 0:
                puyos[i, j, k].transform.position += new Vector3(0.1f * (count % 2 == 0 ? 1 : -1), 0, 0);
                yield return new WaitForSeconds(0.03f);
                puyos[i, j, k].transform.position += new Vector3(-0.1f * (count % 2 == 0 ? 1 : -1), 0, 0);
                yield return new WaitForSeconds(0.03f);
                break;
            case 1:
                puyos[i, j, k].transform.position += new Vector3(-0.1f * (count % 2 == 0 ? 1 : -1), 0, 0);
                yield return new WaitForSeconds(0.03f);
                puyos[i, j, k].transform.position += new Vector3(0.1f * (count % 2 == 0 ? 1 : -1), 0, 0);
                yield return new WaitForSeconds(0.03f);
                break;
            case 2:
                puyos[i, j, k].transform.position += new Vector3(0, 0, 0.1f * (count % 2 == 0 ? 1 : -1));
                yield return new WaitForSeconds(0.03f);
                puyos[i, j, k].transform.position += new Vector3(0, 0, -0.1f * (count % 2 == 0 ? 1 : -1));
                yield return new WaitForSeconds(0.03f);
                break;
            case 3:
                puyos[i, j, k].transform.position += new Vector3(0, 0, -0.1f * (count % 2 == 0 ? 1 : -1));
                yield return new WaitForSeconds(0.03f);
                puyos[i, j, k].transform.position += new Vector3(0, 0, 0.1f * (count % 2 == 0 ? 1 : -1));
                yield return new WaitForSeconds(0.03f);
                break;
            default:
                break;
        }
    }
    public IEnumerator PuyoToHell(float speed, int i, int j, int k)
    {
        // We use this variable to store the biggest ratio, it is used to know how many time we have to wait for the tween to complete
        var biggestRatio = 0f;
        
        var puyo = puyos[i, j, k];
        var oldPosition = puyo.transform.position;
        var newPosition = FindPuyoScreenPosition(i-30, j, k);

        // Position is correct, skip and continue with the next puyo
        if (!(oldPosition.y == newPosition.y))
        {
            var ratio = (oldPosition.y - newPosition.y) / GameVariable.PuyoSize.y;
            if (ratio > biggestRatio) biggestRatio = ratio;

            puyo.transform.positionTo(0.1f * speed * ratio, newPosition);
        }
        yield return new WaitForSeconds(biggestRatio * 0.1f * speed);
    }

    public IEnumerator GameOverAction(System.Action<bool> callBack)
    {
        // 1 = Screen1, 2 = Screen2, 3 = Screen3, 4 = Screen4
        for (int count = 0; count < 4; count++)
        {
            for (int i = 0; i < GameVariable.Rows; i++)
                for (int j = 0; j < GameVariable.ColumnsA; j++)
                    for (int k = 0; k < GameVariable.ColumnsB; k++)
                        if (puyos[i, j, k] != null)
                        {
                            var pPos = puyos[i, j, k].transform.position;
                            if (GameVariable.Scene == Control.Screen1 || GameVariable.Scene == Control.Screen3)
                            {
                                StartCoroutine(SwagPuyo((int)pPos.x % 2, i, j, k, count));
                            }
                            else if (GameVariable.Scene == Control.Screen2 || GameVariable.Scene == Control.Screen4)
                            {
                                StartCoroutine(SwagPuyo((int)pPos.z % 2 + 2, i, j, k, count));
                            }
                        }
            yield return new WaitForSeconds(0.06f);
        }

        System.Random r = new System.Random();
        float[][] speed = new float[3][];

        for (int i = 0; i < 3; i++)
        {
            speed[i] = new float[3];
            for (int j = 0; j < 3; j++)
                speed[i][j] = 1 + (float)r.NextDouble()/2;
        }
        for (int i = 0; i < GameVariable.Rows; i++)
            for (int j = 0; j < GameVariable.ColumnsA; j++)
                for (int k = 0; k < GameVariable.ColumnsB; k++)
                    if (puyos[i, j, k] != null)
                         StartCoroutine(PuyoToHell(speed[j][k], i, j, k));
        yield return null;
        callBack(true);
    }
}

internal interface IEnumerarator
{
        
}