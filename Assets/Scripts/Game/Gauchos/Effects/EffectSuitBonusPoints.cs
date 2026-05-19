using System.Linq;
using UnityEngine;

/* ----- FACU -----
 * Cada clase es un efecto disntinto que se puede agregar a cada joker.
 * El nombre de la clase explica más o menos que hace, pero igual son bastante explicatorias con el código y los debugs también
 * Para agregar una clase, copiar la última, sumarle 1 al comentario con el número y modificar nombre y cosas de adentro
 * No hace falta crear una clase por cada número distinto que se quiera agregar
 * Ej: si quiero +5 y +8 de mult, solo creo 2 EffectMult y al _bonus le pongo 5 y 8 en cada uno
 * Lo mismo para todos los que heredan de GauchoEffectSO
 */

[CreateAssetMenu(fileName = "SuitPoints", menuName = "Gauchos/Effects/SuitPoints")]
public class EffectSuitPoints : GauchoEffectSO // 1
{
    [SerializeField] private Suit _targetSuit;
    [SerializeField] private int _bonus;

    public override void Execute(GauchoContext context, GauchoInstance owner)
    {
        if (context.playedCard.cardDataSO.suit == _targetSuit)
        {
            context.points += _bonus;
            Debug.Log("Suit point effect was successful");
        }
        else
            Debug.Log("Suits weren't equal");
    }
}

[CreateAssetMenu(fileName = "SuitPoints", menuName = "Gauchos/Effects/SuitPoints")]
public class EffectMultEveryRound : GauchoEffectSO // 9
{
    [SerializeField] private int _bonus;

    public override void Execute(GauchoContext context, GauchoInstance owner)
    {
        owner.stack++;
        context.points *= _bonus * owner.stack;
        Debug.Log("Added mult for round played");
    }
}
