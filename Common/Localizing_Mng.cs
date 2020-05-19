using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Localizing_Mng : Singleton<Localizing_Mng>
{
    public class Data_Text : CSVReader.CSVData
    {
        // 설계한 TextData
        /// dictory로 바꾸자
        int iKey; //key
        string[] sLan; //Value
        public override void Init(string[] data)
        {
            //데이터 분리
            sLan = new string[(int)Localizing_Mng.E_Language.MAX];
            iKey = Int32.Parse(data[0]);
            for (int i = 1; i < data.Length; i++)
            {
                sLan[i - 1] = data[i];
            }

            //저장
            Localizing_Mng.I.m_DictionaryData.Add(iKey, sLan);
            sLan = null;
        }
    }

    public enum E_Language
    {
        KOR = 0,
        ENG,
       // JAP,
        MAX
    }

    // 언어 정보는 XML 형식으로 저장하지 않았음
    public E_Language eLanSetting = E_Language.KOR;

    public Dictionary<int, string[]> m_DictionaryData = new Dictionary<int, string[]>(); 

    public UnityEngine.Events.UnityEvent OnLanguageChangeEvent = new UnityEngine.Events.UnityEvent();

    private void Awake()
    {
        //무조건 리스트로 관리된다.
        CSVReader.GetData<Localizing_Mng.Data_Text>(CSVReader.Type.Text);

        // 현재 데이터 가져옴
        eLanSetting = (E_Language)PlayerPrefs.GetInt("E_Language", 0);
    }

   
    void OnChange(E_Language setting)
    {
        PlayerPrefs.SetInt("E_Language", (int)setting);

        eLanSetting = setting;
        OnLanguageChangeEvent.Invoke();
    }

    public void ChangeLan(int lan)
    {
        Localizing_Mng.I.OnChange((Localizing_Mng.E_Language)lan);
    }

 

    //text_ID값과 언어설정을 넣으면 해당 텍스트가 나온다.
    public string ConvertLanguage(int textID) //설정값에 따라 해당 언어만 빼온다.
    {
        string[] str = null;
         str = m_DictionaryData[textID];
        if (str != null)
        {
            return str[(int)eLanSetting];
        }
        return null;
    }

}
