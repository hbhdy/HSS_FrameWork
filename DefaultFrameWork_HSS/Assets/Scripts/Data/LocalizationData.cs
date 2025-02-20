using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem;

namespace HSS
{
    public class LocalizationData
    {
        public string Key;
        public string Korean;       // ko
        public string English;      // en

        public static List<LocalizationData> Create(List<Dictionary<string, object>> csv)
        {
            List<LocalizationData> list = new List<LocalizationData>();

            // 1번 방법
            int count = csv.Count;
            int index = 0;

            for (int i = 0; i < count; i++)
            {
                index = 0;
                List<object> values = csv[i].Values.ToList();
                list.Add(new LocalizationData
                {
                    Key = values[index++].ToString(),
                    Korean = values[index++].ToString(),
                    English = values[index++].ToString(),
                });
            }


            // 2번 방법
            // - Dictionary Values 대신 직접 Key 접근 : 키 순서가 보장되지 않으므로 csv[i].Values 대신 csv[i]["Key"] 접근
            // - 불필요한 List<object> 변환 제거 : List<object> 를 생성하는 연산을 제거하여 성능 최적화
            // - 예외 방지(TryGetValue) : 특정 키가 없을 경우를 대비해 TryGetValue() 사용
            // - 가독성 향상 : index++없이 row["Key"].ToString() 방식으로 명확하게 접근
            //foreach(var row in csv)
            //{
            //    if (row.TryGetValue("Key", out object key) &&
            //    row.TryGetValue("Korean", out object korean) &&
            //    row.TryGetValue("English", out object english))
            //    {
            //        list.Add(new LocalizationData
            //        {
            //            Key = key.ToString(),
            //            Korean = korean.ToString(),
            //            English = english.ToString()
            //        });
            //    }
            //}

            return list;
        }
    }
}
