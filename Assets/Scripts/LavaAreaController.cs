using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaAreaController : MonoBehaviour
{
    private bool isDamaged;
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (isDamaged) return;
            PlayerController pc = other.gameObject.GetComponent<PlayerController>();
            pc.OnHit(1, pc.transform.position);
            StartCoroutine(Delay());
        }
    }

    private IEnumerator Delay()
    {
        isDamaged = true;
        yield return new WaitForSeconds(1f);
        isDamaged = false;
        yield break;
    }
}
