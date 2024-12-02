using HSS;
using System;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine;
using Unity.VisualScripting;

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
    [EnumName("È­¸é")] SCREEN = 100,
    [EnumName("ÆË¾÷")] POPUP = 3000,
}

namespace HSS
{
    public class UIManager
    {
        // ----- Param -----

        private Dictionary<UIType, UIBase> dicUI = new Dictionary<UIType, UIBase>();
        private List<UIBase> openUIList = new List<UIBase>();

        public Transform trScreen { get; private set; }
        public Transform trPopup { get; private set; }

        // ----- Init -----

        public void Init()
        {
        }

        // ----- Set ----- 

        public void SetTrScreen(Transform tr)
        {
            trScreen = tr;
        }
        public void SetTrPopup(Transform tr)
        {
            trPopup = tr;
        }

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
            GameObject objParent = sortOrder == Canvas_SortOrder.POPUP ? trPopup.gameObject : trScreen.gameObject;
            LoadUI<T>(UIAttrUtil.GetUIAttributeResourceName(uiType), objParent, loadAfter);
        }

        private void LoadUI<T>(string uiName, GameObject parent, Action loadAfter = null) where T : MonoBehaviour
        {
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