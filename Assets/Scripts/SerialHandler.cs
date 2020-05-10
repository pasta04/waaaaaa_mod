using UnityEngine;
using System;
using System.Collections;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;

public class SerialHandler : MonoBehaviour
{
    public delegate void SerialDataReceivedEventHandler(string message);
    public event SerialDataReceivedEventHandler OnDataReceived;

    //ポート名
    //例
    //Linuxでは/dev/ttyUSB0
    //windowsではCOM1
    //Macでは/dev/tty.usbmodem1421など
    public string portName = "COM1";
    private string beforePortName;
    public int baudRate = 9600;

    private SerialPort serialPort_;
    private Thread thread_;
    private bool isRunning_ = false;

    private string message_;
    private bool isNewMessageReceived_ = false;

    void Awake()
    {
        beforePortName = portName;
        Open();
    }

    void Update()
    {
        if (isNewMessageReceived_)
        {
            OnDataReceived(message_);
        }
        isNewMessageReceived_ = false;

        // Debug.Log(portName);
        if (portName != beforePortName)
        {
            Debug.Log(portName);
            beforePortName = portName;
            Reconnect();
        }
    }

    async void Reconnect()
    {
        Close();
        await Task.Delay(2000);
        Open();
    }

    void OnDestroy()
    {
        Close();
    }

    private void Open()
    {
        Debug.Log("[SerialHandler] Open start");
        try
        {
            serialPort_ = new SerialPort(portName, baudRate, Parity.None, 8, StopBits.One);
            //または
            //serialPort_ = new SerialPort(portName, baudRate);
            serialPort_.Open();
            Debug.Log("Open: " + portName);

            isRunning_ = true;

            Debug.Log("thread_↓");
            Debug.Log(thread_);
            if (thread_ == null)
            {
                thread_ = new Thread(Read);
                thread_.Start();
            }

        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        Debug.Log("[SerialHandler] Open end");
    }

    private void Close()
    {
        Debug.Log("[SerialHandler] Close start");
        isNewMessageReceived_ = false;
        isRunning_ = false;

        Debug.Log(thread_);
        if (thread_ != null)
        {
            // thread_.Join();
            thread_.Abort();
            thread_ = null;
        }

        if (serialPort_ != null && serialPort_.IsOpen)
        {
            serialPort_.Close();
            serialPort_.Dispose();
        }
        serialPort_ = null;
        Debug.Log("[SerialHandler] Close end");
    }

    private void Read()
    {
        while (isRunning_ && serialPort_ != null && serialPort_.IsOpen)
        {
            try
            {
                message_ = serialPort_.ReadLine();
                isNewMessageReceived_ = true;
            }
            catch (System.Exception e)
            {
                Debug.LogWarning(e.Message);
            }
        }
        Debug.Log("[SerialHandler] Read end");
    }

    public void Write(string message)
    {
        try
        {
            serialPort_.Write(message);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning(e.Message);
        }
    }
}