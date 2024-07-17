using UnityEngine;

public class CustomContentSizeFitter : MonoBehaviour
{
    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        UpdateSize();
    }

    void Update()
    {
        UpdateSize();
    }

    void UpdateSize()
    {
        float width = 0;
        float height = 0;

        foreach (RectTransform child in transform)
        {
            if (child.gameObject.activeSelf)
            {
                width = Mathf.Max(width, child.anchoredPosition.x + child.sizeDelta.x);
                height = Mathf.Max(height, child.anchoredPosition.y + child.sizeDelta.y);
            }
        }

        rectTransform.sizeDelta = new Vector2(width, height);
    }
}
