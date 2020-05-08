using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISS_MS_RS_BpHelper
{
    public interface IRule
    {
        bool Validate();
    }
    class InstructionTokenRule : IRule
    {
        private readonly Game game;
        public InstructionTokenRule(Game game)
        {
            this.game = game;
        }
        public bool Validate()
        {
            Team team = null;
            try
            {
                switch (game.instruction.rotate)
                {
                    case Rotate.ourTurn:
                        team = game.OurTeam;
                        break;
                    case Rotate.enemyTurn:
                        team = game.EnemyTeam;
                        break;
                }
                switch (game.instruction.bPPhase)
                {
                    case BPPhase.pick_3:
                    case BPPhase.pick_2:
                        game.TeamPick(team, game.response);
                        break;
                    case BPPhase.ban_3:
                    case BPPhase.ban_2:
                        game.TeamBan(team, game.response);
                        break;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
    class BPEndRule : IRule
    {
        private readonly Game game;
        public BPEndRule(Game game)
        {
            this.game = game;
        }
        public bool Validate()
        {
            if (game.OurTeam.teammateToPick == 5 && game.EnemyTeam.teammateToPick == 5)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    class RecommendRule : IRule 
    {
        private readonly Game game;
        public RecommendRule(Game game)
        {
            this.game = game;
        }
        public bool Validate() 
        {
            game.ChampionPool = game.ChampionPool.OrderByDescending(c => c.Weight).ToList();
            for (int i = 0; i < StaticValue.recommendCount; i++) 
            {
                ChampionToFront cf = new ChampionToFront(game.ChampionPool[i]);
                game.instruction.recommendedChampions.Add(cf);
            }
            return true;
        }
    }
    class TeammateRule : IRule 
    {
        private readonly Game game;
        public TeammateRule(Game game)
        {
            this.game = game;
        }
        public bool Validate() 
        {
            if (game.BpPhase == BPPhase.ban_2 || game.BpPhase == BPPhase.ban_3)
            {
                if (game.instruction.rotate == Rotate.ourTurn)
                {
                    game.instruction.teammateID = game.OurTeam.teammateToBan;
                }
                else
                {
                    game.instruction.teammateID = game.EnemyTeam.teammateToBan;
                }
            }
            else
            {
                if (game.instruction.rotate == Rotate.enemyTurn)
                {
                    game.instruction.teammateID = game.EnemyTeam.teammateToPick;
                }
                else 
                {
                    game.instruction.teammateID = game.OurTeam.teammateToPick;
                }
            }
            return true;
        }
    }
    class UpdateChampionPoolWeightRule : IRule
    {
        private readonly Game game;
        public UpdateChampionPoolWeightRule(Game game)
        {
            this.game = game;
        }
        public bool Validate()
        {
            try
            {
                if (game.BpPhase.ToString().Contains("pick"))
                {
                    UpdateChampionWeight_pick();
                }
                else
                {
                    UpdateChampionWeight_ban();
                }
                game.instruction.fillChampionToFrontList(game.ChampionPool);
                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }
        public void UpdateChampionWeight_pick()
        {
            for (int j = 0; j < game.ChampionPool.Count; j++)
            {
                Champion champion = game.ChampionPool[j];
                double assistant_winrate = 0;
                for (int i = 0; i < game.OurTeam.TeamMembers.Count; i++)
                {
                    assistant_winrate += champion.getChampionAssistant(game.OurTeam.TeamMembers[i]);
                }

                double counter_winrate = 0;
                for (int i = 0; i < game.EnemyTeam.TeamMembers.Count; i++)
                {
                    counter_winrate += champion.getChampionCounterAdvantage(game.EnemyTeam.TeamMembers[i]);//how much enemy champions are countered by the champion
                }

                double countered_disadvantage = 0;
                for (int i = 0; i < game.EnemyTeam.TeamMembers.Count; i++)
                {
                    countered_disadvantage += champion.getChampionCounteredDisAdvantage(game.EnemyTeam.TeamMembers[i]);//how much the champion is countered by enemy champions
                }
                double counterWeight = counter_winrate + countered_disadvantage;
                champion.Counter = counterWeight;
                champion.Assistant = assistant_winrate;
                champion.Weight = champion.WinRate + assistant_winrate + counterWeight;
            }

        }
        public void UpdateChampionWeight_ban()
        {
            for (int j = 0; j < game.ChampionPool.Count; j++)
            {
                Champion champion = game.ChampionPool[j];
                double enemy_fraid_rate = 0;
                //enemy doesn't wish we use these champions, so raise the weight of champions who owns disadvantages against the enemy baned champions.
                //and these disadvantaged champions maybe enemy team's next pick.
                for (int i = 0; i < game.EnemyTeam.BanedChampions.Count; i++)
                {
                    enemy_fraid_rate += champion.getChampionCounterAdvantage(game.EnemyTeam.BanedChampions[i]);
                }
                double assistant_weight = 0;
                //asistant_winrate for enemy team higher, weight should be higher
                for (int i = 0; i < game.EnemyTeam.TeamMembers.Count; i++)
                {
                    assistant_weight += champion.getChampionAssistant(game.EnemyTeam.TeamMembers[i]);
                }
                //champion counter our team the most, weight should be higher.
                double ourTeam_countered_weight = 0;
                for (int i = 0; i < game.OurTeam.TeamMembers.Count; i++)
                {
                    ourTeam_countered_weight += champion.getChampionCounterAdvantage(game.OurTeam.TeamMembers[i]);
                }
                double enemyTeam_disadvantage_weight = 0;
                for (int i = 0; i < game.EnemyTeam.TeamMembers.Count; i++)
                {
                    enemyTeam_disadvantage_weight += champion.getChampionCounteredDisAdvantage(game.OurTeam.TeamMembers[i]);
                }
                double counter_weight = ourTeam_countered_weight + enemyTeam_disadvantage_weight;
                champion.EnemyAfraid = enemy_fraid_rate;
                champion.Assistant = assistant_weight;
                champion.Counter = counter_weight;
                champion.Weight = champion.BanRate + enemy_fraid_rate + assistant_weight + counter_weight;
            }
        }
    }
}
