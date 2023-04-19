using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class CardData
{
    private string cardName;
    private EffectType mainEffect;
    private string spriteLocation;
    private Sprite cardSprite;
    private List<CardEffect> cardEffects = new List<CardEffect>();

    public CardData(MainEffectData data)
    {
        mainEffect = data.getEffectType();
        this.spriteLocation = "Assets/Sprites/CardSprites/" + data.getSpriteName();
        handle = Addressables.LoadAssetAsync<Sprite>(getSpriteLocation());
        handle.Completed += Handle_Completed;
    }

    // Magic that makes the Sprite work :)
    // Retain handle to release asset and operation
    private AsyncOperationHandle<Sprite> handle;

    // Release asset when parent object is destroyed
    public void OnDestroy()
    {
        Addressables.Release(handle);
    }
    // End magic that makes things load.

    // Instantiate the loaded prefab on complete
    private void Handle_Completed(AsyncOperationHandle<Sprite> operation)
    {
        if (operation.Status == AsyncOperationStatus.Succeeded)
        {
            if(operation.Result is Sprite)
            {
                cardSprite = (Sprite)operation.Result;
            }
        }
        else
        {
            Debug.LogError($"Asset for {getSpriteLocation()} failed to load.");
        }
    }

    public void addCardEffect(CardEffect effect)
    {
        cardEffects.Add(effect);
    }

    public List<CardEffect> getCardEffects()
    {
        return cardEffects;
    }

    public void setName(string newName)
    {
        this.cardName = newName;
    }

    public string getName()
    {
        return cardName;
    }

    public string getSpriteLocation()
    {
        return spriteLocation;
    }

    public EffectType getMainEffect()
    {
        return mainEffect;
    }

    public Sprite getCardSprite()
    {
        return cardSprite;
    }
}
