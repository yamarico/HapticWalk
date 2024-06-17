using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float normalSpeed = 3.0f; // 通常の移動速度
    public float sprintSpeed = 6.0f; // 走るときの移動速度
    public float rotationSpeed = 100.0f; // プレイヤーの回転速度

    private float currentSpeed; // 現在の移動速度

    // Update is called once per frame
    void Update()
    {
        // 移動速度を設定
        if (OVRInput.Get(OVRInput.RawButton.RHandTrigger)) // 右の薬指トリガー
        {
            currentSpeed = sprintSpeed;
        }
        else
        {
            currentSpeed = normalSpeed;
        }

        // 右人差し指トリガーを押している間、前進
        if (OVRInput.Get(OVRInput.RawButton.RIndexTrigger))
        {
            MoveForward();
        }

        RotatePlayer();
    }

    void MoveForward()
    {
        // プレイヤーが向いている方向に進む
        Vector3 forwardDirection = transform.forward;
        transform.position -= forwardDirection * currentSpeed * Time.deltaTime;
    }

    void RotatePlayer()
    {
        // 右スティックの入力を取得
        Vector2 rightStickInput = OVRInput.Get(OVRInput.RawAxis2D.RThumbstick);

        // 左右の入力に応じて回転
        float rotation = rightStickInput.x * rotationSpeed * Time.deltaTime;
        transform.Rotate(0, rotation, 0);
    }
}
