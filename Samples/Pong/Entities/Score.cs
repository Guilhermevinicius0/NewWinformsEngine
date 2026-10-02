using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinformsEngine.Samples.Pong.Entities
{
    public struct Score
    {
        public int Player1_Score { get; set; }
        public int Player2_Score { get; set; }

        public void ResetScore() 
        {
            Player1_Score = 0;
            Player2_Score = 0;
        }
    }
}