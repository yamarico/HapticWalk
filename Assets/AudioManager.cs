using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource leftAudioSource;
    //public AudioSource rightAudioSource;
    public float interval = 0.5f;

    private void Start()
    {
        StartCoroutine(PlayAlternatingAudio());
    }

    private IEnumerator PlayAlternatingAudio()
    {
        while (true)
        {
            // 左のオーディオを再生して右を停止
            leftAudioSource.Play();
            //rightAudioSource.Stop();
            yield return new WaitForSeconds(interval);

            // 右のオーディオを再生して左を停止
           // rightAudioSource.Play();
            leftAudioSource.Stop();
            yield return new WaitForSeconds(interval);
        }
    }
}
