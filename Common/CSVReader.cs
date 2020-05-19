using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class CSVReader
{
    public class CSVData
    {
        public CSVData() { }
        public virtual void Init(string[] data) { }
    }

    static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
    static char[] TRIM_CHARS = { '\"' };

    public static List<Dictionary<string, object>> Read2(string file)
    {
        var list = new List<Dictionary<string, object>>();
        TextAsset data = Resources.Load(file) as TextAsset;

        var lines = Regex.Split(data.text, LINE_SPLIT_RE);

        if (lines.Length <= 1) return list;

        var header = Regex.Split(lines[0], SPLIT_RE);
        for (var i = 1; i < lines.Length; i++)
        {

            var values = Regex.Split(lines[i], SPLIT_RE);
            if (values.Length == 0 || values[0] == "") continue;

            var entry = new Dictionary<string, object>();
            for (var j = 0; j < header.Length && j < values.Length; j++)
            {
                string value = values[j];
                value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");
                object finalvalue = value;
                int n;
                float f;
                if (int.TryParse(value, out n))
                {
                    finalvalue = n;
                }
                else if (float.TryParse(value, out f))
                {
                    finalvalue = f;
                }
                entry[header[j]] = finalvalue;
            }
            list.Add(entry);
        }
        return list;
    }

    public static List<Dictionary<string, string>> Read(string file)
    {
        var list = new List<Dictionary<string, string>>();
        TextAsset data = Resources.Load(file) as TextAsset;

        var lines = Regex.Split(data.text, LINE_SPLIT_RE);

        if (lines.Length <= 1) return list;

        var header = Regex.Split(lines[0], SPLIT_RE);
        for (var i = 1; i < lines.Length; i++)
        {

            var values = Regex.Split(lines[i], SPLIT_RE);
            if (values.Length == 0 || values[0] == "") continue;

            var entry = new Dictionary<string, string>();
            for (var j = 0; j < header.Length && j < values.Length; j++)
            {
                string value = values[j];
                value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");

                entry[header[j]] = value;
            }
            list.Add(entry);
        }
        return list;
    }

    public static List<Dictionary<string, string>> Read(TextAsset pText)
    {
        var list = new List<Dictionary<string, string>>();
        TextAsset data = pText;

        var lines = Regex.Split(data.text, LINE_SPLIT_RE);

        if (lines.Length <= 1) return list;

        var header = Regex.Split(lines[0], SPLIT_RE);
        for (var i = 1; i < lines.Length; i++)
        {

            var values = Regex.Split(lines[i], SPLIT_RE);
            if (values.Length == 0 || values[0] == "") continue;

            var entry = new Dictionary<string, string>();
            for (var j = 0; j < header.Length && j < values.Length; j++)
            {
                string value = values[j];
                value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");

                entry[header[j]] = value;
            }
            list.Add(entry);
        }
        return list;
    }


    public static int GetFindDataRow(List<Dictionary<string, string>> pList, string sColumKey, string sRowKey)
    {
        for (int i = 0; i < pList.Count; i++)
        {
            if ((string)pList[i][sColumKey] == sRowKey)
            {
                return i;
            }
        }
        Debug.Log("Error-GetFindDataRow");
        return -1;
    }





    public enum Type
    {
        ETC_String,
        Text,
    }

    public static T GetETCData<T>(Type type) where T : CSVData, new()
    {
        return GetData<T>(type)[0];
    }
    public static List<T> GetData<T>(Type type) where T : CSVData, new()
    {
        string fileName = "";

        switch (type)
        {
            case Type.ETC_String:
                fileName = "string_etc";
                break;
            case Type.Text:
                fileName = "text";
                break;
        }

        if (string.IsNullOrEmpty(fileName))
        {
            Debug.LogError("CSV Name is Empty : " + type.ToString());
        }

        string textData = null;

        WWW www = new WWW("file://" + Application.streamingAssetsPath + "CSV/" + fileName + ".csv");

        while (www.isDone == false) { }
        if (www.error == null)
        {
            textData = www.text;
        }

        //ReturnValue.
        List<T> retList = new List<T>();

        try
        {
            string[] CSVRow = textData.Split('\n');
            for (int i = 1; i < CSVRow.Length; i++)
            {
                if (!string.IsNullOrEmpty(CSVRow[i]))
                {
                    T t = new T();
                    var data = CSVRow[i].Split(",".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);
                    t.Init(data);
                    retList.Add(t);
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.Log(fileName + " EROREORORER : " + e);
            retList.Clear();

            TextAsset textAsset = Resources.Load("CSV/" + fileName) as TextAsset;

            if (textAsset == null)
            {
                Debug.LogError(fileName + ".csv is Empty!!!");
            }
            textData = textAsset.text;

            string[] CSVRow = textData.Split('\n');
            for (int i = 1; i < CSVRow.Length; i++)
            {
                if (!string.IsNullOrEmpty(CSVRow[i]))
                {
                    T t = new T();
                    var data = CSVRow[i].Split(",".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);
                    t.Init(data);
                    retList.Add(t);
                }
            }
        }

        return retList;
    }

}
