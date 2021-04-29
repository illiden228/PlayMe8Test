using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Lot
{
    public Item Item;
    public Player Player;
    public int ItemCount;

    public Lot(Item item, Player player, int count)
    {
        Item = item;
        Player = player;
        ItemCount = count;
    }
}
