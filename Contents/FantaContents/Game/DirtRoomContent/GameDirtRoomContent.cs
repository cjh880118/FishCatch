using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using JHchoi.Models;
using JHchoi;
using JHchoi.UI.Event;
using JHchoi.Contents.Event;
using JHchoi.Common;
using JHchoi.Module;
using JHchoi.Constants;

using DG.Tweening;
using Rect = UnityEngine.Rect;
using OpenCVForUnity;

public enum DirtRoom_EnvironmentObjectType
{
    Doll,
    Plastic,
    Paint_Movable,
    Paint_Moved,
    Paint_Stain,
}

public enum DirtRoom_FootType
{
    Foot,
    SpriteFoot,
}

public enum DirtRoom_SpriteFootColor
{
    Blue,
    Green,
    Red,
    Yellow,
}

public enum DirtRoom_FootSprite
{
    FootSprite_Shoes,
    FootSprite1_Frog,
    FootSprite2_Horse,
    FootSprite3_Dog,
    FootSprite4_Walker,
    FootSprite5_Camel,
    FootSprite6_Bear,
    FootSprite7_Duck,
    FootSprite8_Dog2,
    FootSprite9_Walker2,
}

namespace JHchoi.Contents
{
    public class GameDirtRoomContent : IFantaBoxContent
    {
        GameDirtRoom_ObjectControl gameDirtRoom_ObjectControl;

        ObjectPool footPool;
        ObjectPool spriteFootPool;

        GameDirtRoom_Foot tempFoot = null;
        GameDirtRoom_SpriteFoot tempSpriteFoot = null;

        GameModel gm;

        DirtRoom_SpriteFootColor spriteFootColor;
        DirtRoom_FootSprite footSprite;

        bool isStainFoot;
        int randomFootSpriteIndex;

        protected override void OnLoadStart()
        {
            gm = Model.First<GameModel>();
            StartCoroutine(Cor_Load());
        }

        IEnumerator Cor_Load()
        {
            yield return null;

            ObjectList = new List<GameObject>();
            string scenename = "GameDirtRoom";
            var fullpath = string.Format("Scenes/FantaScenes/Fanta/{0}", scenename);
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath, o =>
                                                gameDirtRoom_ObjectControl = OnPostLoadProcess(o).GetComponent<GameDirtRoom_ObjectControl>()));

            string prefabname = scenename + "/Object/Foot";
            var fullpath_prefab = string.Format("Prefab/{0}", prefabname);
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath_prefab, o => footPool = OnPostLoadProcess(o, 6)));

            prefabname = scenename + "/Object/SpriteFoot";
            fullpath_prefab = string.Format("Prefab/{0}", prefabname);
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath_prefab, o => spriteFootPool = OnPostLoadProcess(o, 10)));

            SetLoadComplete();

            Debug.Log("LoadCom");

        }
        protected override void OnEnter()
        {
            // 게임 시작시 Ready 연출을 시작해 줍니다.
            ObjectListOn();

            RequestContentEnter<GlobalContent>();
            RequestContentEnter<ReadyContent>();

            Message.Send<UI.Event.FadeOutMsg>(new UI.Event.FadeOutMsg());
            Message.Send<MainCameraMsg>(new MainCameraMsg(mainCamera));

            SoundManager.Instance.PlaySound((int)SoundType_GameBGM.DirtRoom);
        }

        protected override void OnExit()
        {
            isStainFoot = false;

            Message.Send<PoolObjectMsg>(new PoolObjectMsg());

            ObjectListOff();
            ReloadObject();
        }

        void ReloadObject()
        {
            ObjectList.Remove(gameDirtRoom_ObjectControl.gameObject);
            Destroy(gameDirtRoom_ObjectControl.gameObject);

            string scenename = "GameDirtRoom";
            var fullpath = string.Format("Scenes/FantaScenes/Fanta/{0}", scenename);
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath, o =>
                                                gameDirtRoom_ObjectControl = OnPostLoadProcess(o).GetComponent<GameDirtRoom_ObjectControl>()));
        }

        protected override void OnAddMessage()
        {
            Message.AddListener<StainFootMsg>(OnStainFootMsg);
            Message.AddListener<Event.GameObjectDeActiveMessage>(OnDeactive);
        }

        protected override void OnRemoveMessage()
        {
            Message.RemoveListener<StainFootMsg>(OnStainFootMsg);
            Message.RemoveListener<Event.GameObjectDeActiveMessage>(OnDeactive);
        }

        protected override void OnPlay()
        {
            Message.Send<MultiTouchMsg>(new MultiTouchMsg());
        }

        void OnStainFootMsg(StainFootMsg msg)
        {
            isStainFoot = msg.IsStain;

            spriteFootColor = msg.SpriteFootColor;
            randomFootSpriteIndex = UnityEngine.Random.Range(0, Enum.GetNames(typeof(DirtRoom_FootSprite)).Length);
        }

        protected override void OnHit(GameObject obj)
        {
            if (isDelayCheck)
            {
                contentDelayCheckCor = StartCoroutine(CheckDelay());

                tempFoot = footPool.GetObject(footPool.transform).GetComponent<GameDirtRoom_Foot>();
         
                if (tempFoot != null)
                    tempFoot.Hit();

                GameDirtRoom_Paint tempPaint = obj.GetComponent<GameDirtRoom_Paint>();

                if (tempPaint != null)
                    tempPaint.Hit();

                if (isStainFoot)
                {
                    tempSpriteFoot = spriteFootPool.GetObject(spriteFootPool.transform).GetComponent<GameDirtRoom_SpriteFoot>();
                    gameDirtRoom_ObjectControl.SetSpriteFoot(tempSpriteFoot, randomFootSpriteIndex, spriteFootColor);
                }
            }
        }

        protected override void HitPoint(Vector3 hitPoint)
        {
            tempFoot.transform.position = hitPoint;

            if (isStainFoot && tempSpriteFoot != null)
            {
                tempSpriteFoot.transform.position = new Vector3(hitPoint.x, 0.1f, hitPoint.z);
                tempSpriteFoot = null;
            }
        }

        protected override void OnEnd()
        {
            SoundManager.Instance.StopSound((int)SoundType_GameBGM.DirtRoom);
        }

        void OnDeactive(Event.GameObjectDeActiveMessage msg)
        {
            if (msg.TypeIndex == (int)DirtRoom_FootType.Foot)
                footPool.PoolObject(msg.myObject);
            else if (msg.TypeIndex == (int)DirtRoom_FootType.SpriteFoot)
                spriteFootPool.PoolObject(msg.myObject);
        }
    }
}