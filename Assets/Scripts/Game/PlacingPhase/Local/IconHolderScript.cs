using UnityEngine;

public class IconHolderScript : MonoBehaviour
{
    [SerializeField] Sprite _icon;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Sprite GetIcon()
    {
        return _icon;
    }
    public void SetIcon(Sprite icon)
    {
        _icon = icon;
    }
}
