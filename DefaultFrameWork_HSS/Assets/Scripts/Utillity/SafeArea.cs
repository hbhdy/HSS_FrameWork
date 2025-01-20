using HSS;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class SafeArea : MonoBehaviour
{
    private const float MaxBottomMargin = 30f;
    private static Rect s_safeRect;

    public bool fitToTop = true;
    public bool fitToBottom = true;

    private static bool Initialized { get; set; }

    private Vector2 curScreenSize => new Vector2(Screen.width, Screen.height);
    private Vector2 saveScreenSize;

    public void Start()
    {
        if (!Initialized)
        {
            saveScreenSize = curScreenSize;

            float y = Mathf.Min(MaxBottomMargin, Screen.safeArea.y);
            var min = new Vector2(Screen.safeArea.x, y);
            var max = Screen.safeArea.position + Screen.safeArea.size;

#if UNITY_EDITOR
            HSSLog.Log($"<color=white>[ SafeArea ] min = {min}, max = {max}</color>");
#endif

            min.x /= Screen.width;
            max.x /= Screen.width;
            min.y /= Screen.height;
            max.y /= Screen.height;

            s_safeRect = new Rect(min, max);
            Initialized = true;
        }

        var rt = transform as RectTransform;

        rt.anchorMin = fitToBottom ? s_safeRect.position : Vector2.zero;
        rt.anchorMax = fitToTop ? s_safeRect.size : Vector2.one;
        rt.offsetMin = rt.offsetMax = Vector2.zero;
    }
}