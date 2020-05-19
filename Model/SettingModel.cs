using System;
using UnityEngine;
using System.Collections.Generic;
using CellBig.Constants;
using System.Text;
using System.IO;

namespace CellBig.Models
{
    public class SettingModel : Model
    {
        GameModel _owner;

        public LocalizingType LocalizingType = LocalizingType.KR;
        public string PortName = "";
        public int PortRate = 0;
        public int PlayTime = 0;        // 컨텐츠 플레이 시간
        public bool Score = true;       // 스코어 On / Off
        public bool Serson = true;      // 센서 On / Off
        public E_OPENCV_MOD SersonType = E_OPENCV_MOD.E_KINECT;
        public float StarEventTime = 0;
        public float OutLineTime = 0f;
        public float DelayTouch = 0.1f;

        public float xDistort;
        public float yDistort;

        public int Width = 640;
        public int Height = 480;
        public int BrushSize = 100;
        public int BrushForce = 2;
        public float RectDistance = 0.2f;

        public Vector2 MinSize = new Vector2(0, 0);
        public Vector2 MaxSize = new Vector2(1, 1);
        public float SensorDistance = 0.05f;
        public float MaxInputTimer = 0.5f;
        public int FrameRate = 5;
        public bool isEndPoint = true;

        public void Setup(GameModel owner)
        {
            _owner = owner;
            LoadSettingFile();
        }

        void LoadSettingFile()
        {
            string line;
            string pathBasic = Application.dataPath + "/StreamingAssets/";
            string path = "Setting/Setting.txt";
            using (StreamReader file = new StreamReader(@pathBasic + path))
            {
                while ((line = file.ReadLine()) != null)
                {
                    if (line.Contains(";") || string.IsNullOrEmpty(line))
                        continue;

                    if (line.StartsWith("Localizing"))
                        LocalizingType = (LocalizingType)int.Parse(line.Split('=')[1]);
                    else if (line.StartsWith("PortName"))
                        PortName = line.Split('=')[1];
                    else if (line.StartsWith("BaudRate"))
                        PortRate = int.Parse(line.Split('=')[1]);
                    else if (line.StartsWith("PlayTime"))
                        PlayTime = int.Parse(line.Split('=')[1]);
                    else if (line.StartsWith("Score"))
                        Score = bool.Parse(line.Split('=')[1]);
                    else if (line.StartsWith("Serson"))
                        Serson = bool.Parse(line.Split('=')[1]);
                    else if (line.StartsWith("SersonType"))
                        SersonType = (E_OPENCV_MOD)int.Parse(line.Split('=')[1]);
                    else if (line.StartsWith("StarEventTime"))
                        StarEventTime = float.Parse(line.Split('=')[1]);
                    else if (line.StartsWith("OutLineTime"))
                        OutLineTime = float.Parse(line.Split('=')[1]);
                    else if (line.StartsWith("DelayTouch"))
                        DelayTouch = float.Parse(line.Split('=')[1]);

                    else if (line.StartsWith("xDistort"))
                        xDistort = float.Parse(line.Split('=')[1]);
                    else if (line.StartsWith("yDistort"))
                        yDistort = float.Parse(line.Split('=')[1]);

                    else if (line.StartsWith("Width"))
                        Width = int.Parse(line.Split('=')[1]);
                    else if (line.StartsWith("Height"))
                        Height = int.Parse(line.Split('=')[1]);
                    else if (line.StartsWith("BrushSize"))
                        BrushSize = int.Parse(line.Split('=')[1]);
                    else if (line.StartsWith("BrushForce"))
                        BrushForce = int.Parse(line.Split('=')[1]);
                    else if (line.StartsWith("RectDistance"))
                        RectDistance = float.Parse(line.Split('=')[1]);

                    else if (line.StartsWith("MaxSize"))
                        MaxSize = new Vector2(float.Parse(line.Split('=')[1].Split('/')[0]),
                            float.Parse(line.Split('=')[1].Split('/')[1]));
                    else if (line.StartsWith("MinSize"))
                        MinSize = new Vector2(float.Parse(line.Split('=')[1].Split('/')[0]),
                            float.Parse(line.Split('=')[1].Split('/')[1]));
                    else if (line.StartsWith("SensorDistance"))
                        SensorDistance = float.Parse(line.Split('=')[1]);
                    else if (line.StartsWith("MaxInputTimer"))
                        MaxInputTimer = float.Parse(line.Split('=')[1]);
                    else if (line.StartsWith("FrameRate"))
                        FrameRate = int.Parse(line.Split('=')[1]);
                    else if (line.StartsWith("isEndPoint"))
                        isEndPoint = bool.Parse(line.Split('=')[1]);
                }

                file.Close();
                line = string.Empty;
            }
        }

        public void SaveSetting()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Localizing" + ((int)LocalizingType).ToString());
            sb.AppendLine("PortName" + PortName);
            sb.AppendLine("BaudRate" + PortRate.ToString());
            sb.AppendLine("PlayTime" + PlayTime.ToString());
            sb.AppendLine("Score" + Score.ToString());
            sb.AppendLine("Serson" + Serson.ToString());
            sb.AppendLine("StarEvent" + StarEventTime.ToString());

            StreamWriter outStream = System.IO.File.CreateText(Application.dataPath + "/StreamingAssets/Setting/" + "Setting.txt");
            outStream.WriteLine(sb);
            outStream.Close();
        }

        public string GetLocalizingPath()
        {
            return LocalizingType.ToString();
        }
    }
}
