using System;
using System.Collections.Generic;
using UnityEngine;

public class WeightedRandomBag<T>
{
    private class BagItem
    {
        public T Item { get; set; }
        public float Weight { get; set; }
        public float CumulativeWeight { get; set; }
    }

    private List<BagItem> items;
    private float totalWeight;
    private System.Random random;

    public WeightedRandomBag()
    {
        items = new List<BagItem>();
        totalWeight = 0f;
        random = new System.Random();
    }

    public void Add(T item, float weight)
    {
        if (weight <= 0)
        {
            throw new ArgumentOutOfRangeException("Weight must be greater than zero.");
        }

        totalWeight += weight;
        items.Add(new BagItem
        {
            Item = item,
            Weight = weight,
            CumulativeWeight = totalWeight
        });
    }

    public T GetRandom()
    {
        float r = (float)random.NextDouble() * totalWeight;

        foreach (var item in items)
        {
            if (r < item.CumulativeWeight)
            {
                return item.Item;
            }
        }

        return default;
    }
}