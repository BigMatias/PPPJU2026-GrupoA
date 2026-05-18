/* ----- FACU -----
 * Cada variable del enum representa un evento dentro de las manos
 * Estos se llaman durante el juego y afectan a los jokers
 * Ej: Win truco podría activar un joker de x2 mult, qsy
 */

public enum GameEvents
{
    None = -1,
    RoundStart,
    CardPlayed,
    WinTruco,
    LoseTruco,
    WinEnvido,
    LoseEnvido,
    StealCard,
    ScoreCalculation,
    RoundEnd
}