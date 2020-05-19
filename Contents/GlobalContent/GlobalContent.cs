using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CellBig.Models;
using CellBig.Contents.Event;
using CellBig.UI.Event;

namespace CellBig.Contents
{
	public class GlobalContent : IContent
	{
        GameModel gm;

        Coroutine shakeCamera_Cor;
        Coroutine colorCamera_Cor;

        protected override void OnEnter()
		{
            gm = Model.First<GameModel>();

            UI.IDialog.RequestDialogEnter<UI.KioskGlobalDialog>();
            UI.IDialog.RequestDialogEnter<UI.ScreenGlobalDialog>();
        }

		protected override void OnExit()
		{
            UI.IDialog.RequestDialogExit<UI.KioskGlobalDialog>();
            UI.IDialog.RequestDialogExit<UI.ScreenGlobalDialog>();
        }

        protected override void AddMessage()
        {
            Message.AddListener<MainCameraMsg>(OnMainCameraMsg);
            Message.AddListener<ShakeCameraMsg>(OnShakeCameraMsg);
        }

        protected override void RemoveMessage()
        {
            Message.RemoveListener<MainCameraMsg>(OnMainCameraMsg);
            Message.RemoveListener<ShakeCameraMsg>(OnShakeCameraMsg);
        }

        void OnMainCameraMsg(MainCameraMsg msg)
        {
            #if UNITY_EDITOR
            Debug.LogError("MainCamera : " + msg.MainCamera);
            #endif

            mainCamera = msg.MainCamera;
            gm.playContent.Model.mainCamera = mainCamera;
        }

        void OnShakeCameraMsg(ShakeCameraMsg msg)
        {
            if (shakeCamera_Cor != null)
                StopCoroutine(shakeCamera_Cor);

            shakeCamera_Cor = StartCoroutine(ShakeCamera(msg.IsX, msg.IsY, msg.Duration, msg.Amount));
        }

        IEnumerator ShakeCamera(bool isX, bool isY, float duration = 1.0f, float amount = 5.0f)
        {
            Vector3 orignalPosition = mainCamera.transform.position;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                float x = 0.0f;
                float y = 0.0f;

                if(isX)
                    x = Random.Range(-1f, 1f) * amount;
                if(isY)
                    y = Random.Range(-1f, 1f) * amount;

                mainCamera.transform.position = orignalPosition;
                mainCamera.transform.localPosition = new Vector3(mainCamera.transform.localPosition.x + x, mainCamera.transform.localPosition.y + y, mainCamera.transform.localPosition.z);

                elapsed += Time.deltaTime;
                yield return 0;
            }

            mainCamera.transform.position = orignalPosition;
            mainCamera.transform.localPosition = Vector3.zero;
        }
    }
}
