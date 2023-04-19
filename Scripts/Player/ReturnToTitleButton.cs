using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnToTitleButton : MonoBehaviour
{
    [SerializeField]
    PlayerController controller;

    public void returnToTitle()
    {
        controller.setFadeOut(true);
        SceneTransferManager.Instance.loadScene("Scenes/Util/Title");
    }
}
