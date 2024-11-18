using UnityEngine;
using DG.Tweening;

public class ArmSwingMovement : MonoBehaviour
{
    [Header("移動設定")]
    public float initialSpeed = 5.0f; // 腕を振ったときに与える初期速度
    public float decelerationRate = 2.0f; // 減速率
    public float swingThreshold = 1.0f; // 腕振り判定のための速度しきい値

    [Header("ヘッドボビング設定")]
    public float amplitude = 0.1f; // ヘッドボビングの振幅
    public float frequency = 1.0f; // ヘッドボビングの周波数

    [Header("腕振り検出設定")]
    public float swingCooldown = 0.5f; // 同じ手で再度腕振りを検出するまでの待機時間

    private bool leftSwingDetected = false;
    private bool rightSwingDetected = false;

    private float leftSwingCooldownTimer = 0.0f;
    private float rightSwingCooldownTimer = 0.0f;

    private Vector3 currentVelocity = Vector3.zero; // 現在のプレイヤーの速度

    // ヘッドボビングを適用するTransform（カメラの親オブジェクト）
    public Transform headTransform; // インスペクターで設定

    private float headBobTimer = 0.0f;
    private Vector3 originalHeadLocalPosition;

    void Start()
    {
        if (headTransform == null)
        {
            Debug.LogError("Head Transformが設定されていません。インスペクターで設定してください。");
        }
        else
        {
            // ヘッドボビングの基準位置を保存
            originalHeadLocalPosition = headTransform.localPosition;
        }
    }

    void Update()
    {
        DetectArmSwing();
        ApplyMovement();
        UpdateSwingCooldowns();
        ApplyHeadBobbing();
    }

    void DetectArmSwing()
    {
        // グリップボタンが押されているか確認
        bool isLeftGripPressed = OVRInput.Get(OVRInput.Button.PrimaryHandTrigger);
        bool isRightGripPressed = OVRInput.Get(OVRInput.Button.SecondaryHandTrigger);

        // 左コントローラーの速度を取得
        Vector3 leftControllerVelocity = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.LTouch);
        // 右コントローラーの速度を取得
        Vector3 rightControllerVelocity = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch);

        // 左手の腕振り判定（移動とヘッドボビング用）
        if (isLeftGripPressed && !leftSwingDetected && leftControllerVelocity.magnitude > swingThreshold && leftSwingCooldownTimer <= 0)
        {
            AddSpeed(OVRInput.Controller.LTouch);
            leftSwingDetected = true;
            leftSwingCooldownTimer = swingCooldown; // クールダウンを開始
            rightSwingDetected = false; // 右手の検出をリセット

            StartHeadBobbing(); // ヘッドボビングを開始
        }

        // 右手の腕振り判定（移動とヘッドボビング用）
        if (isRightGripPressed && !rightSwingDetected && rightControllerVelocity.magnitude > swingThreshold && rightSwingCooldownTimer <= 0)
        {
            AddSpeed(OVRInput.Controller.RTouch);
            rightSwingDetected = true;
            rightSwingCooldownTimer = swingCooldown; // クールダウンを開始
            leftSwingDetected = false; // 左手の検出をリセット

            StartHeadBobbing(); // ヘッドボビングを開始
        }
    }

    void UpdateSwingCooldowns()
    {
        if (leftSwingCooldownTimer > 0)
        {
            leftSwingCooldownTimer -= Time.deltaTime;
        }
        if (rightSwingCooldownTimer > 0)
        {
            rightSwingCooldownTimer -= Time.deltaTime;
        }
    }

    void AddSpeed(OVRInput.Controller hand)
    {
        // コントローラーの回転を取得
        Quaternion handRotation = OVRInput.GetLocalControllerRotation(hand);

        // コントローラーの前方ベクトルを取得（Y軸の回転のみを考慮）
        Vector3 forward = handRotation * Vector3.forward;
        forward.y = 0; // 水平移動のみ
        forward = forward.normalized;

        // 初期速度を加算
        currentVelocity += forward * initialSpeed;
    }

    void ApplyMovement()
    {
        if (currentVelocity.magnitude > 0.01f)
        {
            // プレイヤーを移動
            transform.position += currentVelocity * Time.deltaTime;

            // 減速処理
            float deceleration = decelerationRate * Time.deltaTime;
            currentVelocity = Vector3.MoveTowards(currentVelocity, Vector3.zero, deceleration);
        }
        else
        {
            currentVelocity = Vector3.zero;
        }
    }

    void StartHeadBobbing()
    {
        // ヘッドボビングのタイマーをリセット
        headBobTimer = 0.0f;
    }

    void ApplyHeadBobbing()
    {
        if (currentVelocity.magnitude > 0.01f && headTransform != null)
        {
            // ヘッドボビングのタイマーを進める
            headBobTimer += Time.deltaTime * frequency;

            // Sin関数を使って上下運動を計算
            float bobOffset = Mathf.Sin(headBobTimer * Mathf.PI * 2) * amplitude;

            // 頭の位置を更新
            Vector3 newPosition = originalHeadLocalPosition;
            newPosition.y += bobOffset;
            headTransform.localPosition = newPosition;
        }
        else if (headTransform != null)
        {
            // 動いていないときは頭の位置を元に戻す
            headTransform.localPosition = originalHeadLocalPosition;
        }
    }
}
