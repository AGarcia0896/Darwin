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
    
    bool running;
    bool hijo;
    bool conectado;
    public int connectionPort;
    public string connectionIP;
    IPAddress localAdd;
    TcpListener listener;
    TcpClient client;
    Thread mThread;
    Thread process;
    

    private void Start()
    {
        running = true;
        hijo = false;
        conectado = false;
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
            client = listener.AcceptTcpClient();
            ThreadStart cl = new ThreadStart(comunicacion);
            process = new Thread(cl);
            process.Start();
        }
        //client = listener.AcceptTcpClient();
        //comunicacion();
    }

    void comunicacion(){
        conectado = true;
        hijo = true;
        while (running)
        {
            Clientes();
        }
        client.Close();
        conectado = client.Connected;
    }

    void Clientes()
    {
        NetworkStream nwStream = client.GetStream();
        byte[] buffer = new byte[client.ReceiveBufferSize];
        int bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize);
        string dataReceived = Encoding.UTF8.GetString(buffer, 0, bytesRead);
        Debug.Log(dataReceived);

        if (dataReceived != null)
        {
            if (dataReceived == "stop")
            {
                running = false;
            }
            //else
            //{
                nwStream.Write(buffer, 0, bytesRead);
            //}
        }
        else{
            running = false;
        }
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            client.Close();
            listener.Stop();
            Application.Quit();
        }
        if(hijo){
            conectado = client.Connected;
        }
        if(!conectado && hijo){
            process.Abort();
        }
    }
}