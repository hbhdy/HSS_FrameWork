using UnityEngine;

namespace HSS
{
    public static  class UtilFunction
    {
        public static void SetRectTransform(GameObject newObj, GameObject originPrefab)
        {
            RectTransform newRect = newObj.GetComponent<RectTransform>();
            RectTransform originRect = originPrefab.GetComponent<RectTransform>();

            if (newRect == null || originRect == null)
                return;

            newRect.anchoredPosition = originRect.anchoredPosition;
            newRect.anchoredPosition3D = originRect.anchoredPosition3D;

            newRect.anchorMax = originRect.anchorMax;
            newRect.anchorMin = originRect.anchorMin;
            newRect.offsetMax = originRect.offsetMax;
            newRect.offsetMin = originRect.offsetMin;

            newRect.pivot = originRect.pivot;

            newRect.sizeDelta = originRect.sizeDelta;

            newRect.localScale = originRect.localScale;
        }
    }
}