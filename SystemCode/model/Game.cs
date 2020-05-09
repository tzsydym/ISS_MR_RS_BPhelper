using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISS_MS_RS_BpHelper
{
    public enum Rotate 
    {
        ourTurn = 0,
        enemyTurn = 1
    }
    public enum BPPhase
    {
        ban_3 = 0,
        pick_3 = 1,
        ban_2 = 2,
        pick_2 = 3
    }
    public enum Lane
    {
        Top,
        Mid,
        Jungle,
        ADCarry,
        Support
    }
    public class Game
    {
        public List<Champion> ChampionPool { get; set; } = new List<Champion>();
        public Instruction instruction { get; set; }
        public Response response { get; set; }
        public Team OurTeam { get; set; } = new Team();
        public Team EnemyTeam { get; set; } = new Team();
        public BPPhase BpPhase { get; set; }
        public bool FirstMove { get; set; }
        public void TeamPick(Team t , Response r)
        {
            for (int i = 0; i < ChampionPool.Count; i++) 
            {
                if (r.ChampionName == ChampionPool[i].Name) 
                {
                    t.PickChampion(ChampionPool[i]);
                    ChampionPool.RemoveAt(i);
                    break;
                }
            }
            
        }
        public void TeamBan(Team t, Response r)
        {
            for (int i = 0; i < ChampionPool.Count; i++)
            {
                if (r.ChampionName == ChampionPool[i].Name)
                {
                    t.BanChampion(ChampionPool[i]);
                    ChampionPool.RemoveAt(i);
                    break;
                }
            }
        }
    }
    public class Team 
    {
        public int teammateToPick { get; set; } = 0;
        public int teammateToBan { get; set; } = 0;
        public List<Lane> lanes { get; set; } = new List<Lane>();
        public List<Champion> TeamMembers { get; set; } = new List<Champion>();
        public List<Champion> BanedChampions { get; set; } = new List<Champion>();
        public void PickChampion(Champion c) 
        {
            TeamMembers.Add(c);
            teammateToPick++;
        }
        public void BanChampion(Champion c) 
        {
            BanedChampions.Add(c);
            teammateToBan++;
        }
        public void UpdateLane(string[] lanes) 
        {
            for (int i = 0; i < lanes.Length; i++) 
            {
                switch (lanes[i]) 
                {
                    case "Top":
                        this.lanes.Add(Lane.Top);
                        break;
                    case "Mid":
                        this.lanes.Add(Lane.Mid);
                        break;
                    case "Support":
                        this.lanes.Add(Lane.Support);
                        break;
                    case "Jungle":
                        this.lanes.Add(Lane.Jungle);
                        break;
                    case "ADCarry":
                        this.lanes.Add(Lane.ADCarry);
                        break;
                }
            }
        }
    }
    public class ChampionToFront 
    {
        public string Name { get; set; }
        public List<Lane> Lanes { get; set; } = new List<Lane>();
        public double Weight { get; set; } = 0;
        public double Assistant { get; set; } = 0;
        public double Counter { get; set; } = 0;
        public double EnemyAfraid { get; set; } = 0;
        public double WinRate { get; set; } = 0.5;
        public double BanRate { get; set; } = 0;
        public ChampionToFront(Champion c) 
        {
            Name = c.Name;
            for (int i = 0; i < c.Lanes.Count; i++) 
            {
                Lanes.Add(c.Lanes[i]); 
            }
            Weight = c.Weight;
            Assistant = c.Assistant;
            Counter = c.Counter;
            EnemyAfraid = c.EnemyAfraid;
            WinRate = c.WinRate;
            BanRate = c.BanRate;
        }
    }
    public class Champion
    {
        public string Name { get; set; }
        public List<Lane> Lanes { get; set; } = new List<Lane>();
        public double Weight { get; set; } = 0;
        public double Assistant { get; set; } = 0;
        public double Counter { get; set; } = 0;
        public double EnemyAfraid { get; set; } = 0;
        public double WinRate { get; set; } = 0.5;
        public double BanRate { get; set; } = 0;
        public CounterInfo counterInfo { get; set; }
        public Champion(Champion c) 
        {
            Name = c.Name;
            for (int i = 0; i < c.Lanes.Count; i++) 
            {
                Lanes.Add(c.Lanes[i]);
            }
            Weight = c.Weight;
            Assistant = c.Assistant;
            Counter = c.Counter;
            EnemyAfraid = c.EnemyAfraid;
            WinRate = c.WinRate;
            BanRate = c.BanRate;
            counterInfo = new CounterInfo(c.counterInfo);
        }
        public Champion() { }
        public double getChampionCounterAdvantage(Champion c_countered)
        {
            double advantage = 0;
            if (counterInfo.BestAgainst.ContainsKey(c_countered.Name)) 
            {
                advantage = counterInfo.BestAgainst[c_countered.Name].Rate;
            }
            return advantage;
        }
        public double getChampionCounteredDisAdvantage(Champion c_counter)
        {
            double disadvantage = 0;
            if (counterInfo.IsCounteredBy.ContainsKey(c_counter.Name))
            {
                disadvantage = counterInfo.IsCounteredBy[c_counter.Name].Rate;
            }
            return disadvantage;
        }
        public double getChampionAssistant(Champion c_assist)
        {
            double assitance = 0;
            if (counterInfo.BestWith.ContainsKey(c_assist.Name))
            {
                assitance = counterInfo.BestWith[c_assist.Name].Rate;
            }
            return assitance;
        }
    }
    public class CounterRelation 
    {
        public string ChampionName { get; set; }
        public Lane Lane { get; set; }
        public double Rate { get; set; }
        public CounterRelation() { }
        public CounterRelation(CounterRelation c) 
        {
            ChampionName = c.ChampionName;
            Lane = c.Lane;
            Rate = c.Rate;
        }
    }
    public class CounterInfo 
    {
        public Dictionary<string, CounterRelation> BestAgainst { get; set; } = new Dictionary<string, CounterRelation>();
        public Dictionary<string, CounterRelation> BestWith { get; set; } = new Dictionary<string, CounterRelation>();
        public Dictionary<string, CounterRelation> IsCounteredBy { get; set; } = new Dictionary<string, CounterRelation>();

        public CounterInfo() { }
        public CounterInfo(CounterInfo c) 
        {
            foreach (KeyValuePair<string, CounterRelation> entry in c.BestAgainst)
            {
                CounterRelation cr = new CounterRelation(entry.Value);
                BestAgainst[entry.Key] = cr;
            }
            foreach (KeyValuePair<string, CounterRelation> entry in c.BestWith)
            {
                CounterRelation cr = new CounterRelation(entry.Value);
                BestWith[entry.Key] = cr;
            }
            foreach (KeyValuePair<string, CounterRelation> entry in c.IsCounteredBy)
            {
                CounterRelation cr = new CounterRelation(entry.Value);
                IsCounteredBy[entry.Key] = cr;
            }
        }
    }
}
