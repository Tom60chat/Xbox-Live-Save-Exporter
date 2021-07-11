using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_Pass_Save_Tranfer
{
    public class Game
    {
        /*public Game(string displayName, string logo)
        {
            DisplayName = displayName;
            Logo = logo;
        }*/

        public string DisplayName { get; set; }
        public string Logo { get; set; }
        public string DataPath { get; set; }
    }
}
