using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.IO;

public class Hilos_Clientes{

    TcpClient cliente;
    Boolean running;

    public Hilos_Clientes(){

    } 
    public Hilos_Clientes(TcpClient clienteS){
        cliente = clienteS;
        running = true;
    }

    public void Run(){
        try
        {
            while (running)
            {
                Clientes();
            }
        }
        catch (ThreadAbortException exception) 
		{
			Debug.Log ("Thread aborted");
		} 
		catch (SocketException exception) 
		{
			Debug.Log ("Socket exception");
		}
        finally
        {
            this.cliente.Close();
            //Debug.Log ("Socket client closed " + this.cliente.Client.RemoteEndPoint);
        }
    }

    void Clientes()
    {
        NetworkStream nwStream = cliente.GetStream();
        byte[] buffer = new byte[cliente.ReceiveBufferSize];
        int bytesRead = nwStream.Read(buffer, 0, cliente.ReceiveBufferSize);
        string dataReceived = Encoding.UTF8.GetString(buffer, 0, bytesRead);
        Debug.Log(dataReceived);

        if (dataReceived != null && dataReceived.Length != 0)
        {
            if (dataReceived == "stop")
            {
                running = false;
            }
            nwStream.Write(buffer, 0, bytesRead);
        }
        else if (dataReceived.Length == 0){
            Debug.Log("Parado por el cliente");
            running =false;
        }
        else{
            running = false;
        }
    }
}