/*****************************************************************************
* ROBOT CONTROL
* DESC: The robot control system is designed to interact with the UDP server
* on the robot. 
* Author: Jonathan L Clark
* Date: 12/9/2019
*****************************************************************************/
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Net;
using System.Net.Sockets;
using UnityStandardAssets.CrossPlatformInput;

public class RobotControl : MonoBehaviour
{
    private string ipAddress = "192.168.1.14";
    public UnityEngine.UI.Text messageText;
    public UnityEngine.UI.InputField ipAddressBox;

    private float nextHeartbeat = 0.0f;

    private Socket gameSocket;
    private byte[] udpData = { 81, 12, 0, 0, 0, 0, 0, 0, 0, 0 };
    private bool lightOn = false;


    /// <summary>
    /// Callback format for get, post requests
    /// </summary>
    /// <param name="error"></param>
    /// <param name="data"></param>
    public delegate void HTTPRequestCallback(string inData, bool error);

    // Start is called before the first frame update
    void Start()
    {
        ipAddressBox.text = PlayerPrefs.GetString("RobotIP", ipAddress);
        gameSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    }

    // Update is called once per frame
    void Update()
    {
        if (nextHeartbeat < Time.time)
        {
            //float yValue = CrossPlatformInputManager.GetAxis("Vertical") * 100.0f;
            //float xValue = CrossPlatformInputManager.GetAxis("Horizontal") * 100.0f;
            float yValue = GetComponent<InstantJoystick>().vertical_left * 100.0f;
            float xValue = GetComponent<InstantJoystick>().horizontal_left * 100.0f;
            float slide = GetComponent<InstantJoystick>().horizontal_right * 100.0f;
            byte yByte = (byte)Mathf.Abs(yValue);
            byte yDir = 0;
            byte xByte = (byte)Mathf.Abs(xValue);
            byte xDir = 0;
            byte slideByte = (byte)Mathf.Abs(slide);
            byte slideDir = 0;
            if (yValue < 0)
            {
                yDir = 0x01;
            }
            if (xValue < 0)
            {
                xDir = 0x01;
            }
            if (slide < 0)
            {
                slideDir = 0x01;
            }
            udpData[2] = xByte;
            udpData[3] = xDir;
            udpData[4] = yByte;
            udpData[5] = yDir;
            udpData[6] = slideByte;
            udpData[7] = slideDir;
            SendData(gameSocket, udpData, ipAddress, 6789);
            nextHeartbeat = Time.time + 0.2f;
        }
    }

    public void SetUnsetLight()
    {
        lightOn = !lightOn;
        if (lightOn)
        {
            udpData[8] = 1;
        }
        else
        {
            udpData[8] = 0;
        }
    }

    public void CloseApp()
    {
        Application.Quit();
    }

    /// <summary>
    /// Sends UDP data to an endpoint on the network
    /// </summary>
    /// <param name="gameSocket"></param>
    /// <param name="sendBuffer"></param>
    /// <param name="targetIp"></param>
    /// <param name="inUdpPort"></param>
    public static void SendData(Socket gameSocket, byte[] sendBuffer, string targetIp, int inUdpPort)
    {
        IPAddress serverAddr = IPAddress.Parse(targetIp);
        IPEndPoint endPoint = new IPEndPoint(serverAddr, inUdpPort);
        gameSocket.SendTo(sendBuffer, endPoint);
    }

    public void OnIPChange()
    {
        if (IPAddress.TryParse(ipAddressBox.text, out IPAddress addr) && ipAddressBox.text.Split('.').Length == 4)
        {
            ipAddress = ipAddressBox.text;
            PlayerPrefs.SetString("RobotIP", ipAddress);
            PlayerPrefs.Save();
        }
    }

 
    /// <summary>
    /// Pulls all data from the system
    /// </summary>
    private void RequestAllData()
    {
        //string URL = string.Format("http://{0}:{1}/data.js", PlayerPrefs.GetString("SystemsServer_IP", "127.0.0.1"), PlayerPrefs.GetString("SystemsServer_PORT", "5000"));
        //StartCoroutine(GetRequest(URL, DataCallback));
    }

    /// <summary>
    /// Sends out an async get request using the Unity networking module
    /// </summary>
    /// <param name="uri"></param>
    /// <param name="getCallback"></param>
    /// <returns></returns>
    IEnumerator GetRequest(string uri, HTTPRequestCallback getCallback)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            // string[] pages = uri.Split('/');
            //int page = pages.Length - 1;
            if (webRequest.isNetworkError)
            {
                getCallback(webRequest.downloadHandler.text, true); // ALWAYS call the callback!
            }
            else
            {
                getCallback(webRequest.downloadHandler.text, false); // ALWAYS call the callback!
            }
        }
    }

    /// <summary>
    /// Posts data to the HTTP server
    /// </summary>
    /// <param name="url"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    IEnumerator PostData(string url, string data, HTTPRequestCallback callback)
    {
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(data);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();
        messageText.text = request.error;
        if (callback != null)
        {
            callback(request.downloadHandler.text, false);
        }
    }
}
