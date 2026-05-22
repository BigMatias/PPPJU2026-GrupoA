using System.Collections;
using TMPro;
using UnityEngine;

public class UiPoints : MonoBehaviour
{
    [SerializeField] private float _timeShowingPoints = 1f;
    [Header("Points")]
    [SerializeField] private TextMeshProUGUI _textNeededPoints;
    [SerializeField] private TextMeshProUGUI _textTotalPoints;
    [SerializeField] private TextMeshProUGUI _textPoints;
    [SerializeField] private TextMeshProUGUI _textMult;
    [Header("Game info")]
    [SerializeField] private TextMeshProUGUI _textRound;
    [SerializeField] private TextMeshProUGUI _textAnte;
    [SerializeField] private TextMeshProUGUI _textMoney;

    private IEnumerator _coroutineShowPoints;

    private GameManager _gm;

    private void Awake()
    {
        _gm = GameManager.Instance;
    }

    private void OnEnable()
    {
        _gm.OnSetRoundInfo += SetRunInfo;

        _gm.OnSetNeededScore += SetNeededPoints;
        _gm.OnCalculateScore += UpdateScore;
    }

    private void OnDisable()
    {
        _gm.OnSetRoundInfo -= SetRunInfo;

        _gm.OnSetNeededScore -= SetNeededPoints;
        _gm.OnCalculateScore -= UpdateScore;
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    private IEnumerator ShowingPoints(float points, float mult, float totalPoints)
    {
        _textPoints.text = points.ToString("0");
        _textMult.text = mult.ToString("0");

        yield return new WaitForSeconds(_timeShowingPoints);

        _textTotalPoints.text = totalPoints.ToString("0");

        yield return new WaitForSeconds(_timeShowingPoints);

        _textPoints.text = "0";
        _textMult.text = "0";

        yield return null;
    }

    private void SetRunInfo(int round, int ante, int money)
    {
        _textRound.text = round.ToString("0");
        _textAnte.text = ante.ToString("0");
        _textMoney.text = money.ToString("0");
    }

    private void SetNeededPoints(float points) => _textNeededPoints.text = points.ToString("0");

    private void UpdateScore(float points, float mult, float totalPoints)
    {
        if (_coroutineShowPoints != null)
            StopCoroutine(_coroutineShowPoints);

        _coroutineShowPoints = ShowingPoints(points, mult, totalPoints);
        StartCoroutine(_coroutineShowPoints);
    }
}