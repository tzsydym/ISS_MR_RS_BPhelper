using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISS_MS_RS_BpHelper
{
    public abstract class Decision
    {
        public abstract void Evaluate(Game game);
    }
    public class DecisionQuery : Decision
    {
        public string Title { get; set; }
        public Decision Positive { get; set; }
        public Decision Negative { get; set; }
        public Func<Game, bool> Test { get; set; }
        public override void Evaluate(Game game)
        {
            bool result = this.Test(game);
            if (result) this.Positive.Evaluate(game);
            else this.Negative.Evaluate(game);
        }
    }
    public class BPPhaseDecisionQuery : DecisionQuery
    {
        public static BPPhaseDecisionQuery BPPhaseDecisionTree()
        {
            //Decision 3
            var IsPick_2Branch = new BPPhaseDecisionQuery
            {
                Title = "Is pick_2 phase",
                Test = (game) => (game.OurTeam.TeamMembers.Count + game.EnemyTeam.TeamMembers.Count < StaticValue.pick_2PhaseCount),
                Positive = new BPPhaseDecisionResult { bPPhase = BPPhase.pick_2 },
                Negative = new BPPhaseDecisionResult { bPPhase = BPPhase.pick_2 }
            };


            //Decision 2
            var IsBan_2Branch = new BPPhaseDecisionQuery
            {
                Title = "Is ban_2 phase",
                Test = (game) => (game.OurTeam.BanedChampions.Count + game.EnemyTeam.BanedChampions.Count) < StaticValue.ban_2PahseCount,
                Positive = new BPPhaseDecisionResult { bPPhase = BPPhase.ban_2 },
                Negative = IsPick_2Branch
            };

            //Decision 1
            var IsPick_3Branch = new BPPhaseDecisionQuery
            {
                Title = "Is pick_3 phase",
                Test = (game) => (game.OurTeam.TeamMembers.Count + game.EnemyTeam.TeamMembers.Count < StaticValue.pick_3PhaseCount),
                Positive = new BPPhaseDecisionResult { bPPhase = BPPhase.pick_3},
                Negative = IsBan_2Branch
            };

            //Decision 0
            var trunk = new BPPhaseDecisionQuery
            {
                Title = "Is ban_3 phase",
                Test = (game) => (game.OurTeam.BanedChampions.Count + game.EnemyTeam.BanedChampions.Count) < StaticValue.ban_3PhaseCount,
                Positive = new BPPhaseDecisionResult { bPPhase = BPPhase.ban_3 },
                Negative = IsPick_3Branch
            };

            return trunk;
        }
    }
    public class BPPhaseDecisionResult : Decision
    {
        public static BPPhase BPPhase { get; set; }
        public BPPhase bPPhase { get; set; }
        public override void Evaluate(Game game)
        {
            BPPhase = bPPhase;
        }
    }
    public class RoatetDecisionQuery : DecisionQuery 
    {
        public static RoatetDecisionQuery RotateDecisionTree() 
        {
            //Decision 0
            var trunk = new RoatetDecisionQuery
            {
                Title = "Is enemy team turn",
                Test = (game) => (game.OurTeam.BanedChampions.Count + game.EnemyTeam.BanedChampions.Count
                + game.OurTeam.TeamMembers.Count + game.EnemyTeam.TeamMembers.Count) % 2== 0 ^ game.FirstMove,
                Positive = new RotateDecisionResult { rotate = Rotate.enemyTurn },
                Negative = new RotateDecisionResult { rotate = Rotate.ourTurn }
            };

            return trunk;
        }
    }
    public class RotateDecisionResult : Decision 
    {
        public static Rotate Rotate { get; set; }
        public Rotate rotate { get; set; }
        public override void Evaluate(Game game)
        {
            Rotate = rotate;
        }
    }
}
