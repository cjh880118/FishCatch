using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Localizing_Text : MonoBehaviour
{
    //ob
    // 자기자신을 start 상에서 연결해야한다.
    // 자기자신을 매니저에 연결
    // 자기자신의 코드를 해당 문자로 바꿔야 한다.
    public int iKey;
    public Text text;

    private void Start()
    {
        if (text == null) text = GetComponent<Text>();
        Localizing_Mng.I.OnLanguageChangeEvent.AddListener(LocalizeCode);
        LocalizeCode();

    }

    private void OnDestroy()
    {
        if (Localizing_Mng.I != null)
        {
            Localizing_Mng.I.OnLanguageChangeEvent.RemoveListener(LocalizeCode);
        }
    }

    // 초기화와 동시에 사용할 수 없음
    //private void OnEnable()
    //{
    //    LocalizeCode();
    //}

    public void LocalizeCode()
    {
        text.text = Localizing_Mng.I.ConvertLanguage(iKey);
    }

}
