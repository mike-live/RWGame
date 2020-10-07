using System;
using System.Collections.Generic;
using System.Text;
using RWGame.Models;

namespace RWGame.ViewModels
{
    public class GameHistoryDisplayData
    {
        public GameHistoryDisplayData()
        {

        }
        private GameHistoryModel gameHistoryModel { get; set; }
        string gameListViewEmptyMessageText { get { return "Here we place your finished games.\nThanks for playing =)"; } }
    }
    public class GameHistoryViewModel
    {
        
    }
}
