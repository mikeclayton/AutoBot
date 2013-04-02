using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoBot.Core.Engine;
using AutoBot.Core.Chat;
using NCalc;

namespace AutoBot.Agents.Calculator
{

    public sealed class CalculatorAgent : IAgent
    {

        #region IAgent Interface

        public void Execute(IChatMessage message, IChatResponse response)
        {
            var expression = new Expression(message.CommandText);
            response.Write(expression.Evaluate().ToString());
        }

        #endregion

    }

}
