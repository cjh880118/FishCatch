using UnityEngine;
using System.Collections.Generic;
using CellBig.Contents;
using CellBig.Constants;
using CellBig.Models;

namespace CellBig.UI.Event
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