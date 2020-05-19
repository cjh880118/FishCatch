using JHchoi.UI.Event;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JHchoi.Contents
{
    public abstract class ICatchFishContent : IContentManager
    {
        protected BackGroundObject backGround;
        protected List<IFish> listFish = new List<IFish>();
        Coroutine corCatchOBJCheck;

        protected override void OnEnter()
        {
            base.OnEnter();
            listFish.Clear();
            UI.IDialog.RequestDialogEnter<UI.CatchDialog>();
            corCatchOBJCheck = StartCoroutine(CatchOBJCheck());
        }

        protected override void AddMessage()
        {
            base.AddMessage();
            Message.AddListener<ArrayListObjMsg<IFish>>(ArrayListObj);
        }

        private void ArrayListObj(ArrayListObjMsg<IFish> msg)
        {
            
            for (int i = 0; i < msg.arrayObj.Length; i++)
            {
                listFish.Add(msg.arrayObj[i]);
            }
        }

        protected IEnumerator CatchOBJCheck()
        {
            while (true)
            {
                yield return null;
                int count = 0;
                for (int i = 0; i < listFish.Count; i++)
                {
                    if (!listFish[i].isCapturePossible)
                    {
                        count++;
                        break;
                    }
                }

                if (count > 0)
                {
                    Message.Send<CatchInfoMsg>(new CatchInfoMsg(true));
                }
                else
                {
                    Message.Send<CatchInfoMsg>(new CatchInfoMsg(false));
                }
            }
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
                    foreach (var o in listFish)
                    {
                        if (o.GetIsCatch() && o.gameObject.activeSelf && !o.GetIsCatchInput())
                        {
                            o.MissingFish();
                        }
                    }
                }
            }
        }

        protected override void RectPosition(RectPositionMsg msg)
        {
            foreach (var o in listFish)
            {
                if (o.GetIsCatch() && !o.GetIsCatchInput())
                {
                    Vector3 objPosition = Camera.main.WorldToViewportPoint(o.transform.position);
                    float distance = Vector2.Distance(msg.vec2RectViewPortPosition, objPosition);
                    Vector3 vecPos = new Vector3(msg.vec2RectViewPortPosition.x, msg.vec2RectViewPortPosition.y, objPosition.z);
                    string logFomat = string.Format("렉트와 뷰포트상 거리 : {0}", distance);
                    Log.Instance.log(logFomat);

                    //잡혓을 뷰포트상 거리로 판단
                    if (distance <= tempDistance)
                    {
                        Vector3 rectWorld = Camera.main.ViewportToWorldPoint(vecPos);
                        o.transform.position = Vector3.Lerp(o.transform.position, rectWorld, Time.deltaTime * 200f);
                        o.RectInputDelay(DateTime.Now);
                    }

                    //잡힌 상태에서 접시와의 거리 뷰포트로 판단
                    for (int i = 0; i < backGround.arrayPlate.Length; i++)
                    {
                        float playteDistance = Vector2.Distance(backGround.GetPlateViewPortPosition(i), objPosition);
                        if (playteDistance <= cm.GetInnerDistance(pcm.GetCurrentContent().ContentName) && o.gameObject.activeSelf)
                        {
                            o.SetIsCatchInput(true);
                            o.StopInputDelay();
                            Debug.Log("잡아 넣었다.");
                            Message.Send<CatchPlateSuccessMsg>(new CatchPlateSuccessMsg(o.GetFishType(), o.GetFishIndex(), i, fm.FishMainName((int)o.GetFishType())));
                            Message.Send<CatchPlayerMsg>(new CatchPlayerMsg(i));
                            CatchInputSound();
                            break;
                        }
                    }
                }
            }
        }

        protected abstract void CatchInputSound();

        protected override IEnumerator ViewPortCheck(List<Vector2> vec2List)
        {
            //print("RectList Count : " + vec2List.Count);
            while (vec2List.Count != 0)
            {
                rectInputDelayCheck = 0;
                bool isCatchPossible = true;

                for (int i = 0; i < listFish.Count; i++)
                {
                    Vector2 fishViewport = Camera.main.WorldToViewportPoint(listFish[i].transform.position);
                    if (Vector3.Distance(vec2List[0], fishViewport) < 0.1f)
                    {
                        if (!listFish[i].isCapturePossible)
                        {
                            isCatchPossible = false;
                            break;
                        }
                    }
                }

                if (isCatchPossible)
                {
                    //Debug.Log("잡을수 있는 렉트");
                    float minDistance = 0;
                    int index = 0;

                    for (int i = 0; i < listFish.Count; i++)
                    {
                        Vector2 fishViewport = Camera.main.WorldToViewportPoint(listFish[i].transform.position);
                        float distance = Vector2.Distance(vec2List[0], fishViewport);
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
                        listFish[index].Catch();
                    }

                }

                Message.Send<RectPositionMsg>(new RectPositionMsg(vec2List[0]));
                vec2List.RemoveAt(0);
            }
            yield return null;
        }

        protected override void RemoveMessage()
        {
            base.RemoveMessage();
            Message.RemoveListener<ArrayListObjMsg<IFish>>(ArrayListObj);
        }

        protected override void OnExit()
        {
            UI.IDialog.RequestDialogExit<UI.CatchDialog>();
            base.OnExit();
            if (corCatchOBJCheck != null)
                StopCoroutine(corCatchOBJCheck);
        }
    }
}
