using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class UsarinBoss : Boss
{
    public List<Bullet> allBullets;
    public List<Enemy> allEnemies;

    [SerializeField] GameObject bigCarrotPrefab;
    [SerializeField] GameObject carrotPrefab;
    [SerializeField] GameObject blueCarrotPrefab;
    [SerializeField] GameObject bigBulletPrefab;

    private enum UsarinBossState
    {
        Dance1, Dance2, Dance3, Defeat
    }

    private UsarinBossState usarinState;
    private bool isPreparingAttack;
    private int attackBeat;

    private int hpThreshold1, hpThreshold2;

    [SerializeField] SpriteRenderer magicCircle;
    private bool magicCircleVisible;

    private Vector3 targetPos;
    private float orbitalBaseAngle;
    private bool orbitRight;

    List<BlueUsarinCarrotBullet> sideBullets;

    public override void OnSpawn()
    {
        base.OnSpawn();

        allBullets = new List<Bullet>();
        allEnemies = new List<Enemy>();
        Map.Instance.enemiesAlive.Add(this);
        CurrentHP = MaxHP;
        emissionColor = new Color(1, 1, 1, 0);
        isMoving = false;
        Sprite.transform.localPosition = Vector3.zero;
        State = BossState.Introduction;// FALTA LA ANIMACION DE INTRODUCCION DEL JEFE
        usarinState = UsarinBossState.Dance1;
        attackBeat = 0;
        isPreparingAttack = true;
        hpThreshold1 = (int)(MaxHP / 3f) * 2; // 2/3
        hpThreshold2 = (int)(MaxHP / 3f); // 1/3
        magicCircle.color = new Color(1, 1, 1, 0);
        magicCircle.transform.localScale = Vector3.one * 1.5f;
        magicCircleVisible = false;
        orbitRight = true;
        sideBullets = new List<BlueUsarinCarrotBullet>();

        PoolManager.CreatePool(typeof(BigUsarinCarrotBullet), bigCarrotPrefab, 100);
        PoolManager.CreatePool(typeof(UsarinCarrotBullet), carrotPrefab, 100);
        PoolManager.CreatePool(typeof(BlueUsarinCarrotBullet), blueCarrotPrefab, 350);
        PoolManager.CreatePool(typeof(BigUsarinBullet), bigBulletPrefab, 100);
        animator.Play("usarin_intro");
    }

    public override void OnIntroductionFinish()
    {
        base.OnIntroductionFinish();
        animator.Play("usarin_normal");
    }

    private void FindTargetPositionAroundPlayer()
    {
        Vector3 finalPos = Vector3.zero;

        int tries = 10;
        while (finalPos == Vector3.zero)
        {
            Vector3 basePos = Map.Instance.bossArea.transform.position;
            basePos = basePos + (Vector3)Random.insideUnitCircle * 5f;
            if (tries <= 0) break;

            if (!Map.isWallAt(basePos))
            {
                finalPos = basePos;
                targetPos = basePos;
            }
            tries--;
        }
        
    }

    protected override void OnBeat()
    {
        if (State == BossState.Phase1)
        {
            if (usarinState == UsarinBossState.Dance1) OnPattern1();
            if (usarinState == UsarinBossState.Dance2) OnPattern2();
            if (usarinState == UsarinBossState.Dance3) OnPattern3();
        }
    }
    
    protected override void OnBehaviourUpdate()
    {
        switch (State)
        {
            case BossState.Dialogue:
                State = BossState.Phase1;
                usarinState = UsarinBossState.Dance1;
                OnBattleStart();
                break;
            case BossState.Phase1:
                if (usarinState == UsarinBossState.Dance1) UpdatePhase1();
                if (usarinState == UsarinBossState.Dance2) UpdatePhase2();
                break;
        }

        if (magicCircleVisible)
        {
            magicCircle.color = Color.Lerp(magicCircle.color, Color.white, Time.deltaTime);
            magicCircle.transform.localScale = Vector3.Lerp(magicCircle.transform.localScale, Vector3.one, Time.deltaTime);
        }
        else
        {
            magicCircle.color = Color.Lerp(magicCircle.color, new Color(1,1,1,0), Time.deltaTime);
        }
        magicCircle.transform.localEulerAngles = new Vector3(45f, 0, magicCircle.transform.localEulerAngles.z + (Time.deltaTime * 50f));
    }

    private void UpdatePhase1()
    {

    }

    private void UpdatePhase2()
    {

    }

    private IEnumerator ChargeAttack1Coroutine(bool reset = true)
    {
        yield return new WaitForSeconds(BeatManager.GetBeatDuration() * 2f);
        while (!BeatManager.isGameBeat) yield return new WaitForEndOfFrame();

        isPreparingAttack = false;
        if (reset) attackBeat = 0;
        yield break;
    }

    public void SpawnBunnies(Vector3 position)
    {
        List<NomSlime> wave = new List<NomSlime>();
        // Spawn Bunnies
        for (int i = 0; i < 5; i++)
        {
            float angle = (360 / 5) * i;

            Vector3 pos = position + new Vector3(1.5f * (Mathf.Cos(angle * Mathf.Deg2Rad)), 1.5f * (Mathf.Sin(angle * Mathf.Deg2Rad)));
            NomSlime nomSlime = (NomSlime)GetEnemyOfType(EnemyType.NomSlime);
            nomSlime.OnSpawn();
            nomSlime.AItype = 1;

            nomSlime.MaxHP = 2000;
            nomSlime.CurrentHP = 2000;
            nomSlime.transform.position = pos;


            wave.Add(nomSlime);
            allEnemies.Add(nomSlime);

            SmokeExplosion smokeExplosion = PoolManager.Get<SmokeExplosion>();
            smokeExplosion.transform.position = pos;
        }
        Vector3 playerPos = Player.instance.GetClosestPlayer(position);
        // Send them
        foreach (Enemy enemy in wave)
        {
            Vector2 dir = (playerPos - enemy.transform.position).normalized;
            enemy.AItype = 2;
            enemy.lifeTime = 10;
            enemy.eventMove = dir;
        }

        
    }

    void OnPattern1()
    {
        // Charge attack for 2 seconds
        if (attackBeat == 0 && isPreparingAttack)
        {
            AudioController.PlaySound(AudioController.instance.sounds.bossChargeAttack);
            attackBeat = -1;
            magicCircle.color = new Color(1, 1, 1, 0);
            magicCircle.transform.localScale = Vector3.one * 1.5f;
            magicCircleVisible = true;

            StartCoroutine(ChargeAttack1Coroutine());
        }
        if (isPreparingAttack) return; // Wait until the attack is charged;

        if (attackBeat >= 2 && attackBeat <= 6) // Carrot Bullets
        {
            StartCoroutine(ShootBulletsPhase1());
        }
        if (attackBeat == 0) // Bunnies
        {
            StartCoroutine(ShootSecondCarrots());
        }

        if (attackBeat == 6) FindTargetPositionAroundPlayer();
        if (attackBeat > 6)
        {
            if (attackBeat == 10)attackBeat = -1;
            if (attackBeat > 6) Move();

        }
        attackBeat++;
    }

    private IEnumerator ShootBulletsPhase1()
    {
        float angle = 0;
        if (attackBeat == 2) angle = 225;
        else if (attackBeat == 3) angle = 315;
        else if (attackBeat == 4) angle = 160;
        else if (attackBeat == 5) angle = 15;
        else yield break;

        yield return ShootCircleOfBullets(angle);
    }

    private IEnumerator ShootCircleOfBullets(float angle)
    {
        Vector2 patternDir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
        List<UsarinCarrotBullet> bullets = new List<UsarinCarrotBullet>();
        for (int i = 0; i < 8; i++)
        {
            float offsetAngle = i * 45f;
            Vector2 offsetDir = new Vector2(Mathf.Cos(offsetAngle * Mathf.Deg2Rad), Mathf.Sin(offsetAngle * Mathf.Deg2Rad));
            AudioController.PlaySound(AudioController.instance.sounds.shootBullet);
            UsarinCarrotBullet bullet = PoolManager.Get<UsarinCarrotBullet>();
            bullet.transform.position = transform.position + (Vector3.up * 0.5f) + (Vector3)(offsetDir * 0.8f) + (Vector3)(patternDir * (1.5f));
            bullet.direction = patternDir;
            bullet.origSpeed = 0;// attackBeat < 2 ? 12f : 20f;
            bullet.speed = 0;// attackBeat < 2 ? 12f : 20f;
            bullet.atk = 3;
            bullet.lifetime = 8;
            bullet.transform.localScale = Vector3.one;
            bullet.OnSpawn();
            allBullets.Add(bullet);
            if (!allBullets.Contains(bullet)) allBullets.Add(bullet);
            bullets.Add(bullet);
            yield return new WaitForSeconds(BeatManager.GetBeatDuration() / 8f);
        }

        yield return new WaitForSeconds(BeatManager.GetBeatDuration() * 3);

        foreach (Bullet b in bullets) b.lifetime = 8;

        Vector3 center = Vector3.zero;
        foreach (var bullet in bullets)
        {
            center += bullet.transform.position;
        }

        center /= 8f;
        Vector3 playerPos = Player.instance.GetClosestPlayer(center);
        Vector2 dir = (playerPos - center).normalized;

        foreach (UsarinCarrotBullet bullet in bullets)
        {
            bullet.phase = 3;
            bullet.direction = dir;
            bullet.angle = Vector2.SignedAngle(Vector2.down, dir) - 90;
            bullet.speed = 9;
            bullet.origSpeed = 9;
        }
        yield break;
    }

    private IEnumerator ShootSecondCarrots()
    {
        for (int i = 0; i < 36; i++)
        {
            float offsetAngle = i * 10f;
            Vector2 offsetDir = new Vector2(Mathf.Cos(offsetAngle * Mathf.Deg2Rad), Mathf.Sin(offsetAngle * Mathf.Deg2Rad));

            BlueUsarinCarrotBullet bullet = PoolManager.Get<BlueUsarinCarrotBullet>();
            bullet.transform.position = transform.position + (Vector3.up * 0.5f) + (Vector3)(offsetDir * 0.3f);
            bullet.direction = offsetDir;
            bullet.origSpeed = 8;// attackBeat < 2 ? 12f : 20f;
            bullet.speed = 8;// attackBeat < 2 ? 12f : 20f;
            bullet.atk = 2;
            bullet.lifetime = 8;
            bullet.transform.localScale = Vector3.one;
            bullet.targetSpeed = 6;
            bullet.OnSpawn();
            if (!allBullets.Contains(bullet)) allBullets.Add(bullet);
        }
        AudioController.PlaySound(AudioController.instance.sounds.shootBullet);
        yield break;
    }

    void OnPattern2()
    {
        // Charge attack for 2 seconds
        if (attackBeat == 0 && isPreparingAttack)
        {
            AudioController.PlaySound(AudioController.instance.sounds.bossChargeAttack);
            attackBeat = -1;
            magicCircle.color = new Color(1, 1, 1, 0);
            magicCircle.transform.localScale = Vector3.one * 1.5f;
            magicCircleVisible = true;

            StartCoroutine(ChargeAttack1Coroutine());
        }
        if (isPreparingAttack) return; // Wait until the attack is charged;

        StartCoroutine(ShootBulletsPhase2());
        if (attackBeat < 16) // Carrot Bullets
        {

        }

        if (attackBeat % 4 == 0)
        {
            StartCoroutine(ShootBigBullets());
        }

        if (attackBeat % 16 == 0)
        {
            //orbitRight = !orbitRight;

        }
        if (attackBeat == -1) orbitRight = !orbitRight;
        attackBeat++;
    }

    private IEnumerator ShootBulletsPhase2()
    {
        for (int i = 0; i < 3; i++)
        {
            float angle = (orbitalBaseAngle + (i * 120)) * Mathf.Deg2Rad;
            Vector2 angleDir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

            BigUsarinCarrotBullet bullet = PoolManager.Get<BigUsarinCarrotBullet>();
            bullet.transform.position = transform.position + (Vector3.up * 0.5f);
            bullet.direction = angleDir;
            bullet.origSpeed = 14f;
            bullet.speed = 14f;
            bullet.atk = 2;
            bullet.lifetime = 12;
            bullet.transform.localScale = Vector3.one;
            bullet.rotateRight = orbitRight;
            bullet.OnSpawn();
            if (!allBullets.Contains(bullet)) allBullets.Add(bullet);
        }

        AudioController.PlaySound(AudioController.instance.sounds.shootBullet);
        orbitalBaseAngle += orbitRight ? 11 : -11;
        yield break;
    }

    private IEnumerator ShootBigBullets()
    {
        for (int i = 0; i < 5; i++)
        {
            float angle = (-orbitalBaseAngle + (i * 72)) * Mathf.Deg2Rad;
            Vector2 angleDir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

            BigUsarinBullet bullet = PoolManager.Get<BigUsarinBullet>();
            bullet.transform.position = transform.position + (Vector3.up * 1f);
            bullet.direction = angleDir;
            bullet.origSpeed = 7f;
            bullet.speed = 7f;
            bullet.atk = 5;
            bullet.lifetime = 12;
            bullet.transform.localScale = Vector3.one;
            bullet.OnSpawn();
            if (!allBullets.Contains(bullet)) allBullets.Add(bullet);
        }

        AudioController.PlaySound(AudioController.instance.sounds.shootBullet);
        yield break;
    }

    void OnPattern3()
    {

        // Charge attack for 2 seconds
        if (attackBeat == 0 && isPreparingAttack)
        {
            AudioController.PlaySound(AudioController.instance.sounds.bossChargeAttack);
            attackBeat = -1;
            magicCircle.color = new Color(1, 1, 1, 0);
            magicCircle.transform.localScale = Vector3.one * 1.5f;
            magicCircleVisible = true;

            StartCoroutine(ChargeAttack1Coroutine());
        }
        if (isPreparingAttack) return; // Wait until the attack is charged;

        if (attackBeat == 0) orbitRight = false;
        if ((attackBeat - 2) % 24 == 0)
        {
            AudioController.PlaySound(AudioController.instance.sounds.bossChargeAttack);
            magicCircle.color = new Color(1, 1, 1, 0);
            magicCircle.transform.localScale = Vector3.one * 1.5f;
            magicCircleVisible = true;

            StartCoroutine(ChargeAttack1Coroutine(false));
        }
        if (attackBeat % 6 == 0) // Carrot Bullets
        {
            StartCoroutine(ShootSpinningCarrots());
            StartCoroutine(ShootSideCarrots());
        }

        if (attackBeat % 6 == 0) FindTargetPositionAroundPlayer();
        if (attackBeat % 6 != 0)
        {
            Move();
        }

        attackBeat++;
    }

    private IEnumerator ShootSpinningCarrots()
    {
        List<BigUsarinCarrotBullet> bullets = new List<BigUsarinCarrotBullet>();
        for (int i = 0; i < 21; i++)
        {
            float angle = ((i * 2) - 21) * Mathf.Deg2Rad;
            Vector2 angleDir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

            BigUsarinCarrotBullet bullet = PoolManager.Get<BigUsarinCarrotBullet>();
            bullet.transform.position = transform.position + (Vector3.up * 2f);
            bullet.direction = angleDir;
            bullet.origSpeed = 15f;
            bullet.speed = 15f;
            bullet.atk = 2;
            bullet.lifetime = 30;
            bullet.transform.localScale = Vector3.one;
            bullet.ai = 1;
            bullet.OnSpawn();
            allBullets.Add(bullet);
            bullets.Add(bullet);
            if (!allBullets.Contains(bullet)) allBullets.Add(bullet);
        }

        AudioController.PlaySound(AudioController.instance.sounds.shootBullet);

        yield return new WaitForSeconds(BeatManager.GetBeatDuration() * 4);
        Vector2 arenaPos = Map.Instance.bossArea.transform.position; // Change this to the arena with the boss
        for (int i = 0; i < 21; i++)
        {
            float xoffset = i - 10;
            bullets[i].transform.position = arenaPos + (Vector2.up * 10) + (Vector2.right * xoffset);
            bullets[i].origSpeed = 6;
            bullets[i].angle = -90 + Random.Range(-30f, 30f);
            bullets[i].direction = Vector2.down;
            bullets[i].ai = 2;
        }

        yield break;
    }

    private IEnumerator ShootSideCarrots()
    {
        Vector2 arenaPos = Map.Instance.bossArea.transform.position; // Change this to the arena with the boss

        if (attackBeat % 24 == 0)
        {
            orbitRight = !orbitRight;
            string anim = orbitRight ? "bluecarrot" : "orangecarrot";
            List<BlueUsarinCarrotBullet> temp = new List<BlueUsarinCarrotBullet>(sideBullets);
            foreach (BlueUsarinCarrotBullet bullet in temp)
            {
                if (!bullet.gameObject.activeSelf)
                {
                    sideBullets.Remove(bullet);
                    continue;
                }
                bullet.animator.Play(anim);
                bullet.angle += 180;
            }
        }


        for (int i = 0; i < 20; i++)
        {
            float angle = ((orbitRight ? 0f : 180f) + Random.Range(-20f, 20f)) * Mathf.Deg2Rad;
            Vector2 angleDir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

            BlueUsarinCarrotBullet bullet = PoolManager.Get<BlueUsarinCarrotBullet>();
            float yoffset = (i / 3f) - 3;
            bullet.transform.position = arenaPos + ((orbitRight ? Vector2.left : Vector2.right) * 14) + (Vector2.up * yoffset * 2);
            bullet.direction = angleDir;
            bullet.origSpeed = 5f;
            bullet.speed = 5f;
            bullet.atk = 2;
            bullet.lifetime = 50;
            bullet.transform.localScale = Vector3.one;
            bullet.ai = 1;
            bullet.animator.Play(orbitRight ? "bluecarrot" : "orangecarrot");
            bullet.OnSpawn();
            if (!allBullets.Contains(bullet)) allBullets.Add(bullet);

            sideBullets.Add(bullet);
        }

        AudioController.PlaySound(AudioController.instance.sounds.shootBullet);
        yield break;
    }

    private void EndAttack()
    {
        StopAllCoroutines();
        rb.velocity = Vector2.zero;
        velocity = Vector2.zero;
        Player.TriggerCameraShake(0.4f, 0.3f);
        foreach (Bullet b in allBullets)
        {
            if (b.gameObject.activeSelf) b.ForceDespawn();
        }
        allBullets.Clear();
        foreach (Enemy e in allEnemies)
        {
            e.ForceDespawn();
        }
        allEnemies.Clear();
        isPreparingAttack = true;
        attackBeat = 0;

        if (usarinState == UsarinBossState.Dance1) 
        {
            usarinState = UsarinBossState.Dance2;
            orbitalBaseAngle = 45;
        }
        else if (usarinState == UsarinBossState.Dance2)
        {
            usarinState = UsarinBossState.Dance3;
        } 
        else
        {
            PoolManager.RemovePool(typeof(BigUsarinCarrotBullet));
            PoolManager.RemovePool(typeof(UsarinCarrotBullet));
            PoolManager.RemovePool(typeof(BlueUsarinCarrotBullet));
            PoolManager.RemovePool(typeof(BigUsarinBullet));
            usarinState = UsarinBossState.Defeat;
            State = BossState.Defeat;
        }
    }

    protected override void OnInitialize()
    {

    }

    void MoveTowardsPlayer()
    {
        if (isMoving) return;

        Move();

    }

    public void Move()
    {
        StartCoroutine(MoveCoroutine());
    }

    IEnumerator MoveCoroutine()
    {
        isMoving = true;

        float time = 0;
        GameObject posObj = new GameObject("objpos");
        posObj.transform.position = targetPos;
        Vector3 playerPos = targetPos;
        Vector2 dir = (playerPos - transform.position).normalized;
        facingRight = dir.x > 0;
        while (time <= BeatManager.GetBeatDuration() / 3f)
        {
            velocity = dir * speed * 8;
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        AudioController.PlaySound(AudioController.instance.sounds.bossWalk);
        Player.TriggerCameraShake(0.5f, 0.2f);
        velocity = Vector2.zero;
        Sprite.transform.localPosition = Vector3.zero;

        isMoving = false;
        yield break;
    }

    public override bool CanTakeDamage()
    {
        return true;
    }

    public override void TakeDamage(int damage, bool isCritical)
    {
        base.TakeDamage(damage, isCritical);

        if (usarinState == UsarinBossState.Dance1 && CurrentHP < hpThreshold1) EndAttack();
        else if (usarinState == UsarinBossState.Dance2 && CurrentHP < hpThreshold2) EndAttack();
        else if (usarinState == UsarinBossState.Dance3 && CurrentHP <= 0) EndAttack();
    }

    public override string GetName()
    {
        return "Usarin";
    }
}