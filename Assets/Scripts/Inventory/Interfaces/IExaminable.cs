using UnityEngine;

namespace TheHunt.Inventory
{
    public interface IExaminable
    {
        string ExaminationText { get; }
        Sprite ExaminationImage { get; }
        bool HasDetailedView { get; }
    }
}
