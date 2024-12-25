using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Net.Sockets;
using System.Text;
using TMPro;

public class HeadBobbing : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] float speed = 1.0f;        // 前進速度
    [SerializeField] GameObject parent;         // OVRPlayerControllerをアサイン

    [Header("Head Bobbing Settings")]
    [SerializeField] float cycle = 1.2f;        // ヘッドボビングの周期
    [SerializeField] float amplitude = 0.5f;    // ヘッドボビング振幅
    [SerializeField] float gap = 0.2f; //

    [Header("Time Settings (Adjust in Inspector)")]
    [SerializeField] float forwardTime1 = 10f;
    [SerializeField] float rotateTime1 = 10f;
    [SerializeField] float forwardTime2 = 10f;
    [SerializeField] float rotateTime2 = 10f;
    [SerializeField] float forwardTime3 = 10f;
    [SerializeField] float rotateTime3 = 10f;
    [SerializeField] float forwardTime4 = 10f;
    [SerializeField] float rotateTime4 = 10f;
    [SerializeField] float forwardTime5 = 10f;

    [Header("Rotation Settings")]
    [SerializeField] float rotationSpeedDegPerSec = 9f; // 回転速度(度/秒), 回転区間で使う

    [Header("Arm Rotation Objects")]
    [SerializeField] float rotationSpeed = 45f ;
    public Transform objectL;
    public Transform objectR;
    [Header("Text Box")]
    public TextMeshProUGUI switch_text;

    // アーム回転用パラメータ(旧DualArmRotate互換)

    private float currentAngleL = 135f;
    private int directionL = -1; // -1: 180度→0度へ
    private float currentAngleR = 45f;
    private int directionR = 1;  // 1: 0度→180度へ

    private bool isMoving = false;
    private bool isArmsActive = false; // アームが回転中かどうか
    private TcpClient client;
    private NetworkStream stream;

    void Start()
    {
        // Pythonサーバに接続
        client = new TcpClient("127.0.0.1", 65432);
        stream = client.GetStream();
        isMoving = true;
        // TriggerPythonStart();
        // StartCoroutine(AutoExplore());

    }

    void Update()
    {
        // シフトキーが押されたら動作を開始
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            isMoving = true;
            TriggerPythonStart();
            StartCoroutine(AutoExplore());
        }

        // アーム回転処理(前進中のみ有効)
        if (isArmsActive)
        {
            RotateArms();
        }
    }

    // ヘッドボビング開始とアーム回転初期化
    void StartHeadAndArms()
    {

        // // ヘッドボビング開始
        // transform.DOPunchPosition(new Vector3(0, amplitude, 0), cycle, 1, 0.5f)
        //          .SetLoops(-1, LoopType.Restart);
        // Tweenを変数に保持
            Tween punchTween = transform.DOPunchPosition(new Vector3(0, amplitude, 0), cycle, 1, 0.5f)
        .SetLoops(-1, LoopType.Restart);
    
        // 0.2秒目の状態へジャンプして、そのまま再生
        punchTween.Goto(gap, true);

        // アームの初期状態セット (方向・角度リセット)
        objectL.localEulerAngles = new Vector3(135, 0, 0);
        currentAngleL = 135f;
        directionL = -1;

        objectR.localEulerAngles = new Vector3(45, 0, 0);
        currentAngleR = 45f;
        directionR = 1;

        // アーム回転有効化
        isArmsActive = true;
    }

    // ヘッドボビングとアーム回転停止
    void StopHeadAndArms()
    {
        // ヘッドボビングTween停止
        DOTween.Kill(transform);
        // アームはDOTweenを使わず手動回転なのでKill不要だが、ここでフラグをfalseに
        isArmsActive = false;
    }

    void RotateArms()
    {
        // Lアーム回転処理
        float rotationStepL = rotationSpeed * Time.deltaTime * directionL;
        objectL.Rotate(rotationStepL, 0, 0);
        currentAngleL += rotationStepL;

        if (currentAngleL <= 45f)
        {
            currentAngleL = 45f;
            directionL = 1; // 向きを逆転
        }
        else if (currentAngleL >= 135f)
        {
            currentAngleL = 135f;
            directionL = -1; // 向きを逆転
        }

       // Rアーム回転処理
       float rotationStepR = rotationSpeed * Time.deltaTime * directionR;
       objectR.Rotate(rotationStepR, 0, 0);
       currentAngleR += rotationStepR;  // Mathf.Abs() を外して、回転方向に応じて角度をそのまま更新
       
       if (currentAngleR >= 135f){
        currentAngleR = 135f;
        directionR = -1; // 向きを逆転
        }
        else if (currentAngleR <= 45f)
        {
            currentAngleR = 45f;
            directionR = 1;  // 向きを逆転
            }
    }

    void TriggerPythonStart()
    {
        if (client != null && client.Connected && stream != null)
        {
            string message = "start";
            byte[] data = Encoding.ASCII.GetBytes(message);
            stream.Write(data, 0, data.Length);
            Debug.Log("Sent 'start' to Python");
        }
        else
        {
            Debug.LogWarning("Client or stream not available. Cannot send 'start' message.");
        }
    }

    IEnumerator AutoExplore()
    {
        // 1. 前進区間1：ヘッドボビング＆アーム回転開始
        StartHeadAndArms();
        switch_text.text = "Go";
        yield return MoveForward(forwardTime1);

        // 2. 回転区間1：停止して回転
        StopHeadAndArms();
        switch_text.text = "Stop";
        yield return RotateY(rotateTime1, rotationSpeedDegPerSec);

        // 3. 前進区間2：再度開始
        StartHeadAndArms();
        switch_text.text = "Go";
        yield return MoveForward(forwardTime2);

        // 4. 回転区間2：停止して逆回転
        StopHeadAndArms();
        switch_text.text = "Stop";
        yield return RotateY(rotateTime2, -rotationSpeedDegPerSec);

        // 5. 前進区間3：開始
        StartHeadAndArms();
        switch_text.text = "Go";
        yield return MoveForward(forwardTime3);

        // 6. 回転区間3：停止して回転（元方向へ）
        StopHeadAndArms();
        switch_text.text = "Stop";
        yield return RotateY(rotateTime3, -rotationSpeedDegPerSec);
        
        StartHeadAndArms();
        switch_text.text = "Go";
        yield return MoveForward(forwardTime4);

        // 6. 回転区間4：停止して回転（元方向へ）
        StopHeadAndArms();
        switch_text.text = "Stop";
        yield return RotateY(rotateTime4, rotationSpeedDegPerSec);
        
        StartHeadAndArms();
        switch_text.text = "Go";
        yield return MoveForward(forwardTime5);

        switch_text.text = "Finish";
        // 必要ならここで停止処理
        // isMoving = false;
    }

    IEnumerator MoveForward(float time)
    {
        float elapsed = 0f;
        while (elapsed < time)
        {
            parent.transform.Translate(-parent.transform.forward * speed * Time.deltaTime, Space.World);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator RotateY(float time, float degPerSec)
    {
        float elapsed = 0f;
        while (elapsed < time)
        {
            parent.transform.Rotate(Vector3.up * degPerSec * Time.deltaTime, Space.World);
            elapsed += Time.deltaTime;
            yield return null;
        }
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
