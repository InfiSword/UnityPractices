using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ItemInfo : UI_Popup
{
    enum GameObjects
    {
        Image_Item,
        Text_Name,
        Text_Description,
        Text_Stats,
        Button_Close,
        DragArea,  // 드래그 가능한 영역 (상단 바)
    }

    private ItemData _itemData;
    private Image _itemImage;    

    public override void Init()
    {
        base.Init();
        Bind<GameObject>(typeof(GameObjects));
        
        // 아이템 이미지 컴포넌트 가져오기
        GameObject imageObj = Get<GameObject>((int)GameObjects.Image_Item);
        if (imageObj != null)
        {
            _itemImage = imageObj.GetComponent<Image>();
        }
        
        // 닫기 버튼 이벤트
        Button closeBtn = Get<GameObject>((int)GameObjects.Button_Close).GetComponent<Button>();
        closeBtn.onClick.AddListener(() => ClosePopupUI());
        
        // 창 드래그 영역 설정
        GameObject dragArea = Get<GameObject>((int)GameObjects.DragArea);
        if (dragArea != null)
        {
            SetDraggableArea(dragArea);
        }
        else
        {
            // DragArea가 없으면 전체 창을 드래그 가능하게 설정
            SetDraggableArea(gameObject);
        }
    }

    public void SetInfo(ItemData itemData)
    {
        _itemData = itemData;
        if (_itemData == null)
        {
            Debug.LogWarning("ItemData가 null입니다.");
            return;
        }

        // 아이템 이미지 설정
        if (_itemImage != null)
        {
            if (_itemData.icon != null)
            {
                _itemImage.sprite = _itemData.icon;
                _itemImage.enabled = true;
            }
            else
            {
                _itemImage.enabled = false;
            }
        }

        // 이름 설정
        Text nameText = Get<GameObject>((int)GameObjects.Text_Name).GetComponent<Text>();
        nameText.text = _itemData.name;

        // 설명 설정
        Text descText = Get<GameObject>((int)GameObjects.Text_Description).GetComponent<Text>();
        descText.text = _itemData.description;

        // 타입별 정보 설정
        Text statsText = Get<GameObject>((int)GameObjects.Text_Stats).GetComponent<Text>();
        statsText.text = GetItemInfoText();
    }

    private string GetItemInfoText()
    {
        if (_itemData == null) return "";
        
        string info = "";
        
        switch (_itemData.itemType)
        {
            case ItemType.Weapon:
                // 무기: 스탯 표시
                if (_itemData is WeaponItemData weaponItem)
                {
                    info = $"타입: 무기\n";
                    info += $"공격력: {weaponItem.stats.attackPower}\n";
                    info += $"공격 속도: {weaponItem.stats.attackSpeed}\n";
                    info += $"내구도: {weaponItem.stats.durability}";
                }
                else
                {
                    info = "무기 데이터를 불러올 수 없습니다.";
                }
                break;
                
            case ItemType.Consumable:
                // 소모형 아이템: 사용 효과, 쿨타임, 스택 등 표시
                if (_itemData is ConsumableItemData consumableItem)
                {
                    info = $"타입: 소모형 아이템\n";
                    if (!string.IsNullOrEmpty(consumableItem.data.effect))
                    {
                        info += $"효과: {consumableItem.data.effect}\n";
                    }
                    if (consumableItem.data.cooldown > 0)
                    {
                        info += $"쿨타임: {consumableItem.data.cooldown}초\n";
                    }
                    if (consumableItem.data.maxStack > 0)
                    {
                        info += $"최대 스택: {consumableItem.data.maxStack}개\n";
                    }
                    if (!string.IsNullOrEmpty(consumableItem.data.usage))
                    {
                        info += $"사용법: {consumableItem.data.usage}";
                    }
                }
                else
                {
                    info = "소모형 아이템 데이터를 불러올 수 없습니다.";
                }
                break;
                
            case ItemType.Equipment:
                // 장비: 스탯 표시
                if (_itemData is EquipmentItemData equipmentItem)
                {
                    info = $"타입: 장비\n";
                    info += $"방어력: {equipmentItem.stats.defense}\n";
                    info += $"마법 저항력: {equipmentItem.stats.magicResist}\n";
                    info += $"내구도: {equipmentItem.stats.durability}";
                }
                else
                {
                    info = "장비 데이터를 불러올 수 없습니다.";
                }
                break;
                
            case ItemType.Collectible:
                // 수집형 아이템: 획득 위치, 용도, 판매 가격 등 표시
                if (_itemData is CollectibleItemData collectibleItem)
                {
                    info = $"타입: 수집형 아이템\n";
                    if (!string.IsNullOrEmpty(collectibleItem.data.obtainLocation))
                    {
                        info += $"획득 위치: {collectibleItem.data.obtainLocation}\n";
                    }
                    if (!string.IsNullOrEmpty(collectibleItem.data.usage))
                    {
                        info += $"용도: {collectibleItem.data.usage}\n";
                    }
                    if (collectibleItem.data.sellPrice > 0)
                    {
                        info += $"판매 가격: {collectibleItem.data.sellPrice}골드\n";
                    }
                    if (!string.IsNullOrEmpty(collectibleItem.data.rarity))
                    {
                        info += $"희귀도: {collectibleItem.data.rarity}";
                    }
                }
                else
                {
                    info = "수집형 아이템 데이터를 불러올 수 없습니다.";
                }
                break;
        }
        
        return info;
    }
}
