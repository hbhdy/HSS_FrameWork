using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using UnityEngine;

namespace HSS 
{
    public interface SeceondCheck
    {
        public void DelayUpdate_OneSeconds();
    }


    public class GameCore : SingletonMono<GameCore>
    {
        // ----- Param -----

        [SerializeField]
        private Transform trUIScreen;
        [SerializeField]
        private Transform trUIPopup;

        public UIManager UIManager { get; private set; } = new UIManager();

        public TimeManager TimeManager { get; private set; } = new TimeManager();

        public SoundManager SoundManager { get; private set; } = new SoundManager();

        public GameObject objEmptyPrefab;

        private float deltaTime = 0f;

        // ----- Init -----

        public IEnumerator Start()
        {
            // 기본 프로젝트 내부 설정 처리
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            Application.targetFrameRate = 60;
            Time.timeScale = 1f;

            // Temp
            ObjectPool.CreatePool(objEmptyPrefab, 1);

            yield return null;

            UIManager.Init();
            //SoundManager.Init();
            yield return TimeManager.Co_GetGoogleTime(null);

            UIManager.SetTrScreen(trUIScreen);
            UIManager.SetTrPopup(trUIPopup);

            //UIManager.OpenUI(UIType.UIPopup_Common, Canvas_SortOrder.POPUP);
        }

        // ----- Main -----

        public void Update()
        {
            // 1초마다 해당 매니저에 있는 타이머로직을 실행
            if (deltaTime >= 1f)
            {
                TimeManager.DelayUpdate_OneSeconds();
                UIManager.DelayUpdate_OneSeconds();

                deltaTime -= 1f;
            }

            deltaTime += Time.deltaTime;
        }
    }
}
