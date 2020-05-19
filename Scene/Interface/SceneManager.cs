//#define LOAD_FROM_ASSETBUNDLE

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CellBig.Common;
using CellBig.Models;
using CellBig.Module;
//using Microsoft.Win32;

namespace CellBig.Scene
{
	public class SceneManager : MonoSingleton<SceneManager>
	{
        public Constants.GameName gameName;
		public Constants.SceneName startScene;
		public Constants.SceneName nowScene;

		// TODO: 한번에 여러 씬을 로딩하는 경우를 위해 구현한 부분인데 이제는 그럴일이 없다. 수정하자.
		List<GameObject> _inSettingsScenes;
		int _inSettingsScenesCount;

		Dictionary<Constants.SceneName, GameObject> _scenes;
		LinkedList<Constants.SceneName> _showList;

		Transform _root;

        //싱글톤 객체 플래그
        bool _InitSingletonObj = false;

        #region Initialize
        protected override void Init()
        {
            for (int i = 0; i < Display.displays.Length; i++)
                if (Display.displays.Length > 1)
                    Display.displays[i].Activate();

            _inSettingsScenes = new List<GameObject>();
            _scenes = new Dictionary<Constants.SceneName, GameObject>();
            _showList = new LinkedList<Constants.SceneName>();

            _root = GetComponent<Transform>();

            if (!_InitSingletonObj)
                StartCoroutine(Load_Singleton());
            else
                LoadStartScene();
        }

        void LoadStartScene()
		{
            if (startScene == Constants.SceneName.None)
                return;

			var sceneName = startScene;
			var scene = GetRoot(sceneName);
			if (scene == null)
			{
				var fullpath = string.Format("Scenes/" + gameName.ToString() +"Scenes/" +"{0}", sceneName);
				StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath, o => OnPostLoadProcess(o)));
			}
		}

		protected override void Release()
		{
			UnloadAll();
		}

#endregion
        //테이블 사운드를 여기서 로딩함.
        IEnumerator Load_Singleton()
        {
            var gameModel = new GameModel();
            gameModel.Setup();

            yield return StartCoroutine(TableManager.Instance.Load());

            while (!TableManager.Instance.IsComplete)
                yield return new WaitForSeconds(0.1f);

            yield return StartCoroutine(SoundManager.Instance.Setup());

            while (!SoundManager.Instance.IsComplete)
                yield return new WaitForSeconds(0.1f);

            yield return StartCoroutine(ModuleManager.Instance.LoadAll());

            LoadStartScene();

//#if !UNITY_EDITOR
//            using (RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
//            {
//                string path = Application.dataPath.Replace("Curling_Data", "LegendStart.exe");
//                path = path.Replace("/", "\\");
//                key.SetValue("LegendCurling", path);
//            }
//#endif
            _InitSingletonObj = true;
        }

		GameObject GetRoot(Constants.SceneName sceneName)
		{
			GameObject scene;
			_scenes.TryGetValue(sceneName, out scene);
			return scene;
		}

		public T GetScript<T>(Constants.SceneName sceneName)
		{
			var scene = GetRoot(sceneName);
			if (scene != null)
				return scene.GetComponent<T>();

			return default(T);
		}

		public T GetScript<T>()
		{
			var scene = GetRoot(EnumExtensions.Parse<Constants.SceneName>(typeof(T).Name.Replace("Scene", "")));
			if (scene != null)
				return scene.GetComponent<T>();

			return default(T);
		}

#region Load/Unload

		public void Load(Constants.SceneName sceneName, bool unload = true)
		{
            LoadRoot(sceneName, unload);
		}

		void LoadRoot(Constants.SceneName sceneName, bool unload)
		{
			if (unload)
				UnloadAll();

			nowScene = sceneName;
			var scene = GetRoot(sceneName);
			if (scene == null)
			{
				var fullpath = string.Format("Scenes/" + gameName.ToString() + "Scenes/" + "{0}", sceneName);
				StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath, o => OnPostLoadProcess(o)));
			}
			else
			{
                var sceneScript = scene.GetComponent<IScene>();
                SetupScene(scene, sceneScript.ContentsList);
			}
		}

		void OnPostLoadProcess(Object o)
		{
			var scene = Instantiate(o) as GameObject;

			var sceneScript = scene.GetComponent<IScene>();
			scene.name = sceneScript.sceneName.ToString();
			scene.transform.SetParent(_root);

			_scenes.Add(sceneScript.sceneName, scene);
			SetupScene(scene, sceneScript.ContentsList);
		}

		void SetupScene(GameObject scene, List<string> enterContentList)
		{
		    _inSettingsScenes.Add(scene);
			_inSettingsScenesCount++;

			scene.SetActive(true);

			var sceneScript = scene.GetComponent<IScene>();
			sceneScript.LoadAssets(gameName.ToString(), enterContentList, 
				() =>
				{
					BringToTop(sceneScript.sceneName);
					_inSettingsScenes.Remove(scene);
                    ModuleManager.Instance.SceneLoadComplete();
				});
		}

		public void Unload(Constants.SceneName sceneName)
		{
			var scene = GetRoot(sceneName);
			if (scene != null)
			{
				scene.GetComponent<IScene>().Unload(gameName.ToString());

				_scenes.Remove(sceneName);
				_showList.Remove(sceneName);

				var fullpath = string.Format("Scenes/" + gameName.ToString() + "Scenes/" + "{0}", sceneName);
				if (ResourceLoader.isAlive)
					ResourceLoader.Instance.Unload(fullpath);
			}
		}

		public void UnloadAll()
		{
			LinkedListNode<Constants.SceneName> node;

			while (true)
			{
				node = _showList.First;
				if (node == null)
					break;
				
				Unload(node.Value);
			}
		}

#endregion

		public void Show(Constants.SceneName sceneName)
		{
			BringToTop(sceneName);
		}

		void BringToTop(Constants.SceneName sceneName)
		{
			_showList.Remove(sceneName);
			_showList.AddFirst(sceneName);

			var scene = GetRoot(sceneName);
			scene.transform.SetAsFirstSibling();
		}
	}
}
