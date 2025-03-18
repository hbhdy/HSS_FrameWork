using UnityEngine;
using System;
using System.Collections;
using System.Globalization;
using UnityEngine.Networking;
using R3;
using Cysharp.Threading.Tasks;

namespace HSS
{
    public class TimeManager : BaseManager, SeceondCheck
    {
        // ----- Param -----

        private DateTime baseTime;
        public DateTime nowUTC => baseTime;

        public ReactiveProperty<int> testProperty = new ReactiveProperty<int>(0);

        // ----- Init -----

        public override IEnumerator Co_Init()
        {
            yield return StartCoroutine(Co_GetGoogleTime(null));

            yield return base.Co_Init();
        }

        public override async void Init()
        {
            // 비동기 하지않기
            Task_GetGoogleTime(null).Forget();
            // 비동기 진행
            await Task_GetGoogleTime(null);

            base.Init();
        }

        public async UniTask Task_GetGoogleTime(Action action)
        {
            UnityWebRequest request = UnityWebRequest.Get("https://google.com");

            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string netTime = request.GetResponseHeader("date");
                baseTime = DateTime.ParseExact(netTime,
                                       "ddd, dd MMM yyyy HH:mm:ss 'GMT'",
                                       CultureInfo.InvariantCulture.DateTimeFormat,
                                       DateTimeStyles.AssumeUniversal).ToLocalTime();
            }
            else
                HSSLog.LogWarning($"Request Error : {request.error}");

            action?.Invoke();
        }


        public IEnumerator Co_GetGoogleTime(Action action)
        {
            UnityWebRequest request = UnityWebRequest.Get("https://google.com");

            yield return request.SendWebRequest();

            if(request.result == UnityWebRequest.Result.Success)
            {
                string netTime = request.GetResponseHeader("date");
                baseTime = DateTime.ParseExact(netTime,
                                       "ddd, dd MMM yyyy HH:mm:ss 'GMT'",
                                       CultureInfo.InvariantCulture.DateTimeFormat,
                                       DateTimeStyles.AssumeUniversal).ToLocalTime();
            }
            else
                HSSLog.LogWarning($"Request Error : {request.error}");

            action?.Invoke();
        }

        // ----- Set ----- 


        // ----- Get ----- 

        public DateTime GetCurTime()
        {
            return nowUTC.AddSeconds(Time.realtimeSinceStartup);
        }

        // ----- Main ----- 

        // 
        public void DelayUpdate_OneSeconds()
        {
            //testProperty.Value -= 1;

            //if (testProperty.Value <= 0)
            //    testProperty.Value = 0;
        }

        // 아래의 방식을 사용했었는데 이곳 저곳에서 시간 코루틴을 사용하면 한번에 관리가 어려웠음
        // 2월 이후 추가 생각 : 꼭 한번에 관리해야하는 것인가??? 여러 고민이 드는 것 같다.

        //public static Coroutine RemainVisibleUI(MonoBehaviour mono, Text textObject, DateTime endTime, string format = "", bool isHourDisplay = true, bool isRealTime = true, Action timeOverAction = null)
        //{
        //    if (mono == null && textObject == null)
        //    {
        //        Debug.Log("MonoBehaviour or Text is Null");
        //        return null;
        //    }

        //    return mono.StartCoroutine(CalcVisibleUI(textObject, endTime, format, isHourDisplay, isRealTime, timeOverAction));
        //}

        //private static IEnumerator CalcVisibleUI(Text textObject, DateTime endTime, string format, bool isHourDisplay = true, bool isRealTime = true, Action timeOverAction = null)
        //{
        //    WaitForSecondsRealtime realWait = new WaitForSecondsRealtime(1f);
        //    WaitForSeconds wait = new WaitForSeconds(1f);

        //    TimeSpan remainTime;
        //    DateTime localSavedUtcNow = realUtcNow;  // 실행 시점의 Time 을 저장해서 사용한다.

        //    while (textObject.gameObject.activeInHierarchy == true)
        //    {
        //        remainTime = isRealTime ? endTime - realUTC
        //                                : endTime - localSavedUtcNow;

        //        if (remainTime.TotalSeconds < 0)
        //        {
        //            timeOverAction?.Invoke();
        //            break;
        //        }

        //        textObject.text = "";
        //        // 시간 갱신
           
        //        if (isRealTime)
        //            yield return realWait;
        //        else
        //        {
        //            yield return wait;
        //            localSavedUtcNow = localSavedUtcNow.AddSeconds(1.0f);
        //        }
        //    }
        //}
    }
}