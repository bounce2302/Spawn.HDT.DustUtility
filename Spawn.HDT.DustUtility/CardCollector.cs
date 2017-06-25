﻿using System;
using System.Collections.Generic;
using System.Windows;
using HearthDb.Enums;
using HearthMirror;
using HearthMirror.Objects;

namespace Spawn.HDT.DustUtility
{
    public class CardCollector
    {
        private List<CardWrapper> m_lstUnusedCards;

        public CardCollector()
        {
            m_lstUnusedCards = new List<CardWrapper>();
        }

        public CardWrapper[] GetDustableCards(Parameters parameters)
        {
            List<Card> lstCollection = Reflection.GetCollection();

            CheckForUnusedCards(lstCollection);

            List<CardWrapper> lstRet = new List<CardWrapper>();

            if (lstCollection.Count > 0)
            {
                int nTotalAmount = 0;

                bool blnDone = false;

                //Golden
                if (parameters.IncludeGoldenCards)
                {
                    ProcessCards(parameters, lstRet, true, ref nTotalAmount, ref blnDone);
                }
                else { }

                //Non golden
                ProcessCards(parameters, lstRet, false, ref nTotalAmount, ref blnDone);

                //Post processing
                //Remove low rarity cards if the total amount is over the targeted amount
                if (nTotalAmount > parameters.DustAmount)
                {
                    PostProcessCards(lstRet, parameters.Rarities, ref nTotalAmount); 
                }
                else { }
            }
            else { }

            return lstRet.ToArray();
        }

        private void PostProcessCards(List<CardWrapper> lstCards, List<Rarity> lstRarites, ref int nTotalAmount)
        {
            if (lstCards.Count > 0 && lstRarites.Count > 0)
            {
                bool blnDone = false;

                for (int i = 0; i < lstRarites.Count && !blnDone; i++)
                {
                    Rarity lowestRarity = lstRarites[i];

                    bool blnContinue = true;

                    do
                    {

                    }
                    while (blnContinue);
                }
            }
            else { }
        }

        private void ProcessCards(Parameters parameters, List<CardWrapper> lstRet, bool blnGolden, ref int nTotalAmount, ref bool blnDone)
        {
            for (int i = 0; i < parameters.Rarities.Count && !blnDone; i++)
            {
                List<CardWrapper> lstCards = GetCardsForRarity(parameters.Rarities[i], blnGolden);

                lstCards = FilterForClasses(lstCards, parameters.Classes);

                for (int j = 0; j < lstCards.Count && !blnDone; j++)
                {
                    CardWrapper card = lstCards[j];

                    nTotalAmount += card.GetDustValue();

                    lstRet.Add(card);

                    blnDone = nTotalAmount >= parameters.DustAmount;
                }
            }
        }

        public int GetTotalDustValueForAllCards()
        {
            List<Card> lstCards = Reflection.GetCollection();

            int nRet = 0;

            for (int i = 0; i < lstCards.Count; i++)
            {
                nRet += new CardWrapper(lstCards[i]).GetDustValue();
            }

            return nRet;
        }

        private void CheckForUnusedCards(List<Card> lstCollection)
        {
            if (lstCollection == null)
            {
                throw new ArgumentNullException("lstCollection");
            }
            else { }

            m_lstUnusedCards.Clear();

            List<Deck> lstDecks = Reflection.GetDecks();

            if (lstDecks.Count > 0 && lstDecks[0].Cards.Count > 0)
            {
                for (int i = 0; i < lstCollection.Count; i++)
                {
                    Card card = lstCollection[i];
                    CardWrapper cardWrapper = new CardWrapper(card);

                    for (int j = 0; j < lstDecks.Count; j++)
                    {
                        if (lstDecks[j].Contains(card.Id))
                        {
                            Card c = lstDecks[j].GetCard(card.Id);

                            if (c.Count > cardWrapper.MaxCountInDecks)
                            {
                                cardWrapper.MaxCountInDecks = c.Count;
                            }
                            else { }
                        }
                        else { }
                    }

                    if (cardWrapper.MaxCountInDecks <= 1 && cardWrapper.Card.Count > cardWrapper.MaxCountInDecks && !(cardWrapper.DBCard.Rarity == Rarity.LEGENDARY && cardWrapper.MaxCountInDecks == 1))
                    {
                        m_lstUnusedCards.Add(cardWrapper);
                    }
                    else { }
                }
            }
            else
            {
                MessageBox.Show("Navigate to the \"Play\" page first!", "Dust Utility", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private List<CardWrapper> FilterForClasses(List<CardWrapper> lstCards, List<CardClass> lstClasses)
        {
            List<CardWrapper> lstRet = new List<CardWrapper>();

            for (int i = 0; i < lstClasses.Count; i++)
            {
                List<CardWrapper> chunk = lstCards.FindAll(delegate (CardWrapper cw) { return cw.DBCard.Class == lstClasses[i]; });

                lstRet.AddRange(chunk);
            }

            return lstRet;
        }

        private List<CardWrapper> GetCardsForRarity(Rarity rarity, bool blnGolden = false)
        {
            return m_lstUnusedCards.FindAll(delegate (CardWrapper c) { return HearthDb.Cards.All[c.Card.Id].Rarity == rarity && c.Card.Premium == blnGolden; });
        }
    }
}
