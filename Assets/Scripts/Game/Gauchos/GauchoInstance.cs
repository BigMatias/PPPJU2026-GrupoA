/* ----- FACU -----
 * Este script es la "representación" de los gauchos, cada uno tiene su instance
 * data es su data, duh jasdjasj
 * stacks lo uso para sumarle en los efectos
 * Ej: si cada X rondas se activa, stacks++ hasta que llegue y desp vuelve a 0
 * Ej 2: si por cada truco ganado me da +2 de mult, la cuenta sería 2 * stacks
 * activatedThisTurn es para saber si ya se activó el joker en la mano, para no hacer los efectos 2 veces
 */

public class GauchoInstance : IPooleable
{
    public GauchoDataSO data;

    public bool isActive;
    public int stacks;
    public bool activatedThisTurn;
    public bool IsActive { get; set; }

    public GauchoInstance(GauchoDataSO gauchoData) => data = gauchoData;

    public void Activate()
    {
        IsActive = true;
    }

    public void DeActivate()
    {
        IsActive = false;
    }
}