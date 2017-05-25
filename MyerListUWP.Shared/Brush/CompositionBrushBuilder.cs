﻿using Microsoft.Graphics.Canvas.Effects;
using Windows.UI;
using Windows.UI.Composition;

namespace MyerList.Common.Brush
{
    public enum BackdropBrushType
    {
        Backdrop,
        HostBackdrop
    }

    public class CompositionBrushBuilder
    {
        private const string SOURCE_KEY = "Source";

        private float _backdropFactor = 0.5f;
        private float _tintColorFactor = 0.5f;
        private float _blurAmount = 2f;
        private Color _tintColor = Colors.Black;
        private BackdropBrushType _brushType = BackdropBrushType.Backdrop;

        public CompositionBrushBuilder(BackdropBrushType type)
        {
            _brushType = type;
        }

        public CompositionBrushBuilder SetBackdropFactor(float factor)
        {
            _backdropFactor = factor;
            return this;
        }

        public CompositionBrushBuilder SetTintColorFactor(float factor)
        {
            _tintColorFactor = factor;
            return this;
        }

        public CompositionBrushBuilder SetTintColor(Color color)
        {
            _tintColor = color;
            return this;
        }

        public CompositionBrushBuilder SetBlurAmount(float blur)
        {
            _blurAmount = blur;
            return this;
        }

        private CompositionEffectBrush CreateBlurEffect(Compositor compositor)
        {
            var blendEffect0 = new ArithmeticCompositeEffect()
            {
                MultiplyAmount = 0,
                Source1Amount = _backdropFactor,
                Source2Amount = _tintColorFactor,
                Source1 = new CompositionEffectSourceParameter(SOURCE_KEY),
                Source2 = new ColorSourceEffect()
                {
                    Color = _tintColor
                }
            };

            var effect = new GaussianBlurEffect()
            {
                BlurAmount = _blurAmount,
                BorderMode = EffectBorderMode.Soft,
                Source = blendEffect0
            };

            var effectFactory = compositor.CreateEffectFactory(effect);
            var effectBrush = effectFactory.CreateBrush();
            return effectBrush;
        }

        public CompositionEffectBrush Build(Compositor compositor)
        {
            var effectBrush = CreateBlurEffect(compositor);
            CompositionBackdropBrush backdropBrush;
            switch (_brushType)
            {
                case BackdropBrushType.Backdrop:
                    backdropBrush = compositor.CreateBackdropBrush();
                    break;
                case BackdropBrushType.HostBackdrop:
                default:
                    backdropBrush = compositor.CreateHostBackdropBrush();
                    break;
            }
            effectBrush.SetSourceParameter(SOURCE_KEY, backdropBrush);
            return effectBrush;
        }
    }
}
