using ChatModule;

namespace ComedianFollowerPlugin
{
    public class RestState : BotState
    {
        private int restCounter;

        public RestState() : base("Rest")
        {
            restCounter = 0;
        }

        public override void doAction(Comedian bot)
        {
            restCounter = ++restCounter % 300;

            if (restCounter == 0) //10 minutes elapsed
            {
                //Say Back to Work
                lock (bot.OutgoingMessage)
                {
                    bot.OutgoingMessage.Enqueue(new ChatMessage(bot.PSource, bot.retrieve(bot.PSource)[0], bot.PSource.MasterAccount, new TextMessage(getAIMLResponse("SayStatement Back to work", getPrimaryUser(bot), bot))));
                }
                bot.setNextState(StateFactory.getBotState("Busy", bot));
            }
        }
    }
}
