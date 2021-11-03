using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public struct bag
{
    public static List<Item> hand;
    public static List<Item> items;
    public static Item item;
    public static Inventory inventory;
}

public class Item
{
    public int nameIndex = 0;
    public int damage1Index = 1;
    public int damage2Index = 2;
    public int damage3Index = 3;
    public int damage4Index = 4;
    public int resistIndex = 5;
    public int hpIndex = 6;
    public int descIndex = 7;

    public string[] array = new string[8];
}

public class Inventory
{
    public Dropdown menu;
    public Text stats;
    
    public void equipItem(int item)
    {
        bag.item = bag.hand[item];

        var i = bag.hand[item];
        
        stats.text = $"Health: +{i.array[i.hpIndex]}\nResist: +{i.array[i.resistIndex]}\nJab Damage: +{i.array[i.damage1Index]}\nSpit Damage: +{i.array[i.damage2Index]}\nCounter Multiplier: +{i.array[i.damage3Index]}\nThrow Towel Damage: +{i.array[i.damage4Index]}\nDesc: {i.array[i.descIndex]}";
    }

    public Inventory()
    {
        menu = GameObject.Find("Inventory").GetComponent<Dropdown>();
        stats = GameObject.Find("InventoryText").GetComponent<Text>();
        
        bag.items = new List<Item>();
        bag.hand = new List<Item>();
        
        // stats.text = "Stats Bonus:\n\nHealth: +0\nResist: +0\nJab Damage: +0\nSpit Damage: +0\nCounter Multiplier: +0\nThrow Towel Damage: +0";

        // string[] lines = System.IO.File.ReadAllLines("Assets/Resources/Data/Items.txt");

        TextAsset file = (TextAsset) Resources.Load("Data/Items");

        string[] lines = file.text.Split('\n');
        
        for (int i = 1; i < lines.Length; ++i)
        {
            bag.items.Add(new Item());
            
            int j = 0;
            foreach (string word in lines[i].Split(','))
            {
                bag.items[i - 1].array[j] = word;
                ++j;
            }
        }
        
        //Start player off with no item in inventory
        bag.hand.Add(bag.items[0]);
        menu.options.Add(new Dropdown.OptionData() {text=bag.items[0].array[bag.items[0].nameIndex]});
        menu.onValueChanged.AddListener(equipItem);
        
        equipItem(0);
    }

    public void addItem()
    {
        int index = UnityEngine.Random.Range(0, bag.items.Count);
        
        if (bag.hand.Contains(bag.items[index]))
            return;
        
        bag.hand.Add(bag.items[index]);
        menu.options.Add(new Dropdown.OptionData() {text=bag.items[index].array[bag.items[index].nameIndex]});
        menu.onValueChanged.AddListener(equipItem);
    }
}
