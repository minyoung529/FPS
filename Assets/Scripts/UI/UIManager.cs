using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    private const string RECORD_KEY = "RECORD_SCORE";
    //[SerializeField] private Text currentAmmoText;
    [SerializeField] private TextMeshProUGUI currentAmmoText;
    [SerializeField] private Camera uiCamera;
    [SerializeField] private RectTransform canvas;
    [SerializeField] private RectTransform hpBar;
    [SerializeField] private GameObject root;
    [SerializeField] private TextMeshProUGUI curHP;
    [SerializeField] private TextMeshProUGUI maxHP;
    [SerializeField] private Image playerHP;

    public Text scoreText;
    public Text highScoreText;
    public int score;
    public int recordScore;
    [SerializeField] private Animator blackPanel;

    public GameObject hpPref;
    public GameObject player;

    public PlayerController playerController;

    private void Awake()
    {
        Instance = this;
        recordScore = PlayerPrefs.GetInt(RECORD_KEY, 0);
        highScoreText.text = string.Format("HIGH SCORE: {0}", recordScore);
    }
    private void Start()
    {
        playerController = player.GetComponent<PlayerController>();
    }
    public void ChangeCurrentAmmoText(int current)
    {
        currentAmmoText.text = current.ToString();
    }

    public void ChangeScore(int _score)
    {
        score += _score;
        scoreText.text = string.Format("SCORE: {0}", score);

        if (recordScore < score)
        {

            recordScore = score;
            highScoreText.text = string.Format("HIGH SCORE: {0}", recordScore);
            PlayerPrefs.SetInt(RECORD_KEY, recordScore);
        }
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

        if (viewportPoint.z < 0)
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
