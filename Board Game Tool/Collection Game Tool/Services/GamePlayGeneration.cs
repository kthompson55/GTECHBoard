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
    class GamePlayGeneration
    {
        private List<ITile> boards;
        private Dictionary<String, List<String>> paths;
        int numMoves;
        int numDice;
        Dictionary<int, List<String>> rollOptions;
        List<Divisions.DivisionModel> divisions;
        List<PrizeLevels.PrizeLevel> prizeLevels;

        GamePlayGeneration(List<ITile> boards)
        {
            this.boards = boards;
            paths = new Dictionary<String, List<String>>();
        }

        private void addPath(String winFor, String path)
        {
            if (paths[winFor] != null)
            {
                paths[winFor].Add(path);
            }
            else
            {
                paths.Add(winFor, new List<String>());
                paths[winFor].Add(path);
            }
        }

        Random rand = new Random();
        PrizeLevelConverter plc = new PrizeLevelConverter();
        private void GeneratePlaysFromBoard(ITile board)
        {
            foreach (int t in board.connections.Keys)
            {
                if(numDice==0)
                {
                    if (board.connections[t].type == TileTypes.collection)
                    {
                        String getter = (String)board.connections[t].tileAction();
                        GeneratePlaysFromBoardHelper(board.connections[t], 1, "|" + t, new PlayGen(getter));
                    }
                    else if (board.connections[t].type == TileTypes.moveForward || board.connections[t].type == TileTypes.moveBack)
                    {
                        ITile nextTile = (ITile)board.connections[t].tileAction();
                        while (nextTile.type == TileTypes.moveForward || nextTile.type == TileTypes.moveBack)
                        {
                            nextTile = (ITile)nextTile.tileAction();
                        }
                        GeneratePlaysFromBoardHelper(nextTile, 1, "|" + t, new PlayGen());
                    }
                    else
                    {
                        GeneratePlaysFromBoardHelper(board.connections[t], 1, "|" + t, new PlayGen());
                    }
                }
                else
                {
                    if (board.connections[t].type == TileTypes.collection)
                    {
                        String getter = (String)board.connections[t].tileAction();
                        GeneratePlaysFromBoardHelper(board.connections[t], 1, "|" + rollOptions[t][rand.Next(0, rollOptions[t].Count)], new PlayGen());
                    }
                    else if (board.connections[t].type == TileTypes.moveForward || board.connections[t].type == TileTypes.moveBack)
                    {
                        ITile nextTile = (ITile)board.connections[t].tileAction();
                        while (nextTile.type == TileTypes.moveForward || nextTile.type == TileTypes.moveBack)
                        {
                            nextTile = (ITile)nextTile.tileAction();
                        }
                        GeneratePlaysFromBoardHelper(nextTile, 1, "|" + rollOptions[t][rand.Next(0, rollOptions[t].Count)], new PlayGen());
                    }
                    else
                    {
                        GeneratePlaysFromBoardHelper(board.connections[t], 1, "|" + rollOptions[t][rand.Next(0, rollOptions[t].Count)], new PlayGen());
                    }
                }
            }
        }

        private void GeneratePlaysFromBoardHelper(ITile board, int moves, String curPath, PlayGen pg)
        {
            if (board.connections.Count == 0 || moves==numMoves)
            {
                bool hasPrizes = true;
                DivisionModel divWon = null;
                foreach (DivisionModel d in divisions)
                {
                    foreach (PrizeLevel pl in d.selectedPrizes)
                    {
                        if (hasPrizes && pl.numCollections != pg.hasCollection((String)plc.Convert(pl.prizeLevel, null, null, new System.Globalization.CultureInfo("en-us"))))
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
                    if (!hasOtherPrizes && divWon!=null && !divWon.selectedPrizes.Contains(pl) && pl.numCollections == pg.hasCollection((String)plc.Convert(pl.prizeLevel, null, null, new System.Globalization.CultureInfo("en-us"))))
                    {
                        hasOtherPrizes = true;
                    }
                    else if (!hasOtherPrizes && divWon == null && pl.numCollections == pg.hasCollection((String)plc.Convert(pl.prizeLevel, null, null, new System.Globalization.CultureInfo("en-us"))))
                    {
                        hasOtherPrizes = true;
                    }
                }

                if (hasPrizes && !hasOtherPrizes)
                    addPath(divWon.DivisionNumber.ToString(), curPath);
                else if (!hasOtherPrizes)
                    addPath("none", curPath);
            }
            foreach (int t in board.connections.Keys)
            {
                if (numDice==0)
                {
                    if (board.connections[t].type == TileTypes.collection)
                    {
                        String getter = (String)board.connections[t].tileAction();
                        GeneratePlaysFromBoardHelper(board.connections[t], moves + 1, curPath + "," + t, new PlayGen(pg, getter));
                    }
                    else if (board.connections[t].type == TileTypes.moveForward || board.connections[t].type == TileTypes.moveBack)
                    {
                        ITile nextTile = (ITile)board.connections[t].tileAction();
                        while (nextTile.type == TileTypes.moveBack || nextTile.type == TileTypes.moveForward)
                        {
                            nextTile = (ITile)nextTile.tileAction();
                        }
                        GeneratePlaysFromBoardHelper(nextTile, moves + 1, curPath + "," + t, new PlayGen(pg));
                    }
                    else
                    {
                        GeneratePlaysFromBoardHelper(board.connections[t], moves + 1, curPath + "," + t, new PlayGen(pg));
                    }
                }
                else
                {
                    if (board.connections[t].type == TileTypes.collection)
                    {
                        String getter = (String)board.connections[t].tileAction();
                        GeneratePlaysFromBoardHelper(board.connections[t], moves + 1, curPath + "," + rollOptions[t][rand.Next(0, rollOptions[t].Count)], new PlayGen(pg, getter));
                    }
                    else if (board.connections[t].type == TileTypes.moveForward || board.connections[t].type == TileTypes.moveBack)
                    {
                        ITile nextTile = (ITile)board.connections[t].tileAction();
                        while (nextTile.type == TileTypes.moveBack || nextTile.type == TileTypes.moveForward)
                        {
                            nextTile = (ITile)nextTile.tileAction();
                        }
                        GeneratePlaysFromBoardHelper(nextTile, moves + 1, curPath + "," + rollOptions[t][rand.Next(0, rollOptions[t].Count)], new PlayGen(pg));
                    }
                    else
                    {
                        GeneratePlaysFromBoardHelper(board.connections[t], moves + 1, curPath + "," + rollOptions[t][rand.Next(0, rollOptions[t].Count)], new PlayGen(pg));
                    }
                }
            }
        }

        public void Generate(int moves, List<Divisions.DivisionModel> divs, List<PrizeLevels.PrizeLevel> pls, int numDice=0)
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

        private void generateRollOptions(int diceOn, int currentRoll, String building)
        {
            if (diceOn != numDice)
            {
                for (int i = 1; i <= 6; i++)
                {
                    if(diceOn==1)
                        generateRollOptions(diceOn + 1, currentRoll + i, building += i);
                    else
                        generateRollOptions(diceOn + 1, currentRoll + i, building += ":" + i);
                }
            }
            else
            {
                for (int i = 1; i <= 6; i++)
                {
                    if (!rollOptions.ContainsKey(currentRoll+i) && rollOptions[currentRoll + i] == null)
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