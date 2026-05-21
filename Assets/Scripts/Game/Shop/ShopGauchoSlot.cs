using System;
using UnityEngine;

/* ----- FACU -----
 * Clase que se usa para crear los jokers en la tienda, tiene su data para saber nombre, costo e icono
 * Tiene un gameObject para ir colocandolo y poder verlo en runtime
 * Tiene un int que marca la posición en la tienda a la que tiene que ir
*/

public class ShopGauchoSlot
{
    public GauchoDataSO data;
    public GameObject go;
    public int slotIndex;
}