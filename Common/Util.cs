using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JHchoi;
using JHchoi.Common;
using JHchoi.Models;
using System.Text.RegularExpressions;
using System.IO;

public enum eUIAniType
{

}

public class Util : MonoSingleton<Util>
{
    //-----------------------------------------------------------------------
    public void PlayAnimator(eUIAniType type, GameObject obj, string LayerType = "UIPlay")
    {
        Animator ani = obj.GetComponent<Animator>();
        if (ani == null)
            ani = obj.AddComponent<Animator>();

        if (ani.runtimeAnimatorController == null)
        {
            string loadPath = GetAnimatorClip(type);
            RuntimeAnimatorController clip = Resources.Load(loadPath, typeof(RuntimeAnimatorController)) as RuntimeAnimatorController;
            if (clip == null)
                return;

            ani.runtimeAnimatorController = clip;
        }

        ani.Rebind();
        ani.Play(LayerType);
    }
    //-----------------------------------------------------------------------
    string GetAnimatorClip(eUIAniType type)
    {
        string clipName = "";
        //switch(type)
        //{
            //case eUIAniType.IconPoint_01:       clipName = "UI/UIAnim/Icon_Point_01";       break;
            //case eUIAniType.IconPoint_02_Num:   clipName = "UI/UIAnim/Icon_Point_02_Num";   break;
            //case eUIAniType.Messege_9Point:     clipName = "UI/UIAnim/Messege_9Point";      break;
            //case eUIAniType.Messege_BG_01:      clipName = "UI/UIAnim/Messege_BG_01";       break;
            //case eUIAniType.Messege_BG_02:      clipName = "UI/UIAnim/Messege_BG_02";       break;
            //case eUIAniType.Messege_BG_03:      clipName = "UI/UIAnim/Messege_BG_03";       break;
            //case eUIAniType.Messege_Player1Win: clipName = "UI/UIAnim/Messege_Player1Win";  break;
            //case eUIAniType.Messege_RoundStart: clipName = "UI/UIAnim/Messege_RoundStart";  break;
            //case eUIAniType.Box_Bottom:         clipName = "UI/UIAnim/Box_Bottom";          break;
            //case eUIAniType.Box_Glow:           clipName = "UI/UIAnim/Box_Glow";            break;
            //case eUIAniType.Box_Result_Text:    clipName = "UI/UIAnim/Box_Result_Text";     break;
            //case eUIAniType.Trophy:             clipName = "UI/UIAnim/Trophy_01";           break;
            //case eUIAniType.Perfect_Text_01:    clipName = "UI/UIAnim/Perfect_Text_01";     break;
            //case eUIAniType.Score_Messege:      clipName = "UI/UIAnim/Score_Messege";       break;
        //}

        return clipName;
    }
    //-----------------------------------------------------------------------
    public Vector2 ChangeMousePos(Vector2 pos)
    {
        Vector2 cPos = pos;
//#if (UNITY_EDITOR || UNITY_EDITOR_64)
//#else
//        var sm = Model.First<SettingModel>();
//        if (sm._touchFull)
//        {
//            if (Display.displays[0].renderingWidth < pos.x)
//                cPos.x -= Display.displays[0].renderingWidth;
//            else if (pos.x < 0)
//                cPos.x += Display.displays[1].renderingWidth;

//            cPos.y -= Display.displays[0].renderingHeight - Display.displays[1].renderingHeight;
//            //cPos.y = Display.displays[0].renderingHeight - cPos.y;
//        }
//        else
//        {
//            cPos.x *= (float)Display.displays[1].renderingWidth / (float)Display.displays[0].renderingWidth;
//            cPos.y *= (float)Display.displays[1].renderingHeight / (float)Display.displays[0].renderingHeight;
//        }

//        //LogContent.log(string.Format("보정_X : {0}      보정_Y : {1}", cPos.x, cPos.y));
//#endif

        return cPos;
    }
    //-----------------------------------------------------------------------
    public IEnumerator LoadSprite(string path, Image image)
    {
        if (image == null)
            yield break;

        string localizingPath = Model.First<SettingModel>().GetLocalizingPath();
        string fullPath = string.Format("UIs/Textures/{0}/{1}", localizingPath, path);
        yield return StartCoroutine(ResourceLoader.Instance.Load<Sprite>(fullPath,
                o =>
                {
                    if (image != null)
                        image.sprite = Instantiate(o) as Sprite;
                }));
    }
    //-----------------------------------------------------------------------
    public List<Dictionary<string, object>> ReadCSV(string fileName)
    {
        string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
        string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
        char[] TRIM_CHARS = { '\"' };

        var list = new List<Dictionary<string, object>>();

        string source;

        try
        {
            StreamReader sr = new StreamReader(Application.dataPath + "/StreamingAssets/Tables/" + fileName + ".csv");
            source = sr.ReadToEnd();
            sr.Close();
        }
        catch (System.Exception)
        {
            return null;
            throw;
        }

        var lines = Regex.Split(source, LINE_SPLIT_RE);

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

    public ObjectPool CreateObjectPool(GameObject root, GameObject target, int count)
    {
        var PoolObject = new GameObject();
        PoolObject.name = target.name + "Pool";
        PoolObject.transform.SetParent(root.transform);
        PoolObject.transform.localScale = Vector3.one;
        PoolObject.transform.localPosition = Vector3.zero;
        ObjectPool p = PoolObject.AddComponent<ObjectPool>();
        p.PreloadObject(count, target as GameObject);
        return p;
    }
}
