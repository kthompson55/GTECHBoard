using Collection_Game_Tool.Divisions;
using Collection_Game_Tool.PrizeLevels;
using Collection_Game_Tool.Services.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Collection_Game_Tool.Services
{
	/// <summary>
	/// Generates game play
	/// </summary>
    public class GamePlayGeneration
    {
		/// <summary>
		/// The list of boards.
		/// </summary>
        private List<ITile> _boards;
		/// <summary>
		/// The list of paths.
		/// </summary>
        private Dictionary<string, List<string>> _paths;
		/// <summary>
		/// The number of moves
		/// </summary>
        private int _numMoves;
		/// <summary>
		/// The number of dice
		/// </summary>
        private int _numDice;
		/// <summary>
		/// The roll options
		/// </summary>
        private Dictionary<int, List<string>> _rollOptions;
		/// <summary>
		/// The list of divisions.
		/// </summary>
        private List<Divisions.DivisionModel> _divisions;
		/// <summary>
		/// The list of prize levels
		/// </summary>
        private List<PrizeLevels.PrizeLevel> _prizeLevels;
       
		/// <summary>
		/// This constructs the Game Play Generater that generates the possible games a board can have. It needs the different boards to generate the games.
		/// </summary>
		/// <param name="boards">The list of starting tiles for each board</param>
        public GamePlayGeneration(List<ITile> boards)
        {
            this._boards = boards;
            _paths = new Dictionary<string, List<string>>();
        }

		/// <summary>
		/// This will add the full path a game play generation will take on a board and also add what the game play generation won
		/// </summary>
		/// <param name="winFor">The win for</param>
		/// <param name="boardDesign">The board design</param>
		/// <param name="path">The path</param>
        private void AddPath(string winFor, string boardDesign, string path)
        {
            string gamePermutation = boardDesign + path;
            if (_paths.ContainsKey(winFor))
            {
                _paths[winFor].Add(gamePermutation);
            }
            else
            {
                _paths.Add(winFor, new List<string>());
                _paths[winFor].Add(gamePermutation);
            }
        }

        private PrizeLevelConverter _prizeLevelConverter = new PrizeLevelConverter();
		/// <summary>
		/// This generates the path a player can take through a board, this will generate every possible path that can exist and will allow for good play through.
		/// </summary>
		/// <param name="board">The start tile of the board</param>
        private void GeneratePlaysFromBoard(ITile board)
        {
            string boardDesign = CreateBoardDesignString(board);
            int count = board.Connections.Keys.Count;
            foreach (int t in board.Connections.Keys)
            {
                if (_numDice == 0)
                    GeneratePlaysFromBoardHelper(board.Connections[t], 1, boardDesign, "|" + t, new PlayGen(), " | ");
                else
                    GeneratePlaysFromBoardHelper(board.Connections[t], 1, boardDesign, "|" + _rollOptions[t][SRandom.NextInt(0, _rollOptions[t].Count)], new PlayGen(), " | ");
            }
        }

		/// <summary>
		/// This continues generating the play from a board, this is a recursive function.
		/// </summary>
		/// <param name="board">The start tile of the board</param>
		/// <param name="moves">The moves</param>
		/// <param name="boardDesign">The board design</param>
		/// <param name="curPath">The current path</param>
		/// <param name="pg">The play gen</param>
        private void GeneratePlaysFromBoardHelper(ITile board, int moves, String boardDesign, String curPath, PlayGen pg, String tilesLandedOn)
        {

            ITile passThroughTile = board.TileAction();
            if (passThroughTile.Type == TileTypes.collection)
            {
                String getter = passThroughTile.TileInformation;
                getter = getter.Substring(getter.Length - 1);
                pg.addPl(getter);
            }

            string tileInfo = (passThroughTile.Type == TileTypes.blank) ? "BS, " : passThroughTile.TileInformation + ", ";
            tilesLandedOn += tileInfo;

            if (moves == _numMoves) // last space
            {
                List<PrizeLevel> wonPL = new List<PrizeLevel>();
                foreach (PrizeLevel pl in _prizeLevels)
                {
                    if (pl.numCollections == pg.HasCollection((String)_prizeLevelConverter.Convert(pl.prizeLevel)))
                    {
                        wonPL.Add(pl);
                    }
                    else if (pl.numCollections < pg.HasCollection((String)_prizeLevelConverter.Convert(pl.prizeLevel)))
                    {
                        AddPath("BadBadGame", boardDesign, curPath + tilesLandedOn);
                        return;
                    }
                }

                if (wonPL.Count == 0)
                {
                    AddPath("none", boardDesign, curPath + tilesLandedOn);
                    return;
                }

                int count = 0;
                foreach (DivisionModel div in _divisions)
                {
                    count = 0;
                    foreach (PrizeLevel pl in div.selectedPrizes)
                    {
                        if (wonPL.Contains(pl))
                        {
                            count++;
                        }
                        else
                        {
                            count = Int32.MinValue;
                        }
                    }

                    if (count == wonPL.Count)
                    {
                        AddPath(div.DivisionNumber.ToString(), boardDesign, curPath + tilesLandedOn);
                        return;
                    }
                }

                AddPath("BadGame", boardDesign, curPath + tilesLandedOn);
                return;
            }

            foreach (int t in passThroughTile.Connections.Keys)
            {
                if (_numDice == 0)
                {
                    GeneratePlaysFromBoardHelper(board.Connections[t].TileAction(), moves + 1, boardDesign, curPath + "," + t, new PlayGen(pg), tilesLandedOn);

                }
                else
                {
                    GeneratePlaysFromBoardHelper(passThroughTile.Connections[t], moves + 1, boardDesign, curPath + "," + _rollOptions[t][SRandom.NextInt(0, _rollOptions[t].Count)], pg, tilesLandedOn);
                }
            }
        }


        /// <summary>
        /// Formats all of the viable gameplay permutations into a string for the file output
        /// </summary>
		/// <remarks>FINISHED, but untested</remarks>
        /// <param name="boards">A list of all of the first tiles of all of the boards</param>
        /// <returns>The string representation of all permutations in "boardDesign|rolls" format</returns>
        public string GetFormattedGameplay(List<ITile> boards)
        {
            string output = ""; // the string which will contain the  "div boardDesign|rolls" output

            foreach (ITile board in boards) //
            {
                foreach (KeyValuePair<string, List<string>> entry in _paths) // For every division entry in paths
                {
                    string division = entry.Key;
                    int count = entry.Value.Count;
                    foreach (string permutation in entry.Value) // For every permutation of the current division
                    {
                        output += (division + " " + permutation + Environment.NewLine);
                    }
                }

            }
            return output;
        }

        /// <summary>
        /// Creates a string which indicates the structure of the board. 
        /// Ex. CS:d,BS,CS:IW:e.etc…
        /// </summary>
		/// <remarks>Untested, but seemingly complete</remarks>
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
                if (currentTile.Type == TileTypes.collection) //Collection Space
                {
                    toAppend = "CS:" + currentTile.TileInformation; //TODO: Make sure Instant Win is contained in "tileInformation"
                }
                else if (currentTile.Type == TileTypes.moveForward) //Move Forward Space
                {
                    toAppend = "MF:" + currentTile.TileInformation;

                }
                else if (currentTile.Type == TileTypes.moveBack) //Move Backward Space
                {
                    toAppend = "MB:" + currentTile.TileInformation;

                }
                else // Blank Space
                {
                    toAppend = "BS";
                }

                if (currentTile.Child == null) // Last tile checked. Exit Loop.
                {
                    boardCompleted = true;
                    boardDesign += toAppend;
                }
                else // More tiles to check
                {
                    currentTile = currentTile.Child;
                    boardDesign += toAppend + ",";
                }

            }

            return boardDesign;
        }

		/// <summary>
		/// This generates all the plays from each board
		/// </summary>
		/// <param name="moves">the moves</param>
		/// <param name="divisions">The divisions</param>
		/// <param name="prizeLevels">The prize levels</param>
		/// <param name="numDice">The number of dice</param>
        public void Generate(int moves, List<DivisionModel> divisions, List<PrizeLevel> prizeLevels, int numDice = 0)
        {
            this._numMoves = moves;
            this._numDice = numDice;
            _prizeLevels = prizeLevels;
            _divisions = divisions;

            if (numDice > 0)
            {
                _rollOptions = new Dictionary<int, List<string>>();
                GenerateRollOptions(1, 0, "");
            }

            foreach (ITile b in _boards)
            {
                GeneratePlaysFromBoard(b);
            }
        }

		/// <summary>
		/// This generates all the possible rolls a player can have and assigns them to a roll value.
		/// This way whenever we are generating and we need a specific roll we can choose one of the random rolls we generated from the roll value.
		/// </summary>
		/// <param name="diceOn">The dice on</param>
		/// <param name="currentRoll">The current roll</param>
		/// <param name="building">The building</param>
        private void GenerateRollOptions(int diceOn, int currentRoll, string building)
        {
            if (diceOn != _numDice)
            {
                for (int i = 1; i <= 6; i++)
                {
                    if (diceOn == 1)
                        GenerateRollOptions(diceOn + 1, currentRoll + i, building += i);
                    else
                        GenerateRollOptions(diceOn + 1, currentRoll + i, building += ":" + i);
                }
            }
            else
            {
                for (int i = 1; i <= 6; i++)
                {
                    if (!_rollOptions.ContainsKey(currentRoll + i) || _rollOptions[currentRoll + i] == null)
                    {
                        _rollOptions.Add(currentRoll + i, new List<string>());

                    }
                    //building += ":" + i;
                    building = "" + i;
                    _rollOptions[currentRoll + i].Add(building);
                }
            }
        }
    }
}