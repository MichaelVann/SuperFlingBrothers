using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCrack : MonoBehaviour
{
    public Sprite[] m_decals;
    public AudioClip m_thudSound;

    // Start is called before the first frame update
    void Start()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = m_decals[VLib.vRandom(0, m_decals.Length-1)];
        float shade = VLib.vRandom(0.1f, 0.3f);
        spriteRenderer.color = new Color(shade*2f, shade*1.3f, shade, 1f);
        GameHandler.m_staticAutoRef.m_audioHandlerRef.PlaySoundEffect(m_thudSound, 1f);
    }




    // Update is called once per frame
    void Update()
    {
        
    }
}
