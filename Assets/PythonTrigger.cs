using UnityEngine;
using System.Net.Sockets;
using System.Text;

public class PythonTrigger : MonoBehaviour
{
    private TcpClient client;
    private NetworkStream stream;

    void Start()
    {
        // Connect to the Python server
        client = new TcpClient("127.0.0.1", 65432);
        stream = client.GetStream();
    }

    void OnApplicationQuit()
    {
        // Send 'exit' message when application quits to stop Python server
        if (client.Connected)
        {
            byte[] exitData = Encoding.ASCII.GetBytes("exit");
            stream.Write(exitData, 0, exitData.Length);
            stream.Close();
            client.Close();
        }
    }

    void Update()
    {
        // シフトキーが押されたらPythonの関数を実行
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            TriggerPythonStart();
        }

        // Escキーが押されたらPythonの実行を停止
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TriggerPythonStop();
        }
    }

    public void TriggerPythonStart()
    {
        if (client.Connected)
        {
            byte[] data = Encoding.ASCII.GetBytes("start");
            stream.Write(data, 0, data.Length);
            Debug.Log("Sent 'start' to Python");
        }
    }

    public void TriggerPythonStop()
    {
        if (client.Connected)
        {
            byte[] stopData = Encoding.ASCII.GetBytes("stop");
            stream.Write(stopData, 0, stopData.Length);
            Debug.Log("Sent 'stop' to Python");
        }
    }
}
