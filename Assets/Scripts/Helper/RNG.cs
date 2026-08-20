using System;
using System.Collections.Generic;

public class RNG
{
    private readonly System.Random _random;

    public RNG()
    {
        _random = new System.Random(Guid.NewGuid().GetHashCode()); 
    }

    /*Generic Method to pick a Random item from the list based on their weight
     Probability of choosing the object is calculated buy object_weight/total_weight */
    public T Pick<T>(IList<T> items,  Func<T, float> selector)
    {
        if (items == null || items.Count == 0)
            throw new ArgumentException("Empty list");

        float totalWeight = 0;
        for (int i = 0; i < items.Count; i++)
            totalWeight += selector(items[i]);

        float pickedWeight = ((float)_random.NextDouble()) * totalWeight;
        float weightSum = 0;

        for (int i = 0; i < items.Count; i++)
        {
            weightSum += selector(items[i]);
            if (pickedWeight < weightSum)
            {
                return items[i];
            }
        }
        
        return items[items.Count - 1];
    }
}
