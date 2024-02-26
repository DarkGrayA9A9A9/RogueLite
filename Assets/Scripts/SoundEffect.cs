using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffect : MonoBehaviour
{
    public static SoundEffect instance;

    AudioSource audio;

    [Header("# Sound Effects")]
    public AudioClip[] attackSound;
    public AudioClip jumpSound;
    public AudioClip dashSound;
    public AudioClip hitSound;
    public AudioClip dieSound;
    public AudioClip guardSound;
    public AudioClip monsterHitSound;
    public AudioClip[] monsterDieSound;
    public AudioClip menuSound;
    public AudioClip enforceSound;
    public AudioClip clearSound;

    void Awake()
    {
        audio = GetComponent<AudioSource>();

        if (SoundEffect.instance == null)
            SoundEffect.instance = this;
    }

    void Update()
    {
        
    }

    public void SoundEffectSetting()
    {
        audio.volume = GameManager.instance.SEValue;
    }

    public void Attack()
    {
        audio.clip = attackSound[0];
        audio.Play();
    }

    public void PowerAttackSound()
    {
        audio.clip = attackSound[1];
        audio.Play();
    }

    public void Jump()
    {
        audio.PlayOneShot(jumpSound);
    }

    public void Dash()
    {
        audio.PlayOneShot(dashSound, audio.volume * 20f);
    }

    public void Hit()
    {
        audio.PlayOneShot(hitSound);
    }

    public void Die()
    {
        audio.PlayOneShot(dieSound);
    }

    public void Guard()
    {
        audio.PlayOneShot(guardSound);
    }

    public void MonsterHit()
    {
        audio.PlayOneShot(monsterHitSound);
    }

    public void MonsterDie(int index)
    {
        audio.PlayOneShot(monsterDieSound[index]);
    }

    public void Menu()
    {
        audio.PlayOneShot(menuSound);
    }

    public void Enforce()
    {
        audio.PlayOneShot(enforceSound);
    }
    
    public void Clear()
    {
        audio.PlayOneShot(clearSound);
    }
}
