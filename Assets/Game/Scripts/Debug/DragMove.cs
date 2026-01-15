using UnityEngine;
using UnityEngine.EventSystems;

namespace EditYournameSpace
{
    public class DragMove : MonoBehaviour, IDragHandler
    {
        RectTransform rt;

        void Awake()
        {
            rt = GetComponent<RectTransform>();
        }

        public void OnDrag(PointerEventData eventData)
        {
            RectTransform parentRect = rt.parent as RectTransform;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentRect,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 localPoint
            );

            rt.anchoredPosition = localPoint;
        }
    }
}

