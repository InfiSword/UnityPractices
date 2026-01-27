using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Inven_Item : UI_Base
{
    enum GameObjects
    {
        Image_Item,
        Text_Item,
    }

    private ItemData _itemData;
    private int _slotIndex;
    private UI_Inven _inven;
    private Image _itemImage;
    private CanvasGroup _canvasGroup;
    private Transform _originalParent;

    public ItemData ItemData => _itemData;
    public int SlotIndex => _slotIndex;

    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));

        _itemImage = Get<GameObject>((int)GameObjects.Image_Item).GetComponent<Image>();
        _canvasGroup = Util.GetOrAddComponent<CanvasGroup>(gameObject);

        // 드래그 이벤트 바인딩
        BindEvent(gameObject, OnBeginDrag, Define.UIEvent.BeginDrag);
        BindEvent(gameObject, OnDrag, Define.UIEvent.Drag);
        BindEvent(gameObject, OnEndDrag, Define.UIEvent.EndDrag);
        BindEvent(gameObject, OnClick, Define.UIEvent.Click);
    }

    public void SetInfo(ItemData itemData, int slotIndex, UI_Inven inven)
    {
        _itemData = itemData;
        _slotIndex = slotIndex;
        _inven = inven;
        
        ApplyDataToUI();
    }
    
    private void ApplyDataToUI()
    {
        GameObject textObj = Get<GameObject>((int)GameObjects.Text_Item);
        if (textObj != null)
        {
            Text text = textObj.GetComponent<Text>();
            if (text != null)
            {
                if (_itemData != null)
                {
                    text.text = _itemData.name;
                }
                else
                {
                    text.text = "";
                }
            }
        }

        if (_itemImage != null)
        {
            if (_itemData != null && _itemData.icon != null)
            {
                _itemImage.sprite = _itemData.icon;
            }
            else
            {
                _itemImage.sprite = null;
            }
        }

        // 아이템이 없으면 클릭/드래그 자체가 안 되도록 처리
        //if (_canvasGroup != null)
        //{
        //    if (_itemData == null)
        //    {
        //        _canvasGroup.blocksRaycasts = false;
        //        _canvasGroup.interactable = false;
        //    }
        //    else
        //    {
        //        _canvasGroup.blocksRaycasts = true;
        //        _canvasGroup.interactable = true;
        //    }
        //}
    }

    public void SetSlotIndex(int index)
    {
        _slotIndex = index;
    }

    private void OnBeginDrag(PointerEventData eventData)
    {
        if (_itemData == null) return;

        _originalParent = transform.parent;
        _canvasGroup.alpha = 0.5f;
        _canvasGroup.blocksRaycasts = false;

        // 인벤토리에 드래그 시작 알림
        if (_inven != null)
        {
            _inven.SetDraggedItem(this);
        }

        // 드래그 중인 아이템을 최상위로 이동
        transform.SetParent(_inven.transform.root);
    }

    private void OnDrag(PointerEventData eventData)
    {
        if (_itemData == null) return;

        transform.position = eventData.position;
    }

    private void OnEndDrag(PointerEventData eventData)
    {
        if (_itemData == null) return;

        _canvasGroup.alpha = 1.0f;
        _canvasGroup.blocksRaycasts = true;

        // 원래 위치로 복귀 (드롭이 성공하지 않았을 경우)
        if (transform.parent == _inven.transform.root || transform.parent != _originalParent)
        {
            // 드래그가 완료되지 않았고 아직 최상위에 있으면 원래 위치로 복귀
            if (_inven != null && _inven.GetDraggedItem() == this)
            {
                transform.SetParent(_originalParent);
                transform.localPosition = Vector3.zero;
                _inven.ClearDraggedItem();
            }
        }
    }

    private void OnClick(PointerEventData eventData)
    {
        if (_itemData == null) return;

        _inven?.ShowItemInfo(_itemData);
    }
}
