using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class Field : MonoBehaviour
{
    public const int FIELD_SIZE_X = 10;
    public const int FIELD_SIZE_Y = 20;

    const int POS_X = 0;
    const int POS_Y = 1;

    public struct CellData
    {
        public bool IsActive;
        public MinoDef.MinoType MinoType;

        public CellData(bool inActive = false, MinoDef.MinoType minoType = MinoDef.MinoType.Num)
        {
            IsActive = inActive;
            MinoType = minoType;
        }

        public void SetActive(bool inActive)
        {
            IsActive = inActive;
        }
    };

    CellData[,] FieldData = new CellData[FIELD_SIZE_X, FIELD_SIZE_Y];
    Piece[,] FieldObject = new Piece[FIELD_SIZE_X, FIELD_SIZE_Y];

    int RotateNum = 0;

    int[] MinoPos = new int[2];

    MinoDef.MinoType ActiveMinoType;
    int[,,] ActiveMinoData = new int[MinoDef.MINO_ROTATE_NUM, MinoDef.MINO_SIZE_X, MinoDef.MINO_SIZE_Y];

    Queue<MinoDef.MinoType> MinoQueue = new Queue<MinoDef.MinoType>();

    int FreeFallIntervalFrame = 20; // 自由落下速度（レベルに応じて変化）

    int FreeFallCount = 0;

    public static Field Instance;

    GameObject PieceInstanse;

    AsyncOperationHandle<GameObject> opHandle;

    GameManager GameManagerComponent;

    public enum eState : uint
    {
        Idle = 0,
        FallStart,
        FreeFall,
        Grounding,
        GameEnd,
    }

    private eState State = eState.Idle;

    private void Awake()
    {
        if(Instance != null)
        {
            Instance = this;
        }
        LoadAssets();
        Initialize();
        FieldBorderGenerate();
        GameManagerComponent = this.GetComponent<GameManager>(); 
    }

    // Update is called once per frame
    void Update()
    {
        switch (State)
        {
            case eState.Idle:
                {
                    // キー入力があるまで待機
                    GameManager.eKeys PressKeys = GameManagerComponent.GetPressKeys();
                    if (PressKeys != 0)
                    {
                        CheckQueue();
                        State = eState.FallStart;
                    }
                }
                break;
            case eState.FallStart:
                {
                    FallStart();
                    CheckQueue();
                    ReflectedInField();
                    if (CheckField())
                    {
                        State = eState.FreeFall;
                    }
                    else
                    {
                        State = eState.GameEnd;
                    }
                }
                break; 
            case eState.FreeFall:
                {
                    GameManager.eKeys PressKeys = GameManagerComponent.GetPressKeys();

                    // 入力に応じた処理
                    if ((PressKeys & GameManager.eKeys.MoveDown) == GameManager.eKeys.MoveDown)
                    {
                        if (MinoMoveDown())
                        {
                            ReflectedInField();
                            FreeFallCount = 0;
                        }
                        else
                        {
                            State = eState.Grounding;
                        }
                    }
                    if ((PressKeys & GameManager.eKeys.MoveLeft) == GameManager.eKeys.MoveLeft)
                    {
                        if(MinoMoveLeft())
                        {
                            ReflectedInField();
                        }
                    }
                    if ((PressKeys & GameManager.eKeys.MoveRight) == GameManager.eKeys.MoveRight)
                    {
                        if (MinoMoveRight())
                        {
                            ReflectedInField();
                        }
                    }
                    if ((PressKeys & GameManager.eKeys.TurnLeft) == GameManager.eKeys.TurnLeft)
                    {
                        if (MinoRotateLeft())
                        {
                            ReflectedInField();
                        }
                    }
                    if ((PressKeys & GameManager.eKeys.TurnRight) == GameManager.eKeys.TurnRight)
                    {
                        if (MinoRotateRight())
                        {
                            ReflectedInField();
                        }
                    }

                    // 自由落下処理
                    FreeFallCount++;
                    if (FreeFallCount >= FreeFallIntervalFrame && PressKeys == 0)
                    {
                        if (MinoMoveDown())
                        {
                            ReflectedInField();
                            FreeFallCount = 0;
                        }
                        else
                        {
                            State = eState.Grounding;
                        }
                    }
                }
                break;
            case eState.Grounding:
                {
                    Grounding();
                    LineCheck();
                    ReflectedInField();
                    State = eState.FallStart;
                }
                break;
            case eState.GameEnd:
                {
                    Debug.Log("GameEnd");

                    GameManager.eKeys PressKeys = GameManagerComponent.GetPressKeys();
                    // キー入力があったら再スタート
                    if (PressKeys != 0)
                    {
                        MinoQueue.Clear();
                        CheckQueue();
                        State = eState.FallStart;
                    }
                }
                break;
        }
        GameManagerComponent.CleaePressKeys();
    }

    public void LoadAssets()
    {
        PieceInstanse = Addressables.LoadAssetAsync<GameObject>("Assets/Scenes/Piece.prefab").WaitForCompletion();
    }

    public void Initialize()
    {
        FieldData = new CellData[FIELD_SIZE_X, FIELD_SIZE_Y];
        RotateNum = 0;

        GameObject parentObj = new GameObject("CellObjectParent");
        for (int x = 0; x < FIELD_SIZE_X; x++)
        {
            for (int y = 0; y < FIELD_SIZE_Y; y++)
            {
                FieldObject[x, y] = Instantiate(PieceInstanse, new Vector2((-FIELD_SIZE_X + 1) / 2.0f + x, (FIELD_SIZE_Y - 1) / 2.0f - y), Quaternion.identity, parentObj.transform).GetComponent<Piece>();
                FieldObject[x, y].gameObject.SetActive(false);
                FieldData[x, y].SetActive(false);
            }
        }
    }

    /// <summary>
    /// フィールド枠を表示するオブジェクトの生成
    /// </summary>
    private void FieldBorderGenerate()
    {
        GameObject parentObj = new GameObject("FieldBorderParent");

        for (int y = 0; y < FIELD_SIZE_Y; y++)
        {
            Instantiate(PieceInstanse, new Vector2(-(FIELD_SIZE_X + 1) / 2.0f, (-FIELD_SIZE_Y + 1) / 2.0f + y), Quaternion.identity, parentObj.transform);
            Instantiate(PieceInstanse, new Vector2((FIELD_SIZE_X + 1) / 2.0f, (-FIELD_SIZE_Y + 1) / 2.0f + y), Quaternion.identity, parentObj.transform);
        }

        for (int x = 0; x < FIELD_SIZE_X + 2; x++)
        {
            Instantiate(PieceInstanse, new Vector2((-FIELD_SIZE_X - 1) / 2.0f + x , -(FIELD_SIZE_Y + 1) / 2.0f), Quaternion.identity, parentObj.transform);
        }
    }

    /// <summary>
    /// ネクストが十分に残っているかチェック
    /// </summary>
    private void CheckQueue()
    {
        if(MinoQueue.Count <= 5)
        {
            // 全種類のミノが入ってるリストの作成
            List<MinoDef.MinoType> minoList = new List<MinoDef.MinoType>() { MinoDef.MinoType.TMino, MinoDef.MinoType.SMino, MinoDef.MinoType.ZMino, MinoDef.MinoType.LMino , MinoDef.MinoType.JMino , MinoDef.MinoType.IMino, MinoDef.MinoType.OMino };

            // 順番のシャッフル
            int n = minoList.Count;
            for (int i = n - 1; i > 0; i--)
            {
                int j = UnityEngine.Random.Range(0, i + 1);
                MinoDef.MinoType tmp = minoList[i];
                minoList[i] = minoList[j];
                minoList[j] = tmp;
            }

            foreach(MinoDef.MinoType type in minoList) 
            {
                MinoQueue.Enqueue(type);
            }
        }
    }

    /// <summary>
    /// 操作するミノの初期化・セット
    /// </summary>
    private void FallStart()
    {
        MinoPos[POS_X] = 3;
        MinoPos[POS_Y] = 0;
        RotateNum = 0;
        ActiveMinoType = MinoQueue.Dequeue();

        switch(ActiveMinoType)
        {
            case MinoDef.MinoType.TMino:
                {
                    Debug.Log("TMino");
                    ActiveMinoData = MinoDef.T_MINO_TABLE;
                }
                break;
            case MinoDef.MinoType.SMino:
                {
                    Debug.Log("SMino");
                    ActiveMinoData = MinoDef.S_MINO_TABLE;
                }
                break;
            case MinoDef.MinoType.ZMino:
                {
                    Debug.Log("ZMino");
                    ActiveMinoData = MinoDef.Z_MINO_TABLE;
                }
                break;
            case MinoDef.MinoType.LMino:
                {
                    Debug.Log("LMino");
                    ActiveMinoData = MinoDef.L_MINO_TABLE;
                }
                break;
            case MinoDef.MinoType.JMino:
                {
                    Debug.Log("JMino");
                    ActiveMinoData = MinoDef.J_MINO_TABLE;
                }
                break;
            case MinoDef.MinoType.IMino:
                {
                    Debug.Log("IMino");
                    ActiveMinoData = MinoDef.I_MINO_TABLE;
                }
                break;
            case MinoDef.MinoType.OMino:
                {
                    Debug.Log("OMino");
                    ActiveMinoData = MinoDef.O_MINO_TABLE;
                }
                break;
        }
    }

    /// <summary>
    /// 操作中のミノが配置されているブロックと競合していないかチェック
    /// </summary>
    private bool CheckField()
    {
        for(int x = 0; x < MinoDef.MINO_SIZE_X; x++) 
        {
            for (int y = 0; y < MinoDef.MINO_SIZE_Y; y++)
            {
                if (ActiveMinoData[RotateNum, y, x] == 1)
                {
                    if (MinoPos[POS_X] + x < 0 || MinoPos[POS_X] + x >= FIELD_SIZE_X)
                    {
                        return false;
                    }
                    if (MinoPos[POS_Y] + y < 0 || MinoPos[POS_Y] + y >= FIELD_SIZE_Y)
                    {
                        return false;
                    }
                    if (FieldData[MinoPos[POS_X] + x, MinoPos[POS_Y] + y].IsActive)
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }

    /// <summary>
    /// フィールド上にミノとブロックの表示を反映
    /// </summary>
    private void ReflectedInField()
    {
        Debug.Log("ReflectedInField");

        // 一度すべてのブロックを非表示
        foreach(var obj in FieldObject)
        {
            obj.gameObject.SetActive(false);
        }

        // ミノ部分の表示
        for (int x = 0; x < MinoDef.MINO_SIZE_X; x++)
        {
            for (int y = 0; y < MinoDef.MINO_SIZE_Y; y++)
            {
                if (ActiveMinoData[RotateNum, y, x] == 1 )
                {
                    FieldObject[MinoPos[POS_X] + x, MinoPos[POS_Y] + y].gameObject.SetActive(true);
                    FieldObject[MinoPos[POS_X] + x, MinoPos[POS_Y] + y].SetColor(ActiveMinoType);
                }
            }
        }
        // ブロック部分の表示
        for (int x = 0; x < FIELD_SIZE_X; x++)
        {
            for (int y = 0; y < FIELD_SIZE_Y; y++)
            {
                if (FieldData[x, y].IsActive)
                {
                    FieldObject[x, y].gameObject.SetActive(true);
                    FieldObject[x, y].SetColor(FieldData[x, y].MinoType);
                }
            }
        }
    }

    /// <summary>
    /// ミノを下移動させる
    /// </summary>
    /// <returns>失敗した場合はfalseを返す</returns>
    private bool MinoMoveDown()
    {
        MinoPos[POS_Y]++;
        if(!CheckField())
        {
            MinoPos[POS_Y]--;
            return false;
        }
        return true;
    }

    /// <summary>
    /// ミノを下移動させる
    /// </summary>
    /// <returns>失敗した場合はfalseを返す</returns>
    private bool MinoMoveLeft()
    {
        MinoPos[POS_X]--;
        if (!CheckField())
        {
            MinoPos[POS_X]++;
            return false;
        }
        return true;
    }

    /// <summary>
    /// ミノを下移動させる
    /// </summary>
    /// <returns>失敗した場合はfalseを返す</returns>
    private bool MinoMoveRight()
    {
        MinoPos[POS_X]++;
        if (!CheckField())
        {
            MinoPos[POS_X]--;
            return false;
        }
        return true;
    }

    /// <summary>
    /// ミノを左回転させる
    /// </summary>
    /// <returns>失敗した場合はfalseを返す</returns>
    private bool MinoRotateLeft()
    {
        RotateNum++;
        RotateNum %= 4;
        if (!CheckField())
        {
            RotateNum += 3;
            RotateNum %= 4;
            return false;
        }
        return true;
    }

    /// <summary>
    /// ミノを右回転させる
    /// </summary>
    /// <returns>失敗した場合はfalseを返す</returns>
    private bool MinoRotateRight()
    {
        RotateNum += 3;
        RotateNum %= 4;
        if (!CheckField())
        {
            RotateNum++;
            RotateNum %= 4;
            return false;
        }
        return true;
    }

    /// <summary>
    /// 接地処理
    /// </summary>
    private void Grounding()
    {
        for (int x = 0; x < MinoDef.MINO_SIZE_X; x++)
        {
            for (int y = 0; y < MinoDef.MINO_SIZE_Y; y++)
            {
                if (ActiveMinoData[RotateNum, y, x] == 1)
                {
                    if (MinoPos[POS_X] + x < 0 || MinoPos[POS_X] + x >= FIELD_SIZE_X)
                    {
                        continue;
                    }
                    if (MinoPos[POS_Y] + y < 0 || MinoPos[POS_Y] + y >= FIELD_SIZE_Y)
                    {
                        continue;
                    }
                    FieldData[MinoPos[POS_X] + x, MinoPos[POS_Y] + y].SetActive(true);
                    FieldData[MinoPos[POS_X] + x, MinoPos[POS_Y] + y].MinoType = ActiveMinoType;
                }
            }
        }
    }

    /// <summary>
    /// ライン消去処理
    /// </summary>
    private void LineCheck()
    {
        for (int y = 0; y < FIELD_SIZE_Y; y++)
        {
            bool isLineActive = true;
            for (int x = 0; x < FIELD_SIZE_X; x++)
            {
                isLineActive &= FieldData[x, y].IsActive;
            }

            if(isLineActive)
            {
                for(int i = y; i > 0; i--)
                {
                    for (int x = 0; x < FIELD_SIZE_X; x++)
                    {
                        FieldData[x, i] = FieldData[x, i - 1];
                    }
                }
                for (int x = 0; x < FIELD_SIZE_X; x++)
                {
                    FieldData[x, 0] = new CellData();
                }
            }
        }
    }
}