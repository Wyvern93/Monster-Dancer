using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class BulletPatternTester : MonoBehaviour
{
    [SerializeField] MapTrack track;

    [SerializeField] GameObject bigCarrotPrefab;
    [SerializeField] GameObject carrotPrefab;
    [SerializeField] GameObject blueCarrotPrefab;
    [SerializeField] GameObject bigBulletPrefab;
    [SerializeField] GameObject baseBulletPrefab;

    [SerializeField] GameObject player;

    [SerializeField] int pattern;
    [SerializeField] int attackBeat;

    List<Enemy> allEnemies;
    List<Bullet> allBullets;
    List<BulletBase> sideBullets;

    // Pattern 1
    private bool orbitRight, starOrbitRight;
    private float orbitalBaseAngle;
    private float starOrbitAngle;
    // Start is called before the first frame update
    void Start()
    {
        StartPools();
        BeatManager.SetTrack(track);
        BeatManager.StartTrack();
        BeatManager.compassless = false;
        orbitRight = true;
        allEnemies = new List<Enemy>();
        allBullets = new List<Bullet>();
        sideBullets = new List<BulletBase>();
    }

    private void StartPools()
    {
        PoolManager.CreatePool(typeof(BigUsarinCarrotBullet), bigCarrotPrefab, 100);
        PoolManager.CreatePool(typeof(UsarinCarrotBullet), carrotPrefab, 100);
        PoolManager.CreatePool(typeof(BlueUsarinCarrotBullet), blueCarrotPrefab, 350);
        PoolManager.CreatePool(typeof(BigUsarinBullet), bigBulletPrefab, 100);

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
                    break;
                case 3:
                    OnPattern3();
                    break;
            }
        }
    }

    void OnPattern1()
    {
        if (attackBeat >= 2 && attackBeat <= 6) // Carrot Bullets
        {
            StartCoroutine(ShootBulletsPhase1());
        }
        if (attackBeat == 0) // Bunnies
        {
            StartCoroutine(ShootSecondCarrots());
        }

        if (attackBeat > 6)
        {
            if (attackBeat == 10) attackBeat = -1;
            //Move();

        }
        attackBeat++;
    }

    private IEnumerator ShootBulletsPhase1()
    {
        float angle = 0;
        if (attackBeat == 2) angle = 225;
        else if (attackBeat == 3) angle = 315;
        else if (attackBeat == 4) angle = 150;
        else if (attackBeat == 5) angle = 25;
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
            AudioController.PlaySound(AudioController.instance.sounds.shootBullet);
            BulletBase bullet = PoolManager.Get<BulletBase>();
            bullet.transform.position = transform.position + (Vector3.up * 0.5f) + (Vector3)(offsetDir * 0.8f) + (Vector3)(patternDir * (1.5f));
            bullet.direction = patternDir;
            bullet.speed = 0;
            bullet.atk = 3;
            bullet.lifetime = 8;
            bullet.transform.localScale = Vector3.one;
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

        yield return new WaitForSeconds(BeatManager.GetBeatDuration() * 3);

        foreach (Bullet b in bullets) b.lifetime = 8;

        Vector3 center = Vector3.zero;
        foreach (var bullet in bullets)
        {
            center += bullet.transform.position;
        }

        center /= 8f;
        Vector3 playerPos = player.transform.position;
        Vector2 dir = (playerPos - center).normalized;

        foreach (BulletBase bullet in bullets)
        {
            bullet.behaviours.Add(new SpeedOverTimeBehaviour() { speedPerBeat = 1, start = 3, end = 6, targetSpeed = 12 });
            bullet.direction = dir;
            bullet.angle = Vector2.SignedAngle(Vector2.down, dir) - 90;
            bullet.speed = 9;
        }
        yield break;
    }

    private IEnumerator ShootSecondCarrots()
    {
        for (int i = 0; i < 36; i++)
        {
            float offsetAngle = i * 10f;
            Vector2 offsetDir = new Vector2(Mathf.Cos(offsetAngle * Mathf.Deg2Rad), Mathf.Sin(offsetAngle * Mathf.Deg2Rad));

            BulletBase bullet = PoolManager.Get<BulletBase>();
            
            bullet.transform.position = transform.position + (Vector3.up * 0.5f) + (Vector3)(offsetDir * 0.3f);
            bullet.direction = offsetDir;
            bullet.speed = 8;
            bullet.atk = 10;
            bullet.lifetime = 10;
            bullet.transform.localScale = Vector3.one;
            bullet.behaviours = new List<BulletBehaviour>
            {
                new SpeedOverTimeBehaviour() { speedPerBeat = 1, start = 0, end = 3, targetSpeed = 6 },
                new SpriteWaveBehaviour() { start = 0, end = -1 },
                new RotateOverBeatBehaviour() { start = 0, end = -1, rotateAmount = 70 }
            };
            bullet.animator.Play("notebullet");
            //bullet.targetSpeed = 5;
            bullet.OnSpawn();
            allBullets.Add(bullet);
        }

        AudioController.PlaySound(AudioController.instance.sounds.shootBullet);
        yield break;
    }

    void OnPattern2()
    {
        if (attackBeat % 18 == 0)
        {
            orbitRight = !orbitRight;

        }
        if (attackBeat % 6 == 0)
        {
            StartCoroutine(ShootBigBullets());
        }
        StartCoroutine(ShootBulletsPhase2());
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
            bullet.speed = 5;
            bullet.atk = 10;
            bullet.lifetime = 10;
            bullet.transform.localScale = Vector3.one;
            bullet.behaviours = new List<BulletBehaviour>
            {
                new SpriteWaveBehaviour() { start = 0, end = -1 }
            };
            bullet.animator.Play(orbitRight ? "notebullet" : "rednotebullet");
            bullet.OnSpawn();
            allBullets.Add(bullet);
        }
        AudioController.PlaySound(AudioController.instance.sounds.shootBullet);
        orbitalBaseAngle += orbitRight ? 6 : -6;

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
                bullet.behaviours = new List<BulletBehaviour>
                {
                    new SpriteSpinBehaviour() { start = 0, end = -1 },
                    new SpeedOverTimeBehaviour() { start = 0, end = 1, speedPerBeat = 5, targetSpeed = 0},
                    new SpeedOverTimeBehaviour() { start = 2, end = 5, speedPerBeat = 1f + (i2 / 2f), targetSpeed = 5f + (i2)},
                    new RotateOverBeatBehaviour() { start = 2, end = -1, rotateAmount = starOrbitRight ? -40f : 40}
                };
                bullet.animator.Play("starbullet");
                //bullet.targetSpeed = 5;
                bullet.OnSpawn();
                allBullets.Add(bullet);
            }
        }
        starOrbitRight = !starOrbitRight;
        starOrbitAngle += 36;
        AudioController.PlaySound(AudioController.instance.sounds.shootBullet);
        yield break;
    }

    void OnPattern3()
    {
        if (attackBeat == 0) orbitRight = false;
        if (attackBeat % 8 == 0) // Carrot Bullets
        {
            StartCoroutine(ShootSideCarrots());
        }
        if (attackBeat % 10 == 0) // Carrot Bullets
        {
            StartCoroutine(ShootSpinningCarrots());
        }

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

        AudioController.PlaySound(AudioController.instance.sounds.shootBullet);

        yield return new WaitForSeconds(BeatManager.GetBeatDuration() * 4);
        Vector2 arenaPos = Vector2.zero; // Change this to the arena with the boss
        for (int i = 0; i < 21; i++)
        {
            float xoffset = i - 10;
            bullets[i].transform.position = arenaPos + (Vector2.up * 10) + (Vector2.right * xoffset);
            bullets[i].speed = 5;
            bullets[i].angle = -90 + Random.Range(-20f, 20f);
            bullets[i].direction = Vector2.down;
        }
        yield break;
    }

    private IEnumerator ShootSideCarrots()
    {
        Vector2 arenaPos = Vector2.zero; // Change this to the arena with the boss

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

    public void SpawnBunnies(Vector3 position)
    {
        List<NomSlime> wave = new List<NomSlime>();
        // Spawn Bunnies
        for (int i = 0; i < 5; i++)
        {
            float angle = (360 / 5) * i;

            Vector3 pos = position + new Vector3(1f * (Mathf.Cos(angle * Mathf.Deg2Rad)), 1f * (Mathf.Sin(angle * Mathf.Deg2Rad)));
            NomSlime nomSlime = (NomSlime)Enemy.GetEnemyOfType(EnemyType.NomSlime);
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
        Vector3 playerPos = player.transform.position; //Player.instance.GetClosestPlayer(position);
        // Send them
        foreach (Enemy enemy in wave)
        {
            Vector2 dir = (playerPos - enemy.transform.position).normalized;
            enemy.AItype = 2;
            enemy.lifeTime = 10;
            enemy.eventMove = dir;
        }
    }
}