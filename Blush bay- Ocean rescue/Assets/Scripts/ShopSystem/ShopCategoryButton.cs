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
        // If no button was assigned, try to get one on this object.
        if (button == null)
        {
            button = GetComponent<Button>();
        }

        // When this category button is clicked, show that category.
        if (button != null)
        {
            button.onClick.AddListener(ShowCategory);
        }
    }

    private void ShowCategory()
    {
        if (shopManager == null)
        {
            Debug.LogWarning("No ShopManager assigned on " + name);
            return;
        }

        // Tell the shop to only show items from this category.
        shopManager.ShowCategory(categoryToShow);
    }
}