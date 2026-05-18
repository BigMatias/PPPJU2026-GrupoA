using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PoolSettings", menuName = "Pool/PoolSettings")]

/* ----- FACU -----
 * Es el mismo SO de pool que usé con Fede, pero para acá
 * En la lista va el script con la interfaz IPooleable + la cantidad a poolear
*/

public class PoolSettingsSO : ScriptableObject
{
    public List<PoolSetting> poolSettings;
}

[Serializable]
public class PoolSetting
{
    public MonoBehaviour prefab;
    public int quantity;
}