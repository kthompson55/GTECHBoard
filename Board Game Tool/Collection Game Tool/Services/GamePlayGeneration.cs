using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Collection_Game_Tool.Services
{
    class GamePlayGeneration
    {
        private List<Board> boards;

        GamePlayGeneration(List<Board> boards)
        {
            this.boards = boards;
        }

        private void GeneratePlaysFromBoard(Board board)
        {

        }

        private void Generate()
        {
            foreach (Board b in boards)
            {
                GeneratePlaysFromBoard(b);
            }
        }
    }
}
