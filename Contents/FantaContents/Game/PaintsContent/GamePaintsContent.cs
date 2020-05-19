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

public enum PaintType
{
    BluePaint,
    GreenPaint,
    GreyPaint,
    PurplePaint,
    RedPaint,
    YellowPaint,
}

namespace CellBig.Contents
{
    public class GamePaintsContent : IFantaBoxContent
    {
        GamePaints_ObjectControl gamePaints_ObjectControl;

        ObjectPool bluePaintPool = null;
        ObjectPool greenPaintPool = null;
        ObjectPool greyPaintPool = null;
        ObjectPool purplePaintPool = null;
        ObjectPool redPaintPool = null;
        ObjectPool yellowPaintPool = null;

        List<GamePaints_Paint> paintList = new List<GamePaints_Paint>();

        Coroutine Cor_GameLogic;

        GameModel gm;

        public int maxSpawnCount = 15;
        public int currentSpawnCount = 0;

        protected override void OnLoadStart()
        {
            gm = Model.First<GameModel>();
            StartCoroutine(Cor_Load());
        }

        IEnumerator Cor_Load()
        {
            yield return null;

            ObjectList = new List<GameObject>();
            string scenename = "GamePaints";
            var fullpath = string.Format("Scenes/FantaScenes/Fanta/{0}", scenename);
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath, o =>
                                                gamePaints_ObjectControl = OnPostLoadProcess(o).GetComponent<GamePaints_ObjectControl>()));

            string prefabname = scenename + "/Object/BluePaint";
            var fullpath_prefab = string.Format("Prefab/{0}", prefabname);
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath_prefab, o => bluePaintPool = OnPostLoadProcess(o, 10)));

            prefabname = scenename + "/Object/GreenPaint";
            fullpath_prefab = string.Format("Prefab/{0}", prefabname);
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath_prefab, o => greenPaintPool = OnPostLoadProcess(o, 10)));

            prefabname = scenename + "/Object/GreyPaint";
            fullpath_prefab = string.Format("Prefab/{0}", prefabname);
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath_prefab, o => greyPaintPool = OnPostLoadProcess(o, 10)));

            prefabname = scenename + "/Object/PurplePaint";
            fullpath_prefab = string.Format("Prefab/{0}", prefabname);
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath_prefab, o => purplePaintPool = OnPostLoadProcess(o, 10)));

            prefabname = scenename + "/Object/RedPaint";
            fullpath_prefab = string.Format("Prefab/{0}", prefabname);
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath_prefab, o => redPaintPool = OnPostLoadProcess(o, 10)));

            prefabname = scenename + "/Object/YellowPaint";
            fullpath_prefab = string.Format("Prefab/{0}", prefabname);
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath_prefab, o => yellowPaintPool = OnPostLoadProcess(o, 10)));

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

            SoundManager.Instance.PlaySound((int)SoundType_GameBGM.Paints);
        }

        protected override void OnExit()
        {
            Message.Send<PoolObjectMsg>(new PoolObjectMsg());
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
            Cor_GameLogic = StartCoroutine(CreatePaints());
        }

        IEnumerator CreatePaints()
        {
            while (true)
            {
                while (currentSpawnCount >= maxSpawnCount)
                    yield return null;

                GamePaints_Paint tempPaint = null;

                tempPaint = SelectPaint();

                paintList.Add(tempPaint);
                gamePaints_ObjectControl.paintList = paintList;

                yield return StartCoroutine(gamePaints_ObjectControl.SetSpawn(tempPaint));

                float Random_Dealy = UnityEngine.Random.Range(0.5f, 1f);

                yield return new WaitForSeconds(Random_Dealy);
            }
        }

        GamePaints_Paint SelectPaint()
        {
            GamePaints_Paint tempPaint = null;

            int randomDragonColor = UnityEngine.Random.Range(0, Enum.GetNames(typeof(PaintType)).Length);

            PaintType dragonColor = (PaintType)randomDragonColor;

            switch (dragonColor)
            {
                case PaintType.BluePaint:
                    tempPaint = bluePaintPool.GetObject(bluePaintPool.transform).GetComponent<GamePaints_Paint>();
                    break;
                case PaintType.GreenPaint:
                    tempPaint = greenPaintPool.GetObject(greenPaintPool.transform).GetComponent<GamePaints_Paint>();
                    break;
                case PaintType.GreyPaint:
                    tempPaint = greyPaintPool.GetObject(greyPaintPool.transform).GetComponent<GamePaints_Paint>();
                    break;
                case PaintType.PurplePaint:
                    tempPaint = purplePaintPool.GetObject(purplePaintPool.transform).GetComponent<GamePaints_Paint>();
                    break;
                case PaintType.RedPaint:
                    tempPaint = redPaintPool.GetObject(redPaintPool.transform).GetComponent<GamePaints_Paint>();
                    break;
                case PaintType.YellowPaint:
                    tempPaint = yellowPaintPool.GetObject(yellowPaintPool.transform).GetComponent<GamePaints_Paint>();
                    break;
            }

            currentSpawnCount++;

            return tempPaint;
        }

        protected override void OnHit(GameObject obj)
        {
            GamePaints_Paint obj_script = obj.transform.GetComponent<GamePaints_Paint>();

            if (obj_script != null)
                obj_script.Hit();
        }

        protected override void OnEnd()
        {
            StopCoroutine(Cor_GameLogic);

            Cor_GameLogic = null;

            paintList.Clear();

            SoundManager.Instance.StopSound((int)SoundType_GameBGM.Paints);
        }

        void OnDeactive(Event.GameObjectDeActiveMessage msg)
        {
            ObjectPool tempPool = null;

            if (msg.TypeIndex == (int)PaintType.BluePaint)
                tempPool = bluePaintPool;
            else if (msg.TypeIndex == (int)PaintType.GreenPaint)
                tempPool = greenPaintPool;
            else if (msg.TypeIndex == (int)PaintType.GreyPaint)
                tempPool = greyPaintPool;
            else if (msg.TypeIndex == (int)PaintType.PurplePaint)
                tempPool = purplePaintPool;
            else if (msg.TypeIndex == (int)PaintType.RedPaint)
                tempPool = redPaintPool;
            else if (msg.TypeIndex == (int)PaintType.YellowPaint)
                tempPool = yellowPaintPool;

            tempPool.PoolObject(msg.myObject);

            paintList.Remove(msg.myObject.GetComponent<GamePaints_Paint>());
            gamePaints_ObjectControl.paintList = paintList;

            currentSpawnCount--;
        }
    }
}