using UnityEngine;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
/// <summary>
/// 인벤토리 테스트를 위한 유틸리티 클래스 (에디터/개발 빌드에서만 작동)
/// </summary>
public static class InventoryTestUtility
{
    /// <summary>
    /// 테스트용 아이템 추가
    /// </summary>
    public static void AddTestItems(UI_Inven inventory)
    {
        if (inventory == null)
        {
            Debug.LogWarning("인벤토리가 null입니다.");
            return;
        }

        // 무기 아이템 추가
        AddTestWeapons(inventory);
        
        // 소모품 아이템 추가 (스택 테스트)
        AddTestConsumables(inventory);
        
        // 음식 아이템 추가 (DataManager 스프라이트 사용)
        AddTestFoodItems(inventory);
        
        Debug.Log("[테스트] 인벤토리에 테스트 아이템을 추가했습니다.");
    }

    private static void AddTestWeapons(UI_Inven inventory)
    {
        for (int i = 0; i < 3; i++)
        {
            WeaponItemData weapon = new WeaponItemData(
                i + 1,
                $"테스트 무기 {i + 1}",
                $"테스트용 무기입니다. (ID: {i + 1})"
            );
            weapon.stats = new WeaponStats
            {
                attackPower = 10 + i * 5,
                attackSpeed = 1.0f + i * 0.1f,
                durability = 100 - i * 10
            };
            
            inventory.AddItem(weapon, 1);
        }
    }

    private static void AddTestConsumables(UI_Inven inventory)
    {
        // 물약 아이템 (스택 테스트용)
        ConsumableItemData potion = new ConsumableItemData(
            100,
            "체력 물약",
            "HP를 50 회복합니다."
        );
        potion.data = new ConsumableData
        {
            effect = "HP 50 회복",
            cooldown = 3.0f,
            maxStack = 99,
            usage = "우클릭으로 사용"
        };
        
        // 물약을 여러 번 추가 (스택 테스트)
        inventory.AddItem(potion, 5);
        inventory.AddItem(potion, 3);
    }

    /// <summary>
    /// 음식 아이템 추가 (DataManager의 스프라이트 사용)
    /// </summary>
    private static void AddTestFoodItems(UI_Inven inventory)
    {
        // DataManager에서 음식 스프라이트 가져오기
        for (int i = 0; i < 8; i++)
        {
            Sprite foodSprite = Managers.Data.GetFoodSpriteByIndex(i);
            
            if (foodSprite == null)
            {
                Debug.LogWarning($"[테스트] 음식{i} 스프라이트를 찾을 수 없습니다.");
                continue;
            }

            ConsumableItemData food = new ConsumableItemData(
                200 + i,
                $"음식 {i}",
                $"맛있는 음식입니다. (음식{i})"
            );
            
            food.icon = foodSprite; // DataManager에서 로드한 스프라이트 설정
            
            food.data = new ConsumableData
            {
                effect = $"HP {10 + i * 5} 회복",
                cooldown = 1.0f,
                maxStack = 50,
                usage = "클릭으로 섭취"
            };
            
            inventory.AddItem(food, 1);
        }
    }

    /// <summary>
    /// 장비 테스트 아이템 추가
    /// </summary>
    public static void AddTestEquipment(UI_Inven inventory)
    {
        EquipmentItemData armor = new EquipmentItemData(
            200,
            "테스트 갑옷",
            "기본 갑옷입니다."
        );
        armor.stats = new EquipmentStats
        {
            defense = 20,
            magicResist = 10,
            durability = 100
        };
        
        inventory.AddItem(armor, 1);
    }

    /// <summary>
    /// 수집품 테스트 아이템 추가
    /// </summary>
    public static void AddTestCollectibles(UI_Inven inventory)
    {
        CollectibleItemData collectible = new CollectibleItemData(
            300,
            "신비한 보석",
            "반짝이는 보석입니다."
        );
        collectible.data = new CollectibleData
        {
            obtainLocation = "신비의 동굴",
            usage = "제작 재료",
            sellPrice = 500,
            rarity = "희귀"
        };
        
        inventory.AddItem(collectible, 1);
    }

    /// <summary>
    /// 인벤토리를 가득 채우기 (확장 테스트용)
    /// </summary>
    public static void FillInventory(UI_Inven inventory)
    {
        int maxSlots = inventory.GetMaxSlots();
        
        for (int i = 0; i < maxSlots; i++)
        {
            WeaponItemData item = new WeaponItemData(
                1000 + i,
                $"더미 아이템 {i}",
                "인벤토리 테스트용"
            );
            item.stats = new WeaponStats
            {
                attackPower = 1,
                attackSpeed = 1.0f,
                durability = 10
            };
            
            if (!inventory.AddItem(item, 1))
            {
                Debug.Log($"[테스트] 인벤토리가 {i}번 슬롯에서 가득 찼습니다.");
                break;
            }
        }
    }

    /// <summary>
    /// 특정 음식 스프라이트로 아이템 생성 (개별 테스트용)
    /// </summary>
    public static ConsumableItemData CreateFoodItem(int foodIndex)
    {
        Sprite foodSprite = Managers.Data.GetFoodSpriteByIndex(foodIndex);
        
        if (foodSprite == null)
        {
            Debug.LogWarning($"[테스트] 음식{foodIndex} 스프라이트를 찾을 수 없습니다.");
            return null;
        }

        ConsumableItemData food = new ConsumableItemData(
            200 + foodIndex,
            $"음식 {foodIndex}",
            $"맛있는 음식입니다."
        );
        
        food.icon = foodSprite;
        food.data = new ConsumableData
        {
            effect = $"HP {10 + foodIndex * 5} 회복",
            cooldown = 1.0f,
            maxStack = 50,
            usage = "클릭으로 섭취"
        };
        
        return food;
    }
}
#endif
