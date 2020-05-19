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
    public class GameBlockBreakContent : IFantaBoxContent
    {
        public GameBlockBreak_BlockBatch blockBatch;

        ObjectPool mNormalBlockPool = null;
        ObjectPool mBombBlockPool = null;

        ObjectPool mBlockStarEffPool = null;
        ObjectPool mBlockIceEffPool = null;

        public List<GameBlockBreak_Block> blockList = new List<GameBlockBreak_Block>();

        GameModel gm;

        protected override void OnLoadStart()
        {
            gm = Model.First<GameModel>();
            StartCoroutine(Cor_Load());
        }

        IEnumerator Cor_Load()
        {
            yield return null;
            // 슈팅게임 씬 로드
            ObjectList = new List<GameObject>();
            string scenename = "GameBlockBreak";
            var fullpath = string.Format("Scenes/FantaScenes/Fanta/{0}", scenename);
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath, o => blockBatch = OnPostLoadProcess(o).GetComponent<GameBlockBreak_BlockBatch>()));

            // 슈팅게임 관련 리소스 로딩
            string prefabname = scenename + "/Object/Block_Normal";
            var fullpath_prefab = string.Format("Prefab/{0}", prefabname);
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath_prefab, o => mNormalBlockPool = OnPostLoadProcess(o, 250)));

            prefabname = scenename + "/Object/Block_Boom";
            fullpath_prefab = string.Format("Prefab/{0}", prefabname);
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath_prefab, o => mBombBlockPool = OnPostLoadProcess(o, 10)));

            prefabname = scenename + "/Object/Block_Star";
            fullpath_prefab = string.Format("Prefab/{0}", prefabname);
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath_prefab, o => mBlockStarEffPool = OnPostLoadProcess(o, 100)));

            prefabname = scenename + "/Object/Block_Ice_Effect";
            fullpath_prefab = string.Format("Prefab/{0}", prefabname);
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath_prefab, o => mBlockIceEffPool = OnPostLoadProcess(o, 10)));

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

            SoundManager.Instance.PlaySound((int)SoundType_GameBGM.BlockBreak);
        }

        protected override void OnExit()
        {
            Message.Send<PoolObjectMsg>(new PoolObjectMsg());
            ObjectListOff();
        }

        protected override void OnAddMessage()
        {
            Message.AddListener<BlockDataMsg>(OnBlockDataMsg);
            Message.AddListener<FreezeBlockMsg>(OnFreezeBlockMsg);
            Message.AddListener<Event.GameObjectDeActiveMessage>(OnDeactive);
        }

        protected override void OnRemoveMessage()
        {
            Message.RemoveListener<BlockDataMsg>(OnBlockDataMsg);
            Message.RemoveListener<FreezeBlockMsg>(OnFreezeBlockMsg);
            Message.RemoveListener<Event.GameObjectDeActiveMessage>(OnDeactive);
        }

        protected override void OnPlay()
        {
            Message.Send<MultiTouchMsg>(new MultiTouchMsg());
            blockBatch.CreateNewBlockMap();
        }

        protected override void OnEnd()
        {
            blockList.Clear();
            SoundManager.Instance.StopSound((int)SoundType_GameBGM.BlockBreak);
        }

        protected override void OnHit(GameObject obj)
        {
            GameBlockBreak_Block obj_script = obj.transform.GetComponent<GameBlockBreak_Block>();

            if (obj_script != null)
                obj_script.Hit();
        }

        void OnBlockDataMsg(BlockDataMsg msg)
        {
            StartCoroutine(CreateBlock(msg.Data, msg.DataVec, msg.DataX, msg.DataY));
        }

        IEnumerator CreateBlock(List<string> data, List<Vector3> dataVec, List<int> dataX, List<int> dataY)
        {
            for(int index = 0; index < data.Count; index++)
            {
                GameBlockBreak_Block tempBlock = null;

                if (data[index] == "0")
                    continue;

                else if (data[index] == "1")
                {
                    tempBlock = mNormalBlockPool.GetObject(mNormalBlockPool.transform).GetComponent<GameBlockBreak_Block>();
                    tempBlock.transform.position = dataVec[index];
                }
                else if (data[index] == "2")
                {
                    tempBlock = mBombBlockPool.GetObject(mBombBlockPool.transform).GetComponent<GameBlockBreak_Block>();
                    tempBlock.transform.position = dataVec[index];
                }

                if (tempBlock != null)
                {
                    tempBlock.m_nX = dataX[index];
                    tempBlock.m_nY = dataY[index];

                    blockList.Add(tempBlock);
                    tempBlock = null;
                }

                blockBatch.blockList = blockList;
            }

            SoundManager.Instance.PlaySound((int)SoundType_GameFX.BlockBreak_InitBubble);

            yield return null;
        }

        void OnFreezeBlockMsg(FreezeBlockMsg msg)
        {
            blockBatch.Freeze(msg.X, msg.Y);
        }

        void OnDeactive(Event.GameObjectDeActiveMessage msg)
        {
            ObjectPool tempPool = null;

            if (msg.TypeIndex == (int)BlockType.Normal)
                tempPool = mNormalBlockPool;
            else if (msg.TypeIndex == (int)BlockType.Bomb)
                tempPool = mBombBlockPool;

            blockList.Remove(msg.myObject.GetComponent<GameBlockBreak_Block>());

            tempPool.PoolObject(msg.myObject);

            if (blockList.Count == 0)
            {
                blockList.Clear();
                if(IsPlaying())
                    StartCoroutine(blockBatch.BlockUpdate());
            }
        }
    }
}