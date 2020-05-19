#define LOAD_FROM_ASSETBUNDLE

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JHchoi.Common;
using DG.Tweening;
using JHchoi.Models;

namespace JHchoi
{


    public enum SoundType_System
    {
        None = 0,
        Ready = 1001,
        Go = 1002,
        Clear = 1003

    }


    public enum SoundType_GameBGM
    {
        None = 0,
        GameShooter = 2012,
        AnimalHead = 4000,
        BlockBreak = 5000,
        Skeleton = 6000,
        Weston = 7000,
        HatchDragon = 8000,
        MeteoShower = 9000,
        Vertex = 10000,
        Paints = 11000,
        Quiz = 12000,
        RoadDestruction = 13000,
        SpaceJump = 14000,
        Fallen = 15000,
        SlideSnow = 16000,
        SlideLAVA = 17000,
        SlideFrozenWater = 18000,
        WaterFish = 19000,
        Grass = 20000,
        RockHolder = 21000,
        DirtRoom = 22000,
        Mole = 23000,
        Fruit = 24000,
        Picture = 25000,
        Slime = 26000,
        Octopus = 27000,
        FireFighter = 28000,
        Waste = 29000,
        FireWork = 30000,
        Mask = 31000,
        MathProblem = 32000,
        MeteoBreak = 33000,

        NerfDungeon = 100000,
        NerfSF = 100100,
        NerfForest = 100200,
        NerfAlien = 100300,
        NerfCathedral = 100400,
        NerfSnowTown = 100500,
        NerfDesert = 100600,

        LaserMasterpiece = 101000,
        LaserDrawing = 101100,
        LaserWindow = 101200,

        LaserInterior = 300000,
        LaserExterior = 300100,
        LaserGhost = 300200,
        LaserVirus = 300300,
    }

    public enum SoundType_GameFX
    {
        None = 0,
        Shot = 2502,
        Slug_Explosion = 2503,

        AnimalHead_Explosion = 4001,
        AnimalHead_Balloon = 4002,
        AnimalHead_Bomb = 4003,
        AnimalHead_Cat = 4004,
        AnimalHead_Cow = 4005,
        AnimalHead_Dog = 4006,
        AnimalHead_Duck = 4007,
        AnimalHead_Eagle = 4008,

        BlockBreak_HitBubble = 5001,
        BlockBreak_HitFreezeBubble = 5002,
        BlockBreak_HitIceBomb = 5003,
        BlockBreak_FreezingBubble = 5004,
        BlockBreak_InitBubble = 5005,

        Skeleton_Laugh = 6001,
        Skeleton_Explosion = 6002,
        Skeleton_AvoidSkull = 6003,
        Skeleton_FuseBomb = 6004,
        Skeleton_Death = 6005,

        Weston_Explosion = 7001,
        Weston_FuseBomb = 7002,
        Weston_InitTarget = 7003,
        Weston_ShotgunFire = 7004,

        HatchDragon_DragonCry1 = 8001,
        HatchDragon_DragonCry2 = 8002,
        HatchDragon_EggCrack = 8003,

        MeteoShower_Explosion = 9001,

        Vertex_Hit = 10001,

        Paints_Hit = 11001,

        Quiz_Correct = 12001,
        Quiz_Wrong = 12002,

        RoadDestruction_Impact = 13001,
        RoadDestruction_Rock0 = 13002,
        RoadDestruction_Rock1 = 13003,
        RoadDestruction_Rock2 = 13004,
        RoadDestruction_Rock3 = 13005,
        RoadDestruction_TapleRe = 13006,

        SpaceJump_Chatter = 14001,
        SpaceJump_Space = 14002,
        SpaceJump_Wind = 14003,
        SpaceJump_Cloud1 = 14004,
        SpaceJump_Cloud2 = 14005,

        Fallen_Step1 = 15001,
        Fallen_Step2 = 15002,
        Fallen_Step3 = 15003,

        SlideSnow_Flake1 = 16001,
        SlideSnow_Flake2 = 16002,

        SlideLAVA_Sizzle = 17001,
        SlideLAVA_Step1 = 17002,
        SlideLAVA_Step2 = 17003,
        SlideLAVA_Flame1 = 17004,
        SlideLAVA_Flame2 = 17005,
        SlideLAVA_Flame3 = 17006,
        SlideLAVA_TapleRe = 17007,

        SlideFrozenWater_Wind = 18001,
        SlideFrozenWater_BreakIce1 = 18002,
        SlideFrozenWater_BreakIce2 = 18003,
        SlideFrozenWater_TapleRe = 18004,

        WaterFish_Valley = 19001,
        WaterFish_Step = 19002,

        Grass_Wind = 20001,
        Grass_Step = 20002,

        RockHolder_Valley = 21001,
        RockHolder_Hit1 = 21002,
        RockHolder_Hit2 = 21003,
        RockHolder_Hit3 = 21004,
        RockHolder_Hit4 = 21005,
        RockHolder_Break1 = 21006,
        RockHolder_Break2 = 21007,
        RockHolder_Break3 = 21008,
        RockHolder_Stay = 21009,
        RockHolder_Die1 = 21010,
        RockHolder_Die2 = 21011,

        DirtRoom_Shoes = 22001,
        DirtRoom_Frog = 22002,
        DirtRoom_Horse = 22003,
        DirtRoom_Dog1 = 22004,
        DirtRoom_Walker1 = 22005,
        DirtRoom_Camel = 22006,
        DirtRoom_Bear = 22007,
        DirtRoom_Duck = 22008,
        DirtRoom_Dog2 = 22009,
        DirtRoom_Walker2 = 22010,
        DirtRoom_Doll1 = 22011,
        DirtRoom_Doll2 = 22012,
        DirtRoom_Doll3 = 22013,
        DirtRoom_Plastic = 22014,
        DirtRoom_Bucket = 22015,
        DirtRoom_Paint = 22016,

        Mole_Explosion = 23001,
        Mole_Fuse = 23002,
        Mole_Init = 23003,
        Mole_Hit = 23004,

        Fruit_Init = 24001,
        Fruit_Hit1 = 24002,
        Fruit_Hit2 = 24003,
        Fruit_Explosion = 24004,

        Picture_BubbleInit = 25001,
        Picture_Clear = 25002,
        Picture_Boom = 25003,
        Picture_Hit = 25004,
        Picture_IceHIt = 25005,
        Pictrue_BoomHit = 25006,

        Slime_Die = 26001,
        Slime_Jump = 26002,
        Slime_Down = 26003,

        Octopus_AMB = 27001,
        Octopus_InitPot = 27002,
        Octopus_Break = 27003,
        Octopus_Move = 27004,
        Octopus_Shit = 27005,
        Octopus_Hit = 27006,

        FireFighter_Burning = 28001,
        FireFighter_Siren = 28002,
        FireFighter_Init = 28003,
        FireFighter_Big = 28004,
        FireFighter_Hit = 28005,
        FireFighter_Off = 28006,

        Waste_Hit = 29001,

        FireWork_Move = 30001,
        FireWork_Explosion = 30002,

        Mask_Init = 31001,
        Mask_Hit = 31002,
        Mask_Break = 31003,
        Mask_Alarm = 31005,
        Mask_Monster = 31006,

        MathProblem_Ok = 32001,

        MeteoBreak_Explosion = 33001,

        NerfDungeon_Appear = 100001,
        NerfDungeon_BossAttack = 100002,
        NerfDungeon_BossDie = 100003,
        NerfDungeon_Hit = 100004,
        NerfDungeon_StageEnd = 100005,
        NerfDungeon_Victory = 100006,
        NerfDungeon_Walk = 100007,
        NerfDungeon_Dragon = 100008,
        NerfDungeon_Damage = 100009,
        NerfDungeon_EnemyAttack = 100010,

        NerfSF_Amb = 100101,
        NerfSF_Appear = 100102,
        NerfSF_BossAttack = 100103,
        NerfSF_BossDie = 100104,
        NerfSF_Damage = 100105,
        NerfSF_Hit = 100106,
        NerfSF_StageEnd = 100107,
        NerfSF_Victory = 100108,
        NerfSF_Walk = 100109,
        NerfSF_EnemyAttack = 100110,

        NerfForest_Amb = 100201,
        NerfForest_Appear = 100202,
        NerfForest_BossAttack = 100203,
        NerfForest_BossDie = 100204,
        NerfForest_Damage = 100205,
        NerfForest_Hit = 100206,
        NerfForest_StageEnd = 100207,
        NerfForest_Victory = 100208,
        NerfForest_Walk = 100209,
        NerfForest_EnemyAttack = 100210,

        NerfAlien_Amb = 100301,
        NerfAlien_Appear = 100302,
        NerfAlien_BossAttack = 100303,
        NerfAlien_BossDie = 100304,
        NerfAlien_Damage = 100305,
        NerfAlien_Hit = 100306,
        NerfAlien_StageEnd = 100307,
        NerfAlien_Victory = 100308,
        NerfAlien_Walk = 100309,
        NerfAlien_EnemyAttack = 100310,

        NerfCathedral_Walk = 100401,
        NerfCathedral_Appear = 100402,
        NerfCathedral_BossAttack = 100403,
        NerfCathedral_BossDie = 100404,
        NerfCathedral_CharAttack = 100405,
        NerfCathedral_Hit = 100406,
        NerfCathedral_HitEnemy = 100407,
        NerfCathedral_HitNeutral = 100408,
        NerfCathedral_StageEnd = 100409,
        NerfCathedral_Victory = 100410,
        NerfCathedral_EnemyAttack = 100411,

        NerfSnowTown_Walk = 100501,
        NerfSnowTown_Appear = 100502,
        NerfSnowTown_BossAttack = 100503,
        NerfSnowTown_BossDie = 100504,
        NerfSnowTown_CharAttack = 100505,
        NerfSnowTown_Hit = 100506,
        NerfSnowTown_HitEnemy = 100507,
        NerfSnowTown_HitNeutral = 100508,
        NerfSnowTown_StageEnd = 100509,
        NerfSnowTown_Victory = 100510,
        NerfSnowTown_EnemyAttack = 100511,

        NerfDesert_Walk = 100601,
        NerfDesert_Appear = 100602,
        NerfDesert_BossAttack = 100603,
        NerfDesert_BossDie = 100604,
        NerfDesert_CharAttack = 100605,
        NerfDesert_Hit = 100606,
        NerfDesert_HitEnemy = 100607,
        NerfDesert_HitNeutral = 100608,
        NerfDesert_StageEnd = 100609,
        NerfDesert_Victory = 100610,
        NerfDesert_EnemyAttack = 100611,

        LaserMasterpiece_Rub = 101001,
        LaserMasterpiece_Clear = 101002,

        LaserDrawing_Rub = 101101,
        LaserDrawing_Clear = 101102,

        LaserWindow_Rub = 101201,
        LaserWindow_Clear = 101202,

        LaserInterior_Amb = 300001,
        LaserInterior_Hit = 300002,
        LaserInterior_Off = 300003,
        LaserInterior_Shoot = 300004,
        LaserInterior_Restart = 300005,

        LaserExterior_Amb = 300101,
        LaserExterior_Hit = 300102,
        LaserExterior_Off = 300103,
        LaserExterior_Shoot = 300104,
        LaserExterior_Restart = 300105,

        LaserGhost_Summon = 300201,
        LaserGhost_MonHit = 300202,
        LaserGhost_MonDie = 300203,
        LaserGhost_Wind = 300204,
        LaserGhost_Restart = 300205,

        LaserVirus_Summon = 300301,
        LaserVirus_MonHit = 300302,
        LaserVirus_MonDie = 300303,
        LaserVirus_Wind = 300304,
        LaserVirus_Restart = 300305,
    }

    public enum SoundType_Uangel
    {
        None = 0,
        BGM_LittleStar = 3100,
        BGM_MoonLight = 3200,
    }

    public enum SoundType_UangelFx
    {
        LittleStar_Moon = 3101,
        LittleStar_Stars = 3102,
        LittleStar_Cloud = 3103,
        LittleStar_Tree = 3104,
        LittleStar_Town = 3105,
        Uangel_Firework = 3106,
        Uangel_Ground = 3107,
        Uangel_ChagneSceneStar = 3108,

        MoonLight_Stars = 3201,
        MoonLight_FallFruit = 3202,
        MoonLight_SetFruit = 3203,
        MoonLight_Lamp = 3204,
        MoonLight_Sunflower = 3205,
        MoonLight_Door = 3206,
        MoonLight_Dog = 3207,
        MoonLight_Cat1 = 3208,
        MoonLight_Cat2 = 3209,
        MoonLight_CatDog = 3210,
    }

    public enum SoundFishCatch
    {
        Spring_Bgm = 200000,
        Spring_Amb,
        Spring_Sfx_Catch,
        Sfx_CatchIng,
        Sfx_Fail,
        Summer_Bgm,
        Summer_Amb1,
        Summer_Amb2,
        Fall_Bgm,
        Fall_Amb,
        Bug_Bgm,
        Bug_Amb,
        Bug_Catch,
        Bug_Fail,
        Bug_PopUp,
        Bug_Walk,
        Bug_CatchAfter,
        Sea_Bgm,
        Sea_Amb1,
        Sea_Amb2,
        Sea_CatchAfter,
        Sea_Fail,
        Sea_CatchWater,
        Sushi_Bgm,
        Sushi_Amb,
        Sushi_PopUp,
        Sushi_Right,
        Shshi_Fail,
        Shshi_Catch,
        Dessert_Bgm,
        Dessert_Amb,
        Dessert_PopUp,
        Dessert_Right,
        Dessert_Fail,
        Dessert_Catch,
    }

    public class SoundManager : MonoSingleton<SoundManager>
    {
        public float volume = 1.0f;
        public float fadeDuration = 1.0f;

        BT_Sound _table;

        bool _loadComplete = false;
        public bool IsComplete { get { return _loadComplete; } }

        class ClipCache
        {
            public string resourceName;
            public BT_Sound.Param data;
            public AudioClip clip;
            public bool instant;
            public bool bgm;
        }

        //private void Update()
        //{
        //    if (Input.GetKeyDown(KeyCode.Alpha1))
        //    {
        //        for (int i = 0; i < 4; i++)
        //        {
        //            PlaySound(1);
        //        }
        //    }
        //    else if (Input.GetKeyDown(KeyCode.Alpha2))
        //    {
        //        for (int i = 0; i < 4; i++)
        //        {
        //            StopSound(1, 4 - i);
        //        }
        //    }
        //}

        readonly Dictionary<int, ClipCache> _caches = new Dictionary<int, ClipCache>();

        readonly ObjectPool<AudioSource> _audioSourcePool = new ObjectPool<AudioSource>();

        class PlayingAudio
        {
            public ClipCache clipCache;
            public AudioSource audioSource;
        }
        readonly LinkedList<PlayingAudio> _playingAudio = new LinkedList<PlayingAudio>();

        public IEnumerator Setup()
        {
            _loadComplete = false;
            _table = TableManager.Instance.GetTableClass<BT_Sound>();

            for (int i = 0; i < _table.sheets[0].list.Count; i++)
                yield return StartCoroutine(Load(_table.sheets[0].list[i].Index));

            _loadComplete = true;
        }

        public IEnumerator Load(int id, bool instant = true, bool bgm = false)
        {
            if (id == 0)
                yield break;

            if (_caches.ContainsKey(id))
                yield break;

            if (_table == null)
            {
                _table = TableManager.Instance.GetTableClass<BT_Sound>();
                if (_table == null)
                {
                    //yield return StartCoroutine(TableManager.Instance.Load());
                    _table = TableManager.Instance.GetTableClass<BT_Sound>();

                    if (_table == null)
                        yield break;
                }
            }

            //var data = _table.Rows.Find(x => x.SoundID == id);
            var data = _table.sheets[0].list.Find(x => x.Index == id);
            if (data == null)
            {
                Debug.LogErrorFormat("Could not found 'BT_SoundRow' : {0} of {1}", id, gameObject.name);
                yield break;
            }

            string path = Model.First<SettingModel>().GetLocalizingPath();

            string fullpath = string.Format("Sound/{0}{2}/{1}", path, data.FileName, data.FilePath);
            Debug.Log("Sound Path: " + fullpath);
            yield return StartCoroutine(ResourceLoader.Instance.Load<AudioClip>(fullpath,
                o => OnPostLoadProcess(o, fullpath, id, data, instant, bgm)));
        }

        void OnPostLoadProcess(Object o, string name, int id, BT_Sound.Param data, bool instant, bool bgm)
        {
            if (!_caches.ContainsKey(id))
            {
                var sound = bgm ? o as AudioClip : Instantiate(o) as AudioClip;
                _caches.Add(id, new ClipCache { resourceName = name, data = data, clip = sound, instant = instant, bgm = bgm });
            }
        }

        public int PlaySound(int id, bool fade = false)
        {
            Debug.LogFormat("PlaySound - {0}", id);

            ClipCache cache;
            if (_caches.TryGetValue(id, out cache))
            {
                var source = _audioSourcePool.GetObject() ?? gameObject.AddComponent<AudioSource>();

                _playingAudio.AddLast(new PlayingAudio { clipCache = cache, audioSource = source });

                source.clip = cache.clip;
                source.loop = cache.data.Loop;
                source.volume = fade ? 0.0f : cache.data.Volum;

                source.Play();
                //Debug.LogFormat("PlaySound - {0} - {1} - OK", id, source.clip.name);

                if (fade)
                    source.DOFade(cache.data.Volum, fadeDuration);
            }

            int count = 0;
            var node = _playingAudio.First;
            while (node != null)
            {
                var audio = node.Value;
                if (audio.clipCache.data.Index == id)
                    count++;

                node = node.Next;
            }

            return count;
        }

        public void PauseSound(int id)
        {
            var node = _playingAudio.First;
            while (node != null)
            {
                var audio = node.Value;

                if (audio.clipCache.data.Index == id)
                    audio.audioSource.Pause();

                node = node.Next;
            }
        }

        public void PausePlaySound(int id)
        {
            var node = _playingAudio.First;
            while (node != null)
            {
                var audio = node.Value;
                if (audio.clipCache.data.Index == id)
                    audio.audioSource.Play();

                node = node.Next;
            }
        }

        public void StopSound(int id, int indexCount = 1, bool fade = false)
        {
            //Debug.LogFormat("StopSound - {0}", id);
            int count = indexCount;
            var node = _playingAudio.First;
            while (node != null)
            {
                var audio = node.Value;
                if (audio.clipCache.data.Index == id)
                {
                    count--;
                    if (0 == count)
                    {
                        if (fade)
                        {
                            audio.audioSource.DOFade(0.0f, fadeDuration).OnComplete(
                                () =>
                                {
                                    audio.audioSource.Stop();
                                    audio.audioSource.clip = null;

                                    _audioSourcePool.PoolObject(audio.audioSource);
                                    _playingAudio.Remove(node);
                                });
                        }
                        else
                        {
                            audio.audioSource.Stop();
                            audio.audioSource.clip = null;

                            _audioSourcePool.PoolObject(audio.audioSource);
                            _playingAudio.Remove(node);
                        }

                        //Debug.LogFormat("StopSound - {0} - {1} - OK", id, audio.audioSource.clip.name);
                        break;
                    }
                }

                node = node.Next;
            }
        }

        public bool IsPlaySound(int id)
        {
            var node = _playingAudio.First;
            while (node != null)
            {
                var audio = node.Value;
                if (audio.clipCache.data.Index == id)
                {
                    return audio.audioSource.isPlaying;
                }
                node = node.Next;
            }
            return false;
        }

        public void StopAllSound()
        {
            //Debug.LogFormat("StopAllSound");

            var node = _playingAudio.First;
            while (node != null)
            {
                var audio = node.Value;
                audio.audioSource.Stop();
                audio.audioSource.clip = null;

                _audioSourcePool.PoolObject(audio.audioSource);

                node = node.Next;
            }

            _playingAudio.Clear();
        }

        void LateUpdate()
        {
            var node = _playingAudio.First;
            while (node != null)
            {
                var audio = node.Value;
                //if (audio.clipCache.instant && !audio.audioSource.isPlaying)
                if (!audio.audioSource.isPlaying)
                {
                    if (!audio.audioSource.loop)
                    {
                        //Debug.LogFormat("LateUpdate - {0} - End", audio.audioSource.clip.name);

                        audio.audioSource.Stop();
                        audio.audioSource.clip = null;

                        _audioSourcePool.PoolObject(audio.audioSource);
                        _playingAudio.Remove(node);
                    }
                }

                node = node.Next;
            }
        }

        protected override void Release()
        {
            StopAllSound();
            UnloadAllLoadCaches();
        }

        public void UnloadAllInstantCaches()
        {
            var unloadList = new List<int>();

            foreach (var cache in _caches)
            {
                if (cache.Value.instant)
                {
                    Debug.LogFormat("UnloadAllInstantCaches - {0} - {1} - OK", cache.Value.data.Index, cache.Value.clip.name);

                    Destroy(cache.Value.clip);
                    cache.Value.clip = null;

                    unloadList.Add(cache.Value.data.Index);
                }
                else
                    Debug.LogFormat("UnloadAllInstantCaches - {0} - {1} - NO", cache.Value.data.Index, cache.Value.clip.name);
            }

            for (int i = 0; i < unloadList.Count; ++i)
            {
                _caches.Remove(unloadList[i]);
            }
        }

        public void UnloadAllLoadCaches()
        {
            foreach (var cache in _caches)
            {
                ResourceLoader.Instance.Unload(cache.Value.resourceName);

                if (!cache.Value.bgm)
                    Destroy(cache.Value.clip);

                cache.Value.clip = null;
            }

            _caches.Clear();
        }
    }
}
