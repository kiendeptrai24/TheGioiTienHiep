



using UnityEngine;

public interface IChampionAnimation
{
    public void PlayAnimationSkill(string skillid);
    public void PlayAnimationAttack();
    public void PlayMovement(Vector3 destination);
}