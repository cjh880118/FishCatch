using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CellBig.Common;

public class Config_GameSelect
{
    internal int m_nLoadScene = -1;
    public Config_GameSelect()
    {
        m_nLoadScene = -1;
    }
}

public class Config_OCV
{
    internal List<OpenCVForUnity.Point> m_pPerspectiveCornor = null;

    internal bool m_bFlip = true;
    internal double m_dThreshold = 60.0f;
    internal double m_dBlurPower = 10.0f;
    internal int m_nElement = 50;

    public Config_OCV()
    {
        m_pPerspectiveCornor = new List<OpenCVForUnity.Point>();
    }

    public void Destroy()
    {
        m_pPerspectiveCornor.Clear();
        m_pPerspectiveCornor = null;
    }
}


public class Config_Device
{
    // internal AspectRatio m_pRatio = AspectRatio.Aspect16by9;
    internal Vector2 m_stScreenSize;
    internal float m_fRatioAD_Y = 0.0f;
    internal AspectRatio m_eRatio = AspectRatio.Aspect16by10;
    internal Vector3 m_stRatioScale = Vector3.zero;
    internal string m_sDeviceName = "";
    internal int m_nFrameRate = 60;
    internal bool m_bRunInBackground = true;

    public Config_Device()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Application.runInBackground = m_bRunInBackground;
        Application.targetFrameRate = m_nFrameRate;
        m_stScreenSize.x = Screen.width;
        m_stScreenSize.y = Screen.height;
        m_sDeviceName = SystemInfo.deviceName;

        //SetResolution(1280, 800);
        //SetResolution(Display.displays[0].renderingWidth + Display.displays[1].renderingWidth, Display.displays[0].renderingHeight + Display.displays[1].renderingHeight);
#if !UNITY_EDITOR
       if (Display.displays.Length > 1) Display.displays[1].Activate();
#endif
        //====
#if UNITY_EDITOR
        Debug.Log(m_stScreenSize);
#endif
    }

    public void Destroy()
    {

    }

    public void SetResolution(int nWidth, int nHeight)
    {
        Screen.SetResolution(nWidth, nHeight, true);
        SetRadtio();
        m_eRatio = AspectRatios.GetAspectRatio();
#if UNITY_EDITOR
        Debug.Log(m_eRatio);
#endif
    }


    public void SetResolutionFromDropNum(int nNum)
    {
        switch (nNum)
        {
            case 0: SetResolution(1280, 800); break;
            case 1: SetResolution(1280, 768); break;
            case 2: SetResolution(1024, 768); break;
        }
    }

    void SetRadtio()
    {
#if _debug
        Debug.Log("Current RAspectRatio : " + AspectRatios.GetAspectRatio());
#endif

        switch (AspectRatios.GetAspectRatio())
        {
            case AspectRatio.Aspect4by3:        //! 옵티머스뷰
                m_stRatioScale.x = 0.83f;
                m_stRatioScale.y = 1.0f;
                m_stRatioScale.z = 1.0f;
                break;

            case AspectRatio.Aspect16by9:
                m_stRatioScale.x = 1.107f;
                m_stRatioScale.y = 1.0f;
                m_stRatioScale.z = 1.0f;
                break;

            case AspectRatio.Aspect16by10:
                m_stRatioScale.x = 1.0f;
                m_stRatioScale.y = 1.0f;
                m_stRatioScale.z = 1.0f;
                break;

            case AspectRatio.AspectOthers:
                m_stRatioScale.x = 1.0f;
                m_stRatioScale.y = 1.0f;
                m_stRatioScale.z = 1.0f;
                break;
        }
    }

    public void SetRadtioFromNum(int nNum)
    {
#if _debug
        Debug.Log("Current RAspectRatio : " + AspectRatios.GetAspectRatio());
#endif

        switch (nNum)
        {
            case 2:        //! 옵티머스뷰
                m_stRatioScale.x = 0.83f;
                m_stRatioScale.y = 1.0f;
                m_stRatioScale.z = 1.0f;
                break;

            case 1:
                m_stRatioScale.x = 1.107f;
                m_stRatioScale.y = 1.0f;
                m_stRatioScale.z = 1.0f;
                break;

            case 0:
                m_stRatioScale.x = 1.0f;
                m_stRatioScale.y = 1.0f;
                m_stRatioScale.z = 1.0f;
                break;

        }
    }
}

public class ConfigMng : MonoSingleton<ConfigMng>
{
    // public
    [Space(5)]
    [Header("게임디버그")]
    public bool bDebug_init;

    [HideInInspector]
    public bool bDebugMode = true;
    internal Config_Device pDevice = null;
    internal Config_OCV pOCV = null;
    internal Config_GameSelect pGameSelect = null;



    public enum E_Game_BuildType
    {
        BUILD_BALLPOOL = 0,
        BUILD_FLOOR,
        BUILD_SLIDE,
    };

    public enum E_MARKET_TYPE
    {
        MARKET_NORMAR = 0,
        MARKET_PETITMONDE_LOGO, //쁘띠몽드 로고
        MARKET_MAGIC_LOGO,
        MARKET_CHINA,
    };


    public string m_sGameVersion = "b.1.0.0";
    public E_Game_BuildType m_eBuildType = E_Game_BuildType.BUILD_BALLPOOL;
    //public InGameList m_pInGame = null;
    //public GameList m_pGameList = null;
    public E_MARKET_TYPE m_eMarketType = E_MARKET_TYPE.MARKET_NORMAR;


    void Awake()
    {
        pDevice = new Config_Device();
        pOCV = new Config_OCV();
        pGameSelect = new Config_GameSelect();

        //GameManager.I.mPettiLogo.SetActive(false);
        //GameManager.I.mMagicLogo.SetActive(false);

        //switch (m_eMarketType)
        //{
        //    case E_MARKET_TYPE.MARKET_NORMAR:
        //        GameManager.I.mPettiLogo.SetActive(false);
        //        GameManager.I.mMagicLogo.SetActive(false);
        //        break;
        //    case E_MARKET_TYPE.MARKET_PETITMONDE_LOGO:
        //        GameManager.I.mPettiLogo.SetActive(true);
        //        break;
        //    case E_MARKET_TYPE.MARKET_MAGIC_LOGO:
        //        GameManager.I.mMagicLogo.SetActive(true);
        //        break;
        //    case E_MARKET_TYPE.MARKET_CHINA:
        //        break;
        //    default:
        //        break;
        //}
    }

    protected override void Release()
    {
        pDevice.Destroy();
        pDevice = null;

        pOCV.Destroy();
        pOCV = null;
        //m_pGameList = null;
        pGameSelect = null;
    }

    void OnApplicationFocus(bool isFocus)
    {

#if UNITY_EDITOR
        return;
#endif
        if (isFocus)
        {
            for (int i = Display.displays.Length; i >= 0; i--)
            {
                Display.displays[i].SetParams(Display.displays[i].renderingWidth, Display.displays[i].renderingHeight, 0, 0);
            }
            Screen.SetResolution(Screen.width, Screen.height, false);
            Screen.SetResolution(Screen.width, Screen.height, true);
        }
    }
}
