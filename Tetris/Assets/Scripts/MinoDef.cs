using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinoDef
{
    public enum MinoType : uint
    {
        TMino = 0,
        SMino,
        ZMino,
        LMino,
        JMino,
        IMino,
        OMino,
        Num,
    }

    public const int MINO_ROTATE_NUM = 4;
    public const int MINO_SIZE_X = 4;
    public const int MINO_SIZE_Y = 4;

    public static int[,,] T_MINO_TABLE = new int[MINO_ROTATE_NUM, MINO_SIZE_Y, MINO_SIZE_X]{
        {
            {0, 1, 0, 0 },
            {1, 1, 1, 0 },
            {0, 0, 0, 0 },
            {0, 0, 0, 0 },
        },
        {
            {0, 1, 0, 0 },
            {0, 1, 1, 0 },
            {0, 1, 0, 0 },
            {0, 0, 0, 0 },
        },
        {
            {0, 0, 0, 0 },
            {1, 1, 1, 0 },
            {0, 1, 0, 0 },
            {0, 0, 0, 0 },
        },
        {
            {0, 1, 0, 0 },
            {1, 1, 0, 0 },
            {0, 1, 0, 0 },
            {0, 0, 0, 0 },
        },
    };

    public static int[,,] S_MINO_TABLE = new int[MINO_ROTATE_NUM, MINO_SIZE_X, MINO_SIZE_Y]{
        {
            {0, 1, 1, 0 },
            {1, 1, 0, 0 },
            {0, 0, 0, 0 },
            {0, 0, 0, 0 },
        },
        {
            {0, 1, 0, 0 },
            {0, 1, 1, 0 },
            {0, 0, 1, 0 },
            {0, 0, 0, 0 },
        },
        {
            {0, 0, 0, 0 },
            {0, 1, 1, 0 },
            {1, 1, 0, 0 },
            {0, 0, 0, 0 },
        },
        {
            {1, 0, 0, 0 },
            {1, 1, 0, 0 },
            {0, 1, 0, 0 },
            {0, 0, 0, 0 },
        },
    };

    public static int[,,] Z_MINO_TABLE = new int[MINO_ROTATE_NUM, MINO_SIZE_X, MINO_SIZE_Y]{
        {
            {1, 1, 0, 0 },
            {0, 1, 1, 0 },
            {0, 0, 0, 0 },
            {0, 0, 0, 0 },
        },
        {
            {0, 0, 1, 0 },
            {0, 1, 1, 0 },
            {0, 1, 0, 0 },
            {0, 0, 0, 0 },
        },
        {
            {0, 0, 0, 0 },
            {1, 1, 0, 0 },
            {0, 1, 1, 0 },
            {0, 0, 0, 0 },
        },
        {
            {0, 1, 0, 0 },
            {1, 1, 0, 0 },
            {1, 0, 0, 0 },
            {0, 0, 0, 0 },
        },
    };

    public static int[,,] L_MINO_TABLE = new int[MINO_ROTATE_NUM, MINO_SIZE_X, MINO_SIZE_Y]{
        {
            {0, 0, 1, 0 },
            {1, 1, 1, 0 },
            {0, 0, 0, 0 },
            {0, 0, 0, 0 },
        },
        {
            {0, 1, 0, 0 },
            {0, 1, 0, 0 },
            {0, 1, 1, 0 },
            {0, 0, 0, 0 },
        },
        {
            {0, 0, 0, 0 },
            {1, 1, 1, 0 },
            {1, 0, 0, 0 },
            {0, 0, 0, 0 },
        },
        {
            {1, 1, 0, 0 },
            {0, 1, 0, 0 },
            {0, 1, 0, 0 },
            {0, 0, 0, 0 },
        },
    };

    public static int[,,] J_MINO_TABLE = new int[MINO_ROTATE_NUM, MINO_SIZE_X, MINO_SIZE_Y]{
        {
            {1, 0, 0, 0 },
            {1, 1, 1, 0 },
            {0, 0, 0, 0 },
            {0, 0, 0, 0 },
        },
        {
            {0, 1, 1, 0 },
            {0, 1, 0, 0 },
            {0, 1, 0, 0 },
            {0, 0, 0, 0 },
        },
        {
            {0, 0, 0, 0 },
            {1, 1, 1, 0 },
            {0, 0, 1, 0 },
            {0, 0, 0, 0 },
        },
        {
            {0, 1, 0, 0 },
            {0, 1, 0, 0 },
            {1, 1, 0, 0 },
            {0, 0, 0, 0 },
        },
    };

    public static int[,,] I_MINO_TABLE = new int[MINO_ROTATE_NUM, MINO_SIZE_X, MINO_SIZE_Y]{
        {
            {0, 0, 0, 0 },
            {1, 1, 1, 1 },
            {0, 0, 0, 0 },
            {0, 0, 0, 0 },
        },
        {
            {0, 0, 1, 0 },
            {0, 0, 1, 0 },
            {0, 0, 1, 0 },
            {0, 0, 1, 0 },
        },
        {
            {0, 0, 0, 0 },
            {0, 0, 0, 0 },
            {1, 1, 1, 1 },
            {0, 0, 0, 0 },
        },
        {
            {0, 1, 0, 0 },
            {0, 1, 0, 0 },
            {0, 1, 0, 0 },
            {0, 1, 0, 0 },
        },
    };

    public static int[,,] O_MINO_TABLE = new int[MINO_ROTATE_NUM, MINO_SIZE_X, MINO_SIZE_Y]{
        {
            {0, 1, 1, 0 },
            {0, 1, 1, 0 },
            {0, 0, 0, 0 },
            {0, 0, 0, 0 },
        },
        {
            {0, 1, 1, 0 },
            {0, 1, 1, 0 },
            {0, 0, 0, 0 },
            {0, 0, 0, 0 },
        },
        {
            {0, 1, 1, 0 },
            {0, 1, 1, 0 },
            {0, 0, 0, 0 },
            {0, 0, 0, 0 },
        },
        {
            {0, 1, 1, 0 },
            {0, 1, 1, 0 },
            {0, 0, 0, 0 },
            {0, 0, 0, 0 },
        },
    };
}
