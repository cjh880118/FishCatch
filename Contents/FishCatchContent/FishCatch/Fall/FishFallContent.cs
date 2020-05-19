using JHchoi.Constants.FishCatch;
using JHchoi.UI.Event;
using System.Collections;
using UnityEngine;

namespace JHchoi.Contents
{
    public class FishFallContent : ICatchFishContent
    {
        //TTT
        protected Carps_Controller carpsController;

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

            path = "Prefab/FishCatch/Controller/Carps_Controller";
            yield return StartCoroutine(ResourceLoader.Instance.Load<GameObject>(path,
               o =>
               {
                   var inGameObject = Instantiate(o) as GameObject;
                   inGameObject.transform.parent = this.gameObject.transform;
                   listGameObject.Add(inGameObject);
                   inGameObject.name = o.name;
                   carpsController = inGameObject.GetComponent<Carps_Controller>();
                   carpsController.InitFishController((int)FishType.ThreeColors_Carp, fm, backGround, CatchObjectType.Fish);
               }));

            while (!carpsController.isLoadComplete)
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
            SoundManager.Instance.PlaySound((int)SoundFishCatch.Fall_Bgm);
            listCorSound.Add(StartCoroutine(SoundCorLoop(3, SoundFishCatch.Fall_Amb)));
        }

        protected override void OnExit()
        {
            base.OnExit();
            UI.IDialog.RequestDialogExit<UI.FishInfoDialog>();
        }
    }
}