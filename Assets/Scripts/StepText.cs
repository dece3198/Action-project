using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StepText : MonoBehaviour
{
    public float alphaSpeed;
    private TextMeshProUGUI text;
    private Color alpha;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        alpha = text.color;
    }

    private void OnEnable()
    {
        alpha.a = 255f;
    }

    private void Update()
    {
        alpha.a = Mathf.Lerp(alpha.a, 0, Time.deltaTime * alphaSpeed);
        text.color = alpha;

        if(text.color.a <= 0.01)
        {
            gameObject.SetActive(false);
        }
    }
}
