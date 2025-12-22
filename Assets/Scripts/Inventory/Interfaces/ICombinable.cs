using System.Collections.Generic;

namespace TheHunt.Inventory
{
    public interface ICombinable
    {
        bool CanCombineWith(ItemData otherItem);
        
        List<ItemData> GetPossibleCombinations();
        
        string GetCombinationHint(ItemData otherItem);
    }
}
