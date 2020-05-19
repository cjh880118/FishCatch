using CellBig.Constants;
using CellBig.Constants.FishCatch;
using CellBig.UI.Event;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CellBig.Contents
{
    public class FishSeaContent : ICatchFishContent
    {
        Bettafishs_Controller bettafishs;
        Discussfishs_Controller discussfishs;
        Guppies_Controller guppies;
        Octopuses_Controller octopuses;
        Piranhas_Controller piranhas;
        Turtles_Controller turtles;
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

            #region FishObjectLoad
            //>>>>>>>>>>>>>>>Bettafishs_Controller 오브젝트 로드
            path = "Prefab/FishCatch/Controller/Bettafishs_Controller";
            yield return StartCoroutine(ResourceLoader.Instance.Load<GameObject>(path,
               o =>
               {
                   var inGameObject = Instantiate(o) as GameObject;
                   inGameObject.transform.parent = this.gameObject.transform;
                   listGameObject.Add(inGameObject);
                   inGameObject.name = o.name;
                   bettafishs = inGameObject.GetComponent<Bettafishs_Controller>();
                   bettafishs.InitFishController((int)FishType.Bettafish, fm, backGround, CatchObjectType.Fish);
               }));

            while (!bettafishs.isLoadComplete)
                yield return null;
            //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

            //>>>>>>>>>>>>>>>Discussfishs_Controller 오브젝트 로드
            path = "Prefab/FishCatch/Controller/Discussfishs_Controller";
            yield return StartCoroutine(ResourceLoader.Instance.Load<GameObject>(path,
               o =>
               {
                   var inGameObject = Instantiate(o) as GameObject;
                   inGameObject.transform.parent = this.gameObject.transform;
                   listGameObject.Add(inGameObject);
                   inGameObject.name = o.name;
                   discussfishs = inGameObject.GetComponent<Discussfishs_Controller>();
                   discussfishs.InitFishController((int)FishType.Discussfish, fm, backGround, CatchObjectType.Fish);
               }));

            while (!discussfishs.isLoadComplete)
                yield return null;

            //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

            //>>>>>>>>>>>>>>>Guppies_Controller 오브젝트 로드
            path = "Prefab/FishCatch/Controller/Guppies_Controller";
            yield return StartCoroutine(ResourceLoader.Instance.Load<GameObject>(path,
               o =>
               {
                   var inGameObject = Instantiate(o) as GameObject;
                   inGameObject.transform.parent = this.gameObject.transform;
                   listGameObject.Add(inGameObject);
                   inGameObject.name = o.name;
                   guppies = inGameObject.GetComponent<Guppies_Controller>();
                   guppies.InitFishController((int)FishType.Guppy, fm, backGround, CatchObjectType.Fish);
               }));

            while (!guppies.isLoadComplete)
                yield return null;

            //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

            //>>>>>>>>>>>>>>>Octopuses_Controller 오브젝트 로드
            path = "Prefab/FishCatch/Controller/Octopuses_Controller";
            yield return StartCoroutine(ResourceLoader.Instance.Load<GameObject>(path,
               o =>
               {
                   var inGameObject = Instantiate(o) as GameObject;
                   inGameObject.transform.parent = this.gameObject.transform;
                   listGameObject.Add(inGameObject);
                   inGameObject.name = o.name;
                   octopuses = inGameObject.GetComponent<Octopuses_Controller>();
                   octopuses.InitFishController((int)FishType.Octopus, fm, backGround, CatchObjectType.Fish);
               }));

            while (!octopuses.isLoadComplete)
                yield return null;
            //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

            //>>>>>>>>>>>>>>>Piranhas_Controller 오브젝트 로드
            path = "Prefab/FishCatch/Controller/Piranhas_Controller";
            yield return StartCoroutine(ResourceLoader.Instance.Load<GameObject>(path,
               o =>
               {
                   var inGameObject = Instantiate(o) as GameObject;
                   inGameObject.transform.parent = this.gameObject.transform;
                   listGameObject.Add(inGameObject);
                   inGameObject.name = o.name;
                   piranhas = inGameObject.GetComponent<Piranhas_Controller>();
                   piranhas.InitFishController((int)FishType.Piranha, fm, backGround, CatchObjectType.Fish);
               }));

            while (!piranhas.isLoadComplete)
                yield return null;

            //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

            //>>>>>>>>>>>>>>>Turtles_Controller 오브젝트 로드
            path = "Prefab/FishCatch/Controller/Turtles_Controller";
            yield return StartCoroutine(ResourceLoader.Instance.Load<GameObject>(path,
               o =>
               {
                   var inGameObject = Instantiate(o) as GameObject;
                   inGameObject.transform.parent = this.gameObject.transform;
                   listGameObject.Add(inGameObject);
                   inGameObject.name = o.name;
                   turtles = inGameObject.GetComponent<Turtles_Controller>();
                   turtles.InitFishController((int)FishType.Turtle, fm, backGround, CatchObjectType.Fish);
               }));

            while (!turtles.isLoadComplete)
                yield return null;

            #endregion

            UI.IDialog.RequestDialogEnter<UI.SeaInfoDialog>();
            ObjectLoadComplete();
        }

        protected override void CatchInputSound()
        {
            SoundManager.Instance.PlaySound((int)SoundFishCatch.Sea_CatchAfter);
            SoundManager.Instance.StopSound((int)SoundFishCatch.Sfx_CatchIng);
        }

        protected override void PlaySound()
        {
            SoundManager.Instance.PlaySound((int)SoundFishCatch.Sea_Bgm);
            listCorSound.Add(StartCoroutine(SoundCorLoop(4, SoundFishCatch.Sea_Amb1)));
            listCorSound.Add(StartCoroutine(SoundCorLoop(4, SoundFishCatch.Sea_Amb2)));
        }

        protected override void OnExit()
        {
            UI.IDialog.RequestDialogExit<UI.SeaInfoDialog>();
            base.OnExit();
        }
    }
}