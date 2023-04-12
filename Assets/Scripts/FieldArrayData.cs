// ---------------------------------------------------------  
// FieldArrayData.cs  
//   
// 作成日:  2021/4/8
// 作成者:  HCS 宮西
// ---------------------------------------------------------  
using UnityEngine;
/// <summary>
/// フィールド関係を管理するクラス
/// </summary>
public class FieldArrayData : MonoBehaviour
{

    #region フィールド変数、定数

    // タグリストの名前に紐づく番号
    const int NO_BLOCK = 0;
    const int STATIC_BLOCK = 1;
    const int MOVE_BLOCK = 2;
    const int PLAYER = 3;
    const int TARGET = 4;

    /// <summary>
    /// フィールドオブジェクトのタグリスト
    /// 0 空欄
    /// 1 動かないブロック
    /// 2 動くブロック
    /// 3 プレイヤー
    /// 4 ターゲット
    /// </summary>
    string[] g_fieldObjectTagList =
    {
        "","StaticBlock","MoveBlock","Player","TargetPosition"
    };

    /// <summary>
    /// シーンに配置するオブジェクトのルートをヒエラルキーから設定する
    /// </summary>
    [Header("配置するオブジェクトの親オブジェクトを設定")]
    [SerializeField] GameObject g_fieldRootObject;

    /// <summary>
    /// 動かいないオブジェクトを設定
    /// prefabでも可
    /// </summary>
    [Header("動かないオブジェクトを設定(Tagを識別する)")]
    [SerializeField] GameObject g_staticBlock;
    /// <summary>
    /// 動くオブジェクトを設定
    /// prefabでも可
    /// </summary>
    [Header("動くオブジェクトを設定(Tagを識別する)")]
    [SerializeField] GameObject g_moveBlock;
    /// <summary>
    /// プレイヤーオブジェクトを設定
    /// prefabでも可
    /// </summary>
    [Header("プレイヤーオブジェクトを設定(Tagを識別する)")]
    [SerializeField] GameObject g_player;
    /// <summary>
    /// ターゲットオブジェクトを設定
    /// prefabでも可
    /// </summary>
    [Header("ターゲットオブジェクトを設定(Tagを識別する)")]
    [SerializeField] GameObject g_target;

    /// <summary>
    /// フィールドデータ用の変数を定義
    /// </summary>
    int[,] g_fieldData = {
        { 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0 },
    };

    // 縦横の最大数
    int g_horizontalMaxCount = 0;
    int g_verticalMaxCount = 0;

    /// <summary>
    /// ターゲットデータ用の変数を定義
    /// 初期にg_fieldDataを複製する
    /// ※フィールドデータは常に変化するが
    /// 　ターゲット用データは動かさないことで
    /// 　ターゲットにオブジェクトが重なっても動かせるようにする
    /// 　クリア判定はこのターゲットデータを使う
    /// </summary>
    int[,] g_targetData = {
        { 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0 },
    };
    // ブロックがターゲットに入った数
    int g_targetClearCount = 0;
    // ターゲットの最大数
    int g_targetMaxCount = 0;

    /// <summary>
    /// プレイヤーの位置情報
    /// </summary>
    public Vector2 PlayerPosition { get; set; }

    #endregion

    #region メソッド

    /// <summary>
    /// 初回起動時
    /// シーンに配置されたオブジェクトを元に配列データを生成する
    /// </summary>
    private void Awake()
    {
        SetFieldMaxSize();
        ImageToArray();

    }
    /// <summary>
    /// テスト用
    /// 実装時は削除する方向で！
    /// </summary>
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B)) {
            ArrayToImage();
        }

        if (Input.GetKeyDown(KeyCode.C)) {
            SetFieldMaxSize();
            ImageToArray();
        }

        if (Input.GetKeyDown(KeyCode.H)) {

            // 配列を出力するテスト
            print("Field------------------------------------------");
            for (int y = 0; y <= g_verticalMaxCount; y++) {
                string outPutString = "";
                for (int x = 0; x <= g_horizontalMaxCount; x++) {
                    outPutString += g_fieldData[y, x];
                }
                print(outPutString);
            }
            print("Field------------------------------------------");
            // ターゲット配列を出力するテスト
            print("Target------------------------------------------");
            for (int y = 0; y <= g_verticalMaxCount; y++) {
                string outPutString = "";
                for (int x = 0; x <= g_horizontalMaxCount; x++) {
                    outPutString += g_targetData[y, x];
                }
                print(outPutString);
            }
            print("Target------------------------------------------");
            print("プレイヤーポジション:" + PlayerPosition);
        }
    }

    #region フィールド初期化に関する設定

    /// <summary>
    /// フィールドのサイズを設定する
    /// フィールドの初期化に使う
    /// </summary>
    public void SetFieldMaxSize()
    {
        // フィールドの縦と横の最大数を取得（フィールドの大きさを取得）
        foreach (Transform fieldObject in g_fieldRootObject.transform) {
            /* 
             * 縦方向に関しては座標の兼ね合い上
             * 下に行くほどyは減っていくので-をつけることで
             * yの位置を逆転させている
             */
            int col = Mathf.FloorToInt(fieldObject.position.x);
            int row = Mathf.FloorToInt(-fieldObject.position.y);

            // 横の最大数を設定する
            if (g_horizontalMaxCount < col) {
                g_horizontalMaxCount = col;
            }

            // 縦の最大数を設定する
            if (g_verticalMaxCount < row) {
                g_verticalMaxCount = row;
            }
        }

        // フィールド配列の初期化
        g_fieldData = new int[g_verticalMaxCount + 1, g_horizontalMaxCount + 1];
    }

    /// <summary>
    /// fieldRootObjectの配下にあるオブジェクトのタグを読み取り
    /// XとY座標を基にfieldDataへ格納します（fieldDataは上書き削除します）
    /// fieldDataはfieldData[Y,X]で紐づいている
    /// フィールド初期化に使う
    /// </summary>
    /// <param name="fieldRootObject">フィールドオブジェクトのルートオブジェクトを設定</param>
    public void ImageToArray()
    {
        // フィールドの縦と横の最大数を取得（フィールドの大きさを取得）
        foreach (Transform fieldObject in g_fieldRootObject.transform) {
            /* 
             * 縦方向に関しては座標の兼ね合い上
             * 下に行くほどyは減っていくので-をつけることで
             * yの位置を逆転させている
             */
            int col = Mathf.FloorToInt(fieldObject.position.x);
            int row = Mathf.FloorToInt(-fieldObject.position.y);

            if (g_fieldObjectTagList[STATIC_BLOCK].Equals(fieldObject.tag)) {
                g_fieldData[row, col] = STATIC_BLOCK;
            } else if (g_fieldObjectTagList[MOVE_BLOCK].Equals(fieldObject.tag)) {
                g_fieldData[row, col] = MOVE_BLOCK;
            } else if (g_fieldObjectTagList[PLAYER].Equals(fieldObject.tag)) {
                g_fieldData[row, col] = PLAYER;

                PlayerPosition = new Vector2(row, col);
            } else if (g_fieldObjectTagList[TARGET].Equals(fieldObject.tag)) {
                g_fieldData[row, col] = TARGET;

                // ターゲットの最大カウント
                g_targetMaxCount++;
            }

            // フィールドデータをターゲット用のデータにコピーする
            g_targetData = (int[,])g_fieldData.Clone();
        }
    }
    /// <summary>
    /// fieldData配列を基に
    /// fieldRootObjectの配下を再生成する
    /// （重いので非推奨です、テスト用に作りました）
    /// </summary>
    public void ArrayToImage()
    {
        // 全てのフィールドオブジェクトを削除する
        foreach (Transform fieldObject in g_fieldRootObject.transform) {
            Destroy(fieldObject.gameObject);
        }

        // 配列データを基にオブジェクトを生成する
        for (int row = 0; row <= g_verticalMaxCount; row++) {
            for (int col = 0; col <= g_horizontalMaxCount; col++) {
                switch (g_fieldData[row, col]) {
                    /* 
                     * 縦方向に関しては座標の兼ね合い上
                     * 下に行くほどyは減っていくので-をつけることで
                     * yの位置を逆転させている
                     */
                    case STATIC_BLOCK:
                        Instantiate(g_staticBlock, new Vector2(col, -row), 
                            Quaternion.identity, g_fieldRootObject.transform);
                        break;

                    case MOVE_BLOCK:
                        Instantiate(g_moveBlock, new Vector2(col, -row), 
                            Quaternion.identity, g_fieldRootObject.transform);
                        break;

                    case PLAYER:
                        Instantiate(g_player, new Vector2(col, -row), 
                            Quaternion.identity, g_fieldRootObject.transform);
                        break;

                    case TARGET:
                        Instantiate(g_target, new Vector2(col, -row), 
                            Quaternion.identity, g_fieldRootObject.transform);
                        break;
                }
            }
        }
    }

    /// <summary>
    /// ターゲットの最大数を数える
    /// （未使用）
    /// </summary>
    public void GetTargetCount()
    {
        for (int row = 0; row <= g_verticalMaxCount; row++) {
            for (int col = 0; col <= g_horizontalMaxCount; col++) {
                if (g_fieldData[row, col] == TARGET) {
                    g_targetMaxCount++;
                }
            }
        }
    }
    #endregion

    #region フィールド情報を操作する

    /// <summary>
    /// フィールド情報を取得する
    /// 縦位置と横位置を指定し、その場所のフィールド情報を取得する
    /// （未使用）
    /// </summary>
    /// <param name="col">縦位置</param>
    /// <param name="row">横位置</param>
    /// <returns>ブロックの種類</returns>
    public int GetFieldData(int row, int col)
    {
        return g_fieldData[row, col];
    }

    /// <summary>
    /// フィールド情報を取得する(タグ)
    /// 縦位置と横位置を指定し、その場所のフィールド情報を取得する
    /// （未使用）
    /// </summary>
    /// <param name="row">縦位置</param>
    /// <param name="col">横位置</param>
    /// <returns>タグ名</returns>
    public string GetFieldDataTag(int row, int col)
    {
        // ブロックの種類取得
        int blockKind = GetFieldData(row, col);

        // ブロック種類を指定してタグリストからタグを取得する
        return g_fieldObjectTagList[blockKind];
    }

    /// <summary>
    /// オブジェクトを移動する
    /// データを上書きするので移動できるかどうか検査して
    /// 移動可能な場合処理を実行してください
    /// </summary>
    /// <param name="preRow">移動前縦情報</param>
    /// <param name="preCol">移動前横情報</param>
    /// <param name="nextRow">移動後縦情報</param>
    /// <param name="nextCol">移動後横情報</param>
    public void MoveData(int preRow, int preCol, int nextRow, int nextCol)
    {
        // オブジェクトを移動する
        GameObject moveObject = GetFieldObject(g_fieldData[preRow, preCol], preRow, preCol);
        if (moveObject != null) {
            /* 
             * 縦方向に関しては座標の兼ね合い上
             * 下に行くほどyは減っていくので-をつけることで
             * yの位置を逆転させている
             */
            // 座標情報なので最初の引数はX
            moveObject.transform.position = new Vector2(nextCol, -nextRow);
        }
        // 上書きするので要注意
        g_fieldData[nextRow, nextCol] = g_fieldData[preRow, preCol];

        // 移動したら元のデータは削除する
        g_fieldData[preRow, preCol] = NO_BLOCK;

    }
    /// <summary>
    /// フィールドオブジェクトから指定した座標のオブジェクトを取得する
    /// tagIdが-1の場合すべてのタグを対象に検索する
    /// 検索にヒットしない場合Nullを返す
    /// </summary>
    /// <param name="tagId">検索対象のタグを指定</param>
    /// <param name="row">縦位置</param>
    /// <param name="col">横位置</param>
    /// <returns></returns>
    public GameObject GetFieldObject(int tagId, int row, int col)
    {
        foreach (Transform fieldObject in g_fieldRootObject.transform) {
            if (tagId != -1 && fieldObject.tag != g_fieldObjectTagList[tagId]) {
                continue;
            }
            /* 
             * 縦方向に関しては座標の兼ね合い上
             * 下に行くほどyは減っていくので-をつけることで
             * yの位置を逆転させている
             */
            if (fieldObject.transform.position.x == col && 
                fieldObject.transform.position.y == -row) {
                return fieldObject.gameObject;
            }
        }
        return null;

    }
    /// <summary>
    /// ブロックを移動することが可能かチェックする
    /// trueの場合移動可能　flaseの場合移動不可能
    /// </summary>
    /// <param name="y">移動先Y座標</param>
    /// <param name="x">移動先X座標</param>
    /// <returns>ブロック移動の可否</returns>
    public bool BlockMoveCheck(int y, int x)
    {
        // ターゲットブロックだったら
        if (g_targetData[y, x] == TARGET) {
            // ターゲットクリアカウントを上げる
            g_targetClearCount++;

            return true;
        }

        return g_fieldData[y, x] == NO_BLOCK;
    }
    /// <summary>
    /// ブロックを移動する(ブロック移動チェックも実施)
    /// </summary>
    /// <param name="preRow">移動前縦情報</param>
    /// <param name="preCol">移動前横情報</param>
    /// <param name="nextRow">移動後縦情報</param>
    /// <param name="nextCol">移動後横情報</param>
    public bool BlockMove(int preRow, int preCol, int nextRow, int nextCol)
    {
        // 境界線外エラー
        if (nextRow < 0 || nextCol < 0 ||
            nextRow > g_verticalMaxCount || nextCol > g_horizontalMaxCount) {
            return false;
        }

        bool moveFlag = BlockMoveCheck(nextRow, nextCol);
        // 移動可能かチェックする
        if (moveFlag) {
            // 移動が可能な場合移動する
            MoveData(preRow, preCol, nextRow, nextCol);
        }
        return moveFlag;
    }
    /// <summary>
    /// プレイヤーを移動することが可能かチェックする
    /// trueの場合移動可能　flaseの場合移動不可能
    /// </summary>
    /// <param name="preRow">移動前縦情報</param>
    /// <param name="preCol">移動前横情報</param>
    /// <param name="nextRow">移動後縦情報</param>
    /// <param name="nextCol">移動後横情報</param>
    /// <returns>プレイヤー移動の可否</returns>
    public bool PlayerMoveCheck(int preRow, int preCol, int nextRow, int nextCol)
    {
        /* プレイヤーの移動先が動くブロックの時
         * ブロックを移動する処理を実施する
         */
        if (g_fieldData[nextRow, nextCol] == MOVE_BLOCK) {
            bool blockMoveFlag = BlockMove(nextRow, nextCol, 
                nextRow + (nextRow - preRow), 
                nextCol + (nextCol - preCol));
            // ターゲットブロックかつ移動できる移動ブロックだったら
            if (g_targetData[nextRow, nextCol] == TARGET && blockMoveFlag) {
                // ターゲットクリアカウントを下げる
                g_targetClearCount--;
            }

            return blockMoveFlag;

        }
        // プレイヤーの移動先が空の時移動する
        // プレイヤーの移動先がターゲットの時移動する
        if (g_fieldData[nextRow, nextCol] == NO_BLOCK ||
            g_fieldData[nextRow, nextCol] == TARGET) {
            return true;
        }
        return false;
    }
    /// <summary>
    /// プレイヤーを移動する(プレイヤー移動チェックも実施)
    /// </summary>
    /// <param name="preRow">移動前縦情報</param>
    /// <param name="preCol">移動前横情報</param>
    /// <param name="nextRow">移動後縦情報</param>
    /// <param name="nextCol">移動後横情報</param>
    public void PlayerMove(int preRow, int preCol, int nextRow, int nextCol)
    {
        // 移動可能かチェックする
        if (PlayerMoveCheck(preRow, preCol, nextRow, nextCol)) {
            // 移動が可能な場合移動する
            MoveData(preRow, preCol, nextRow, nextCol);
            // プレイヤーの位置を更新する
            // 座標情報なので最初の引数はX
            PlayerPosition = new Vector2(nextRow, nextCol);
        }

    }
    #endregion

    #region ゲームクリア判定

    /// <summary>
    /// ゲームクリアの判定
    /// </summary>
    /// <returns>ゲームクリアの有無</returns>
    public bool GetGameClearJudgment()
    {
        // ターゲットクリア数とターゲットの最大数が一致したらゲームクリア
        if (g_targetClearCount == g_targetMaxCount) {
            return true;
        }
        return false;
    }

    #endregion

    #endregion

}
