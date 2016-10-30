using System;
using PluginSDK;

namespace ComedianFollowerPlugin
{
    public class SleepState : BotState
    {
        public SleepState() : base("Sleep")
        {

        }

        public override void doAction(Comedian bot)
        {
            if ((bot.WakeTime > bot.SleepTime && (DateTime.Now.TimeOfDay >= bot.WakeTime || DateTime.Now.TimeOfDay < bot.SleepTime)) || (bot.WakeTime < bot.SleepTime && (DateTime.Now.TimeOfDay >= bot.WakeTime && DateTime.Now.TimeOfDay < bot.SleepTime)))
            {
                bot.setNextState(StateFactory.getBotState("Greeting", bot));
            }
        }
    }
}
