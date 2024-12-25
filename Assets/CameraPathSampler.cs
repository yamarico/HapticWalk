using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CameraPathSampler : MonoBehaviour
{
    [SerializeField] Transform cameraTransform;  // サンプリング対象のカメラ
    [SerializeField] Transform RHandTransform;  // サンプリング対象の右腕
    [SerializeField] Transform LHandTransform;  // サンプリング対象の左腕
    [SerializeField] float duration = 5.0f;      // サンプリングする全体の時間
    [SerializeField] float interval = 0.1f;     // サンプリング間隔（秒）

    private List<Vector4> sampledData = new List<Vector4>(); // 時間、カメラ、右腕、左腕のデータを保持

    void Start()
    {
        // StartCoroutine(SampleCameraPath());
    }
    float time = 0f;
    float logTime = 0f;
    void FixedUpdate()
    {
        time += Time.deltaTime;
        logTime += Time.deltaTime;
        if(time < duration){
            if(logTime > interval){
                // 記録
                sampledData.Add(new Vector4(time, cameraTransform.position.y, RHandTransform.rotation.x, LHandTransform.rotation.x));
                logTime = 0;
            }
        }
    }

    IEnumerator SampleCameraPath()
    {
        float elapsedTime = 0f;

        while (elapsedTime <= duration)
        {
            // 時間、カメラのY座標、右腕のY座標、左腕のY座標を記録
            sampledData.Add(new Vector4(elapsedTime, cameraTransform.position.y, RHandTransform.rotation.x, LHandTransform.rotation.x));

            elapsedTime += interval;
            yield return new WaitForSeconds(interval);
        }

        // データをCSVファイルに保存
        SaveToCSV();
    }

    void SaveToCSV()
    {
        string path = Path.Combine(Application.dataPath, "CameraAndArmsPathData_v5.csv");
        using (StreamWriter writer = new StreamWriter(path))
        {
            // ヘッダー行にカメラと両腕のデータを追加
            writer.WriteLine("Time,CameraY,RHandY,LHandY");
            
            foreach (var data in sampledData)
            {
                // 各データポイントをCSVに書き込む
                writer.WriteLine($"{data.x},{data.y},{data.z},{data.w}");
            }
        }

        Debug.Log($"Data saved to {path}");
    }

    private void OnApplicationQuit(){
        SaveToCSV();
    }
}
