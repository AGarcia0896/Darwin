using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class Conexion: MonoBehaviour
{

    public int connectionPort = 8888;
    public string connectionIP = "127.0.0.1";   
    IPAddress localAdd;
    TcpListener listener;

    private Conexion(){
        aceptarConexion();
    }

    private void aceptarConexion(){
        localAdd = IPAddress.Parse(connectionIP);
        listener = new TcpListener(IPAddress.Any, connectionPort);
        listener.Start();
        TcpClient client;

        while((client = listener.AcceptTcpClient()) != null){
            print("Hola");
        }
        listener.Stop();
    }
}
