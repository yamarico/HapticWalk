using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.EventSystems;
public class PlayerController : MonoBehaviour
{
    // public float normalSpeed = 3.0f; // 通常の移動速度
    // public float sprintSpeed = 6.0f; // 走るときの移動速度

    private float currentSpeed =3.0f; // 現在の移動速度

    public SerialHandler serialHandler;
    [SerializeField] AudioSource audioSource;
   [SerializeField] TextMeshProUGUI textMeshProField;
    public string number ="2";
    
    bool isSending = false;
    bool isMovingForward = false;

    void Update()
    {
        // 移動速度を設定
        if (OVRInput.Get(OVRInput.RawButton.RHandTrigger)) // 右の薬指トリガー
        {
            //currentSpeed = sprintSpeed;
            isMovingForward = true;
            serialHandler.Write(number); // Arduinoに1を送信
            PlayAudioLoop();
             StartCoroutine(ShowTextAfterDelay(30f));
            if (!isSending)
            {
                StartCoroutine(SendSerialData());
            }
        }
        // else
        // {
        //     serialHandler.Write("0"); // Arduinoに0を送信
        //     // currentSpeed = normalSpeed;
        //     if (isSending)
        //     {
        //         StopCoroutine(SendSerialData());
        //         isSending = false;
        //     }
        // }
        Debug.Log(isMovingForward);
 if (isMovingForward == true)
        {
            
            MoveForward();
            
        }
        // // 右人差し指トリガーを押したら前進フラグをセット
        // if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger))
        // {
        //     isMovingForward = true;
        //     Debug.Log("aa");
        // }
        //           isMovingForward = true;

        // 前進フラグが立っている間、前進

        
    }    IEnumerator SendSerialData()
    {
        isSending = true;
        serialHandler.Write(number); // Arduinoに1を送信
        Debug.Log("送信");
        yield return new WaitForSeconds(0.5f);
    }

    void MoveForward()
    {
        // プレイヤーが向いている方向に進む
        Vector3 forwardDirection = transform.forward;
        transform.position -= forwardDirection * currentSpeed * Time.deltaTime;
        Debug.Log("信仰");
    }
    void PlayAudioLoop()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.loop = true;
            audioSource.Play();
        }
    }
    private IEnumerator ShowTextAfterDelay(float delay)
    {
        // 指定された時間待つ
        yield return new WaitForSeconds(delay);

        // TextMeshProUGUIをアクティベートする
        if (textMeshProField != null)
        {
            textMeshProField.gameObject.SetActive(true);
            textMeshProField.text = "Fin";
        }
        else
        {
            Debug.LogError("TextMeshProUGUIフィールドが設定されていません！");
        }
    }
}
