using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Collection_Game_Tool.Services 
{
    public class FileGenerationService : Teller
    {
        private List<Listener> audiance = new List<Listener>();
        private int extraPermutationBuffer = 10000;

        public FileGenerationService() { }

        public void buildGameData(
            Divisions.DivisionsModel divisions,
            PrizeLevels.PrizeLevels prizeLevels,
            GameSetup.GameSetupModel gameInfo,
            string fileName, GameSetup.GameSetupUC gameSetupUC)
        {
            addListener(gameSetupUC);
            int numberOfDivisions = divisions.getNumberOfDivisions() + 1;
            List<int[]>[] divisionLevles = new List<int[]>[numberOfDivisions];
            List<Thread> threads = new List<Thread>();
            for (int i = 0; i < numberOfDivisions; i++)
            {
                int divisionIndex = i;
                Thread t;
                if (divisionIndex == numberOfDivisions - 1)
                {
                    t = new Thread(() => divisionLevles[divisionIndex] = getDivisionLossingPermutations(gameInfo, prizeLevels).OrderBy(a => Guid.NewGuid()).ToList());
                }
                else
                {
                    t = new Thread(() => divisionLevles[divisionIndex] = getDivisionWinningPermutations(divisionIndex, gameInfo.totalPicks, (int)gameInfo.maxPermutations, divisions.getDivision(divisionIndex), prizeLevels).OrderBy(a => Guid.NewGuid()).ToList());
                }
                t.Start();
                threads.Add(t);
            }
            foreach (Thread t in threads)
            {
                t.Join();
            }
            writeFile(fileName, divisionLevles);
            shout("FileFinished");
        }

        /// <summary>
        /// Gets all losing permutations for a given game
        /// </summary>
        /// <param name="gameInfo">All of the game information</param>
        /// <param name="prizeLevels">All of the prizelevel informaiton for the game</param>
        /// <returns>returns a list of int[] where each int[] is a unique permutation that has no winning selections</returns>
        private List<int[]> getDivisionLossingPermutations(
            GameSetup.GameSetupModel gameInfo,
            PrizeLevels.PrizeLevels prizeLevels
            )
        {
            List<int[]> lossPermutations = new List<int[]>();
            List<int> extraPicks = getExtraPicks(new int[1], prizeLevels).ToList();
            List<int[]> baseLossconditions = new List<int[]>();
            int numberOfPermutationsForNearWinAmount = (int)gameInfo.maxPermutations;
            if (gameInfo.isNearWin)
            {
                numberOfPermutationsForNearWinAmount = (int)(gameInfo.maxPermutations / gameInfo.nearWins);
                baseLossconditions.AddRange(getBaseNearWinLosspermutations(gameInfo.nearWins, gameInfo.totalPicks, prizeLevels));
            }
            else
            {
                int[] baseLoss = new int[gameInfo.totalPicks];
                baseLossconditions.Add(baseLoss);
            }
            lossPermutations = getAllLossPermutations(baseLossconditions, prizeLevels, gameInfo.nearWins, numberOfPermutationsForNearWinAmount);
            return lossPermutations;
        }

        /// <summary>
        /// Gets all permutations for a losing condition. Can be different if near win is needed.
        /// </summary>
        /// <param name="baseLossconditions">The base set of lossing permutations</param>
        /// <param name="prizeLevels">All of the prize level information</param>
        /// <param name="numberOfNearWins">How many near wins can there be</param>
        /// <param name="maxNumberOfpermutationsPerNearWin">Maximum number of permutations </param>
        /// <returns> Returns a list of all losing permutations</returns>
        private List<int[]> getAllLossPermutations(
            List<int[]> baseLossconditions,
            PrizeLevels.PrizeLevels prizeLevels,
            int numberOfNearWins,
            int maxNumberOfpermutationsPerNearWin)
        {
            List<int[]> lossPermituations = new List<int[]>();
            int[] nearWinCounts = new int[numberOfNearWins + 1];

            foreach (int[] lossCondition in baseLossconditions)
            {
                int nearWinType = 0;
                for (int i = 1; i < lossCondition.Length; i++)
                {
                    if (lossCondition[i] != lossCondition[i - 1])
                    {
                        nearWinType++;
                    }
                }

                int[] bsaePermuitation = new int[lossCondition.Length];
                lossCondition.CopyTo(bsaePermuitation, 0);
                bool ableToFindNextdivision = true;
                for (int i = nearWinCounts[nearWinType]; i < maxNumberOfpermutationsPerNearWin && ableToFindNextdivision; i++)
                {
                    int[] newPermuitation = new int[lossCondition.Length];
                    lossCondition.CopyTo(newPermuitation, 0);
                    if (ableToFindNextdivision)
                    {
                        lossPermituations.Add(newPermuitation);
                        nearWinCounts[nearWinType]++;
                    }
                    bsaePermuitation = findNextPermutation(lossCondition);
                    ableToFindNextdivision = !(bsaePermuitation[0] == -1);
                }
            }
            return createExtrapermutations(lossPermituations, maxNumberOfpermutationsPerNearWin, prizeLevels); ;
        }

        private List<int[]> getBaseNearWinLosspermutations(
            int nearWinPrizeLevels,
            int totalNumberOfPicks,
            PrizeLevels.PrizeLevels prizeLevels)
        {
            List<int[]> nearWinBasepermutations = new List<int[]>();
            List<int[]> prizeLevelCombinations = getPrizeLevelCombinationsForNearWins(nearWinPrizeLevels, totalNumberOfPicks, prizeLevels);
            foreach (int[] combo in prizeLevelCombinations)
            {
                List<int> neededPicks = new List<int>();
                for (int i = 0; i < combo.Length; i++)
                {
                    int prizeLevelToSelect = combo[i];
                    for (int k = 0; k < prizeLevels.getPrizeLevel(prizeLevelToSelect).numCollections - 1; k++)
                    {
                        neededPicks.Add(combo[i] + 1);
                    }
                }
                nearWinBasepermutations.Add(getBaseCombinaiton(totalNumberOfPicks, neededPicks.ToArray()).ToArray());
            }
            return nearWinBasepermutations;
        }

        private List<int[]> getPrizeLevelCombinationsForNearWins(
            int nearWinPrizeLevels,
            int totalNumberOfPicks,
            PrizeLevels.PrizeLevels prizeLevels)
        {
            int numberOfPrizeLevels = prizeLevels.getNumPrizeLevels();
            List<int[]> prizeLevelCombinations = new List<int[]>();
            int numberOfMaximumCombinations = prizeLevels.getNumPrizeLevels();

            List<int> prizeLevelsIndexes = new List<int>();
            for (int i = 0; i < numberOfPrizeLevels; i++)
            {
                prizeLevelsIndexes.Add(i);
            }
            // Max possible Base
            for (int i = 1; i <= nearWinPrizeLevels; i++)
            {
                for (int j = 0; j < numberOfMaximumCombinations; j++)
                {
                    Random rand = new Random();
                    int[] tempPrizeLevelCombinations = new int[i];
                    int numberOfPicksForPrizeLevelCombintation = 0;
                    bool newBaseComboAdded = false;
                    List<int> tempPrizeLevelIndexes = new List<int>(prizeLevelsIndexes);
                    do
                    {
                        for (int k = 0; k < i; k++)
                        {
                            int randomPrizeLevelSelection = rand.Next(0, tempPrizeLevelIndexes.Count);
                            numberOfPicksForPrizeLevelCombintation += prizeLevels.getPrizeLevel(randomPrizeLevelSelection).numCollections - 1;
                            tempPrizeLevelCombinations[k] = randomPrizeLevelSelection;
                            tempPrizeLevelIndexes.RemoveAt(randomPrizeLevelSelection);
                        }
                        if (!prizeLevelCombinations.Any(tempPrizeLevelCombinations.SequenceEqual) && numberOfPicksForPrizeLevelCombintation <= totalNumberOfPicks)
                        {
                            prizeLevelCombinations.Add(tempPrizeLevelCombinations);
                            newBaseComboAdded = true;
                        }
                        tempPrizeLevelCombinations = new int[i];
                        numberOfPicksForPrizeLevelCombintation = 0;
                        tempPrizeLevelIndexes = new List<int>(prizeLevelsIndexes);
                    } while (!newBaseComboAdded);
                }
            }
            return prizeLevelCombinations;
        }

        private void writeFile(string fileName, List<int[]>[] divisionLevles)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(fileName))
            {
                List<string> lines = new List<string>();
                int divisionIndicator = 0;
                foreach (List<int[]> li in divisionLevles)
                {
                    foreach (int[] i in li)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append((divisionIndicator + 1) + " ");

                        for (int j = 0; j < i.Length; j++)
                        {
                            if (i[j] > 0)
                            {
                                sb.Append(", " + charFromInt(i[j]));
                            }
                            else
                            {
                                sb.Append("," + "W:" + charFromInt((i[j] * -1)));
                            }
                        }
                        lines.Add(sb.ToString());
                    }
                    divisionIndicator++;
                }
                foreach (string s in lines)
                {
                    file.WriteLine(s);
                }
            }
        }

        //Creates the collection of win permutations
        private List<int[]> getDivisionWinningPermutations(
            int divisionIndicator,
            short totalNumberOfPicks,
            int numberOfPermuitations,
            Divisions.DivisionModel division,
            PrizeLevels.PrizeLevels prizeLevels)
        {
            List<int[]> divisionIncompleteWinpermutations = new List<int[]>();
            int[] permuitationArray = getBaseCombinaiton(totalNumberOfPicks, getNeededPicksForDivision(division, prizeLevels).ToArray()).ToArray();
            int[] firstPermuitation = new int[totalNumberOfPicks];
            permuitationArray.CopyTo(firstPermuitation, 0);
            bool ableToFindNextdivision = true;
            int[] nonWinningPicks = getExtraPicks(permuitationArray, prizeLevels);
            for (int i = 0; i < numberOfPermuitations + extraPermutationBuffer && ableToFindNextdivision; i++)
            {
                int[] newPermuitation = new int[totalNumberOfPicks];
                permuitationArray.CopyTo(newPermuitation, 0);
                if (ableToFindNextdivision)
                {
                    divisionIncompleteWinpermutations.Add(newPermuitation);
                }
                permuitationArray = findNextPermutation(permuitationArray);
                ableToFindNextdivision = !(permuitationArray[0] == -1);
            }

            List<int[]> maximumPermutations = fillBlankDivisionpermutationsWithNonWinningData(
                divisionIncompleteWinpermutations,
                nonWinningPicks,
                division,
                prizeLevels,
                (numberOfPermuitations + extraPermutationBuffer)).OrderBy(a => Guid.NewGuid()).ToList();
            List<int[]> finalPermutations = maximumPermutations.Take(numberOfPermuitations).ToList();
            return finalPermutations;
        }

        private List<int[]> fillBlankDivisionpermutationsWithNonWinningData(
            List<int[]> nonWinningPermutations,
            int[] extraPicks,
            Divisions.DivisionModel div,
            PrizeLevels.PrizeLevels prizeLevels,
            int maxNumberOfpermutations)
        {
            List<int[]> filledpermutations = new List<int[]>();
            filledpermutations = createExtrapermutations(nonWinningPermutations, maxNumberOfpermutations, prizeLevels);
            return filledpermutations;
        }

        /// <summary>
        /// Creates extra permutations for each permutation given
        /// </summary>
        /// <param name="permutations"> A list of base permutations to genreate new permutations from</param>
        /// <param name="desiredAmountOfPermutations"> The amount of desired permutations</param>
        /// <param name="prizeLevels"> All the prize levels for a given game</param>
        /// <returns>Returns a list of all extra permutations from the base list.</returns>
        private List<int[]> createExtrapermutations(
            List<int[]> permutations,
            int desiredAmountOfPermutations,
            PrizeLevels.PrizeLevels prizeLevels)
        {
            HashSet<string> permiutationList = new HashSet<string>();
            for (int i = 0; i < permutations.Count; i++)
            {
                HashSet<string> extrasForPerm = createExtrapermutationsFormBase(permutations[i], getExtraPicks(permutations[i], prizeLevels), (int)desiredAmountOfPermutations);
                permiutationList.UnionWith(extrasForPerm);
            }
            List<int[]> permutationions = new List<int[]>();
            foreach (string s in permiutationList)
            {
                permutationions.Add(permutationStringToIntArray(s));
            }
            return permutationions;
        }

        /// <summary>
        /// Creates large numbers of permutations from single base permutation
        /// </summary>
        /// <param name="basePermiutation"> A base permutation containing the desired picks and all other values equaling zero ex: {0,0,0,1,1}</param>
        /// <param name="extraPicks">A int[] containing all extra picks to place in zero valued indexes of the base permiutation</param>
        /// <param name="maxNumberOfPermiutaitons">The maximum number of permutations for this array </param>
        /// <returns>Returns a hash set of strings representations of extra permutations</returns>
        private HashSet<string> createExtrapermutationsFormBase(
            int[] basePermiutation,
            int[] extraPicks,
            int maxNumberOfPermiutaitons)
        {
            HashSet<string> extrapermutations = new HashSet<string>();
            int[] copyOfExtraPicks = new int[extraPicks.Length];
            extraPicks.CopyTo(copyOfExtraPicks, 0);
            int[] copyOfBase = new int[basePermiutation.Length];
            basePermiutation.CopyTo(copyOfBase, 0);
            int numberOfFailuers = 0;
            while (extrapermutations.Count < maxNumberOfPermiutaitons && numberOfFailuers < maxNumberOfPermiutaitons + 15000)
            {
                if (!extrapermutations.Add(permutationToString(fillPermiutation(copyOfExtraPicks.ToList(), copyOfBase))))
                {
                    numberOfFailuers++;
                }
                else
                {
                    numberOfFailuers = 0;
                }
            }
            return extrapermutations;
        }

        /// <summary>
        /// Fills in the zero values of a permutation with desired extra pick values
        /// </summary>
        /// <param name="extraPicks"> List of extra picks that will be used to fill in zero values</param>
        /// <param name="permutation"> Base permutation to be filled in </param>
        /// <returns> Returns a filled in permutation with no zero values</returns>
        private int[] fillPermiutation(
            List<int> extraPicks,
            int[] permutation)
        {
            List<int> extraPicksCopy = new List<int>(extraPicks);
            int[] filledPermiutation = new int[permutation.Length];
            permutation.CopyTo(filledPermiutation, 0);
            Random rand = new Random();
            for (int i = 0; i < filledPermiutation.Length; i++)
            {
                if (filledPermiutation[i] == 0)
                {
                    int extraPickIndex = rand.Next(0, extraPicksCopy.Count);
                    filledPermiutation[i] = extraPicksCopy.ElementAt(extraPickIndex);
                    extraPicksCopy.RemoveAt(extraPickIndex);
                }
            }
            return filledPermiutation;
        }

        /// <summary>
        /// Gets all non winning picks for a given permutation
        /// </summary>
        /// <param name="permutation"> The base permutation</param>
        /// <param name="prizeLevels"> Used to find collection amount for unused pic<s/param>
        /// <returns> Returns an array of non winning picks</returns>
        private int[] getExtraPicks(
            int[] permutation,
            PrizeLevels.PrizeLevels prizeLevels)
        {
            List<int> usedIndexs = new List<int>();
            List<int> extraPicks = new List<int>();
            foreach (int i in permutation)
            {
                int j = Math.Abs(i);
                if (j - 1 != -1 && !usedIndexs.Contains((j - 1)))
                {
                    usedIndexs.Add(j - 1);
                }
            }
            int numberOfPrizeLevels = prizeLevels.getNumPrizeLevels();
            for (int i = 0; i < numberOfPrizeLevels; i++)
            {
                if (!usedIndexs.Contains(i))
                {
                    int numberOfCollections = prizeLevels.getPrizeLevel(i).numCollections - 1;
                    for (int j = 0; j < numberOfCollections; j++)
                    {
                        extraPicks.Add(i + 1);
                    }
                }
            }
            return extraPicks.ToArray();
        }

        /// <summary>
        /// Gets a base combination to generate extra permutations off of
        /// </summary>
        /// <param name="totalNumberOfPicks">The total number of picks needed for a permutation ex:4</param>
        /// <param name="picks">The array of picks that need to be in the base combinaiton ex{1,1}</param>
        /// <returns>Returns a list of ints representing a permutation of picks ex:{0,0,1,1}</returns>
        private List<int> getBaseCombinaiton(
            int totalNumberOfPicks,
            int[] picks)
        {
            List<int> neededPicksForComination = picks.ToList();
            for (int i = neededPicksForComination.Count; i < totalNumberOfPicks; i++)
            {
                neededPicksForComination.Add(0);
            }
            neededPicksForComination.Sort();
            return neededPicksForComination;
        }

        /// <summary>
        /// Gets the needed picks to win all prize levels of a given division
        /// </summary>
        /// <param name="division">The division is the division containing the winning prize levels</param>
        /// <param name="prizeLevels">All prize levels in the game used to get the index of the prize level</param>
        /// <returns>Returns a list of ints containing the needed picks to win a division</returns>
        private List<int> getNeededPicksForDivision(
            Divisions.DivisionModel division,
            PrizeLevels.PrizeLevels prizeLevels)
        {
            List<int> neededPicks = new List<int>();
            List<PrizeLevels.PrizeLevel> pls = division.getPrizeLevelsAtDivision();
            foreach (PrizeLevels.PrizeLevel pl in pls)
            {
                int numberToCollect = pl.numCollections;
                int indexinPrizeLevels = prizeLevels.getLevelOfPrize(pl) + 1;
                if (pl.isInstantWin || pl.numCollections == 0)
                {
                    numberToCollect = 1;
                    neededPicks.Add(-indexinPrizeLevels);
                }
                else
                {
                    numberToCollect = pl.numCollections;
                    for (int i = 0; i < numberToCollect; i++)
                    {
                        neededPicks.Add(indexinPrizeLevels);
                    }
                }
            }
            return neededPicks;
        }

        /// <summary>
        /// Finds the next permutation of a given array
        /// </summary>
        /// <param name="basePermutation">The base permutation that the next permutation will be based off of </param>
        /// <returns> Returns a new permutation based off the next permutation </returns>
        private int[] findNextPermutation(int[] basePermutation)
        {

            int i = basePermutation.Length - 1;
            while (i > 0 && basePermutation[i - 1] >= basePermutation[i])
            {
                i--;
            }

            if (i <= 0)
            {
                int[] fail = { -1 };
                return fail;
            }

            int j = basePermutation.Length - 1;
            while (basePermutation[j] <= basePermutation[i - 1])
            {
                j--;
            }

            int temp = basePermutation[i - 1];
            basePermutation[i - 1] = basePermutation[j];
            basePermutation[j] = temp;

            j = basePermutation.Length - 1;
            while (i < j)
            {
                temp = basePermutation[i];
                basePermutation[i] = basePermutation[j];
                basePermutation[j] = temp;
                i++;
                j--;
            }

            return basePermutation;
        }

        /// <summary>
        /// Converts a permutation to a string representation of itself
        /// </summary>
        /// <param name="permutation">A permutation repestented by a int[] ex:{1,2,3,4}</param>
        /// <returns>Returns a string representation of an int[] ex:"1,2,3,4"</returns>
        private string permutationToString(
            int[] permutation)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < permutation.Length; i++)
            {
                sb.Append(permutation[i]);
                if (i != permutation.Length - 1)
                {
                    sb.Append(",");
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Converts a string representation of a permutation to its int[] equivalent 
        /// </summary>
        /// <param name="permutationString">The string representation of a permutation ex:"1,2,3,4"</param>
        /// <returns>Returns a int[] from the string representation ex:{1,2,3,4}</returns>
        private int[] permutationStringToIntArray(
            string permutationString)
        {
            string[] split = permutationString.Split(',');
            int[] permutation = new int[split.Length];
            for (int i = 0; i < split.Length; i++)
            {
                permutation[i] = int.Parse(split[i]);
            }
            return permutation;
        }

        private char charFromInt(int value)
        {
            char character = (char)(value + 64);
            return character;
        }

        public void shout(object pass)
        {
            foreach (Listener list in audiance)
            {
                list.onListen(pass);
            }
        }

        public void addListener(Listener list)
        {
            audiance.Add(list);
        }
    }
}
