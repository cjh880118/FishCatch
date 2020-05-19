using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JHchoi.Models;
using JHchoi.Constants;
using JHchoi.UI.Event;
using System;
using DG.Tweening;
using Random = UnityEngine.Random;
using JHchoi.Constants.FishCatch;

namespace JHchoi.Contents
{
    public class FishDessertContent : ITycoonContent
    {
        protected override IEnumerator LoadObject()
        {
            listFood.Clear();

            string path = "Prefab/FishCatch/BackGround/" + cm.GetBackGroundName(pcm.GetCurrentContent().ContentName);
            yield return StartCoroutine(ResourceLoader.Instance.Load<GameObject>(path,
               o =>
               {
                   var inGameObject = Instantiate(o) as GameObject;
                   inGameObject.transform.parent = this.gameObject.transform;
                   listGameObject.Add(inGameObject);
                   inGameObject.name = o.name;
                   backGround = inGameObject.GetComponent<ITycoonBackGroundObject>();
                   backGround.InitBackGround(0, 0);
               }));

            int index = 0;
            for (int i = (int)FoodType.Cake1; i < (int)FoodType.IceCream + 1; i++)
            {
                path = "Prefab/FishCatch/Dessert/" + fm.PrefabName(i);
                yield return StartCoroutine(ResourceLoader.Instance.Load<GameObject>(path,
                   o =>
                   {
                       int count = fm.FishCount(i);

                       for (int j = 0; j < count; j++)
                       {
                           var inGameObject = Instantiate(o) as GameObject;
                           inGameObject.name = o.name + "_" + j;
                           inGameObject.transform.parent = this.gameObject.transform;
                           listFood.Add(inGameObject.GetComponent<IFood>());
                           inGameObject.SetActive(false);
                           listFood[index].InitFood(index, (FoodType)i, fm.FishCatchDelay(i), fm.FishViewPosZ(i), this.transform);
                           index++;
                       }
                   }));
            }

            ObjectLoadComplete();
        }

        protected override void OnEnter()
        {
            Debug.Log("DessertContent OnEnter");
            base.OnEnter();
        }

        protected override void OnExit()
        {
            Debug.Log("DessertContent OnExit");
            foreach (var o in listFood)
            {
                Destroy(o.gameObject);
            }
            if (corResetTimer != null)
                StopCoroutine(corResetTimer);
            base.OnExit();
        }

        protected override void MissSoundPlay()
        {
        }

        protected override void EmptyPlate(int plateNum)
        {
        }

        protected override void CatchInputSound()
        {
        }

        protected override void PlaySound()
        {
            SoundManager.Instance.PlaySound((int)SoundFishCatch.Dessert_Bgm);
            SoundManager.Instance.PlaySound((int)SoundFishCatch.Dessert_Amb);
        }

        protected override void IsRightSound(bool isRight)
        {
            if (isRight)
                SoundManager.Instance.PlaySound((int)SoundFishCatch.Dessert_Right);
            else
                SoundManager.Instance.PlaySound((int)SoundFishCatch.Dessert_Fail);
        }

        protected override void RespawnSoundPlay()
        {
            SoundManager.Instance.PlaySound((int)SoundFishCatch.Sushi_PopUp);
        }
    }
}

