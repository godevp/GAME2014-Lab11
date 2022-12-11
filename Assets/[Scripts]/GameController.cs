using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject onScreenControls;
    public GameObject miniMap;
    private SoundManager soundManager;

    // Start is called before the first frame update
    void Start()
    {
        onScreenControls = GameObject.Find("OnScreenControls");
        
        onScreenControls.SetActive((Application.platform != RuntimePlatform.WindowsPlayer && 
                                    Application.platform != RuntimePlatform.WindowsEditor));

        miniMap = GameObject.Find("MiniMap");
        miniMap.SetActive(false);

        soundManager = FindObjectOfType<SoundManager>();
        soundManager.PlayMusic();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            miniMap.SetActive(!miniMap.activeInHierarchy);
        }
    }
}
