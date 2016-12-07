using System;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using ezBot;
using PvPNetClient;
using PvPNetClient.RiotObjects;
using PvPNetClient.RiotObjects.Platform.Catalog.Champion;
using PvPNetClient.RiotObjects.Platform.Clientfacade.Domain;
using PvPNetClient.RiotObjects.Platform.Game;
using PvPNetClient.RiotObjects.Platform.Game.Message;
using PvPNetClient.RiotObjects.Platform.Matchmaking;
using PvPNetClient.RiotObjects.Platform.Statistics;
using PvPNetClient.RiotObjects.Leagues.Pojo;
using PvPNetClient.RiotObjects.Platform.Game.Practice;
using PvPNetClient.RiotObjects.Platform.Harassment;
using PvPNetClient.RiotObjects.Platform.Leagues.Client.Dto;
using PvPNetClient.RiotObjects.Platform.Login;
using PvPNetClient.RiotObjects.Platform.Reroll.Pojo;
using PvPNetClient.RiotObjects.Platform.Statistics.Team;
using PvPNetClient.RiotObjects.Platform.Summoner;
using PvPNetClient.RiotObjects.Platform.Summoner.Boost;
using PvPNetClient.RiotObjects.Platform.Summoner.Masterybook;
using PvPNetClient.RiotObjects.Platform.Summoner.Runes;
using PvPNetClient.RiotObjects.Platform.Summoner.Spellbook;
using PvPNetClient.RiotObjects.Team;
using PvPNetClient.RiotObjects.Team.Dto;
using PvPNetClient.RiotObjects.Platform.Game.Map;
using PvPNetClient.RiotObjects.Platform.Summoner.Icon;
using PvPNetClient.RiotObjects.Platform.Catalog.Icon;
using PvPNetClient.RiotObjects.Platform.Messaging;
using PvPNetClient.RiotObjects.Platform.Trade;
using ezBot.Utils;

namespace ChallengerBot
{
    internal class ChallengerBot
    {
        public Process exeProcess;
        public LoginDataPacket loginPacket = new LoginDataPacket();
        public PvPNetConnection connection = new PvPNetConnection();

        public bool firstTimeInLobby = true;
        public bool firstTimeInQueuePop = true;
        public bool firstTimeInCustom = true;
        public bool firstTimeInPostChampSelect = true;

        public string Accountname;
        public string Password;
        public string ipath;

        public string region { get; set; }

        public string sumName { get; set; }
        public double sumId { get; set; }
        public double sumLevel { get; set; }
        public double archiveSumLevel { get; set; }
        public double rpBalance { get; set; }
        public double ipBalance { get; set; }

        public int m_leaverBustedPenalty { get; set; }
        public string m_accessToken { get; set; }

        public QueueTypes queueType { get; set; }
        public QueueTypes actualQueueType { get; set; }

        public ezBot(string username, string password, string reg, string path, QueueTypes QueueType, string LoLVersion)
        {
            ipath = path;
            Accountname = username;
            Password = password;
            queueType = QueueType;
            region = reg;
            connection.OnConnect += new PvPNetConnection.OnConnectHandler(connection_OnConnect);
            connection.OnDisconnect += new PvPNetConnection.OnDisconnectHandler(connection_OnDisconnect);
            connection.OnError += new PvPNetConnection.OnErrorHandler(connection_OnError);
            connection.OnLogin += new PvPNetConnection.OnLoginHandler(connection_OnLogin);
            connection.OnLoginQueueUpdate += new PvPNetConnection.OnLoginQueueUpdateHandler(connection_OnLoginQueueUpdate);
            connection.OnMessageReceived += new PvPNetConnection.OnMessageReceivedHandler(connection_OnMessageReceived);
            switch (region)
            {
                case "EUW":
                    connection.Connect(username, password, Region.EUW, LoLVersion);
                    break;
                case "EUNE":
                    connection.Connect(username, password, Region.EUN, LoLVersion);
                    break;
                case "NA":
                    connection.Connect(username, password, Region.NA, LoLVersion);
                    break;
                case "KR":
                    connection.Connect(username, password, Region.KR, LoLVersion);
                    break;
                case "BR":
                    connection.Connect(username, password, Region.BR, LoLVersion);
                    break;
                case "OCE":
                    connection.Connect(username, password, Region.OCE, LoLVersion);
                    break;
                case "RU":
                    connection.Connect(username, password, Region.RU, LoLVersion);
                    break;
                case "TR":
                    connection.Connect(username, password, Region.TR, LoLVersion);
                    break;
                case "LAS":
                    connection.Connect(username, password, Region.LAS, LoLVersion);
                    break;
                case "LAN":
                    connection.Connect(username, password, Region.LAN, LoLVersion);
                    break;
                case "JP":
                    connection.Connect(username, password, Region.JP, LoLVersion);
                    break;
            }
        }

        public async void connection_OnMessageReceived(object sender, object message)
        {
            if (message is GameDTO)
            {
                GameDTO game = message as GameDTO;
                switch (game.GameState)
                {
                    case "CHAMP_SELECT":
                        firstTimeInCustom = true;
                        firstTimeInQueuePop = true;
                        if (firstTimeInLobby)
                        {
                            firstTimeInLobby = false;
                            Tools.ConsoleMessage("You are in champion select.", ConsoleColor.White);
                            object obj = await connection.SetClientReceivedGameMessage(game.Id, "CHAMP_SELECT_CLIENT");

                            //select champion and spells for non ARAM  games
                            if (queueType != QueueTypes.ARAM)
                            {
                                int Spell1;
                                int Spell2;
                                if (!Program.randomSpell)
                                {
                                    Spell1 = Enums.GetSpell(Program.spell1);
                                    Spell2 = Enums.GetSpell(Program.spell2);
                                }
                                else
                                {
                                    var random = new Random();
                                    var spellList = new List<int> { 13, 6, 7, 10, 1, 11, 21, 12, 3, 14, 2, 4 };

                                    int index = random.Next(spellList.Count);
                                    int index2 = random.Next(spellList.Count);

                                    int randomSpell1 = spellList[index];
                                    int randomSpell2 = spellList[index2];

                                    if (randomSpell1 == randomSpell2)
                                    {
                                        int index3 = random.Next(spellList.Count);
                                        randomSpell2 = spellList[index3];
                                    }

                                    Spell1 = Convert.ToInt32(randomSpell1);
                                    Spell2 = Convert.ToInt32(randomSpell2);
                                }

                                await connection.SelectSpells(Spell1, Spell2);

                                string championPick = "";
                                int championPickID = 0;
                                int champRand = Generator.CreateRandom(1, 5);
                                switch (champRand)
                                {
                                    case 1:
                                        championPickID = Enums.GetChampion(Program.firstChampionPick);
                                        championPick = Program.firstChampionPick;
                                        break;
                                    case 2:
                                        championPickID = Enums.GetChampion(Program.secondChampionPick);
                                        championPick = Program.secondChampionPick;
                                        break;
                                    case 3:
                                        championPickID = Enums.GetChampion(Program.thirdChampionPick);
                                        championPick = Program.thirdChampionPick;
                                        break;
                                    case 4:
                                        championPickID = Enums.GetChampion(Program.fourthChampionPick);
                                        championPick = Program.fourthChampionPick;
                                        break;
                                    case 5:
                                        championPickID = Enums.GetChampion(Program.fifthChampionPick);
                                        championPick = Program.fifthChampionPick;
                                        break;
                                }

                                await connection.SelectChampion(championPickID);
                                Tools.ConsoleMessage("Selected Champion: " + championPick.ToUpper(), ConsoleColor.DarkYellow);
                                await connection.ChampionSelectCompleted();
                            }

                            //select spells for ARAM
                            if (queueType == QueueTypes.ARAM)
                            {
                                int Spell1;
                                int Spell2;
                                if (!Program.randomSpell)
                                {
                                    Spell1 = Enums.GetSpell(Program.spell1);
                                    Spell2 = Enums.GetSpell(Program.spell2);
                                }
                                else
                                {
                                    var random = new Random();
                                    var spellList = new List<int> { 13, 6, 7, 10, 1, 11, 21, 12, 3, 14, 2, 4 };

                                    int index = random.Next(spellList.Count);
                                    int index2 = random.Next(spellList.Count);

                                    int randomSpell1 = spellList[index];
                                    int randomSpell2 = spellList[index2];

                                    if (randomSpell1 == randomSpell2)
                                    {
                                        int index3 = random.Next(spellList.Count);
                                        randomSpell2 = spellList[index3];
                                    }

                                    Spell1 = Convert.ToInt32(randomSpell1);
                                    Spell2 = Convert.ToInt32(randomSpell2);
                                }

                                await connection.SelectSpells(Spell1, Spell2);
                                await connection.ChampionSelectCompleted();
                            }
                            break;
                        }
                        else
                            break;
                    case "POST_CHAMP_SELECT":
                        firstTimeInLobby = false;
                        if (firstTimeInPostChampSelect)
                        {
                            firstTimeInPostChampSelect = false;
                            Tools.ConsoleMessage("Waiting for league of legends to respond.", ConsoleColor.White);
                        }
                        break;
                    case "IN_QUEUE":
                        Tools.ConsoleMessage("You are now in queue.", ConsoleColor.White);
                        break;
                    case "TERMINATED":
                        //Tools.ConsoleMessage("Re-entering queue.", ConsoleColor.White);
                        firstTimeInPostChampSelect = true;
                        firstTimeInQueuePop = true;
                        break;
                    case "LEAVER_BUSTED":
                        Tools.ConsoleMessage("You have leave buster.", ConsoleColor.White);
                        break;
                    case "JOINING_CHAMP_SELECT":
                        if (firstTimeInQueuePop && game.StatusOfParticipants.Contains("1"))
                        {
                            Tools.ConsoleMessage("Match found and accepted.", ConsoleColor.White);
                            firstTimeInQueuePop = false;
                            firstTimeInLobby = true;
                            object obj = await connection.AcceptPoppedGame(true);
                            break;
                        }
                        else
                            break;
                }
            }
            else if (message.GetType() == typeof(TradeContractDTO))
            {
                var tradeDto = message as TradeContractDTO;
                if (tradeDto == null)
                    return;
                switch (tradeDto.State)
                {
                    case "PENDING":
                        {
                            if (tradeDto != null)
                            {

                            }
                        }
                        break;
                }
                return;
            }
            else if (message is PlayerCredentialsDto)
            {
                firstTimeInPostChampSelect = true;
                PlayerCredentialsDto dto = message as PlayerCredentialsDto;
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.CreateNoWindow = false;
                startInfo.WorkingDirectory = GetLeagueClient();
                startInfo.FileName = "League of Legends.exe";
                startInfo.Arguments = "\"8394\" \"LoLLauncher.exe\" \"\" \"" + dto.ServerIp + " " +
                    dto.ServerPort + " " + dto.EncryptionKey + " " + dto.SummonerId + "\"";

                new Thread(() =>
                {
                    exeProcess = Process.Start(startInfo);
                    exeProcess.Exited += new EventHandler(exeProcess_Exited);
                    while (exeProcess.MainWindowHandle == IntPtr.Zero) { }
                    if (Program.LOWPriority)
                        exeProcess.PriorityClass = ProcessPriorityClass.Idle;
                    else
                        exeProcess.PriorityClass = ProcessPriorityClass.High;

                    exeProcess.EnableRaisingEvents = true;
                    //Thread.Sleep(1000);
                }).Start();

                Tools.ConsoleMessage("Launching League of Legends.", ConsoleColor.White);
            }
            else if (!(message is GameNotification) && !(message is SearchingForMatchNotification))
            {
                if (message is EndOfGameStats)
                {
                    EnterQueue();
                }
                else
                {
                    if (message.ToString().Contains("EndOfGameStats"))
                    {
                        try
                        {
                            if (exeProcess != null)
                            {
                                EndOfGameStats eog = new EndOfGameStats();
                                connection_OnMessageReceived(sender, eog);
                                exeProcess.Exited -= exeProcess_Exited;
                                exeProcess.Kill();
                                Thread.Sleep(500);
                                if (exeProcess.Responding)
                                {
                                    Process.Start("taskkill /F /IM \"League of Legends.exe\"");
                                }
                                loginPacket = await this.connection.GetLoginDataPacketForUser();
                                archiveSumLevel = sumLevel;
                                sumLevel = loginPacket.AllSummonerData.SummonerLevel.Level;
                                if (sumLevel != archiveSumLevel)
                                {
                                    OnLevelUp();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Tools.Log(ex.Message.ToString());
                        }
                    }
                }
            }
        }

        private async void EnterQueue()
        {
            MatchMakerParams matchParams = new MatchMakerParams();
            //Set BotParams
            if (queueType == QueueTypes.BOT_INTO)
            {
                matchParams.BotDifficulty = "INTRO";
            }
            else if (queueType == QueueTypes.BOT_BEGINNER)
            {
                matchParams.BotDifficulty = "EASY";
            }
            else if (queueType == QueueTypes.BOT_MEDIUM)
            {
                matchParams.BotDifficulty = "MEDIUM";
            }
            else if (queueType == QueueTypes.BOT_3x3) //TT Map
            {
                matchParams.BotDifficulty = "EASY";
            }

            //Check if is available to join queue.
            if (sumLevel == 3 && actualQueueType == QueueTypes.NORMAL_5x5)
            {
                queueType = actualQueueType;
            }
            else if (sumLevel == 6 && actualQueueType == QueueTypes.ARAM)
            {
                queueType = actualQueueType;
            }
            else if (sumLevel == 7 && actualQueueType == QueueTypes.NORMAL_3x3)
            {
                queueType = actualQueueType;
            }
            else if (sumLevel == 10 && actualQueueType == QueueTypes.TT_HEXAKILL)
            {
                queueType = actualQueueType;
            }

            matchParams.QueueIds = new Int32[1] { (int)queueType };
            SearchingForMatchNotification m = await connection.AttachToQueue(matchParams);

            if (m.PlayerJoinFailures == null)
            {
                Tools.ConsoleMessage("In Queue: " + queueType.ToString() + " as " + loginPacket.AllSummonerData.Summoner.Name + ".", ConsoleColor.Cyan);
            }
            else
            {
                foreach (var failure in m.PlayerJoinFailures)
                {
                    if (failure.ReasonFailed == "LEAVER_BUSTED")
                    {
                        m_accessToken = failure.AccessToken;
                        if (failure.LeaverPenaltyMillisRemaining > m_leaverBustedPenalty)
                        {
                            m_leaverBustedPenalty = failure.LeaverPenaltyMillisRemaining;
                        }
                    }
                    else if (failure.ReasonFailed == "LEAVER_BUSTER_TAINTED_WARNING")
                    {
                        await connection.ackLeaverBusterWarning();
                        await connection.callPersistenceMessaging(new SimpleDialogMessageResponse()
                        {
                            AccountID = loginPacket.AllSummonerData.Summoner.SumId,
                            MsgID = loginPacket.AllSummonerData.Summoner.SumId,
                            Command = "ack"
                        });
                        connection_OnMessageReceived(null, (object)new EndOfGameStats());
                    }
                }

                if (String.IsNullOrEmpty(m_accessToken))
                {
                    // Queue dodger or something else
                }
                else
                {
                    Tools.ConsoleMessage("Waiting leaver buster time: " + m_leaverBustedPenalty / 1000 / (float)60 + " minutes!", ConsoleColor.White);
                    System.Threading.Thread.Sleep(TimeSpan.FromMilliseconds(m_leaverBustedPenalty));
                    m = await connection.AttachToLowPriorityQueue(matchParams, m_accessToken);
                    if (m.PlayerJoinFailures == null)
                    {
                        Tools.ConsoleMessage("Joined lower priority queue! as " + loginPacket.AllSummonerData.Summoner.Name + ".", ConsoleColor.Cyan);
                    }
                    else
                    {
                        Tools.ConsoleMessage("There was an error in joining lower priority queue.\nDisconnecting.", ConsoleColor.White);
                        connection.Disconnect();
                    }
                }
            }
        }

        private String GetLeagueClient()
        {
            String installPath = ipath;
            if (installPath.Contains("notfound"))
                return installPath;
            installPath += @"RADS\solutions\lol_game_client_sln\releases\";
            installPath = Directory.EnumerateDirectories(installPath).OrderBy(f => new DirectoryInfo(f).CreationTime).Last();
            installPath += @"\deploy\";
            return installPath;
        }

        async void exeProcess_Exited(object sender, EventArgs e)
        {
            Tools.ConsoleMessage("Restart League of Legends.", ConsoleColor.White);
            loginPacket = await connection.GetLoginDataPacketForUser();
            if (this.loginPacket.ReconnectInfo != null && this.loginPacket.ReconnectInfo.Game != null)
            {
                this.connection_OnMessageReceived(sender, (object)this.loginPacket.ReconnectInfo.PlayerCredentials);
            }
            else
                this.connection_OnMessageReceived(sender, (object)new EndOfGameStats());
        }

        private async void RegisterNotifications()
        {
            object obj1 = await connection.Subscribe("bc", this.connection.AccountID());
            object obj2 = await connection.Subscribe("cn", this.connection.AccountID());
            object obj3 = await connection.Subscribe("gn", this.connection.AccountID());
        }

        private void connection_OnLoginQueueUpdate(object sender, int positionInLine)
        {
            if (positionInLine <= 0)
                return;

            Tools.ConsoleMessage("Position to login: " + (object)positionInLine, ConsoleColor.White);
        }

        private void connection_OnLogin(object sender, string username, string ipAddress)
        {
            new Thread((ThreadStart)(async () =>
            {
                Tools.ConsoleMessage("Logging onto account...", ConsoleColor.White);
                RegisterNotifications();
                loginPacket = await this.connection.GetLoginDataPacketForUser();
                if (loginPacket.AllSummonerData == null)
                {
                    Tools.ConsoleMessage("Summoner not found in account.", ConsoleColor.Red);
                    Tools.ConsoleMessage("Creating Summoner...", ConsoleColor.Red);
                    Random rnd = new Random();
                    String summonerName = Accountname;
                    if (summonerName.Length > 16)
                        summonerName = summonerName.Substring(0, 12) + new Random().Next(1000, 9999).ToString();
                    AllSummonerData sumData = await connection.CreateDefaultSummoner(summonerName);
                    loginPacket.AllSummonerData = sumData;
                    Tools.ConsoleMessage("Created Summoner: " + summonerName, ConsoleColor.White);
                }
                sumLevel = loginPacket.AllSummonerData.SummonerLevel.Level;
                string sumName = loginPacket.AllSummonerData.Summoner.Name;
                double sumId = loginPacket.AllSummonerData.Summoner.SumId;
                rpBalance = loginPacket.RpBalance;
                ipBalance = loginPacket.IpBalance;
                if (sumLevel >= Program.maxLevel)
                {
                    Tools.ConsoleMessage("Summoner: " + sumName + " is already max level.", ConsoleColor.White);
                    Tools.ConsoleMessage("Log into new account.", ConsoleColor.White);
                    connection.Disconnect();
                    Program.LognNewAccount();
                    return;
                }

                if (rpBalance == 400.0 && Program.buyExpBoost)
                {
                    Tools.ConsoleMessage("Buying XP Boost", ConsoleColor.White);
                    try
                    {
                        Task t = new Task(OnBuyBoost);
                        t.Start();
                    }
                    catch (Exception exception)
                    {
                        Tools.ConsoleMessage("Couldn't buy RP Boost.\n" + exception.Message.ToString(), ConsoleColor.White);
                    }
                }

                if (sumLevel < 3.0 && queueType == QueueTypes.NORMAL_5x5)
                {
                    Tools.ConsoleMessage("Need to be Level 3 before NORMAL_5x5 queue.", ConsoleColor.White);
                    Tools.ConsoleMessage("Joins Co-Op vs AI (Beginner) queue until 3", ConsoleColor.White);
                    queueType = QueueTypes.BOT_BEGINNER;
                    actualQueueType = QueueTypes.NORMAL_5x5;
                }
                else if (sumLevel < 6.0 && queueType == QueueTypes.ARAM)
                {
                    Tools.ConsoleMessage("Need to be Level 6 before ARAM queue.", ConsoleColor.White);
                    Tools.ConsoleMessage("Joins Co-Op vs AI (Beginner) queue until 6", ConsoleColor.White);
                    queueType = QueueTypes.BOT_BEGINNER;
                    actualQueueType = QueueTypes.ARAM;
                }
                else if (sumLevel < 7.0 && queueType == QueueTypes.NORMAL_3x3)
                {
                    Tools.ConsoleMessage("Need to be Level 7 before NORMAL_3x3 queue.", ConsoleColor.White);
                    Tools.ConsoleMessage("Joins Co-Op vs AI (Beginner) queue until 7", ConsoleColor.White);
                    queueType = QueueTypes.BOT_BEGINNER;
                    actualQueueType = QueueTypes.NORMAL_3x3;
                }

                Tools.ConsoleMessage("Welcome " + loginPacket.AllSummonerData.Summoner.Name + " - lvl (" + loginPacket.AllSummonerData.SummonerLevel.Level + ") IP: (" + ipBalance.ToString() + ")", ConsoleColor.White);
                PlayerDTO player = await connection.CreatePlayer();
                if (this.loginPacket.ReconnectInfo != null && this.loginPacket.ReconnectInfo.Game != null)
                {
                    this.connection_OnMessageReceived(sender, (object)this.loginPacket.ReconnectInfo.PlayerCredentials);
                }
                else
                    this.connection_OnMessageReceived(sender, (object)new EndOfGameStats());
            })).Start();
        }

        private void connection_OnError(object sender, PvPNetClient.Error error)
        {
            if (error.Message.Contains("is not owned by summoner"))
            {
                return;
            }
            else if (error.Message.Contains("Your summoner level is too low to select the spell"))
            {
                var random = new Random();
                var spellList = new List<int> { 13, 6, 7, 10, 1, 11, 21, 12, 3, 14, 2, 4 };

                int index = random.Next(spellList.Count);
                int index2 = random.Next(spellList.Count);

                int randomSpell1 = spellList[index];
                int randomSpell2 = spellList[index2];

                if (randomSpell1 == randomSpell2)
                {
                    int index3 = random.Next(spellList.Count);
                    randomSpell2 = spellList[index3];
                }

                int Spell1 = Convert.ToInt32(randomSpell1);
                int Spell2 = Convert.ToInt32(randomSpell2);
                return;
            }

            Tools.ConsoleMessage("error received:\n" + error.Message, ConsoleColor.White);
        }

        private void connection_OnDisconnect(object sender, EventArgs e)
        {
            Console.Title = "ChallengerBot is Offline";
            Tools.ConsoleMessage("Disconnected", ConsoleColor.White);
        }

        private void connection_OnConnect(object sender, EventArgs e)
        {
            Console.Title = "ChallengerBot is Online";
        }

        private async void OnBuyBoost()
        {
            try
            {
                if (region == "EUW")
                {
                    string url = await connection.GetStoreUrl();
                    HttpClient httpClient = new HttpClient();
                    Console.WriteLine(url);
                    await httpClient.GetStringAsync(url);

                    string storeURL = "https://store.euw1.lol.riotgames.com/store/tabs/view/boosts/1";
                    await httpClient.GetStringAsync(storeURL);

                    string purchaseURL = "https://store.euw1.lol.riotgames.com/store/purchase/item";

                    List<KeyValuePair<string, string>> storeItemList = new List<KeyValuePair<string, string>>();
                    storeItemList.Add(new KeyValuePair<string, string>("item_id", "boosts_2"));
                    storeItemList.Add(new KeyValuePair<string, string>("currency_type", "rp"));
                    storeItemList.Add(new KeyValuePair<string, string>("quantity", "1"));
                    storeItemList.Add(new KeyValuePair<string, string>("rp", "260"));
                    storeItemList.Add(new KeyValuePair<string, string>("ip", "null"));
                    storeItemList.Add(new KeyValuePair<string, string>("duration_type", "PURCHASED"));
                    storeItemList.Add(new KeyValuePair<string, string>("duration", "3"));
                    HttpContent httpContent = new FormUrlEncodedContent(storeItemList);
                    await httpClient.PostAsync(purchaseURL, httpContent);

                    Tools.ConsoleMessage("Bought 'XP Boost: 3 Days'!", ConsoleColor.White);
                    httpClient.Dispose();
                }
                else if (region == "EUNE")
                {
                    string url = await connection.GetStoreUrl();
                    HttpClient httpClient = new HttpClient();
                    Console.WriteLine(url);
                    await httpClient.GetStringAsync(url);

                    string storeURL = "https://store.eun1.lol.riotgames.com/store/tabs/view/boosts/1";
                    await httpClient.GetStringAsync(storeURL);

                    string purchaseURL = "https://store.eun1.lol.riotgames.com/store/purchase/item";

                    List<KeyValuePair<string, string>> storeItemList = new List<KeyValuePair<string, string>>();
                    storeItemList.Add(new KeyValuePair<string, string>("item_id", "boosts_2"));
                    storeItemList.Add(new KeyValuePair<string, string>("currency_type", "rp"));
                    storeItemList.Add(new KeyValuePair<string, string>("quantity", "1"));
                    storeItemList.Add(new KeyValuePair<string, string>("rp", "260"));
                    storeItemList.Add(new KeyValuePair<string, string>("ip", "null"));
                    storeItemList.Add(new KeyValuePair<string, string>("duration_type", "PURCHASED"));
                    storeItemList.Add(new KeyValuePair<string, string>("duration", "3"));
                    HttpContent httpContent = new FormUrlEncodedContent(storeItemList);
                    await httpClient.PostAsync(purchaseURL, httpContent);

                    Tools.ConsoleMessage("Bought 'XP Boost: 3 Days'!", ConsoleColor.White);
                    httpClient.Dispose();
                }
                else if (region == "NA")
                {
                    string url = await connection.GetStoreUrl();
                    HttpClient httpClient = new HttpClient();
                    Console.WriteLine(url);
                    await httpClient.GetStringAsync(url);

                    string storeURL = "https://store.na2.lol.riotgames.com/store/tabs/view/boosts/1";
                    await httpClient.GetStringAsync(storeURL);

                    string purchaseURL = "https://store.na2.lol.riotgames.com/store/purchase/item";

                    List<KeyValuePair<string, string>> storeItemList = new List<KeyValuePair<string, string>>();
                    storeItemList.Add(new KeyValuePair<string, string>("item_id", "boosts_2"));
                    storeItemList.Add(new KeyValuePair<string, string>("currency_type", "rp"));
                    storeItemList.Add(new KeyValuePair<string, string>("quantity", "1"));
                    storeItemList.Add(new KeyValuePair<string, string>("rp", "260"));
                    storeItemList.Add(new KeyValuePair<string, string>("ip", "null"));
                    storeItemList.Add(new KeyValuePair<string, string>("duration_type", "PURCHASED"));
                    storeItemList.Add(new KeyValuePair<string, string>("duration", "3"));
                    HttpContent httpContent = new FormUrlEncodedContent(storeItemList);
                    await httpClient.PostAsync(purchaseURL, httpContent);

                    Tools.ConsoleMessage("Bought 'XP Boost: 3 Days'!", ConsoleColor.White);
                    httpClient.Dispose();
                }
                else if (region == "KR")
                {
                    string url = await connection.GetStoreUrl();
                    HttpClient httpClient = new HttpClient();
                    Console.WriteLine(url);
                    await httpClient.GetStringAsync(url);

                    string storeURL = "https://store.kr.lol.riotgames.com/store/tabs/view/boosts/1";
                    await httpClient.GetStringAsync(storeURL);

                    string purchaseURL = "https://store.kr.lol.riotgames.com/store/purchase/item";

                    List<KeyValuePair<string, string>> storeItemList = new List<KeyValuePair<string, string>>();
                    storeItemList.Add(new KeyValuePair<string, string>("item_id", "boosts_2"));
                    storeItemList.Add(new KeyValuePair<string, string>("currency_type", "rp"));
                    storeItemList.Add(new KeyValuePair<string, string>("quantity", "1"));
                    storeItemList.Add(new KeyValuePair<string, string>("rp", "260"));
                    storeItemList.Add(new KeyValuePair<string, string>("ip", "null"));
                    storeItemList.Add(new KeyValuePair<string, string>("duration_type", "PURCHASED"));
                    storeItemList.Add(new KeyValuePair<string, string>("duration", "3"));
                    HttpContent httpContent = new FormUrlEncodedContent(storeItemList);
                    await httpClient.PostAsync(purchaseURL, httpContent);

                    Tools.ConsoleMessage("Bought 'XP Boost: 3 Days'!", ConsoleColor.White);
                    httpClient.Dispose();
                }
                else if (region == "BR")
                {
                    string url = await connection.GetStoreUrl();
                    HttpClient httpClient = new HttpClient();
                    Console.WriteLine(url);
                    await httpClient.GetStringAsync(url);

                    string storeURL = "https://store.br.lol.riotgames.com/store/tabs/view/boosts/1";
                    await httpClient.GetStringAsync(storeURL);

                    string purchaseURL = "https://store.br.lol.riotgames.com/store/purchase/item";

                    List<KeyValuePair<string, string>> storeItemList = new List<KeyValuePair<string, string>>();
                    storeItemList.Add(new KeyValuePair<string, string>("item_id", "boosts_2"));
                    storeItemList.Add(new KeyValuePair<string, string>("currency_type", "rp"));
                    storeItemList.Add(new KeyValuePair<string, string>("quantity", "1"));
                    storeItemList.Add(new KeyValuePair<string, string>("rp", "260"));
                    storeItemList.Add(new KeyValuePair<string, string>("ip", "null"));
                    storeItemList.Add(new KeyValuePair<string, string>("duration_type", "PURCHASED"));
                    storeItemList.Add(new KeyValuePair<string, string>("duration", "3"));
                    HttpContent httpContent = new FormUrlEncodedContent(storeItemList);
                    await httpClient.PostAsync(purchaseURL, httpContent);

                    Tools.ConsoleMessage("Bought 'XP Boost: 3 Days'!", ConsoleColor.White);
                    httpClient.Dispose();
                }
                else if (region == "RU")
                {
                    string url = await connection.GetStoreUrl();
                    HttpClient httpClient = new HttpClient();
                    Console.WriteLine(url);
                    await httpClient.GetStringAsync(url);

                    string storeURL = "https://store.ru.lol.riotgames.com/store/tabs/view/boosts/1";
                    await httpClient.GetStringAsync(storeURL);

                    string purchaseURL = "https://store.ru.lol.riotgames.com/store/purchase/item";

                    List<KeyValuePair<string, string>> storeItemList = new List<KeyValuePair<string, string>>();
                    storeItemList.Add(new KeyValuePair<string, string>("item_id", "boosts_2"));
                    storeItemList.Add(new KeyValuePair<string, string>("currency_type", "rp"));
                    storeItemList.Add(new KeyValuePair<string, string>("quantity", "1"));
                    storeItemList.Add(new KeyValuePair<string, string>("rp", "260"));
                    storeItemList.Add(new KeyValuePair<string, string>("ip", "null"));
                    storeItemList.Add(new KeyValuePair<string, string>("duration_type", "PURCHASED"));
                    storeItemList.Add(new KeyValuePair<string, string>("duration", "3"));
                    HttpContent httpContent = new FormUrlEncodedContent(storeItemList);
                    await httpClient.PostAsync(purchaseURL, httpContent);

                    Tools.ConsoleMessage("Bought 'XP Boost: 3 Days'!", ConsoleColor.White);
                    httpClient.Dispose();
                }
                else if (region == "TR")
                {
                    string url = await connection.GetStoreUrl();
                    HttpClient httpClient = new HttpClient();
                    Console.WriteLine(url);
                    await httpClient.GetStringAsync(url);

                    string storeURL = "https://store.tr.lol.riotgames.com/store/tabs/view/boosts/1";
                    await httpClient.GetStringAsync(storeURL);

                    string purchaseURL = "https://store.tr.lol.riotgames.com/store/purchase/item";

                    List<KeyValuePair<string, string>> storeItemList = new List<KeyValuePair<string, string>>();
                    storeItemList.Add(new KeyValuePair<string, string>("item_id", "boosts_2"));
                    storeItemList.Add(new KeyValuePair<string, string>("currency_type", "rp"));
                    storeItemList.Add(new KeyValuePair<string, string>("quantity", "1"));
                    storeItemList.Add(new KeyValuePair<string, string>("rp", "260"));
                    storeItemList.Add(new KeyValuePair<string, string>("ip", "null"));
                    storeItemList.Add(new KeyValuePair<string, string>("duration_type", "PURCHASED"));
                    storeItemList.Add(new KeyValuePair<string, string>("duration", "3"));
                    HttpContent httpContent = new FormUrlEncodedContent(storeItemList);
                    await httpClient.PostAsync(purchaseURL, httpContent);

                    Tools.ConsoleMessage("Bought 'XP Boost: 3 Days'!", ConsoleColor.White);
                    httpClient.Dispose();
                }
                else if (region == "LAS")
                {
                    string url = await connection.GetStoreUrl();
                    HttpClient httpClient = new HttpClient();
                    Console.WriteLine(url);
                    await httpClient.GetStringAsync(url);

                    string storeURL = "https://store.la2.lol.riotgames.com/store/tabs/view/boosts/1";
                    await httpClient.GetStringAsync(storeURL);

                    string purchaseURL = "https://store.la2.lol.riotgames.com/store/purchase/item";

                    List<KeyValuePair<string, string>> storeItemList = new List<KeyValuePair<string, string>>();
                    storeItemList.Add(new KeyValuePair<string, string>("item_id", "boosts_2"));
                    storeItemList.Add(new KeyValuePair<string, string>("currency_type", "rp"));
                    storeItemList.Add(new KeyValuePair<string, string>("quantity", "1"));
                    storeItemList.Add(new KeyValuePair<string, string>("rp", "260"));
                    storeItemList.Add(new KeyValuePair<string, string>("ip", "null"));
                    storeItemList.Add(new KeyValuePair<string, string>("duration_type", "PURCHASED"));
                    storeItemList.Add(new KeyValuePair<string, string>("duration", "3"));
                    HttpContent httpContent = new FormUrlEncodedContent(storeItemList);
                    await httpClient.PostAsync(purchaseURL, httpContent);

                    Tools.ConsoleMessage("Bought 'XP Boost: 3 Days'!", ConsoleColor.White);
                    httpClient.Dispose();
                }
                else if (region == "LAN")
                {
                    string url = await connection.GetStoreUrl();
                    HttpClient httpClient = new HttpClient();
                    Console.WriteLine(url);
                    await httpClient.GetStringAsync(url);

                    string storeURL = "https://store.la1.lol.riotgames.com/store/tabs/view/boosts/1";
                    await httpClient.GetStringAsync(storeURL);

                    string purchaseURL = "https://store.la1.lol.riotgames.com/store/purchase/item";

                    List<KeyValuePair<string, string>> storeItemList = new List<KeyValuePair<string, string>>();
                    storeItemList.Add(new KeyValuePair<string, string>("item_id", "boosts_2"));
                    storeItemList.Add(new KeyValuePair<string, string>("currency_type", "rp"));
                    storeItemList.Add(new KeyValuePair<string, string>("quantity", "1"));
                    storeItemList.Add(new KeyValuePair<string, string>("rp", "260"));
                    storeItemList.Add(new KeyValuePair<string, string>("ip", "null"));
                    storeItemList.Add(new KeyValuePair<string, string>("duration_type", "PURCHASED"));
                    storeItemList.Add(new KeyValuePair<string, string>("duration", "3"));
                    HttpContent httpContent = new FormUrlEncodedContent(storeItemList);
                    await httpClient.PostAsync(purchaseURL, httpContent);

                    Tools.ConsoleMessage("Bought 'XP Boost: 3 Days'!", ConsoleColor.White);
                    httpClient.Dispose();
                }
                else if (region == "OCE")
                {
                    string url = await connection.GetStoreUrl();
                    HttpClient httpClient = new HttpClient();
                    Console.WriteLine(url);
                    await httpClient.GetStringAsync(url);

                    string storeURL = "https://store.oc1.lol.riotgames.com/store/tabs/view/boosts/1";
                    await httpClient.GetStringAsync(storeURL);

                    string purchaseURL = "https://store.oc1.lol.riotgames.com/store/purchase/item";

                    List<KeyValuePair<string, string>> storeItemList = new List<KeyValuePair<string, string>>();
                    storeItemList.Add(new KeyValuePair<string, string>("item_id", "boosts_2"));
                    storeItemList.Add(new KeyValuePair<string, string>("currency_type", "rp"));
                    storeItemList.Add(new KeyValuePair<string, string>("quantity", "1"));
                    storeItemList.Add(new KeyValuePair<string, string>("rp", "260"));
                    storeItemList.Add(new KeyValuePair<string, string>("ip", "null"));
                    storeItemList.Add(new KeyValuePair<string, string>("duration_type", "PURCHASED"));
                    storeItemList.Add(new KeyValuePair<string, string>("duration", "3"));
                    HttpContent httpContent = new FormUrlEncodedContent(storeItemList);
                    await httpClient.PostAsync(purchaseURL, httpContent);

                    Tools.ConsoleMessage("Bought 'XP Boost: 3 Days'!", ConsoleColor.White);
                    httpClient.Dispose();
                }
                else if (region == "JP")
                {
                    string url = await connection.GetStoreUrl();
                    HttpClient httpClient = new HttpClient();
                    Console.WriteLine(url);
                    await httpClient.GetStringAsync(url);

                    string storeURL = "https://store.jp1.lol.riotgames.com/store/tabs/view/boosts/1";
                    await httpClient.GetStringAsync(storeURL);

                    string purchaseURL = "https://store.jp1.lol.riotgames.com/store/purchase/item";

                    List<KeyValuePair<string, string>> storeItemList = new List<KeyValuePair<string, string>>();
                    storeItemList.Add(new KeyValuePair<string, string>("item_id", "boosts_2"));
                    storeItemList.Add(new KeyValuePair<string, string>("currency_type", "rp"));
                    storeItemList.Add(new KeyValuePair<string, string>("quantity", "1"));
                    storeItemList.Add(new KeyValuePair<string, string>("rp", "260"));
                    storeItemList.Add(new KeyValuePair<string, string>("ip", "null"));
                    storeItemList.Add(new KeyValuePair<string, string>("duration_type", "PURCHASED"));
                    storeItemList.Add(new KeyValuePair<string, string>("duration", "3"));
                    HttpContent httpContent = new FormUrlEncodedContent(storeItemList);
                    await httpClient.PostAsync(purchaseURL, httpContent);

                    Tools.ConsoleMessage("Bought 'XP Boost: 3 Days'!", ConsoleColor.White);
                    httpClient.Dispose();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OnLevelUp()
        {
            Tools.ConsoleMessage("Level Up: " + sumLevel, ConsoleColor.Yellow);
            rpBalance = loginPacket.RpBalance;
            ipBalance = loginPacket.IpBalance;
            //Tools.ConsoleMessage("Your Current RP: " + rpBalance, ConsoleColor.Yellow);
            Tools.ConsoleMessage("Current IP: " + ipBalance, ConsoleColor.Yellow);

            if (sumLevel >= Program.maxLevel)
            {
                Tools.ConsoleMessage("Your character reached the max level: " + Program.maxLevel, ConsoleColor.Red);
                connection.Disconnect();
                return;
            }

            if (rpBalance == 400.0 && Program.buyExpBoost)
            {
                Tools.ConsoleMessage("Buying XP Boost", ConsoleColor.White);
                try
                {
                    Task t = new Task(OnBuyBoost);
                    t.Start();
                }
                catch (Exception exception)
                {
                    Tools.ConsoleMessage("Couldn't buy RP Boost.\n" + exception, ConsoleColor.Red);
                }
            }

        }

        private static Random random = new Random((int)DateTime.Now.Ticks);
        private string RandomString(int size)
        {
            StringBuilder builder = new StringBuilder();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }
    }

    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            return source.Shuffle(new Random());
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, Random rng)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (rng == null) throw new ArgumentNullException("rng");

            return source.ShuffleIterator(rng);
        }

        private static IEnumerable<T> ShuffleIterator<T>(
            this IEnumerable<T> source, Random rng)
        {
            List<T> buffer = source.ToList();
            for (int i = 0; i < buffer.Count; i++)
            {
                int j = rng.Next(i, buffer.Count);
                yield return buffer[j];

                buffer[j] = buffer[i];
            }
        }
    }
}
