using UnityEngine;

public class DualArmRotate : MonoBehaviour
{
    // 回転させるオブジェクトをInspectorでセットする
    public Transform objectL;
    public Transform objectR;

    // 回転速度
    public float rotationSpeed = 10f;

    // Lオブジェクト用の角度と方向
    private float currentAngleL = 180f;
    private int directionL = -1; // -1: 180度→0度へ向かう

    // Rオブジェクト用の角度と方向
    private float currentAngleR = 0f;
    private int directionR = 1;  // 1: 0度→180度へ向かう

    void Update()
    {
        // --- Lオブジェクトの回転処理 ---
        float rotationStepL = rotationSpeed * Time.deltaTime * directionL;
        objectL.Rotate(rotationStepL, 0, 0);
        currentAngleL += rotationStepL;

        if (currentAngleL <= 0f)
        {
            currentAngleL = 0f;
            directionL = 1; // 向きを逆転して0度→180度へ
        }
        else if (currentAngleL >= 180f)
        {
            currentAngleL = 180f;
            directionL = -1; // 向きを逆転して180度→0度へ
        }

        // --- Rオブジェクトの回転処理 ---
        float rotationStepR = rotationSpeed * Time.deltaTime * directionR;
        objectR.Rotate(rotationStepR, 0, 0);
        currentAngleR += Mathf.Abs(rotationStepR);

        if (currentAngleR >= 180f)
        {
            directionR *= -1; // 回転方向を逆転
            currentAngleR = 0f; // 角度をリセット
        }
    }
}
