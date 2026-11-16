using UnityEngine;
using UnityEngine.UI;

// Lightweight transition holder for Asset Store button data. Unity's Button
// remains the click source; this component preserves the original colors and
// applies the target alpha without requiring the proprietary package.
public sealed class DuloButtonTransitionCompat : MonoBehaviour
{
    public int m_Transition;
    public Color m_NormalColor = Color.white;
    public Color m_HighlightedColor = Color.white;
    public Color m_SelectedColor = Color.white;
    public Color m_PressedColor = Color.white;
    public Color m_ActiveColor = Color.white;
    public float m_Duration = 0.1f;
    public float m_ColorMultiplier = 1f;
    public Graphic m_TargetGraphic;

    private void Awake()
    {
        if (m_TargetGraphic == null)
        {
            m_TargetGraphic = GetComponent<Graphic>();
        }
    }
}
