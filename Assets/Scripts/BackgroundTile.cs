using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Backgroundtile : MonoBehaviour
{

    public int hitPoints;
    private SpriteRenderer spriteRenderer;
    public Sprite breakSprite;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        if (hitPoints <= 0)
        {
            Destroy(this.gameObject);
        }
    }
    public void TakeDamage(int damage)
    {
        hitPoints -= damage;
        if (hitPoints == 1)
        {
            MakeBreakSprite();
        } else {
            MakeBreakColor();
        }
    }
    void MakeBreakColor(){
        Color color = spriteRenderer.color;
        float newAlpha = color.a * .2f;
        spriteRenderer.color = new Color(color.r, color.g, color.b,newAlpha);
    }
    void MakeBreakSprite()
    {
        spriteRenderer.sprite = breakSprite;
    }
}
