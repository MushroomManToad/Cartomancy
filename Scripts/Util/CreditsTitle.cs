using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsTitle : MonoBehaviour
{
    public void returnToTitle()
    {
        SceneTransferManager.Instance.loadScene("Scenes/Util/Title");
    }
}
