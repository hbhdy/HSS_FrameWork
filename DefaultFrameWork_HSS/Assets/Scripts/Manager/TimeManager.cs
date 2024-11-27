using UnityEngine;
using System;
using System.Collections;
using System.Globalization;
using UnityEngine.Networking;

namespace HSS
{
    public class TimeManager
    {
        // ----- Param -----

        private DateTime baseTime;
        public DateTime nowUTC => baseTime;


        // ----- Init -----

        public IEnumerator GetGoogleTime(Action action)
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

    }
}