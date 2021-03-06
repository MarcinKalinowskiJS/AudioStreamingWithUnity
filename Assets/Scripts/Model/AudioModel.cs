﻿using CSCore;
using CSCore.SoundIn;
using CSCore.Streams;
using CSCore.SoundOut;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CSCore.Codecs.WAV;
using System;
using CSCore.CoreAudioAPI;

public class AudioModel : MonoBehaviour
{
    private static AudioModel instance = null;

    WasapiCapture capture;
    IWaveSource finalSource;
    List<ISoundOut> _soundOutList;
    public int bufferLength=2048;
    float[] buffer;
    int sampleNumberForBuffer;
    // Start is called before the first frame update
    void Start()
    {
        //o.updateObserverErgoSendData(new byte[] { 1, 3, 5, 7 });
        
        
        /*
        buffer = new float[bufferLength];
        observers = new List<ObserverSender>();
        Debug.Log("WasapiCapture.IsSupportedOnCurrentPlatform: " + WasapiCapture.IsSupportedOnCurrentPlatform);
        capture = new WasapiLoopbackCapture();
        capture.Initialize();
        IWaveSource source = new SoundInSource(capture);
        var notificationSource = new SingleBlockNotificationStream(source.ToSampleSource());
        notificationSource.SingleBlockRead += NotificationSource_SingleBlockRead;

        finalSource = notificationSource.ToWaveSource();

        capture.DataAvailable += Capture_DataAvailable;
        capture.Start();

        var mmdeviceEnumerator = new MMDeviceEnumerator();
        var mmdeviceCollection = mmdeviceEnumerator.EnumAudioEndpoints(DataFlow.Render, DeviceState.Active);
        _soundOutList = new List<ISoundOut>();
        foreach (var device in mmdeviceCollection) {
            _soundOutList.Add(new WasapiOut() { Latency = 2000, Device = device });
        }
        foreach (var so in _soundOutList) {
            so.Initialize(finalSource);
            so.Volume = 0.8f;
            so.Play();
        }
        Debug.Log("Devices Count: " + mmdeviceCollection.GetCount());
        */
    }

    /*public static AudioModel Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new AudioModel();
            }
            return instance;
        }
    }*/

    private void Capture_DataAvailable(object sender, DataAvailableEventArgs e) {
        finalSource.Read(e.Data, e.Offset, e.ByteCount);
    }

    private void NotificationSource_SingleBlockRead(object sender, SingleBlockReadEventArgs e) {
        //Probably actions for readed data from source
        //Debug.Log("Left: " + e.Left + " \t\tRight: " + e.Right);
        buffer[sampleNumberForBuffer] = e.Left;
        buffer[sampleNumberForBuffer+1] = e.Right;
        sampleNumberForBuffer = (sampleNumberForBuffer + 2) % bufferLength;
        notify();
    }

    private void notify() {

    }

    void OnApplicationQuit()
    {
        //TODO: Delete 1==0
        if (enabled && 1==0)
        {
            foreach (var so in _soundOutList) {
                so.Stop();
                so.Dispose();
            }
            capture.Stop();
            capture.Dispose();
        }
    }


    
}
