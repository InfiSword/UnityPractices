using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Weapon,     // 무기
    Consumable, // 소모형 아이템
    Equipment,  // 장비
    Collectible // 수집형 아이템
}

// 기본 아이템 데이터 클래스
[System.Serializable]
public class ItemData
{
    public int id;
    public string name;
    public string description;
    public Sprite icon;
    public ItemType itemType;
    
    public ItemData(int id, string name, string description, ItemType type)
    {
        this.id = id;
        this.name = name;
        this.description = description;
        this.itemType = type;
    }
}

// 무기 아이템 데이터
[System.Serializable]
public class WeaponItemData : ItemData
{
    public WeaponStats stats;
    
    public WeaponItemData(int id, string name, string description) 
        : base(id, name, description, ItemType.Weapon)
    {
    }
}

// 소모형 아이템 데이터
[System.Serializable]
public class ConsumableItemData : ItemData
{
    public ConsumableData data;
    
    public ConsumableItemData(int id, string name, string description) 
        : base(id, name, description, ItemType.Consumable)
    {
    }
}

// 장비 아이템 데이터
[System.Serializable]
public class EquipmentItemData : ItemData
{
    public EquipmentStats stats;
    
    public EquipmentItemData(int id, string name, string description) 
        : base(id, name, description, ItemType.Equipment)
    {
    }
}

// 수집형 아이템 데이터
[System.Serializable]
public class CollectibleItemData : ItemData
{
    public CollectibleData data;
    
    public CollectibleItemData(int id, string name, string description) 
        : base(id, name, description, ItemType.Collectible)
    {
    }
}

[System.Serializable]
public struct WeaponStats
{
    public int attackPower;      // 공격력
    public float attackSpeed;    // 공격 속도
    public int durability;       // 내구도
}

[System.Serializable]
public struct ConsumableData
{
    public string effect;        // 사용 효과 설명 (예: "HP 50 회복", "이동속도 30% 증가")
    public float cooldown;       // 쿨타임 (초)
    public int maxStack;         // 최대 스택 수
    public string usage;         // 사용 방법 (예: "우클릭으로 사용", "자동 사용")
}

[System.Serializable]
public struct EquipmentStats
{
    public int defense;         // 방어력
    public int magicResist;     // 마법 저항력
    public int durability;      // 내구도
}

[System.Serializable]
public struct CollectibleData
{
    public string obtainLocation;  // 획득 위치 (예: "숲", "동굴", "고블린 드롭")
    public string usage;           // 용도 (예: "제작 재료", "퀘스트 아이템", "판매용")
    public int sellPrice;          // 판매 가격
    public string rarity;           // 희귀도 (예: "일반", "희귀", "전설")
}
