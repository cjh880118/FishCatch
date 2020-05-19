using UnityEngine;
using UnityEngine.Rendering;
using System;
using System.Collections;
using CellBig.UI.Event;
using CellBig.Contents.Event;
using CellBig.Models;

namespace CellBig.Contents
{
	public abstract class IContent : MonoBehaviour
	{
        [SerializeField]
        protected Camera mainCamera = null;

        protected string _name;

		public IContentUILoader _uiLoader;
		public bool dontDestroy = false;
		public bool stackable = true;

		public delegate void OnComplete(GameObject obj);
		OnComplete _onLoadComplete;

		protected bool _contentLoadComplete = true;
		protected bool _uiLoadComplete = true;

		public bool isActive { get; private set; }

        protected PlayContentModel pcm;
        protected GameModel gm;
        protected SettingModel sm;

        [Header("[ Lighting Setting ]")]
        public LightingsSetting lightingSetting;

        private void Awake()
        {
            gm = Model.First<GameModel>();
            sm = Model.First<SettingModel>();
            pcm = Model.First<PlayContentModel>();
        }

        public void Load(string gameName, OnComplete complete)
		{
			_name = GetType().Name;
			_onLoadComplete = complete;
            
			Message.AddListener<Event.EnterContentMsg>(_name, Enter);
			Message.AddListener<Event.ExitContentMsg>(_name, Exit);

            StartCoroutine(LoadingProcess(gameName));
		}

		void LoadContentsUI(string gameName)
		{
			if (_uiLoader != null)
			{
				_uiLoader.Load(gameName,
					() =>
					{
						_uiLoadComplete = true;
						OnUILoadComplete();
					});
			}
			else
			{
				_uiLoadComplete = true;
			}
		}

		IEnumerator LoadingProcess(string gameName)
		{
			_uiLoadComplete = false;
			_contentLoadComplete = false;

            OnLoadStart();
            LoadContentsUI(gameName);

			do
			{
				yield return null;
			}
			while (!_uiLoadComplete || !_contentLoadComplete);

			OnLoadComplete();

            LightingInit();

            if (_onLoadComplete != null)
				_onLoadComplete(gameObject);
		}

		protected void SetLoadComplete()
		{
			_contentLoadComplete = true;
		}

		/// <summary>
		/// 생성과 동시에 메시지 및 모델을 생성해야 할 경우 재정의 한 후 구현한다.
		/// 이 콜백을 재정의 하게 되면 적절한 타이밍에 SetLoadComplete() 를 호출해주어야 한다.
		/// </summary>
		protected virtual void OnLoadStart()
		{
			SetLoadComplete();
		}

		protected virtual void OnLoading(float progress)
		{
			/* BLANK */
		}

		protected virtual void OnLoadComplete()
		{
			/* BLANK */
		}

		protected virtual void OnUILoadComplete()
		{
			/* BLANK */
		}

		public void Unload()
		{
			Message.RemoveListener<Event.EnterContentMsg>(_name, Enter);
			Message.RemoveListener<Event.ExitContentMsg>(_name, Exit);

            OnExit();

			if (_uiLoader != null)
				_uiLoader.Unload();

			OnUnload();
		}

        /// <summary>
        /// OnLoad()에서 생성된 메세지나 모델을 이곳에서 해제 한다.
        /// </summary>
        protected virtual void OnUnload()
		{
			/* BLANK */
		}

		void Enter(Event.EnterContentMsg msg)
		{
        #if UNITY_EDITOR
            //Debug.LogError("Enter" + gameObject.name);
        #endif
			if (isActive)
			{
				Debug.LogWarningFormat("{0} are entered.", _name);
				return;
			}
            isActive = true;
            AddMessage();

          #if UNITY_EDITOR
            Debug.Log("IContent Enter");
          #endif

            OnEnter();

            if (lightingSetting.isLightingSetting)
                LightingSetting();
            //else
            //    LightingInit();
        }

		void Exit(Event.ExitContentMsg msg)
		{
        #if UNITY_EDITOR
            Debug.Log("Exit:" + _name);
        #endif
            OnExit();
            RemoveMessage();

            isActive = false;
		}

        protected virtual void AddMessage()
        {
        }

        protected virtual void RemoveMessage()
        {
        }

        protected abstract void OnEnter();
        protected abstract void OnExit();

        void LightingInit()
        {
            RenderSettings.skybox = null;
            RenderSettings.sun = null;

            RenderSettings.ambientMode = AmbientMode.Skybox;
            RenderSettings.ambientIntensity = 1;
            RenderSettings.ambientSkyColor = new Color(0.212f,0.227f,0.259f);
            RenderSettings.defaultReflectionMode = DefaultReflectionMode.Skybox;

            RenderSettings.defaultReflectionResolution = 128;
            RenderSettings.reflectionIntensity = 1;
            RenderSettings.reflectionBounces = 1;
        }

        public void LightingSetting()
        {
            if (!lightingSetting.isLightingSetting)
                return;

            RenderSettings.skybox = lightingSetting.skyBoxMaterial;
            RenderSettings.sun = lightingSetting.sunSource;
            RenderSettings.ambientMode = lightingSetting.ambientMode;

            if (lightingSetting.ambientMode == AmbientMode.Skybox)
            {
                RenderSettings.ambientIntensity = lightingSetting.SkyBoxMode_IntensityMultiplier;
            }
            else if (lightingSetting.ambientMode == AmbientMode.Trilight)
            {
                RenderSettings.ambientSkyColor = lightingSetting.TrilightMode_SkyColor;
                RenderSettings.ambientEquatorColor = lightingSetting.TrilightMode_EquatorColor;
                RenderSettings.ambientGroundColor = lightingSetting.TrilightMode_GroundColor;
            }
            else if (lightingSetting.ambientMode == AmbientMode.Flat)
                RenderSettings.ambientSkyColor = lightingSetting.FlatMode_AmbientColor;

            RenderSettings.defaultReflectionMode = lightingSetting.reflectionMode;

            if (RenderSettings.defaultReflectionMode == DefaultReflectionMode.Custom)
                RenderSettings.customReflection = lightingSetting.CubemapMode_Cubemap;

            RenderSettings.defaultReflectionResolution = lightingSetting.reflection_Resolution;

            RenderSettings.reflectionIntensity = lightingSetting.Reflections_IntensityMultiplier;
            RenderSettings.reflectionBounces = lightingSetting.Reflections_Bounces;

            if (lightingSetting.isLightMap)
            {
                lightingSetting.lightMapData = new LightmapData[lightingSetting.lightMapCount];

                for (int i = 0; i < lightingSetting.lightMapCount; i++)
                {
                    LightmapData data = new LightmapData();
                    data.lightmapColor = Resources.Load("LightMap/" + lightingSetting.contentName + "/Lightmap-"+ i +"_comp_light") as Texture2D;
                    data.lightmapDir = Resources.Load("LightMap/" + lightingSetting.contentName + "/Lightmap-" + i + "_comp_dir") as Texture2D;

                    lightingSetting.lightMapData[i] = data;
                }

                LightmapSettings.lightmaps = lightingSetting.lightMapData;
            }
        }

        public static void RequestContentEnter<T>() where T : IContent
        {
            Message.Send<Event.EnterContentMsg>(typeof(T).Name, new Event.EnterContentMsg());
        }

        public static void RequestContentExit<T>() where T : IContent
        {
            Message.Send<Event.ExitContentMsg>(typeof(T).Name, new Event.ExitContentMsg());
        }

        public static void RequestContentEnter(string name)
        {
            Message.Send<Event.EnterContentMsg>(name, new Event.EnterContentMsg());
        }

        public static void RequestContentExit(string name)
        {
            Message.Send<Event.ExitContentMsg>(name, new Event.ExitContentMsg());
        }

        public static string GetMsgName<T>()
		{
			return typeof(T).Name;
		}
	}

    [Serializable]
    public class LightingsSetting
    {
        public bool isLightingSetting;

        [Header("[ Environment ]")]
        public Material skyBoxMaterial;
        public Light sunSource;

        [Header("[ Environment_Environment Lighting ]")]
        public AmbientMode ambientMode;

        public float SkyBoxMode_IntensityMultiplier;

        [ColorUsage(false, true)]
        public Color TrilightMode_SkyColor;
        [ColorUsage(false, true)]
        public Color TrilightMode_EquatorColor;
        [ColorUsage(false, true)]
        public Color TrilightMode_GroundColor;

        [ColorUsage(false, true)]
        public Color FlatMode_AmbientColor;

        [Header("[ Environment_Environment Reflections ]")]
        public DefaultReflectionMode reflectionMode;
        public Cubemap CubemapMode_Cubemap;
        public int reflection_Resolution;
        public ReflectionCubemapCompression reflection_Compression;

        [Range(0f, 1f)]
        public float Reflections_IntensityMultiplier;
        [Range(1, 5)]
        public int Reflections_Bounces;

        public bool isLightMap;
        public int lightMapCount;
        public string contentName;

        public LightmapData[] lightMapData;
    }
}
