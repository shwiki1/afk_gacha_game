using UnityEngine;
using UnityEngine.UI;

// Replacement for the optional orientation companion used by the original UI
// package. The color effect is handled by DuloGradientColorsCompat when present.
public sealed class DuloGradientOrientationCompat : MonoBehaviour
{
    public bool m_Horizontal;
    public bool m_Veritical;
}
