using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 아이템 UI (인벤토리, 대장간 등 다양한 UI에서 사용 가능)
/// </summary>
public class UI_Item : UI_Base
{
    enum GameObjects
    {
        Image_Item,
        Text_Item,
    }

    private ItemData _itemData;
    private int _quantity;
    private int _slotIndex;
    private UI_Inven _inven;
    private Image _itemImage;

    private CanvasGroup _canvasGroup;
    private Transform _originalParent;
    private RectTransform _rectTransform;
    private Canvas _rootCanvas;
    private Vector2 _dragOffset;

    // 드래그 모드 설정
    [SerializeField] private bool _dragFromCenter = false; // true: 중앙 기준, false: 클릭 위치 기준

    public ItemData ItemData => _itemData;
    public int Quantity => _quantity;
    public int SlotIndex => _slotIndex;

    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));

        _itemImage = Get<GameObject>((int)GameObjects.Image_Item).GetComponent<Image>();
        _canvasGroup = Util.GetOrAddComponent<CanvasGroup>(gameObject);
        _rectTransform = GetComponent<RectTransform>();

        // 드래그 모드 설정 적용
        _dragFromCenter = InventoryConfig.DRAG_FROM_CENTER;

        // 드래그 이벤트 바인딩
        BindEvent(gameObject, OnBeginDrag, Define.UIEvent.BeginDrag);
        BindEvent(gameObject, OnDrag, Define.UIEvent.Drag);
        BindEvent(gameObject, OnEndDrag, Define.UIEvent.EndDrag);
        BindEvent(gameObject, OnClick, Define.UIEvent.Click);
    }

    public void SetInfo(ItemData itemData, int quantity, int slotIndex, UI_Inven inven)
    {
        _itemData = itemData;
        _quantity = quantity;
        _slotIndex = slotIndex;
        _inven = inven;
        
        UpdateUI();

        // 위치 초기화
        transform.localPosition = Vector3.zero;
    }
    
    public void SetSlotIndex(int index)
    {
        _slotIndex = index;
    }

    /// <summary>
    /// UI 갱신 (아이콘, 텍스트, 수량)
    /// </summary>
    private void UpdateUI()
    {
        UpdateItemText();
        UpdateItemImage();
    }

    /// <summary>
    /// 아이템 텍스트 갱신 (이름 + 수량)
    /// </summary>
    private void UpdateItemText()
    {
        GameObject textObj = Get<GameObject>((int)GameObjects.Text_Item);
        if (textObj == null)
            return;

        Text text = textObj.GetComponent<Text>();
        if (text == null)
            return;

        if (_itemData != null)
        {
            // 수량이 1보다 크면 표시
            string quantityText = _quantity > 1 ? $" x{_quantity}" : "";
            text.text = $"{_itemData.name}{quantityText}";
        }
        else
        {
            text.text = "";
        }
    }

    /// <summary>
    /// 아이템 이미지 갱신
    /// </summary>
    private void UpdateItemImage()
    {
        if (_itemImage == null)
            return;

        if (_itemData != null && _itemData.icon != null)
        {
            _itemImage.sprite = _itemData.icon;
            _itemImage.enabled = true;
        }
        else
        {
            _itemImage.sprite = null;
            _itemImage.enabled = false;
        }
    }

    /// <summary>
    /// 드래그 시작
    /// </summary>
    private void OnBeginDrag(PointerEventData eventData)
    {
        if (_itemData == null || _inven == null)
            return;

        _originalParent = transform.parent;
        
        // 루트 Canvas 찾기
        _rootCanvas = _inven.GetComponentInParent<Canvas>();
        if (_rootCanvas == null)
            _rootCanvas = FindRootCanvas();
        
        SetDraggingState(true);
        _inven.SetDraggedItem(this);
        
        // 드래그 중인 아이템을 최상위로 이동
        transform.SetParent(_rootCanvas.transform);
        
        if (_dragFromCenter)
        {
            // 방법 1: 아이템 중앙을 마우스 커서에 맞춤 (오프셋 없음)
            _dragOffset = Vector2.zero;
        }
        else
        {
            // 방법 2: 클릭한 위치를 기준으로 오프셋 유지
            Vector2 localPointerPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _rootCanvas.transform as RectTransform,
                eventData.position,
                _rootCanvas.worldCamera,
                out localPointerPosition
            );
            
            _dragOffset = _rectTransform.anchoredPosition - localPointerPosition;
        }
    }

    /// <summary>
    /// 드래그 중
    /// </summary>
    private void OnDrag(PointerEventData eventData)
    {
        if (_itemData == null || _rootCanvas == null)
            return;

        // 마우스 위치를 Canvas 로컬 좌표로 변환
        Vector2 localPointerPosition;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _rootCanvas.transform as RectTransform,
            eventData.position,
            _rootCanvas.worldCamera,
            out localPointerPosition))
        {
            // 오프셋을 적용하여 위치 설정
            _rectTransform.anchoredPosition = localPointerPosition + _dragOffset;
        }
    }

    /// <summary>
    /// 드래그 종료
    /// </summary>
    private void OnEndDrag(PointerEventData eventData)
    {
        if (_itemData == null)
            return;

        SetDraggingState(false);

        // 드롭 대상이 있는지 확인
        if (TryDropToTarget(eventData))
            return;

        // 드롭 실패 시 원래 위치로 복귀
        ReturnToOriginalPosition();
    }

    /// <summary>
    /// 드래그 상태 설정 (투명도, Raycast 차단)
    /// </summary>
    private void SetDraggingState(bool isDragging)
    {
        if (_canvasGroup == null)
            return;

        _canvasGroup.alpha = isDragging ? InventoryConfig.DRAG_ALPHA : InventoryConfig.NORMAL_ALPHA;
        _canvasGroup.blocksRaycasts = !isDragging;
    }

    /// <summary>
    /// 드롭 대상 처리 시도
    /// </summary>
    private bool TryDropToTarget(PointerEventData eventData)
    {
        if (eventData?.pointerEnter == null)
            return false;

        IItemDropTarget dropTarget = eventData.pointerEnter.GetComponentInParent<IItemDropTarget>();
        if (dropTarget != null)
        {
            // 드롭 가능 여부 확인
            if (!dropTarget.CanAcceptDrop(this))
            {
                Debug.Log($"[UI_Item] '{_itemData.name}'을(를) 드롭할 수 없습니다.");
                return false;
            }
            
            // 드롭 처리
            _inven?.ClearDraggedItem();
            bool dropSuccess = dropTarget.OnItemDropped(this);
            
            if (!dropSuccess)
            {
                Debug.LogWarning($"[UI_Item] '{_itemData.name}' 드롭 처리 실패");
            }
            
            return dropSuccess;
        }

        return false;
    }

    /// <summary>
    /// 원래 위치로 복귀
    /// </summary>
    private void ReturnToOriginalPosition()
    {
        if (_inven == null)
            return;

        // 드래그 상태가 아직 유지되고 있고, 최상위에 있으면 원래 위치로 복귀
        if (_inven.GetDraggedItem() == this && 
            (transform.parent == _rootCanvas?.transform || transform.parent != _originalParent))
        {
            transform.SetParent(_originalParent);
            transform.localPosition = Vector3.zero;
            _inven.ClearDraggedItem();
        }
    }

    /// <summary>
    /// 루트 Canvas 찾기
    /// </summary>
    private Canvas FindRootCanvas()
    {
        Canvas[] canvases = GetComponentsInParent<Canvas>();
        if (canvases != null && canvases.Length > 0)
        {
            return canvases[canvases.Length - 1];
        }
        return null;
    }

    /// <summary>
    /// 클릭 이벤트 (아이템 정보 표시)
    /// </summary>
    private void OnClick(PointerEventData eventData)
    {
        if (_itemData == null || _inven == null)
            return;

        _inven.ShowItemInfo(_itemData);
    }
    
    /// <summary>
    /// 드래그 모드 설정 (중앙 기준 / 클릭 위치 기준)
    /// </summary>
    public void SetDragFromCenter(bool fromCenter)
    {
        _dragFromCenter = fromCenter;
    }
}
