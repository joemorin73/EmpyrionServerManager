using EmpyrionModApi;
using System.Collections.Generic;

namespace VotingRewardMod
{
    public class Configuration
    {
        public string VotingApiServerKey { get; set; }
        public bool Cumulative { get; set; }
        public class VotingReward
        {
            public int MinVotesNeeded { get; set; }
            public ItemStacks Rewards { get; set; }
        }
        public List<VotingReward> VotingRewards { get; set; }
    }
}
