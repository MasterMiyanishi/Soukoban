// ---------------------------------------------------------  
// GameController.cs  
//   
// 作成日:  2021/4/8
// 作成者:  HCS 宮西
// ---------------------------------------------------------  
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ゲームの状態を管理するクラス
/// </summary>
public class GameController : MonoBehaviour
{
    // フィールド操作クラスの定義
    FieldArrayData g_fieldArrayData;

    #region ゲーム状態関連
    /// <summary>
    /// ゲームの状態管理用構造体
    /// START : ゲーム開始前処理
    /// STOP : ゲーム停止状態
    /// BLOCK_MOVE : ブロック移動処理
    /// PLAYER : プレイヤー操作処理
    /// END : ゲームオーバー処理
    /// </summary>
    enum GameState
    {
        START,
        STOP,
        BLOCK_MOVE,
        PLAYER,
        END,
    }
    /// <summary>
    /// 現在のゲーム状態
    /// </summary>
    [SerializeField] GameState g_gameState = GameState.START;

    /// <summary>
    /// ゲームの状態設定を行うメソッド
    /// ゲームの状態は以下参照
    /// START : ゲーム開始前処理
    /// STOP : ゲーム停止状態
    /// BLOCK_MOVE : ブロック移動処理
    /// PLAYER : プレイヤー操作処理
    /// END : ゲームオーバー処理
    /// </summary>
    /// <param name="gameState">ゲームの状態を指定</param>
    void SetGameState(GameState gameState)
    {
        this.g_gameState = gameState;
    }
    /// <summary>
    /// 現在のゲーム状態を取得する
    /// ゲームの状態は以下参照
    /// START : ゲーム開始前処理
    /// STOP : ゲーム停止状態
    /// BLOCK_MOVE : ブロック移動処理
    /// PLAYER : プレイヤー操作処理
    /// END : ゲームオーバー処理
    /// </summary>
    /// <returns>ゲーム状態</returns>
    GameState GetGameState()
    {
        return this.g_gameState;
    }
    #endregion

    // キーパットの入力状態
    bool g_inputState = false;

    void Awake()
    {
        // コンポーネント取得
        g_fieldArrayData = GetComponent<FieldArrayData>();
    }

    void Update()
    {
        // ゲーム状態によって処理を分ける
        switch (g_gameState) {
            case GameState.START:
                SetGameState(GameState.PLAYER);
                break;
            case GameState.STOP:
                break;
            case GameState.PLAYER:
                float horizontalInput = Input.GetAxisRaw("Horizontal");
                float verticalInput = Input.GetAxisRaw("Vertical");

                // 横入力が0より大きい場合は右に移動
                if (horizontalInput > 0 && !g_inputState) {
                    g_fieldArrayData.PlayerMove(
                        Mathf.FloorToInt(g_fieldArrayData.PlayerPosition.x), 
                        Mathf.FloorToInt(g_fieldArrayData.PlayerPosition.y),
                        Mathf.FloorToInt(g_fieldArrayData.PlayerPosition.x), 
                        Mathf.FloorToInt(g_fieldArrayData.PlayerPosition.y + 1));
                    g_inputState = true;
                }
                // 横入力が0より小さい場合は左に移動
                else if (horizontalInput < 0 && !g_inputState) {
                    g_fieldArrayData.PlayerMove(
                        Mathf.FloorToInt(g_fieldArrayData.PlayerPosition.x), 
                        Mathf.FloorToInt(g_fieldArrayData.PlayerPosition.y),
                        Mathf.FloorToInt(g_fieldArrayData.PlayerPosition.x),
                        Mathf.FloorToInt(g_fieldArrayData.PlayerPosition.y - 1));
                    g_inputState = true;
                }
                // 縦入力が0より大きい場合は上に移動
                if (verticalInput > 0 && !g_inputState) {
                    g_fieldArrayData.PlayerMove(
                        Mathf.FloorToInt(g_fieldArrayData.PlayerPosition.x), 
                        Mathf.FloorToInt(g_fieldArrayData.PlayerPosition.y),
                        Mathf.FloorToInt(g_fieldArrayData.PlayerPosition.x - 1), 
                        Mathf.FloorToInt(g_fieldArrayData.PlayerPosition.y));
                    g_inputState = true;
                }
                // 縦入力が0より小さい場合は下に移動
                else if (verticalInput < 0 && !g_inputState) {
                    g_fieldArrayData.PlayerMove(
                        Mathf.FloorToInt(g_fieldArrayData.PlayerPosition.x),
                        Mathf.FloorToInt(g_fieldArrayData.PlayerPosition.y),
                        Mathf.FloorToInt(g_fieldArrayData.PlayerPosition.x + 1), 
                        Mathf.FloorToInt(g_fieldArrayData.PlayerPosition.y));
                    g_inputState = true;
                }

                // 入力状態が解除されるまで再入力できないようにする
                if ((horizontalInput + verticalInput) == 0) {

                    g_inputState = false;
                }

                // クリア判定
                if (g_fieldArrayData.GetGameClearJudgment()) {
                    g_gameState = GameState.END;
                }

                break;
            case GameState.BLOCK_MOVE:
                break;
            case GameState.END:
                break;
        }
    }
}
