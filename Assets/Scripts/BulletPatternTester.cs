using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UIElements;

public class BulletPatternTester : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] MapTrack track;

    [SerializeField] GameObject laserPrefab;
    [SerializeField] GameObject baseBulletPrefab;
    [SerializeField] GameObject smallMagicCirclePrefab;

    [SerializeField] GameObject player;

    [SerializeField] int pattern;
    [SerializeField] int attackBeat;

    List<Enemy> allEnemies;
    List<Bullet> allBullets;
    List<BulletBase> sideBullets;
    private bool isPreparingAttack = true;

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

    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        cloneDistance = 0;
        cloneAngle = 0;
        cloneColor = new Color(1, 1, 1, 0);
        clone1.color = cloneColor;
        clone2.color = cloneColor;
        StartPools();
        BeatManager.SetTrack(track);
        BeatManager.StartTrack();
        BeatManager.compassless = false;
        cometRight = true;
        allEnemies = new List<Enemy>();
        allBullets = new List<Bullet>();
        sideBullets = new List<BulletBase>();
        constellation.Play("none");
        
    }

    private void StartPools()
    {
        PoolManager.CreatePool(typeof(LongLaser), laserPrefab, 6);
        PoolManager.CreatePool(typeof(SmallMagicCircle), smallMagicCirclePrefab, 20);
        PoolManager.CreatePool(typeof(BulletBase), baseBulletPrefab, 450);
    }

    // Update is called once per frame
    void Update()
    {
        if (BeatManager.isBeat)
        {
            switch (pattern)
            {
                case 1:
                    OnPattern1();
                    break;
                case 2: 
                    OnPattern2();
                    StartCoroutine(ClonePositionCoroutine());
                    break;
                case 3:
                    OnPattern3();
                    StartCoroutine(ClonePositionCoroutine());
                    break;
                case 4:
                    OnPattern4();
                    StartCoroutine(ClonePositionCoroutine());
                    break;
                case 5:
                    OnEnemyPattern();
                    break;
            }
        }
        if (pattern == 2 || pattern == 3)
        {
            cloneColor.a = Mathf.MoveTowards(cloneColor.a, 0.3f, Time.deltaTime * 8f);
            clone1.color = cloneColor;
            clone2.color = cloneColor;
            clone1.sprite = spriteRenderer.sprite;
            clone2.sprite = spriteRenderer.sprite;

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
                    laser1.transform.localEulerAngles = new Vector3(laser1.transform.localEulerAngles.x, laser1.transform.localEulerAngles.y, Mathf.MoveTowardsAngle(laser1.transform.localEulerAngles.z, 300f, Time.deltaTime * 32));
                    laser2.transform.localEulerAngles = new Vector3(laser2.transform.localEulerAngles.x, laser2.transform.localEulerAngles.y, Mathf.MoveTowardsAngle(laser2.transform.localEulerAngles.z, 240f, Time.deltaTime * 32));
                    laser1col.enabled = true;
                    laser2col.enabled = false;
                }
                else
                {
                    laser1spr.sortingOrder = 2;
                    laser2spr.sortingOrder = 4;
                    laser1col.enabled = false;
                    laser2col.enabled = true;
                    laser1.transform.localEulerAngles = new Vector3(laser1.transform.localEulerAngles.x, laser1.transform.localEulerAngles.y, Mathf.MoveTowardsAngle(laser1.transform.localEulerAngles.z, 240f, Time.deltaTime * 32));
                    laser2.transform.localEulerAngles = new Vector3(laser2.transform.localEulerAngles.x, laser2.transform.localEulerAngles.y, Mathf.MoveTowardsAngle(laser2.transform.localEulerAngles.z, 300f, Time.deltaTime * 32));
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
            while (GameManager.isPaused)//|| stunStatus.isStunned())
            {
                yield return new WaitForEndOfFrame();
            }
            float beatProgress = time / beatDuration;
            beatTime = Mathf.Lerp(1, 0f, beatProgress);
            cloneDistance = Mathf.MoveTowards(cloneDistance, targetCloneDistance, Time.deltaTime * 4f * beatTime);
            cloneAngle += 120f * beatTime * Time.deltaTime;
            float clone2Angle = cloneAngle + 180f;
            clone1.transform.position = new Vector3(transform.position.x + ((cloneDistance * 2f) * Mathf.Cos(cloneAngle * Mathf.Deg2Rad)), transform.position.y + ((cloneDistance/2f) * Mathf.Sin(cloneAngle * Mathf.Deg2Rad)), 10);
            clone2.transform.position = new Vector3(transform.position.x + ((cloneDistance * 2f) * Mathf.Cos(clone2Angle * Mathf.Deg2Rad)), transform.position.y + ((cloneDistance / 2f) * Mathf.Sin(clone2Angle * Mathf.Deg2Rad)), 10);
            //transform.position += ((Vector3)direction * speed * beatTime * Time.deltaTime);
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        yield break;
    }

    void OnPattern1()
    {
        // Falling commets \ \ \
        //                  o o o

        if (attackBeat <= 4)
        {
            SpawnComet();
            AudioController.PlaySound(AudioController.instance.sounds.bulletwaveShootSound);
        }

        if (attackBeat >= 2 && attackBeat <= 6)
        {
            //StartCoroutine(ShootBulletsPhase1());
        }
        if (attackBeat == 0) // Bunnies
        {
            //StartCoroutine(ShootSecondCarrots());
        }

        if (attackBeat > 4)
        {
            if (attackBeat == 6) AudioController.PlaySound(AudioController.instance.sounds.chargeBulletSound);
            if (attackBeat == 8)
            {
                attackBeat = -1;
                cometRight = !cometRight;
            }
            
            //Move();

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
        bullet.transform.position = new Vector3(xposition + areaPos.x, 12f);
        
        bullet.direction = new Vector2(cometRight ? Random.Range(0.1f, 0.3f) : Random.Range(-0.1f, -0.3f), -1f);
        bullet.speed = 14;
        bullet.atk = 3;
        bullet.lifetime = 12;
        bullet.transform.localScale = Vector3.one;
        bullet.startOnBeat = true;
        bullet.behaviours = new List<BulletBehaviour>
            {
                new SpriteLookAngleBehaviour() { start = 0, end = -1 },
                new SpawnBulletOnBeatBehaviour(SpawnCometStar, BeatManager.GetBeatDuration() / 3) { start = 0, end = -1}
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
        for (int i = 0; i < 5; i++)
        {
            float angle = (360f / 5f) * i;
            BulletBase bullet = PoolManager.Get<BulletBase>();
            Vector2 areaPos = Vector2.zero; // This should be the arena
            Vector2 localPos = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * 0.3f;
            bullet.transform.position = blackhole.transform.position + (Vector3)localPos;

            bullet.direction = localPos.normalized;
            bullet.speed = 0.5f;
            bullet.atk = 1;
            bullet.lifetime = 20;
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

    void OnPattern2()
    {
        // Charge attack for 2 seconds
        if (attackBeat == 0 && isPreparingAttack)
        {
            AudioController.PlaySound(AudioController.instance.sounds.bossChargeAttack);
            attackBeat = -1;
            //magicCircle.color = new Color(1, 1, 1, 0);
            //magicCircle.transform.localScale = Vector3.one * 1.5f;
            //magicCircleVisible = true;
            targetCloneDistance = 3f;
            StartCoroutine(ChargeAttack1Coroutine());
        }
        
        

        if (isPreparingAttack) return; // Wait until the attack is charged;

        if (attackBeat == 0)
        {
            StartCoroutine(SpawnLasers());
        }

        if (attackBeat % 8 == 0)
        {
            ShootBlackHoleToPlayer(transform.position);

        }
        if (attackBeat % 6 == 0)
        {
            //StartCoroutine(ShootBigBullets());
        }
        //StartCoroutine(ShootBulletsPhase2());
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
        laser1.atk = 10;
        laser1.animator.Play("laser_spawn");

        LongLaser laser2 = PoolManager.Get<LongLaser>();
        laser2.transform.eulerAngles = new Vector3(0, 0, 275);
        laser2.transform.parent = clone2.transform;
        laser2.transform.localPosition = new Vector3(0, -0.2f, 0);
        this.laser2 = laser2;
        laser2.atk = 10;
        laser2.animator.Play("laser_spawn");

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
        Vector3 playerPos = player.transform.position;//Player.instance.transform.position;
        Vector2 dir = (playerPos - origin).normalized;
        bullet.direction = dir;
        bullet.speed = 4;
        bullet.atk = 4;
        bullet.lifetime = 9;
        bullet.transform.localScale = Vector3.one;
        bullet.startOnBeat = true;
        bullet.behaviours = new List<BulletBehaviour>
            {
                new HomingToPlayerBehaviour(player) { start = 0, end = -1},
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
        targetCloneDistance = 3f;
        if (attackBeat == 0)
        {
            constellation.Play("taurus");
            constellation.speed = 1f / BeatManager.GetBeatDuration() / 2f;
        }

        if (attackBeat == 12)
        {
            constellation.Play("libra");
            constellation.speed = 1f / BeatManager.GetBeatDuration() / 2f;
        }
        if (attackBeat == 24)
        {
            constellation.Play("virgo");
            constellation.speed = 1f / BeatManager.GetBeatDuration() / 2f;
        }
        if (attackBeat % 6 == 0 && attackBeat > 0) // Comet
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

    private IEnumerator MagicCometCoroutine()
    {
        Vector3 circlePos = Vector2.zero + (Random.insideUnitCircle * 8f) * new Vector2(1f, 0.5f); // Area zone
        Vector3 circle2Pos = Vector2.zero + (Random.insideUnitCircle * 8f) * new Vector2(1f, 0.5f); // Area zone

        SmallMagicCircle circle1 = PoolManager.Get<SmallMagicCircle>();
        circle1.transform.position = circlePos;
        circle1.despawnTime = 2;

        SmallMagicCircle circle2 = PoolManager.Get<SmallMagicCircle>();
        circle2.transform.position = circle2Pos;
        circle2.despawnTime = 2;

        AudioController.PlaySound(AudioController.instance.sounds.chargeBulletSound);

        float time = 0;
        while (time <= BeatManager.GetBeatDuration() * 2f)
        {
            while (GameManager.isPaused) yield return new WaitForEndOfFrame(); // isStunned!
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        while (!BeatManager.isGameBeat) yield return new WaitForEndOfFrame();

        AudioController.PlaySound(AudioController.instance.sounds.bulletwaveShootSound);
        SpawnMagicComet(circlePos);
        SpawnMagicComet(circle2Pos);
    }

    private void SpawnMagicComet(Vector3 pos)
    {
        BulletBase bullet = PoolManager.Get<BulletBase>();
        Vector2 areaPos = Vector2.zero; // This should be the arena
        bullet.transform.position = pos;

        Vector3 playerPos = player.transform.position;//Player.instance.transform.position;
        Vector2 dir = (playerPos - pos).normalized;

        bullet.direction = dir;
        bullet.speed = 14;
        bullet.atk = 3;
        bullet.lifetime = 12;
        bullet.transform.localScale = Vector3.one;
        bullet.startOnBeat = true;
        bullet.behaviours = new List<BulletBehaviour>
            {
                new SpriteLookAngleBehaviour() { start = 0, end = -1 },
                new SpawnBulletOnBeatBehaviour(SpawnCometStar, BeatManager.GetBeatDuration() / 3) { start = 0, end = -1}
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
            //magicCircle.color = new Color(1, 1, 1, 0);
            //magicCircle.transform.localScale = Vector3.one * 1.5f;
            //magicCircleVisible = true;
            targetCloneDistance = 0f;
            StartCoroutine(ChargeAttack1Coroutine());
        }



        if (isPreparingAttack) return; // Wait until the attack is charged;

        if (attackBeat == 0)
        {
            StartCoroutine(SpawnBlackHoleArray());
        }
        /*
        if (attackBeat % 8 == 0)
        {
            ShootBlackHoleToPlayer(transform.position);

        }*/
        if (attackBeat == 12)
        {
            StartCoroutine(ShootLaserAttack());
        }
        if (attackBeat == 16)
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
        /*
        for (int i = 0; i < 12; i++)
        {
            float angle = (360f / 12f) * i * Mathf.Deg2Rad;
            Vector3 pos = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * 7f;
            positions.Add(pos);
        }
        for (int i = 0; i < 6; i++)
        {
            float angle = (360f / 6f) * i * Mathf.Deg2Rad;
            Vector3 pos = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * 3.5f;
            positions.Add(pos);
        }*/

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
                positions.Add(pos);
            }
        }
        AudioController.PlaySound(AudioController.instance.sounds.chargeBulletSound);
        for (int i = 0; i < positions.Count; i++)
        {
            SmallMagicCircle circle1 = PoolManager.Get<SmallMagicCircle>();
            circle1.transform.position = positions[i];
            circle1.despawnTime = 8;
            yield return new WaitForSeconds(BeatManager.GetBeatDuration() / 8f);
        }

        float time = 0;
        while (time <= BeatManager.GetBeatDuration() * 2f)
        {
            while (GameManager.isPaused) yield return new WaitForEndOfFrame(); // isStunned!
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        while (!BeatManager.isGameBeat) yield return new WaitForEndOfFrame();

        for (int i = 0; i < positions.Count; i++)
        {
            SpawnArrayBlackHole(positions[i]);
            yield return new WaitForSeconds(BeatManager.GetBeatDuration() / 8f);
        }
       // AudioController.PlaySound(AudioController.instance.sounds.bulletwaveShootSound);

        yield break;
    }

    private void SpawnArrayBlackHole(Vector3 origin)
    {
        BulletBase bullet = PoolManager.Get<BulletBase>();

        bullet.transform.position = origin + (-Vector3.up * 0.2f);
        Vector3 playerPos = player.transform.position;//Player.instance.transform.position;
        Vector2 dir = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        bullet.direction = dir;
        bullet.speed = 0;
        bullet.atk = 4;
        bullet.lifetime = 10;
        bullet.transform.localScale = Vector3.one;
        bullet.startOnBeat = false;
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
        laser.transform.parent = clone1.transform;
        laser.transform.localPosition = new Vector3(0, -0.2f, 0);
        laser.animator.Play("laser_spawn");
        laser.atk = 10;

        Vector3 playerPos = player.transform.position;//Player.instance.transform.position;
        Vector2 dir = (playerPos - laser.transform.position).normalized;
        float angle = Vector2.SignedAngle(Vector2.down, dir) - 90;

        laser.transform.eulerAngles = new Vector3(0, 0, angle);

        AudioController.PlaySound(AudioController.instance.sounds.chargeBulletSound);

        yield return new WaitForSeconds(BeatManager.GetBeatDuration() * 6f);

        laser.Despawn();

        yield break;
    }

    private IEnumerator ChargeAttack1Coroutine(bool reset = true)
    {
        //magicCircle.transform.localScale = Vector3.one * 1.5f;
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
        //animator.Play("usarin_dance");
        yield break;
    }

    public void OnEnemyPattern()
    {
        if (attackBeat == 0) Shoot();
        //else Move();
        if (attackBeat == 8) attackBeat = -1;
        attackBeat++;
    }

    private void Shoot()
    {
        BulletBase bullet = PoolManager.Get<BulletBase>();

        bullet.transform.position = transform.position + (-Vector3.up * 0.2f);
        Vector3 playerPos = player.transform.position;//Player.instance.transform.position;
        Vector2 dir = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        bullet.direction = dir;
        bullet.speed = 0;
        bullet.atk = 4;
        bullet.lifetime = 6;
        bullet.transform.localScale = Vector3.one;
        bullet.startOnBeat = false;
        bullet.behaviours = new List<BulletBehaviour>
            {
                new SpriteLookAngleBehaviour() { start = 0, end = -1 }
            };
        bullet.animator.Play("redbullet");
        bullet.OnSpawn();
    }
}
