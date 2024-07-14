using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Enemy
{
    public override void OnSpawn()
    {
        CurrentHP = MaxHP;
        emissionColor = new Color(1, 1, 1, 0);
        isMoving = false;
        Sprite.transform.localPosition = Vector3.zero;
    }

    protected override void OnBeat()
    {
        throw new System.NotImplementedException();
    }

    protected override void OnBehaviourUpdate()
    {
        
    }

    protected override void OnInitialize()
    {
        
    }

    public override bool CanTakeDamage()
    {
        return true;
    }

    public override void Die()
    {
        AudioController.PlaySound(AudioController.instance.sounds.enemyDeathSound);
        Gem gem = PoolManager.Get<Gem>();
        gem.transform.position = transform.position;
        KillEffect deathFx = PoolManager.Get<KillEffect>();
        deathFx.transform.position = transform.position;
        Map.Instance.OnBossDeath();
        PoolManager.Return(gameObject, GetType());
    }
}
