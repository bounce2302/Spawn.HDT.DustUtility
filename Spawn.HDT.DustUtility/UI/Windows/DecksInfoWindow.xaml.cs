﻿using HearthMirror.Objects;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Spawn.HDT.DustUtility.UI.Windows
{
    public partial class DecksInfoWindow
    {
        #region Member Variables
        private Account m_account;
        #endregion

        #region Ctor
        public DecksInfoWindow()
        {
            InitializeComponent();

            listView.ItemContainerGenerator.StatusChanged += OnItemContainerGeneratorStatusChanged;
        }

        public DecksInfoWindow(Account account)
            : this()
        {
            m_account = account;
        }
        #endregion

        #region Events
        #region OnLoaded
        private void OnLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            List<Deck> lstDecks = m_account.LoadDecks();

            for (int i = 0; i < lstDecks.Count; i++)
            {
                Deck deck = lstDecks[i];

                ListViewDeckItem deckItem = new ListViewDeckItem
                {
                    DeckId = deck.Id,
                    HeroImage = GetHeroImage(deck),
                    Name = deck.Name,
                    CardCount = $"{deck.GetCardCount()}/30",
                    Cost = $"{deck.GetCraftingCost().ToString()} Dust"
                };

                listView.Items.Add(deckItem);
            }
        }
        #endregion

        #region OnMenuItemClick
        private void OnMenuItemClick(object sender, System.Windows.RoutedEventArgs e)
        {
            ListViewItem item = GetListViewItem();

            ToggleListViewItemState(item);

            long nDeckId = (listView.SelectedItem as ListViewDeckItem).DeckId;

            if ((bool)item.Tag)
            {
                m_account.IncludeDeck(nDeckId);
            }
            else
            {
                m_account.ExcludeDeck(nDeckId);
            }
        }
        #endregion

        #region OnContextMenuOpening
        private void OnContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            MenuItem menuItem = GetContextMenuItem();

            ListViewItem item = GetListViewItem();

            if (item.Tag != null)
            {
                if ((bool)item.Tag)
                {
                    menuItem.Header = "Exclude (Search)";
                }
                else
                {
                    menuItem.Header = "Include (Search)";
                }
            }
            else
            {
                menuItem.Header = "Exclude (Search)";
            }
        }
        #endregion

        #region OnItemContainerGeneratorStatusChanged
        private void OnItemContainerGeneratorStatusChanged(object sender, EventArgs e)
        {
            if (listView.ItemContainerGenerator.Status == System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated)
            {
                for (int i = 0; i < listView.Items.Count; i++)
                {
                    ListViewDeckItem deckItem = listView.Items.GetItemAt(i) as ListViewDeckItem;

                    if (m_account.IsDeckExcluded(deckItem.DeckId))
                    {
                        ToggleListViewItemState(GetListViewItem(i));
                    }
                    else { }
                }
            }
            else { }
        }
        #endregion
        #endregion

        #region GetHeroImage
        public ImageSource GetHeroImage(Deck deck)
        {
            string strSource = string.Empty;

            switch (deck.Hero.Substring(0, 7))
            {
                case "HERO_01": //Warrior
                    strSource = "/Spawn.HDT.DustUtility;component/Resources/Garrosh.png";
                    break;

                case "HERO_02": //Shaman
                    strSource = "/Spawn.HDT.DustUtility;component/Resources/Thrall.png";
                    break;

                case "HERO_03": //Rogue
                    strSource = "/Spawn.HDT.DustUtility;component/Resources/Valeera.png";
                    break;

                case "HERO_04": //Paladin
                    strSource = "/Spawn.HDT.DustUtility;component/Resources/Uther.png";
                    break;

                case "HERO_05": //Hunter
                    strSource = "/Spawn.HDT.DustUtility;component/Resources/Rexxar.png";
                    break;

                case "HERO_06": //Druid
                    strSource = "/Spawn.HDT.DustUtility;component/Resources/Malfurion.png";
                    break;

                case "HERO_07": //Warlock
                    strSource = "/Spawn.HDT.DustUtility;component/Resources/Gul'dan.png";
                    break;

                case "HERO_08": //Mage
                    strSource = "/Spawn.HDT.DustUtility;component/Resources/Jaina.png";
                    break;

                case "HERO_09": //Priest
                    strSource = "/Spawn.HDT.DustUtility;component/Resources/Anduin.png";
                    break;
            }

            return new BitmapImage(new Uri(strSource, UriKind.Relative));
        }
        #endregion

        #region GetListViewItem
        private ListViewItem GetListViewItem(int nIndex = -1)
        {
            if (nIndex == -1)
            {
                nIndex = listView.SelectedIndex;
            }
            else { }

            return listView.ItemContainerGenerator.ContainerFromIndex(nIndex) as ListViewItem;
        }
        #endregion

        #region GetContextMenuItem
        private MenuItem GetContextMenuItem()
        {
            ContextMenu contextMenu = listView.Resources["contextMenu"] as ContextMenu;

            return contextMenu.Items.GetItemAt(0) as MenuItem;
        }
        #endregion

        #region ToggleListViewItemState
        private void ToggleListViewItemState(ListViewItem item)
        {
            if (item != null)
            {
                if (item.Opacity == 1)
                {
                    item.Opacity = .5;

                    item.Tag = false;
                }
                else
                {
                    item.Opacity = 1;

                    item.Tag = true;
                }
            }
            else { }
        }
        #endregion
    }
}