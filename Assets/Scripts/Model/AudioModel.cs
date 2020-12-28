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
    IWaveSource finalSource;
    List<ISoundOut> _soundOutList;
    public int bufferLength = 2048;
    float[] buffer;
    Queue<float[]> buffersToSend = new Queue<float[]>();
    int sampleNumberForBuffer;
    int sampleCount;
    System.Diagnostics.Stopwatch clock;
    System.Random randomGenerator = new System.Random();

    TcpClient tcpWrite;
    TcpListener tcpServerRead;
    TcpClient tcpRead;
    BinaryWriter writer;
    BinaryReader reader;
    
    

    // Start is called before the first frame update
    void Start()
    {
        tcpServerRead = new TcpListener(IPAddress.Parse("192.168.0.3"), 65501);
        tcpServerRead.Start();

        tcpWrite = new TcpClient();
        tcpWrite.Client = new Socket(AddressFamily.InterNetwork,
            SocketType.Stream,
            ProtocolType.Tcp);
        tcpWrite.Client.Connect(IPAddress.Parse("192.168.0.3"), 65501);

        tcpRead = tcpServerRead.AcceptTcpClient();


        tcpWrite.GetStream().Write(new byte[] { 1, 2, 9 }, 0, 3);

        byte[] rec = new byte[3];
        tcpRead.GetStream().Read(rec, 0, 3);

        string recString = "";
        foreach (byte b in rec) {
            recString += b + " ";
        }

        Debug.Log("Rec:" + recString);

        tcpRead.Close();
        tcpServerRead.Stop();
        tcpWrite.Close();

        /*
        s = new Socket(AddressFamily.InterNetwork,
            SocketType.Stream,
            ProtocolType.Tcp);
        s.Connect(IPAddress.Parse("192.168.0.3"), 65535);
        tcpRead = (TcpClient)s;
        */

        writer = new BinaryWriter(tcpWrite.GetStream());
        reader = new BinaryReader(tcpRead.GetStream());


        //o.updateObserverErgoSendData(new byte[] { 1, 3, 5, 7 });
        buffer = new float[bufferLength];
        //observers = new List<ObserverSender>();
        Debug.Log("WasapiCapture.IsSupportedOnCurrentPlatform: " + WasapiCapture.IsSupportedOnCurrentPlatform);
        capture = new WasapiLoopbackCapture();
        capture.Initialize();
        //HERE:
        capture.DataAvailable += (s, a) => writeToStream(s, a);
        capture.Start();

        //IWaveSource source = new SoundInSource(capture);
        //SingleBlockNotificationStream notificationSource = new SingleBlockNotificationStream(source.ToSampleSource());

        
        //IWaveSource wave8 = notificationSource.ToWaveSource(8);
        //Debug.Log(wave8.WaveFormat.BytesPerSecond);
        //wave8.WriteToWaveStream
        
        //notificationSource.SingleBlockRead += NotificationSource_SingleBlockRead;
        clock = new System.Diagnostics.Stopwatch();

        //finalSource = notificationSource.ToWaveSource();
        //finalSourceReceived = notificationSourceReceived.ToWaveSource()

        //capture.DataAvailable += Capture_DataAvailable;
        /*
        var mmdeviceEnumerator = new MMDeviceEnumerator();
        var mmdeviceCollection = mmdeviceEnumerator.EnumAudioEndpoints(DataFlow.Render, DeviceState.Active);
        _soundOutList = new List<ISoundOut>();
        foreach (var device in mmdeviceCollection)
        {
            _soundOutList.Add(new WasapiOut() { Latency = 2000, Device = device });
        }
        foreach (var so in _soundOutList)
        {
            so.Initialize(finalSource);
            so.Volume = 0.8f;
            so.Play();
        }
        Debug.Log("Devices Count: " + mmdeviceCollection.GetCount());
        
        StartCoroutine(SendBufferCoroutine());
        */
        
        WriteableBufferingSource src = new WriteableBufferingSource(new CSCore.WaveFormat(48000, 32, 2)) { FillWithZeros = true };

        WasapiOut soundOut = new WasapiOut();
        soundOut.Initialize(src);
        soundOut.Play();
        int size = 3840;

        while (true) {
            byte[] bytes = reader.ReadBytes(size);
            src.Write(bytes, 0, size);
        }
    }

    private void writeToStream(object sender, DataAvailableEventArgs a) {
        writer.Write(a.Data, a.Offset, a.ByteCount);
    }

    private void Capture_DataAvailable1(object sender, DataAvailableEventArgs e)
    {
        throw new NotImplementedException();
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
        Debug.Log("senderCap" + sender);
        //Debug.Log("cap" + e.Data + " " + e.Offset + " " + e.ByteCount);
        //finalSource.Read(e.Data, e.Offset, e.ByteCount);
        byte[] data = new byte[38400];
        randomGenerator.NextBytes(data);
        Debug.Log("data[0]: " + data[0]);
        finalSource.Read(data, 0, data.Length);
    }

    private void playReceived(byte[] data) {

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
        //sampleCount++;

        /*
        if (sampleNumberForBuffer == 0) {
            buffersToSend.Enqueue(buffer);
            buffer = new float[bufferLength];
        }
        */
        
    }

    IEnumerator SendBufferCoroutine() {
        int i = 0;
        byte[] bufferToSend = new byte[bufferLength * 4];
        //byte[] bytesFromFloat;
        //float[] additionalBufferFloat;
        while (true)
        {
            if (buffersToSend.Count > 0) {
                //additionalBufferFloat = buffersToSend.Dequeue();
                //for (i =0; i< bufferLength; i++) {
                //   bufferToSend[i * 4]) = BitConverter.GetBytes(additionalBufferFloat[i]);
                //}
                Buffer.BlockCopy(buffersToSend.Dequeue(), 0, bufferToSend, 0, bufferLength * 4);

                Debug.Log("SENDED?: " + NetworkModel.Instance.Send(bufferToSend,
                    TransferProtocol.DataType.LRChannelBuffer));
            }
            yield return null;
        }
        
    }

    public float getAverageSampleRateForSecond()
    {
        float sampleRate = (float)sampleCount / (clock.ElapsedMilliseconds) * 1000;
        clock.Stop();
        return sampleRate;
    }

    private void notifyObservers(byte[] data, TransferProtocol.DataType dataType)
    {
        if (1==0 && notified == false)
        {
            string bytes = "";
            foreach (byte b in data) {
                bytes += b + " ";
            }
            Debug.Log(bytes + " " + dataType.ToString() + "SENDED?: " + NetworkModel.Instance.Send(data, dataType));
            //notified = true;
        }
    }

    public void Cleanup()
    {
        foreach (var so in _soundOutList)
        {
            so.Stop();
            so.Dispose();
        }
        capture.Stop();
        capture.Dispose();
    }

    void OnApplicationQuit()
    {
        //Before there was a some kind of variable associated with MonoBehaviour - enabled
        if (enabled)
        {
            foreach (var so in _soundOutList)
            {
                so.Stop();
                so.Dispose();
            }
            capture.Stop();
            capture.Dispose();
        }
    }



}