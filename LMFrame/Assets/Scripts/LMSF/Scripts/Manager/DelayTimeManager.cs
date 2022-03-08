using DG.Tweening;
using System;
namespace LMSF.Utils
{
    public class DelayTimeManager:MonoSingleton<DelayTimeManager>
    {
        DelayTimeHelper helper = null;
        public void Init()
        {
            helper = this.gameObject.AddComponent<DelayTimeHelper>();
        }
        public int delay_time_run(float time, Action callback)
        {
            return helper.delay_time_run(time, callback);
        }
        public int delay_time_run_loop(float time, Action callback, int loop_num)
        {
            return helper.delay_time_run_loop(time, callback, loop_num);
        }
        public void cancel_delay_run(int id)
        {
            helper.cancel_delay_run(id);
        }
        public void delay_time_run_without_timescale(float time, Action callback)
        {
            int A = 0;
            Tween tween = DOTween.To(() => A, x => A = x, 1, time).SetUpdate(true).OnComplete(() =>
            {
                callback?.Invoke();
            });
        }

    }
}