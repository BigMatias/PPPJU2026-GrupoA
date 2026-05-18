using UnityEngine;

/* ----- FACU -----
 * Cada clase es un efecto disntinto que se puede agregar a cada joker.
 * El nombre de la clase explica más o menos que hace, pero igual son bastante explicatorias con el código y los debugs también
 * Para agregar una clase, copiar la última, sumarle 1 al comentario con el número y modificar nombre y cosas de adentro
 * No hace falta crear una clase por cada número distinto que se quiera agregar
 * Ej: si quiero +5 y +8 de mult, solo creo 2 EffectMult y al _bonus le pongo 5 y 8 en cada uno
 * Eso debería ser todo por ahora, cuando los termine los voy a separar en scripts, los tengo así para más comodidad lo prometo
 */

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

public class EffectSuitMult : GauchoEffectSO // 2
{
    [SerializeField] private Suit _targetSuit;
    [SerializeField] private int _bonus;

    public override void Execute(GauchoContext context, GauchoInstance owner)
    {
        if (context.playedCard.cardDataSO.suit == _targetSuit)
        {
            context.mult += _bonus;
            Debug.Log("Suit mult effect was successful");
        }
        else
            Debug.Log("Suits weren't equal");
    }
}

public class EffectRandomMult : GauchoEffectSO // 3
{
    [SerializeField] private int _bonus;

    public override void Execute(GauchoContext context, GauchoInstance owner)
    {   
        float rand = Random.value;
        if (rand > 0.75f)
        {
            context.mult += _bonus;
            Debug.Log("Random mult effect was successfull");
        }
        else
            Debug.Log("Random was less than 0.75");
    }
}

public class EffectMultEveryNumRounds : GauchoEffectSO // 4
{
    [SerializeField] private int _roundsToAffect;
    [SerializeField] private int _bonus;

    public override void Execute(GauchoContext context, GauchoInstance owner)
    {
        owner.stacks++;
        if (owner.stacks == _roundsToAffect)
        {
            context.mult *= _bonus;
            owner.stacks = 0;
            Debug.Log("Rounds mult effect was successfull");
        }
        else
            Debug.Log("Wait " + (_roundsToAffect - owner.stacks) + " rounds for the effect...");
    }
}

public class EffectPointsRoundLost : GauchoEffectSO // 5
{
    [SerializeField] private int _bonus;

    public override void Execute(GauchoContext context, GauchoInstance owner)
    {
        if (!context.wonLastRound)
        {
            context.points += _bonus;
            Debug.Log("Last round was lost, so added points to bonus");
        }
        else
            Debug.Log("Last round was won");
    }
}