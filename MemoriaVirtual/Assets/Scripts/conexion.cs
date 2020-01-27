using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class conexion : MonoBehaviour
{
    public int connectionPort;
    public string connectionIP;
    IPAddress localAdd;
    TcpListener listener;
    Thread mThread;    

    private void Start()
    {
        connectionIP = GetLocalIPAddress();
        connectionPort = 8888;
        ThreadStart ts = new ThreadStart(conexionServer);
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

    void conexionServer()
    {
        localAdd = IPAddress.Parse(connectionIP);
        listener = new TcpListener(IPAddress.Any, connectionPort);
        listener.Start();
        while(true){
            listener.BeginAcceptTcpClient(new AsyncCallback(AcceptCallBack), this.listener);
            System.Threading.Thread.Sleep(5000);
        }
    }

    protected void AcceptCallBack(IAsyncResult ar){
        int ThreadId = Thread.CurrentThread.ManagedThreadId;
        TcpListener listener = (TcpListener)ar.AsyncState;
        TcpClient client = listener.EndAcceptTcpClient(ar);
        Hilos_Clientes hc = new Hilos_Clientes(client);
        Thread clientThread = new Thread(new ThreadStart(hc.Run));
        clientThread.Start();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            listener.Stop();
            Application.Quit();
        }
    }
}