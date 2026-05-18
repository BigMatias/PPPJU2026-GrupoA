using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GauchosList", menuName = "Gauchos/GauchosList")]

/* ----- FACU -----
 * Una lista de todos los gauchos del juego
 */

public class GauchosListSO : ScriptableObject
{
    public List<GauchoDataSO> gauchosList;
}