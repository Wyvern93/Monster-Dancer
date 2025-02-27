using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class BeatObject : MonoBehaviour
{
    public float beatTime;
    public bool isBeat;
    public bool isRight;
    [SerializeField] RectTransform rectTransform;
    float position;
    [SerializeField] Image image;
    [SerializeField] Image mirror;
    public int midBeat;
    public float height;
    private Sprite currentSprite;

    public void OnInit(float time, bool isRight, int midbeats, Sprite sprite, float height)
    {
        currentSprite = sprite;
        this.height = height;
        beatTime = time;
        this.isRight = isRight;
        // The position should decrease towards 0 as we move toward the center
        double musicTime = BeatManager.instance.currentTime;
        float timeDifference = beatTime - (float)musicTime;

        // The position starts far outside and moves toward the center (0)
        position = timeDifference * 200f;  // Adjust movement to center
        transform.localPosition = new Vector3(position, 0, 0); // Right side moves to center

        if (sprite == null)
        {
            image.color = Color.clear;
            mirror.color = Color.clear;
        }
        else
        {
            image.color = Color.white;//new Color(1, 1, 1, 1 - (position / 320));
            mirror.color = image.color;
            image.sprite = sprite;
            mirror.sprite = sprite;
            image.GetComponent<RectTransform>().sizeDelta = new Vector2(sprite.rect.width, sprite.rect.height);
            mirror.GetComponent<RectTransform>().sizeDelta = new Vector2(sprite.rect.width, sprite.rect.height);
        }
        image.sprite = sprite;
        mirror.sprite = sprite;
        
        mirror.transform.localPosition = new Vector3(position * 2f, height);
        this.midBeat = midbeats;
    }

    public void Update()
    {
        // The position should decrease towards 0 as we move toward the center
        double musicTime = BeatManager.instance.currentTime;
        float timeDifference = beatTime - (float)musicTime;

        // The position starts far outside and moves toward the center (0)
        position = timeDifference * 200f;  // Adjust movement to center

        // Moving towards the center (positive X for right side, negative X for left side)
        if (isRight)
        {
            transform.localPosition = new Vector3(position, height, 0); // Right side moves to center
        }
        else
        {
            transform.localPosition = new Vector3(-position, height, 0); // Left side moves to center
        }

        if (currentSprite == null)
        {
            image.color = Color.clear;
            mirror.color = Color.clear;
        }
        else
        {
            image.color = Color.white;//new Color(1, 1, 1, 1 - (position / 320));
            mirror.color = image.color;
        }
        mirror.transform.localPosition = new Vector3(position * 2f, 0);
    }
}