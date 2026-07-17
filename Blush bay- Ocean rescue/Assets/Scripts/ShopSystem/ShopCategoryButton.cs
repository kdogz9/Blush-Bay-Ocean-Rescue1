using UnityEngine;
using UnityEngine.UI;

public class ShopCategoryButton : MonoBehaviour
{
    [Header("Category This Button Shows")]
    [SerializeField] private ShopCategory categoryToShow;

    [Header("Shop Manager")]
    [SerializeField] private ShopManager shopManager;

    [Header("Button")]
    [SerializeField] private Button button;

    private void Start()
    {
        if (button == null)
        {
            button = GetComponent<Button>();
        }

        if (button != null)
        {
            button.onClick.AddListener(ShowCategory);
        }
    }

    private void ShowCategory()
    {
        if (shopManager == null)
        {
            Debug.LogWarning(name + " has no ShopManager assigned.");
            return;
        }

        shopManager.ShowCategory(categoryToShow);
    }
}