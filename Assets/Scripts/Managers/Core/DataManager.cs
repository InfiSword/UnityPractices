using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILoader<Key, Value>
{
    Dictionary<Key, Value> MakeDict();
}

public class DataManager
{
    //public Dictionary<int, Stat> StatDict { get; private set; } = new Dictionary<int, Stat>();

    // 스프라이트 딕셔너리 (아이템 아이콘 등)
    public Dictionary<string, Sprite> FoodSprites { get; private set; } = new Dictionary<string, Sprite>();
    
    // 추후 확장 가능한 다른 스프라이트 딕셔너리
    // public Dictionary<string, Sprite> WeaponSprites { get; private set; } = new Dictionary<string, Sprite>();
    // public Dictionary<string, Sprite> EquipmentSprites { get; private set; } = new Dictionary<string, Sprite>();

    public void Init()
    {
        //StatDict = LoadJson<StatData, int, Stat>("StatData").MakeDict();
        
        // 스프라이트 로드
        LoadFoodSprites();
    }

    /// <summary>
    /// 음식 스프라이트 로드 (음식_0 ~ 음식_7)
    /// </summary>
    private void LoadFoodSprites()
    {
        // Resources/Sprites/음식 폴더에서 모든 스프라이트 로드
        Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites/음식");
        
        if (sprites == null || sprites.Length == 0)
        {
            Debug.LogWarning("[DataManager] 음식 스프라이트를 찾을 수 없습니다. 경로: Resources/Sprites/음식");
            return;
        }

        FoodSprites.Clear();
        
        foreach (Sprite sprite in sprites)
        {
            if (sprite != null)
            {
                FoodSprites[sprite.name] = sprite;
                Debug.Log($"[DataManager] 음식 스프라이트 로드: {sprite.name}");
            }
        }
        
        Debug.Log($"[DataManager] 총 {FoodSprites.Count}개의 음식 스프라이트 로드 완료");
    }

    /// <summary>
    /// 스프라이트 이름으로 가져오기
    /// </summary>
    public Sprite GetFoodSprite(string spriteName)
    {
        if (FoodSprites.TryGetValue(spriteName, out Sprite sprite))
        {
            return sprite;
        }
        
        Debug.LogWarning($"[DataManager] 스프라이트를 찾을 수 없습니다: {spriteName}");
        return null;
    }

    /// <summary>
    /// 인덱스로 음식 스프라이트 가져오기 (음식_0 ~ 음식_7)
    /// </summary>
    public Sprite GetFoodSpriteByIndex(int index)
    {
        string spriteName = $"음식_{index}";  // 언더스코어 추가
        return GetFoodSprite(spriteName);
    }

    /// <summary>
    /// 모든 음식 스프라이트 이름 목록 가져오기
    /// </summary>
    public List<string> GetAllFoodSpriteNames()
    {
        return new List<string>(FoodSprites.Keys);
    }

    Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
    {
		TextAsset textAsset = Managers.Resource.Load<TextAsset>($"Data/{path}");
        return JsonUtility.FromJson<Loader>(textAsset.text);
	}
}
