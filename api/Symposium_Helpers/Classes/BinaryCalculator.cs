using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Symposium.Models.Models.Helper;
using Symposium.Models.Models.Hotel;

namespace Symposium.Helpers.Classes
{
    public class BinaryCalculator
    {
        public BinaryCalculatorHelperModel helper = new BinaryCalculatorHelperModel();

        Dictionary<string, ParameterExpression> parameters = new Dictionary<string, ParameterExpression>();

        Dictionary<string, bool> values;

        bool ovride;

        bool adult;

        public BinaryCalculator(BinaryCalculatorHelperModel helper, bool ovride)
        {
            this.helper = helper;

            this.adult = true;

            this.ovride = ovride;
        }

        public bool Execute(Dictionary<string, bool> values, Node tree)
        {
            bool adultResult = false;

            bool childResult = false;

            setParameters(values);

            this.values = values;

            adultResult = CalculateResult(tree);

            adult = false;

            childResult = CalculateResult(tree);

            return adultResult || childResult;
        }

        private void setParameters(Dictionary<string, bool> values)
        {
            foreach (string key in values.Keys.ToList())
            {
                parameters.Add(key, Expression.Parameter(typeof(bool), key));
            }
        }

        private bool CalculateResult(Node node)
        {
            bool result = false;

            foreach (Node child in node.Children)
            {
                result = CalculateResult(child);
            }

            if (result)
            {
                return true;
            }
            else
            {
                return GenerateFinalCheck(node);
            }
        }

        private bool GenerateFinalCheck(Node node)
        {
            int allowance = 0;

            Dictionary<string, int> allowances = new Dictionary<string, int>();

            Dictionary<string, int> consumptions = new Dictionary<string, int>();

            if (adult)
            {
                allowance = helper.allowanceAdult;

                foreach (BinaryCalculatorConsumptionHelperModel temp in helper.consumptions)
                {
                    consumptions.Add(temp.timezoneCode, temp.consumptionAdult);
                }
            }
            else
            {
                allowance = helper.allowanceChild;

                foreach (BinaryCalculatorConsumptionHelperModel temp in helper.consumptions)
                {
                    consumptions.Add(temp.timezoneCode, temp.consumptionChild);
                }
            }

            foreach (KeyValuePair<string, int> temp in consumptions)
            {
                if (!allowances.ContainsKey(temp.Key))
                    allowances.Add(temp.Key, allowance);
                else
                    allowances[temp.Key] = allowance;
            }

            foreach (string operant in node.Operants)
            {
                int timezoneConsumption = 0;

                if (node.Operator == '.')
                {
                    foreach (string operant2 in node.Operants)
                    {
                        timezoneConsumption += consumptions[operant2];
                    }
                }
                else
                {
                    timezoneConsumption = consumptions[operant];
                }

                if (ovride)
                {
                    if (operant == helper.timezoneCode)
                    {
                        if (adult)
                        {
                            foreach (KeyValuePair<string, int> temp in allowances)
                            {
                                if (operant == temp.Key)
                                {
                                    helper.remainingAllowanceAdult = temp.Value - timezoneConsumption;
                                }
                            }
                        }
                        else
                        {
                            foreach (KeyValuePair<string, int> temp in allowances)
                            {
                                if (operant == temp.Key)
                                {
                                    helper.remainingAllowanceChild = temp.Value - timezoneConsumption;
                                }
                            }
                        }

                        return true;
                    }
                }
                else
                {
                    if (operant == helper.timezoneCode && timezoneConsumption < allowance)
                    {
                        if (adult)
                        {
                            foreach (KeyValuePair<string, int> temp in allowances)
                            {
                                if (operant == temp.Key)
                                {
                                    helper.remainingAllowanceAdult = temp.Value - timezoneConsumption;
                                }
                            }
                        }
                        else
                        {
                            foreach (KeyValuePair<string, int> temp in allowances)
                            {
                                if (operant == temp.Key)
                                {
                                    helper.remainingAllowanceChild = temp.Value - timezoneConsumption;
                                }
                            }
                        }

                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// calculate allawance for all timezones based on timezone expression
        /// </summary>
        /// <param name="node">Node</param>
        /// <returns>bool</returns>
        private bool GenerateFinalEvaluationCheck(Node node)
        {
            int allowance = 0;

            Dictionary<string, int> allowances = new Dictionary<string, int>();

            if (adult)
            {
                allowance = helper.allowanceAdult;
            }
            else
            {
                allowance = helper.allowanceChild;

                foreach (BinaryCalculatorConsumptionHelperModel temp in helper.consumptions)
                {
                }
            }

            foreach (string operant in node.Operants)
            {
                int timezoneConsumption = 0;

                if (node.Operator == '.')
                {
                    foreach (string operant2 in node.Operants)
                    {
                        //foreach (KeyValuePair<string, int> temp in consumptions)
                        //{
                        //    if (operant2 == temp.Key)
                        //    {
                        //        allowances.Add(temp.Key, allowance);

                        //        timezoneConsumption = temp.Value;
                        //    }
                        //}
                    }
                }
                else
                {
                    //foreach (KeyValuePair<string, int> temp in consumptions)
                    //{
                    //    if (operant == temp.Key)
                    //    {
                    //        allowances.Add(temp.Key, allowance);

                    //        timezoneConsumption = temp.Value;
                    //    }
                    //}
                }

                if (operant == helper.timezoneCode) // && timezoneConsumption < allowance)
                {
                    if (adult)
                    {
                        foreach (KeyValuePair<string, int> temp in allowances)
                        {
                            if (operant == temp.Key)
                            {
                                helper.remainingAllowanceAdult = temp.Value - timezoneConsumption;
                            }
                        }
                    }
                    else
                    {
                        foreach (KeyValuePair<string, int> temp in allowances)
                        {
                            if (operant == temp.Key)
                            {
                                helper.remainingAllowanceChild = temp.Value - timezoneConsumption;
                            }
                        }
                    }

                    return true;
                }
            }

            return false;
        }
    }

    /// <summary>
    /// Convert a string representing a boolean expression and construct a Tree structure.
    /// The input string could be: "A+((B.C)+(D.E))", where '+' = AND, '.' = OR.
    /// </summary>
    public class TreeManager
    {
        //3+6=9   operand '3',  operator '+'. 

        /// <summary>
        /// The tree structure containing binary expressions
        /// </summary>
        public Node Tree = new Node();

        /// <summary>
        /// The deepest level of nodes. The top Node has Level=0
        /// </summary>
        public int DeepestLevel { get; set; } = 0;


        List<char> operators = new List<char>() { '+', '.' }; // <---- every operator should be one character!!!!
        string expression;
        int i = 0;

        /// <summary>
        /// Construct tree with binary expressions
        /// </summary>
        /// <param name="expression"></param>
        public void Construct(string expression)
        {
            checkParantheses(expression);
            this.expression = expression;
            DoConstruct(Tree);
        }

        private void DoConstruct(Node curNode)
        {
            StringBuilder operantStack = new StringBuilder();

            while (i < expression.Length)
            {

                var curChar = (char)expression[i];

                if (isOperant(curChar))
                {
                    operantStack.Append(curChar);
                }
                else if (isOperator(curChar))
                {
                    if (curNode.Operator != '\0' && curNode.Operator != curChar)
                    {
                        throw new Exception($"Error setting new Operator '{curChar}'. Operator '{curNode.Operator}' already exists for the current Node . Did you miss a ')' ?. {Environment.NewLine}  Expression : '{expression}', Position: {i + 1} ");
                    }
                    curNode.Operator = curChar;
                    setOperant(operantStack, curNode);
                }
                else if (isOpenParathensis(curChar))
                {
                    Node childNode = new Node();
                    childNode.Parent = curNode;
                    childNode.Level = curNode.Level + 1;
                    setDeepestLevel(childNode.Level);
                    curNode.Children.Add(childNode);
                    i++;
                    DoConstruct(childNode);
                }
                else if (isCloseParathensis(curChar))
                {
                    setOperant(operantStack, curNode);
                    return;
                }
                i++;
            }

            setOperant(operantStack, curNode);
        }

        private void checkParantheses(string expression)
        {
            int o = expression.Count(x => x == '(');
            int c = expression.Count(x => x == ')');
            if (c > o) throw new Exception($" Missing '(' into Expression : '{expression}'");
            if (o > c) throw new Exception($" Missing ')' into Expression : '{expression}'");
        }

        private void setDeepestLevel(int level)
        {
            if (level > DeepestLevel) DeepestLevel = level;
        }

        private void setOperant(StringBuilder operantStack, Node curNode)
        {
            if (operantStack.Length > 0)
            {
                curNode.Operants.Add(operantStack.ToString());
                operantStack.Clear();
            }
        }


        /// <summary>
        /// is '+' or '.'
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private bool isOperator(char c)
        {
            return operators.Contains(c);
        }

        private bool isOpenParathensis(char c)
        {
            return (c == '(');
        }

        private bool isCloseParathensis(char c)
        {
            return (c == ')');
        }

        /// <summary>
        /// character is part of operant (variable)
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private bool isOperant(char c)
        {
            if (isOperator(c)) return false;
            if (isOpenParathensis(c) || isCloseParathensis(c)) return false;
            return true;
        }
    }
}
