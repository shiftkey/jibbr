﻿using System;
using System.Collections.Generic;
using Jabbot.CommandSprockets;
using NCalc;

namespace Jabbot.Extensions
{
    public class CalculatorSprocket : CommandSprocket
    {
        private readonly string _helpInfo;

        public CalculatorSprocket()
        {
            _helpInfo = "Hi {0}," + Environment.NewLine + Environment.NewLine
                        + "I accept the following commands:" + Environment.NewLine
                        + "info/help:\t" + "this ChatMessage" + Environment.NewLine
                        + "expr/calc:\t" + "accepts a math expression to evaluate and returns the result" + Environment.NewLine + Environment.NewLine
                        + "for expression documentation please refer to ncalcs website:" + Environment.NewLine
                        + "operators: http://ncalc.codeplex.com/wikipage?title=operators&referringTitle=Home" + Environment.NewLine
                        + "values: http://ncalc.codeplex.com/wikipage?title=values&referringTitle=Home" + Environment.NewLine
                        + "functions: http://ncalc.codeplex.com/wikipage?title=functions&referringTitle=Home";
                        
        }

        public override IEnumerable<string> SupportedInitiators
        {
            get
            {
                yield return "calc";
                yield return "mathbot";
            }
        }

        public override IEnumerable<string> SupportedCommands
        {
            get
            {
                yield return "info";
                yield return "help";
                yield return "expr";
                yield return "calc";
            }
        }

        public override bool ExecuteCommand()
        {
            switch (Command)
            {
                case "info":
                case "help":
                    return ShowInfo();
                case "expr":
                case "calc":
                    return CalculateExpression(GetExpression());
                default:
                    return false;
            }
        }

        private bool CalculateExpression(string mathExpression)
        {
            var expression = new Expression(mathExpression, EvaluateOptions.IgnoreCase);
            object result = null;
            try
            {
                result = expression.Evaluate();
            }
            catch (Exception ex)
            {
                if (ex is ArgumentException || ex is EvaluationException)
                    // TODO: where is receiver coming from?
                    Bot.Say(string.Format("Sorry {0} - i couldn't evaluate your expression", ChatMessage.User.Name), ""); // ChatMessage.Receiver);
            }

            if (result == null)
                return false;

            Bot.Say(string.Format("{0} = {1}", mathExpression, result), ChatMessage.User.Name);

            return true;
        }

        private string GetExpression()
        {
            var expression = ChatMessage.Content.Substring(ChatMessage.Content.LastIndexOf(Command, StringComparison.Ordinal) + Command.Length).Trim();

            return expression;
        }

        private bool ShowInfo()
        {
            Bot.PrivateReply(ChatMessage.User.Name, string.Format(_helpInfo, ChatMessage.User.Name));

            return true;

        }
    }
}
