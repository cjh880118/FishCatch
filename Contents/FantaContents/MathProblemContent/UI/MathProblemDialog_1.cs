using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace JHchoi.UI
{
    public class MathProblemDialog_1 : IDialog
    {
        public Text Problem_Text = null;
        public List<Text> Questions_Text = new List<Text>();

        public List<GameObject> Answers = new List<GameObject>();
        public List<GameObject> WrongAnswers = new List<GameObject>();

        bool Production = false;
        List<bool> WrongAnswerProductions = new List<bool>();

        private void Start()
        {
            for (int i = 0; i < 4; i++)
                WrongAnswerProductions.Add(false);
        }

        protected override void OnEnter()
        {
            Message.AddListener<Event.MathProblemDialogInfoMsg>(OnSetDialog);
            Message.AddListener<Event.MathProblemAnswerMsg>(OnMathProblemAnswer);
        }

        protected override void OnExit()
        {
            StopAllCoroutines();

            Message.RemoveListener<Event.MathProblemDialogInfoMsg>(OnSetDialog);
            Message.RemoveListener<Event.MathProblemAnswerMsg>(OnMathProblemAnswer);
        }

        void DataReset()
        {
            Production = false;

            for (int i = 0; i < 4; i++)
                WrongAnswerProductions[i] = false;

            for (int i = 0; i < Answers.Count; i++)
                Answers[i].SetActive(false);

            for (int i = 0; i < WrongAnswers.Count; i++)
                WrongAnswers[i].SetActive(false);
        }

        void OnSetDialog(Event.MathProblemDialogInfoMsg msg)
        {
            DataReset();

            Problem_Text.text = msg.Problem;

            for (int i = 0; i < msg.Questions.Count; i++)
                Questions_Text[i].text = msg.Questions[i];
        }

        void OnMathProblemAnswer(Event.MathProblemAnswerMsg msg)
        {
            if (Production)
                return; 

            if (msg.Answer)
                StartCoroutine(AnswerProduction(Answers[msg.SelectIndex]));
            else
                StartCoroutine(WrongAnswerProduction(msg.SelectIndex));
        }

        IEnumerator AnswerProduction(GameObject obj)
        {
            if (Production)
                yield break;

            Production = true;

            obj.SetActive(true);
            var img = obj.GetComponent<Image>();
            img.fillAmount = 0.0f;
            img.DOFillAmount(1f, 0.2f);

            Message.Send<UI.Event.ADDScore>(new UI.Event.ADDScore(100));
            SoundManager.Instance.PlaySound((int)SoundType_GameFX.MathProblem_Ok);

            yield return new WaitForSeconds(1.5f);
            Message.Send<Event.FinishAnswerProductionMsg>(new Event.FinishAnswerProductionMsg());
        }

        IEnumerator WrongAnswerProduction(int index)
        {
            if (WrongAnswerProductions[index])
                yield break;

            WrongAnswerProductions[index] = true;
            WrongAnswers[index].SetActive(true);
            WrongAnswers[index].transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            WrongAnswers[index].transform.DOScale(0.5f, 0.3f);

            yield return new WaitForSeconds(1.5f);

            WrongAnswers[index].SetActive(false);
            WrongAnswerProductions[index] = false;
        }
    }
}

