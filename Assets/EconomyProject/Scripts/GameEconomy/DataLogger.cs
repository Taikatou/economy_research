using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Inventory;
using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy
{
// CODE COURTESY OF https://sushanta1991.blogspot.com/2015/02/how-to-write-data-to-csv-file-in-unity.html

    public class DataLogger : MonoBehaviour
    {
        public static int staticLoggerId = 0;

        public string learningEnvironmentId = "agent_id_";

        public int loggerId;

        public string CurrentTime
        {
            get
            {
                var endTimer = FindObjectOfType<EndTimerScript>();
                return endTimer ? endTimer.CurrentTime : "";
            }
        }
        public string GetFileName(string fileName)
        {
            var nowStr = DateTime.Now.ToString("_dd_MM_yyyy_HH_mm");
            return fileName + loggerId + nowStr + ".csv";
        }

        protected virtual void Start()
        {
            staticLoggerId++;
            loggerId = staticLoggerId;
        }

        public void OutputCsv(List<string[]> rowData, string fileName)
        {
            string[][] output = new string[rowData.Count][];

            for (int i = 0; i < output.Length; i++)
            {
                output[i] = rowData[i];
            }

            int length = output.GetLength(0);
            string delimiter = ",";

            StringBuilder sb = new StringBuilder();

            for (int index = 0; index < length; index++)
                sb.AppendLine(string.Join(delimiter, output[index]));


            var filePath = GetPath(fileName);

            var exists = Directory.Exists(GetPath());
            if (!exists)
            {
                Directory.CreateDirectory(filePath);   
            }

            System.IO.Directory.CreateDirectory(GetPath(""));
            StreamWriter outStream = File.CreateText(filePath);
            outStream.WriteLine(sb);
            outStream.Close();
        }

        private string GetPath()
        {
            #if UNITY_EDITOR
                return Application.dataPath + "/CSV/";
            #elif UNITY_ANDROID
                return Application.persistentDataPath + "/CSV/";
            #elif UNITY_IPHONE
                return Application.persistentDataPath + "/CSV/";
            #else
                return Application.dataPath + "/CSV/";
            #endif
        }

        // Following method is used to retrive the relative path as device platform
        private string GetPath(string fileName)
        {
            return GetPath() + GetFileName(fileName);
        }
    }
}