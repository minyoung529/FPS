using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkinManager : MonoBehaviour
{
    public SkinnedMeshRenderer meshRenderer;

    void Start()
    {
        Color c = meshRenderer.material.color;
        c.a = 0.5f;

        meshRenderer.material.color = c;
    }

    public void StartInvincible()
    {

    }

    private IEnumerator InvincibleCoroutine()
    {
        Color originColor = meshRenderer.material.color;
        Color minAlphaColor = originColor;
        minAlphaColor.a = 0.5f;

        float duration = 0f;
        float elapsedTime = 0f;

        while(true)
        {

            if ()
            {
                elapsedTime += Time.deltaTime;
                meshRenderer.material.color = Color.Lerp(originColor, minAlphaColor, elapsedTime / duration);
            }

            else
            {
                meshRenderer.material.color = Color.Lerp(minAlphaColor, originColor, elapsedTime / duration);
            }
        }

        
    }
}
