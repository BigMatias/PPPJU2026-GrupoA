using System.Collections;
using TMPro;
using UnityEngine;
using DG.Tweening;

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
    private float _visualPoints = 0;
    private float _visualMult = 0;
    private float _visualTotalPoints = 0;

    private void OnEnable()
    {
        _rm.OnSetNeededScore += SetNeededPoints;
        _scoreManager.OnScoreChanged += UpdateLiveScore;
        _scoreManager.OnCalculateScore += ShowFinalScore;
        _rm.OnInfoUpdated += UpdateRoundInfo;

    }

    private void Start()
    {
        RunManager.Instance.MoneySystem.OnUpdateMoney += OnUpdateMoney_UpdateText;
        DOTween.Init();
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
        DOTween.Clear();
    }

    private void UpdateLiveScore(float points, float mult)
    {
        // points
        DOTween.To(() => _visualPoints, p => _visualPoints = p, points, 0.75f).SetTarget(this).SetEase(Ease.OutQuad).OnUpdate(() =>
        {
            _textPoints.text = _visualPoints.ToString("F0");
        });

        // mult
        DOTween.To(() => _visualMult, x => _visualMult = x, mult, 0.75f).SetTarget(this).SetEase(Ease.OutQuad).OnUpdate(() =>
        {
            _textMult.text = _visualMult.ToString("F0");
        });
    }

    private void ShowFinalScore(float points, float mult, float totalPoints)
    {
        if (_coroutineShowPoints != null)
            StopCoroutine(_coroutineShowPoints);
        _coroutineShowPoints = StartCoroutine(FinalScoreCoroutine(points, mult, totalPoints));
    }

    private IEnumerator FinalScoreCoroutine(float points, float mult, float totalPoints)
    {
        // points
        DOTween.To(() => _visualPoints, p => _visualPoints = p, points, 0.75f).SetTarget(this).SetEase(Ease.OutQuad).OnUpdate(() =>
        {
            _textPoints.text = _visualPoints.ToString("F0");
        });

        // mult
        DOTween.To(() => _visualMult, y => _visualMult = y, mult, 0.75f).SetTarget(this).SetEase(Ease.OutQuad).OnUpdate(() =>
        {
            _textMult.text = _visualMult.ToString("F0");
        });

        // total
        DOTween.To(() => _visualTotalPoints, x => _visualTotalPoints = x, totalPoints, 1.5f).SetTarget(this).SetEase(Ease.OutQuad).OnUpdate(() =>
        {
            _textTotalPoints.text = _visualTotalPoints.ToString("F0");
        });

        yield return new WaitForSeconds(_timeShowingPoints);

        _textPoints.text = "0";
        _textMult.text = "0";
        _visualPoints = 0;
        _visualMult = 0;
    }

    private void UpdateRoundInfo(int mesa, int chico, int manosRestantes)
    {
        _textMesa.text = mesa.ToString();
        _textChico.text = chico.ToString();
        _textManosRestantes.text = manosRestantes.ToString();
    }

    private void SetNeededPoints(float points)
    {
        _textNeededPoints.text = points.ToString("0");
        _textTotalPoints.text = "0";
        _visualPoints = 0;
        _visualMult = 0;
        _visualTotalPoints = 0;
    }

    private void OnUpdateMoney_UpdateText() => _textMoney.text = RunManager.Instance.MoneySystem.CurrentMoney.ToString("0");
}