using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using CellBig.Common;

// 프로그램 종료시 텍스트 파일 연결
public class TextFileOutPut : MonoSingleton<TextFileOutPut>
{
    string path;
    static int SaveNum;
    string FileName;
    string SavePath;

    [HideInInspector]
    public string SaveText;

    private void Awake()
    {
        path = Application.streamingAssetsPath;

        SaveNum =  PlayerPrefs.GetInt("MySaveNum", 0);        
         SaveNum++;
        if (SaveNum > 10) SaveNum = 0;
        PlayerPrefs.SetInt("MySaveNum", SaveNum);
        FileName = SaveNum + "_DebugOutPut.txt";
        SavePath = path + "\\" + FileName;
    }
  
    public void SaveStringLine(string s)
    {
        SaveText += s +"\n";
    }

    protected override void Release()
    {
       
        using (StreamWriter outputFile = new StreamWriter(SavePath))
        {
            outputFile.WriteLine(SaveText);
        }
    }
   
}
