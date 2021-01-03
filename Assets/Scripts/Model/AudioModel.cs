using CSCore;
using CSCore.SoundIn;
using CSCore.Streams;
using CSCore.SoundOut;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CSCore.Codecs.WAV;
using System;
using CSCore.CoreAudioAPI;
using Assets.Scripts.Model.Additional;
using System.ComponentModel;
using System.IO;
using System.Net.Sockets;
using System.Net;

public class AudioModel : MonoBehaviour
{
    private static AudioModel instance = null;

    bool notified = false;
    WasapiCapture capture;
    //IWaveSource finalSource;
    WriteableBufferingSource src;
    List<ISoundOut> _soundOutList;
    public int bufferLength = 2048;
    float[] buffer;
    Queue<float[]> buffersToSend = new Queue<float[]>();
    int sampleNumberForBuffer;
    int sampleCount;
    System.Diagnostics.Stopwatch clock;
    System.Random randomGenerator = new System.Random();
    TCPStream tcps;
    byte[] randomsTest;
    int sentBuffersCount;


    // Start is called before the first frame update
    void Start()
    {
        WaveFormat wf = new CSCore.WaveFormat(48000, 32, 2, AudioEncoding.Extensible);

        capture = new WasapiLoopbackCapture();

        capture.Initialize();
        
        capture.DataAvailable += Capture_DataAvailable;
        capture.Start();
        tcps = new TCPStream("Win", TransferProtocol.ConnectionType.ReceiveAndSend, "192.168.0.3", 65535, "192.168.0.3", 65535);

        Debug.Log(capture.WaveFormat.ToString());
        
        Debug.Log(wf.ToString());
        Debug.Log("Equals: " + wf.Equals(capture.WaveFormat));

        //src = new WriteableBufferingSource(new CSCore.WaveFormat(48000, 32, 2, AudioEncoding.Extensible)) { FillWithZeros = true };
        src = new WriteableBufferingSource(capture.WaveFormat) { FillWithZeros = true };
        var mmDeviceCollection = (new MMDeviceEnumerator()).EnumAudioEndpoints(DataFlow.Render, DeviceState.Active);
        _soundOutList = new List<ISoundOut>();
        foreach (var device in mmDeviceCollection)
        {
            _soundOutList.Add(new WasapiOut() { Latency = 10, Device = device });
        }
        foreach (var so in _soundOutList)
        {
            
            so.Initialize(src);
            so.Volume = 0.1f;
            so.Play();
        }

        randomsTest = new byte[38400];
        randomGenerator.NextBytes(randomsTest);

        /*
        Debug.Log("Sending 1");
        tcps.send(randomsTest, TransferProtocol.DataType.LRChannelBuffer);

        Debug.Log("Sending 2");
        tcps.send(randomsTest, TransferProtocol.DataType.LRChannelBuffer);
        Debug.Log("Sent two buffers");*/
        sentBuffersCount = 0;
        //StartCoroutine(testSending());

        StartCoroutine(ReceiveBufferCoroutine());
        
    }

    IEnumerator testSending() {
        while (true)
        {
            /*
            if (randomsTest == null) {
                Debug.Log("TestSending randomsTest == null");
            }
            else{
                Debug.Log("TestSending randomsTest != null");
            }*/
            //Debug.Log("TestSending")
            //Debug.Log("Before in testSending ranTestLen: " + randomsTest.Length);
            tcps.send(randomsTest, TransferProtocol.DataType.LRChannelBuffer);
            //sentBuffersCount++;
            //Debug.Log("Sent from testSending, bufferCount: " + sentBuffersCount);
            yield return null ;
        }
    }

    IEnumerator ReceiveBufferCoroutine()
    {
        while (true)//Should be true after debugging
        {
            Tuple<byte[], int, TransferProtocol.DataType> dataWithHeader = tcps.receive();
            Debug.Log("RecLen: " + dataWithHeader.Item2);
            src.Write(dataWithHeader.Item1, 0, dataWithHeader.Item2);
            yield return null;
        }
    }

    //https://stackoverflow.com/questions/62204099/c-sharp-live-streaming-audio-over-socket-connection
    public static AudioModel Instance
    {
        get
        {
            if (instance == null)
            {
                //instance = new AudioModel();
                instance = AppModel.Instance.gameObject.AddComponent<AudioModel>();

            }
            return instance;
        }

    }

    private void Capture_DataAvailable(object sender, DataAvailableEventArgs e)
    {
        Debug.Log("Sending Len:" + e.Data.Length);
        tcps.send(e.Data, TransferProtocol.DataType.LRChannelBuffer);
    }

    private void playReceived(byte[] data)
    {

    }

    private void NotificationSource_SingleBlockRead(object sender, SingleBlockReadEventArgs e)
    {
        Debug.Log("senderNot" + sender);
        //Probably actions for readed data from source
        //Debug.Log("Left: " + e.Left + " \t\tRight: " + e.Right);
        Debug.Log("Notif");
        if (!clock.IsRunning)
        {
            clock.Start();
            sampleCount = 0;
        }
        buffer[sampleNumberForBuffer] = e.Left;
        buffer[sampleNumberForBuffer + 1] = e.Right;
        sampleNumberForBuffer += 2;
        sampleNumberForBuffer = (sampleNumberForBuffer + 2) % bufferLength;
        sampleCount++;

        /*
        if (sampleNumberForBuffer == 0) {
            buffersToSend.Enqueue(buffer);
            buffer = new float[bufferLength];
        }
        */

    }

    

    public float getAverageSampleRateForSecond()
    {
        float sampleRate = (float)sampleCount / (clock.ElapsedMilliseconds) * 1000;
        clock.Stop();
        return sampleRate;
    }

    private void notifyObservers(byte[] data, TransferProtocol.DataType dataType)
    {
        if (1 == 0 && notified == false)
        {
            string bytes = "";
            foreach (byte b in data)
            {
                bytes += b + " ";
            }
            Debug.Log(bytes + " " + dataType.ToString() + "SENDED?: " + NetworkModel.Instance.Send(data, dataType));
            //notified = true;
        }
    }

    public void Cleanup()
    {
        if (_soundOutList != null)
        {
            foreach (var so in _soundOutList)
            {
                so.Stop();
                so.Dispose();
            }
        }
        if (capture != null)
        {
            capture.Stop();
            capture.Dispose();
        }
    }

    void OnApplicationQuit()
    {
        if (tcps != null)
        {
            tcps.Clean();
        }
        if (enabled)
        {
            Cleanup();
        }
        
    }



}