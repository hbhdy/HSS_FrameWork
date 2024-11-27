using System;
using System.Collections;
using System.Globalization;
using System.Net;
using UnityEngine;

namespace HSS 
{
    public class GameCore : SingletonMono<GameCore>
    {
        // ----- Param -----

        [SerializeField]
        private Transform trUIScreen;
        [SerializeField]
        private Transform trUIPopup;

        public UIManager UIManager { get; private set; } = new UIManager();

        public TimeManager TimeManager { get; private set; } = new TimeManager();

        public GameObject objEmptyPrefab;

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
            yield return TimeManager.GetGoogleTime(null);

            yield return null;

            UIManager.SetTrScreen(trUIScreen);
            UIManager.SetTrPopup(trUIPopup);

            //UIManager.OpenUI(UIType.UIPopup_Common, Canvas_SortOrder.POPUP);
        }

        // ----- Main -----

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                if (UIManager.GetUI<UIPopup_Common>(UIType.UIPopup_Common) != null)
                {
                    if (UIManager.GetUI<UIPopup_Common>(UIType.UIPopup_Common).gameObject.activeSelf)
                        UIManager.GetUI<UIPopup_Common>(UIType.UIPopup_Common).Close();
                    else
                        UIManager.OpenUI(UIType.UIPopup_Common, Canvas_SortOrder.POPUP);
                }
                else
                    UIManager.OpenUI(UIType.UIPopup_Common, Canvas_SortOrder.POPUP);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                if (UIManager.GetUI<UIPopup_Option>(UIType.UIPopup_Option) != null)
                {
                    if (UIManager.GetUI<UIPopup_Option>(UIType.UIPopup_Option).gameObject.activeSelf)
                        UIManager.GetUI<UIPopup_Option>(UIType.UIPopup_Option).Close();
                    else
                        UIManager.OpenUI(UIType.UIPopup_Option, Canvas_SortOrder.POPUP);
                }
                else
                    UIManager.OpenUI(UIType.UIPopup_Option, Canvas_SortOrder.POPUP);
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                if (UIManager.GetUI<UIPopup_Notice>(UIType.UIPopup_Notice) != null)
                {
                    if (UIManager.GetUI<UIPopup_Notice>(UIType.UIPopup_Notice).gameObject.activeSelf)
                        UIManager.GetUI<UIPopup_Notice>(UIType.UIPopup_Notice).Close();
                    else
                        UIManager.OpenUI(UIType.UIPopup_Notice, Canvas_SortOrder.POPUP);
                }
                else
                    UIManager.OpenUI(UIType.UIPopup_Notice, Canvas_SortOrder.POPUP);
            }
        }
    }
}
