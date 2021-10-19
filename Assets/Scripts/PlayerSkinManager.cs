using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkinManager : MonoBehaviour
{
    public SkinnedMeshRenderer meshRenderer;
    private bool invincible;
    private Color originColor;

    void Start()
    {
        Color c = meshRenderer.material.color;
        originColor = c;
    }

    public void StartInvincible()
    {
        StartCoroutine(InvincibleCoroutine());
        invincible = true;
    }

    public void StopInvincible()
    {
        invincible = false;
    }


    private IEnumerator InvincibleCoroutine()
    {
        Color originColor = meshRenderer.material.color;
        Color minAlphaColor = originColor;
        minAlphaColor.a = 0.5f;

        float duration = 0f;
        float elapsedTime = 0f;
        bool isFadeOut = true;

        while (invincible)
        {

            if (isFadeOut)
            {
                elapsedTime += Time.deltaTime;
                meshRenderer.material.color = Color.Lerp(originColor, minAlphaColor, elapsedTime / duration);
                if (elapsedTime > duration)
                {
                    isFadeOut = false;
                }
                yield return null;
            }

            else
            {
                elapsedTime -= Time.deltaTime;
                meshRenderer.material.color = Color.Lerp(minAlphaColor, originColor, duration - elapsedTime / duration);

                if (elapsedTime < duration)
                {
                    isFadeOut = false;
                }

                yield return null;
            }
        }

        meshRenderer.material.color = originColor;
        yield break;
    }
}
