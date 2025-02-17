using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using UnityEngine;
using UnityEngine.Video;

namespace HSS 
{
    public interface SeceondCheck
    {
        public void DelayUpdate_OneSeconds();
    }


    public class GameCore : SingletonMono<GameCore>
    {
        // ----- Param -----

        [Header("Base Managers")]
        public List<BaseManager> managerList;

        private Dictionary<Type, BaseManager> dicManagers = new Dictionary<Type, BaseManager>();
        private float deltaTime = 0f;

        public static UIManager UI { get { return Instance.Get<UIManager>(); } }
        public static TimeManager TIME { get { return Instance.Get<TimeManager>(); } }
        public static SoundManager SOUND { get { return Instance.Get<SoundManager>(); } }

        [HideInInspector]
        public static bool isCoreReady = false;

        // ----- Init -----

        public T Get<T>() where T : BaseManager
        {
            var type = typeof(T);
            return dicManagers.ContainsKey(type) ? dicManagers[type] as T : null;
        }

        public IEnumerator Start()
        {
            // 기본 프로젝트 내부 설정 처리
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            Application.targetFrameRate = 60;
            Time.timeScale = 1f;

            yield return null;

            dicManagers.Clear();
            for (int i = 0; i < managerList.Count; i++)
            {
                yield return StartCoroutine(managerList[i].Co_Init());
                
                var type = managerList[i].GetType();
                dicManagers.Add(type, managerList[i]);
            }

            yield return new WaitUntil(() => isCoreReady);

            for (int i = 0; i < managerList.Count; i++)
                managerList[i].Init();


            //UI.OpenUI(UIType.UIPopup_Common, Canvas_SortOrder.POPUP);
        }

        // ----- Main -----

        public void Update()
        {
            // 1초마다 해당 매니저에 있는 타이머로직을 실행
            if (deltaTime >= 1f)
            {
                //Time.DelayUpdate_OneSeconds();
                //UI.DelayUpdate_OneSeconds();

                deltaTime -= 1f;
            }

            deltaTime += Time.deltaTime;
        }
    }
}
