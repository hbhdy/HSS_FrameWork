using UnityEngine;

namespace HSS
{
    public static  class UtilFunction
    {
        public static void SetRectTransform(GameObject newObj, GameObject originPrefab)
        {
            RectTransform newRect = newObj.GetComponent<RectTransform>();
            RectTransform oringRect = originPrefab.GetComponent<RectTransform>();

            if (newRect == null || oringRect == null)
                return;

            newRect.anchoredPosition = oringRect.anchoredPosition;
            newRect.anchoredPosition3D = oringRect.anchoredPosition3D;

            newRect.anchorMax = oringRect.anchorMax;
            newRect.anchorMin = oringRect.anchorMin;
            newRect.offsetMax = oringRect.offsetMax;
            newRect.offsetMin = oringRect.offsetMin;

            newRect.pivot = oringRect.pivot;

            newRect.sizeDelta = oringRect.sizeDelta;

            newRect.localScale = oringRect.localScale;
        }
    }
}