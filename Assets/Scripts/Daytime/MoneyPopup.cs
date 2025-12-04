using UnityEngine;
using TMPro;
using DG.Tweening;

public class MoneyPopup : MonoBehaviour
{
    public TextMeshProUGUI tmpText;
    public float moveDistance = 100f;
    public float duration = 1.0f;

    public void Setup(int amount)
    {
        // 1. Set Text & Color
        if (amount > 0)
        {
            tmpText.text = "+" + amount.ToString();
            tmpText.color = Color.green;
        }
        else
        {
            tmpText.text = amount.ToString(); // "-50" already has the minus
            tmpText.color = Color.red;
        }

        // 2. Animate Movement (Float Up)
        transform.DOLocalMoveY(transform.localPosition.y + moveDistance, duration);

        // 3. Animate Fade (Disappear)
        tmpText.DOFade(0f, duration).SetEase(Ease.InExpo).OnComplete(() => 
        {
            Destroy(gameObject); // Clean up after animation
        });
    }
}