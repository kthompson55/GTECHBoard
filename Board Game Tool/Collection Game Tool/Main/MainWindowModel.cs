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
		private static MainWindowModel _instance;
		public static MainWindowModel Instance { get { return _instance ?? ( _instance = new MainWindowModel() ); } set { _instance = value; } }
		public GameSetupModel GameSetupModel { get; set; }
		public PrizeLevels.PrizeLevels PrizeLevelsModel { get; set; }
		public DivisionsModel DivisionsModel { get; set; }

		private string MainWindowErrorID { get; set; }

        /// <summary>
        /// Sends error report if board size is too small for the required number of special spaces
        /// </summary>
        public void verifyNumTiles()
        {
            int needed = PrizeLevels.PrizeLevels.totalCollections + GameSetupModel.numMoveBackwardTiles + GameSetupModel.numMoveForwardTiles;
            int actual = GameSetupModel.boardSize;
            if (needed > actual)
            {
				MainWindowErrorID = ErrorService.Instance.ReportError( "013", new List<String> { }, MainWindowErrorID );
            }
            else
            {
				ErrorService.Instance.ResolveError( "013", MainWindowErrorID );
            }
            int maxDiceMovement = (GameSetupModel.diceSelected ? GameSetupModel.numDice * 6 : GameSetupModel.spinnerMaxValue) * GameSetupModel.numTurns;
            if (maxDiceMovement > actual)
            {
				MainWindowErrorID = ErrorService.Instance.ReportWarning( "009", new List<string> { }, MainWindowErrorID );
            }
            else
            {
				ErrorService.Instance.ResolveWarning( "009", MainWindowErrorID );
            }
        }

        /// <summary>
        /// Sends error report if a division is impossible to obtain
        /// </summary>
        public void verifyDivisions()
        {
            bool verifiedTurnCount = true;
            // verify that player has enough turns to get the largest division payout
            for (int i = 0; i < DivisionsModel.getNumberOfDivisions(); i++)
            {
                int divisionMinimumTurns = 0;
                Divisions.DivisionModel currentDivision = DivisionsModel.getDivision(i);
                foreach (PrizeLevels.PrizeLevel currentPrizeLevel in currentDivision.selectedPrizes)
                {
                    divisionMinimumTurns += currentPrizeLevel.numCollections;
                }
                if(GameSetupModel.numTurns < divisionMinimumTurns)
                {
                    // number of turns needed to obtain current prize level is not enough
					MainWindowErrorID = ErrorService.Instance.ReportError( "010", new List<string> { currentDivision.DivisionNumber.ToString() }, MainWindowErrorID );
                    verifiedTurnCount = false;
                    break;
                }
            }

            if (verifiedTurnCount)
            {
				ErrorService.Instance.ResolveError( "010", MainWindowErrorID );
            }
        }
    }
}
