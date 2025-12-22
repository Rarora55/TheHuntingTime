using System.Collections.Generic;

namespace TheHunt.Inventory
{
    public interface ICombinable
    {
        bool CanCombineWith(ItemData otherItem);
        string GetCombinationHint(ItemData otherItem);
    }
}
