using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class RabiClone : MonoBehaviour
{
    public Vector2 position;
    public SpriteRenderer spriteRend;

    float beatDuration = 10f;
    float SpriteX = 1f;

    private bool facingRight = true;
    public void OnInit()
    {
        position = transform.position;
        if ((int)Player.instance.abilityValues["ability.bunnyhop.level"] >= 4)
        {
            beatDuration = 20;
        }
        else
        {
            beatDuration = 10;
        }
    }

    public void OnDespawn()
    {
        //SmokeExplosion smoke = PoolManager.Get<SmokeExplosion>();
        //smoke.transform.position = transform.position;
        Player.instance.playerClones.Remove(gameObject);

        CarrotExplosion carrotExplosion = PoolManager.Get<CarrotExplosion>();
        carrotExplosion.transform.position = transform.position;
        carrotExplosion.dmg = 30;

        PoolManager.Return(gameObject, GetType());
    }

    public void Update()
    {
        if (BeatManager.isGameBeat)
        {
            beatDuration--;
            if (BeatManager.beats % 2 == 0)
            {
                facingRight = !facingRight;
            }
            if (beatDuration == 0)
            {
                OnDespawn();
            }
        }

        UpdateSprite();
    }

    private void UpdateSprite()
    {
        SpriteX = Mathf.MoveTowards(SpriteX, facingRight ? 1 : -1, Time.deltaTime * 24f);
        spriteRend.transform.localScale = new Vector3(SpriteX, 1, 1);
    }
}