using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

    [SerializeField] Slider slider;
    public Color fillColor;

    private void Awake()
    {
        ChangeColor();
    }

    public void SetMaxHealth(float health) {
        slider.maxValue = health;
        slider.value = health; 
    }

    public void SetHealth(float health) {  slider.value = health; }
    public void ChangeColor()
    {
        Transform fill = transform.GetChild(0);
        Transform icon = transform.GetChild(2);
        if (fill.TryGetComponent<SpriteRenderer>(out SpriteRenderer fillSprite))
            fillSprite.color = fillColor;
        if (icon.TryGetComponent<SpriteRenderer>(out SpriteRenderer iconSprite))
            iconSprite.color = fillColor;
    }
}
