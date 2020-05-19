using CellBig.Constants;
using CellBig.Constants.FishCatch;
using CellBig.UI.Event;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CellBig.Contents
{
    public class FishBeetleContent : ICatchFishContent
    {
        Goldbeetles_Controller goldbeetles;
        MonsterBeetles_Controller monsterBeetles;
        RinoBeetles_Controller rinoBeetles;
        UnicornBeetles_Controller unicornBeetles;
    
        protected override IEnumerator LoadObject()
        {
            string path = "Prefab/FishCatch/BackGround/" + cm.GetBackGroundName(pcm.GetCurrentContent().ContentName);
            yield return StartCoroutine(ResourceLoader.Instance.Load<GameObject>(path,
               o =>
               {
                   var inGameObject = Instantiate(o) as GameObject;
                   inGameObject.transform.parent = this.gameObject.transform;
                   listGameObject.Add(inGameObject);
                   inGameObject.name = o.name;
                   backGround = inGameObject.GetComponent<BackGroundObject>();
                   backGround.InitBackGround(0, 0);
               }));

            #region FishObjectLoad
            //>>>>>>>>>>>>>>>Goldbeetles_Controller 오브젝트 로드
            path = "Prefab/FishCatch/Controller/Goldbeetles_Controller";
            yield return StartCoroutine(ResourceLoader.Instance.Load<GameObject>(path,
               o =>
               {
                   var inGameObject = Instantiate(o) as GameObject;
                   inGameObject.transform.parent = this.gameObject.transform;
                   listGameObject.Add(inGameObject);
                   inGameObject.name = o.name;
                   goldbeetles = inGameObject.GetComponent<Goldbeetles_Controller>();
                   goldbeetles.InitFishController((int)FishType.GoldBeetle, fm, backGround, CatchObjectType.Beetle);
               }));

            while (!goldbeetles.isLoadComplete)
                yield return null;
            //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

            //>>>>>>>>>>>>>>>MonsterBeetles_Controller 오브젝트 로드
            path = "Prefab/FishCatch/Controller/MonsterBeetles_Controller";
            yield return StartCoroutine(ResourceLoader.Instance.Load<GameObject>(path,
               o =>
               {
                   var inGameObject = Instantiate(o) as GameObject;
                   inGameObject.transform.parent = this.gameObject.transform;
                   listGameObject.Add(inGameObject);
                   inGameObject.name = o.name;
                   monsterBeetles = inGameObject.GetComponent<MonsterBeetles_Controller>();
                   monsterBeetles.InitFishController((int)FishType.MonsterBeetle, fm, backGround, CatchObjectType.Beetle);
               }));

            while (!monsterBeetles.isLoadComplete)
                yield return null;

            //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

            //>>>>>>>>>>>>>>>RinoBeetles_Controller 오브젝트 로드
            path = "Prefab/FishCatch/Controller/RinoBeetles_Controller";
            yield return StartCoroutine(ResourceLoader.Instance.Load<GameObject>(path,
               o =>
               {
                   var inGameObject = Instantiate(o) as GameObject;
                   inGameObject.transform.parent = this.gameObject.transform;
                   listGameObject.Add(inGameObject);
                   inGameObject.name = o.name;
                   rinoBeetles = inGameObject.GetComponent<RinoBeetles_Controller>();
                   rinoBeetles.InitFishController((int)FishType.RinoBeetle, fm, backGround, CatchObjectType.Beetle);
               }));

            while (!rinoBeetles.isLoadComplete)
                yield return null;

            //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

            //>>>>>>>>>>>>>>>UnicornBeetles_Controller 오브젝트 로드
            path = "Prefab/FishCatch/Controller/UnicornBeetles_Controller";
            yield return StartCoroutine(ResourceLoader.Instance.Load<GameObject>(path,
               o =>
               {
                   var inGameObject = Instantiate(o) as GameObject;
                   inGameObject.transform.parent = this.gameObject.transform;
                   listGameObject.Add(inGameObject);
                   inGameObject.name = o.name;
                   unicornBeetles = inGameObject.GetComponent<UnicornBeetles_Controller>();
                   unicornBeetles.InitFishController((int)FishType.UnicornBeetle, fm, backGround, CatchObjectType.Beetle);
               }));

            while (!unicornBeetles.isLoadComplete)
                yield return null;
            //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
            #endregion

            UI.IDialog.RequestDialogEnter<UI.BeetleInfoDialog>();
            ObjectLoadComplete();
        }

        protected override void CatchInputSound()
        {
            SoundManager.Instance.PlaySound((int)SoundFishCatch.Sea_CatchAfter);
            SoundManager.Instance.StopSound((int)SoundFishCatch.Sfx_CatchIng);
        }

        protected override void PlaySound()
        {
            SoundManager.Instance.PlaySound((int)SoundFishCatch.Bug_Bgm);
            SoundManager.Instance.PlaySound((int)SoundFishCatch.Bug_Walk);
            listCorSound.Add(StartCoroutine(SoundCorLoop(8, SoundFishCatch.Bug_Amb)));
        }

        protected override void OnExit()
        {
            base.OnExit();
            UI.IDialog.RequestDialogExit<UI.BeetleInfoDialog>();
        }
    }
}