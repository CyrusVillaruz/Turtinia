using System;
using UnityEngine;
using UnityEngine.UI;

namespace Turtinia.UI
{
    public enum CrystalStatus
    {
        Empty = 0,
        Half = 1,
        Full = 2
    }
    
    public class ManaCrystal : MonoBehaviour
    {
        public Sprite FullCrystal, HalfCrystal, EmptyCrystal;
        private Image _crystalImage;

        private void Awake()
        {
            _crystalImage = GetComponent<Image>();
        }

        public void SetCrystalImage(CrystalStatus crystalStatus)
        {
            switch (crystalStatus)
            {
                case CrystalStatus.Empty:
                    _crystalImage.sprite = EmptyCrystal;
                    break;
                case CrystalStatus.Half:
                    _crystalImage.sprite = HalfCrystal;
                    break;
                case CrystalStatus.Full:
                    _crystalImage.sprite = FullCrystal;
                    break;
                default:
                    break;
            }
        }
    }
}