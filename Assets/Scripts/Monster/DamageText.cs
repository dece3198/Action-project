using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    public float moveSpeed;
    public float alphaSpeed;
    public CanvasGroup canvasGroup;
    public Monster monster;

    private void Update()
    {
        transform.Translate(new Vector3(0, moveSpeed * Time.deltaTime, 0));
        canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 0, Time.deltaTime * alphaSpeed);

        if(monster != null)
        {
            if(canvasGroup.alpha <= 0.1)
            {
                canvasGroup.alpha = 1;
                monster.EnterPool(gameObject);
            }
        }
    }
}
