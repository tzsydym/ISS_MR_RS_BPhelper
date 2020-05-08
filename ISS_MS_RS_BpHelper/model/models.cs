using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISS_MS_RS_BpHelper
{    
    public static class Rank 
    {
        public static string IRON { get; } = "iron";
        public static string BRONZE { get; } = "bronze";
        public static string SILVER { get; } = "silver";
        public static string GOLD { get; } = "gold";
        public static string PLATINUM { get; } = "platinum";
        public static string DIAMOND { get; } = "diamond";
        public static string MASTER { get; } = "master";
    }
    public static class Region 
    {
        public static string All { get; } = "all";
        public static string EuropeWest { get; } = "euw";
        public static string EuropeNordic { get; } = "eune";
        public static string Brazil { get; } = "br";
        public static string LatinAmericaNorth { get; } = "lan";
        public static string LatinAmericaSouth { get; } = "las";
        public static string NorthAmerica { get; } = "na";
        public static string Oceania { get; } = "oce";
        public static string Russia { get; } = "ru";
        public static string Turkey { get; } = "tr";
        public static string Japan { get; } = "jp";
        public static string Korea { get; } = "ko";
    }
}
