using System;
using System.Collections.Generic;
using System.Drawing;
using System.Timers;
using System.IO;
using AIMLbot;
using ChatModule;
using PluginSDK;

namespace ComedianFollowerPlugin
{
    public enum BotStatus { Available, Busy, Sleep };

    [PluginAttribute(AuthorName = "Que Trac", PluginName = "Comedian", PluginVersion = "1.0")]
    public class Comedian : Bot, IFollowerPlugin
    {
        public string AuthorName { get; private set; }

        public string PluginName { get; private set; }

        public string PluginVersion { get; private set; }

        private SortedList<string, UserAccount> accounts;

        private BotStatus status;

        public BotStatus Status
        {
            get
            {
                return status;
            }
            protected set
            {
                if (status != value)
                {
                    status = value;

                    foreach (UserAccount account in accounts.Values)
                    {
                        switch (status)
                        {
                            case BotStatus.Busy:
                                account.Status = ChatStatus.Busy;
                                break;
                            case BotStatus.Sleep:
                                account.Status = ChatStatus.Offline;
                                break;
                            default:
                                account.Status = ChatStatus.Available;
                                break;
                        }
                    }
                }
            }
        }

        public Color FontColor { get; protected set; }

        public IProfile Profile { get; private set; }

        public string Nickname { get; private set; }

        public IChatSource PSource { get; private set; }

        public Queue<ChatMessage> OutgoingMessage { get; private set; }

        public string Role { get; private set; }

        public SortedList<string, IChatSource> Sources { get; private set; }
        public SortedList<string, User> Users { get; private set; }

        public TimeSpan WakeTime { get; private set; }
        public TimeSpan SleepTime { get; private set; }
        public TimeSpan WorkTime { get; private set; }
        public TimeSpan OffWorkTime { get; private set; }

        protected BotState currentState;
        private BotState nextState;
        private Timer stateTimer;

        public Comedian()
        {
            initialize();

            AuthorName = "Que Trac";
            PluginName = "Comedian Follower Plugin";
            PluginVersion = "1.0";
            FontColor = Color.Green;
            Users = new SortedList<string, User>();

            accounts = new SortedList<string, UserAccount>();

            OutgoingMessage = new Queue<ChatMessage>();

            stateTimer = new Timer();
            stateTimer.Interval = 2000; // 2 seconds
            stateTimer.Elapsed += new ElapsedEventHandler(stateTimer_Elapsed);

            Sources = new SortedList<string, IChatSource>();

            Profile = new Profile(GlobalSettings.grabSetting("name"));
            WakeTime = Convert.ToDateTime(GlobalSettings.grabSetting("waketime")).TimeOfDay;
            SleepTime = Convert.ToDateTime(GlobalSettings.grabSetting("sleeptime")).TimeOfDay;
            WorkTime = Convert.ToDateTime(GlobalSettings.grabSetting("worktime")).TimeOfDay;
            OffWorkTime = Convert.ToDateTime(GlobalSettings.grabSetting("offworktime")).TimeOfDay;

            status = BotStatus.Sleep;
            currentState = BotState.StateFactory.getBotState("Initiate", this);
        }

        public void loadDatabase()
        {

        }

        public void setNextState(BotState newState)
        {
            nextState = newState;
        }

        public void start()
        {
            stateTimer.Start();
        }

        public void add(UserAccount account)
        {
            accounts.Add(account.SourceName, account);
        }

        public IList<UserAccount> retrieve(IChatSource source)
        {
            //Currently, handling one account per chat source.
            List<UserAccount> list = new List<UserAccount>();
            list.Add(accounts[source.SourceName]);
            return list;
        }

        public void join(IChatSource source)
        {
            if (!Sources.ContainsKey(source.SourceName))
            {
                if (PSource == null)
                    PSource = source;
                Sources.Add(source.SourceName, source);
            }
        }

        public void listen(ChatMessage message)
        {
            string response;

            //New Chat Sender?
            if (message.Sender.Owner != null)
            {
                if (!Users.ContainsKey(message.Sender.Owner.Profile.Name))
                    Users.Add(message.Sender.Owner.Profile.Name, new User(message.Sender.Owner.Profile.Name, this));
                response = currentState.getAIMLResponse(message.Content.Message, Users[message.Sender.Owner.Profile.Name], this);
            }
            else
            {
                if (!Users.ContainsKey(message.Sender.Username))
                    Users.Add(message.Sender.Username, new User(message.Sender.Username, this));
                response = currentState.getAIMLResponse(message.Content.Message, Users[message.Sender.Username], this);
            }

            if (!response.Equals(""))
            {
                lock (OutgoingMessage)
                {
                    OutgoingMessage.Enqueue(new ChatMessage(message.Source, message.Recipient, message.Sender, new TextMessage(response)));
                }
            }
        }

        public void receive(ChatMessage message)
        {
            if (Status != BotStatus.Sleep)
            {
                string response;

                //New Chat Sender?
                if (message.Sender.Owner != null)
                {
                    if (!Users.ContainsKey(message.Sender.Owner.Profile.Name))
                        Users.Add(message.Sender.Owner.Profile.Name, new User(message.Sender.Owner.Profile.Name, this));
                    response = currentState.getAIMLResponse(message.Content.Message, Users[message.Sender.Owner.Profile.Name], this);
                }
                else
                {
                    if (!Users.ContainsKey(message.Sender.Username))
                        Users.Add(message.Sender.Username, new User(message.Sender.Username, this));
                    response = currentState.getAIMLResponse(message.Content.Message, Users[message.Sender.Username], this);
                }

                if (!response.Equals(""))
                {
                    lock (OutgoingMessage)
                    {
                        OutgoingMessage.Enqueue(new ChatMessage(message.Source, message.Recipient, message.Sender, new TextMessage(response)));
                    }
                }

                if (!(currentState is AvailableState))
                    nextState = BotState.StateFactory.getBotState("Available", this);
                else
                    ((AvailableState)currentState).resetAttentionCounter();
            }
        }

        /// <summary>
        /// Loads AIML files located in the AIML folder.
        /// </summary>
        private void initialize()
        {
            try
            {
                string path = Path.Combine(Environment.CurrentDirectory, Path.Combine("config", "ComedianSettings.xml"));
                loadSettings(path);
                isAcceptingUserInput = false;
                loadAIMLFromFiles();
                isAcceptingUserInput = true;
            }
            catch (FileNotFoundException ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                //Terminate program
            }
        }

        private void stateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            stateLoop();
        }

        private void stateLoop()
        {
            if (nextState != null)
            {
                if (nextState is BusyState)
                    Status = BotStatus.Busy;
                else if (nextState is SleepState)
                    Status = BotStatus.Sleep;
                else
                    Status = BotStatus.Available;

                currentState = nextState;
                nextState = null;
            }
            currentState.doAction(this);
        }
    }
}
