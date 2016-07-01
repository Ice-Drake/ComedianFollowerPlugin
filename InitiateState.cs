using System;
using PluginSDK;

namespace FollowerPlugins
{
    public class InitiateState : BotState
    {
        public InitiateState() : base("Initiate")
        {

        }

        public override void doAction(Comedian bot)
        {
            if ((bot.WakeTime > bot.SleepTime && (DateTime.Now.TimeOfDay < bot.WakeTime && DateTime.Now.TimeOfDay >= bot.SleepTime)) || (bot.WakeTime < bot.SleepTime && (DateTime.Now.TimeOfDay < bot.WakeTime || DateTime.Now.TimeOfDay >= bot.SleepTime)))
            {
                bot.setNextState(StateFactory.getBotState("Sleep", bot));
            }
            else
            {
                bot.setNextState(StateFactory.getBotState("Greeting", bot));
            }
        }
    }
}
