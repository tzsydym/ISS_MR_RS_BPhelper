using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using System.IO;
using System.Threading;
using System.Net;
using System.Runtime.InteropServices;
using Microsoft.VisualBasic.FileIO;

namespace ISS_MS_RS_BpHelper
{
    public partial class BPHelper : Form
    {
        public static ChromiumWebBrowser myBrowser;
        CefSettings settings;
        Game game = null;
        Task game_t = null;
        CancellationTokenSource tokenSource = new CancellationTokenSource();
        
        EventWaitHandle actionDone = new AutoResetEvent(false),
            ruleAndDesicionDone = new AutoResetEvent(false);
        string JSONInstruction = "";
        public BPHelper()
        {
            InitializeComponent();
            CancellationToken token = tokenSource.Token;
        }


        private void BPHelper_Load(object sender, EventArgs e)
        {
            this.KeyPreview = true;

            CheckForIllegalCrossThreadCalls = false;
            settings = new CefSettings();

            Cef.EnableHighDPISupport();
            Cef.Initialize(settings);

            string page = string.Format(@"{0}web res/html/basePage.html",
                 AppDomain.CurrentDomain.BaseDirectory);

            Directory.CreateDirectory(StaticValue.csvSavingFolder);

            myBrowser = new ChromiumWebBrowser(page);
            
            BrowserSettings browserSettings = new BrowserSettings();
            browserSettings.FileAccessFromFileUrls = CefState.Enabled;
            browserSettings.UniversalAccessFromFileUrls = CefState.Enabled;

            myBrowser.BrowserSettings = browserSettings;
            myBrowser.Parent = tabBP;

            myBrowser.Dock = DockStyle.Fill;

            JsToCsharpConverter jsToCsharpConverter = new JsToCsharpConverter(myBrowser, this);
            jsToCsharpConverter.contentHtml = "bpPage.html";
            jsToCsharpConverter.contentScript = "../js/html_js/bpPage.js";

            myBrowser.JavascriptObjectRepository.Register("boundedObj", jsToCsharpConverter, true);

            ChromeDevToolsSystemMenu.CreateSysMenu(this);
        }

        public virtual string mapService(string msg)
        {
            var serializer = new JavaScriptSerializer();
            serializer.RegisterConverters(new[] { new DynamicJsonConverter() });
            dynamic paraObj = serializer.Deserialize(msg, typeof(object));

            switch (paraObj.message)
            {
                case "check data existence":
                    return csvAlreadyExist(paraObj.region, paraObj.rank);
                case "start crawler":
                    return startCrawler(paraObj.region, paraObj.rank);
                case "website source":
                    if (tabs.TabCount <= 1)
                    {
                        this.BeginInvoke((MethodInvoker)delegate ()
                        {
                            TabPage newTab = new TabPage();
                            tabs.Controls.Add(newTab);
                            tabs.SelectedIndex = tabs.TabCount - 1;

                            ChromiumWebBrowser chromiumWebBrowser = new ChromiumWebBrowser(StaticValue.lolDataDefaultUrl);
                            chromiumWebBrowser.Parent = newTab;
                            chromiumWebBrowser.Dock = DockStyle.Fill;
                            chromiumWebBrowser.TitleChanged += chrom_titleChanged;
                        });
                    }
                    else
                    {
                        tabs.SelectedIndex = tabs.TabCount - 1;
                    }
                    return "";
                case "persist data":
                    return persistCsvData(paraObj.region, paraObj.rank);
                case "get avatars":
                    return getAllChampionAvatar();
                case "start new game":
                    string[] ourLane = new string[5],
                    enemyLane = new string[5];
                    for (int i = 0; i < ourLane.Length;i++) 
                    {
                        ourLane[i] = (string)paraObj.ourLane[i];
                        ourLane[i] = (string)paraObj.enemyLane[i];
                    }
                    bool firstMove = (bool)paraObj.firstMove;
                    return startNewGame(ourLane, enemyLane, firstMove);
                case "take action":
                    return takeAction(paraObj.selectedChampion);
            }
            return "";
        }
        void rotateBPDecision(Game game) 
        {
            Rotate rotate;
            BPPhase bpPhase;
            Instruction instruction = new Instruction();

            var trunk_rotate = RoatetDecisionQuery.RotateDecisionTree();
            trunk_rotate.Evaluate(game);
            rotate = RotateDecisionResult.Rotate;

            var trunk_bpPhase = BPPhaseDecisionQuery.BPPhaseDecisionTree();
            trunk_bpPhase.Evaluate(game);
            bpPhase = BPPhaseDecisionResult.BPPhase;

            game.BpPhase = bpPhase;
            instruction.rotate = rotate;
            instruction.bPPhase = bpPhase;
            game.instruction = instruction;
        }
        string takeAction(string selectedChampion) 
        {
            if (game != null && !new BPEndRule(game).Validate())
            {
                Response r = new Response();
                r.ChampionName = selectedChampion;
                if (game != null)
                {
                    game.response = r;
                }
                actionDone.Set();
                ruleAndDesicionDone.WaitOne();
                return JSONInstruction;
            }
            else 
            {
                return "Ban and pick has finished.";
            }
        }
        void BP() 
        {
            if (game != null) 
            {
                while (!new BPEndRule(game).Validate()) 
                {
                    actionDone.WaitOne();

                    InstructionTokenRule instructionTokenRule = new InstructionTokenRule(game);
                    instructionTokenRule.Validate();

                    rotateBPDecision(game);

                    TeammateRule teammateRule = new TeammateRule(game);
                    teammateRule.Validate();

                    UpdateChampionPoolWeightRule updateChampionPoolWeightRule = new UpdateChampionPoolWeightRule(game);
                    updateChampionPoolWeightRule.Validate();

                    RecommendRule recommendRule = new RecommendRule(game);
                    recommendRule.Validate();
                    
                    JSONInstruction = new JavaScriptSerializer().Serialize(game.instruction);
                    ruleAndDesicionDone.Set();
                }
            }
        }
        void cancelRunningGameTask() 
        {
            if (game_t != null && !game_t.IsCompleted)
            {
                tokenSource.Cancel();
            }
        }
        string startNewGame(string[] ourLane, string[] enemylane, bool firstMove) 
        {
            string msg = "";
            bool dataPersisted = StaticValue.allChampions.Count > 0;
            if (dataPersisted)
            {
                cancelRunningGameTask();
                game = new Game();
                game.FirstMove = firstMove;
                game.ChampionPool = StaticValue.getCopyOfAllChampions();
                game.OurTeam.UpdateLane(ourLane);
                game.EnemyTeam.UpdateLane(enemylane);

                rotateBPDecision(game);

                TeammateRule teammateRule = new TeammateRule(game);
                teammateRule.Validate();

                UpdateChampionPoolWeightRule updateChampionPoolWeightRule = new UpdateChampionPoolWeightRule(game);
                updateChampionPoolWeightRule.Validate();

                RecommendRule recommendRule = new RecommendRule(game);
                recommendRule.Validate();

                game_t = Task.Run(BP);

                var jsonresponse =  new JavaScriptSerializer().Serialize(game.instruction);
                return jsonresponse;
            }
            else 
            {
                msg = "Data has not been persisted yet.";
            }
            return msg;
        }
        string getAllChampionAvatar() 
        {
            string avatarPath = @"web res/IMG/champion-avatar";
            DirectoryInfo di = new DirectoryInfo(avatarPath);
            string returnedJSON = "[";
 
            foreach (FileInfo file in di.GetFiles()) 
            {
                
                returnedJSON += "\"" + file.Name + "\",";
            }
            returnedJSON += "\"\"]";//add empty string to the end, compensating for the extra comma.
            return returnedJSON;
        }
        string csvAlreadyExist(string region, string rank) 
        {
            string regionFolderPath = StaticValue.getRegionRankDirectory(region, rank);
            Directory.CreateDirectory(regionFolderPath);
            DirectoryInfo di = new DirectoryInfo(regionFolderPath);

            bool overallDataExisting = false, championCounterDataExisting = false;
            foreach (FileInfo file in di.GetFiles())
            {
                if (file.Name == StaticValue.csv_overallChampionsInfo) 
                {
                    overallDataExisting = true;
                }
                if (file.Name == StaticValue.csv_championCounter) 
                {
                    championCounterDataExisting = true;
                }
            }
            if (overallDataExisting && championCounterDataExisting)
            {
                return "Csv file already exists.";
            }
            else 
            {
                myBrowser.ExecuteScriptAsync("$('.overlay').toggleClass('scale-out');");
                return startCrawler(region, rank);
            }
        }
        string startCrawler(string region,string rank)
        {
            string msg = "";
            try
            {
                string regionFolderPath = StaticValue.getRegionRankDirectory(region,rank),
                overallDataPath = string.Format("{0}\\{1}", regionFolderPath, StaticValue.csv_overallChampionsInfo),
                championCounterDataPath = string.Format("{0}\\{1}", regionFolderPath, StaticValue.csv_championCounter);
                Directory.CreateDirectory(regionFolderPath);
                DirectoryInfo di = new DirectoryInfo(regionFolderPath);
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
                // Load website
                HtmlWeb web = new HtmlWeb();
                web.UserAgent = StaticValue.user_agent;//change the user agent of htmlweb object.
                ChangeUserAgent();//change the user agent of webbrowser(.net control).
                var doc = new HtmlAgilityPack.HtmlDocument();
                string rank_url = StaticValue.mapRegionNameAndRankNameToUrlPara(rank),
                    region_url = StaticValue.mapRegionNameAndRankNameToUrlPara(region);
                string websiteSource = StaticValue.getOverallbyRegionRankUrl(region_url, rank_url);
                doc = web.Load(websiteSource);
                var div = doc.GetElementbyId("mainContent");
                HtmlNodeCollection rows_hero = div.SelectNodes("//table[@class = 'data_table with_sortable_column']//tr");

                //overall data
                using (StreamWriter sw_overall = File.CreateText(overallDataPath), sw_champion = File.CreateText(championCounterDataPath))
                {
                    sw_overall.WriteLine(string.Format("champion name,url,lane,win rate,ban rate"));
                    sw_champion.WriteLine("champion name, relationship, agent champion name, lane, win rate");
                    //counter data for every individual champion
                    for (int i = 0; i < rows_hero.Count; i++) 
                    {
                        HtmlNode node = rows_hero[i];
                        if (node.Attributes.Count > 0) //table header, skip
                        {
                            continue;
                        }

                        HtmlNodeCollection championAttributes = node.SelectNodes("td");
                        if (championAttributes.Count > 1) 
                        {
                            HtmlNode attribute1 = championAttributes[1].SelectSingleNode("a[1]");

                            string name = attribute1.SelectSingleNode("div/div[@class='txt']/span").InnerText.Replace("\r\n", "").Replace(" ", String.Empty),
                                alias = attribute1.Attributes["href"].Value.Split('/')[3].ToLower();
                            string lane = attribute1.SelectSingleNode("div/div[@class='txt']/i").InnerText.Replace("\r\n", "").Replace(" ", String.Empty).Replace(",", ";");
                            string winRate = championAttributes[3].SelectSingleNode("progressbar[1]").Attributes["data-value"].Value;
                            string banRate = championAttributes[4].SelectSingleNode("progressbar[1]").Attributes["data-value"].Value;
                            string championCounterURL = StaticValue.getChampionCounterbyRegionRankUrl(alias,region_url,rank_url);
                            sw_overall.WriteLine(string.Format("{0},{1},{2},{3},{4}", name, championCounterURL, lane, winRate, banRate));
                            var imgClient = new WebClient();//champion avatar downloader
                            myBrowser.ExecuteScriptAsync("M.toast({html: 'Crawling data of "+ alias + "', classes: 'rounded'});");
                            this.Invoke(((Action)(() => {

                                Dictionary<int, string> rankDic = new Dictionary<int, string>();
                                rankDic[0] = Rank.IRON;
                                rankDic[1] = Rank.BRONZE;
                                rankDic[2] = Rank.GOLD;
                                rankDic[3] = Rank.SILVER;
                                rankDic[4] = Rank.PLATINUM;
                                rankDic[5] = Rank.DIAMOND;
                                rankDic[6] = Rank.MASTER;

                                //the website clears and restatistic data from servers.
                                //so some times the data is not available for the slected rank and server.
                                //in this case, craw data from lower rank and all server.
                                string[] tableNmae = new string[3] { "best against", "is countered by", "best with" };
                                bool[] dataGot = new bool[] { false, false, false };
                                int rank_i = rankDic.FirstOrDefault(x => x.Value == rank_url).Key; 
                                while (!dataGot[0] || !dataGot[1] || !dataGot[2]) //while not all counter data are retrieved
                                {                                   
                                    if (rank_i < 0) 
                                    {
                                        break;
                                    }
                                    var doc_champion = web.LoadFromBrowser(championCounterURL, html =>
                                    {
                                    // WAIT until the dynamic text is set
                                    return !html.Contains("<button type='button' class='see_more_button'>See more</button>");
                                    });

                                    var content = doc_champion.DocumentNode;

                                    var avarta_url = content.SelectSingleNode("//div[@class='pageBanner img-align-block']/div[@class='img']/img").Attributes["src"].Value;
                                    //download champion avatar
                                    string avartaPath = @"../debug/web res/IMG/champion-avatar/" + name + ".jpg";
                                    if (!File.Exists(avartaPath))
                                    imgClient.DownloadFile("https:" + avarta_url, avartaPath);

                                    var winRateTables = content.SelectNodes("//table[@class= 'data_table sortable_table']");


                                    //best with, best against, is countered by
                                    for (int j = 0; j < winRateTables.Count; j++)
                                    {
                                        HtmlNodeCollection agentChampions = winRateTables[j].SelectNodes("tbody/tr");
                                        if (agentChampions.Count > 1 && !dataGot[j])
                                        {
                                            dataGot[j] = true;
                                            for (int k = 1; k < agentChampions.Count; k++)
                                            {
                                                HtmlNode button = agentChampions[k].SelectSingleNode("td/button[@class='see_more_button']");
                                                if (button == null)
                                                {
                                                    HtmlNodeCollection agentChampionInfo = agentChampions[k].SelectNodes("td");
                                                    string agent_name = agentChampionInfo[0].SelectSingleNode("a/div/div[@class = 'txt']/span").InnerText.Replace("\r\n", "").Replace(" ", String.Empty),
                                                        agent_lane = agentChampionInfo[0].SelectSingleNode("a/div/div[@class = 'txt']/i").InnerText.Replace("\r\n", "").Replace(" ", String.Empty).Replace(",", ";"),
                                                        winRate_agent = agentChampionInfo[1].SelectSingleNode("progressbar").Attributes["data-value"].Value;

                                                    sw_champion.WriteLine(string.Format("{0},{1},{2},{3},{4}", name, tableNmae[j], agent_name, agent_lane, winRate_agent));
                                                }
                                            }
                                        }
                                    }
                                    championCounterURL = StaticValue.getChampionCounterbyRegionRankUrl(alias, ISS_MS_RS_BpHelper.Region.All,rankDic[--rank_i]);
                                }
                            })), null);                                                     
                        }                       
                    }
                }
                msg = "Crawler has finished, csv has been saved to " + regionFolderPath;
            }
            catch (Exception e)
            {
                msg = "error " + e.Message;
            }

            return msg;

        }
        string persistCsvData(string region, string rank)
        {
            string msg = "";
            string regionFolderPath = StaticValue.getRegionRankDirectory(region, rank),
                overallDataPath = string.Format("{0}\\{1}", regionFolderPath, StaticValue.csv_overallChampionsInfo),
                championCounterDataPath = string.Format("{0}\\{1}", regionFolderPath, StaticValue.csv_championCounter);
            if (File.Exists(overallDataPath) && File.Exists(championCounterDataPath))
            {
                StaticValue.allChampions.Clear();
                FileStream stream_overall = new FileStream(overallDataPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                TextFieldParser csvReader_overall = new TextFieldParser(stream_overall);
                csvReader_overall.SetDelimiters(new string[] { "," });

                FileStream stream_counter = new FileStream(championCounterDataPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                TextFieldParser csvReader_counter = new TextFieldParser(stream_counter);
                csvReader_counter.SetDelimiters(new string[] { "," });

                csvReader_overall.ReadFields();//header
                csvReader_counter.ReadFields();//header

                string[] filed_counter = null;
                while (!csvReader_overall.EndOfData) 
                {
                    string[] filed = csvReader_overall.ReadFields();
                    Champion champion = new Champion();
                    champion.Name = filed[0];
                    string championName = champion.Name;
                    string[] lanes = filed[2].Split(';');
                    for (int i = 0; i < lanes.Count(); i++) 
                    {
                        champion.Lanes.Add(StaticValue.mapLane(lanes[i]));
                    }
                    champion.WinRate = Convert.ToDouble(filed[3]);
                    champion.BanRate = Convert.ToDouble(filed[4]);

                    CounterInfo counterInfo = new CounterInfo();
                    if (filed_counter != null) 
                    {
                        CounterRelation cf = new CounterRelation();
                        cf.ChampionName = filed_counter[2];
                        cf.Lane = StaticValue.mapLane(filed_counter[3]);
                        cf.Rate = Convert.ToDouble(filed_counter[4]);
                        switch (filed_counter[1])
                        {
                            case "best against":
                                counterInfo.BestAgainst[cf.ChampionName] = cf;
                                break;
                            case "is countered by":
                                counterInfo.IsCounteredBy[cf.ChampionName] = cf;
                                break;
                            case "best with":
                                counterInfo.BestWith[cf.ChampionName] = cf;
                                break;
                        }
                    }
                    while (!csvReader_counter.EndOfData && championName == champion.Name) 
                    {
                        filed_counter = csvReader_counter.ReadFields();
                        championName = filed_counter[0];
                        if (championName == champion.Name)
                        {
                            CounterRelation cf = new CounterRelation();
                            cf.ChampionName = filed_counter[2];
                            cf.Lane = StaticValue.mapLane(filed_counter[3]);
                            cf.Rate = Convert.ToDouble(filed_counter[4]);
                            switch (filed_counter[1])
                            {
                                case "best against":
                                    counterInfo.BestAgainst[cf.ChampionName] = cf;
                                    break;
                                case "is countered by":
                                    counterInfo.IsCounteredBy[cf.ChampionName] = cf;
                                    break;
                                case "best with":
                                    counterInfo.BestWith[cf.ChampionName] = cf;
                                    break;
                            }
                            filed_counter = null;
                        }
                    }
                    champion.counterInfo = counterInfo;
                    StaticValue.allChampions.Add(champion);
                }
                msg = "Data has been persisted into memory";
            }
            else 
            {
                msg = "No CSV file has been crawlered for the selected rank and region.";
            }
            return msg;
        }
        //change web browser user agents
        [DllImport("urlmon.dll", CharSet = CharSet.Ansi)]
        private static extern int UrlMkSetSessionOption(
    int dwOption, string pBuffer, int dwBufferLength, int dwReserved);

        const int URLMON_OPTION_USERAGENT = 0x10000001;
        const int URLMON_OPTION_USERAGENT_REFRESH = 0x10000002;
        public void ChangeUserAgent()
        {
            string ua = StaticValue.user_agent;

            UrlMkSetSessionOption(URLMON_OPTION_USERAGENT_REFRESH, null, 0, 0);
            UrlMkSetSessionOption(URLMON_OPTION_USERAGENT, ua, ua.Length, 0);
        }
        void chrom_titleChanged(object sender, TitleChangedEventArgs e)
        {
            this.Invoke(new MethodInvoker(() =>
            {

                if (tabs.SelectedIndex != 0)
                    tabs.SelectedTab.Text = e.Title;

            }));
        }

        private void BaseForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Cef.Shutdown();
        }
       
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if ((m.Msg == ChromeDevToolsSystemMenu.WM_SYSCOMMAND) && ((int)m.WParam == ChromeDevToolsSystemMenu.SYSMENU_CHROME_DEV_TOOLS))
            {
                myBrowser.ShowDevTools();
            }
        }
    }
}