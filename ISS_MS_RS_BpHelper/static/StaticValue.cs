using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ISS_MS_RS_BpHelper
{
    public static class StaticValue
    {
        public static List<Champion> allChampions = new List<Champion>();
        public const string user_agent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/65.0.3325.181 Safari/537.36";
        public const string jsToCsharpConverterName = "objDelivery";
        public const string csvSavingFolder = "crawler data";
        public const string lolDataWebsiteDomain = @"https://www.leagueofgraphs.com";
        public const string lolDataDefaultUrl = @"https://www.leagueofgraphs.com/champions/builds/sr-ranked/by-winrate";//ALL region, platinum plus, rank only
        public const string csv_overallChampionsInfo = "overallChampionsInfo.csv";
        public const string csv_championCounter = "championCounter.csv";
        public const int ban_3PhaseCount = 6;
        public const int pick_3PhaseCount = 6;
        public const int ban_2PahseCount = 10;
        public const int pick_2PhaseCount = 10;
        public const int recommendCount = 8;
        public static List<Champion> getCopyOfAllChampions() 
        {
            List<Champion> champions = new List<Champion>();
            for (int i = 0; i < allChampions.Count; i++) 
            {
                Champion c = new Champion(allChampions[i]);
                champions.Add(c);
            }
            return champions;
        }
        public static Lane mapLane(string lane) 
        {
            switch (lane) 
            {
                case "Top":
                    return Lane.Top;
                case "Mid":
                    return Lane.Mid;
                case "Jungle":
                    return Lane.Jungle;
                case "ADCarry":
                    return Lane.Top;
                case "Support":
                    return Lane.Support;
                default:
                    return Lane.Support;
            }
        }
        public static string mapRegionNameAndRankNameToUrlPara(string name) 
        {
            switch (name) 
            {
                case "IRON":
                    return Rank.IRON;
                case "BRONZE":
                    return Rank.BRONZE;
                case "SILVER":
                    return Rank.SILVER;
                case "GOLD":
                    return Rank.GOLD;
                case "PLATINUM":
                    return Rank.PLATINUM;
                case "DIAMOND":
                    return Rank.DIAMOND;
                case "MASTER":
                    return Rank.MASTER;
                case "All":
                    return Region.All;
                case "Europe West":
                    return Region.EuropeWest;
                case "Europe Nordic":
                    return Region.EuropeNordic;
                case "Brazil":
                    return Region.Brazil;
                case "Latin America North":
                    return Region.LatinAmericaNorth;
                case "Latin America South":
                    return Region.LatinAmericaSouth;
                case "North America":
                    return Region.NorthAmerica;
                case "Oceania":
                    return Region.Oceania;
                case "Russia":
                    return Region.Russia;
                case "Turkey":
                    return Region.Turkey;
                case "Japan":
                    return Region.Japan;
                case "Korea":
                    return Region.Korea;
                default:return "";
            }
        }
        public static string getRegionRankDirectory(string region,string rank) 
        {
            string path = string.Format("{0}\\{1}\\{2}", csvSavingFolder, region,rank);           
            return path;
        }
        public static string getChampionAvatarUrl(string name) 
        {
            return string.Format("../IMG/champion-avatar/" + name + ".jpg");
        }
        public static string getChampionCounterbyRegionRankUrl(string name , string region, string rank) 
        {
            string region_urlpara = "", rank_urlpara = "";
            if (region != Region.All)
            {
                region_urlpara = region + "/";
            }
            if (rank != Rank.PLATINUM)
            {
                rank_urlpara = rank + "/";
            }
            return lolDataWebsiteDomain + "/champions/counters/" + name + "/" + region_urlpara  + rank_urlpara + "sr-ranked";
        }
        public static string getOverallbyRegionRankUrl(string region, string rank) 
        {
            string region_urlpara = "", rank_urlpara = "";
            if (region != Region.All)
            {
                region_urlpara = region + "/";
            }
            if (rank != Rank.PLATINUM) 
            {
                rank_urlpara = rank + "/";
            }
            string insertedUrlpara = region_urlpara + rank_urlpara;
            string url = lolDataDefaultUrl.Insert(lolDataDefaultUrl.IndexOf("sr-ranked"), insertedUrlpara);
            return url;
        }
    }   
}
