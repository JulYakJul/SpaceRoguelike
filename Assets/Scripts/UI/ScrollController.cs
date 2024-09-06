using UnityEngine;
using UnityEngine.UI;

public class ScrollController : MonoBehaviour
{
    public ScrollRect scrollRect;
    public Button forwardButton;
    public Button backwardButton;

    public float scrollSpeed = 0.1f;

   void Start()
    {
        if (forwardButton != null)
        {
            forwardButton.onClick.AddListener(ScrollForward);
        }
        else
        {
            Debug.LogError("Forward button is not assigned in the inspector.");
        }

        if (backwardButton != null)
        {
            backwardButton.onClick.AddListener(ScrollBackward);
        }
        else
        {
            Debug.LogError("Backward button is not assigned in the inspector.");
        }

        if (scrollRect == null)
        {
            Debug.LogError("ScrollRect is not assigned in the inspector.");
        }
    }

    void ScrollForward()
    {
        scrollRect.horizontalNormalizedPosition += scrollSpeed;
    }

    void ScrollBackward()
    {
        scrollRect.horizontalNormalizedPosition -= scrollSpeed;
    }
}
