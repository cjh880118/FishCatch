using UnityEngine;
using System.Collections.Generic;
using JHchoi.Contents;
using JHchoi.Constants;
using JHchoi.Models;

namespace JHchoi.UI.Event
{
    public class MathProblemDialogInfoMsg : Message
    {
        public string Problem = "";
        public List<string> Questions = new List<string>();
    }

    public class MathProblemAnswerMsg : Message
    {
        public bool Answer = false;
        public int SelectIndex = 0;

        public MathProblemAnswerMsg (bool answer, int selectIndex)
        {
            Answer = answer;
            SelectIndex = selectIndex;
        }
    }

    public class FinishAnswerProductionMsg : Message
    {

    }
}