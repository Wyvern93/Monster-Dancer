using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class UsarinBoss : Boss
{
    public List<Bullet> allBullets;
    public List<Enemy> allEnemies;
    private enum UsarinBossState
    {
        Dance1, Dance2, Dance3, Defeat
    }

    private UsarinBossState usarinState;
    private bool isPreparingAttack;
    private int attackBeat;
    bool isDancing;

    private int hpThreshold1, hpThreshold2;

    [SerializeField] SpriteRenderer magicCircle;
    private bool magicCircleVisible;

    private Vector3 targetPos;
    private bool orbitRight, starOrbitRight;
    private float orbitalBaseAngle;
    private float starOrbitAngle;

    List<BulletBase> sideBullets;

    [SerializeField] Cutscene rabiDialogue;

    public override void OnSpawn()
    {
        base.OnSpawn();
        isDancing = false;
        canBeKnocked = false;
        speed = 1.5f;
        shouldMove = false;
        allBullets = new List<Bullet>();
        allEnemies = new List<Enemy>();
        //Stage.Instance.enemiesAlive.Add(this);
        //CurrentHP = MaxHP;
        //emissionColor = new Color(1, 1, 1, 0);
        //isMoving = false;
        //Sprite.transform.localPosition = Vector3.zero;
        //State = BossState.Introduction;// FALTA LA ANIMACION DE INTRODUCCION DEL JEFE
        usarinState = UsarinBossState.Dance1;
        attackBeat = 0;
        isPreparingAttack = true;
        hpThreshold1 = (int)(MaxHP / 3f) * 2; // 2/3
        hpThreshold2 = (int)(MaxHP / 3f); // 1/3
        magicCircle.color = new Color(1, 1, 1, 0);
        magicCircle.transform.localScale = Vector3.one * 1.5f;
        magicCircleVisible = false;
        orbitRight = true;
        sideBullets = new List<BulletBase>();

        animator.Play("usarin_normal");
        animator.speed = 1f / BeatManager.GetBeatDuration();
        if (TestingEnvironment.Instance)
        {
            State = BossState.Dialogue;
        }
        else
        {
            UIManager.Instance.cutsceneManager.StartCutscene(CutsceneType.Boss);
            State = BossState.Dialogue;
        }
        
        transform.localScale = Vector3.one * 2f;
    }

    private void FindTargetPositionAroundPlayer()
    {
        Vector3 finalPos = Vector3.zero;

        int tries = 20;
        while (finalPos == Vector3.zero)
        {
            Vector3 basePos = Stage.Instance.bossArea.transform.position;
            basePos = basePos + (Vector3)Random.insideUnitCircle * 5f;
            if (tries <= 0) break;

            if (!Stage.isWallAt(basePos))
            {
                finalPos = basePos;
                targetPos = basePos;
            }
            tries--;
        }
        
    }

    public override void MoveUpdate()
    {
        facingRight = transform.position.x < targetPos.x;
        if (GameManager.isPaused || stunStatus.isStunned())
        {
            velocity = Vector2.zero;
            return;
        }
        UpdateAnimation();
        if (!shouldMove || !CanMove() || isAttacking)
        {
            velocity = Vector2.zero;
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * 3 * Time.deltaTime);
        velocity = Vector2.zero;
    }

    private void UpdateAnimation()
    {
        AnimatorClipInfo animInfo = animator.GetCurrentAnimatorClipInfo(0)[0];
        if (shouldMove)
        {
            if ((Vector2)transform.position != (Vector2)targetPos) // Moving and not on target
            {
                if (animInfo.clip.name != "usarin_move")
                {
                    animator.Play("usarin_move");
                }
            }
            else // If should move but are on target
            {
                if (isDancing && animInfo.clip.name != "usarin_dance") animator.Play("usarin_dance");
                if (!isDancing && animInfo.clip.name != "usarin_normal") animator.Play("usarin_normal");
            }
        }
        else
        {
            if (isDancing && animInfo.clip.name != "usarin_dance") animator.Play("usarin_dance");
            if (!isDancing && animInfo.clip.name != "usarin_normal") animator.Play("usarin_normal");
        }
    }

    protected override void OnBeat()
    {
        while (GameManager.isPaused) return;
        if (State == BossState.Introduction) return;

        // Move Behaviour
        UpdateAnimation();

        if (Player.instance.transform.position.x < transform.position.x) facingRight = false;
        else facingRight = true;
        if (State == BossState.Phase1)
        {
            if (usarinState == UsarinBossState.Dance1) OnPattern1();
            if (usarinState == UsarinBossState.Dance2) OnPattern2();
            if (usarinState == UsarinBossState.Dance3) OnPattern3();
        }
    }
    protected void OnMidBeat()
    {
        if (usarinState == UsarinBossState.Dance2)
        {
            if (isPreparingAttack) return; // Wait until the attack is charged;
            StartCoroutine(ShootBulletsPhase2());
        }
    }
    
    protected override void OnBehaviourUpdate()
    {
        while (GameManager.isPaused) return;
        switch (State)
        {
            case BossState.Dialogue:
                shouldMove = false;
                if (UIManager.Instance.cutsceneManager.hasFinished)
                {
                    State = BossState.Phase4;
                    StartCoroutine(OnBattleStart());
                    UIManager.Instance.cutsceneManager.hasFinished = false;
                }
                break;
            case BossState.Phase1:
                if (usarinState == UsarinBossState.Dance1) UpdatePhase1();
                if (usarinState == UsarinBossState.Dance2) UpdatePhase2();
                if (BeatManager.isMidBeat) OnMidBeat();
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
        magicCircle.transform.localScale = Vector3.one * 1.5f;
        float time = BeatManager.GetBeatDuration() * (reset ? 3f : 2f);
        while (time > 0)
        {
            while (GameManager.isPaused) yield return null;
            time -= Time.deltaTime;
            yield return null;
        }
        while (!BeatManager.isBeat) yield return null;

        isPreparingAttack = false;
        if (reset) attackBeat = 0;
        animator.Play("usarin_dance");
        yield break;
    }

    void OnPattern1()
    {
        // Charge attack for 2 seconds
        if (attackBeat == 0 && isPreparingAttack)
        {
            isDancing = false;
            shouldMove = false;
            AudioController.PlaySound(AudioController.instance.sounds.bossChargeAttack);
            attackBeat = -1;
            magicCircle.color = new Color(1, 1, 1, 0);
            magicCircle.transform.localScale = Vector3.one * 1.5f;
            magicCircleVisible = true;

            StartCoroutine(ChargeAttack1Coroutine());
        }
        if (isPreparingAttack) return; // Wait until the attack is charged;

        if (attackBeat >= 4 && attackBeat <= 8) // Carrot Bullets
        {
            //animator.Play("usarin_dance");
            isDancing = true;
            shouldMove = false;
            StartCoroutine(ShootBulletsPhase1());
        }
        if (attackBeat == 0) // Bunnies
        {
            shouldMove = false;
            isDancing = true;
            animator.Play("usarin_dance");
            StartCoroutine(ShootSecondCarrots());
        }

        if (attackBeat == 8) FindTargetPositionAroundPlayer();
        if (attackBeat > 8)
        {
            isDancing = false;
            if (attackBeat == 12) attackBeat = -1;
            shouldMove = true;
            //Move();

        }
        attackBeat++;
    }

    private IEnumerator ShootBulletsPhase1()
    {
        float angle = 0;
        if (attackBeat == 4) angle = 225;
        else if (attackBeat == 5) angle = 315;
        else if (attackBeat == 6) angle = 150;
        else if (attackBeat == 7) angle = 25;
        else yield break;

        yield return ShootCircleOfBullets(angle);
    }

    private IEnumerator ShootCircleOfBullets(float angle)
    {
        Vector2 patternDir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
        List<BulletBase> bullets = new List<BulletBase>();
        for (int i = 0; i < 8; i++)
        {
            float offsetAngle = i * 45f;
            Vector2 offsetDir = new Vector2(Mathf.Cos(offsetAngle * Mathf.Deg2Rad), Mathf.Sin(offsetAngle * Mathf.Deg2Rad));
            AudioController.PlaySoundWithoutCooldown(AudioController.instance.sounds.shootBullet);
            BulletBase bullet = PoolManager.Get<BulletBase>();
            bullet.transform.position = transform.position + (Vector3.up * 0.5f) + (Vector3)(offsetDir * 0.6f) + (Vector3)(patternDir * (1.5f));
            bullet.direction = patternDir;
            bullet.speed = 0;
            bullet.atk = 3;
            bullet.lifetime = 6;
            bullet.transform.localScale = Vector3.one;
            bullet.startOnBeat = false;
            bullet.enemySource = this;
            bullet.frozen = true;
            bullet.behaviours = new List<BulletBehaviour>
            {
                new SpriteLookAngleBehaviour() { start = 0, end = -1 },
            };
            bullet.OnSpawn();
            allBullets.Add(bullet);
            if (!allBullets.Contains(bullet)) allBullets.Add(bullet);
            bullets.Add(bullet);


            bullet.animator.Play("orangecarrot");

            yield return new WaitForSeconds(BeatManager.GetBeatDuration() / 8f);
        }

        float time = BeatManager.GetBeatDuration() * 3f;
        while (time > 0)
        {
            while (GameManager.isPaused || stunStatus.isStunned()) yield return null;
            time -= Time.deltaTime;
            yield return null;
        }

        foreach (Bullet b in bullets) b.lifetime = 8;

        Vector3 center = Vector3.zero;
        foreach (var bullet in bullets)
        {
            center += bullet.transform.position;
        }

        center /= 8f;
        Vector3 playerPos = Player.instance.transform.position + (Vector3)(Random.insideUnitCircle.normalized * 2);
        Vector2 dir = (playerPos - center).normalized;

        foreach (BulletBase bullet in bullets)
        {
            bullet.ResetBeat();
            bullet.frozen = false;
            bullet.beat = 0;
            bullet.behaviours.Add(new SpeedOverTimeBehaviour() { speedPerBeat = 100, start = -1, end = -1, targetSpeed = 14 });
            bullet.direction = dir;
            bullet.angle = Vector2.SignedAngle(Vector2.down, dir) - 90;
            bullet.speed = 9;
        }
        yield break;
    }

    private IEnumerator ShootSecondCarrots()
    {
        for (int i = 0; i < 48; i++)
        {
            float offsetAngle = i * 7.5f;
            Vector2 offsetDir = new Vector2(Mathf.Cos(offsetAngle * Mathf.Deg2Rad), Mathf.Sin(offsetAngle * Mathf.Deg2Rad));

            BulletBase bullet = PoolManager.Get<BulletBase>();

            bullet.transform.position = transform.position + (Vector3.up * 0.5f) + (Vector3)(offsetDir * 0.3f);
            bullet.direction = offsetDir;
            bullet.speed = 10;
            bullet.atk = 10;
            bullet.lifetime = 8;
            bullet.transform.localScale = Vector3.one;
            bullet.startOnBeat = true;
            bullet.enemySource = this;
            bullet.behaviours = new List<BulletBehaviour>
            {
                new SpeedOverTimeBehaviour() { speedPerBeat = 1, start = 0, end = -1, targetSpeed = 6 },
                new SpriteWaveBehaviour() { start = 0, end = -1 },
                new RotateOverBeatBehaviour() { start = 0, end = -1, rotateAmount = 90 }
            };
            bullet.animator.Play("notebullet");
            bullet.OnSpawn();
            allBullets.Add(bullet);
        }

        AudioController.PlaySound(AudioController.instance.sounds.bulletwaveShootSound);
        yield break;
    }

    void OnPattern2()
    {
        // Charge attack for 2 seconds
        if (attackBeat == 0 && isPreparingAttack)
        {
            isDancing = false;
            shouldMove = false;
            AudioController.PlaySound(AudioController.instance.sounds.bossChargeAttack);
            attackBeat = -1;
            magicCircle.color = new Color(1, 1, 1, 0);
            magicCircle.transform.localScale = Vector3.one * 1.5f;
            magicCircleVisible = true;

            StartCoroutine(ChargeAttack1Coroutine());
        }
        if (isPreparingAttack) return; // Wait until the attack is charged;

        isDancing = true;
        if (attackBeat % 18 == 0)
        {
            orbitRight = !orbitRight;

        }
        if (attackBeat % 6 == 0)
        {
            StartCoroutine(ShootBigBullets());
        }
        //StartCoroutine(ShootBulletsPhase2());
        if (attackBeat == -1) orbitRight = !orbitRight;
        attackBeat++;
    }

    private IEnumerator ShootBulletsPhase2()
    {
        for (int i = 0; i < 5; i++)
        {
            float angle = (orbitalBaseAngle + (i * 72)) * Mathf.Deg2Rad;
            Vector2 angleDir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

            BulletBase bullet = PoolManager.Get<BulletBase>();

            bullet.transform.position = transform.position + (Vector3.up) + (Vector3)(angleDir * 0.5f);
            bullet.direction = angleDir;
            bullet.speed = 8;
            bullet.atk = 10;
            bullet.lifetime = 10;
            bullet.transform.localScale = Vector3.one;
            bullet.startOnBeat = true;
            bullet.enemySource = this;
            bullet.behaviours = new List<BulletBehaviour>
            {
                new SpriteWaveBehaviour() { start = 0, end = -1 }
            };
            bullet.animator.Play(orbitRight ? "notebullet" : "rednotebullet");
            bullet.OnSpawn();
            allBullets.Add(bullet);
        }
        AudioController.PlaySoundWithoutCooldown(AudioController.instance.sounds.shootBullet);
        orbitalBaseAngle += orbitRight ? 4 : -4;

        yield break;
    }

    private IEnumerator ShootBigBullets()
    {
        for (int i = 0; i < 5; i++)
        {
            for (int i2 = 0; i2 < 3; i2++)
            {
                float angle = (orbitalBaseAngle + (i * 72f)) * Mathf.Deg2Rad;
                Vector2 angleDir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

                BulletBase bullet = PoolManager.Get<BulletBase>();

                bullet.transform.position = transform.position + (Vector3.up) + (Vector3)(angleDir * 0.3f);
                bullet.direction = angleDir;
                bullet.speed = 5;
                bullet.atk = 10;
                bullet.lifetime = 10;
                bullet.transform.localScale = Vector3.one;
                bullet.startOnBeat = true;
                bullet.enemySource = this;
                bullet.behaviours = new List<BulletBehaviour>
                {
                    new SpriteSpinBehaviour() { start = 0, end = -1 },
                    new SpeedOverTimeBehaviour() { start = 0, end = 1, speedPerBeat = 5, targetSpeed = 0},
                    new SpeedOverTimeBehaviour() { start = 2, end = 5, speedPerBeat = 1f + (i2), targetSpeed = 8f + (i2)},
                    new RotateOverBeatBehaviour() { start = 2, end = -1, rotateAmount = starOrbitRight ? -40f : 40}
                };
                bullet.animator.Play("starbullet");
                bullet.OnSpawn();
                allBullets.Add(bullet);
            }
        }
        starOrbitRight = !starOrbitRight;
        starOrbitAngle += 36;
        AudioController.PlaySound(AudioController.instance.sounds.bulletwaveShootSound);
        yield break;
    }

    void OnPattern3()
    {
        // Charge attack for 2 seconds
        if (attackBeat == 0 && isPreparingAttack)
        {
            isDancing = false;
            shouldMove = true;
            //shouldMove = false;
            AudioController.PlaySound(AudioController.instance.sounds.bossChargeAttack);
            attackBeat = -1;
            magicCircle.color = new Color(1, 1, 1, 0);
            magicCircle.transform.localScale = Vector3.one * 1.5f;
            magicCircleVisible = true;

            StartCoroutine(ChargeAttack1Coroutine());
            targetPos = Stage.Instance.bossArea.transform.position + (Vector3.up * 2.75f);
        }

        if (transform.position != targetPos)
        {
            shouldMove = true;
            isDancing = false;
        }
        else
        {
            isDancing = true;
            shouldMove = false;
        }
        if (isPreparingAttack) return; // Wait until the attack is charged;

        if (attackBeat == 0) orbitRight = false;
        if ((attackBeat - 2) % 24 == 0 && attackBeat > 2)
        {
            AudioController.PlaySound(AudioController.instance.sounds.bossChargeAttack);
            magicCircle.color = new Color(1, 1, 1, 0);
            magicCircle.transform.localScale = Vector3.one * 1.5f;
            magicCircleVisible = true;

            StartCoroutine(ChargeAttack1Coroutine(false));
        }
        if (attackBeat % 8 == 0) // Carrot Bullets
        {
            StartCoroutine(ShootSideCarrots());
        }
        if (attackBeat % 10 == 0) // Carrot Bullets
        {
            animator.Play("usarin_dance");
            StartCoroutine(ShootSpinningCarrots());
        }
        /*
        if (attackBeat % 10 == 0) FindTargetPositionAroundPlayer();
        if (attackBeat % 10 != 0 && attackBeat % 10 > 1 && attackBeat % 10 < 4)
        {
            Move();
        }
        else
        {
            animator.Play("usarin_dance");
        }*/

        attackBeat++;
    }

    private IEnumerator ShootSpinningCarrots()
    {
        List<BulletBase> bullets = new List<BulletBase>();
        for (int i = 0; i < 21; i++)
        {
            float angle = (((i * 2) - 21) + 90) * Mathf.Deg2Rad;
            Vector2 angleDir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

            BulletBase bullet = PoolManager.Get<BulletBase>();
            bullet.transform.position = transform.position + (Vector3.up * 2f);
            bullet.direction = angleDir;
            bullet.speed = 15f;
            bullet.atk = 10;
            bullet.lifetime = 30;
            bullet.transform.localScale = Vector3.one;
            bullet.startOnBeat = true;
            bullet.enemySource = this;
            bullet.behaviours = new List<BulletBehaviour>
                {
                    new SpriteSpinBehaviour() { start = 0, end = 4 },
                    new SpriteLookAngleBehaviour() { start = 5, end = -1},
                };
            bullet.animator.Play("bigcarrot");
            bullet.OnSpawn();
            allBullets.Add(bullet);
            bullets.Add(bullet);
        }

        AudioController.PlaySound(AudioController.instance.sounds.bulletwaveShootSound);

        float time = BeatManager.GetBeatDuration() * 4f;
        while (time > 0)
        {
            while (GameManager.isPaused) yield return null;
            time -= Time.deltaTime;
            yield return null;
        }
        Vector2 arenaPos = Stage.Instance.bossArea.transform.position;
        for (int i = 0; i < 21; i++)
        {
            float xoffset = i - 10;
            bullets[i].transform.position = arenaPos + (Vector2.up * 10) + (Vector2.right * xoffset);
            bullets[i].speed = 6;
            bullets[i].angle = -90 + Random.Range(-20f, 20f);
            bullets[i].direction = Vector2.down;
        }
        yield break;
    }

    private IEnumerator ShootSideCarrots()
    {
        Vector2 arenaPos = Stage.Instance.bossArea.transform.position;

        if (attackBeat % 24 == 0)
        {
            orbitRight = !orbitRight;
            string anim = orbitRight ? "bluecarrot" : "orangecarrot";
            List<BulletBase> temp = new List<BulletBase>(sideBullets);
            foreach (BulletBase bullet in temp)
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

            BulletBase bullet = PoolManager.Get<BulletBase>();
            float yoffset = (i / 3f) - 3;
            bullet.transform.position = arenaPos + ((orbitRight ? Vector2.left : Vector2.right) * 14) + (Vector2.up * yoffset * 2);
            bullet.direction = angleDir;
            bullet.speed = 4f;
            bullet.atk = 10;
            bullet.lifetime = 50;
            bullet.transform.localScale = Vector3.one;
            bullet.startOnBeat = true;
            bullet.enemySource = this;
            bullet.behaviours = new List<BulletBehaviour>
            {
                    new SpriteLookAngleBehaviour() { start = 0, end = -1 },
            };
            bullet.animator.Play(orbitRight ? "bluecarrot" : "orangecarrot");
            bullet.OnSpawn();
            allBullets.Add(bullet);
            sideBullets.Add(bullet);
        }

        AudioController.PlaySound(AudioController.instance.sounds.shootBullet);
        yield break;
    }

    private void EndAttack()
    {
        animator.Play("usarin_normal");
        StopAllCoroutines();
        rb.velocity = Vector2.zero;
        velocity = Vector2.zero;
        PlayerCamera.TriggerCameraShake(1f, 1f);

        AudioController.PlaySound(AudioController.instance.sounds.bossPhaseEnd, side:true);
        foreach (Bullet b in allBullets)
        {
            if (!b.gameObject.activeSelf) continue;
            Coin coin = PoolManager.Get<Coin>();
            coin.transform.position = b.transform.position;
            coin.followPlayer = true;
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
        StartCoroutine(JumpCoroutine());
    }
    IEnumerator MoveToTarget()
    {
        isMoving = true;

        float time = 0;
        GameObject posObj = new GameObject("objpos");
        posObj.transform.position = targetPos;
        Vector3 playerPos = targetPos;
        Vector2 dir = (playerPos - transform.position).normalized;
        facingRight = dir.x > 0;
        animator.Play("usarin_move");
        while (time <= BeatManager.GetBeatDuration() / 2)
        {
            while (GameManager.isPaused || stunStatus.isStunned()) yield return null;

            velocity = Vector2.zero;
            transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * speed * 6);
            time += Time.deltaTime;
            yield return null;
        }
        if (transform.position == targetPos) animator.Play("usarin_dance");
        else animator.Play("usarin_move");

        velocity = Vector2.zero;
        Sprite.transform.localPosition = Vector3.zero;

        isMoving = false;
        yield break;
    }

    protected override IEnumerator JumpCoroutine()
    {
        isMoving = true;

        float time = 0;
        GameObject posObj = new GameObject("objpos");
        posObj.transform.position = targetPos;
        Vector3 playerPos = targetPos;
        Vector2 dir = (playerPos - transform.position).normalized;
        facingRight = dir.x > 0;
        animator.Play("usarin_move");
        while (time <= BeatManager.GetBeatDuration() / 2)
        {
            while (GameManager.isPaused || stunStatus.isStunned()) yield return null;
            
            velocity = dir * speed * 6;
            time += Time.deltaTime;
            yield return null;
        }
        animator.Play("usarin_normal");
        AudioController.PlaySound(AudioController.instance.sounds.bossWalk);
        PlayerCamera.TriggerCameraShake(0.5f, 0.2f);
        velocity = Vector2.zero;
        Sprite.transform.localPosition = Vector3.zero;

        isMoving = false;
        yield break;
    }

    public override bool CanTakeDamage()
    {
        return true;
    }

    public override void TakeDamage(float damage, bool isCritical)
    {
        base.TakeDamage(damage, isCritical);

        if (usarinState == UsarinBossState.Dance1 && CurrentHP < hpThreshold1) EndAttack();
        else if (usarinState == UsarinBossState.Dance2 && CurrentHP < hpThreshold2) EndAttack();
        else if (usarinState == UsarinBossState.Dance3 && CurrentHP <= 0) EndAttack();
    }

    public override void Die()
    {
        foreach (Bullet b in allBullets)
        {
            b.ForceDespawn();
        }
        PlayerCamera.TriggerCameraShake(2f, 0.45f);
        base.Die();
    }

    public override string GetName()
    {
        return "Usarin";
    }
    public override  IEnumerator OnBattleStart()
    {
        // Fade off music
        UIManager.Instance.PlayerUI.OnCloseMenu();
        BeatManager.FadeOut(1);
        Vector3 target = (new Vector3(Player.instance.transform.position.x, Player.instance.transform.position.y, Camera.main.transform.position.z) + transform.position) / 2f;
        target.z = -60;
        float time = 2f;
        while (time > 0)
        {
            while (GameManager.isPaused) yield return null;
            Camera.main.transform.position = Vector3.MoveTowards(Camera.main.transform.position, target, Time.deltaTime * 2f);
            time -= Time.deltaTime;
            yield return null;
        }
        PlayerCamera.instance.SetCameraPos(target);
        PlayerCamera.instance.followPlayer = true;
        UIManager.Instance.PlayerUI.UpdateBossBar(CurrentHP, MaxHP);
        UIManager.Instance.PlayerUI.SetBossBarName(GetName());
        UIManager.Instance.PlayerUI.ShowBossBar(true);
        UIManager.Instance.PlayerUI.SetStageText($"BOSS WAVE");
        BeatManager.SetTrack(bossTrack);
        BeatManager.StartTrack();
        Stage.Instance.OnBossFightStart(this);
        Stage.isBossWave = true;
        Player.instance.canDoAnything = true;
        State = BossState.Phase1;
        usarinState = UsarinBossState.Dance1;
    }
}