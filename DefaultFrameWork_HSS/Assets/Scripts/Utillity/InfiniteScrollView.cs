using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace HSS
{
    public class InfiniteScrollView : MonoBehaviour
    {
        // ----- Param -----
        public bool initByUser = false;
        public ScrollRect.MovementType moveType = ScrollRect.MovementType.Unrestricted;
        public bool isLimitHeight = false;
        public float moveOffset = 50f;
        
        // 각 아이템간의 거리
        private float recordOffsetX = 0;
        private float recordOffsetY = 0;

        // 아이템을 이동시키는 한계 값
        private float disableMarginX = 0;
        private float disableMarginY = 0;

        private List<RectTransform> items = new List<RectTransform>();
        private int itemCount = 0;

        private bool isDisableGrid = false;
        private bool isVertical = false;
        private bool isHorizontal = false;
        private Vector2 oringStartPos = Vector2.zero;
        private Vector2 tempAnchoredPos = Vector2.zero;
        private float limitHeight = 0;

        private ScrollRect scrollRect;
        private ContentSizeFitter contentSizeFitter;
        private VerticalLayoutGroup verticalGroup;
        private HorizontalLayoutGroup horizontalGroup;
        private GridLayoutGroup gridGroup;

        // ----- Init -----

        private void Awake()
        {
            if (!initByUser)
                Init();
        }

        public void Init()
        {
            if (GetComponent<ScrollRect>() != null)
            {
                scrollRect = GetComponent<ScrollRect>();
                scrollRect.onValueChanged.AddListener(OnScroll);
                scrollRect.movementType = moveType;

                scrollRect.content.TryGetComponent<ContentSizeFitter>(out contentSizeFitter);
                scrollRect.content.TryGetComponent<GridLayoutGroup>(out gridGroup);
                scrollRect.content.TryGetComponent<VerticalLayoutGroup>(out verticalGroup);
                scrollRect.content.TryGetComponent<HorizontalLayoutGroup>(out horizontalGroup);

                isHorizontal = scrollRect.horizontal;
                isVertical = scrollRect.vertical;

                if (isHorizontal && isVertical)
                    HSSLog.LogError("(horizontal or vertical) One Pick Please");

                SetItems();
            }
            else
                HSSLog.LogError("Not Found ScrollRect Component");
        }

        // ----- Set ----- 

        private void SetItems()
        {
            items.Clear();

            foreach (RectTransform childRect in scrollRect.content)
                items.Add(childRect);

            itemCount = items.Count;
        }

        /// <summary>
        /// Contents의 설정한 높이를 기준으로 스크롤 이동을 제한함
        /// </summary>
        /// <param name="height"></param>
        public void SetLimitHeight(float height)
        {
            // 사용 하기전에 Elastic을 설정하고 Child Force Expand Height를 해제해야함
            limitHeight = height;
            scrollRect.content.sizeDelta = new Vector2(scrollRect.content.rect.width, height);
        }

        // 스크롤 하지 않고 확인이 필요할때 사용
        public void FirstCheck()
        {
            if (!isDisableGrid)
                DisableGridComponents();
        }

        // ----- Get ----- 

        // ----- Main ----- 

        void DisableGridComponents()
        {
            oringStartPos = items[0].anchoredPosition;

            if (isVertical)
            {
                recordOffsetY = Mathf.Abs(items[1].anchoredPosition.y - items[0].anchoredPosition.y);
                disableMarginY = recordOffsetY * (itemCount / 2);
            }

            if (isHorizontal)
            {
                recordOffsetX = Mathf.Abs(items[1].anchoredPosition.x - items[0].anchoredPosition.x);
                disableMarginX = recordOffsetX * (itemCount / 2);
            }

            if (gridGroup)
                gridGroup.enabled = false;

            if (contentSizeFitter)
                contentSizeFitter.enabled = false;

            if (verticalGroup)
                verticalGroup.enabled = false;

            if (horizontalGroup)
                horizontalGroup.enabled = false;
  
            isDisableGrid = true;
        }

        public void OnScroll(Vector2 pos)
        {
            if (!isDisableGrid)
                DisableGridComponents();

            for (int i = 0; i < items.Count; i++)
            {
                Vector3 itemLocalPos = scrollRect.transform.InverseTransformPoint(items[i].transform.position);

                if (isHorizontal)
                {
                    if (itemLocalPos.x > disableMarginX + moveOffset)
                    {
                        tempAnchoredPos = items[i].anchoredPosition;
                        tempAnchoredPos.x -= itemCount * recordOffsetX;
                        items[i].anchoredPosition = tempAnchoredPos;
                        scrollRect.content.GetChild(itemCount - 1).transform.SetAsFirstSibling();
                    }
                    else if (itemLocalPos.x < -disableMarginX)
                    {
                        tempAnchoredPos = items[i].anchoredPosition;
                        tempAnchoredPos.x += itemCount * recordOffsetX;
                        items[i].anchoredPosition = tempAnchoredPos;
                        scrollRect.content.GetChild(0).transform.SetAsLastSibling();
                    }
                }

                if (isVertical)
                {
                    if (itemLocalPos.y > disableMarginY + moveOffset)
                    {
                        tempAnchoredPos = items[i].anchoredPosition;
                        tempAnchoredPos.y -= itemCount * recordOffsetY;

                        if (isLimitHeight == true && limitHeight < Mathf.Abs(tempAnchoredPos.y))
                            return;

                        items[i].anchoredPosition = tempAnchoredPos;
                        scrollRect.content.GetChild(itemCount - 1).transform.SetAsFirstSibling();
                    }
                    else if (itemLocalPos.y < -disableMarginY)
                    {
                        tempAnchoredPos = items[i].anchoredPosition;
                        tempAnchoredPos.y += itemCount * recordOffsetY;

                        if (isLimitHeight == true && oringStartPos.y < tempAnchoredPos.y)
                                return;

                        items[i].anchoredPosition = tempAnchoredPos;
                        scrollRect.content.GetChild(0).transform.SetAsLastSibling();
                    }
                }
            }
        }
    }
}