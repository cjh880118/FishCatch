using JHchoi.Constants;
using JHchoi.Constants.FishCatch;
using JHchoi.UI.Event;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JHchoi.Contents
{
    public class FishSummerContent : ICatchFishContent
    {
        protected Goldfishs_Controller goldfishsController;
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
                   backGround.InitBackGround(-1, 1);
               }));


            path = "Prefab/FishCatch/Controller/GoldFishs_Controller";
            yield return StartCoroutine(ResourceLoader.Instance.Load<GameObject>(path,
               o =>
               {
                   var inGameObject = Instantiate(o) as GameObject;
                   inGameObject.transform.parent = this.gameObject.transform;
                   listGameObject.Add(inGameObject);
                   inGameObject.name = o.name;
                   goldfishsController = inGameObject.GetComponent<Goldfishs_Controller>();
                   goldfishsController.InitFishController((int)FishType.Orange_Goldfish, fm, backGround, CatchObjectType.Fish);
               }));

            while (!goldfishsController.isLoadComplete)
                yield return null;

            UI.IDialog.RequestDialogEnter<UI.FishInfoDialog>();
            ObjectLoadComplete();
        }
        protected override void CatchInputSound()
        {
            SoundManager.Instance.PlaySound((int)SoundFishCatch.Spring_Sfx_Catch);
            SoundManager.Instance.StopSound((int)SoundFishCatch.Sfx_CatchIng);
        }

        protected override void PlaySound()
        {
            SoundManager.Instance.PlaySound((int)SoundFishCatch.Summer_Bgm);
            SoundManager.Instance.PlaySound((int)SoundFishCatch.Summer_Amb2);
            listCorSound.Add(StartCoroutine(SoundCorLoop(10, SoundFishCatch.Summer_Amb1)));
        }

        protected override void OnExit()
        {
            base.OnExit();
            UI.IDialog.RequestDialogExit<UI.FishInfoDialog>();
        }
    }
}