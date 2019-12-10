using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper
{
    class Field
    {
        public bool IsMine { get; set; }
        public int HowManyBombsAround { get; set; }
        public string BombsString { get; set; }
        public bool wasClicked { get; set; }
        public string rightClick { get; set; }
        public Field(bool isMarginal)
        {
            IsMine = false;
            if (isMarginal == false) HowManyBombsAround = 0;
            else HowManyBombsAround = -1;
            BombsString = "";
            wasClicked = false;
            rightClick = "";
        }
    }
}
