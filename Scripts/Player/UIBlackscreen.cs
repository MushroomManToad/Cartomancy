using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBlackscreen : MonoBehaviour
{
    [SerializeField]
    private PlayerController player;
    [SerializeField]
    private Image blackScreen;

    [SerializeField]
    private int fadeInTime, fadeOutTime;

    private int fadeInTimer, fadeOutTimer = 0;

    // Start is called before the first frame update
    void Start()
    {
        player.setFadeIn(true);
        blackScreen.enabled = true;
        blackScreen.color = new Color(blackScreen.color.r, blackScreen.color.g, blackScreen.color.b, 1.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (player.getFadeIn())
        {
            if(fadeInTimer < fadeInTime)
            {
                fadeInTimer++;
                blackScreen.color = new Color(blackScreen.color.r, blackScreen.color.g, blackScreen.color.b, ((float)fadeInTime - (float)fadeInTimer) / (float)fadeInTime);
            }
            else
            {
                player.setFadeIn(false);
                fadeInTimer = 0;
                blackScreen.color = new Color(blackScreen.color.r, blackScreen.color.g, blackScreen.color.b, 0.0f);
            }
        }
        if (player.getFadeOut())
        {
            if (fadeOutTimer < fadeOutTime)
            {
                fadeOutTimer++;
                blackScreen.color = new Color(blackScreen.color.r, blackScreen.color.g, blackScreen.color.b, ((float)fadeOutTimer) / (float)fadeInTime);
            }
            else
            {
                player.setFadeOut(false);
                fadeOutTimer = 0;
                blackScreen.color = new Color(blackScreen.color.r, blackScreen.color.g, blackScreen.color.b, 1.0f);
            }
        }
    }
}
