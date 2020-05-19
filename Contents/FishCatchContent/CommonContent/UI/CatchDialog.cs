using JHchoi.Common;
using JHchoi.UI.Event;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JHchoi.UI
{
    public class CatchDialog : IDialog
    {
        public GameObject[] CatchPanel;
        public GameObject CatchInfoPanel;
        public GameObject MissAni;

        ObjectPool ObjPoolMiss;

        protected override void OnEnter()
        {
            AddMessage();

            if (MissAni != null)
                ObjPoolMiss = CreateObjectPool(MissAni, 10);

            foreach (var o in CatchPanel)
                o.SetActive(false);

            CatchInfoPanel.SetActive(false);
        }

        ObjectPool CreateObjectPool(GameObject o, int count)
        {
            var PoolObject = new GameObject();
            PoolObject.name = o.name + "Pool";
            PoolObject.layer = 5;
            PoolObject.transform.SetParent(dialogView.transform);
            PoolObject.transform.localPosition = Vector3.zero;
            PoolObject.transform.localScale = Vector3.one;
            ObjectPool p = PoolObject.AddComponent<ObjectPool>();
            p.PreloadObject(count, o as GameObject);
            return p;
        }

        private void AddMessage()
        {
            Message.AddListener<CatchPlayerMsg>(CatchPlayer);
            Message.AddListener<CatchInfoMsg>(CatchInfo);
            Message.AddListener<MissFishMsg>(MissFish);
        }

        private void CatchPlayer(CatchPlayerMsg msg)
        {
            CatchPanel[msg.playerIndex].SetActive(true);
            StartCoroutine(AniOff(msg.playerIndex));
        }

        IEnumerator AniOff(int index)
        {
            yield return new WaitForSeconds(1.5f);
            CatchPanel[index].SetActive(false);
        }

        private void CatchInfo(CatchInfoMsg msg)
        {
            CatchInfoPanel.SetActive(msg.isCatch);
        }

        private void MissFish(MissFishMsg msg)
        {
            GameObject missAni = ObjPoolMiss.GetObject(ObjPoolMiss.transform);
            missAni.transform.localScale = new Vector3(100, 100, 100);
            Vector2 screenPoint = Camera.main.WorldToScreenPoint(msg.position);
            missAni.transform.localPosition = new Vector2(screenPoint.x - (Screen.width / 2), screenPoint.y - (Screen.height / 2));

            if (screenPoint.x < Screen.width / 2 && screenPoint.y > (Screen.height / 2))
            {
                missAni.transform.localEulerAngles = new Vector3(0, 0, -120);
            }
            else if (screenPoint.x < Screen.width / 2 && screenPoint.y < (Screen.height / 2))
            {
                missAni.transform.localEulerAngles = new Vector3(0, 0, -45);
            }
            else if (screenPoint.x > Screen.width / 2 && screenPoint.y < (Screen.height / 2))
            {
                missAni.transform.localEulerAngles = new Vector3(0, 0, 45);
            }
            else if (screenPoint.x > Screen.width / 2 && screenPoint.y > (Screen.height / 2))
            {
                missAni.transform.localEulerAngles = new Vector3(0, 0, 120);
            }

            StartCoroutine(AniActiveFalse(missAni));
        }

        IEnumerator AniActiveFalse(GameObject obj)
        {
            yield return new WaitForSeconds(1.0f);
            ObjPoolMiss.PoolObject(obj);
        }

        protected override void OnExit()
        {
            RemoveMessage();
        }

        private void RemoveMessage()
        {
            Message.RemoveListener<CatchPlayerMsg>(CatchPlayer);
            Message.RemoveListener<CatchInfoMsg>(CatchInfo);
            Message.RemoveListener<MissFishMsg>(MissFish);
        }
    }
}