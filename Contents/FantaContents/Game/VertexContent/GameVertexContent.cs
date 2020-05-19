using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

namespace JHchoi.Contents
{
    public class GameVertexContent : IFantaBoxContent
    {
        GameVertex_ObjectControl gameVertex_ObjectControl;

        ObjectPool vertexPool;

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
            string scenename = "GameVertex";
            var fullpath = string.Format("Scenes/FantaScenes/Fanta/{0}", scenename);
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath, o =>
                                gameVertex_ObjectControl = OnPostLoadProcess(o).GetComponent<GameVertex_ObjectControl>()));

            string prefabname = scenename + "/Object/Vertex";
            var fullpath_prefab = string.Format("Prefab/{0}", prefabname);
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath_prefab, o => vertexPool = OnPostLoadProcess(o, 20)));

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

            SoundManager.Instance.PlaySound((int)SoundType_GameBGM.Vertex);
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
            Cor_GameLogic = StartCoroutine(CreateVertex());
        }

        IEnumerator CreateVertex()
        {
            while (true)
            {
                GameVertex_Vertex tempVertex = null;

                isPossibleCreateList.Clear();

                yield return null;

                for (int index = 0; index < gameVertex_ObjectControl.spawnPoint.Length; index++)
                {
                    if (gameVertex_ObjectControl.isPossibleSpawn[index])
                        isPossibleCreateList.Add(index);
                }

                if (isPossibleCreateList.Count <= 0)
                    continue;

                int createRandomIndex = UnityEngine.Random.Range(0, isPossibleCreateList.Count);

                tempVertex = vertexPool.GetObject(vertexPool.transform).GetComponent<GameVertex_Vertex>();

                gameVertex_ObjectControl.SetSpawn(tempVertex, isPossibleCreateList[createRandomIndex]);

                float Random_Dealy = UnityEngine.Random.Range(1f, 2f);

                yield return new WaitForSeconds(Random_Dealy);
            }
        }

        protected override void OnHit(GameObject obj)
        {
            GameVertex_Vertex obj_script = obj.transform.GetComponent<GameVertex_Vertex>();

            if (obj_script != null)
                obj_script.Hit();
        }

        protected override void OnEnd()
        {
            StopCoroutine(Cor_GameLogic);
            Cor_GameLogic = null;

            SoundManager.Instance.StopSound((int)SoundType_GameBGM.Vertex);
        }

        void OnDeactive(Event.GameObjectDeActiveMessage msg)
        {
            vertexPool.PoolObject(msg.myObject);
            gameVertex_ObjectControl.ResetSpawn(msg.TypeIndex);
        }
    }
}