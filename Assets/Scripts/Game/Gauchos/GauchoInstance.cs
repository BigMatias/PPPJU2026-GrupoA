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

    public int stack;
    public bool activatedThisTurn;

    private bool isActive;
    public bool IsActive => isActive;

    public GauchoInstance(GauchoDataSO gauchoData) => data = gauchoData;

    public void Activate()
    {
        isActive = true;
    }

    public void DeActivate()
    {
        isActive = false;
    }

    public virtual void Reset() { }
}