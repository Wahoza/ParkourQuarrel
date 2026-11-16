using System.Collections.Generic;
using UnityEngine;

public class ItemSelectionPointer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    List<Transform> SelectableIcons;
    void Start()
    {
        
    }

    // Update is called once per frame
    public void SetSelectableParent(RectTransform holder)
    {
        Debug.Log("Changed Transform to " + holder.gameObject);

        SelectableIcons = new List<Transform>();
        if(holder == null) { return; }

        for (int i = 0; i < holder.childCount; i++)
        {
            if (holder.GetChild(i).transform.childCount != 0)
            {
                SelectableIcons.Add(holder.GetChild(i).GetChild(0));
            }
        }

        transform.SetParent(SelectableIcons[0]);
    }
    public void SwitchSelection(int index)
    {
        if(SelectableIcons == null)
        {
            return;
        }

        if (index >= 0 && index < SelectableIcons.Count)
        {
            transform.SetParent(SelectableIcons[index]);
        }
    }
}
