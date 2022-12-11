using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndSceneGameController : MonoBehaviour
{
    private SoundManager soundManager;

    // Start is called before the first frame update
    void Start()
    {

        soundManager = FindObjectOfType<SoundManager>();

        soundManager.PlaySoundFX(SoundFX.DEATH, Channel.PLAYER_DEATH_FX);
        soundManager.PlayMusic();
    }

}
