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
using System.Linq;
using Assets.Scripts.Model;

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
    TCPStream tcps;
    byte[] randomsTest;
    int sentBuffersCount;

    // Start is called before the first frame update
    void Start()
    {
        TestEncodings();
        /*
        capture = new WasapiLoopbackCapture(200, new CSCore.WaveFormat(44100, 16, 2, AudioEncoding.IeeeFloat));
        //WasapiLoopbackCapture.
        capture.Initialize();
        capture.DataAvailable += Capture_DataAvailable;
        capture.Start();

        tcps = new TCPStream("Win", TransferProtocol.ConnectionType.ReceiveAndSend, "192.168.0.3", 65535, "192.168.0.3", 65535);
        //SoundInSource newWave = new SoundInSource(capture);
        //WasapiCapture wCapture = new WasapiCapture(false, AudioClientShareMode.Shared, 100, 
        //new WaveFormat(48000, 32, 2, AudioEncoding.MpegLayer3));
        //var sampleTonewPCM8 = new CSCore.(new CSCore.Streams.SampleConverter.IeeeFloatToSample(capture));


        Debug.Log(capture.WaveFormat.ToString());


        src = new WriteableBufferingSource(new CSCore.WaveFormat(44100, 16, 2, AudioEncoding.IeeeFloat)) { FillWithZeros = true };


        Debug.Log("!!!!!!!!!!!!!!!!!!Eq1 ?: " + capture.WaveFormat.Equals(src.WaveFormat));


        var mmDeviceCollection = (new MMDeviceEnumerator()).EnumAudioEndpoints(DataFlow.Render, DeviceState.Active);
        _soundOutList = new List<ISoundOut>();
        foreach (var device in mmDeviceCollection)
        {
            _soundOutList.Add(new WasapiOut() { Latency = 1000, Device = device });
        }
        foreach (var so in _soundOutList)
        {
            so.Initialize(src);
            so.Volume = 0.1f;
            so.Play();
        }



        StartCoroutine(ReceiveBufferCoroutine());
            */
    }

    //TODO: Implement functions
    public List<WaveFormatInfo> TestEncodings() {
        List<WaveFormatInfo> supportedRecordingsEncodings = null;

        FileAccessModel fam = new FileAccessModel();
        fam.WriteSupportedEncodings(CheckSupportedWasapiLoopbackCaptureWaveFormat());

        /*
        //Ask which file to read
        string path = "";

        //Check if file exists
        if (true == false) {
            ;// supportedRecordingsEncodings = CheckSupportedWasapiLoopbackCaptureWaveFormat();
        }

        //Ask to overwrite it with new data
        if (true == false){

            ;//WriteSupportedEncodings(supportedRecordingsEncodings);
        }

        //Read from file
        ReadSupportedEncodings(path);
        //List<WaveFormatInfo> playableEncodings = PlaySupportedEncodings(supportedRecordingsEncodings);
        */
        return null;
    }
    


    public List<WaveFormatInfo> CheckSupportedWasapiLoopbackCaptureWaveFormat() {

        //TODO: implement different supported capture tests length
        short testsLength = 0;

        List<int> sampleRatesToTest = new List<int>() { 44100 };//, 48000 };
        List<int> bitsToTest = new List<int>() { 8 };//, 16, 25, 32 };
        List<int> channelsToTest = new List<int>() { 1, 2 };

        //TODO HERE: Reenable scanning of all properties of AudioEncoding
        //Also can make a list of all WaveFormats etc. and test all in one try catch block
        //to potentially speed up process
        List<AudioEncoding> AEncToTest = new List<AudioEncoding>();//AudioEncoding.GetValues(typeof(AudioEncoding)).OfType<AudioEncoding>().ToList();
        AEncToTest.Add(AudioEncoding.Acelp);

        WasapiCapture testCapture = null;
        List<WaveFormatInfo> supported = new List<WaveFormatInfo>();

        Debug.Log("starting tests");
        Debug.Log("AEncToTest: " + AEncToTest.Count);

        
        //create, initialize and get result
        foreach (int srtt in sampleRatesToTest)
        {
            foreach (int btt in bitsToTest)
            {
                foreach (int ctt in channelsToTest)
                {
                    foreach (AudioEncoding aett in AEncToTest)
                    {
                        //Latency: 200
                        int latency = 200;
                        testCapture = new WasapiLoopbackCapture(latency, new CSCore.WaveFormat(srtt, btt, ctt, aett));
                        try
                        {
                            testCapture.Initialize();
                            testCapture.Start();
                        } catch (CoreAudioAPIException CSCoreEx) {
                            //Something else than format not supported exception
                            if (CSCoreEx.ErrorCode != -2147024809) 
                            {
                                Debug.Log("Something went wrong");
                                throw (CSCoreEx);
                            }
                        }
                        if (testCapture.RecordingState.Equals(RecordingState.Recording)) {
                            supported.Add(new WaveFormatInfo(latency, srtt, btt, ctt, aett));
                        }
                        testCapture.Stop();
                    }
                }
            }
        }

        Debug.Log(supported.Count);
        return supported;
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