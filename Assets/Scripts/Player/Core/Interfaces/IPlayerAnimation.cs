using UnityEngine;

public interface IPlayerAnimation
{
    void SetBool(string parameterName, bool value);
    void SetFloat(string parameterName, float value);
    void SetTrigger(string parameterName);
    void AnimationTrigger();
    void AnimationFinishTrigger();
}
