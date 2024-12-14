using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Net.Sockets;
using System.Text;

public class HeadBobbing : MonoBehaviour
{
    [SerializeField] float cycle = 0.7f;      // ヘッドボビング周期
    [SerializeField] float amplitude = 0.5f;  // ヘッドボビング振幅
    [SerializeField] float speed = 1.0f;      // 前進速度
    [SerializeField] GameObject parent;       // OVRPlayerControllerをアサイン

    [SerializeField] float firstSegmentTime = 20f;   // 最初まっすぐ進む時間
    [SerializeField] float secondSegmentTime = 20f;  // 左方向へ進む時間
    [SerializeField] float thirdSegmentTime = 20f;   // 前方向へ戻る時間

    private bool isMoving = false;
    private TcpClient client;
    private NetworkStream stream;

    // 今回はmoveDirectionは毎フレームparentのforwardを使用するため、独立変数は不要
    // private Vector3 moveDirection;

    void Start()
    {
        // Pythonサーバに接続
        client = new TcpClient("127.0.0.1", 65432);
        stream = client.GetStream();

        // parent（OVRPlayerController）の初期向きはそのまま
        // moveDirection = parent.transform.forward; 
    }

    void Update()
    {
        // シフトキーが押されたら動作を開始
        if (!isMoving && (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)))
        {
            isMoving = true;
            StartMovement();
        }

        // 動作中はparentのforward方向へ移動
        if (isMoving)
        {
            parent.transform.Translate(-parent.transform.forward * speed * Time.deltaTime, Space.World);
        }
    }

    void StartMovement()
    {
        // ヘッドボビング開始
        StartHeadBobbing();

        // Pythonへ"start"メッセージ送信
        TriggerPythonStart();

        // 自動探索開始
        StartCoroutine(AutoExplore());
    }

    void StartHeadBobbing()
    {
        transform.DOPunchPosition(new Vector3(0, amplitude, 0), cycle, 1, 0.5f).SetLoops(-1, LoopType.Restart);
    }

    void TriggerPythonStart()
    {
        if (client.Connected)
        {
            string message = "start";
            byte[] data = Encoding.ASCII.GetBytes(message);
            stream.Write(data, 0, data.Length);
            Debug.Log("Sent 'start' to Python to trigger sequence");
        }
    }

    IEnumerator AutoExplore()
    {
        // 1. 最初の区間: 初期方向に20秒進む
        yield return new WaitForSeconds(firstSegmentTime);

        // 2. 左へ向きを変える(ここで-90度回転)
        // parent自体を回転させることでforward方向が左向きに変わる
        parent.transform.Rotate(0, -90, 0);
        Debug.Log("Direction changed to LEFT: " + parent.transform.forward);
        yield return new WaitForSeconds(secondSegmentTime);

        // 3. 元の前方向へ戻る(ここで+90度回転し、初期方向に戻す)
        parent.transform.Rotate(0, 90, 0);
        Debug.Log("Direction changed BACK to FORWARD: " + parent.transform.forward);
        yield return new WaitForSeconds(thirdSegmentTime);

        // 必要に応じて動作終了
        // isMoving = false;
        // DOTween.Kill(transform);
    }

    void OnApplicationQuit()
    {
        // アプリケーション終了時に接続を閉じる
        if (client != null && client.Connected)
        {
            stream.Close();
            client.Close();
        }
    }
}
