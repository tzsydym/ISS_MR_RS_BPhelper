using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISS_MS_RS_BpHelper
{
    public class Instruction
    {
        public BPPhase bPPhase { get; set; }
        public Rotate rotate { get; set; }
        public int teammateID { get; set; }
        public List<ChampionToFront> champions { get; set; } = new List<ChampionToFront>();
        public List<ChampionToFront> recommendedChampions { get; set; } = new List<ChampionToFront>();
        public void fillChampionToFrontList(List<Champion> champions) 
        {
            for (int i = 0; i < champions.Count; i++) 
            {
                this.champions.Add(new ChampionToFront(champions[i]));
            }
        }
    }
    public class Response 
    {
        public string ChampionName { get; set; }
    }
}
