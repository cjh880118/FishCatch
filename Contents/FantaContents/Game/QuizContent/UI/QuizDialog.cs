using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using CellBig.UI.Event;

namespace CellBig.UI
{
    public class QuizDialog : IDialog
    {
        public Image quizImage;
        public Sprite[] sprites;

        protected override void OnEnter()
        {
            AddMessage();
        }

        protected override void OnExit()
        {
            RemoveMessage();
        }

        void AddMessage()
        {
            Message.AddListener<QuizMsg>(OnQuestionMsg);
        }

        void RemoveMessage()
        {
            Message.RemoveListener<QuizMsg>(OnQuestionMsg);
        }

        void OnQuestionMsg(QuizMsg msg)
        {
            int meshType = (int)msg.MeshType;
            int patternType = (int)msg.PatternType;
            int spriteIndex = (meshType * Enum.GetNames(typeof(MeshType)).Length) + patternType;

            quizImage.sprite = sprites[spriteIndex];
        }
    }
}