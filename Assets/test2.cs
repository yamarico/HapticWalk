using UnityEngine;
using System.Net.Sockets;
using System.Text;
using DG.Tweening;  // DOTweenを使用
using System.Collections;

public class test2 : MonoBehaviour
{
    [SerializeField] private float stepInterval = 0.75f;   // 一歩ごとの時間間隔
    [SerializeField] private float amplitude = 0.1f;      // ヘッドボビングの振幅
    [SerializeField] private float speed = 2.0f;          // 移動速度
    [SerializeField] private OVRPlayerController playerController;  // OVRPlayerControllerへの参照

    [SerializeField] private float rotateSpeed = 2.0f;

    private TcpClient client;
    private NetworkStream stream;

    private bool isMoving = false;
    private Coroutine stepCoroutine;


    void Start()
    {
        // Pythonサーバに接続
        try
        {
            client = new TcpClient("127.0.0.1", 65432);
            stream = client.GetStream();
        }
        catch (SocketException e)
        {
            Debug.LogError("Pythonサーバへの接続に失敗しました: " + e.Message);
        }

        // OVRPlayerControllerのデフォルト移動を無効化
        if (playerController != null)
        {
            playerController.EnableLinearMovement = false;
            playerController.EnableRotation = false;
        }
    }

    void Update()
    {
        // 左スティックの入力を取得（移動用）
        Vector2 primaryThumbstick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);

        // 右スティックの入力を取得（回転用）
        float rotationInput = OVRInput.Get(OVRInput.RawAxis2D.LThumbstick).x;

        // プレイヤーの回転を常に適用
        playerController.transform.Rotate(0, rotationInput * rotateSpeed, 0);

        // 移動入力がある場合
        if (OVRInput.Get(OVRInput.RawButton.A))
        {
            // 移動開始
            if (!isMoving)
            {
                isMoving = true;
                stepCoroutine = StartCoroutine(StepRoutine());
            }

            // 移動方向を計算（プレイヤーの現在の向きに基づく）
            Vector3 moveDirection = -playerController.transform.forward * speed * Time.deltaTime;
            playerController.transform.position += moveDirection;
        }
        else
        {
            // 移動停止
            if (isMoving)
            {
                isMoving = false;
                if (stepCoroutine != null)
                {
                    StopCoroutine(stepCoroutine);
                }
                // ヘッドボビングを停止
                DOTween.Kill(transform);
            }
        }
    }

    IEnumerator StepRoutine()
    {
        while (isMoving)
        {
            PerformStep();
            yield return new WaitForSeconds(stepInterval);
        }
    }

    void PerformStep()
    {
        SendStepToPython();
        StartHeadBobbing();
    }

    void SendStepToPython()
    {
        if (stream != null)
        {
            try
            {
                string message = "step";
                byte[] data = Encoding.UTF8.GetBytes(message);
                stream.Write(data, 0, data.Length);
            }
            catch (SocketException e)
            {
                Debug.LogError("Pythonへのステップメッセージの送信に失敗しました: " + e.Message);
            }
        }
    }

    void StartHeadBobbing()
    {
        // ローカルY位置をリセット
        Vector3 localPosition = transform.localPosition;
        localPosition.y = 0;
        transform.localPosition = localPosition;

        // ヘッドボビングのアニメーション
        transform.DOPunchPosition(new Vector3(0, amplitude, 0), stepInterval, 0, 0)
            .SetEase(Ease.Linear);
    }

    void OnApplicationQuit()
    {
        if (stream != null)
        {
            try
            {
                string message = "exit";
                byte[] data = Encoding.UTF8.GetBytes(message);
                stream.Write(data, 0, data.Length);
                client.Close();
            }
            catch (SocketException e)
            {
                Debug.LogError("Pythonへの終了メッセージの送信に失敗しました: " + e.Message);
            }
        }
    }
}
