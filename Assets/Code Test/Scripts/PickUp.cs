using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    [SerializeField] private PickUpType type;
    public PickUpType Type { get { return type; } }

    [SerializeField] private AudioClip onConsumeSFX;

    private bool hoverUp = true;
    private float yStart;
    [SerializeField] private float yExtent = 1.0f;
    [SerializeField] private float ySpeed = 1.0f;

    private void Start()
    {
        yStart = transform.position.y;
    }

    public void Consume()
    {
        AudioManager.Instance.PlaySFX(onConsumeSFX);
        Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        Vector2 position = transform.position;

        if(hoverUp)
        {
            position.y += ySpeed * Time.fixedDeltaTime;
            if (position.y > yStart + yExtent)
            {
                hoverUp = false;
            }
        }
        else
        {
            position.y -= ySpeed * Time.fixedDeltaTime;
            if (position.y < yStart - yExtent)
            {
                hoverUp = true;
            }
        }

        transform.position = position;
    }
}
