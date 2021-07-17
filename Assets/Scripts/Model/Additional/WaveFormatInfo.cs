using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Model.Additional
{
    public class WaveFormatInfo
    {
        private int _latency;
        private int _sampleRate;
        private int _bits;
        private int _channels;
        private CSCore.AudioEncoding _audioEncoding;

        public int Latency
        {
            get => _latency;
        }
        public int SampleRate
        {
            get => _sampleRate;
        }
        public int Bits
        {
            get => _bits;
        }
        public int Channels
        {
            get => _channels;
        }
        public CSCore.AudioEncoding AudioEncoding
        {
            get => _audioEncoding;
        }
        public WaveFormatInfo(int latency, int sampleRate, int bits, int channels, CSCore.AudioEncoding audioEncoding) {
            _latency = latency;
            _sampleRate = sampleRate;
            _bits = bits;
            _channels = channels;
            _audioEncoding = audioEncoding;
        }

        /*public override string ToString()
        {
            return "WaveFormatInfo(Latency: @" + Latency + "@ | SampleRate: @" + SampleRate + "@ | Bits: @" + Bits +
                "@ | Channels: @" + Channels + "@ | AudioEncoding: @" + AudioEncoding + "@)";
        }*/

        public override string ToString()
        {
            return string.Format("latency={0}&sampleRate={1}&bits={2}&channels={3}&audioEncoding={4}", 
                _latency, _sampleRate, _bits, _channels, _audioEncoding);
        }

        

        public static WaveFormatInfo Parse(string line){
            
            
            string[] lineSplitted = line.Split(new char[]{ '=', '&'});


            int latency = int.Parse(lineSplitted[1]);
            int sampleRate = int.Parse(lineSplitted[3]);
            int bits = int.Parse(lineSplitted[5]);
            int channels = int.Parse(lineSplitted[7]);
            CSCore.AudioEncoding audioEncoding = (CSCore.AudioEncoding) Enum.Parse(typeof(CSCore.AudioEncoding), lineSplitted[9], false);

            WaveFormatInfo wfiTmp = new WaveFormatInfo(latency, sampleRate, bits, channels, audioEncoding);

            return wfiTmp;
        }
    }
}
