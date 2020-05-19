using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using System.Collections.Generic;

using JHchoi.Constants;
using JHchoi.Models;
using JHchoi.UI.Event;
using JHchoi.Module;
using JHchoi.Contents.Event;

namespace JHchoi.Contents
{
	public class MathProblemContent : IFantaBoxContent
    {
        PointerEventData pointEventData = new PointerEventData(null);
        GraphicRaycaster Raycaster;

        Camera UICamera = null;

        protected override void OnLoadStart()
        {
            var moudule = ModuleManager.Instance.GetModule<BaseCameraModule>(ModuleName.BaseCamera);
            UICamera = moudule.GetComponent<BaseCameraModule>().GetCamera();
            Raycaster = moudule.GetComponent<BaseCameraModule>().GetRaycaster();

            SetLoadComplete();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                //마우스 눌렀을때 해당위치에 Rect생성
                if (UICamera == null)
                    return;

                List<Rect> rects = new List<Rect>();
                Vector2 screenPoint = UICamera.ScreenToViewportPoint(Input.mousePosition);
                rects.Add(new Rect(new UnityEngine.Rect(screenPoint, new Vector2(0.1f, 0.1f))));  
                
                Message.Send<TouchRectMsg>(new TouchRectMsg(rects));
            }
        }

        protected override void OnEnter()
        {
            Message.Send<FadeOutMsg>(new FadeOutMsg());

            SoundManager.Instance.PlaySound((int)SoundType_GameBGM.MathProblem);

            float max, min = 0;
            max = Screen.width < Screen.height ? Screen.width : Screen.height;
            min = Screen.width < Screen.height ? Screen.height : Screen.width;

            float temp = 0;
            while (max % min != 0)
            {
                temp = max % min;
                max = min;
                min = max;
            }

            float gcd = min;
            float har = (float)Screen.height / gcd;

            if (har == 0.625f)
                UI.IDialog.RequestDialogEnter<UI.MathProblemDialog_3>();
            else if (har == 0.5625f)
                UI.IDialog.RequestDialogEnter<UI.MathProblemDialog_2>();
            else
                UI.IDialog.RequestDialogEnter<UI.MathProblemDialog_1>();

            InitProblem();
        }

        protected override void OnExit()
        {
            UI.IDialog.RequestDialogExit<UI.MathProblemDialog_1>();
            UI.IDialog.RequestDialogExit<UI.MathProblemDialog_2>();
            UI.IDialog.RequestDialogExit<UI.MathProblemDialog_3>();

            SoundManager.Instance.StopSound((int)SoundType_GameBGM.MathProblem);
        }

        protected override void OnAddMessage()
        {
            Message.AddListener<FinishAnswerProductionMsg>(OnFinishAnswerProduction);
        }

        protected override void OnRemoveMessage()
        {
            Message.RemoveListener<FinishAnswerProductionMsg>(OnFinishAnswerProduction);
        }

        void InitProblem()
        {
            var mpm = Model.First<MathProblemModel>();
            mpm.ResetProblem();

            var msg = new MathProblemDialogInfoMsg();
            msg.Problem = "";
            msg.Questions = new List<string>() { "","","","" };
            Message.Send<MathProblemDialogInfoMsg>(msg);

            IContent.RequestContentEnter<ReadyContent>();
        }

        protected override void OnPlay()
        {
            // BGM 사운드 출력
            SetProblem();
        }

        void SetProblem()
        {
            var mpm = Model.First<MathProblemModel>();
            var info = mpm.GetNextProblem();

            var msg = new MathProblemDialogInfoMsg();
            msg.Problem = info.Problem;
            msg.Questions = new List<string>(info.Questions);

            Message.Send<MathProblemDialogInfoMsg>(msg);
        }

        protected override void OnTouchRect(TouchRectMsg msg)
        {
            if (UICamera == null)
            {
                Debug.Log("ViewPortCamera is None");
                return;
            }

            var mpm = Model.First<MathProblemModel>();
            var info = mpm.GetProblem();

            for (int i = 0; i < msg.TouchRects.Count; i++)
            {
                var rect = msg.TouchRects[i];
                float k1Plus = rect.width * 0.1f;
                if (rect.width <= 0.1f)
                    k1Plus = rect.width;

                float k2Plus = rect.height * 0.1f;
                if (rect.height <= 0.1f)
                    k2Plus = rect.height;

                for (float k1 = rect.x; k1 < rect.x + rect.width; k1 += k1Plus)
                {
                    for (float k2 = rect.y; k2 < rect.y + rect.height; k2 += k2Plus)
                    {
                        Vector3 ScreenPoint = UICamera.ViewportToScreenPoint(new Vector3(k1, k2));
                        List<RaycastResult> result = new List<RaycastResult>();
                        pointEventData.position = ScreenPoint;
                        Raycaster.Raycast(pointEventData, result);

                        if (result.Count != 0)
                        {
                            for (int p = 0; p < result.Count; p++)
                            {
                                string objName = result[p].gameObject.name;
                                objName = objName.Replace("NO_", "");
                                int index = int.Parse(objName);
                                if (index < 1 || 4 < index)
                                    return;

                                Message.Send<MathProblemAnswerMsg>(new MathProblemAnswerMsg(index == info.AnswerIndex, index - 1));
                            }
                        }
                    }
                }
            }
        }

        void OnFinishAnswerProduction(FinishAnswerProductionMsg msg)
        {
            SetProblem();
        }

        protected override void OnEnd()
        {
            // BGM 사운드 종료
        }

        protected override void OnHit(GameObject obj)
        {
            throw new System.NotImplementedException();
        }
    }
}