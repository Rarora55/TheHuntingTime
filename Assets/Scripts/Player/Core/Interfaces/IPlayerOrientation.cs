using UnityEngine;

public interface IPlayerOrientation
{
    int FacingDirection { get; }
    
    void Flip();
    void CheckFlip(int xInput);
}
