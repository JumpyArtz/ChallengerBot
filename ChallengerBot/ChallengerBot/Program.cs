using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
using System.Threading;
using System.Net;
using System.Management;
using PvPNetClient;
using System.Windows.Forms;
using System.Diagnostics;
using ChallengerBot.Utils;
using ChallengerBot;
using ChallengerBot.Utils;

namespace ChallengerBot
{
    class Program
    {
        public static string lolPath;
        public static ArrayList accounts = new ArrayList();
        public static ArrayList accountsNew = new ArrayList();
        public static int maxBots = 1;
        public static bool replaceConfig = false;
        public static string firstChampionPick = "";
        public static string secondChampionPick = "";
        public static string thirdChampionPick = "";
        public static string fourthChampionPick = "";
        public static string fifthChampionPick = "";
        public static int maxLevel = 30;
        public static bool randomSpell = false;
        public static string spell1 = "flash";
        public static string spell2 = "ignite";
        public static string LoLVersion = "";
        public static bool buyExpBoost = false;

        public static int delay1 = 1;
        public static int delay2 = 1;

        //lol screen
        public static int lolHeight = 200;
        public static int lolWidth = 300;
        public static bool LOWPriority = true;

        private static void Main(string[] args)
        {
            LoadConfigs();
            LoadLeagueVersion();

            Console.Title = "ChallengerBot";
            Tools.TitleMessage("ChallengerBot - Bot for Leauge Of Legends: " + LoLVersion.Substring(0, 4));
            Tools.TitleMessage("Current Version: " + Tools.ezVersion.ToString());
            Tools.TitleMessage("Made by JumpyArtz.");

            Tools.ConsoleMessage("Config loaded.", ConsoleColor.White);

            if (replaceConfig)
            {
                Tools.ConsoleMessage("Changing Game Config.", ConsoleColor.White);
                ChangeGameConfig();
            }

            Tools.ConsoleMessage("Loading accounts.", ConsoleColor.White);
            LoadAccounts();
            int curRunning = 0;
            foreach (string acc in accounts)
            {
                try
                {
                    accountsNew.RemoveAt(0);
                    string Accs = acc;
                    string[] stringSeparators = new string[] { "|" };
                    var result = Accs.Split(stringSeparators, StringSplitOptions.None);
                    curRunning += 1;

                    QueueTypes queuetype;
                    ChallengerBot ChallengerBot;
                    if (result[3] != null)
                    {
                        Generator.CreateRandomThread(delay1, delay2);
                        queuetype = (QueueTypes)System.Enum.Parse(typeof(QueueTypes), result[3]);
                        ChallengerBot = new ChallengerBot(result[0], result[1], result[2].ToUpper(), lolPath, queuetype, LoLVersion);
                    }
                    else
                    {
                        Generator.CreateRandomThread(delay1, delay2);
                        queuetype = QueueTypes.ARAM;
                        ChallengerBot = new ChallengerBot(result[0], result[1], result[2].ToUpper(), lolPath, queuetype, LoLVersion);
                    }

                    if (curRunning == maxBots)
                        break;
                    Tools.ConsoleMessage("Maximun bots running: " + maxBots, ConsoleColor.Red);
                }
                catch (Exception)
                {
                    Tools.ConsoleMessage("You may have an issue in your accounts.txt", ConsoleColor.Red);
                    Tools.ConsoleMessage("Acconts structure ACCOUNT|PASSWORD|REGION|QUEUE_TYPE", ConsoleColor.Red);
                    Console.ReadKey();
                }
            }
        }

        public static void LoadLeagueVersion()
        {
            var versiontxt = File.OpenText(AppDomain.CurrentDomain.BaseDirectory + @"configs\version.txt");
            LoLVersion = versiontxt.ReadLine();
        }

        private static void ChangeGameConfig()
        {
            try
            {
                string path = lolPath + @"Config\game.cfg";
                FileInfo fileInfo = new FileInfo(path);
                fileInfo.IsReadOnly = false;
                fileInfo.Refresh();
                string str = "[General]\nGameMouseSpeed=9\nEnableAudio=0\nUserSetResolution=1\nBindSysKeys=0\nSnapCameraOnRespawn=1\nOSXMouseAcceleration=1\nAutoAcquireTarget=0\nEnableLightFx=0\nWindowMode=1\nShowTurretRangeIndicators=0\nPredictMovement=0\nWaitForVerticalSync=0\nColors=16\nHeight=" + lolHeight + "\nWidth=" + lolWidth + "\nSystemMouseSpeed=0\nCfgVersion=4.13.265\n\n[HUD]\nShowNeutralCamps=0\nDrawHealthBars=0\nAutoDisplayTarget=0\nMinimapMoveSelf=0\nItemShopPrevY=19\nItemShopPrevX=117\nShowAllChannelChat=0\nShowTimestamps=0\nObjectTooltips=0\nFlashScreenWhenDamaged=0\nNameTagDisplay=1\nShowChampionIndicator=0\nShowSummonerNames=0\nScrollSmoothingEnabled=0\nMiddleMouseScrollSpeed=0.5000\nMapScrollSpeed=0.5000\nShowAttackRadius=0\nNumericCooldownFormat=3\nSmartCastOnKeyRelease=0\nEnableLineMissileVis=0\nFlipMiniMap=0\nItemShopResizeHeight=47\nItemShopResizeWidth=455\nItemShopPrevResizeHeight=200\nItemShopPrevResizeWidth=300\nItemShopItemDisplayMode=1\nItemShopStartPane=1\n\n[Performance]\nShadowsEnabled=0\nEnableHUDAnimations=0\nPerPixelPointLighting=0\nEnableParticleOptimizations=0\nBudgetOverdrawAverage=10\nBudgetSkinnedVertexCount=10\nBudgetSkinnedDrawCallCount=10\nBudgetTextureUsage=10\nBudgetVertexCount=10\nBudgetTriangleCount=10\nBudgetDrawCallCount=1000\nEnableGrassSwaying=0\nEnableFXAA=0\nAdvancedShader=0\nFrameCapType=3\nGammaEnabled=1\nFull3DModeEnabled=0\nAutoPerformanceSettings=0\n=0\nEnvironmentQuality=0\nEffectsQuality=0\nShadowQuality=0\nGraphicsSlider=0\n\n[Volume]\nMasterVolume=1\nMusicMute=0\n\n[LossOfControl]\nShowSlows=0\n\n[ColorPalette]\nColorPalette=0\n\n[FloatingText]\nCountdown_Enabled=0\nEnemyTrueDamage_Enabled=0\nEnemyMagicalDamage_Enabled=0\nEnemyPhysicalDamage_Enabled=0\nTrueDamage_Enabled=0\nMagicalDamage_Enabled=0\nPhysicalDamage_Enabled=0\nScore_Enabled=0\nDisable_Enabled=0\nLevel_Enabled=0\nGold_Enabled=0\nDodge_Enabled=0\nHeal_Enabled=0\nSpecial_Enabled=0\nInvulnerable_Enabled=0\nDebug_Enabled=1\nAbsorbed_Enabled=1\nOMW_Enabled=1\nEnemyCritical_Enabled=0\nQuestComplete_Enabled=0\nQuestReceived_Enabled=0\nMagicCritical_Enabled=0\nCritical_Enabled=1\n\n[Replay]\nEnableHelpTip=0";
                StringBuilder builder = new StringBuilder();
                builder.AppendLine(str);
                using (StreamWriter writer = new StreamWriter(lolPath + @"Config\game.cfg"))
                {
                    writer.Write(builder.ToString());
                }
                fileInfo.IsReadOnly = true;
                fileInfo.Refresh();
            }
            catch (Exception exception2)
            {
                Tools.ConsoleMessage("game.cfg Error: If using VMWare Shared Folder, make sure it is not set to Read-Only.\nException:" + exception2.Message, ConsoleColor.Red);
            }
        }


        public static void LognNewAccount()
        {
            accountsNew = accounts;
            accounts.RemoveAt(0);
            int curRunning = 0;
            if (accounts.Count == 0)
            {
                Tools.ConsoleMessage("No more acocunts to login", ConsoleColor.Red);
            }
            foreach (string acc in accounts)
            {
                string Accs = acc;
                string[] stringSeparators = new string[] { "|" };
                var result = Accs.Split(stringSeparators, StringSplitOptions.None);
                curRunning += 1;

                QueueTypes queuetype;
                ChallengerBot ChallengerBot;
                if (result[3] != null)
                {
                    Generator.CreateRandomThread(delay1, delay2);
                    queuetype = (QueueTypes)System.Enum.Parse(typeof(QueueTypes), result[3]);
                    ChallengerBot = new ChallengerBot(result[0], result[1], result[2].ToUpper(), lolPath, queuetype, LoLVersion);
                }
                else
                {
                    Generator.CreateRandomThread(delay1, delay2);
                    queuetype = QueueTypes.ARAM;
                    ChallengerBot = new ChallengerBot(result[0], result[1], result[2].ToUpper(), lolPath, queuetype, LoLVersion);
                }

                if (curRunning == maxBots)
                     break;
                Tools.ConsoleMessage("Maximun bots running: " + maxBots, ConsoleColor.Red);
            }
        }

        public static void LoadConfigs()
        {
            try
            {
                IniFile iniFile = new IniFile(AppDomain.CurrentDomain.BaseDirectory + @"configs\settings.ini");

                //General
                lolPath = iniFile.Read("GENERAL", "LauncherPath");
                maxBots = Convert.ToInt32(iniFile.Read("GENERAL", "MaxBots"));
                maxLevel = Convert.ToInt32(iniFile.Read("GENERAL", "MaxLevel"));
                randomSpell = Convert.ToBoolean(iniFile.Read("GENERAL", "RandomSpell"));
                spell1 = iniFile.Read("GENERAL", "Spell1").ToUpper();
                spell2 = iniFile.Read("GENERAL", "Spell2").ToUpper();

                //Account
                delay1 = Convert.ToInt32(iniFile.Read("ACCOUNT", "MinDelay"));
                delay2 = Convert.ToInt32(iniFile.Read("ACCOUNT", "MaxDelay"));
                buyExpBoost = Convert.ToBoolean(iniFile.Read("ACCOUNT", "BuyExpBoost"));

                //Champions
                firstChampionPick = iniFile.Read("CHAMPIONS", "FirstChampionPick").ToUpper();
                secondChampionPick = iniFile.Read("CHAMPIONS", "SecondChampionPick").ToUpper();
                thirdChampionPick = iniFile.Read("CHAMPIONS", "ThirdChampionPick").ToUpper();
                fourthChampionPick = iniFile.Read("CHAMPIONS", "FourthChampionPick").ToUpper();
                fifthChampionPick = iniFile.Read("CHAMPIONS", "FifthChampionPick").ToUpper();

                //LOL Sreen
                replaceConfig = Convert.ToBoolean(iniFile.Read("LOLSCREEN", "ReplaceLoLConfig"));
                lolHeight = Convert.ToInt32(iniFile.Read("LOLSCREEN", "SreenHeight"));
                lolWidth = Convert.ToInt32(iniFile.Read("LOLSCREEN", "SreenWidth"));
                LOWPriority = Convert.ToBoolean(iniFile.Read("LOLSCREEN", "LOWPriority"));
            }
            catch (Exception e)
            {
                Tools.Log(e.Message);
                Thread.Sleep(10000);
                Application.Exit();
            }
        }

        public static void LoadAccounts()
        {
            TextReader tr = File.OpenText(AppDomain.CurrentDomain.BaseDirectory + @"configs\accounts.txt");
            string line;
            while ((line = tr.ReadLine()) != null)
            {
                accounts.Add(line);
                accountsNew.Add(line);
            }
            tr.Close();
        }
    }
}