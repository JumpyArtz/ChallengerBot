using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace PvPNetClient
{
    [Flags]
    public enum GuiState
    {
        None = 1,
        LoggedOut = 1 << 1,
        LoggingIn = 1 << 2,
        LoggedIn = 1 << 3,
        CustomSearchGame = 1 << 4,
        CustomCreateGame = 1 << 5,
        GameLobby = 1 << 6
    }

    [Flags]
    public enum GameLobbyGuiState
    {
        IDLE = 1,
        TEAM_SELECT = 1 << 1,
        CHAMP_SELECT = 1 << 2,
        POST_CHAMP_SELECT = 1 << 3,
        PRE_CHAMP_SELECT = 1 << 4,
        START_REQUESTED = 1 << 5,
        GAME_START_CLIENT = 1 << 6,
        GameClientConnectedToServer = 1 << 7,
        IN_PROGRESS = 1 << 8,
        IN_QUEUE = 1 << 9,
        POST_GAME = 1 << 10,
        TERMINATED = 1 << 11,
        TERMINATED_IN_ERROR = 1 << 12,
        CHAMP_SELECT_CLIENT = 1 << 13,
        GameReconnect = 1 << 14,
        GAME_IN_PROGRESS = 1 << 15,
        JOINING_CHAMP_SELECT = 1 << 16,
        DISCONNECTED = 1 << 17
    }

    /// <summary>
    /// Game Modes enumerator.
    /// </summary>
    public enum GameMode
    {
        [StringValue("CLASSIC")]
        SummonersRift = 1,
        [StringValue("CLASSIC")]
        TwistedTreeline = 10,
        [StringValue("ARAM")]
        HowlingAbyss = 12,
        [StringValue("TUTORIAL")]
        Tutorial,
    }

    /// <summary>
    /// Seasons enumerator.
    /// </summary>
    public enum CompetitiveSeason
    {
        [StringValue("CURRENT")]
        Current,

        [StringValue("ONE")]
        One,

        [StringValue("TWO")]
        Two,

        [StringValue("THREE")]
        Three,

        [StringValue("FOUR")]
        Four,

        [StringValue("FIVE")]
        Five,

        [StringValue("SIX")]
        Six
    }

    /// <summary>
    /// Game types enumerator.
    /// </summary>
    public enum GameType
    {
        [StringValue("RANKED_TEAM_GAME")]
        RankedTeamGame,

        [StringValue("RANKED_GAME")]
        RankedGame,

        [StringValue("NORMAL_GAME")]
        NormalGame,

        [StringValue("CUSTOM_GAME")]
        CustomGame,

        [StringValue("TUTORIAL_GAME")]
        TutorialGame,

        [StringValue("PRACTICE_GAME")]
        PracticeGame,

        [StringValue("RANKED_GAME_SOLO")]
        RankedGameSolo,

        [StringValue("COOP_VS_AI")]
        CoopVsAi,

        [StringValue("RANKED_GAME_PREMADE")]
        RankedGamePremade
    }

    public enum CustomGameTypes
    {
        [StringValue("unknown")]
        Unknown1,
        [StringValue("Blind Pick")]
        BlindPick,
        [StringValue("Draft")]
        Draft,
        [StringValue("No Ban Draft")]
        NoBanDraft,
        [StringValue("AllRandom")]
        AllRandom,
        [StringValue("Tournament Draft")]
        TournamentDraft,
        [StringValue("Blind Draft")]
        BlindDraft,
        [StringValue("unknown")]
        Unknown2,
        [StringValue("unknown")]
        Unknown3,
        [StringValue("Tutorial")]
        Tutorial,
        [StringValue("Battle Training")]
        BattleTraining,
        [StringValue("Bugged Blind Pick")]
        BuggedBlindPick,
        [StringValue("Blind Random")]
        BlindRandom,
        [StringValue("Blind Duplicate")]
        BlindDuplicate
    }
    /// <summary>
    /// Queue types Enumeartor.
    /// </summary>
    /// 
    public class QueueTypes2
    {
        public Dictionary<String, Int32> dict = new Dictionary<String, Int32>()
        {
            {"NORMAL_5x5", 2},
            {"RANKED_SOLO_5x5", 4},
            {"BOT_5x5", 7},
            {"NORMAL_3x3", 8},
            {"NORMAL_5x5_draft", 14},
            {"RANKED_TEAM_3x3", 41},
            {"RANKED_TEAM_5x5", 42},
            {"BOT_TT_3x3", 52},
            {"ARAM_5x5", 65}
        };
    }

    public enum QueueTypes
    {
        NORMAL_5x5 = 2,
        NORMAL_3x3 = 8,
        ODIN = 16,
        BOT_INTO = 31,
        BOT_BEGINNER = 32,
        BOT_MEDIUM = 33,
        BOT_3x3 = 52,
        ARAM = 65,
        ONEFORALL = 70,
        SR_HEXAKILL = 75,
        URF = 76,
        ASCENSION = 96,
        TT_HEXAKILL = 98,
        CUSTOM_ARAM = 997,
        CUSTOM = 999
    }

    public enum AllowSpectators
    {
        [StringValue("ALL")]
        All = 1,

        [StringValue("LOBBYONLY")]
        LobbyOnly = 2,

        [StringValue("DROPINONLY")]
        DropInOnly = 3,

        [StringValue("NONE")]
        None = 0
    }

    /// <summary>
    /// The StringEnum value with GetStringValue method
    /// </summary>
    public static class StringEnum
    {
        /// <summary>
        /// Gets the string value from Atrribute.
        /// </summary>
        /// <param name="value">Enum value.</param>
        /// <returns></returns>
        public static string GetStringValue(Enum value)
        {
            string output = null;
            Type type = value.GetType();

            //Check first in our cached results...

            //Look for our 'StringValueAttribute' 

            //in the field's custom attributes

            FieldInfo fi = type.GetField(value.ToString());
            StringValue[] attrs =
               fi.GetCustomAttributes(typeof(StringValue),
                                       false) as StringValue[];
            if (attrs.Length > 0)
            {
                output = attrs[0].Value;
            }

            return output;
        }
    }

    public class StringValue : Attribute
    {
        private string _value;

        public StringValue(string value)
        {
            _value = value;
        }

        public string Value
        {
            get { return _value; }
        }
    }
}
