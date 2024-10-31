using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Net.Sockets;
using System.Text;

public class HeadBobbing : MonoBehaviour
{
    [SerializeField] float cycle = 0.7f;      // 周波数（周期）
    [SerializeField] float amplitude = 0.5f;  // 振幅
    [SerializeField] float speed = 1.0f;      // Z軸方向への移動速度
    [SerializeField] GameObject parent;      // Z軸方向への移動速度
    private bool isMoving = false;            // 動作開始フラグ

    private TcpClient client;
    private NetworkStream stream;

    void Start()
    {
        // Pythonサーバに接続
        client = new TcpClient("127.0.0.1", 65432);
        stream = client.GetStream();
    }

    void Update()
    {
        // シフトキーが押されたら動作を開始
        if (!isMoving && (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)))
        {
            isMoving = true;
            StartMovement();
        }

        // シフトキーが押された後、Z軸方向に「speed」だけ進む
        if (isMoving)
        {
            parent.transform.Translate(-Vector3.forward * speed * Time.deltaTime);
        }
    }

    void StartMovement()
    {
        // ヘッドボビングの動き開始
        StartHeadBobbing();

        // Pythonに「start」メッセージを送信
        TriggerPythonStart();
    }

    void StartHeadBobbing()
    {
        // Y軸方向に「振幅」だけ周期的に動く（周波数は「周期」で調整）
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

    void OnApplicationQuit()
    {
        // アプリケーション終了時に接続を閉じる
        if (client.Connected)
        {
            stream.Close();
            client.Close();
        }
    }
}
