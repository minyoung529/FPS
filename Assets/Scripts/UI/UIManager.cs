using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    //[SerializeField] private Text currentAmmoText;
    [SerializeField] private TextMeshProUGUI currentAmmoText;
    [SerializeField] private Camera uiCamera;
    [SerializeField] private RectTransform canvas;
    [SerializeField] private RectTransform hpBar;
    [SerializeField] private GameObject root;
    [SerializeField] private TextMeshProUGUI curHP;
    [SerializeField] private TextMeshProUGUI maxHP;
    [SerializeField] private Image playerHP;

    [SerializeField] private Animator blackPanel;

    public GameObject hpPref;
    public GameObject player;

    public PlayerController playerController;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        playerController = player.GetComponent<PlayerController>();
    }
    public void ChangeCurrentAmmoText(int current)
    {
        currentAmmoText.text = current.ToString();
    }

    private void LateUpdate()
    {
        Vector3 targetPos = player.transform.position;
        Vector2 screenPoint = Camera.main.WorldToScreenPoint(targetPos);

        Vector2 canvasPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas, screenPoint, uiCamera, out canvasPos);
        hpBar.localPosition = canvasPos;
    }

    public GameObject AddEnemyHUD()
    {
        GameObject hud = Instantiate(hpPref, root.transform);
        return hud;
    }

    public void UpdateHUDPosition(GameObject hud, Vector3 target)
    {
        Vector3 viewportPoint = Camera.main.WorldToViewportPoint(target);

        if(viewportPoint.z < 0)
        {
            return;
        }

        Vector2 position = new Vector2(viewportPoint.x * canvas.rect.width, viewportPoint.y * canvas.rect.height);
        //RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas, new Vector2(viewportPoint.x* canvas.sizeDelta.x,viewportPoint.y * canvas.sizeDelta.y), uiCamera, out canvasPos);

        hud.transform.localPosition = position;
    }

    public void UpdatePlayerHP(int max, int cur)
    {
        curHP.text = cur.ToString();
        maxHP.text = max.ToString();
        playerHP.fillAmount = (float)cur / (float)max;
    }

    public void GameOver()
    {
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(2f);
        blackPanel.Play("BlackOut");
    }
}
