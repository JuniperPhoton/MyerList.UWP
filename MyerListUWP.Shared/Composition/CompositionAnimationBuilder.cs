using System;
using System.Numerics;
using Windows.Foundation;
using Windows.UI.Composition;

namespace MyerListUWP.Common.Composition
{
    public class CompositionAnimationBuilder
    {
        private Visual _visusal;
        private AnimateProperties _property;

        private float _durationMillis;
        private float _delayMillis;

        private Vector3 _vector3Value;
        private float _scalarValue;
        private AnimationType _type;

        public event TypedEventHandler<object, CompositionBatchCompletedEventArgs> OnCompleted;

        public CompositionAnimationBuilder(Visual visual) => _visusal = visual;

        public CompositionAnimationBuilder Animate(AnimateProperties property)
        {
            _property = property;
            return this;
        }

        public CompositionAnimationBuilder To(Vector3 vector)
        {
            _type = AnimationType.Vector3;
            _vector3Value = vector;
            return this;
        }

        public CompositionAnimationBuilder To(float value)
        {
            _type = AnimationType.Scalar;
            _scalarValue = value;
            return this;
        }

        public CompositionAnimationBuilder Delay(float durationMillis)
        {
            _delayMillis = durationMillis;
            return this;
        }

        public CompositionAnimationBuilder Spend(int durationMillis)
        {
            _durationMillis = durationMillis;
            return this;
        }

        public CompositionAnimationBuilder Start()
        {
            var comp = _visusal.Compositor;
            KeyFrameAnimation animation;
            switch (_type)
            {
                case AnimationType.Scalar:
                    animation = comp.CreateScalarKeyFrameAnimation();
                    (animation as ScalarKeyFrameAnimation).InsertKeyFrame(1f, _scalarValue);
                    break;
                case AnimationType.Vector3:
                    animation = comp.CreateVector3KeyFrameAnimation();
                    (animation as Vector3KeyFrameAnimation).InsertKeyFrame(1f, _vector3Value);
                    break;
                default:
                    throw new ArgumentException("Unknown animation type");
            }
            animation.Duration = TimeSpan.FromMilliseconds(_durationMillis);
            animation.DelayTime = TimeSpan.FromMilliseconds(_delayMillis);

            var batch = comp.CreateScopedBatch(CompositionBatchTypes.Animation);
            _visusal.StartAnimation(_property.GetPropertyValue(), animation);
            batch.Completed += Batch_Completed;
            batch.End();

            return this;
        }

        private void Batch_Completed(object sender, CompositionBatchCompletedEventArgs args)
        {
            OnCompleted?.Invoke(sender, args);
        }

        private enum AnimationType
        {
            Vector3,
            Scalar
        }
    }

    public enum AnimateProperties
    {
        Translation,
        TranslationY,
        TranslationX,
        Opacity,
        RotationAngleInDegrees
    }
}
