using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class NebulionBoss : Boss
{
    public List<Bullet> allBullets;

    private enum NebulionBossState
    {
        Dance1, Dance2, Dance3, Dance4, Defeat
    }

    private NebulionBossState nebulionState;
    private bool isPreparingAttack;
    private int attackBeat;

    private int hpThreshold1, hpThreshold2, hpThreshold3;

    [SerializeField] SpriteRenderer magicCircle;
    private bool magicCircleVisible;

    private Vector3 targetPos;

    public GameObject constellationPrefab;
    public GameObject laserPrefab;
    public GameObject smallCirclePrefab;

    List<BulletBase> sideBullets;

    [SerializeField] Dialogue rabiDialogue;
    [SerializeField] GameObject background;

    // Pattern 1
    private bool cometRight;

    // Clones
    [SerializeField] private SpriteRenderer clone1, clone2;
    private Color cloneColor = new Color(1, 1, 1, 0);
    private float cloneDistance, cloneAngle;
    private float targetCloneDistance;

    private LongLaser laser1, laser2;
    private SpriteRenderer laser1spr, laser2spr;
    private BoxCollider2D laser1col, laser2col;
    public Animator constellation;

    public override void OnSpawn()
    {
        base.OnSpawn();
        background.transform.localScale = Vector3.zero;
        background.transform.localPosition = Vector3.zero;
        PoolManager.CreatePool(typeof(Constellation), constellationPrefab, 1);
        PoolManager.CreatePool(typeof(LongLaser), laserPrefab, 5);
        PoolManager.CreatePool(typeof(SmallMagicCircle), smallCirclePrefab, 50);
        Constellation cons = PoolManager.Get<Constellation>();
        cons.transform.position = Map.Instance.bossArea.transform.position;
        constellation = cons.GetComponent<Animator>();
        allBullets = new List<Bullet>();
        Map.Instance.enemiesAlive.Add(this);
        CurrentHP = MaxHP;
        emissionColor = new Color(1, 1, 1, 0);
        isMoving = false;
        Sprite.transform.localPosition = Vector3.zero;
        State = BossState.Introduction;// FALTA LA ANIMACION DE INTRODUCCION DEL JEFE
        nebulionState = NebulionBossState.Dance1;
        attackBeat = 0;
        isPreparingAttack = true;
        hpThreshold1 = (int)(MaxHP / 4f) * 3; // 2/3
        hpThreshold2 = (int)(MaxHP / 4f) * 2; // 1/3
        hpThreshold3 = (int)(MaxHP / 4f); // 1/3
        magicCircle.color = new Color(1, 1, 1, 0);
        magicCircle.transform.localScale = Vector3.one * 1.5f;
        magicCircleVisible = false;

        cloneDistance = 0;
        cloneAngle = 0;
        cloneColor = new Color(1, 1, 1, 0);
        clone1.color = cloneColor;
        clone2.color = cloneColor;
        cometRight = true;

        sideBullets = new List<BulletBase>();
        if (transform.position.x < Player.instance.transform.position.x)
        {
            facingRight = true;
        }
        else
        {
            facingRight = false;
        }

        animator.Play("nebulion_intro");
        animator.speed = 1;
        transform.localScale = Vector3.one * 2f;
    }

    public override void OnIntroductionFinish()
    {
        base.OnIntroductionFinish();
        animator.Play("nebulion_normal");
        animator.speed = 1f / BeatManager.GetBeatDuration();
        
        Dialogue dialogue = Player.instance is PlayerRabi ? rabiDialogue : rabiDialogue;
        UIManager.Instance.dialogueMenu.StartCutscene(dialogue.entries);

        State = BossState.Dialogue;
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
        while (GameManager.isPaused) return;
        if (State == BossState.Introduction) return;
        if (Player.instance.transform.position.x < transform.position.x) facingRight = false;
        else facingRight = true;
        if (State == BossState.Phase1)
        {
            if (nebulionState == NebulionBossState.Dance1) OnPattern1();
            if (nebulionState == NebulionBossState.Dance2) OnPattern2();
            if (nebulionState == NebulionBossState.Dance3) OnPattern3();
            if (nebulionState == NebulionBossState.Dance4) OnPattern4();

            StartCoroutine(ClonePositionCoroutine());
        }
    }
    
    protected override void OnBehaviourUpdate()
    {
        while (GameManager.isPaused) return;
        switch (State)
        {
            case BossState.Dialogue:
                if (UIManager.Instance.dialogueMenu.hasFinished)
                {
                    Debug.Log("it was finished");
                    State = BossState.Phase4;
                    StartCoroutine(OnBattleStart());
                    UIManager.Instance.dialogueMenu.hasFinished = false;
                }
                break;
        }

        if (magicCircleVisible)
        {
            magicCircle.color = Color.Lerp(magicCircle.color, Color.white, Time.deltaTime);
            magicCircle.transform.localScale = Vector3.Lerp(magicCircle.transform.localScale, Vector3.one, Time.deltaTime);
            background.transform.localScale = Vector3.Lerp(background.transform.localScale, Vector3.one * 30f, Time.deltaTime / 2f);
        }
        else
        {
            magicCircle.color = Color.Lerp(magicCircle.color, new Color(1,1,1,0), Time.deltaTime);
        }
        magicCircle.transform.localEulerAngles = new Vector3(45f, 0, magicCircle.transform.localEulerAngles.z + (Time.deltaTime * 50f));

        if (nebulionState == NebulionBossState.Dance2)
        {
            cloneColor.a = Mathf.MoveTowards(cloneColor.a, 0.3f, Time.deltaTime * 8f);
            clone1.color = cloneColor;
            clone2.color = cloneColor;
            clone1.sprite = Sprite.sprite;
            clone2.sprite = Sprite.sprite;

            bool laser1forward = cloneAngle % 360 >= 0 && cloneAngle % 360 < 180;
            bool laser2forward = !laser1forward;

            Color laserForward = Color.white;
            Color laserBackward = new Color(0, 0.2f, 0.7f, 0.3f);
            if (laser1spr != null)
            {
                laser1spr.color = Color.Lerp(laser1spr.color, laser1forward ? laserForward : laserBackward, Time.deltaTime * 4f);
                laser2spr.color = Color.Lerp(laser2spr.color, laser2forward ? laserForward : laserBackward, Time.deltaTime * 4f);

                if (laser1forward)
                {
                    laser1spr.sortingOrder = 4;
                    laser2spr.sortingOrder = 2;
                    laser1.transform.localEulerAngles = new Vector3(laser1.transform.localEulerAngles.x, laser1.transform.localEulerAngles.y, Mathf.MoveTowardsAngle(laser1.transform.localEulerAngles.z, 300f, Time.deltaTime * 24));
                    laser2.transform.localEulerAngles = new Vector3(laser2.transform.localEulerAngles.x, laser2.transform.localEulerAngles.y, Mathf.MoveTowardsAngle(laser2.transform.localEulerAngles.z, 240f, Time.deltaTime * 24));
                    laser1col.enabled = true;
                    laser2col.enabled = false;
                }
                else
                {
                    laser1spr.sortingOrder = 2;
                    laser2spr.sortingOrder = 4;
                    laser1col.enabled = false;
                    laser2col.enabled = true;
                    laser1.transform.localEulerAngles = new Vector3(laser1.transform.localEulerAngles.x, laser1.transform.localEulerAngles.y, Mathf.MoveTowardsAngle(laser1.transform.localEulerAngles.z, 240f, Time.deltaTime * 24));
                    laser2.transform.localEulerAngles = new Vector3(laser2.transform.localEulerAngles.x, laser2.transform.localEulerAngles.y, Mathf.MoveTowardsAngle(laser2.transform.localEulerAngles.z, 300f, Time.deltaTime * 24));
                }
            }

        }
        else
        {
            cloneColor.a = Mathf.MoveTowards(cloneColor.a, 0f, Time.deltaTime * 8f);
            clone1.color = cloneColor;
            clone2.color = cloneColor;
        }
    }

    IEnumerator ClonePositionCoroutine()
    {

        float beatDuration = BeatManager.GetBeatDuration();
        float beatTime = 1;

        float time = 0;
        while (time <= beatDuration)
        {
            while (GameManager.isPaused || stunStatus.isStunned())//|| stunStatus.isStunned())
            {
                yield return new WaitForEndOfFrame();
            }
            float beatProgress = time / beatDuration;
            beatTime = Mathf.Lerp(1, 0f, beatProgress);
            cloneDistance = Mathf.MoveTowards(cloneDistance, targetCloneDistance, Time.deltaTime * 4f * beatTime);
            cloneAngle += 70f * beatTime * Time.deltaTime;
            float clone2Angle = cloneAngle + 180f;
            clone1.transform.position = new Vector3(transform.position.x + ((cloneDistance * 2f) * Mathf.Cos(cloneAngle * Mathf.Deg2Rad)), transform.position.y + ((cloneDistance) * Mathf.Sin(cloneAngle * Mathf.Deg2Rad)), 10);
            clone2.transform.position = new Vector3(transform.position.x + ((cloneDistance * 2f) * Mathf.Cos(clone2Angle * Mathf.Deg2Rad)), transform.position.y + ((cloneDistance) * Mathf.Sin(clone2Angle * Mathf.Deg2Rad)), 10);
            //transform.position += ((Vector3)direction * speed * beatTime * Time.deltaTime);
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        yield break;
    }


    private IEnumerator ChargeAttack1Coroutine(bool reset = true)
    {
        magicCircle.transform.localScale = Vector3.one * 1.5f;
        float time = BeatManager.GetBeatDuration() * (reset ? 3f : 2f);
        while (time > 0)
        {
            while (GameManager.isPaused) yield return new WaitForEndOfFrame();
            time -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        while (!BeatManager.isGameBeat) yield return new WaitForEndOfFrame();

        isPreparingAttack = false;
        if (reset) attackBeat = 0;
        animator.Play("usarin_dance");
        yield break;
    }
    public void OnPattern1()
    {
        if (attackBeat == 0 && isPreparingAttack)
        {
            AudioController.PlaySound(AudioController.instance.sounds.bossChargeAttack);
            attackBeat = -1;
            magicCircle.color = new Color(1, 1, 1, 0);
            magicCircle.transform.localScale = Vector3.one * 1.5f;
            magicCircleVisible = true;
            animator.Play("nebulion_dance");
            StartCoroutine(ChargeAttack1Coroutine());
        }
        if (isPreparingAttack) return;

        if (attackBeat <= 4)
        {
            SpawnComet();
            AudioController.PlaySound(AudioController.instance.sounds.bulletwaveShootSound);
        }

        if (attackBeat == 0)
        {
            animator.Play("nebulion_dance");
        }
        if (attackBeat == 4)
        {
            FindTargetPositionAroundPlayer();
        }

        if (attackBeat > 4)
        {
            Move();
            if (attackBeat == 6) AudioController.PlaySound(AudioController.instance.sounds.chargeBulletSound);
            if (attackBeat == 8)
            {
                attackBeat = -1;
                cometRight = !cometRight;
            }
        }
        attackBeat++;
    }

    private void SpawnComet()
    {
        int xposition = 0;
        switch (attackBeat)
        {
            case 0:
                xposition = -10;
                break;
            case 1:
                xposition = 0;
                break;
            case 2:
                xposition = -5;
                break;
            case 3:
                xposition = -13;
                break;
            case 4:
                xposition = 4;
                break;
        }
        if (!cometRight) xposition += 6;
        else xposition += 2;
        BulletBase bullet = PoolManager.Get<BulletBase>();
        Vector2 areaPos = Vector2.zero; // This should be the arena
        bullet.transform.position = new Vector3(xposition + areaPos.x, 10f);

        bullet.direction = new Vector2(cometRight ? Random.Range(0.1f, 0.3f) : Random.Range(-0.1f, -0.3f), -1f);
        bullet.speed = 14;
        bullet.atk = 3;
        bullet.lifetime = 12;
        bullet.transform.localScale = Vector3.one;
        bullet.startOnBeat = true;
        bullet.enemySource = this;
        bullet.behaviours = new List<BulletBehaviour>
            {
                new SpriteLookAngleBehaviour() { start = 0, end = -1 },
                new SpawnBulletOnBeatBehaviour(SpawnCometStar) { start = 0, end = -1}
            };
        bullet.OnSpawn();
        allBullets.Add(bullet);
        if (!allBullets.Contains(bullet)) allBullets.Add(bullet);


        bullet.animator.Play("cometbullet");
    }

    public void SpawnCometStar(BulletBase comet)
    {
        BulletBase bullet = PoolManager.Get<BulletBase>();
        Vector2 areaPos = Vector2.zero; // This should be the arena
        bullet.transform.position = comet.transform.position;

        bullet.direction = (comet.direction + new Vector2(Random.Range(-0.3f, 0.3f), 0)).normalized;
        bullet.speed = 3;
        bullet.atk = 1;
        bullet.lifetime = 6;
        bullet.transform.localScale = Vector3.one;
        bullet.startOnBeat = true;
        bullet.enemySource = this;
        bullet.behaviours = new List<BulletBehaviour>
            {
                new SpriteSpinBehaviour() { start = 0, end = -1 },
            };
        bullet.OnSpawn();
        allBullets.Add(bullet);
        if (!allBullets.Contains(bullet)) allBullets.Add(bullet);
        bullet.animator.Play("starbullet");
    }

    public void SpawnBlackHoleExplosion(BulletBase blackhole)
    {
        if (isPreparingAttack || nebulionState == NebulionBossState.Defeat) return;
        for (int i = 0; i < 8; i++)
        {
            float angle = (360f / 8f) * i;
            BulletBase bullet = PoolManager.Get<BulletBase>();
            Vector2 areaPos = Vector2.zero; // This should be the arena
            Vector2 localPos = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * 0.3f;
            bullet.transform.position = blackhole.transform.position + (Vector3)localPos;

            bullet.direction = localPos.normalized;
            bullet.speed = 2f;
            bullet.atk = 1;
            bullet.lifetime = 20;
            bullet.enemySource = this;
            bullet.transform.localScale = Vector3.one;
            bullet.startOnBeat = true;
            bullet.behaviours = new List<BulletBehaviour>
            {
                new SpriteSpinBehaviour() { start = 0, end = -1 },
            };
            bullet.OnSpawn();
            allBullets.Add(bullet);
            if (!allBullets.Contains(bullet)) allBullets.Add(bullet);
            bullet.animator.Play("starbullet");
        }
        AudioController.PlaySound(AudioController.instance.sounds.bulletwaveShootSound);
    }

    public void SmallBlackHoleExplosion(BulletBase blackhole)
    {
        if (isPreparingAttack || nebulionState == NebulionBossState.Defeat) return;
        for (int i = 0; i < 5; i++)
        {
            float angle = (360f / 5f) * i;
            BulletBase bullet = PoolManager.Get<BulletBase>();
            Vector2 areaPos = Vector2.zero; // This should be the arena
            Vector2 localPos = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * 0.3f;
            bullet.transform.position = blackhole.transform.position + (Vector3)localPos;

            bullet.direction = localPos.normalized;
            bullet.speed = 1.5f;
            bullet.atk = 1;
            bullet.lifetime = 20;
            bullet.transform.localScale = Vector3.one;
            bullet.startOnBeat = true;
            bullet.enemySource = this;
            bullet.behaviours = new List<BulletBehaviour>
            {
                new SpriteSpinBehaviour() { start = 0, end = -1 },
            };
            bullet.OnSpawn();
            allBullets.Add(bullet);
            if (!allBullets.Contains(bullet)) allBullets.Add(bullet);
            bullet.animator.Play("starbullet");
        }
        AudioController.PlaySound(AudioController.instance.sounds.bulletwaveShootSound);
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
            targetCloneDistance = 3f;
            StartCoroutine(ChargeAttack1Coroutine());
            targetPos = Map.Instance.bossArea.transform.position + (Vector3.up * 6f);
        }

        if (transform.position != targetPos)
        {
            StartCoroutine(MoveToTarget());
        }
        if (isPreparingAttack) return; // Wait until the attack is charged;

        if (attackBeat == 0)
        {
            animator.Play("nebulion_dance");
            StartCoroutine(SpawnLasers());
        }
        if ((attackBeat - 2) % 12 == 0)
        {
            AudioController.PlaySound(AudioController.instance.sounds.chargeBulletSound);
        }

        if (attackBeat % 12 == 0)
        {
            ShootBlackHoleToPlayer(transform.position);

        }
        if (attackBeat == -1) cometRight = !cometRight;
        attackBeat++;
    }

    private IEnumerator SpawnLasers()
    {
        LongLaser laser1 = PoolManager.Get<LongLaser>();
        laser1.transform.eulerAngles = new Vector3(0, 0, 265);
        laser1.transform.parent = clone1.transform;
        laser1.transform.localPosition = new Vector3(0, -0.2f, 0);
        this.laser1 = laser1;
        laser1.animator.Play("laser_spawn");
        laser1.atk = 10;

        LongLaser laser2 = PoolManager.Get<LongLaser>();
        laser2.transform.eulerAngles = new Vector3(0, 0, 275);
        laser2.transform.parent = clone2.transform;
        laser2.transform.localPosition = new Vector3(0, -0.2f, 0);
        this.laser2 = laser2;
        laser2.animator.Play("laser_spawn");
        laser2.atk = 10;

        AudioController.PlaySound(AudioController.instance.sounds.chargeBulletSound);
        laser1spr = laser1.GetComponent<SpriteRenderer>();
        laser2spr = laser2.GetComponent<SpriteRenderer>();
        laser1col = laser1.GetComponent<BoxCollider2D>();
        laser2col = laser2.GetComponent<BoxCollider2D>();
        yield break;
    }

    private void ShootBlackHoleToPlayer(Vector3 origin)
    {
        BulletBase bullet = PoolManager.Get<BulletBase>();

        bullet.transform.position = origin + (-Vector3.up * 0.2f);
        Vector3 playerPos = Player.instance.transform.position;
        Vector2 dir = (playerPos - origin).normalized;
        bullet.direction = dir;
        bullet.speed = 3;
        bullet.atk = 4;
        bullet.lifetime = 10;
        bullet.transform.localScale = Vector3.one;
        bullet.startOnBeat = true;
        bullet.enemySource = this;
        bullet.behaviours = new List<BulletBehaviour>
            {
                new HomingToPlayerBehaviour(Player.instance.gameObject) { start = 0, end = -1},
                new BlackHoleBehaviour() {start = 0, end = -1},
                new BehaviourOnDespawn(SpawnBlackHoleExplosion) { start = 0, end = -1 }
            };
        bullet.animator.Play("blackholebullet");
        bullet.OnSpawn();
        allBullets.Add(bullet);

        AudioController.PlaySound(AudioController.instance.sounds.bulletwaveShootSound);
    }

    void OnPattern3()
    {
        if (attackBeat == 0 && isPreparingAttack)
        {
            AudioController.PlaySound(AudioController.instance.sounds.bossChargeAttack);
            attackBeat = -1;
            magicCircle.color = new Color(1, 1, 1, 0);
            magicCircle.transform.localScale = Vector3.one * 1.5f;
            magicCircleVisible = true;
            animator.Play("nebulion_dance");
            StartCoroutine(ChargeAttack1Coroutine());
        }
        if (isPreparingAttack) return;

        targetCloneDistance = 0f;
        if (attackBeat == 0)
        {
            animator.Play("nebulion_dance");
            FindTargetPositionAroundPlayer();
            constellation.Play("taurus");
            constellation.speed = 1f / BeatManager.GetBeatDuration() / 2f;
        }
        else if (attackBeat == 12)
        {
            animator.Play("nebulion_dance");
            FindTargetPositionAroundPlayer();
            constellation.Play("libra");
            constellation.speed = 1f / BeatManager.GetBeatDuration() / 2f;
        }
        else if (attackBeat == 24)
        {
            animator.Play("nebulion_dance");
            FindTargetPositionAroundPlayer();
            constellation.Play("virgo");
            constellation.speed = 1f / BeatManager.GetBeatDuration() / 2f;
        }
        else
        {
            Move();
        }
        StartCoroutine(ConstellationRotate());

        if (attackBeat % 8 == 0 && attackBeat > 0) // Comet
        {
            StartCoroutine(MagicCometCoroutine());
        }
        if (attackBeat == 33)
        {
            attackBeat = -1;
        }

        if (attackBeat % 10 == 0) // Carrot Bullets
        {
            //StartCoroutine(ShootSpinningCarrots());
        }

        attackBeat++;
    }

    private IEnumerator ConstellationRotate()
    {
        float time = 0;
        while (time <= BeatManager.GetBeatDuration() * 2f)
        {
            while (GameManager.isPaused || stunStatus.isStunned()) yield return new WaitForEndOfFrame(); // isStunned!
            time += Time.deltaTime;
            constellation.transform.localEulerAngles = new Vector3(0, 0, constellation.transform.localEulerAngles.z + (3f * Time.deltaTime));
            yield return new WaitForEndOfFrame();
        }
    }
    private IEnumerator MagicCometCoroutine()
    {
        Vector3 circlePos = GetMagicCircleSpawnSpot(Vector3.zero);
        Vector3 circle2Pos = GetMagicCircleSpawnSpot(circlePos);

        SmallMagicCircle circle1 = PoolManager.Get<SmallMagicCircle>();
        circle1.transform.position = circlePos;
        circle1.despawnTime = 4;

        SmallMagicCircle circle2 = PoolManager.Get<SmallMagicCircle>();
        circle2.transform.position = circle2Pos;
        circle2.despawnTime = 4;

        AudioController.PlaySound(AudioController.instance.sounds.chargeBulletSound);

        float time = 0;
        while (time <= BeatManager.GetBeatDuration() * 4f)
        {
            while (GameManager.isPaused || stunStatus.isStunned()) yield return new WaitForEndOfFrame(); // isStunned!
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        while (!BeatManager.isGameBeat) yield return new WaitForEndOfFrame();

        AudioController.PlaySound(AudioController.instance.sounds.bulletwaveShootSound);
        SpawnMagicComet(circlePos);
        SpawnMagicComet(circle2Pos);
    }

    private Vector3 GetMagicCircleSpawnSpot(Vector3 other)
    {
        Vector3 playerPos = Player.instance.transform.position;
        float playerDis = 4;
        float magicDis = 2.5f;

        Vector3 finalPos = Vector3.zero;
        int chances = 25;
        while (finalPos == Vector3.zero)
        {
            Vector3 circlePos = Map.Instance.bossArea.transform.position + (Vector3)((Random.insideUnitCircle * 8f) * new Vector2(1f, 0.5f));
            if (other != Vector3.zero) // Beware the other magic circle
            {
                if (Vector2.Distance(circlePos, other) > magicDis && Vector2.Distance(circlePos, playerPos) > playerDis) finalPos = circlePos;
                else chances--;
            }
            else
            {
                if (Vector2.Distance(circlePos, playerPos) > playerDis) finalPos = circlePos;
                else chances--;
            }
            if (chances <= 0) return circlePos;
        }
        return Map.Instance.bossArea.transform.position + (Vector3)((Random.insideUnitCircle * 8f) * new Vector2(1f, 0.5f));
    }

    private void SpawnMagicComet(Vector3 pos)
    {
        BulletBase bullet = PoolManager.Get<BulletBase>();
        Vector2 areaPos = Vector2.zero; // This should be the arena
        bullet.transform.position = pos;

        Vector3 playerPos = Player.instance.transform.position;
        Vector2 dir = (playerPos - pos).normalized;

        bullet.direction = dir;
        bullet.speed = 14;
        bullet.atk = 3;
        bullet.lifetime = 12;
        bullet.transform.localScale = Vector3.one;
        bullet.startOnBeat = true;
        bullet.enemySource = this;
        bullet.behaviours = new List<BulletBehaviour>
            {
                new SpriteLookAngleBehaviour() { start = 0, end = -1 },
                new SpawnBulletOnBeatBehaviour(SpawnCometStar) { start = 0, end = -1}
            };
        bullet.OnSpawn();
        allBullets.Add(bullet);
        if (!allBullets.Contains(bullet)) allBullets.Add(bullet);


        bullet.animator.Play("cometbullet");
    }

    void OnPattern4()
    {
        // Charge attack for 2 seconds
        if (attackBeat == 0 && isPreparingAttack)
        {
            AudioController.PlaySound(AudioController.instance.sounds.bossChargeAttack);
            attackBeat = -1;
            magicCircle.color = new Color(1, 1, 1, 0);
            magicCircle.transform.localScale = Vector3.one * 1.5f;
            magicCircleVisible = true;
            targetCloneDistance = 0f;
            StartCoroutine(ChargeAttack1Coroutine());
        }

        if (isPreparingAttack) return; // Wait until the attack is charged;

        if (attackBeat == 0)
        {
            animator.Play("nebulion_dance");
            FindTargetPositionAroundPlayer();
            StartCoroutine(SpawnBlackHoleArray());
        }
        else if (attackBeat < 10)
        {
            Move();
        }
        /*
        if (attackBeat % 8 == 0)
        {
            ShootBlackHoleToPlayer(transform.position);

        }*/
        if (attackBeat == 10)
        {
            animator.Play("nebulion_dance");
            StartCoroutine(ShootLaserAttack());
        }
        if (attackBeat == 14)
        {
            attackBeat = -1;
        }
        if (attackBeat % 6 == 0)
        {
            //StartCoroutine(ShootBigBullets());
        }
        //StartCoroutine(ShootBulletsPhase2());
        if (attackBeat == -1) cometRight = !cometRight;
        attackBeat++;
    }

    private IEnumerator SpawnBlackHoleArray()
    {
        List<Vector3> positions = new List<Vector3>();

        for (int y = 0; y < 6; y++)
        {
            for (int x = 0; x < 6; x++)
            {
                float basex = (x * 4);
                float basey = -(y * 4);
                if (y % 2 == 1 && cometRight)
                {
                    basex += 2;
                }
                if (y % 2 == 0 && !cometRight)
                {
                    basex += 2;
                }
                if (!cometRight) basey += 2;
                Vector3 pos = new Vector3(basex, basey) - new Vector3(10, -12f);
                positions.Add(pos + Map.Instance.bossArea.transform.position);
            }
        }
        AudioController.PlaySound(AudioController.instance.sounds.chargeBulletSound);
        for (int i = 0; i < positions.Count; i++)
        {
            SmallMagicCircle circle1 = PoolManager.Get<SmallMagicCircle>();
            circle1.transform.position = positions[i];
            circle1.despawnTime = 6;
            yield return new WaitForSeconds(BeatManager.GetBeatDuration() / 16f);
        }
        AudioController.PlaySound(AudioController.instance.sounds.bulletwaveShootSound);
        for (int i = 0; i < positions.Count; i++)
        {
            SpawnArrayBlackHole(positions[i]);
            yield return new WaitForSeconds(BeatManager.GetBeatDuration() / 16f);
        }

        yield break;
    }

    private void SpawnArrayBlackHole(Vector3 origin)
    {
        BulletBase bullet = PoolManager.Get<BulletBase>();

        bullet.transform.position = origin + (-Vector3.up * 0.2f);
        Vector3 playerPos = Player.instance.transform.position;
        Vector2 dir = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        bullet.direction = dir;
        bullet.speed = 0;
        bullet.atk = 4;
        bullet.lifetime = 10;
        bullet.transform.localScale = Vector3.one;
        bullet.startOnBeat = false;
        bullet.enemySource = this;
        bullet.behaviours = new List<BulletBehaviour>
            {
                new SpeedOverTimeBehaviour() {start = 0, end = -1, speedPerBeat = 0.25f, targetSpeed = 1f },
                new BlackHoleBehaviour() {start = 0, end = -1},
                new BehaviourOnDespawn(SmallBlackHoleExplosion) { start = 0, end = -1 }
            };
        bullet.animator.Play("blackholebullet");
        bullet.OnSpawn();
        allBullets.Add(bullet);
    }

    private IEnumerator ShootLaserAttack()
    {
        LongLaser laser = PoolManager.Get<LongLaser>();

        laser.transform.parent = transform;
        laser.transform.localPosition = new Vector3(0, -0.2f, 0);
        laser.GetComponent<SpriteRenderer>().color = Color.white;
        laser.animator.Play("laser_spawn");
        laser.atk = 10;

        Vector3 playerPos = Player.instance.transform.position;
        Vector2 dir = (playerPos - laser.transform.position).normalized;
        float angle = Vector2.SignedAngle(Vector2.down, dir) - 90;

        laser.transform.eulerAngles = new Vector3(0, 0, angle);

        AudioController.PlaySound(AudioController.instance.sounds.chargeBulletSound);

        yield return new WaitForSeconds(BeatManager.GetBeatDuration() * 6f);

        laser.Despawn();

        yield break;
    }

    private void EndAttack()
    {
        animator.Play("nebulion_normal");
        StopAllCoroutines();
        rb.velocity = Vector2.zero;
        velocity = Vector2.zero;
        PlayerCamera.TriggerCameraShake(1f, 1f);

        AudioController.PlaySound(AudioController.instance.sounds.bossPhaseEnd, side: true);
        foreach (Bullet b in allBullets)
        {
            if (!b.gameObject.activeSelf) continue;
            Coin coin = PoolManager.Get<Coin>();
            coin.transform.position = b.transform.position;
            coin.followPlayer = true;
            if (b.gameObject.activeSelf) b.ForceDespawn();
        }
        allBullets.Clear();
        if (laser1 != null)
        {
            laser1.Despawn();
            laser2.Despawn();
        }
        isPreparingAttack = true;
        attackBeat = 0;

        if (nebulionState == NebulionBossState.Dance1) 
        {
            nebulionState = NebulionBossState.Dance2;
        }
        else if (nebulionState == NebulionBossState.Dance2)
        {
            nebulionState = NebulionBossState.Dance3;
        }
        else if (nebulionState == NebulionBossState.Dance3)
        {
            constellation.GetComponent<Constellation>().OnDespawn();
            nebulionState = NebulionBossState.Dance4;
        }
        else
        {
            nebulionState = NebulionBossState.Defeat;
            State = BossState.Defeat;
            PoolManager.RemovePool(typeof(Constellation));
            PoolManager.RemovePool(typeof(LongLaser));
            PoolManager.RemovePool(typeof(SmallMagicCircle));
        }
    }

    protected override void OnInitialize()
    {

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
        animator.Play("nebulion_move");
        while (time <= BeatManager.GetBeatDuration() / 2)
        {
            while (GameManager.isPaused || stunStatus.isStunned()) yield return new WaitForEndOfFrame();
            
            velocity = dir * speed * 6;
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        animator.Play("nebulion_move");
        velocity = Vector2.zero;
        Sprite.transform.localPosition = Vector3.zero;

        isMoving = false;
        yield break;
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
        animator.Play("nebulion_move");
        while (time <= BeatManager.GetBeatDuration() / 2)
        {
            while (GameManager.isPaused || stunStatus.isStunned()) yield return new WaitForEndOfFrame();

            velocity = Vector2.zero;
            transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * speed * 6);
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        if (transform.position == targetPos) animator.Play("nebulion_dance");
        else animator.Play("nebulion_move");

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

        if (nebulionState == NebulionBossState.Dance1 && CurrentHP < hpThreshold1) EndAttack();
        else if (nebulionState == NebulionBossState.Dance2 && CurrentHP < hpThreshold2) EndAttack();
        else if (nebulionState == NebulionBossState.Dance3 && CurrentHP < hpThreshold3) EndAttack();
        else if (nebulionState == NebulionBossState.Dance4 && CurrentHP <= 0) EndAttack();
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
        return "Nebulion";
    }
    public override  IEnumerator OnBattleStart()
    {
        // Fade off music

        BeatManager.FadeOut(1);
        Vector3 target = (new Vector3(Player.instance.transform.position.x, Player.instance.transform.position.y, Camera.main.transform.position.z) + transform.position) / 2f;
        target.z = -60;
        float time = 2f;
        while (time > 0)
        {
            while (GameManager.isPaused) yield return new WaitForEndOfFrame();
            Camera.main.transform.position = Vector3.MoveTowards(Camera.main.transform.position, target, Time.deltaTime * 2f);
            time -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        PlayerCamera.instance.SetCameraPos(target);
        PlayerCamera.instance.followPlayer = true;
        UIManager.Instance.PlayerUI.UpdateBossBar(CurrentHP, MaxHP);
        UIManager.Instance.PlayerUI.SetBossBarName(GetName());
        UIManager.Instance.PlayerUI.SetStageText($"{Localization.GetLocalizedString("playerui.stageboss")}");
        BeatManager.SetTrack(bossTrack);
        BeatManager.StartTrack();
        Map.isBossWave = true;
        Player.instance.canDoAnything = true;
        State = BossState.Phase1;
        nebulionState = NebulionBossState.Dance1;
    }
}