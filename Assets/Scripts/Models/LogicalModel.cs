using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Avalon.Models
{
    public class LogicalModel<Element>
    {
        public int GoodCount;
        public int N
        {
            get { return AllElements.Count; }
        }

        public HashSet<Element> AllElements;
        public HashSet<HashSet<Element>> PossibleTeams;

        public LogicalModel(HashSet<Element> allElements, int goodCount)
        {
            GoodCount = goodCount;
            AllElements = allElements;
            InitializePossibleTeams();
        }

        public String PossibleTeamsToString()
        {
            String result = "Possible teams:";
            foreach (HashSet<Element> set in PossibleTeams)
            {
                result += "\n{";
                foreach (Element element in set)
                {
                    result += " " + element.ToString();
                }
                result += "}";
            }
            return result;
        }

        public void FilterContainsElement(Element element)
        {
            HashSet<HashSet<Element>> oldPossibleTeams = new HashSet<HashSet<Element>>(PossibleTeams);
            foreach (HashSet<Element> possibleTeam in oldPossibleTeams)
            {
                if (!possibleTeam.Contains(element))
                {
                    PossibleTeams.Remove(possibleTeam);
                }
            }
        }


        public void AddDirtySet(HashSet<Element> dirtySet)
        {
            HashSet<HashSet<Element>> oldPossibleTeams = new HashSet<HashSet<Element>>(PossibleTeams);
            foreach (HashSet<Element> possibleTeam in oldPossibleTeams)
            {
                if (dirtySet.IsSubsetOf(possibleTeam))
                {
                    PossibleTeams.Remove(possibleTeam);
                }
            }
        }

        public void AddCleanSet(HashSet<Element> cleanSet)
        {
            HashSet<HashSet<Element>> oldPossibleTeams = new HashSet<HashSet<Element>>(PossibleTeams);
            foreach (HashSet<Element> possibleTeam in oldPossibleTeams)
            {
                if (!cleanSet.IsSubsetOf(possibleTeam))
                {
                    PossibleTeams.Remove(possibleTeam);
                }
            }
        }

        private void InitializePossibleTeams()
        {
            HashSet<Element> NotChoosedElements = new HashSet<Element>(AllElements);
            PossibleTeams = new HashSet<HashSet<Element>>();
            HashSet<Element> empty = new HashSet<Element>();
            PossibleTeams.Add(empty);
            for (int i = 0; i < GoodCount; i++)
            {
                HashSet<HashSet<Element>> oldPossibleTeams = new HashSet<HashSet<Element>>(PossibleTeams);
                    //Utilities.CopyPowerSet<Element>(PossibleTeams);
                foreach (HashSet<Element> oldSet in oldPossibleTeams)
                {
                    foreach (Element elem in AllElements)
                    {
                        if (!oldSet.Contains(elem))
                        {
                            HashSet<Element> newSet = new HashSet<Element>(oldSet);
                            newSet.Add(elem);
                            if (PossibleTeams.All(set => !set.SetEquals(newSet)))
                            {
                                PossibleTeams.Add(newSet);
                            }
                        }
                    }
                    PossibleTeams.Remove(oldSet);
                }
            }
        }

        HashSet<HashSet<Element>> BuildUpRecursively (int level)
        {
            HashSet<HashSet<Element>> resultPowerSet = new HashSet<HashSet<Element>>();
            HashSet<HashSet<Element>> lastPowerSet = new HashSet<HashSet<Element>>();
            if (level > 0)
            {
                lastPowerSet = BuildUpRecursively(level - 1);
            }
            foreach (HashSet<Element> currentSet in lastPowerSet)
            {
                foreach (Element item in AllElements)
                {
                    if (!currentSet.Contains(item))
                    {
                        HashSet<Element> newSet = new HashSet<Element>(currentSet);
                        newSet.Add(item);
                        if (!resultPowerSet.Contains(newSet))
                        {
                            resultPowerSet.Add(newSet);
                        }
                    }
                }
            }
            return null;
        }
    }
}
