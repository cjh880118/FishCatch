using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JHchoi.Constants;

namespace JHchoi.Models
{
    public class GameModel : Model
    {
        //여기에는 테이블 같은 정적 데이터(모드별 Default 값)
        public ModelRef<SettingModel> setting = new ModelRef<SettingModel>(); // 게임세팅
        public ModelRef<LocalizingModel> localizing = new ModelRef<LocalizingModel>(); // 로컬라이징
        public ModelRef<PlayContentModel> playContent = new ModelRef<PlayContentModel>(); // 게임순서 / 정보
        public ModelRef<MathProblemModel> mathProblem = new ModelRef<MathProblemModel>();
        public ModelRef<FantaBoxSettingModel> fantaBoxSetting = new ModelRef<FantaBoxSettingModel>();
        public ModelRef<MaskPatternModel> mask = new ModelRef<MaskPatternModel>();


        //물고기 잡기 모델
        //public ModelRef<SettingModel> setting = new ModelRef<SettingModel>();
        public ModelRef<CommonModel> common = new ModelRef<CommonModel>();
        public ModelRef<FishModel> fish = new ModelRef<FishModel>();

        public enum UangelNameType
        {
            None,
            LittleStar,
            MoonLight,
        }
        public UangelNameType nameType = UangelNameType.None;

        // 키넥트 관련 사항 추가

        public bool Sound = true;       // 사운드 On / Off
        public bool PlayStop = false;   // 게임 플레이 On / Off

        public void Setup()
        {
            setting.Model = new SettingModel();
            setting.Model.Setup(this);

            localizing.Model = new LocalizingModel();
            localizing.Model.Setup(this);

            playContent.Model = new PlayContentModel();
            playContent.Model.Setup(this);

            mathProblem.Model = new MathProblemModel();
            mathProblem.Model.Setup(this);

            fantaBoxSetting.Model = new FantaBoxSettingModel();
            fantaBoxSetting.Model.Setup(this);

            mask.Model = new MaskPatternModel();
            mask.Model.Setup(this);

            //물고기 잡기
            common.Model = new CommonModel();
            common.Model.Setup("CommonSetting", this);

            fish.Model = new FishModel();
            fish.Model.Setup("FishSetting", this);
        }
    }
}

