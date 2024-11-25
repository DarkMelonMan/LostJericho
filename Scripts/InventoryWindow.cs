using System;
using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryWindow : MonoBehaviour
{
    [SerializeField] Inventory inventory;
    [SerializeField] RectTransform itemsPanel;


    // Start is called before the first frame update
    void Start()
    {
        inventory.onItemAdded += onItemAdded;
        redraw();
    }

    private void onItemAdded(Item item) => redraw();
    
  

    private void Update()
    {
        
    }

    void redraw() {
        for (var i = 0; i < inventory.InventoryItem.Count; i++) {
            var icon = new GameObject("Icon");
            var item = inventory.InventoryItem[i];
            icon.AddComponent<Image>().sprite = item.Icon;
            icon.transform.SetParent(itemsPanel);
        }
    }
}
