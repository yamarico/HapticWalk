using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

public class PlaySound : MonoBehaviour
{
    private static readonly HttpClient client = new HttpClient();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            SendPlayRequest("left");
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            SendPlayRequest("right");
        }
    }

    async void SendPlayRequest(string channel)
    {
        var content = new StringContent("{\"channel\":\"" + channel + "\"}", Encoding.UTF8, "application/json");
        var response = await client.PostAsync("http://150.65.60.20:5000/play", content);
        if (response.IsSuccessStatusCode)
        {
            Debug.Log("Playing sound on " + channel + " channel.");
        }
        else
        {
            Debug.LogError("Error playing sound: " + response.ReasonPhrase);
        }
    }
}
