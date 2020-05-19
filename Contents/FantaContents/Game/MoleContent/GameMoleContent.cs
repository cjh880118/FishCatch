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

public enum MoleType
{
    Mole_Normal,
    Mole_Bomb,
    Mole_Check,
    Mole_Dr,
    Mole_Eyebrow,
    Mole_Hair,
    Mole_Hat,
    Mole_Pickaxe,
    Mole_Radio,
    Mole_Sunglass,
    Mole_Tooth,
}

public enum MoleState
{
    Show,
    Hide,
    Fake,
    Max,
}

namespace CellBig.Contents
{
    public class GameMoleContent : IFantaBoxContent
    {
        GameMole_ObjectControl gameMole_ObjectControl;

        public ObjectPool[] molePools;

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
            string scenename = "GameMole";
            var fullpath = string.Format("Scenes/FantaScenes/Fanta/{0}", scenename);
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath, o =>
                                gameMole_ObjectControl = OnPostLoadProcess(o).GetComponent<GameMole_ObjectControl>()));

            string prefabname = null;
            string fullpath_prefab = null;

            molePools = new ObjectPool[Enum.GetNames(typeof(MoleType)).Length];

            for (int index = 0; index < molePools.Length; index++)
            {
                prefabname = scenename + "/Object/" + ((MoleType)index).ToString();
                fullpath_prefab = string.Format("Prefab/{0}", prefabname);
                yield return StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath_prefab, o => molePools[index] = OnPostLoadProcess(o, 10)));
            }

            SetLoadComplete();
        }
        protected override void OnEnter()
        {
            // 게임 시작시 Ready 연출을 시작해 줍니다.
            ObjectListOn();

            RequestContentEnter<ReadyContent>();
            RequestContentEnter<GlobalContent>();

            Message.Send<UI.Event.FadeOutMsg>(new UI.Event.FadeOutMsg());
            Message.Send<MainCameraMsg>(new MainCameraMsg(mainCamera));

            SoundManager.Instance.PlaySound((int)SoundType_GameBGM.Mole);
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
            Cor_GameLogic = StartCoroutine(Cor_PlayContent_Mole());
        }

        IEnumerator Cor_PlayContent_Mole()
        {
            while (true)
            {
                int randomMoleIndex = UnityEngine.Random.Range(0, Enum.GetNames(typeof(MoleType)).Length);

                GameMole_Mole tempMole = null;

                if (gameMole_ObjectControl.IsPossibleCreate())
                {
                    tempMole = molePools[randomMoleIndex].GetObject(molePools[randomMoleIndex].transform).GetComponent<GameMole_Mole>();
                    gameMole_ObjectControl.FindHole(tempMole);

                    float Random_Dealy = UnityEngine.Random.Range(0.5f, 1f);
                    yield return new WaitForSeconds(Random_Dealy);
                }

                yield return null;
            }
        }

        protected override void OnHit(GameObject obj)
        {
            GameMole_Mole obj_script = obj.transform.GetComponent<GameMole_Mole>();

            if (obj_script != null)
            {
                obj_script.Hit();
                gameMole_ObjectControl.ActiveHammer(obj_script);
            }
        }

        protected override void OnEnd()
        {
            StopCoroutine(Cor_GameLogic);
            Cor_GameLogic = null;

            SoundManager.Instance.StopSound((int)SoundType_GameBGM.Mole);
        }

        void OnDeactive(Event.GameObjectDeActiveMessage msg)
        {
            ObjectPool tempPool = null;

            if (msg.TypeIndex == (int)MoleType.Mole_Bomb)
                tempPool = molePools[(int)MoleType.Mole_Bomb];

            else if (msg.TypeIndex == (int)MoleType.Mole_Check)
                tempPool = molePools[(int)MoleType.Mole_Check];

            else if (msg.TypeIndex == (int)MoleType.Mole_Dr)
                tempPool = molePools[(int)MoleType.Mole_Dr];

            else if (msg.TypeIndex == (int)MoleType.Mole_Eyebrow)
                tempPool = molePools[(int)MoleType.Mole_Eyebrow];

            else if (msg.TypeIndex == (int)MoleType.Mole_Hair)
                tempPool = molePools[(int)MoleType.Mole_Hair];

            else if (msg.TypeIndex == (int)MoleType.Mole_Hat)
                tempPool = molePools[(int)MoleType.Mole_Hat];

            else if (msg.TypeIndex == (int)MoleType.Mole_Normal)
                tempPool = molePools[(int)MoleType.Mole_Normal];

            else if (msg.TypeIndex == (int)MoleType.Mole_Pickaxe)
                tempPool = molePools[(int)MoleType.Mole_Pickaxe];

            else if (msg.TypeIndex == (int)MoleType.Mole_Radio)
                tempPool = molePools[(int)MoleType.Mole_Radio];

            else if (msg.TypeIndex == (int)MoleType.Mole_Sunglass)
                tempPool = molePools[(int)MoleType.Mole_Sunglass];

            else if (msg.TypeIndex == (int)MoleType.Mole_Tooth)
                tempPool = molePools[(int)MoleType.Mole_Tooth];

            tempPool.PoolObject(msg.myObject);
        }
    }
}