using DG.Tweening;
using System;
namespace LMSF.Utils
{
    public static class DelayTimeUtils
    {
        public static int delay_time_run(float time, Action callback)
        {
            return DelayTimeHelper.Instance.delay_time_run(time, callback);
        }
        public static int delay_time_run_loop(float time, Action callback, int loop_num)
        {
            return DelayTimeHelper.Instance.delay_time_run_loop(time, callback, loop_num);
        }
        public static void cancel_delay_run(int id)
        {
            DelayTimeHelper.Instance.cancel_delay_run(id);
        }
        public static void delay_time_run_without_timescale(float time, Action callback)
        {
            int A = 0;
            Tween tween = DOTween.To(() => A, x => A = x, 1, time).SetUpdate(true).OnComplete(() =>
            {
                callback?.Invoke();
            });
        }

    }
}