using System;
using UnityEngine;
using System.Collections.Generic;
using JHchoi.Constants;
using System.Text;
using System.IO;

namespace JHchoi.Models
{
    public class MaskPatternInfo
    {
        public int Index = 0;
        public List<List<int>> Lists = new List<List<int>>();
    }

    public class MaskPatternModel : Model
    {
        GameModel _owner;
        List<MaskPatternInfo> Patterns = new List<MaskPatternInfo>();

        static string fileName = "mask_pattern_";

        public void Setup(GameModel owner)
        {
            _owner = owner;

            LoadFile();
        }

        void LoadFile()
        {
            for (int i = 0; i < 7; i++)
            {
                string name = String.Format("Mask/{0}{1}", fileName, (i+1).ToString());
                var data = Util.Instance.ReadCSV(name);

                MaskPatternInfo info = new MaskPatternInfo();
                for (int k = 0; k < data.Count; k++)
                {
                    List<int> temp = new List<int>();

                    foreach (object pObj in data[k].Values)
                        temp.Add(int.Parse(pObj.ToString()));

                    info.Lists.Add(temp);
                }

                Patterns.Add(info);
            }
        }

        public MaskPatternInfo GetPatternInfo()
        {
            Patterns.Shuffle<MaskPatternInfo>();
            return Patterns[0];
        }
    }
}
