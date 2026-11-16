using UnityEngine;
using UnityEngine.UI;

// Minimal local replacement for the single DuloGames.UI type used by the
// public scripts. It keeps gameplay code compiling when the optional RPG &
// MMO UI package is absent; the visual fill image can be assigned in a prefab.
namespace DuloGames.UI
{
    public sealed class UIProgressBar : MonoBehaviour
    {
        // Names match the public serialized data from the original Asset
        // Store component so existing prefab references survive reimport.
        [SerializeField]
        private Image m_TargetImage;

        [SerializeField]
        private float m_FillAmount;

        [SerializeField]
        private Image fillImage;

        [SerializeField, Range(0f, 1f)]
        private float currentFillAmount;

        public float fillAmount
        {
            get => currentFillAmount;
            set
            {
                currentFillAmount = Mathf.Clamp01(value);
                m_FillAmount = currentFillAmount;
                if (fillImage != null)
                    fillImage.fillAmount = currentFillAmount;
                if (m_TargetImage != null)
                    m_TargetImage.fillAmount = currentFillAmount;
            }
        }

        private void Awake()
        {
            if (fillImage == null)
                fillImage = m_TargetImage != null ? m_TargetImage : GetComponentInChildren<Image>();
            fillAmount = m_FillAmount > 0f ? m_FillAmount : currentFillAmount;
        }
    }
}
