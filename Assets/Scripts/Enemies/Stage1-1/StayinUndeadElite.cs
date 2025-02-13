using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StayinUndeadElite : Enemy
{
    int beatCD;
    private int phase;
    private Vector3 targetPos;
    float spiralBaseAngle;
    int balls = 0;
    bool shootDisco;

    private List<Vector2> positionNodes = new List<Vector2>()
    {
        new Vector2(-6.75f, 0), // Left Up
        new Vector2(0,0), // Center
        new Vector2(6.75f, 0), // Right Up
        new Vector2(0, -3.5f) // Center Down
    };

    public override void OnSpawn()
    {
        shootDisco = false;
        phase = 0;
        beatCD = -1;
        base.OnSpawn();
        UIManager.Instance.PlayerUI.SetBossBarName("Stayin' Undead");
        UIManager.Instance.PlayerUI.ShowBossBar(true);
        UIManager.Instance.PlayerUI.UpdateBossBar(CurrentHP, MaxHP);
        speed = 1.0f;
        canBeKnocked = false;
    }

    protected override void OnBeat()
    {
        switch (phase)
        {
            case 0:
                SpinPhase();
                break;
            case 1:
                MoveDisco();
                break;
            case 2:
                MoveToCenter();
                break;
        }
    }

    private void SpinPhase()
    {
        canBeKnocked = false;
        beatCD++;
        animator.Play("stayinundead_spin");

        if (beatCD == 0) StartCoroutine(SpinCoroutine());
        if (beatCD >= 6)
        {
            phase = 1;
            balls = 0;
            beatCD = -1;
            shootDisco = true;
        }
    }

    private IEnumerator SpinCoroutine()
    {
        AudioController.PlaySound(AudioController.instance.sounds.chargeBulletSound);
        BulletSpawnEffect bulletSpawnEffect = PoolManager.Get<BulletSpawnEffect>();
        bulletSpawnEffect.source = this;
        bulletSpawnEffect.transform.position = transform.position;
        bulletSpawnEffect.finalScale = 1f;
        float time = 0;
        while (time <= BeatManager.GetBeatDuration())
        {
            while (GameManager.isPaused) yield return null; // isStunned!
            time += Time.deltaTime;
            yield return null;
        }
        while (GameManager.isPaused || stunStatus.isStunned()) yield return null;
        while (!BeatManager.isBeat) yield return new WaitForSeconds(BeatManager.GetBeatDuration());
        ShootCircle(0);
        ShootCircle(-1f);
        ShootCircle(-2f);


        time = 0;
        while (time <= BeatManager.GetBeatDuration())
        {
            while (GameManager.isPaused) yield return null; // isStunned!
            time += Time.deltaTime;
            yield return null;
        }
        AudioController.PlaySound(AudioController.instance.sounds.chargeBulletSound);
        time = 0;
        while (time <= BeatManager.GetBeatDuration())
        {
            while (GameManager.isPaused) yield return null; // isStunned!
            time += Time.deltaTime;
            yield return null;
        }

        ShootCircle(0);
        ShootCircle(-1f);
        ShootCircle(-2f);

        while (beatCD <= 4)
        {
            while (!BeatManager.isBeat) yield return null;
            while (GameManager.isPaused || stunStatus.isStunned()) yield return null;
            yield return null;
        }
        bulletSpawnEffect.Despawn();
        animator.Play("stayinundead_normal");
    }

    private void ShootCircle(float speedchange)
    {
        float diff = 360f / 6f;

        for (int i = 0; i < 6; i++)
        {
            float angle = spiralBaseAngle + (i * diff);

            SpawnStarBullet(angle, 6, 0.7f, speedchange);
        }
        spiralBaseAngle += 10f;
    }

    private void MoveDisco()
    {
        if (shootDisco)
        {
             if (balls <= 3) StartCoroutine(DiscoMoveCoroutine());
             else
             {
                phase = 2;
                beatCD = -1;
                balls = 0;
             }
        }
    }

    private IEnumerator DiscoMoveCoroutine()
    {
        shootDisco = false;
        // First move towards the shooting point
        Vector2 camPos = Camera.main.transform.position;

        if (balls == 0) targetPos = positionNodes[1] + camPos;
        else if (balls == 1) targetPos = positionNodes[0] + camPos;
        else if (balls == 2) targetPos = positionNodes[3] + camPos;
        else if (balls == 3) targetPos = positionNodes[2] + camPos;

        while ((Vector2)transform.position != (Vector2)targetPos)
        {
            if (BeatManager.isBeat && CanMove()) StartCoroutine(MoveToTarget());
            yield return null;
        }
        // Charge it
        AudioController.PlaySound(AudioController.instance.sounds.chargeBulletSound);
        BulletSpawnEffect bulletSpawnEffect = PoolManager.Get<BulletSpawnEffect>();
        bulletSpawnEffect.source = this;
        bulletSpawnEffect.transform.position = transform.position;
        bulletSpawnEffect.finalScale = 1f;
        float time = 0;
        while (time <= BeatManager.GetBeatDuration() * 2f)
        {
            while (GameManager.isPaused) yield return null; // isStunned!
            time += Time.deltaTime;
            yield return null;
        }

        // Shoot it
        SpawnDiscoBall();
        bulletSpawnEffect.Despawn();

        time = 0;
        while (time <= BeatManager.GetBeatDuration())
        {
            while (GameManager.isPaused) yield return null; // isStunned!
            time += Time.deltaTime;
            yield return null;
        }

        // Dance left
        targetPos = transform.position - Vector3.left * 2f;
        StartCoroutine(MoveToTarget());
        time = 0;
        while (time <= BeatManager.GetBeatDuration())
        {
            while (GameManager.isPaused) yield return null; // isStunned!
            time += Time.deltaTime;
            yield return null;
        }

        // Dance right
        targetPos = transform.position - Vector3.right * 2f;
        StartCoroutine(MoveToTarget());
        time = 0;
        while (time <= BeatManager.GetBeatDuration())
        {
            while (GameManager.isPaused) yield return null; // isStunned!
            time += Time.deltaTime;
            yield return null;
        }
        // Dance left
        targetPos = transform.position - Vector3.left * 2f;
        StartCoroutine(MoveToTarget());
        time = 0;
        while (time <= BeatManager.GetBeatDuration())
        {
            while (GameManager.isPaused) yield return null; // isStunned!
            time += Time.deltaTime;
            yield return null;
        }

        // Dance right
        targetPos = transform.position - Vector3.right * 2f;
        StartCoroutine(MoveToTarget());
        time = 0;
        while (time <= BeatManager.GetBeatDuration())
        {
            while (GameManager.isPaused) yield return null; // isStunned!
            time += Time.deltaTime;
            yield return null;
        }

        // Finish
        balls++;
        shootDisco = true;
        yield break;
    }

    private void MoveToCenter()
    {
        targetPos = (Vector2)Camera.main.transform.position;
        if ((Vector2)transform.position == (Vector2)targetPos)
        {
            phase = 0;
            beatCD = -1;
        }
        else
        {
            if (CanMove()) StartCoroutine(MoveToTarget());
        }
    }

    IEnumerator MoveToTarget()
    {
        isMoving = true;

        float time = 0;
        Vector2 dir = (targetPos - transform.position).normalized;
        facingRight = dir.x > 0;
        animator.Play(moveAnimation);
        while (time <= BeatManager.GetBeatDuration() / 2)
        {
            while (GameManager.isPaused || stunStatus.isStunned()) yield return null;

            velocity = Vector2.zero;
            transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * speed * 6);
            if (transform.position == targetPos) break;
            time += Time.deltaTime;
            yield return null;
        }
        animator.Play(idleAnimation);

        velocity = Vector2.zero;
        Sprite.transform.localPosition = Vector3.zero;

        isMoving = false;
        yield break;
    }

    private BulletBase SpawnStarBullet(float angle, float speed, float dist, float speeddiff)
    {
        AudioController.PlaySound(AudioController.instance.sounds.shootBullet);
        Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

        BulletBase bullet = PoolManager.Get<BulletBase>();

        bullet.transform.position = transform.position + ((Vector3)dir * dist) + (Vector3.up * 0.5f);// + (Vector3)(dir * -dist) + (Vector3.one * 0.5f);
        bullet.direction = dir;
        bullet.speed = 0;
        bullet.atk = 5;
        bullet.lifetime = 8;
        bullet.transform.localScale = Vector3.one;
        bullet.startOnBeat = false;
        bullet.behaviours = new List<BulletBehaviour>
            {
                new SpriteSpinBehaviour() { start = 0, end = -1 },
                new SpeedOverTimeBehaviour() { start = 0, end = -1, targetSpeed = 8 + speeddiff, speedPerBeat = 4f},
            };
        bullet.animator.Play("starbullet");
        bullet.OnSpawn();
        bullet.beat = 0;
        bullet.enemySource = this;
        return bullet;
    }

    private void SpawnDiscoBall()
    {
        AudioController.PlaySound(AudioController.instance.sounds.bulletwaveShootSound);

        BulletBase bullet = PoolManager.Get<BulletBase>();
        Vector2 dir = Player.instance.GetClosestPlayer(transform.position) - transform.position;
        dir.Normalize();
        bullet.transform.position = transform.position + ((Vector3)dir) + (Vector3.up * 0.5f);
        bullet.direction = dir;
        bullet.speed = 3;
        bullet.atk = 3;
        bullet.lifetime = 4;
        bullet.startOnBeat = false;
        bullet.behaviours = new List<BulletBehaviour>
            {
                new SpriteSpinBehaviour() { start = 0, end = -1 },
                new SpawnBulletOnBeatBehaviour(SpawnBlueStar, BeatManager.GetBeatDuration()) { start = 0, end = -1}
            };
        bullet.animator.Play("discobullet");
        bullet.OnSpawn();
        bullet.beat = 0;
        bullet.enemySource = this;
    }

    public void SpawnBlueStar(BulletBase disco)
    {
        AudioController.PlaySound(AudioController.instance.sounds.shootBullet);
        SpawnBlueStarBullet(disco, spiralBaseAngle);
        SpawnBlueStarBullet(disco, spiralBaseAngle + 120);
        SpawnBlueStarBullet(disco, spiralBaseAngle + 240);
        spiralBaseAngle += 45;
    }

    private BulletBase SpawnBlueStarBullet(BulletBase disco, float angle)
    {
        AudioController.PlaySound(AudioController.instance.sounds.shootBullet);
        Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

        BulletBase bullet = PoolManager.Get<BulletBase>();

        bullet.transform.position = disco.transform.position;// + (Vector3)(dir * -dist) + (Vector3.one * 0.5f);
        bullet.direction = dir;
        bullet.speed = 8;
        bullet.atk = 3;
        bullet.lifetime = 4;
        bullet.transform.localScale = Vector3.one;
        bullet.startOnBeat = true;
        bullet.behaviours = new List<BulletBehaviour>
            {
                new SpriteSpinBehaviour() { start = 0, end = -1 }
            };
        bullet.animator.Play("bluestarbullet");
        bullet.OnSpawn();
        bullet.beat = 0;
        bullet.enemySource = this;
        return bullet;
    }

    public override void Die()
    {
        Stage.ForceDespawnBullets();
        base.Die();
    }


    protected override void OnBehaviourUpdate()
    {

    }

    public void Move()
    {
        StartCoroutine(JumpCoroutine());
    }

    protected override IEnumerator JumpCoroutine()
    {
        isMoving = true;

        Vector3 playerPos = Player.instance.GetClosestPlayer(transform.position);
        Vector2 dir = (playerPos - transform.position).normalized;

        facingRight = dir.x > 0;
        animator.Play(moveAnimation);
        animator.speed = 1f / BeatManager.GetBeatDuration() * 1.5f;
        //animator.speed *= 2;

        float time = 0;
        float spd = 6;

        if (dir.x > 0) speed += (dir.x * Time.deltaTime);
        velocity = dir * speed * spd;

        float beatDuration = BeatManager.GetBeatDuration() / 1.5f;
        float beatTime = 1;

        while (time <= BeatManager.GetBeatDuration() / 1.5f)
        {
            while (GameManager.isPaused || stunStatus.isStunned()) yield return null;

            float beatProgress = time / beatDuration;
            beatTime = Mathf.SmoothStep(1, 0f, beatProgress);
            velocity = dir * speed * spd * beatTime;
            time += Time.deltaTime;
            yield return null;
        }
        AudioController.PlaySound(AudioController.instance.sounds.bossWalk);
        PlayerCamera.TriggerCameraShake(0.5f, 0.2f);

        animator.Play(idleAnimation);
        animator.speed = 1f / BeatManager.GetBeatDuration();
        velocity = Vector2.zero;
        Sprite.transform.localPosition = Vector3.zero;

        isMoving = false;
        yield break;
    }

    public override bool CanTakeDamage()
    {
        return true;
    }
}