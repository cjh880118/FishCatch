using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace JHchoi.Models
{
    public class FishModel : Model
    {
        GameModel _owner;
        StreamingCSVLoader fileData = new StreamingCSVLoader();
        Dictionary<int, string> MapPrefabName = new Dictionary<int, string>();
        Dictionary<int, int> MapFishCout = new Dictionary<int, int>();
        Dictionary<int, float> MapFishRespawnSec = new Dictionary<int, float>();
        Dictionary<int, float> MapFishSizeMin = new Dictionary<int, float>();
        Dictionary<int, float> MapFishSizeMaX = new Dictionary<int, float>();
        Dictionary<int, float> MapFishSpeedMin = new Dictionary<int, float>();
        Dictionary<int, float> MapFishSpeedMaX = new Dictionary<int, float>();
        Dictionary<int, float> MapFishCatchDelay = new Dictionary<int, float>();
        Dictionary<int, float> MapFishViewPositionZ = new Dictionary<int, float>();
        Dictionary<int, string> MapFishMainName = new Dictionary<int, string>();

        SettingModel sm;

        public void Setup(string fileName, GameModel owner)
        {
            sm = Model.First<SettingModel>();
            _owner = owner;
            fileData.Load("FishCatchSetting/" + fileName + ".CSV", CsvLoaded);
        }

        void CsvLoaded()
        {
            var datas = fileData.GetValue("Index");

            foreach (var o in datas)
            {
                var index = fileData.GetEqualsIndex("Index", o);
                var prefabName = fileData.GetValue("PrefabName", index);
                var fishCount = fileData.GetValue("FishCount", index);
                var fishRespawnSec = fileData.GetValue("FishRespawnSec", index);
                var fishSizeMin = fileData.GetValue("FishSizeMin", index);
                var fishSizeMax = fileData.GetValue("FishSizeMax", index);
                var fishSpeedMin = fileData.GetValue("FishSpeedMin", index);
                var fishSpeedMax = fileData.GetValue("FishSpeedMax", index);
                var fishCatchDelay = fileData.GetValue("CatchDelay", index);
                var fishViewPositionZ = fileData.GetValue("CatchViewPositionZ", index);

                MapPrefabName.Add(index, prefabName);
                MapFishCout.Add(index, int.Parse(fishCount));
                MapFishRespawnSec.Add(index, float.Parse(fishRespawnSec));
                MapFishSizeMin.Add(index, float.Parse(fishSizeMin));
                MapFishSizeMaX.Add(index, float.Parse(fishSizeMax));
                MapFishSpeedMin.Add(index, float.Parse(fishSpeedMin));
                MapFishSpeedMaX.Add(index, float.Parse(fishSpeedMax));
                MapFishCatchDelay.Add(index, float.Parse(fishCatchDelay));
                MapFishViewPositionZ.Add(index, float.Parse(fishViewPositionZ));

                if (sm.LocalizingType == Constants.LocalizingType.KR)
                {
                    var fishMainName = fileData.GetValue("KRName", index);
                    MapFishMainName.Add(index, fishMainName);
                }
                if (sm.LocalizingType == Constants.LocalizingType.JP)
                {
                    var fishMainName = fileData.GetValue("JPName", index);
                    MapFishMainName.Add(index, fishMainName);
                }
            }
        }


        public string PrefabName(int index) { return MapPrefabName[index]; }
        public int FishCount(int index) { return MapFishCout[index]; }
        public float FishRespawnSec(int index) { return MapFishRespawnSec[index]; }
        public float FishSizeMin(int index) { return MapFishSizeMin[index]; }
        public float FishSizeMax(int index) { return MapFishSizeMaX[index]; }
        public float FishSpeedMin(int index) { return MapFishSpeedMin[index]; }
        public float FishSpeedMax(int index) { return MapFishSpeedMaX[index]; }
        public float FishCatchDelay(int index) { return MapFishCatchDelay[index]; }
        public float FishViewPosZ(int index) { return MapFishViewPositionZ[index]; }
        public string FishMainName(int index) { return MapFishMainName[index]; }
    }
}