using FlippinTen.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace FlippinTen.Extensions
{
    public static class ObservableCollectionExtensions
    {
        public static void Update<T>(this ObservableCollection<T> collection, IList<T> updatedCollection) where T : IEquatable<T>
        {
            var cardsToRemove = collection.Except(updatedCollection).ToList();
            foreach (var card in cardsToRemove)
            {
                collection.Remove(card);
            }

            var cardsToAdd = updatedCollection.Except(collection).ToList();
            foreach (var card in cardsToAdd)
            {
                collection.Add(card);
            }
        }

        public static void Sort(this ObservableCollection<Card> collection)
        {
            for (var i = 0; i < collection.Count - 1; i++)
            {
                var maxIndex = i;
                for (var j = i + 1; j < collection.Count; j++)
                {
                    if (collection[j].Number > collection[maxIndex].Number)
                    {
                        maxIndex = j;
                    }
                }

                if (maxIndex != i)
                {
                    var tmp = collection[maxIndex];
                    collection[maxIndex] = collection[i];
                    collection[i] = tmp;
                }
            }
        }
    }
}
