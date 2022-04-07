using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Lens.Core.Lib.Extensions
{
    public static class EnumerableExtensions
    {
        private static Random random = new Random();

        /// <summary>
        /// If Enumerable is NULL, return empty (resolved Null Exception issues when trying to deal with an empty Enumerable)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">Enumerable</param>
        /// <returns></returns>
        public static IEnumerable<T> OrEmpty<T>(this IEnumerable<T> value)
        {
            return value ?? Enumerable.Empty<T>();
        }

        /// <summary>
        /// Any returns a Exception when enumerable is NULL. This subverts that by just returning false if it's null.
        /// (doesn't contain items anyway when NULL).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sequence"></param>
        /// <param name="predicate"></param>
        /// <returns>If enumerable contains any item matching the predicate</returns>
        public static bool AnyOrDefault<T>(this IEnumerable<T> sequence, Func<T, bool> predicate = null)
        {
            if (sequence == null)
            {
                return false;

            }

            if (predicate != null)
            {
                return sequence.Any(predicate);
            }

            return sequence.Any();
        }

        /// <summary>
        /// The difference between 2 lists, which can seperate added, deleted, unchanged results.
        /// Returns true if there indeed is a difference.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="originalList"></param>
        /// <param name="newList"></param>
        /// <param name="compareFunction"></param>
        /// <param name="addList"></param>
        /// <param name="deleteList"></param>
        /// <param name="unchangedList"></param>
        /// <returns></returns>
        public static bool DiffLists<T>(this IEnumerable<T> originalList, IEnumerable<T> newList, IEqualityComparer<T> compareFunction, out List<T> addList, out List<T> deleteList, out List<T> unchangedList)
        {
            addList = new List<T>();
            deleteList = new List<T>();
            unchangedList = new List<T>();

            bool hasChanged = false;

            if (newList != null)
            {
                foreach (var itemNew in newList)
                {
                    if (!originalList.Contains(itemNew, compareFunction))
                    {
                        addList.Add(itemNew);
                        hasChanged = true;
                    }
                    else
                    {
                        unchangedList.Add(itemNew);
                    }
                }
            }

            if (originalList != null)
            {
                foreach (var itemNew in originalList)
                {
                    if (newList == null || !newList.Contains(itemNew, compareFunction))
                    {
                        deleteList.Add(itemNew);
                        hasChanged = true;
                    }
                }
            }

            return hasChanged;
        }

        /// <summary>
        /// The difference between 2 lists, which can seperate added, deleted, unchanged results.
        /// Returns true if there indeed is a difference.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="originalList"></param>
        /// <param name="newList"></param>
        /// <param name="compareFunction"></param>
        /// <param name="addList"></param>
        /// <param name="deleteList"></param>
        /// <param name="unchangedList"></param>
        /// <returns></returns>
        public static bool DiffLists<T>(this IEnumerable<T> originalList, IEnumerable<T> newList, out List<T> addList, out List<T> deleteList, out List<T> unchangedList)
        {
            addList = new List<T>();
            deleteList = new List<T>();
            unchangedList = new List<T>();

            bool hasChanged = false;

            if (newList != null)
            {
                foreach (var itemNew in newList)
                {
                    if (!originalList.Contains(itemNew))
                    {
                        addList.Add(itemNew);
                        hasChanged = true;
                    }
                    else
                    {
                        unchangedList.Add(itemNew);
                    }
                }
            }

            if (originalList != null)
            {
                foreach (var itemNew in originalList)
                {
                    if (newList == null || !newList.Contains(itemNew))
                    {
                        deleteList.Add(itemNew);
                        hasChanged = true;
                    }
                }
            }

            return hasChanged;
        }

        /// <summary>
        /// Order By with direction. So you can use 1 function, instead of switching between OrderBy and OrderByDescending.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <param name="descending"></param>
        /// <returns></returns>
        public static IOrderedEnumerable<TSource> OrderByWithDirection<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, bool descending)
        {
            return descending ? source.OrderByDescending(keySelector)
                              : source.OrderBy(keySelector);
        }

        /// <summary>
        /// Order By with direction. So you can use 1 function, instead of switching between OrderBy and OrderByDescending.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <param name="descending"></param>
        /// <returns></returns>
        public static IOrderedQueryable<TSource> OrderByWithDirection<TSource, TKey>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, bool descending)
        {
            return descending ? source.OrderByDescending(keySelector)
                              : source.OrderBy(keySelector);
        }

        public static List<T> ShuffleListInPlace<T>(this List<T> inputList, int maxIterations = 0)
        {
            if (inputList.Count > 1)
            {
                for (int i = inputList.Count - 1; i >= 0; i--)
                {
                    T tmp = inputList[i];
                    int randomIndex = random.Next(i + 1);

                    if (randomIndex == i)
                    {
                        continue;
                    }

                    //Swap elements
                    inputList[i] = inputList[randomIndex];
                    inputList[randomIndex] = tmp;

                    if (maxIterations > 0 && i == (maxIterations - 1))
                    {
                        break;
                    }
                }
            }

            return inputList;
        }

    }
}
