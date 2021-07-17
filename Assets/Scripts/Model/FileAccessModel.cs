using Assets.Scripts.Model.Additional;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Model
{
    public class FileAccessModel
    {
        List<WaveFormatInfo> wfiL = new List<WaveFormatInfo>();

        //The path cannot start with slash or backslash because file won't be created
        private string _path = "";
        private string _workingFilename = "";
        private string _workingDirectory = "";
        private string _extension = "";

        public FileAccessModel() {
            setPath(UnityEngine.Windows.Directory.localFolder + "/Supported Encoding List.wfi");
            Debug.Log("Path:" + _path);
        }

        private int setPath(string path) {
            //No file name
            if (path.Length <= 0)
            {
                return 1;
            }

            if (!"".Equals(GetOnlyFilename(path))) {
                _workingFilename = GetOnlyFilename(path);
            }
            else{
                return 2;
            }

            if (!"".Equals(GetOnlyExtension(path)))
            {
                _extension = GetOnlyExtension(path);
            }
            else {
                _extension = "wfi";
            }

            _path = path;

            _workingDirectory = GetOnlyDirectoryPath(path);
            if (_workingDirectory != null && _workingDirectory.Length > 0)
            {
                Directory.CreateDirectory(_workingDirectory);
            }
            return 0;
        }

        private static string GetOnlyDirectoryPath(string path)
        {
            return Path.GetDirectoryName(path);
        }

        private static string GetOnlyFilename(string path)
        {
            return Path.GetFileName(path);
        }

        private static string GetOnlyExtension(string path)
        {
            return Path.GetExtension(path);
        }
        /*    string directoryPath = "";

            int last = path.LastIndexOfAny(new char[] { '\\', '/' });
            if (last >= 0 && last < path.Length)
            {
                directoryPath = path.Remove(last, path.Length - last);
            }

            return directoryPath;
        }*/
        public bool WriteSupportedEncodings(List<WaveFormatInfo> wfiL)
        {
            StreamWriter writer = new StreamWriter(_path, false);
            wfiL.ForEach(s => writer.WriteLine(s.ToString()));
            writer.Close();

            return true;
        }

        public List<WaveFormatInfo> ReadSupportedEncodings()
        {
            List<WaveFormatInfo> supportedRecordingsEncodings = new List<WaveFormatInfo>();
            StreamReader reader = new StreamReader(_path);

            string line = "";
            WaveFormatInfo wfiTmp = null;
            while ((line = reader.ReadLine()) != null) {
                wfiTmp = WaveFormatInfo.Parse(line);
                supportedRecordingsEncodings.Add(wfiTmp);
            }

            return supportedRecordingsEncodings;
        }


        
    }
}
