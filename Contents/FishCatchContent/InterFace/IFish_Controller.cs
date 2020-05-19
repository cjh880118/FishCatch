using CellBig.Models;
using CellBig.Module.Detection.CV.Output;
using CellBig.UI.Event;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;
using CellBig.Constants.FishCatch;

namespace CellBig.Contents
{
    public abstract class IFish_Controller : MonoBehaviour
    {
        public IFish[] arrayFish;
        public bool isLoadComplete;
        protected Coroutine[] arrayFishMove;
        protected FishModel fm;
        protected BackGroundObject bgObject;
        protected int salmonCount;
        protected List<GameObject> listGameObject = new List<GameObject>();
        protected int fishIndexNum;

        public virtual void InitFishController(int fishIndex, FishModel fm, BackGroundObject bg, CatchObjectType objectType)
        {
            AddMessage();
            isLoadComplete = false;
            this.fm = fm;
            Debug.Log("IFishContent OnEnter");
            fishIndexNum = fishIndex;
            bgObject = bg;
            StartCoroutine(InitGameObject(fishIndex, bg, objectType));
        }

        IEnumerator InitGameObject(int fishIndex, BackGroundObject bg, CatchObjectType objectType)
        {
            listGameObject.Clear();
            string path = string.Format("Prefab/FishCatch/{0}/{1}", objectType.ToString(), fm.PrefabName(fishIndex));
            yield return StartCoroutine(ResourceLoader.Instance.Load<GameObject>(path,
               o =>
               {
                   int salmonCount = fm.FishCount(fishIndex);
                   arrayFish = new IFish[salmonCount];
                   arrayFishMove = new Coroutine[salmonCount];

                   for (int i = 0; i < salmonCount; i++)
                   {
                       var inGameObject = Instantiate(o) as GameObject;
                       inGameObject.name = o.name + i;
                       inGameObject.transform.parent = this.gameObject.transform;
                       listGameObject.Add(inGameObject);
                       arrayFish[i] = inGameObject.GetComponent<IFish>();
                       arrayFish[i].InitFish(i, fm.FishSizeMax(fishIndex), fm.FishSizeMin(fishIndex), fm.FishSpeedMax(fishIndex), fm.FishSpeedMin(fishIndex), fm.FishCatchDelay(fishIndex), fm.FishViewPosZ(fishIndex));
                       int rndTarget = Random.Range(0, bg.GetTargetCount());
                       arrayFish[i].transform.position = bg.GetTargetPosition(rndTarget);
                       MoveFish(i, rndTarget);
                   }

                   Message.Send<ArrayListObjMsg<IFish>>(new ArrayListObjMsg<IFish>(arrayFish));
               }));

            isLoadComplete = true;
        }

        protected virtual void AddMessage()
        {
            Message.AddListener<CatchPlateSuccessMsg>(CatchPlateSuccess);
            Message.AddListener<FishArriveMsg>(FishArrive);
            Message.AddListener<MissFishMsg>(MissFish);
        }

        public void MoveFish(int index, int targetIndex)
        {
            if (arrayFishMove[index] != null)
            {
                StopCoroutine(arrayFishMove[index]);
                arrayFishMove[index] = null;
            }

            Vector3 target = bgObject.GetTargetPosition(targetIndex);
            arrayFishMove[index] = StartCoroutine(arrayFish[index].MoveTarget(target, targetIndex));
        }

        protected virtual void CatchPlateSuccess(CatchPlateSuccessMsg msg)
        {
            int rndTarget = Random.Range(0, bgObject.GetTargetCount());
            arrayFish[msg.fishIndex].CatchPlate();

            //목적지 향해 이동
            StartCoroutine(RespawnStay(msg.fishIndex, rndTarget, msg.playerIndex));
        }

        IEnumerator RespawnStay(int fishIndex, int rndTarget, int playerIndex)
        {
            float distance = Vector3.Distance(arrayFish[fishIndex].transform.position, Camera.main.gameObject.transform.position);
            Vector3 vec3;
            if (playerIndex == 0)
            {
                vec3 = Camera.main.ViewportToWorldPoint(new Vector3(0.07f, 0.93f, distance));
                arrayFish[fishIndex].transform.localRotation = Quaternion.Euler(0, 135, 0);
            }
            else if (playerIndex == 1)
            {
                vec3 = Camera.main.ViewportToWorldPoint(new Vector3(0.07f, 0.07f, distance));
                arrayFish[fishIndex].transform.localRotation = Quaternion.Euler(0, -135, 0);
            }
            else if (playerIndex == 2)
            {
                vec3 = Camera.main.ViewportToWorldPoint(new Vector3(0.93f, 0.07f, distance));
                arrayFish[fishIndex].transform.localRotation = Quaternion.Euler(0, 135, 0);
            }
            else
            {
                vec3 = Camera.main.ViewportToWorldPoint(new Vector3(0.93f, 0.93f, distance));
                arrayFish[fishIndex].transform.localRotation = Quaternion.Euler(0, -135, 0);
            }

            arrayFish[fishIndex].transform.position = vec3;
            arrayFish[fishIndex].transform.localScale = Vector3.one;
            yield return new WaitForSeconds(1.0f);
            arrayFish[fishIndex].transform.DOMove(bgObject.arrayPlate[playerIndex].transform.position, 1.5f);
            yield return StartCoroutine(arrayFish[fishIndex].LayerCheck(false));
            arrayFish[fishIndex].gameObject.SetActive(false);
            StartCoroutine(Respawn(fishIndex, rndTarget));
        }

        protected virtual IEnumerator Respawn(int index, int target)
        {
            yield return new WaitForSeconds(fm.FishRespawnSec(fishIndexNum));
            arrayFish[index].InitFish(index, fm.FishSizeMax(fishIndexNum), fm.FishSizeMin(fishIndexNum), fm.FishSpeedMax(fishIndexNum), fm.FishSpeedMin(fishIndexNum), fm.FishCatchDelay(fishIndexNum), fm.FishViewPosZ(fishIndexNum));
            arrayFish[index].gameObject.SetActive(true);
            arrayFish[index].Respawn(bgObject.GetTargetPosition(target));
            MoveFish(index, target);
        }

        protected virtual void FishArrive(FishArriveMsg msg)
        {
            int index;
            do
            {
                index = UnityEngine.Random.Range(0, bgObject.arrayTarget.Length);
            } while (index == msg.targetIndex);

            MoveFish(msg.index, index);
        }

        protected virtual void MissFish(MissFishMsg msg)
        {
            MissSoundPlay();
            MoveFish(msg.index, bgObject.GetMostNearPosition(arrayFish[msg.index].gameObject.transform.position));
            Debug.Log("놓침");
        }

        protected abstract void MissSoundPlay();

        private void OnDestroy()
        {
            RemoveMessage();
            foreach (var o in listGameObject)
            {
                if (o != null)
                    Destroy(o);
            }
        }
        protected virtual void RemoveMessage()
        {
            Message.RemoveListener<CatchPlateSuccessMsg>(CatchPlateSuccess);
            Message.RemoveListener<FishArriveMsg>(FishArrive);
            Message.RemoveListener<MissFishMsg>(MissFish);
        }
    }
}