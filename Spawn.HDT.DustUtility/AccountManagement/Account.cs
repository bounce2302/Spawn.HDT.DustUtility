﻿#region Using
using HearthMirror;
using HearthMirror.Objects;
using Hearthstone_Deck_Tracker.Enums;
using Hearthstone_Deck_Tracker.Utility.Logging;
using Spawn.HDT.DustUtility.CardManagement.Offline;
using System;
using System.Collections.Generic;
using System.Diagnostics;
#endregion

namespace Spawn.HDT.DustUtility.AccountManagement
{
    [DebuggerDisplay("{AccountString}")]
    public class Account
    {
        #region Constants
        public const string CollectionString = "collection";
        public const string DecksString = "decks";
        public const string PreferencesString = "prefs";
        public const string HistoryString = "history";
        #endregion

        #region Static Properties
        #region Empty
        public static Account Empty => new Account(null, Region.UNKNOWN);
        #endregion

        #region Test
        public static Account Test => new Account(new BattleTag() { Name = "Test", Number = 123 }, Region.EU);
        #endregion

        #region LoggedInAccount
        public static Account LoggedInAccount => GetLoggedInAccount();
        #endregion
        #endregion

        #region Member Variables
        private AccountPreferences m_preferences;
        #endregion

        #region Properties
        #region BattleTag
        public BattleTag BattleTag { get; }
        #endregion

        #region Region
        public Region Region { get; }
        #endregion

        #region DisplayString
        public string DisplayString { get; }
        #endregion

        #region AccountString
        public string AccountString { get; }
        #endregion

        #region IsEmpty
        public bool IsEmpty => BattleTag == null && Region == Region.UNKNOWN;
        #endregion

        #region IsValid
        public bool IsValid => !IsEmpty && !string.IsNullOrEmpty(AccountString);
        #endregion

        #region Preferences
        public AccountPreferences Preferences => m_preferences ?? (m_preferences = AccountPreferences.Load(this));
        #endregion

        #region HasFiles
        public bool HasFiles => System.IO.File.Exists(DustUtilityPlugin.GetFullFileName(this, CollectionString))
            || System.IO.File.Exists(DustUtilityPlugin.GetFullFileName(this, DecksString))
            || System.IO.File.Exists(DustUtilityPlugin.GetFullFileName(this, HistoryString))
            || System.IO.File.Exists(DustUtilityPlugin.GetFullFileName(this, PreferencesString));
        #endregion
        #endregion

        #region Ctor
        private Account(BattleTag battleTag, Region region)
        {
            BattleTag = battleTag;
            Region = region;

            if (BattleTag != null)
            {
                AccountString = $"{battleTag.Name}_{battleTag.Number}_{region}";

                DisplayString = $"{battleTag.Name}#{battleTag.Number} ({region})";
            }
            else
            {
                AccountString = string.Empty;

                DisplayString = string.Empty;
            }
        }
        #endregion

        #region GetCollection
        public List<Card> GetCollection()
        {
            List<Card> lstRet = null;

            Log.WriteLine("Loading collection...", LogType.Debug);

            if (DustUtilityPlugin.IsOffline)
            {
                lstRet = Cache.LoadCollection(this);
            }
            else
            {
                lstRet = Reflection.GetCollection();
            }

            if (lstRet != null)
            {
                Log.WriteLine("Loaded collection", LogType.Debug);
            }
            else
            {
                Log.WriteLine("Couldn't load collection!", LogType.Error);
            }

            return lstRet;
        }
        #endregion

        #region GetDecks
        public List<Deck> GetDecks()
        {
            List<Deck> lstRet = null;

            Log.WriteLine("Loading decks...", LogType.Debug);

            if (DustUtilityPlugin.IsOffline)
            {
                lstRet = Cache.LoadDecks(this);
            }
            else
            {
                lstRet = Reflection.GetDecks();
            }

            if (lstRet != null)
            {
                Log.WriteLine("Loaded decks", LogType.Debug);
            }
            else
            {
                Log.WriteLine("Couldn't load decks!", LogType.Error);
            }

            return lstRet;
        }
        #endregion

        #region ExcludeDeck
        public void ExcludeDeck(long nDeckId)
        {
            if (!IsDeckExcluded(nDeckId))
            {
                Preferences.ExcludedDecks.Add(nDeckId);
            }
            else { }
        }
        #endregion

        #region IncludeDeck
        public void IncludeDeck(long nDeckId)
        {
            if (IsDeckExcluded(nDeckId))
            {
                Preferences.ExcludedDecks.Remove(nDeckId);
            }
            else { }
        }
        #endregion

        #region IsDeckExcluded
        public bool IsDeckExcluded(long nDeckId)
        {
            return Preferences.ExcludedDecks.Contains(nDeckId);
        }
        #endregion

        #region SavePreferences
        public void SavePreferences()
        {
            if (m_preferences != null)
            {
                m_preferences.Save(this);

                Log.WriteLine($"Saved preferences for {AccountString}", LogType.Debug);
            }
            else { }
        }
        #endregion

        #region Equals
        public override bool Equals(object obj)
        {
            bool blnRet = false;

            if (obj is Account)
            {
                Account acc = obj as Account;

                blnRet = true;

                if (acc.BattleTag != null)
                {
                    blnRet &= acc.BattleTag.Name.Equals(BattleTag.Name);

                    blnRet &= acc.BattleTag.Number == BattleTag.Number;
                }
                else { }

                blnRet &= acc.Region == Region;
            }
            else
            {
                blnRet = base.Equals(obj);
            }

            return blnRet;
        }
        #endregion

        #region GetHashCode
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion

        #region Static Methods
        #region Parse
        public static Account Parse(string strAccountString)
        {
            string[] vTemp = strAccountString.Split('_');

            BattleTag battleTag = new BattleTag()
            {
                Name = vTemp[0],
                Number = Convert.ToInt32(vTemp[1])
            };

            return new Account(battleTag, (Region)Enum.Parse(typeof(Region), vTemp[2]));
        }
        #endregion

        #region GetLoggedInAccount
        private static Account GetLoggedInAccount()
        {
            Account retVal = Empty;

            if (Hearthstone_Deck_Tracker.API.Core.Game.IsRunning)
            {
                retVal = new Account(Reflection.GetBattleTag(), Hearthstone_Deck_Tracker.Helper.GetCurrentRegion().Result);
            }
            else { }

            return retVal;
        }
        #endregion
        #endregion
    }
}