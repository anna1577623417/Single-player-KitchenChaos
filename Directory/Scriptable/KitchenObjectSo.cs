using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class KitchenObjectSo : ScriptableObject
{
    public Transform prefab;
    public Sprite sprite;
    [SerializeField]private string objectName;
}
