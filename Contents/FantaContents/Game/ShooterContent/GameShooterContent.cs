using UnityEngine;
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

namespace JHchoi.Contents
{
    public class GameShooterContent : IFantaBoxContent
    {
        Vector3[] StartPos;
        ObjectPool mClayPool = null;

        Coroutine Cor_GameLogic = null;
        Coroutine Cor_InputMouse = null;

        protected override void OnLoadStart()
        {
            StartPos = new Vector3[3]
            { new Vector3(-0.75f,-2.44f,-0.867f), new Vector3(0,-2.44f,-0.867f), new Vector3(0.75f,-2.44f,-0.867f)};
            StartCoroutine(Cor_Load());
        }

        IEnumerator Cor_Load()
        {
            yield return null;
            // 슈팅게임 씬 로드
            ObjectList = new List<GameObject>();
            string scenename = "GameShooter";
            var fullpath = string.Format("Scenes/FantaScenes/Fanta/{0}", scenename);
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath, o => OnPostLoadProcess(o)));

            // 슈팅게임 관련 리소스 로딩
            string prefabname = scenename + "/Clay";
            var fullpath_prefab = string.Format("Prefab/{0}", prefabname);           
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath_prefab, o => mClayPool = OnPostLoadProcess(o, 5)));
            
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
            // 실제 로직을 돌리고
            SoundManager.Instance.PlaySound((int)SoundType_GameBGM.GameShooter);
            Cor_GameLogic =  StartCoroutine(Cor_PlayContent_Shooter());
            Cor_InputMouse = StartCoroutine(Cor_InputMouseCheck());
        }

        IEnumerator Cor_PlayContent_Shooter()
        {
            while (true)
            {
                int CountRandomObject = Random.Range(1, 2);
                for (int i = 0; i < CountRandomObject; i++)
                {
                    Game_Shooter_Clay ShooterObj = mClayPool.GetObject(mClayPool.transform).GetComponent<Game_Shooter_Clay>();
                    ShooterObj.transform.position = StartPos[Random.Range(0, StartPos.Length)];
                    ShooterObj.Active();
                }

                int Random_Dealy = Random.Range(1, 3);
                yield return new WaitForSeconds(Random_Dealy);
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
            SoundManager.Instance.StopSound((int)SoundType_GameBGM.GameShooter);
            StopCoroutine(Cor_GameLogic);
            StopCoroutine(Cor_InputMouse);
            Cor_GameLogic = null;
        }

        protected override void OnHit(GameObject obj)
        {
            Game_Shooter_Clay obj_script = obj.transform.GetComponent<Game_Shooter_Clay>();
            if (obj_script != null)
            {
                obj_script.Hit();
                //사운드 재생
                SoundManager.Instance.PlaySound((int)SoundType_GameFX.Slug_Explosion);
                //점수
                // CameraShake
               // mainCamera.DOShakeRotation(a, b,c,d, false);
            }
        }

        void OnDeactive(Event.GameObjectDeActiveMessage msg)
        {
            mClayPool.PoolObject(msg.myObject);
        }
    }
}
