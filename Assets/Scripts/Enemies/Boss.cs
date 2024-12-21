using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Enemy
{
    public BossState State;
    public MapTrack bossTrack;
    [SerializeField] GameObject defeatEffect;
    public override void OnSpawn()
    {
        base.OnSpawn();
        Stage.Instance.enemiesAlive.Add(this);
        CurrentHP = MaxHP;
        emissionColor = new Color(1, 1, 1, 0);
        isMoving = false;
        Sprite.transform.localPosition = Vector3.zero;
        State = BossState.Introduction;// FALTA LA ANIMACION DE INTRODUCCION DEL JEFE
        Player.instance.facingRight = transform.position.x > Player.instance.transform.position.x;
    }

    protected override void OnBeat()
    {
        
    }

    public void OnStart()
    {
        State = BossState.Dialogue;
    }

    protected override void OnBehaviourUpdate()
    {
        switch (State)
        {
            case BossState.Dialogue:
                Debug.Log("changing to phase 1");
                State = BossState.Phase1;
                OnBattleStart();
                break;
        }
    }

    public virtual void OnIntroductionFinish()
    {

    }

    protected override void OnInitialize()
    {
        
    }

    public override void TakeDamage(float damage, bool isCritical)
    {
        if (State == BossState.Introduction || State == BossState.Dialogue || State == BossState.Defeat) return;
        base.TakeDamage(damage, isCritical);
        UIManager.Instance.PlayerUI.UpdateBossBar(CurrentHP, MaxHP);
    }

    public override bool CanTakeDamage()
    {
        if (State == BossState.Introduction || State == BossState.Dialogue) return false;
        return true;
    }

    public override void Die()
    {
        bool full = true;
        if (Player.instance == null) full = false;
        else if (Player.instance.isDead) full = false;
        State = BossState.Defeat;
        if (full)
        {
            GameObject o = Instantiate(defeatEffect);
            o.transform.position = transform.position;
            o.SetActive(true);

            AudioController.PlaySound(AudioController.instance.sounds.bossDeath, side: true);
            Stage.Instance.OnBossDeath(this);
        }
        else
        {
            PoolManager.Return(gameObject, GetType());
        }
        
    }

    public virtual string GetName()
    {
        return "Unnamed Boss";
    }

    public virtual IEnumerator OnBattleStart()
    {
        // Fade off music
        UIManager.Instance.PlayerUI.OnCloseMenu();
        BeatManager.FadeOut(1);
        yield return new WaitForSeconds(1f);
        UIManager.Instance.PlayerUI.UpdateBossBar(CurrentHP, MaxHP);
        UIManager.Instance.PlayerUI.SetBossBarName(GetName());
        UIManager.Instance.PlayerUI.SetStageText($"{Localization.GetLocalizedString("playerui.stageboss")}");
        BeatManager.SetTrack(bossTrack);
        BeatManager.StartTrack();
        Stage.isBossWave = true;
        Player.instance.canDoAnything = true;
        State = BossState.Phase1;
        //usarinState = UsarinBossState.Dance1;
    }
}
