using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// EnemyHPFill handles the fill or remaining HP of an enemy

public class EnemyHPFill : MonoBehaviour
{
    SpriteRenderer sr;
    EnemyDefaults enemy;
    Transform health;
    float barRatio;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        enemy = GetComponentInParent<EnemyDefaults>();
        health = gameObject.transform;
        barRatio = transform.localScale.x / enemy.Enemy.maxHP;
    }

    void OnEnable()
    {
        // OnEnable, update health and listen for OnDamaged event
        UpdateHealthDamaged(); 
        enemy.OnDamaged.AddListener(UpdateHealthDamaged);
    }

    void OnDisable()
    {
        enemy.OnDamaged.RemoveListener(UpdateHealthDamaged);
    }

    // Flash fill bar on damaged
    public IEnumerator Flash()
    {
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, .5f);
        yield return new WaitForSeconds(.1f);
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, .8f);
        yield return new WaitForSeconds(.1f);
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, .5f);
        yield return new WaitForSeconds(.1f);
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1f);
    }

    // Update healths
    public void UpdateHealth()
    {
        float barSize = barRatio * enemy.HP > 0 ? barRatio * enemy.HP : 0;
        health.localScale = new Vector2(barSize, health.localScale.y);
    }

    // Updates health and flashes for damaged
    public void UpdateHealthDamaged()
    {
        UpdateHealth();
        StartCoroutine(Flash());
    }
}
