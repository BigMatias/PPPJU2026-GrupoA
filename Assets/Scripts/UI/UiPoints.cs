using System.Collections;
using TMPro;
using UnityEngine;

public class UiPoints : MonoBehaviour
{
    [SerializeField] private float _timeShowingPoints = 1.5f;
    [SerializeField] private TextMeshProUGUI _textNeededPoints;
    [SerializeField] private TextMeshProUGUI _textTotalPoints;
    [SerializeField] private TextMeshProUGUI _textPoints;
    [SerializeField] private TextMeshProUGUI _textMult;
    [SerializeField] private TextMeshProUGUI _textResult;

    private IEnumerator _coroutineShowPoints;

    private void Start()
    {
        // set _textNeededPoints
    }

    private void OnEnable()
    {

    }

    private void OnDisable()
    {

    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    private IEnumerator ShowingPoints(float points, float mult, float result, float totalPoints)
    {
        // Esto está con whiles en vez de con waitForSeconds() porque la idea que hacer un lerp que vaya subiendo y bajando los números como en Balatro
        float clock = _timeShowingPoints;
        while (clock > 0) // MOSTRAR PUNTOS Y MULT
        {
            _textPoints.text = points.ToString("0");
            _textMult.text = mult.ToString("0");
            clock -= Time.deltaTime;
            yield return null;
        }

        clock = _timeShowingPoints;
        while (clock > 0) // MOSTRAR RESULTADO DE PUNTOS * MULT
        {
            _textResult.text = result.ToString("0");
            _textTotalPoints.text = totalPoints.ToString("0");
            clock -= Time.deltaTime;
            yield return null;
        }

        clock = _timeShowingPoints;
        while (clock > 0) // MOSTRAR EL PUNTAJE TOTAL + RESULTADO DE LA MANO
        {
            _textTotalPoints.text = totalPoints.ToString("0");
            clock -= Time.deltaTime;
            yield return null;
        }

        yield return null;
    }

    private void ChangePointsAndMultText(float points, float mult, float result, float totalPoints)
    {
        if (_coroutineShowPoints != null)
            StopCoroutine(_coroutineShowPoints);

        _coroutineShowPoints = ShowingPoints(points, mult, result, totalPoints);
        StartCoroutine(_coroutineShowPoints);
    }
}