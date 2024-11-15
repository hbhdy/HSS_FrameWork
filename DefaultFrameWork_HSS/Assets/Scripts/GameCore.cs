using System.Collections;
using System.Globalization;
using UnityEngine;

namespace HSS 
{
    public class GameCore : SingletonMono<GameCore>
    {
        public UIManager UIManager { get; private set; } = new UIManager();

        public GameObject objEmptyPrefab;

        public IEnumerator Start()
        {
            yield return null;

            Init();
        }

        public void Init()
        {
            // 기본 프로젝트 내부 설정 처리
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            Application.targetFrameRate = 60;
            Time.timeScale = 1f;

            //Screen.sleepTimeout = SleepTimeout.NeverSleep;  // 절전모드 해제

            ObjectPool.CreatePool(objEmptyPrefab, 1);
        }
    }
}
