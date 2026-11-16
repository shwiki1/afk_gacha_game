using UnityEngine;

// Preserves the serialized element/shade settings from the optional UI package.
// The actual visual is supplied by the Image and the generated fallback sprites.
public sealed class DuloElementCompat : MonoBehaviour
{
    public int m_ElementType;
    public int m_Shade;
}
