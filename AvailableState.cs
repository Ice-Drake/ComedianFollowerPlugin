using System;
using ChatModule;

namespace ComedianFollowerPlugin
{
    public class AvailableState : BotState
    {
        private int attentionCounter;

        public AvailableState() : base("Available")
        {
            attentionCounter = 0;
        }

        public override void doAction(Comedian bot)
        {
            //Go to Sleep State if bedtime
            if ((bot.WakeTime > bot.SleepTime && (DateTime.Now.TimeOfDay < bot.WakeTime && DateTime.Now.TimeOfDay >= bot.SleepTime)) || (bot.WakeTime < bot.SleepTime && (DateTime.Now.TimeOfDay < bot.WakeTime || DateTime.Now.TimeOfDay >= bot.SleepTime)))
            {
                attentionCounter = ++attentionCounter % 4;

                if (attentionCounter == 0) //8 seconds elapsed
                {
                    //Say Good Night
                    lock (bot.OutgoingMessage)
                    {
                        bot.OutgoingMessage.Enqueue(new ChatMessage(bot.PSource, bot.retrieve(bot.PSource)[0], bot.PSource.MasterAccount, new TextMessage(getAIMLResponse("SayStatementPattern sleepy", getPrimaryUser(bot), bot))));
                    }
                    bot.setNextState(StateFactory.getBotState("Sleep", bot));
                }
            }
            else if ((bot.OffWorkTime > bot.WorkTime && (DateTime.Now.TimeOfDay < bot.OffWorkTime && DateTime.Now.TimeOfDay >= bot.WorkTime)) || (bot.OffWorkTime < bot.WorkTime && (DateTime.Now.TimeOfDay < bot.OffWorkTime || DateTime.Now.TimeOfDay >= bot.WorkTime)))
            {
                attentionCounter = ++attentionCounter % 30;

                if (attentionCounter == 0) //1 minute elapsed
                {
                    //Say Time for Work
                    lock (bot.OutgoingMessage)
                    {
                        bot.OutgoingMessage.Enqueue(new ChatMessage(bot.PSource, bot.retrieve(bot.PSource)[0], bot.PSource.MasterAccount, new TextMessage(getAIMLResponse("SayStatementPattern time for work", getPrimaryUser(bot), bot))));
                    }
                    bot.setNextState(StateFactory.getBotState("Busy", bot));
                }
            }
        }

        public void resetAttentionCounter()
        {
            attentionCounter = 0;
        }
    }
}
