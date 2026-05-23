using System.Collections;
using TMPro;
using UnityEngine;

public class UiPoints : MonoBehaviour
{
    [SerializeField] private float _timeShowingPoints = 2f;
    [Header("Points")]
    [SerializeField] private TextMeshProUGUI _textNeededPoints;
    [SerializeField] private TextMeshProUGUI _textTotalPoints;
    [SerializeField] private TextMeshProUGUI _textPoints;
    [SerializeField] private TextMeshProUGUI _textMult;
    [Header("Game info")]
    [SerializeField] private TextMeshProUGUI _textMoney;
    [Header("Round Info")]
    [SerializeField] private TextMeshProUGUI _textMesa;
    [SerializeField] private TextMeshProUGUI _textChico;
    [SerializeField] private TextMeshProUGUI _textManosRestantes;

    [SerializeField] private ScoreManager _scoreManager;
    [SerializeField] private RoundManager _rm;

    private Coroutine _coroutineShowPoints;

    private void OnEnable()
    {
        _rm.OnSetNeededScore += SetNeededPoints;
        _scoreManager.OnScoreChanged += UpdateLiveScore;
        _scoreManager.OnCalculateScore += ShowFinalScore;
        _rm.OnInfoUpdated += UpdateRoundInfo;

        RunManager.Instance.MoneySystem.OnUpdateMoney += OnUpdateMoney_UpdateText;
    }

    private void OnDisable()
    {
        _rm.OnSetNeededScore -= SetNeededPoints;
        _scoreManager.OnScoreChanged -= UpdateLiveScore;
        _scoreManager.OnCalculateScore -= ShowFinalScore;
        _rm.OnInfoUpdated -= UpdateRoundInfo;

        RunManager.Instance.MoneySystem.OnUpdateMoney -= OnUpdateMoney_UpdateText;
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    private void UpdateLiveScore(float points, float mult)
    {
        _textPoints.text = points.ToString("0");
        _textMult.text = mult.ToString("0");
        _textTotalPoints.text = (points * mult).ToString("0");
    }

    private void ShowFinalScore(float points, float mult, float totalPoints)
    {
        if (_coroutineShowPoints != null)
            StopCoroutine(_coroutineShowPoints);
        _coroutineShowPoints = StartCoroutine(FinalScoreCoroutine(points, mult, totalPoints));
    }

    private IEnumerator FinalScoreCoroutine(float points, float mult, float totalPoints)
    {
        _textPoints.text = points.ToString("0");
        _textMult.text = mult.ToString("0");
        _textTotalPoints.text = totalPoints.ToString("0");

        yield return new WaitForSeconds(_timeShowingPoints);

        _textPoints.text = "0";
        _textMult.text = "0";
    }

    private void UpdateRoundInfo(int mesa, int chico, int manosRestantes)
    {
        _textMesa.text = mesa.ToString();
        _textChico.text = chico.ToString();
        _textManosRestantes.text = manosRestantes.ToString();
    }

    private void SetNeededPoints(float points) => _textNeededPoints.text = points.ToString("0");

    private void OnUpdateMoney_UpdateText() => _textMoney.text = RunManager.Instance.MoneySystem.CurrentMoney.ToString("0");
}