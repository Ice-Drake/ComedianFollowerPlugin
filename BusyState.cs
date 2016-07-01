using System;
using System.Collections.Generic;
using PluginSDK;

namespace FollowerPlugins
{
    public class BusyState : BotState
    {
        private int workCounter;

        public BusyState() : base("Busy")
        {
            workCounter = 0;
        }

        public override void doAction(Comedian bot)
        {
            workCounter = ++workCounter % 2700;

            //Time to get off work?
            if ((bot.OffWorkTime > bot.WorkTime && (DateTime.Now.TimeOfDay >= bot.OffWorkTime || DateTime.Now.TimeOfDay < bot.WorkTime)) || (bot.OffWorkTime < bot.WorkTime && (DateTime.Now.TimeOfDay >= bot.OffWorkTime && DateTime.Now.TimeOfDay < bot.WorkTime)))
            {
                bot.setNextState(StateFactory.getBotState("Available", bot));
            }
            //Time for a break from work
            else if (workCounter == 0) //90 minutes elapsed
            {
                if (new Random().Next(0,2) == 0)
                    bot.setNextState(StateFactory.getBotState("Social", bot));
                else
                    bot.setNextState(StateFactory.getBotState("Rest", bot));
            }
        }
    }
}
