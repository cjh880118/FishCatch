using JHchoi.Constants.FishCatch;
using JHchoi.UI.Event;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace JHchoi.Contents
{
    public abstract class ITycoonContent : IContentManager
    {
        protected ITycoonBackGroundObject backGround;
        protected TycoonPlayer[] arrayPlayer;
        protected List<int> listPlateNum = new List<int>();     //접시
        protected List<IFood> listFood = new List<IFood>();     //전체 음식 리스트
        protected Dictionary<int, FoodType> mapActiveFood = new Dictionary<int, FoodType>();             //활성화 음식
        protected Dictionary<int, FoodType> mapPlayerMissionFood = new Dictionary<int, FoodType>();     //플레이어 미션
        //protected Dictionary<int, FoodType> mapListFoodType = new Dictionary<int, FoodType>();
        protected Coroutine corSetFirstMission;
        protected Coroutine corResetTimer;
        protected Coroutine corSetMission;
        protected bool isShuffle;

        protected override void OnEnter()
        {
            Debug.Log("ITycoonContent : OnEnter ");
            InitPlayer(cm.GetPlayerCount(pcm.GetCurrentContent().ContentName));
            UI.IDialog.RequestDialogEnter<UI.OXDialog>();
            base.OnEnter();
        }

        void InitPlayer(int playerCount)
        {
            arrayPlayer = new TycoonPlayer[playerCount];
            for (int i = 0; i < playerCount; i++)
            {
                arrayPlayer[i] = new TycoonPlayer();
                arrayPlayer[i].InitPlayer(i);
            }
        }

        protected override void ObjectLoadComplete()
        {
            base.ObjectLoadComplete();
            corSetFirstMission = StartCoroutine(SetFirstMission());
        }

        protected override void AddMessage()
        {
            base.AddMessage();
            Message.AddListener<ArrayListObjMsg<IFood>>(ArrayListObj);
            Message.AddListener<MissFoodMsg>(MissFood);
            Message.AddListener<CatchFoodPlateNumMsg>(CatchFoodPlateNum);
            Message.AddListener<SetTycoonMissionMsg<FoodType>>(SetTycoonMission);
        }

        private void ArrayListObj(ArrayListObjMsg<IFood> msg)
        {
            for (int i = 0; i < msg.arrayObj.Length; i++)
            {
                listFood.Add(msg.arrayObj[i]);
                //mapListFoodType.Add(i, listFood[i].GetFoodType());
            }
        }

        protected virtual void MissFood(MissFoodMsg msg)
        {
            MissSoundPlay();
            RemoveFood(msg.index);
            Debug.Log("IFoodContent :놓침");
        }

        protected abstract void MissSoundPlay();
        protected abstract void RespawnSoundPlay();

        private void CatchFoodPlateNum(CatchFoodPlateNumMsg msg)
        {
            EmptyPlate(msg.plateNum);
        }

        private void SetTycoonMission(SetTycoonMissionMsg<FoodType> msg)
        {
            mapPlayerMissionFood.Add(msg.playerIndex, msg.food);
            arrayPlayer[msg.playerIndex].SetFoodMission(msg.food);
            arrayPlayer[msg.playerIndex].IsMissSet = true;
        }

        protected abstract void EmptyPlate(int plateNum);

        //최초 미션 셋팅
        protected IEnumerator SetFirstMission()
        {
            isShuffle = true;
            mapActiveFood.Clear();
            listPlateNum.Clear();
            mapPlayerMissionFood.Clear();

            for (int i = 0; i < backGround.arrayPlate.Length; i++)
            {
                Message.Send<MissionShuffleMsg>(new MissionShuffleMsg(i));
            }

            foreach (var o in listFood)
            {
                o.gameObject.SetActive(false);
            }

            while (listPlateNum.Count < backGround.arrayTarget.Length)
            {
                int plateNum;
                do
                {
                    plateNum = Random.RandomRange(0, backGround.arrayTarget.Length);
                } while (listPlateNum.Contains(plateNum));
                listPlateNum.Add(plateNum);

                int rndFood;
                do
                {
                    rndFood = Random.RandomRange(0, listFood.Count);
                } while (listFood[rndFood].gameObject.activeSelf);

                SetFood(rndFood, plateNum);
                yield return new WaitForSeconds(0.15f);
            }

            for (int i = 0; i < backGround.arrayPlate.Length; i++)
            {
                int rndFood;
                do
                {
                    rndFood = Random.RandomRange(0, listFood.Count);
                } while (!mapActiveFood.ContainsKey(rndFood));

                Message.Send<SetTycoonMissionMsg<FoodType>>(new SetTycoonMissionMsg<FoodType>(mapActiveFood[rndFood], i));
            }

            isShuffle = false;
            corResetTimer = StartCoroutine(ResetMissionTimer());
        }

        void SetFood(int foodIndex, int plateIndex)
        {
            mapActiveFood.Add(foodIndex, listFood[foodIndex].GetFoodType());
            listFood[foodIndex].gameObject.transform.parent = backGround.GetRespawnObject(plateIndex).transform;
            listFood[foodIndex].gameObject.transform.localPosition = Vector3.zero;
            listFood[foodIndex].gameObject.transform.localScale = Vector3.one;
            listFood[foodIndex].gameObject.transform.localRotation = new Quaternion(0, 0, 0, 0);
            listFood[foodIndex].gameObject.SetActive(true);
            listFood[foodIndex].SetPlateNum(plateIndex);
            listFood[foodIndex].ResetFood();
            backGround.SetPlate(plateIndex);
            RespawnSoundPlay();
        }

        IEnumerator ResetMissionTimer()
        {
            yield return new WaitForSeconds(cm.GetResetTime(pcm.GetCurrentContent().ContentName));
            corSetMission = StartCoroutine(SetNextMissino());
        }

        IEnumerator SetNextMissino()
        {
            yield return StartCoroutine(backGround.ResetPlate());
            isShuffle = true;
            listPlateNum.Clear();

            for (int i = 0; i < listFood.Count; i++)
            {
                if (listFood[i].isCapturePossible && mapActiveFood.ContainsKey(i))
                {
                    listFood[i].gameObject.SetActive(false);
                    mapActiveFood.Remove(i);
                }
            }

            while (listPlateNum.Count < backGround.arrayTarget.Length)
            {
                int plateNum;
                do
                {
                    plateNum = Random.RandomRange(0, backGround.arrayTarget.Length);
                } while (listPlateNum.Contains(plateNum));

                listPlateNum.Add(plateNum);


                int rndFood;
                do
                {
                    rndFood = Random.RandomRange(0, listFood.Count);
                } while (listFood[rndFood].gameObject.activeSelf && mapActiveFood.ContainsKey(rndFood));

                SetFood(rndFood, plateNum);
                yield return new WaitForSeconds(0.15f);
            }
            isShuffle = false;
            corResetTimer = StartCoroutine(ResetMissionTimer());
        }

        void RemoveFood(int foodIndex)
        {
            mapActiveFood.Remove(foodIndex);
        }

        protected override IEnumerator LoadObject()
        {
            throw new System.NotImplementedException();
        }

        protected override IEnumerator RectInputDelay()
        {
            while (true)
            {
                rectInputDelayCheck += Time.deltaTime;
                yield return null;
                if (rectInputDelayCheck > 1)
                {
                    foreach (var o in listFood)
                    {
                        if (o.GetIsCatch() && o.gameObject.activeSelf)
                        {
                            o.MissionFood();
                        }
                    }
                }
            }
        }

        protected IEnumerator CatchInputAfter(IFood food, Vector3 target, int playerIndex)
        {
            float distance = Vector3.Distance(food.gameObject.transform.position, target);
            while (distance > 0.3f)
            {
                distance = Vector3.Distance(food.gameObject.transform.position, target);
                yield return null;
            }

            food.gameObject.SetActive(false);
            SendFoodTypePlateNum(food.GetFoodType(), playerIndex);
            food.SetIsCatchInput(false);
            RemoveFood(food.GetFoodIndex());
        }

        private void SendFoodTypePlateNum(FoodType foodType, int playerIndex)
        {
            bool isRight;
            if (arrayPlayer[playerIndex].foodType == foodType)
                isRight = true;
            else
                isRight = false;

            mapPlayerMissionFood.Remove(playerIndex);
            IsRightSound(isRight);
            Message.Send<CatchFoodPlayerMsg>(new CatchFoodPlayerMsg(isRight, playerIndex));
        }

        protected abstract void IsRightSound(bool isRight);

        protected abstract void CatchInputSound();

        protected override IEnumerator ViewPortCheck(List<Vector2> vec2List)
        {
            Debug.Log("RectList Count : " + vec2List.Count);
            while (vec2List.Count != 0)
            {
                rectInputDelayCheck = 0;
                bool isCatchPossible = true;

                for (int i = 0; i < listFood.Count; i++)
                {
                    if (!listFood[i].gameObject.activeSelf)
                        continue;

                    Vector2 foodViewport = Camera.main.WorldToViewportPoint(listFood[i].transform.position);
                    if (Vector3.Distance(vec2List[0], foodViewport) < 0.1f)
                    {
                        //근쳐에 잡을수 없고 
                        if (!listFood[i].isCapturePossible && listFood[i].gameObject.activeSelf)
                        {
                            isCatchPossible = false;
                            break;
                        }
                    }
                }

                if (isCatchPossible && !isShuffle)
                {
                    Debug.Log("잡을수 있는 렉트");
                    float minDistance = 0;
                    int index = 0;

                    for (int i = 0; i < listFood.Count; i++)
                    {
                        if (!listFood[i].gameObject.activeSelf)
                            continue;

                        Vector2 foodViewport = Camera.main.WorldToViewportPoint(listFood[i].transform.position);
                        float distance = Vector2.Distance(vec2List[0], foodViewport);
                        if (distance < cm.GetCatchDistance(pcm.GetCurrentContent().ContentName))
                        {
                            if (minDistance == 0)
                            {
                                minDistance = distance;
                                index = i;
                            }
                            else if (minDistance < distance)
                            {
                                minDistance = distance;
                                index = i;
                            }
                        }
                    }

                    if (minDistance > 0)
                    {
                        listFood[index].Catch();
                    }

                }

                Message.Send<RectPositionMsg>(new RectPositionMsg(vec2List[0]));
                vec2List.RemoveAt(0);
            }

            yield return null;
        }

        protected override void RectPosition(RectPositionMsg msg)
        {
            foreach (var o in listFood)
            {

                if (o.GetIsCatch() && !o.GetIsCatchInput())
                {
                    Vector3 objPosition = Camera.main.WorldToViewportPoint(o.transform.position);
                    float distance = Vector2.Distance(msg.vec2RectViewPortPosition, objPosition);
                    Vector3 vecPos = new Vector3(msg.vec2RectViewPortPosition.x, msg.vec2RectViewPortPosition.y, objPosition.z);
                    string logFomat = string.Format("렉트와 뷰포트상 거리 : {0}", distance);
                    Log.Instance.log(logFomat);

                    //잡혓을 뷰포트상 거리로 판단
                    if (distance <= catchDistance && o.gameObject.activeSelf && !o.isMissObj)
                    {
                        Vector3 rectWorld = Camera.main.ViewportToWorldPoint(vecPos);
                        o.transform.position = rectWorld;
                        o.RectInputDelay(DateTime.Now);
                    }

                    //잡힌 상태에서 접시와의 거리 뷰포트로 판단
                    for (int i = 0; i < backGround.arrayPlate.Length; i++)
                    {
                        float playteDistance = Vector2.Distance(backGround.GetPlateViewPortPosition(i), objPosition);
                        if (playteDistance <= cm.GetInnerDistance(pcm.GetCurrentContent().ContentName) && o.gameObject.activeSelf && arrayPlayer[i].IsMissSet)
                        {
                            arrayPlayer[i].IsMissSet = false;
                            CatchInputSound();
                            o.SetIsCatchInput(true);
                            Debug.Log("잡아 넣었다.");
                            o.isMissObj = true;
                            o.transform.DOMove(backGround.GetPlateWorldPosition(i), 0.5f);
                            StartCoroutine(CatchInputAfter(o, backGround.GetPlateWorldPosition(i), i));
                            break;
                        }
                    }
                }
            }
        }

        protected override void OnExit()
        {
            base.OnExit();
            CoroutineCheckStop(corSetFirstMission);
            CoroutineCheckStop(corResetTimer);
            CoroutineCheckStop(corSetMission);
            UI.IDialog.RequestDialogExit<UI.OXDialog>();
        }

        protected override void RemoveMessage()
        {
            base.RemoveMessage();
            Message.RemoveListener<ArrayListObjMsg<IFood>>(ArrayListObj);
            Message.RemoveListener<MissFoodMsg>(MissFood);
            Message.RemoveListener<CatchFoodPlateNumMsg>(CatchFoodPlateNum);
            Message.RemoveListener<SetTycoonMissionMsg<FoodType>>(SetTycoonMission);
        }
    }
}
