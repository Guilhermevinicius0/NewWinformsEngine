using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinformsEngine.Engine.Systems.General
{
    public static class Scheduler
    {
        //Debris
        public static void DestroyAfter(Scene.Scene scene, Entities.Entity entity, float seconds) 
        {
            var timer = new CountDownTimer(scene, seconds, () => entity?.Destroy());
            timer.Start();
        }

        //Debris
        public static CountDownTimer DestroyAfterWithTimer(Scene.Scene scene, Entities.Entity entity, float seconds)
        {
            var timer = new CountDownTimer(scene, seconds, () => entity?.Destroy());
            timer.Start();
            return timer;
        }

        //CallBack Schedule
        public static void DoAfter(Scene.Scene scene, float seconds, Action action) 
        {
            var timer = new CountDownTimer(scene, seconds, action);
            timer.Start();
        }

        //CallBack Schedule
        public static CountDownTimer DoAfterWithTImer(Scene.Scene scene, float seconds, Action action)
        {
            var timer = new CountDownTimer(scene, seconds, action);
            timer.Start();
            return timer;
        }
    }
}
