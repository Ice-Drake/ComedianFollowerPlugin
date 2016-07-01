using System;
using PluginSDK;

namespace FollowerPlugins
{
    public class GreetingState : BotState
    {
        public GreetingState() : base("Greeting")
        {

        }

        public override void doAction(Comedian bot)
        {
            lock (bot.OutgoingMessage)
            {
                bot.OutgoingMessage.Enqueue(new ChatMessage(bot.PSource, bot.retrieve(bot.PSource)[0], bot.PSource.MasterAccount, new TextMessage(getAIMLResponse("SayStatementPattern hello", getPrimaryUser(bot), bot))));
            }

            bot.setNextState(StateFactory.getBotState("Available", bot));
        }
    }
}
