using Core;
using DG.Tweening;
using Saves;
using UniRx;
using UnityEngine;

namespace Example
{
    public class ExampleObject : BaseDisposable
    {
        public struct Ctx
        {
            public ExampleObjectView exampleObjectView;
            public ReactiveEvent<string> onSpin;
            public GlobalUserData globalUserData;
        }
        public ExampleObject(Ctx ctx)
        {
            _ctx = ctx;
            Init();
        }
        
        private Ctx _ctx;
        //Scale animation settings
        private Vector3 _punchScale = new Vector3(1.1f, 1.1f, 1.1f);
        private float _animationDuration = 0.2f;

        private void Init()
        {
            _ctx.onSpin.Notify($"Spins: {_ctx.globalUserData.spins.Value}");
            
            _ctx.exampleObjectView.transform
                .DORotate(new Vector3(0, 360, 0), 2f, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart)
                .OnStepComplete(OnSpinComplete)
                .SetLink(_ctx.exampleObjectView.gameObject);
        }

        private void OnSpinComplete()
        {
            _ctx.globalUserData.spins.Value++;
            _ctx.onSpin.Notify($"Spins: {_ctx.globalUserData.spins.Value}");
            ScaleAnimation();
        }
        
        private void ScaleAnimation()
        {
            _ctx.exampleObjectView.transform
                .DOPunchScale(_punchScale, _animationDuration, 1)
                .SetLink(_ctx.exampleObjectView.gameObject);
        }
    }
}
