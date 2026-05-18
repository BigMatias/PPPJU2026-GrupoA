using System.Collections.Generic;

/* ----- FACU -----
 * El contexto de la partida
 * Sirve para saber cuantos son los puntos, el mult, la mano, las cartas jugadas
 * En resúmen, los datos que puedo usar para analizar y activar efectos en gauchos
 * Si está acá, puede activar un gaucho basically
 */

public class GauchoContext
{
    public int points;
    public float mult;

    public Card playedCard;
    public List<Card> hand;

    public int trucosWon;
    public bool wonLastRound;
}