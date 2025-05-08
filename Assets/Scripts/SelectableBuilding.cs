using UnityEngine;

public class SelectableBuilding : MonoBehaviour
{
    private SpriteRenderer sr;
    private Color defaultColor;
    public bool isSelected = false;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        defaultColor = sr.color;
    }

    void OnMouseDown()
    {
        GameManager.Instance.SelectBuildingObject(this);
    }

    public void Highlight(bool state)
    {
        isSelected = state;
        sr.color = state ? Color.cyan : defaultColor;  // Or outline shader, etc.
    }
}
