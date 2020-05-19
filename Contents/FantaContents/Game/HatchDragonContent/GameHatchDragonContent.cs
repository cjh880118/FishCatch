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

public enum HatchDragonColorType
{
    Blue = 0,
    Green,
    Purple,
    Red,
    Yellow,
}

namespace CellBig.Contents
{
    public class GameHatchDragonContent : IFantaBoxContent
    {
        GameHatchDragon_ObjectControl gameHatchDragon_ObjectControl;

        ObjectPool blueEggPool = null;
        ObjectPool greenEggPool = null;
        ObjectPool purpleEggPool = null;
        ObjectPool redEggPool = null;
        ObjectPool yellowEggPool = null;

        public List<int> isPossibleCreateList = new List<int>();

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
            string scenename = "GameHatchDragon";
            var fullpath = string.Format("Scenes/FantaScenes/Fanta/{0}", scenename);
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath, o =>
                                                gameHatchDragon_ObjectControl = OnPostLoadProcess(o).GetComponent<GameHatchDragon_ObjectControl>()));

            string prefabname = scenename + "/Object/BlueEgg";
            var fullpath_prefab = string.Format("Prefab/{0}", prefabname);
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath_prefab, o => blueEggPool = OnPostLoadProcess(o, 10)));

            prefabname = scenename + "/Object/GreenEgg";
            fullpath_prefab = string.Format("Prefab/{0}", prefabname);
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath_prefab, o => greenEggPool = OnPostLoadProcess(o, 10)));

            prefabname = scenename + "/Object/PurpleEgg";
            fullpath_prefab = string.Format("Prefab/{0}", prefabname);
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath_prefab, o => purpleEggPool = OnPostLoadProcess(o, 10)));

            prefabname = scenename + "/Object/RedEgg";
            fullpath_prefab = string.Format("Prefab/{0}", prefabname);
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath_prefab, o => redEggPool = OnPostLoadProcess(o, 10)));

            prefabname = scenename + "/Object/YellowEgg";
            fullpath_prefab = string.Format("Prefab/{0}", prefabname);
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath_prefab, o => yellowEggPool = OnPostLoadProcess(o, 10)));

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

            SoundManager.Instance.PlaySound((int)SoundType_GameBGM.HatchDragon);
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
            Cor_GameLogic = StartCoroutine(CreateEgg());
        }

        IEnumerator CreateEgg()
        {
            while (true)
            {
                GameHatchDragon_HatchDragon tempDragon = null;

                isPossibleCreateList.Clear();

                yield return null;

                // 전체 스폰 포인트 검색.
                for (int index = 0; index < gameHatchDragon_ObjectControl.spawnPoint.Length; index++)
                {
                    // 스폰 가능한 포인트가 있으면 isPossibleCreateList 에 해당 포인트 인덱스 값 추가.
                    if (gameHatchDragon_ObjectControl.isPossibleSpawn[index])
                        isPossibleCreateList.Add(index);
                }

                // 스폰 가능한 포인트가 없으면 다시 검색.
                if (isPossibleCreateList.Count <= 0)
                        continue;

                // 스폰 가능한 포인트 랜덤 선택.
                int createRandomIndex = UnityEngine.Random.Range(0, isPossibleCreateList.Count);

                // GameHatchDragon_HatchDragon Color Type 랜덤 선택.
                tempDragon = SelectHatchDragon();

                // 선택된 드래곤 오브젝트 세팅 및 스폰 포인트 인덱스 할당.
                gameHatchDragon_ObjectControl.SetSpawn(tempDragon, isPossibleCreateList[createRandomIndex]);

                float Random_Dealy = UnityEngine.Random.Range(1f, 2f);

                yield return new WaitForSeconds(Random_Dealy);
            }
        }

        GameHatchDragon_HatchDragon SelectHatchDragon()
        {
            GameHatchDragon_HatchDragon tempDragon = null;

            int randomDragonColor = UnityEngine.Random.Range(0, Enum.GetNames(typeof(HatchDragonColorType)).Length);

            HatchDragonColorType dragonColor = (HatchDragonColorType)randomDragonColor;

            switch (dragonColor)
            {
                case HatchDragonColorType.Blue:
                    tempDragon = blueEggPool.GetObject(blueEggPool.transform).GetComponent<GameHatchDragon_HatchDragon>();
                    break;
                case HatchDragonColorType.Green:
                    tempDragon = greenEggPool.GetObject(greenEggPool.transform).GetComponent<GameHatchDragon_HatchDragon>();
                    break;
                case HatchDragonColorType.Purple:
                    tempDragon = purpleEggPool.GetObject(purpleEggPool.transform).GetComponent<GameHatchDragon_HatchDragon>();
                    break;
                case HatchDragonColorType.Red:
                    tempDragon = redEggPool.GetObject(redEggPool.transform).GetComponent<GameHatchDragon_HatchDragon>();
                    break;
                case HatchDragonColorType.Yellow:
                    tempDragon = yellowEggPool.GetObject(yellowEggPool.transform).GetComponent<GameHatchDragon_HatchDragon>();
                    break;
            }

            return tempDragon;
        }

        protected override void OnHit(GameObject obj)
        {
            GameHatchDragon_HatchDragon obj_script = obj.transform.GetComponent<GameHatchDragon_HatchDragon>();

            if (obj_script != null)
                obj_script.Hit();
        }

        protected override void OnEnd()
        {
            StopCoroutine(Cor_GameLogic);
            Cor_GameLogic = null;

            SoundManager.Instance.StopSound((int)SoundType_GameBGM.HatchDragon);
        }

        void OnDeactive(Event.GameObjectDeActiveMessage msg)
        {
            ObjectPool tempPool = null;

            if (msg.TypeIndex == (int)HatchDragonColorType.Blue)
                tempPool = blueEggPool;
            else if (msg.TypeIndex == (int)HatchDragonColorType.Green)
                tempPool = greenEggPool;
            else if (msg.TypeIndex == (int)HatchDragonColorType.Purple)
                tempPool = purpleEggPool;
            else if (msg.TypeIndex == (int)HatchDragonColorType.Red)
                tempPool = redEggPool;
            else if (msg.TypeIndex == (int)HatchDragonColorType.Yellow)
                tempPool = yellowEggPool;

            tempPool.PoolObject(msg.myObject);
            gameHatchDragon_ObjectControl.ResetSpawn(msg.OtherInfo);
        }
    }
}