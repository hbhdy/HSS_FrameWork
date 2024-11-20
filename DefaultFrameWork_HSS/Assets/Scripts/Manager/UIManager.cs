using HSS;
using System;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine;

public enum UIType
{
    None,

    [UIAttrType("UIPopup_Common")]
    UIPopup_Common,
    [UIAttrType("UIPopup_Option")]
    UIPopup_Option,
}

public enum Canvas_SortOrder
{
    SCREEN = 100,
    POPUP = 3000,
}

namespace HSS
{
    public class UIManager
    {
        // ----- Param -----

        private Dictionary<UIType, UIBase> dicUI = new Dictionary<UIType, UIBase>();
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

        // ----- Main ----- 

        public void OpenUI(UIType uiType, Canvas_SortOrder sortOrder = Canvas_SortOrder.POPUP, Action openAfter = null, Action closeAtger = null)
        {
            Action loadComplete = () =>
            {
                UIBase openUI = dicUI[uiType];
                openUI.callOpenAfter = openAfter;
                openUI.callCloseAfter = closeAtger;
                _OpenUI(openUI);
            };

            if (dicUI.ContainsKey(uiType) == false)
            {
                LoadUI<UIBase>(uiType, sortOrder, loadComplete);
            }
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
    }
}