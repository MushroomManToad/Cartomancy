using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Logo : MonoBehaviour
{
    [SerializeField]
    private Image logo;

    // Current Frames
    private int fICount, fOCount = 0;

    // Max frames
    [SerializeField]
    private int fIFrames, fOFrames;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(fadeIn());
    }

    private IEnumerator fadeIn()
    {
        while(fICount < fIFrames)
        {
            logo.color = new Color(logo.color.r, logo.color.g, logo.color.b, (float)fICount / (float)fIFrames);
            fICount++;
            yield return new WaitForSeconds(0.02f);
        }

        logo.color = new Color(logo.color.r, logo.color.g, logo.color.b, 1);
        yield return new WaitForSeconds(0.5f);

        StartCoroutine(fadeWait());
    }

    private IEnumerator fadeWait()
    {
        yield return new WaitForSeconds(1.5f);

        StartCoroutine(fadeOut());
    }

    private IEnumerator fadeOut()
    {
        while (fOCount < fOFrames)
        {
            logo.color = new Color(logo.color.r, logo.color.g, logo.color.b, ((float)fOFrames - (float)fOCount) / (float)fOFrames);
            fOCount++;
            yield return new WaitForSeconds(0.02f);
        }
        logo.color = new Color(logo.color.r, logo.color.g, logo.color.b, 0);
        yield return new WaitForSeconds(0.5f);

        SceneTransferManager.Instance.loadScene("Scenes/Util/Title");
    }
}
