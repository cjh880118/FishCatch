using CellBig.Constants.FishCatch;
using CellBig.UI.Event;
using System.Collections;
using UnityEngine;

namespace CellBig.Contents
{
    public class FishSpringContent : ICatchFishContent
    {
        protected Salmons_Controller salmonsController;
        protected override IEnumerator LoadObject()
        {
            listFish.Clear();
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


            path = "Prefab/FishCatch/Controller/Salmons_Controller";
            yield return StartCoroutine(ResourceLoader.Instance.Load<GameObject>(path,
               o =>
               {
                   var inGameObject = Instantiate(o) as GameObject;
                   inGameObject.transform.parent = this.gameObject.transform;
                   listGameObject.Add(inGameObject);
                   inGameObject.name = o.name;
                   salmonsController = inGameObject.GetComponent<Salmons_Controller>();
                   salmonsController.InitFishController((int)FishType.Orange_Salmon, fm, backGround, CatchObjectType.Fish);
               }));

            while (!salmonsController.isLoadComplete)
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
            SoundManager.Instance.PlaySound((int)SoundFishCatch.Spring_Bgm);
            SoundManager.Instance.PlaySound((int)SoundFishCatch.Spring_Amb);
        }

        protected override void OnExit()
        {
            base.OnExit();
            UI.IDialog.RequestDialogExit<UI.FishInfoDialog>();
        }
    }
}