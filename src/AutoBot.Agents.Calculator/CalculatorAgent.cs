using AutoBot.Core.Chat;
using AutoBot.Core.Engine;
using NCalc;

namespace AutoBot.Agents.Calculator
{

    public sealed class CalculatorAgent : IAutoBotAgent
    {

        #region IAutoBotAgent Interface

        public void ProcessMessage(IChatMessage message, IChatResponse response)
        {
            var expression = new Expression(message.CommandText);
            response.Write(expression.Evaluate().ToString());
        }

        #endregion

    }

}
