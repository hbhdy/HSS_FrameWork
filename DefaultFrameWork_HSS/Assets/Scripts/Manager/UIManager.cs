using System;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine;
using System.Collections;
using Cysharp.Threading.Tasks;

namespace HSS
{
    public enum UIType
    {
        None,

        [UIAttrType("UIPopup_Common")]
        UIPopup_Common,
        [UIAttrType("UIPopup_Option")]
        UIPopup_Option,
        [UIAttrType("UIPopup_Notice")]
        UIPopup_Notice,
    }

    public enum Canvas_SortOrder
    {
        [EnumName("화면")] SCREEN = 100,
        [EnumName("팝업")] POPUP = 3000,
    }

    public class UIManager : BaseManager
    {
        // ----- Param -----
        [Header("Base Transform UI")]
        public Transform trUIScreen;
        public Transform trUIPopup;

        private Dictionary<UIType, UIBase> dicUI = new Dictionary<UIType, UIBase>();
        private List<UIBase> openUIList = new List<UIBase>();

        // ----- Init -----

        public override IEnumerator Co_Init()
        {
            yield return base.Co_Init();
        }

        public override async UniTask Task_Init()
        {
            await base.Task_Init();
        }

        public override void Init()
        {
            base.Init();
        }

        // ----- Set ----- 

        // ----- Get ----- 

        public UIBase GetUI(UIType type)
        {
            if (dicUI.ContainsKey(type))
                return dicUI[type];

            return null;
        }

        public T GetUI<T>(UIType type)
        {
            if (dicUI.ContainsKey(type))
                return dicUI[type].GetComponent<T>();

            return default(T);
        }

        // ----- Main ----- 

        public void DelayUpdate_OneSeconds()
        {

        }

        public void OpenUI(UIType uiType, Canvas_SortOrder sortOrder = Canvas_SortOrder.POPUP, Action openAfter = null, Action closeAfter = null)
        {
            Action loadComplete = () =>
            {
                UIBase openUI = dicUI[uiType];
                openUI.callOpenAfter = () =>
                {
                    openAfter?.Invoke();
                };
                openUI.callCloseAfter = () =>
                {
                    openUIList.Remove(openUI);
                    closeAfter?.Invoke();

                    RefreshPopupSortingOrder();
                };

                if (!openUIList.Contains(openUI))
                    openUIList.Add(openUI);

                _OpenUI(openUI);
                RefreshPopupSortingOrder();
            };

            if (dicUI.ContainsKey(uiType) == false)
                LoadUI<UIBase>(uiType, sortOrder, loadComplete);
            else
            {
                if (dicUI[uiType] != null)
                    loadComplete?.Invoke();
            }
        }

        private void LoadUI<T>(UIType uiType, Canvas_SortOrder sortOrder = Canvas_SortOrder.POPUP, Action loadAfter = null) where T : MonoBehaviour
        {
            GameObject objParent = sortOrder == Canvas_SortOrder.POPUP ? trUIPopup.gameObject : trUIScreen.gameObject;
            LoadUI<T>(UIAttrUtil.GetUIAttributeResourceName(uiType), objParent, loadAfter);
        }

        private void LoadUI<T>(string uiName, GameObject parent, Action loadAfter = null) where T : MonoBehaviour
        {
            // 어드레서블 사용 시
            Addressables.InstantiateAsync(uiName).Completed += obj =>
            {
                var initUI = obj.Result;
                initUI.transform.SetParent(parent.transform, false);

                T findComponent = initUI.GetComponent<T>();
                UIBase uiBase = findComponent as UIBase;

                if (uiBase != null)
                    dicUI.Add(uiBase.UIType, uiBase);    

                loadAfter?.Invoke(); 
            };

            //// Resources 폴더 사용 시
            //GameObject uiObj = null;
            //UtilFunction.CreateInstantiateUIObject(out uiObj, Resources.Load<GameObject>(uiName), parent);

            //T findComponent = uiObj.GetComponent<T>();
            //UIBase uiBase = findComponent as UIBase;

            //if (uiBase != null)
            //    dicUI.Add(uiBase.UIType, uiBase);

            //loadAfter?.Invoke();
        }

        private void _OpenUI(UIBase openUI)
        {
            Type type = typeof(UIBase);
            System.Reflection.MethodInfo method = type.GetMethod("OpenUI", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            method.Invoke(openUI, null);
        }

        private void RefreshPopupSortingOrder()
        {
            int index = (int)Canvas_SortOrder.POPUP;

            foreach (var uiBase in openUIList)
            {
                if (uiBase == null)
                    continue;

                var baseCanvas = uiBase.GetCanvas();
                if (baseCanvas != null)
                    baseCanvas.sortingOrder = index;

                index += 10;
            }
        }
    }
}