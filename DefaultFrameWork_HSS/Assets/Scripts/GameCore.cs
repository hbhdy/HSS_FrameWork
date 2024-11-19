using System.Collections;
using System.Globalization;
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

            yield return null;

            UIManager.SetTrScreen(trUIScreen);
            UIManager.SetTrPopup(trUIPopup);



            UIManager.OpenUI(UIType.UIPopup_Common, Canvas_SortOrder.POPUP);
        }
    }
}
