
    public struct Agent
    {
        //use classes? or ... i don't love this way to store stats....
        public AgentType AgentType;
        public int? Health;
        public int? Attack;
        public int? Reward;

        public Agent(AgentType agentType)
        {
            AgentType = agentType;
            Health = null;
            Attack = null;
            Reward = null;
        }
    }
    