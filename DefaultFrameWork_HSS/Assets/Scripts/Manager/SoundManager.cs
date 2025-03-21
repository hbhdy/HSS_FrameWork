using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HSS
{
    public class SoundManager : BaseManager
    {
        // ----- Param -----

        public float SfxVolume
        {
            get { return sfxVolume; }
            set { sfxVolume = value; }
        }

        public float BgmVolume
        {
            get { return bgmVolume; }
            set { bgmVolume = value; }
        }

        public bool Mute
        {
            get { return MuteSFX && MuteBGM; }
            set { MuteSFX = MuteBGM = value; }
        }

        public bool MuteSFX
        {
            get { return muteSfx; }
            set { muteSfx = value; }
        }

        public bool MuteBGM
        {
            get { return muteBgm; }
            set { muteBgm = value; }
        }

        [SerializeField]
        private float sameSoundIgnoreTime = 0.1f;    // 마지막 재생한 사운드와 같은게 특정시간 내에오면 무시
        [SerializeField]
        private int maxAudioSources = 10;        // AudioSource 최대 개수

        [Header("Audio Sources")]
        public AudioSource bgmSource;

        [Header("Audio Clips")]
        public List<AudioClip> bgmClips;
        public List<AudioClip> sfxClips;

        private Dictionary<string, AudioClip> dicSoundData;
        private List<AudioSource> sfxSources;

        private float lastPlaySoundTime;
        private string lastPlaySoundName;
        private bool isinitializing;

        private float sfxVolume;
        private float bgmVolume;
        private bool muteSfx;
        private bool muteBgm;

        private readonly string SOUND_SFX_VOLUME_KEY = "sound_sfx_volume";
        private readonly string SOUND_SFX_MUTE_KEY = "sound_sfx_mute";
        private readonly string SOUND_BGM_VOLUME_KEY = "sound_bgm_volume";
        private readonly string SOUND_BGM_MUTE_KEY = "sound_bgm_mute";

        // ----- Init -----

        public override IEnumerator Co_Init()
        {
            yield return base.Co_Init();
        }

        public override async UniTask Task_Init()
        {
            await base.Task_Init();
        }

        public override void Init()
        {
            dicSoundData = new Dictionary<string, AudioClip>();
            foreach (var clip in bgmClips)
                dicSoundData[clip.name] = clip;

            foreach (var clip in sfxClips)
                dicSoundData[clip.name] = clip;

            if (sfxSources.Count < maxAudioSources)
            {
                sfxSources = new List<AudioSource>();
                for (int i = 0; i < maxAudioSources; i++)
                {
                    var source = gameObject.AddComponent<AudioSource>();
                    sfxSources.Add(source);
                }
            }

            MuteSFX = PlayerPrefs.GetInt(SOUND_SFX_MUTE_KEY, 0) == 1;
            MuteBGM = PlayerPrefs.GetInt(SOUND_BGM_MUTE_KEY, 0) == 1;
            SfxVolume = PlayerPrefs.GetFloat(SOUND_SFX_VOLUME_KEY, 1);
            BgmVolume = PlayerPrefs.GetFloat(SOUND_BGM_VOLUME_KEY, 1);

            base.Init();
        }

        // ----- Set ----- 

        public void SetAllVolume(float bgmVolume, float sfxVolume)
        {
            BgmVolume = bgmVolume;
            SfxVolume = sfxVolume;

            bgmSource.volume = BgmVolume;
            foreach (var source in sfxSources)
                source.volume = SfxVolume;
        }

        // ----- Get ----- 

        private AudioSource GetAvailableAudioSource()
        {
            foreach (var source in sfxSources)
            {
                if (!source.isPlaying)
                    return source;
            }

            HSSLog.Log("Nothing Available Source");
            return null;
        }

        // ----- Main ----- 

        public void PlayBGM(string clipName)
        {
            if (dicSoundData.TryGetValue(clipName, out AudioClip clip))
            {
                bgmSource.clip = clip;
                bgmSource.volume = muteBgm ? 0 : bgmVolume;
                bgmSource.loop = true;
                bgmSource.Play();
            }
            else
                HSSLog.Log($"BGM is Null : {clipName}");
        }

        public void PlaySFX(string clipName)
        {
            if (dicSoundData.TryGetValue(clipName, out AudioClip clip))
            {
                AudioSource source = GetAvailableAudioSource();
                if (source != null)
                {
                    if (clip.name == lastPlaySoundName)
                    {
                        if (Time.realtimeSinceStartup - lastPlaySoundTime < sameSoundIgnoreTime)
                            return;
                    }

                    lastPlaySoundName = clip.name;
                    lastPlaySoundTime = Time.realtimeSinceStartup;

                    source.volume = muteSfx ? 0 : sfxVolume;
                    source.PlayOneShot(clip);
                }
            }
            else
                HSSLog.Log($"SFX is Null : {clipName}");
        }   

        public void StopBGM()
        {
            bgmSource.Stop();
        }

        public void StopSFX()
        {
            foreach (var source in sfxSources)
                source.Stop();
        }
    }
}