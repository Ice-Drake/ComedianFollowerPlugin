using System;
using System.Collections.Generic;

namespace ComedianFollowerPlugin
{
    public class FlyweightStateFactory
    {
        private SortedList<string, BotState> flyweights;

        public FlyweightStateFactory()
        {
            flyweights = new SortedList<string, BotState>();
        }

        public BotState getBotState(string stateName, Comedian bot)
        {
            if (flyweights.ContainsKey(stateName))
                return flyweights[stateName];
            //if Flyweight is not found
            else
            {
                BotState newState;
                
                switch (stateName)
                {
                    case "Initiate":
                        newState = new InitiateState();
                        flyweights.Add(stateName, newState);
                        break;
                    case "Greeting":
                        newState = new GreetingState();
                        flyweights.Add(stateName, newState);
                        break;
                    case "Available":
                        newState = new AvailableState();
                        flyweights.Add(stateName, newState);
                        break;
                    case "Busy":
                        newState = new BusyState();
                        flyweights.Add(stateName, newState);
                        break;
                    case "Rest":
                        newState = new RestState();
                        flyweights.Add(stateName, newState);
                        break;
                    case "Sleep":
                        newState = new SleepState();
                        flyweights.Add(stateName, newState);
                        break;
                    default:
                        newState = null;
                        break;
                }

                return newState;
            }
        }
    }
}
