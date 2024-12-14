using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeRotate_R : MonoBehaviour
{
    // 回転速度を設定するための変数
    public float rotationSpeed = 10f;

    // 現在の回転角度を追跡する変数
    private float currentAngle = 0f;

    // 回転方向を制御する変数
    private int direction = 1; // 1: 正方向, -1: 逆方向

    void Update()
    {
        // フレームごとの回転角度を計算
        float rotationStep = rotationSpeed * Time.deltaTime * direction;

        // 回転を適用
        transform.Rotate(rotationStep, 0, 0);

        // 現在の回転角度を更新
        currentAngle += Mathf.Abs(rotationStep);

        // 180度回転したら方向を逆にする
        if (currentAngle >= 180f)
        {
            direction *= -1;
            currentAngle = 0f; // 角度をリセット
        }
    }
}
