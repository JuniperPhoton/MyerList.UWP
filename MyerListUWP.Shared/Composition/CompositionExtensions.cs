using MyerListUWP.Shared.Util;
using System;
using System.Numerics;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Hosting;

namespace MyerListUWP.Common.Composition
{
    public static class CompositionExtensions
    {
        private const string TRANSLATION = "Translation";
        private const string OFFSET = "Offset";

        public static Visual GetVisual(this UIElement element)
        {
            var visual = ElementCompositionPreview.GetElementVisual(element);
            if (DeviceUtil.IsRS2OS)
            {
                ElementCompositionPreview.SetIsTranslationEnabled(element, true);
                visual.Properties.InsertVector3(TRANSLATION, Vector3.Zero);
            }
            return visual;
        }

        public static CompositionAnimationBuilder StartBuildAnimation(this Visual visual)
        {
            return new CompositionAnimationBuilder(visual);
        }

        public static void SetTranslation(this Visual set, Vector3 value)
        {
            if (DeviceUtil.IsRS2OS)
            {
                set.Properties.InsertVector3(TRANSLATION, value);
            }
            else
            {
                set.Offset = value;
            }
        }

        public static Vector3 GetTranslation(this Visual visual)
        {
            if (DeviceUtil.IsRS2OS)
            {
                visual.Properties.TryGetVector3(TRANSLATION, out Vector3 value);
                return value;
            }
            else
            {
                return visual.Offset;
            }
        }

        public static string GetTranslationPropertyName(this Visual visual)
        {
            return AnimateProperties.Translation.GetPropertyValue();
        }

        public static string GetTranslationXPropertyName(this Visual visual)
        {
            return AnimateProperties.TranslationX.GetPropertyValue();
        }

        public static string GetTranslationYPropertyName(this Visual visual)
        {
            return AnimateProperties.TranslationY.GetPropertyValue();
        }

        public static string GetPropertyValue(this AnimateProperties property)
        {
            switch (property)
            {
                case AnimateProperties.Translation:
                    if (DeviceUtil.IsRS2OS)
                    {
                        return TRANSLATION;
                    }
                    else return OFFSET;
                case AnimateProperties.TranslationX:
                    if (DeviceUtil.IsRS2OS)
                    {
                        return $"{TRANSLATION}.X";
                    }
                    else return $"{OFFSET}.X"; ;
                case AnimateProperties.TranslationY:
                    if (DeviceUtil.IsRS2OS)
                    {
                        return $"{TRANSLATION}.Y";
                    }
                    else return $"{OFFSET}.Y"; ;
                case AnimateProperties.Opacity:
                    return "Opacity";
                case AnimateProperties.RotationAngleInDegrees:
                    return "RotationAngleInDegrees";
                default:
                    throw new ArgumentException("Unknown properties");
            }
        }
    }
}
