using CellBig.Constants;
using CellBig.Constants.FishCatch;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CellBig.Models
{
    public class CommonModel : Model
    {
        GameModel _owner;
        StreamingCSVLoader fileData = new StreamingCSVLoader();
        Dictionary<string, string> MapBackGroundName = new Dictionary<string, string>();
        Dictionary<string, int> MapPlayerCount = new Dictionary<string, int>();
        Dictionary<string, float> MapPlayTimeSec = new Dictionary<string, float>();
        Dictionary<string, float> MpaResetTimeSec = new Dictionary<string, float>();
        Dictionary<string, float> MapCatchDistance = new Dictionary<string, float>();
        Dictionary<string, float> MapInnerDistance = new Dictionary<string, float>();

        public FishGameMode FishGameMode = FishGameMode.Spring;
        public void Setup(string fileName, GameModel owner)
        {
            _owner = owner;
            fileData.Load("FishCatchSetting/" + fileName + ".CSV", CsvLoaded);
        }

        void CsvLoaded()
        {
            var datas = fileData.GetValue("Index");

            foreach (var o in datas)
            {
                var index = fileData.GetEqualsIndex("Index", o);
                var content = fileData.GetValue("Content", index);
                var backGroundName = fileData.GetValue("BackGroundName", index);
                var playerCount = fileData.GetValue("PlayerCount", index);
                var playTimeSec = fileData.GetValue("PlayTimeSec", index);
                var playResetTimeSec = fileData.GetValue("PlayResetTime", index);
                var catchDistance = fileData.GetValue("CatchDistance", index);
                var InnerDistance = fileData.GetValue("InnerDistance", index);

                MapBackGroundName.Add(content, backGroundName);
                MapPlayerCount.Add(content, int.Parse(playerCount));
                MapPlayTimeSec.Add(content, float.Parse(playTimeSec));
                MpaResetTimeSec.Add(content, float.Parse(playResetTimeSec));
                MapCatchDistance.Add(content, float.Parse(catchDistance));
                MapInnerDistance.Add(content, float.Parse(InnerDistance));
            }
        }

        public int GetPlayerCount(string content)
        {
            return MapPlayerCount[content];
        }

        public string GetBackGroundName(string content)
        {
            return MapBackGroundName[content];
        }

        public float GetPlayTime(string content)
        {
            return MapPlayTimeSec[content];
        }

        public float GetResetTime(string content)
        {
            return MpaResetTimeSec[content];
        }

        public float GetCatchDistance(string content)
        {
            return MapCatchDistance[content];
        }

        public float GetInnerDistance(string content)
        {
            return MapInnerDistance[content];
        }
    }
}