using System;
using UnityEngine;
using System.Collections.Generic;
using JHchoi.Constants;
using System.Text;
using System.IO;


namespace JHchoi.Models
{
    public class MathProblemInfo
    {
        public int Index = 0;
        public string Problem = "";
        public List<string> Questions = new List<string>();
        public int AnswerIndex = 0;
    }

    public class MathProblemModel : Model
    {
        GameModel _owner;
        int ProblemIndex = 0;
        List<MathProblemInfo> Problems = new List<MathProblemInfo>();
        int WrongAnswer = 0;
        int Answer = 0;

        static string fileName = "MathProblem";

        public void Setup(GameModel owner)
        {
            _owner = owner;

            LoadFile();
            ResetProblem();
        }

        void LoadFile()
        {
            var data = Util.Instance.ReadCSV(fileName);

            for (int i = 0; i < data.Count; i++)
            {
                MathProblemInfo info = new MathProblemInfo();
                info.Index = int.Parse(data[i]["Index"].ToString());
                info.Problem = data[i]["Problem"].ToString();
                info.Questions.Add(data[i]["Question_1"].ToString());
                info.Questions.Add(data[i]["Question_2"].ToString());
                info.Questions.Add(data[i]["Question_3"].ToString());
                info.Questions.Add(data[i]["Question_4"].ToString());
                info.AnswerIndex = int.Parse(data[i]["Answer"].ToString());

                Problems.Add(info);
            }
        }

        public void ResetProblem()
        {
            WrongAnswer = 0;
            Answer = 0;
            ProblemIndex = 0;
            Problems.Shuffle<MathProblemInfo>();
        }

        public void ProblemAnswer(bool answer)
        {
            if (answer)
                Answer++;
            else
                WrongAnswer++;
        }

        public int GetAnswerCount()
        {
            return Answer;
        }

        public int GetWrongAnswerCount()
        {
            return WrongAnswer;
        }

        public MathProblemInfo GetProblem()
        {
            MathProblemInfo info = Problems[ProblemIndex];
            return info;
        }

        public MathProblemInfo GetNextProblem()
        {
            ProblemIndex++;
            if (Problems.Count <= ProblemIndex)
                ProblemIndex = 0;

            return GetProblem();
        }
    }
}
