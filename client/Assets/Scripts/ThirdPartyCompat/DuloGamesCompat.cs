using UnityEngine;
using UnityEngine.UI;

// Minimal local replacement for the single DuloGames.UI type used by the
// public scripts. It keeps gameplay code compiling when the optional RPG &
// MMO UI package is absent; the visual fill image can be assigned in a prefab.
namespace DuloGames.UI
{
    public sealed class UIProgressBar : MonoBehaviour
    {
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
                if (fillImage != null)
                    fillImage.fillAmount = currentFillAmount;
            }
        }

        private void Awake()
        {
            if (fillImage == null)
                fillImage = GetComponentInChildren<Image>();
            fillAmount = currentFillAmount;
        }
    }
}
