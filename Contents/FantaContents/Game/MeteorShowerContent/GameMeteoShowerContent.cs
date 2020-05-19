using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using CellBig.Models;
using CellBig;
using CellBig.UI.Event;
using CellBig.Contents.Event;
using CellBig.Common;
using CellBig.Module;
using CellBig.Constants;

using DG.Tweening;
using Rect = UnityEngine.Rect;
using OpenCVForUnity;

public enum MeteoType
{
    Jupiter,
    Mars,
    Mercury,
    Neptune,
    Pluto,
}

namespace CellBig.Contents
{
    public class GameMeteoShowerContent : IFantaBoxContent
    {
        GameMeteoShower_ObjectControl gameMeteoShower_ObjectControl;

        ObjectPool jupiterPool;
        ObjectPool marsPool;
        ObjectPool mercuryPool;
        ObjectPool neptunePool;
        ObjectPool plutoPool;

        Coroutine Cor_GameLogic;

        GameModel gm;

        protected override void OnLoadStart()
        {
            gm = Model.First<GameModel>();
            StartCoroutine(Cor_Load());
        }

        IEnumerator Cor_Load()
        {
            yield return null;
            ObjectList = new List<GameObject>();
            string scenename = "GameMeteoShower";
            var fullpath = string.Format("Scenes/FantaScenes/Fanta/{0}", scenename);
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath, o =>
                                gameMeteoShower_ObjectControl = OnPostLoadProcess(o).GetComponent<GameMeteoShower_ObjectControl>()));

            string prefabname = scenename + "/JupiterObj";
            var fullpath_prefab = string.Format("Prefab/{0}", prefabname);
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath_prefab, o => jupiterPool = OnPostLoadProcess(o, 10)));

            prefabname = scenename + "/MarsObj";
            fullpath_prefab = string.Format("Prefab/{0}", prefabname);
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath_prefab, o => marsPool = OnPostLoadProcess(o, 10)));

            prefabname = scenename + "/MercuryObj";
            fullpath_prefab = string.Format("Prefab/{0}", prefabname);
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath_prefab, o => mercuryPool = OnPostLoadProcess(o, 10)));

            prefabname = scenename + "/NeptuneObj";
            fullpath_prefab = string.Format("Prefab/{0}", prefabname);
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath_prefab, o => neptunePool = OnPostLoadProcess(o, 10)));

            prefabname = scenename + "/PlutoObj";
            fullpath_prefab = string.Format("Prefab/{0}", prefabname);
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath_prefab, o => plutoPool = OnPostLoadProcess(o, 10)));


            SetLoadComplete();
        }
        protected override void OnEnter()
        {
            // 게임 시작시 Ready 연출을 시작해 줍니다.
            ObjectListOn();

            RequestContentEnter<GlobalContent>();
            RequestContentEnter<ReadyContent>();

            Message.Send<UI.Event.FadeOutMsg>(new UI.Event.FadeOutMsg());
            Message.Send<MainCameraMsg>(new MainCameraMsg(mainCamera));

            SoundManager.Instance.PlaySound((int)SoundType_GameBGM.MeteoShower);
        }

        protected override void OnExit()
        {
            ObjectListOff();
        }

        protected override void OnAddMessage()
        {
            Message.AddListener<Event.GameObjectDeActiveMessage>(OnDeactive);
        }

        protected override void OnRemoveMessage()
        {
            Message.RemoveListener<Event.GameObjectDeActiveMessage>(OnDeactive);
        }

        protected override void OnPlay()
        {
            Message.Send<MultiTouchMsg>(new MultiTouchMsg());
            Cor_GameLogic = StartCoroutine(Cor_PlayContent());
        }

        IEnumerator Cor_PlayContent()
        {
            while(true)
            {
                yield return StartCoroutine(SelectCreateMeteo());

                float Random_Dealy = UnityEngine.Random.Range(0.5f, 1f);

                yield return new WaitForSeconds(Random_Dealy);
            }
        }

        IEnumerator SelectCreateMeteo()
        {
            int randomObject = UnityEngine.Random.Range(0, Enum.GetNames(typeof(MeteoType)).Length);

            MeteoType meteoType = (MeteoType)randomObject;

            if (meteoType == MeteoType.Jupiter)
                jupiterPool.GetObject(jupiterPool.transform);
            else if (meteoType == MeteoType.Mars)
                marsPool.GetObject(marsPool.transform);
            else if (meteoType == MeteoType.Mercury)
                mercuryPool.GetObject(mercuryPool.transform);
            else if (meteoType == MeteoType.Neptune)
                neptunePool.GetObject(neptunePool.transform);
            else if (meteoType == MeteoType.Pluto)
                plutoPool.GetObject(plutoPool.transform);

            yield return null;
        }

        protected override void OnHit(GameObject obj)
        {
            GameMeteoShower_Meteo obj_script = obj.transform.GetComponent<GameMeteoShower_Meteo>();

            if (Vector3.Distance(mainCamera.transform.position, obj_script.transform.position) < 100f)
            {
                if (obj_script != null)
                    obj_script.Hit();
            }
        }

        protected override void OnEnd()
        {
            StopCoroutine(Cor_GameLogic);
            Cor_GameLogic = null;

            SoundManager.Instance.StopSound((int)SoundType_GameBGM.MeteoShower);
        }

        void OnDeactive(Event.GameObjectDeActiveMessage msg)
        {
            ObjectPool tempPool = null;

            if (msg.TypeIndex == (int)MeteoType.Jupiter)
                tempPool = jupiterPool;
            else if (msg.TypeIndex == (int)MeteoType.Mars)
                tempPool = marsPool;
            else if (msg.TypeIndex == (int)MeteoType.Mercury)
                tempPool = mercuryPool;
            else if (msg.TypeIndex == (int)MeteoType.Neptune)
                tempPool = neptunePool;
            else if (msg.TypeIndex == (int)MeteoType.Pluto)
                tempPool = plutoPool;

            tempPool.PoolObject(msg.myObject);
        }
    }
}