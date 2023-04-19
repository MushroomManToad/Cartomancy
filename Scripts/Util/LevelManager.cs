using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    List<Enemy> enemies = new List<Enemy>();

    List<CardEffect> activeEffects = new List<CardEffect>();

    [SerializeField]
    List<ClearEffect> clearEffects = new List<ClearEffect>();

    private bool currSceneCleared = false;

    public void removeEnemy(Enemy e)
    {
        enemies.Remove(e);
    }

    private void FixedUpdate()
    {
        if(enemies.Count <= 0 && !currSceneCleared)
        {
            currSceneCleared = true;
            onClear();
        }
    }

    public void addEffect(CardEffect effect)
    {
        /*
         * First, determine if the effect already exists. If so, check if the new effect is of higher magnitude than the active one. If it is, overwrite it and sync to all enemies.
         * 
         * If the effect doesn't exist yet, simply add it and sync to all enemies.
         */
        CardEffect activeEffect = containsEffect(effect);
        if (activeEffect != null)
        {
            if(effect.getPower() > activeEffect.getPower())
            {
                activeEffects.Remove(activeEffect);
                syncRemoveFromEnemies(activeEffect);
                activeEffects.Add(effect);
                syncToEnemies(effect);
            }
            else { /* Do nothing. a stronger effect is already active */ }
        }
        else
        {
            activeEffects.Add(effect);
            syncToEnemies(effect);
        }
    }

    // Returns the card effect (if it exists) of the same CardType in activeEffects. Returns null if not found.
    private CardEffect containsEffect(CardEffect effect)
    {
        return null;
    }

    private void syncToEnemies(CardEffect effect)
    {
        foreach (Enemy e in enemies)
        {
            if (e != null) e.addEffect(effect);
        }
    }

    private void syncRemoveFromEnemies(CardEffect effect)
    {
        foreach(Enemy e in enemies)
        {
            if(e != null) e.removeEffect(effect);
        }
    }

    private void onClear()
    {
        foreach (ClearEffect ce in clearEffects)
        {
            ce.onClear();
        }
        activeEffects.Clear();
    }

    public bool getSceneCleared()
    {
        return currSceneCleared;
    }
}