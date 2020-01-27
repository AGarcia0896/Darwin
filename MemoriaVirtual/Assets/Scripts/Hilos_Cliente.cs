using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.IO;
using System.Globalization;

[System.Serializable]
// using System.Text.Json;
// using System.Text.Json.Serialization;

public class Frames
{
    public string frame;
}
public class Hilos_Clientes{

    TcpClient cliente;
    Boolean running;

    double profundidad;

    public Hilos_Clientes(){

    } 
    public Hilos_Clientes(TcpClient clienteS){
        cliente = clienteS;
        running = true;
        profundidad = 0;
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
			Debug.Log ("Thread aborted. " + exception.Message);
		} 
		catch (SocketException exception) 
		{
			Debug.Log ("Socket exception. " + exception.Message);
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

        if(profundidad == 0)
        {
            profundidad = double.Parse(dataReceived, CultureInfo.InvariantCulture.NumberFormat);
            Debug.Log(profundidad);
        }
        else
        {
            int size = int.Parse(dataReceived);
            byte[] data = new byte[size];
            int total = 0; // total bytes to received

            while (total < size) 
            {
                // receive bytes in byte array data[]
                // from position of total received and if the case data that havend been received.
                NetworkStream profundidadJson = cliente.GetStream();
                byte[] bufferAux = new byte[cliente.ReceiveBufferSize];
                int recv = profundidadJson.Read(bufferAux, 0, cliente.ReceiveBufferSize);
                if (recv == 0) // if received data = 0 than stop reseaving
                {
                    data = null;
                    break;
                }
                System.Buffer.BlockCopy(bufferAux, 0, data, total, recv);
                total += recv;  // total bytes read + bytes that are received
            }

            dataReceived = Encoding.UTF8.GetString(data, 0, size);
            Frames dato = JsonUtility.FromJson<Frames>(dataReceived);
            
            string separador = "";
            List<string> termsList = new List<string>();

            // Loop through array.
            for (int i = 0; i < dato.frame.Length; i++)
            {
                if(dato.frame[i].Equals(' '))
                {
                    termsList.Add(separador);
                    separador = "";
                }
                else
                {
                    separador = separador + dato.frame[i];
                }
            }
            termsList.Add(separador);
            string[] terms = termsList.ToArray();
            int[,] frame = new int[480, 640];
            for(int j = 0; j < 480; j++){
                for(int i = 0; i < 640; i++)
                {
                    string svalor = terms[i + (j * 640)];
                    int valor = System.Convert.ToInt32(svalor);
                    frame[j, i] = valor;
                }
            }
        }

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