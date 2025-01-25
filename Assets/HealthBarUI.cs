using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public Image HealthBar;

    public Image Health;

    public Attributes PlayerAttributes; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerAttributes != null)
        {
            Health.rectTransform.sizeDelta
                = new Vector2((PlayerAttributes.CurrentHP / PlayerAttributes.MaxHP) * HealthBar.rectTransform.sizeDelta.x, HealthBar.rectTransform.sizeDelta.y);
        }
        else
        {
            if (GameManager.instance?.Player != null)
            {
                PlayerAttributes = GameManager.instance.Player.GetComponent<Attributes>();
            }
        }
    }
}
