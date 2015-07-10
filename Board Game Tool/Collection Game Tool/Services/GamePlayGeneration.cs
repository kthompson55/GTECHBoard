using Collection_Game_Tool.Divisions;
using Collection_Game_Tool.PrizeLevels;
using Collection_Game_Tool.Services.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Collection_Game_Tool.Services
{
    public class GamePlayGeneration
    {
        private List<ITile> boards;
        private Dictionary<String, List<String>> paths;
        int numMoves;
        int numDice;
        Dictionary<int, List<String>> rollOptions;
        List<Divisions.DivisionModel> divisions;
        List<PrizeLevels.PrizeLevel> prizeLevels;

        //This constructs the Game Play Generater that generates the possible games a board can have. It needs the different boards to generate the games.
        public GamePlayGeneration(List<ITile> boards)
        {
            this.boards = boards;
            paths = new Dictionary<String, List<String>>();
        }

        //This will add the full path a game play generation will take on a board and also add what the game play generation won
        private void addPath(String winFor, String boardDesign, String path)
        {
            string gamePermutation = boardDesign + path;
            if (paths.ContainsKey(winFor))
            {
                paths[winFor].Add(gamePermutation);
            }
            else
            {
                paths.Add(winFor, new List<String>());
                paths[winFor].Add(gamePermutation);
            }
        }

        Random rand = new Random();
        PrizeLevelConverter plc = new PrizeLevelConverter();
        //This generates the path a player can take through a board, this will generate every possible path that can exist and will allow for good play through.
        private void GeneratePlaysFromBoard(ITile board)
        {
            string boardDesign = CreateBoardDesignString(board);
            int count = board.connections.Keys.Count;
            foreach (int t in board.connections.Keys)
            {
                if (numDice == 0)
                    GeneratePlaysFromBoardHelper(board.connections[t], 1, boardDesign, "|" + t, new PlayGen());
                else
                    GeneratePlaysFromBoardHelper(board.connections[t], 1, boardDesign, "|" + rollOptions[t][rand.Next(0, rollOptions[t].Count)], new PlayGen());
            }
        }

        //This continues generating the play from a board, this is a recursive function.
        private void GeneratePlaysFromBoardHelper(ITile board, int moves, String boardDesign, String curPath, PlayGen pg)
        {
            if (moves == numMoves) // last space
            {
                bool hasPrizes = true;
                DivisionModel divWon = null;
                foreach (DivisionModel d in divisions)
                {
                    foreach (PrizeLevel pl in d.selectedPrizes)
                    {
                        if (hasPrizes && pl.numCollections != pg.hasCollection((String)plc.Convert(pl.prizeLevel)))
                        {
                            hasPrizes = false;
                        }
                    }
                    if (hasPrizes)
                        divWon = d;
                }

                bool hasOtherPrizes = false;
                foreach (PrizeLevel pl in prizeLevels)
                {
                    if (!hasOtherPrizes && divWon != null && !divWon.selectedPrizes.Contains(pl) && pl.numCollections == pg.hasCollection((String)plc.Convert(pl.prizeLevel)))
                    {
                        hasOtherPrizes = true;
                    }
                    else if (!hasOtherPrizes && divWon == null && pl.numCollections == pg.hasCollection((String)plc.Convert(pl.prizeLevel)))
                    {
                        hasOtherPrizes = true;
                    }
                }

                if (hasPrizes && !hasOtherPrizes)
                    addPath(divWon.DivisionNumber.ToString(), boardDesign, curPath);            ////EXIT
                else if (!hasOtherPrizes)
                    addPath("none", boardDesign, curPath);                                      ////EXIT
            }


            foreach (int t in board.connections.Keys)
            {
                if (numDice == 0)
                {
                    if (board.connections[t].type == TileTypes.collection)
                    {
                        String getter = board.connections[t].tileAction().tileInformation;
                        GeneratePlaysFromBoardHelper(board.connections[t], moves + 1, boardDesign, curPath + "," + t, new PlayGen(pg, getter));
                    }
                    else
                    {
                        GeneratePlaysFromBoardHelper(board.connections[t], moves + 1, boardDesign, curPath + "," + t, new PlayGen(pg));
                    }
                }
                else
                {
                    if (board.connections[t].type == TileTypes.collection)
                    {
                        String getter = board.connections[t].tileAction().tileInformation;
                        GeneratePlaysFromBoardHelper(board.connections[t], moves + 1, boardDesign, curPath + "," + rollOptions[t][rand.Next(0, rollOptions[t].Count)], new PlayGen(pg, getter));
                    }
                    else
                    {
                        GeneratePlaysFromBoardHelper(board.connections[t], moves + 1, boardDesign, curPath + "," + rollOptions[t][rand.Next(0, rollOptions[t].Count)], new PlayGen(pg));
                    }
                }
            }
        }


        // FINISHED, but untested 
        /// <summary>
        /// Formats all of the viable gameplay permutations into a string for the file output
        /// </summary>
        /// <param name="boards">A list of all of the first tiles of all of the boards</param>
        /// <returns>The string representation of all permutations in "boardDesign|rolls" format</returns>
        public string GetFormattedGameplay(List<ITile> boards)
        {
            string output = ""; // the string which will contain the  "div boardDesign|rolls" output

            foreach (ITile board in boards) //
            {
                foreach (KeyValuePair<String, List<String>> entry in paths) // For every division entry in paths
                {
                    string division = entry.Key;
                    int count = entry.Value.Count;
                    foreach (String permutation in entry.Value) // For every permutation of the current division
                    {
                        output += (division + " " + permutation + Environment.NewLine);
                    }
                }

            }
            return output;
        }

        //Untested, but seemingly complete
        /// <summary>
        /// Creates a string which indicates the structure of the board. 
        /// Ex. CS:d,BS,CS:IW:e.etc…
        /// </summary>
        /// <param name="board">The first tile of the board</param>
        /// <returns>The board design in string format.</returns>
        private string CreateBoardDesignString(ITile board)
        {
            string boardDesign = "";
            ITile currentTile = board;
            bool boardCompleted = false;
            while (!boardCompleted)
            {
                string toAppend = "";
                if (currentTile.type == TileTypes.collection) //Collection Space
                {
                    toAppend = "CS:" + currentTile.tileInformation; //TODO: Make sure Instant Win is contained in "tileInformation"
                }
                else if (currentTile.type == TileTypes.moveForward) //Move Forward Space
                {
                    toAppend = "MF:" + currentTile.tileInformation;

                }
                else if (currentTile.type == TileTypes.moveBack) //Move Backward Space
                {
                    toAppend = "MB:" + currentTile.tileInformation;

                }
                else // Blank Space
                {
                    toAppend = "BS";
                }

                if (currentTile.child == null) // Last tile checked. Exit Loop.
                {
                    boardCompleted = true;
                    boardDesign += toAppend;
                }
                else // More tiles to check
                {
                    currentTile = currentTile.child;
                    boardDesign += toAppend + ",";
                }

            }

            return boardDesign;
        }

        //This generates all the plays from each board
        public void Generate(int moves, List<Divisions.DivisionModel> divs, List<PrizeLevels.PrizeLevel> pls, int numDice = 0)
        {
            this.numMoves = moves;
            this.numDice = numDice;
            prizeLevels = pls;
            divisions = divs;

            if (numDice > 0)
            {
                rollOptions = new Dictionary<int, List<string>>();
                generateRollOptions(1, 0, "");
            }

            foreach (ITile b in boards)
            {
                GeneratePlaysFromBoard(b);
            }
        }

        //This generates all the possible rolls a player can have and assigns them to a roll value. This way whenever we are generating and we need a specific roll we can choose one of the random rolls
        //we generated from the roll value.
        private void generateRollOptions(int diceOn, int currentRoll, String building)
        {
            if (diceOn != numDice)
            {
                for (int i = 1; i <= 6; i++)
                {
                    if (diceOn == 1)
                        generateRollOptions(diceOn + 1, currentRoll + i, building += i);
                    else
                        generateRollOptions(diceOn + 1, currentRoll + i, building += ":" + i);
                }
            }
            else
            {
                for (int i = 1; i <= 6; i++)
                {
                    if (!rollOptions.ContainsKey(currentRoll + i) || rollOptions[currentRoll + i] == null)
                    {
                        rollOptions.Add(currentRoll + i, new List<String>());

                    }
                    building += ":" + i;
                    rollOptions[currentRoll + i].Add(building);
                }
            }
        }
    }
}