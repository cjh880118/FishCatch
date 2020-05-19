using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using CellBig.Contents;


namespace CellBig.Scene
{
	// 해당 씬을 구성하는 역할을 한다.
	// 씬의 구성은 씬에서 사용하는 UI를 로드하고 씬에 사용될 Prefab 등을 로드하는 것을 기본으로 한다.
	public abstract class IScene : MonoBehaviour
	{
        public Constants.GameName gameName;
		public Constants.SceneName sceneName;

        public List<string> ContentsList = new List<string>();
        public List<string> CommonContentsList = new List<string>();

        public List<string> defaultContentList = new List<string>();
		List<string> _enterContentList;

		Action _onLoadComplete = null;
		//bool _resourceLoadComplete = false;
		int _loadingContentsCount = 0;

		public void LoadAssets(string gameName, List<string> enterContentList, Action onComplete)
		{
			_enterContentList = enterContentList;
			_onLoadComplete = onComplete;

			StartCoroutine(LoadContents(gameName));
		}

        IEnumerator LoadContents(string gamName)
		{
            OnLoadStart();
            Application.targetFrameRate = -1;


            _loadingContentsCount = CommonContentsList.Count;

            for (int i = 0; i < CommonContentsList.Count; ++i)
            {
                yield return StartCoroutine(ContentsLoader.Instance.Load("Common", CommonContentsList[i],
                    c =>
                    {
                        _loadingContentsCount--;
                        OnContentLoadComplete(c);
                    }));
            }


            _loadingContentsCount = ContentsList.Count;

			for (int i = 0; i < ContentsList.Count; ++i)
			{
				yield return StartCoroutine(ContentsLoader.Instance.Load(gamName, ContentsList[i],
					c =>
					{
						_loadingContentsCount--;
						OnContentLoadComplete(c);
					}));
			}
            EnterContents();
            OnLoadComplete();
            Application.targetFrameRate = 60;

            if (_onLoadComplete != null)
                _onLoadComplete();
        }

		void EnterContents()
		{
			for (int i = 0; i < defaultContentList.Count; i++)
				Message.Send<Contents.Event.EnterContentMsg>(defaultContentList[i], new Contents.Event.EnterContentMsg());
		}

		public void SetResourceLoadComplete()
		{
            //_resourceLoadComplete = true;
		}

		/// <summary>
		/// 이 콜백을 재정의 하게 되면 적절한 타이밍에 SetAssetLoadComplete() 를 호출해주어야 한다.
		/// </summary>
		protected virtual void OnLoadStart()
		{
            SetResourceLoadComplete();
		}

		protected virtual void OnLoadComplete()
		{
			/* BLANK */
		}

		protected virtual void OnContentLoadComplete(GameObject content)
		{
			/* BLANK */
		}

		public void Unload(string gameName)
		{
			 OnUnload();

			if (ContentsLoader.isAlive)
            {
                for (int i = 0; i < CommonContentsList.Count; ++i)
                    ContentsLoader.Instance.Unload("Common", CommonContentsList[i]);
                for (int i = 0; i < ContentsList.Count; ++i)
					ContentsLoader.Instance.Unload(gameName, ContentsList[i]);
			}

			Destroy(gameObject);
		}

		protected virtual void OnUnload()
		{
			/* BLANK */
		}
	}
}
