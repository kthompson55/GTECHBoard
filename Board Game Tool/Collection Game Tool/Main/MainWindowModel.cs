using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Collection_Game_Tool.GameSetup;
using Collection_Game_Tool.PrizeLevels;
using Collection_Game_Tool.Divisions;


namespace Collection_Game_Tool.Main
{
    public class MainWindowModel
    {
        public static GameSetupModel gameSetupModel;
        public static PrizeLevels.PrizeLevels prizeLevelsModel;
        public static DivisionsModel divisionsModel;

        private static string mainWindowErrorID;

        public static void verifyNumTiles()
        {
            int needed = PrizeLevels.PrizeLevels.totalCollections + MainWindowModel.gameSetupModel.numMoveBackwardTiles + MainWindowModel.gameSetupModel.numMoveForwardTiles;
            int actual = MainWindowModel.gameSetupModel.boardSize;
            if (needed > actual)
            {
                mainWindowErrorID = ErrorService.Instance.reportError("013", new List<String> { }, mainWindowErrorID);
            }
            else
            {
                ErrorService.Instance.resolveError("013", mainWindowErrorID);
            }
            int maxDiceMovement = (MainWindowModel.gameSetupModel.diceSelected ? MainWindowModel.gameSetupModel.numDice * 6 : MainWindowModel.gameSetupModel.spinnerMaxValue) * MainWindowModel.gameSetupModel.numTurns;
            if (maxDiceMovement > actual)
            {
                mainWindowErrorID = ErrorService.Instance.reportWarning("009", new List<string> { }, mainWindowErrorID);
            }
            else
            {
                ErrorService.Instance.resolveWarning("009", mainWindowErrorID);
            }
        }

        public static void verifyDivisions()
        {
            bool verifiedTurnCount = true;
            // verify that player has enough turns to get the largest division payout
            for (int i = 0; i < MainWindowModel.divisionsModel.getNumberOfDivisions(); i++)
            {
                int divisionMinimumTurns = 0;
                Divisions.DivisionModel currentDivision = MainWindowModel.divisionsModel.getDivision(i);
                foreach (PrizeLevels.PrizeLevel currentPrizeLevel in currentDivision.selectedPrizes)
                {
                    divisionMinimumTurns += currentPrizeLevel.numCollections;
                }
                if(MainWindowModel.gameSetupModel.numTurns < divisionMinimumTurns)
                {
                    // number of turns needed to obtain current prize level is not enough
                    mainWindowErrorID = ErrorService.Instance.reportError("010", new List<string> { currentDivision.DivisionNumber.ToString() }, mainWindowErrorID);
                    verifiedTurnCount = false;
                    break;
                }
            }

            if (verifiedTurnCount)
            {
                ErrorService.Instance.resolveError("010", mainWindowErrorID);
            }
        }
    }
}
