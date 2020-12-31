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

    // Start is called before the first frame update
    void Start()
    {
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
        //Debug.Log("senderCap" + sender);
        //Debug.Log("cap" + e.Data + " " + e.Offset + " " + e.ByteCount);
        //finalSource.Read(e.Data, e.Offset, e.ByteCount);
        //byte[] data = new byte[38400];
        //randomGenerator.NextBytes(data);
        //Debug.Log("data[0]: " + data[0]);
        //finalSource.Read(data, 0, data.Length);
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
        sampleCount++;

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