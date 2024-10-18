using UnityEngine;

public class BurningVisualEffect : MonoBehaviour
{
    public Enemy source;
    public void Update()
    {
        if (!source.gameObject.activeSelf || !source.burnStatus.isBurning())
        {
            PoolManager.Return(gameObject, GetType());
        }
        else
        {
            transform.position = new Vector3(source.Sprite.transform.position.x, source.Sprite.transform.position.y, source.Sprite.transform.position.z - 2f);
            transform.localScale = source.transform.localScale;
        }
    }
}