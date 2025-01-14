using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public Action<Item> onItemAdded;

    [SerializeField] List<Key> StartKey = new List<Key>();
    [SerializeField] List<Consumable> StartCons = new List<Consumable>();
    [SerializeField] List<ArmorItem> StartArmor = new List<ArmorItem>();
    [SerializeField] List<WeaponItem> StartWeapon = new List<WeaponItem>();
    public List<Item> InventoryItem = new List<Item>();
    public bool hidden = true;
    // Start is called before the first frame update


    void Awake()
    {
        for (var i = 0; i < StartKey.Count; i++) {
            addItem(StartKey[i]);
        }
    }

  

    void addItem(Item item) {
        
        InventoryItem.Add(item);
        onItemAdded?.Invoke(item);
        
       
    }
}
