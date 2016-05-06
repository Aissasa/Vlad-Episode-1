using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    public AudioSource playerCombatEfxSource;                   
    public AudioSource playerReactionEfxSource;
    //public AudioSource enemiesCombatEfxSource;
    public AudioSource enemiesReactionEfxSource;

    public AudioSource musicSource;                 
    public static SoundManager instance = null;    
    public float lowPitchRange = .95f;              
    public float highPitchRange = 1.05f;           


    void Awake()
    {
        //Check if there is already an instance of SoundManager
        if (instance == null)
            //if not, set it to this.
            instance = this;
        //If instance already exists:
        else if (instance != this)
            //Destroy this, this enforces our singleton pattern so there can only be one instance of SoundManager.
            Destroy(gameObject);

        //Set SoundManager to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
        DontDestroyOnLoad(gameObject);
    }


    public void PlaySingleCombatSfx(bool player, AudioClip clip)
    {
        if (player)
        {
            PlaySingle(playerCombatEfxSource, clip);

        }
        //else
        //{
        //    PlaySingle(enemiesCombatEfxSource, clip);
        //}
    }

    public void PlaySingleReactionSfx(bool player, AudioClip clip)
    {
        if (player)
        {
            PlaySingle(playerReactionEfxSource, clip);

        }
        else
        {
            PlaySingle(enemiesReactionEfxSource, clip);
        }
    }

    public void RandomizeCombatSfx(bool player, params AudioClip[] clips)
    {
        if (player)
        {
            RandomizeSfx(playerCombatEfxSource, clips);

        }
        //else
        //{
        //    RandomizeSfx(enemiesCombatEfxSource, clips);
        //}
    }

    public void RandomizeReactionSfx(bool player, params AudioClip[] clips)
    {
        if (player)
        {
            RandomizeSfx(playerReactionEfxSource, clips);

        }
        else
        {
            RandomizeSfx(enemiesReactionEfxSource, clips);
        }
    }

    //Used to play single sound clips.
    private void PlaySingle(AudioSource source, AudioClip clip)
    {
        //Set the clip of our efxSource audio source to the clip passed in as a parameter.
        source.clip = clip;

        //Play the clip.
        source.Play();
    }


    //RandomizeSfx chooses randomly between various audio clips and slightly changes their pitch.
    private void RandomizeSfx(AudioSource source, params AudioClip[] clips)
    {
        //Generate a random number between 0 and the length of our array of clips passed in.
        int randomIndex = Random.Range(0, clips.Length);

        //Choose a random pitch to play back our clip at between our high and low pitch ranges.
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);

        //Set the pitch of the audio source to the randomly chosen pitch.
        source.pitch = randomPitch;

        //Set the clip to the clip at our randomly chosen index.
        source.clip = clips[randomIndex];

        //Play the clip.
        source.Play();
    }
}
