using System;
using UnityEngine;
using UnityEngine.UI;

namespace Turtinia.UI
{
    public enum HeartStatus
    {
        Empty = 0,
        Half = 1,
        Full = 2
    }
    
    public class HealthHeart : MonoBehaviour
    {
        public Sprite FullHeart, HalfHeart, EmptyHeart;
        private Image _heartImage;

        private void Awake()
        {
            _heartImage = GetComponent<Image>();
        }

        public void SetHeartImage(HeartStatus status)
        {
            switch (status)
            {
                case HeartStatus.Empty:
                    _heartImage.sprite = EmptyHeart;
                    break;
                case HeartStatus.Half:
                    _heartImage.sprite = HalfHeart;
                    break;
                case HeartStatus.Full:
                    _heartImage.sprite = FullHeart;
                    break;
                default:
                    break;
            }
        }
    }
}