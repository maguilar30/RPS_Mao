using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API.Util
{
    public class Rules
    {
        public bool[,] RPSRules = new bool[3, 3];

        public Rules()
        {
            RPSRules[(int)StrategyEnum.R, (int)StrategyEnum.R] = true;
            RPSRules[(int)StrategyEnum.R, (int)StrategyEnum.P] = false;
            RPSRules[(int)StrategyEnum.R, (int)StrategyEnum.S] = true;
            RPSRules[(int)StrategyEnum.P, (int)StrategyEnum.R] = true;
            RPSRules[(int)StrategyEnum.P, (int)StrategyEnum.P] = true;
            RPSRules[(int)StrategyEnum.P, (int)StrategyEnum.S] = false;
            RPSRules[(int)StrategyEnum.S, (int)StrategyEnum.R] = false;
            RPSRules[(int)StrategyEnum.S, (int)StrategyEnum.P] = true;
            RPSRules[(int)StrategyEnum.S, (int)StrategyEnum.S] = true;
        }

        public bool GetRuleResult(StrategyEnum compA, StrategyEnum compB)
        {
            try
            {
                return RPSRules[(int)compA, (int)compB];
            }
            catch
            {
                throw;
            }
        }
    }
}