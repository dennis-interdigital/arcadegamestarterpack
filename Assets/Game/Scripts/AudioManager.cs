using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EditYourNameSpace
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioSource bgmSource;
        [SerializeField] AudioSO audioSO;

        Dictionary<BGM, AudioClip> bgmDict;
        Dictionary<SFX, AudioClip> sfxDict;

        GameManager gameManager;

        Coroutine bgmCoroutine;
        BGM currentBGM;

        public void Init(GameManager inGameManager)
        {
            gameManager = inGameManager;   

            bgmDict = new Dictionary<BGM, AudioClip>();
            sfxDict = new Dictionary<SFX, AudioClip>();

            BGMData[] bgmDatas = audioSO.bgmDatas;
            SFXData[] sfxDatas = audioSO.sfxDatas;

            int count = bgmDatas.Length;
            for (int i = 0; i < count; i++)
            {
                BGMData data = bgmDatas[i];
                bool hasKey = bgmDict.ContainsKey(data.name);
                if (!hasKey)
                {
                    bgmDict.Add(data.name, data.clip);
                }
            }

            count = sfxDatas.Length;
            for (int i = 0; i < count; i++)
            {
                SFXData data = sfxDatas[i];
                bool hasKey = sfxDict.ContainsKey(data.name);
                if (!hasKey)
                {
                    sfxDict.Add(data.name, data.clip);
                }
            }

            sfxSource.loop = false;
            sfxSource.playOnAwake = false;

            bgmSource.loop = true;
            bgmSource.playOnAwake = false;
            bgmSource.volume = 0.5f;
        }

        public void PlayBGM(BGM bgm)
        {
            if (currentBGM != bgm)
            {
                if (bgmCoroutine != null)
                {
                    StopCoroutine(bgmCoroutine);
                }

                AudioClip toPlay = bgmDict[bgm];
                bgmCoroutine = StartCoroutine(FadeBGM(toPlay));
            }
        }

        IEnumerator FadeBGM(AudioClip clip)
        {
            if (bgmSource.isPlaying)
            {
                while (bgmSource.volume > 0)
                {
                    bgmSource.volume -= 0.1f;
                    yield return gameManager.coroutine.WaitForSeconds(0.2f);
                }

                bgmSource.Stop();
            }

            bgmSource.clip = clip;
            bgmSource.Play();

            while (bgmSource.volume < 0.5f)
            {
                bgmSource.volume += 0.1f;
                yield return gameManager.coroutine.WaitForSeconds(0.2f);
            }

            bgmSource.volume = 0.5f;
        }

        public void PlaySFX(SFX sfx)
        {
            AudioClip toPlay = sfxDict[sfx];
            sfxSource.PlayOneShot(toPlay);
        }

        public void StopBGM()
        {
            bgmSource.Stop();
        }

        public void StopSFX()
        {
            sfxSource.Stop();
        }
    }
}
