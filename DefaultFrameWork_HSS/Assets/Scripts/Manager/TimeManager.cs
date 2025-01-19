using UnityEngine;
using System;
using System.Collections;
using System.Globalization;
using UnityEngine.Networking;

namespace HSS
{
    public class TimeManager : SeceondCheck
    {
        // ----- Param -----

        private DateTime baseTime;
        public DateTime nowUTC => baseTime;

        // ----- Init -----

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
                Debug.LogWarning($"Request Error : {request.error}");

            action?.Invoke();
        }

        // ----- Set ----- 


        // ----- Get ----- 

        public DateTime GetCurTime()
        {
            return nowUTC.AddSeconds(Time.realtimeSinceStartup);
        }

        // ----- Main ----- 

        public void DelayUpdate_OneSeconds()
        {

        }

        // �Ʒ��� ����� ����߾��µ� �̰� �������� �ð� �ڷ�ƾ�� ����ϸ� �ѹ��� ������ �������

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
        //    DateTime localSavedUtcNow = realUtcNow;  // ���� ������ Time �� �����ؼ� ����Ѵ�.

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
        //        // �ð� ����
           
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