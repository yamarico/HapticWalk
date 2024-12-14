using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeRotate_L : MonoBehaviour
{
    // 回転速度を設定するための変数
    public float rotationSpeed = 10f;

    // 現在の回転角度を追跡する変数
    private float currentAngle = 180f; // 初期値を180度に設定

    // 回転方向を制御する変数
    private int direction = -1; // -1: 逆方向 (180度から0度)

    void Update()
    {
        // フレームごとの回転角度を計算
        float rotationStep = rotationSpeed * Time.deltaTime * direction;

        // 回転を適用
        transform.Rotate(rotationStep, 0, 0);

        // 現在の回転角度を更新
        currentAngle += rotationStep;

        // 180度から0度の範囲で回転を制限
        if (currentAngle <= 0f)
        {
            currentAngle = 0f; // 下限を0度に固定
            direction = 1; // 回転方向を正転に変更 (0度から180度へ戻る)
        }
        else if (currentAngle >= 180f)
        {
            currentAngle = 180f; // 上限を180度に固定
            direction = -1; // 回転方向を逆転 (180度から0度)
        }
    }
}
