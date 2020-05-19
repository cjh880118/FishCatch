using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using CellBig.Models;
using CellBig;
using CellBig.UI.Event;
using CellBig.Contents.Event;
using CellBig.Common;
using CellBig.Module;
using CellBig.Constants;

using DG.Tweening;
using Rect = UnityEngine.Rect;
using OpenCVForUnity;

public enum MeshType
{
    Circle,
    Square,
    Triangle,
}

public enum PatternType
{
    CheckPattern,
    StripePattern,
    ZigzagPattern,
}

namespace CellBig.Contents
{
    public class GameQuizContent : IFantaBoxContent
    {
        public MeshType meshType;
        public PatternType patternType;

        int randomMeshType;
        int randomPatternType;

        GameQuiz_ObjectControl gameQuiz_ObjectControl;

        ObjectPool figurePool = null;

        public List<GameQuiz_Figure> figureList = new List<GameQuiz_Figure>();

        Coroutine Cor_GameLogic;
        Coroutine Cor_SetQuiz;

        GameModel gm;

        protected override void OnLoadStart()
        {
            gm = Model.First<GameModel>();
            StartCoroutine(Cor_Load());
        }

        IEnumerator Cor_Load()
        {
            yield return null;

            ObjectList = new List<GameObject>();
            string scenename = "GameQuiz";
            var fullpath = string.Format("Scenes/FantaScenes/Fanta/{0}", scenename);
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath, o =>
                                                gameQuiz_ObjectControl = OnPostLoadProcess(o).GetComponent<GameQuiz_ObjectControl>()));

            string prefabname = scenename + "/Object/Figure";
            var fullpath_prefab = string.Format("Prefab/{0}", prefabname);
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath_prefab, o => figurePool = OnPostLoadProcess(o, 20)));

            SetLoadComplete();
        }
        protected override void OnEnter()
        {
            // 게임 시작시 Ready 연출을 시작해 줍니다.
            ObjectListOn();

            RequestContentEnter<GlobalContent>();
            RequestContentEnter<ReadyContent>();

            Message.Send<UI.Event.FadeOutMsg>(new UI.Event.FadeOutMsg());
            Message.Send<MainCameraMsg>(new MainCameraMsg(mainCamera));

            SoundManager.Instance.PlaySound((int)SoundType_GameBGM.Quiz);
        }

        protected override void OnExit()
        {
            Message.Send<PoolObjectMsg>(new PoolObjectMsg());
            ObjectListOff();
        }

        protected override void OnAddMessage()
        {
            Message.AddListener<Event.GameObjectDeActiveMessage>(OnDeactive);
        }

        protected override void OnRemoveMessage()
        {
            Message.RemoveListener<Event.GameObjectDeActiveMessage>(OnDeactive);
        }

        protected override void OnPlay()
        {
            UI.IDialog.RequestDialogEnter<UI.QuizDialog>();
            Message.Send<MultiTouchMsg>(new MultiTouchMsg());

            Cor_GameLogic = StartCoroutine(CreateFigure());
            Cor_SetQuiz = StartCoroutine(SetQuiz());
        }

        IEnumerator CreateFigure()
        {
            while(true)
            {
                GameQuiz_Figure tempFigure = null;

                tempFigure = figurePool.GetObject(figurePool.transform).GetComponent<GameQuiz_Figure>();

                figureList.Add(tempFigure);
                gameQuiz_ObjectControl.figureList = figureList;

                yield return StartCoroutine(gameQuiz_ObjectControl.SetSpawn(tempFigure));

                float Random_Dealy = UnityEngine.Random.Range(0.5f, 1f);

                yield return new WaitForSeconds(Random_Dealy);
            }
        }

        IEnumerator SetQuiz()
        {
            while(true)
            {
                randomMeshType = UnityEngine.Random.Range(0, Enum.GetNames(typeof(MeshType)).Length);
                randomPatternType = UnityEngine.Random.Range(0, Enum.GetNames(typeof(PatternType)).Length);

                meshType = (MeshType)randomMeshType;
                patternType = (PatternType)randomPatternType;

                Message.Send<QuizMsg>(new QuizMsg(meshType, patternType));

                yield return new WaitForSeconds(10.0f);
                yield return null;
            }
        }

        protected override void OnHit(GameObject obj)
        {
            GameQuiz_Figure obj_script = obj.transform.GetComponent<GameQuiz_Figure>();

            if (obj_script != null)
                obj_script.Hit(meshType, patternType);
        }

        protected override void OnEnd()
        {
            UI.IDialog.RequestDialogExit<UI.QuizDialog>();

            StopCoroutine(Cor_GameLogic);
            StopCoroutine(Cor_SetQuiz);

            Cor_GameLogic = null;
            Cor_SetQuiz = null;

            figureList.Clear();

            SoundManager.Instance.StopSound((int)SoundType_GameBGM.Quiz);
        }

        void OnDeactive(Event.GameObjectDeActiveMessage msg)
        {
            figureList.Remove(msg.myObject.GetComponent<GameQuiz_Figure>());
            gameQuiz_ObjectControl.figureList = figureList;

            figurePool.PoolObject(msg.myObject);
        }
    }
}