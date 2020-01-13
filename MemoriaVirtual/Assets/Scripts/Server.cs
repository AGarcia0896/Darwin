using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class Server : MonoBehaviour
{
    bool running = true;
    int aux = 0;
    public int connectionPort = 8888;
    public string connectionIP = "127.0.0.1";
    public event Action mainThreadQueuedCallbacks;
    public event Action eventsClone;
    public GameObject SPHERE;
    public Vector3 pos = Vector3.zero;

    IPAddress localAdd;
    TcpListener listener;
    
    Thread mThread;
    

    private void Start()
    {
        ThreadStart ts = new ThreadStart(GetInfo);
        mThread = new Thread(ts);
        mThread.Start();
    }

    public static string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        throw new System.Exception("No network adapters with an IPv4 address in the system!");
    }

    void GetInfo()
    {
        
        localAdd = IPAddress.Parse(connectionIP);
        listener = new TcpListener(IPAddress.Any, connectionPort);
        listener.Start();

        while (running)
        {
            TcpClient client;
            client = listener.AcceptTcpClient();
            Connection(client);
        }
        listener.Stop();
    }

/*
    public static Vector3 StringToVector3(string sVector)
    {
        // Remove the parentheses
        if (sVector.StartsWith("(") && sVector.EndsWith(")"))
        {
            sVector = sVector.Substring(1, sVector.Length - 2);
        }

        // split the items
        string[] sArray = sVector.Split(',');

        // store as a Vector3
        Vector3 result = new Vector3(
            float.Parse(sArray[0]),
            float.Parse(sArray[1]),
            float.Parse(sArray[2]));

        return result;
    }*/

    void Connection(TcpClient client)
    {
        NetworkStream nwStream = client.GetStream();
        byte[] buffer = new byte[client.ReceiveBufferSize];
        int bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize);
        string dataReceived = Encoding.UTF8.GetString(buffer, 0, bytesRead);
        print(dataReceived);
        running = false;
        /*
        Vector3 distance = StringToVector3(dataReceived);

        if (dataReceived != null)
        {
            if (dataReceived == "stop")
            {
                running = false;
            }
            else
            {
                mainThreadQueuedCallbacks += () => {Instantiate(SPHERE, new Vector3(distance.x, distance.y, distance.z), Quaternion.identity); aux++;};
                print("moved"); 
                //print(dataReceived);
                nwStream.Write(buffer, 0, bytesRead);
            }
        }*/
        
    }

    void Update()
    {
        /*if (mainThreadQueuedCallbacks != null)
        {
            eventsClone = mainThreadQueuedCallbacks;
            mainThreadQueuedCallbacks = null;
            eventsClone.Invoke();
            eventsClone = null;
        }*/
    }
}
