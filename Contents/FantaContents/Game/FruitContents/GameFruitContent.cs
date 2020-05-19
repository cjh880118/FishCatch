using UnityEngine;
using System;
using System.Collections;
using JHchoi.Models;
using JHchoi;
using JHchoi.UI.Event;
using JHchoi.Contents.Event;
using JHchoi.Common;
using DG.Tweening;
using System.Collections.Generic;
using JHchoi.Module;
using JHchoi.Constants;
using DG.Tweening;
using Rect = UnityEngine.Rect;
using OpenCVForUnity;

public enum FruitType
{
    Fruit_Apple,
    Fruit_Banana,
    Fruit_Lemon,
    Fruit_Orange,
    Fruit_Pear,
    Fruit_Melon,
    Fruit_Bomb,
}

public enum FruitDirection
{
    Left,
    Right,
}

namespace JHchoi.Contents
{
    public class GameFruitContent : IFantaBoxContent
    {
        GameFruit_ObjectControl gameFruit_ObjectControl;

        List<GameFruit_Fruit> fruitList = new List<GameFruit_Fruit>();

        Coroutine mCor_GameLogic = null;
        Coroutine mCor_InputMouse = null;

        ObjectPool[] fruitPools;

        protected override void OnLoadStart()
        {
            StartCoroutine(Cor_Load());
        }

        IEnumerator Cor_Load()
        {
            yield return null;
            // 게임 씬 로드
            ObjectList = new List<GameObject>();
            string scenename = "GameFruit";
            var fullpath = string.Format("Scenes/FantaScenes/Fanta/{0}", scenename);
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath, o =>
                                                        gameFruit_ObjectControl = OnPostLoadProcess(o).GetComponent<GameFruit_ObjectControl>()));

            fruitPools = new ObjectPool[Enum.GetNames(typeof(FruitType)).Length];

            for (int index = 0; index < fruitPools.Length; index++)
            {
                string prefabname = scenename + "/Object/" + ((FruitType)index).ToString();
                var fullpath_prefab = string.Format("Prefab/{0}", prefabname);
                yield return StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath_prefab, o => fruitPools[index] = OnPostLoadProcess(o, 5)));
            }

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

            Physics.gravity = new Vector3(0.0f, -300.0f, 0.0f);

            SoundManager.Instance.PlaySound((int)SoundType_GameBGM.Fruit);
        }

        protected override void OnExit()
        {
            Message.Send<PoolObjectMsg>(new PoolObjectMsg());
            fruitList.Clear();
            ObjectListOff();

            Physics.gravity = new Vector3(0.0f, -9.8f, 0.0f);
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
            mCor_GameLogic = StartCoroutine(Cor_GameLogic());
        }

        // 게임로직 코루틴 삽입
        IEnumerator Cor_GameLogic()
        {
            while (true)
            {
                int CountRandomObject = UnityEngine.Random.Range(5, 12);

                for (int i = 0; i < CountRandomObject; i++)
                {
                    int randomFruitIndex = UnityEngine.Random.Range(0, Enum.GetNames(typeof(FruitType)).Length);
                    GameFruit_Fruit tempFruit = fruitPools[randomFruitIndex].GetObject(fruitPools[randomFruitIndex].transform).GetComponent<GameFruit_Fruit>();

                    gameFruit_ObjectControl.SetSpawn(tempFruit);

                    fruitList.Add(tempFruit);
                }

                yield return new WaitForSeconds(4.0f);
                yield return null;
            }
        }

        IEnumerator Cor_InputMouseCheck()
        {
            List<Rect> rects = new List<Rect>();
            while (true)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    //마우스 눌렀을때 해당위치에 Rect생성
                    rects.Clear();
                    Vector2 screenPoint = mainCamera.ScreenToViewportPoint(Input.mousePosition);
                    UnityEngine.Rect mouseRect = new UnityEngine.Rect(screenPoint, new Vector2(0.1f, 0.1f));
                    rects.Add(mouseRect);

                    Message.Send<TouchRectMsg>(new TouchRectMsg(rects));
                }
                yield return null;
            }
        }

        protected override void OnEnd()
        {
            StopCoroutine(mCor_GameLogic);
            mCor_GameLogic = null;

            SoundManager.Instance.StopSound((int)SoundType_GameBGM.Fruit);
        }

        protected override void OnHit(GameObject obj)
        {
            GameFruit_Fruit obj_script = obj.transform.GetComponent<GameFruit_Fruit>();

            if (obj_script != null)
                obj_script.Hit();
        }

        void OnDeactive(Event.GameObjectDeActiveMessage msg)
        {
            ObjectPool tempPool = fruitPools[msg.TypeIndex];
            tempPool.PoolObject(msg.myObject);

            fruitList.Remove(msg.myObject.GetComponent<GameFruit_Fruit>());
        }
    }
}
