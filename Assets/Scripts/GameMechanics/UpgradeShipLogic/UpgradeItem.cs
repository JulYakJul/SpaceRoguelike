using UnityEngine;

public class UpgradeItem : MonoBehaviour
{
    [Header("Upgrade Settings")]
    [SerializeField] private UpgradeType type1;
    [SerializeField] private int value1;
    [SerializeField] private UpgradeType type2;
    [SerializeField] private int value2;

    private UpgradePanelController upgradePanelController;

    private void Start()
    {
        value1 = GenerateRandomValue(type1);
        value2 = GenerateRandomValue(type2);

        if (GameManager.Instance.upgradePanelController == null)
        {
            upgradePanelController = FindObjectOfType<UpgradePanelController>();
            
            if (upgradePanelController != null)
            {
                GameManager.Instance.upgradePanelController = upgradePanelController.gameObject;
            }
            else
            {
                Debug.LogError("UpgradePanelController GameObject not found in the scene.");
            }
        }
        else
        {
            upgradePanelController = GameManager.Instance.upgradePanelController.GetComponent<UpgradePanelController>();
            
            if (upgradePanelController == null)
            {
                Debug.LogError("UpgradePanelController component not found on the object.");
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (upgradePanelController != null)
            {
                upgradePanelController.gameObject.SetActive(true);
                upgradePanelController.ShowUpgradePanel(type1, value1, type2, value2);
                Destroy(gameObject);
            }
            else
            {
                Debug.LogError("UpgradePanelController is not set.");
            }
        }
    }

    private int GenerateRandomValue(UpgradeType type)
    {
        return type switch
        {
            UpgradeType.Health => Random.Range(5, 21),
            _ => Random.Range(1, 3),
        };
    }
}
