using System;
using System.Collections.Generic;
using Microsoft.Maui.Controls;

namespace MauiCalculator
{
    public partial class MainPage : ContentPage
    {
        private string _currentInput = "";
        private List<string> _expression = new List<string>(); // Stores numbers & operators
        private bool _isNewEntry = false;
        private bool _isDegrees = false; // Toggle for degrees/radians mode

        public MainPage()
        {
            InitializeComponent();
        }

        private void OnPiClicked(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                if (_isNewEntry)
                {
                    _currentInput = Math.PI.ToString();
                    _isNewEntry = false;
                }

                _currentInput += Math.PI.ToString();
                ResultEntry.Text = _currentInput;
            }
        }

        private void OnDigitClicked(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                if (_isNewEntry)
                {
                    _currentInput = "";
                    _isNewEntry = false;
                }

                _currentInput += button.Text;
                ResultEntry.Text = _currentInput;
            }
        }

        private void OnOperatorClicked(object sender, EventArgs e)
        {
            // If user just calculated a result and presses an operator, use that result.
            if (string.IsNullOrEmpty(_currentInput) && _expression.Count > 0)
            {
                _currentInput = _expression.Last();
            }

            if (!string.IsNullOrEmpty(_currentInput))
            {
                _expression.Add(_currentInput); // Store previous number
                _currentInput = "";

                if (sender is Button button)
                {
                    _expression.Add(button.Text); // Store operator
                }

                _isNewEntry = true; // Prepare for next input
            }
        }

        private void OnFunctionClicked(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                string function = button.Text.ToLower();

                if (function == "sin")
                {
                    if (string.IsNullOrEmpty(_currentInput) && _expression.Count > 0)
                    {
                        // If _currentInput is empty, use the last evaluated result
                        _currentInput = _expression.Last();
                    }

                    if (!string.IsNullOrEmpty(_currentInput))
                    {
                        double value = double.Parse(_currentInput);
                        if (_isDegrees)
                        {
                            value = value * Math.PI / 180; // Convert to radians if needed
                        }

                        double result = Math.Sin(value);
                        ResultEntry.Text = result.ToString();

                        _expression.Clear();
                        _expression.Add(result.ToString());
                        _currentInput = result.ToString();  // Store result for further operations
                        _isNewEntry = true;
                    }
                }

                if (function == "cos")
                {
                    if (string.IsNullOrEmpty(_currentInput) && _expression.Count > 0)
                    {
                        // If _currentInput is empty, use the last evaluated result
                        _currentInput = _expression.Last();
                    }

                    if (!string.IsNullOrEmpty(_currentInput))
                    {
                        double value = double.Parse(_currentInput);
                        if (_isDegrees)
                        {
                            value = value * Math.PI / 180; // Convert to radians if needed
                        }

                        double result = Math.Cos(value);
                        ResultEntry.Text = result.ToString();

                        _expression.Clear();
                        _expression.Add(result.ToString());
                        _currentInput = result.ToString();  // Store result for further operations
                        _isNewEntry = true;
                    }
                }

                if (function == "tan")
                {
                    if (string.IsNullOrEmpty(_currentInput) && _expression.Count > 0)
                    {
                        // If _currentInput is empty, use the last evaluated result
                        _currentInput = _expression.Last();
                    }

                    if (!string.IsNullOrEmpty(_currentInput))
                    {
                        double value = double.Parse(_currentInput);
                        if (_isDegrees)
                        {
                            value = value * Math.PI / 180; // Convert to radians if needed
                        }

                        double result = Math.Tan(value);
                        ResultEntry.Text = result.ToString();

                        _expression.Clear();
                        _expression.Add(result.ToString());
                        _currentInput = result.ToString();  // Store result for further operations
                        _isNewEntry = true;
                    }
                }
            }
        }


        private void OnCalculateClicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_currentInput))
            {
                _expression.Add(_currentInput);
                _currentInput = "";

                double result = EvaluateExpression(_expression);
                ResultEntry.Text = result.ToString();

                _expression.Clear();
                _expression.Add(result.ToString()); // Store result for further calculations
                _isNewEntry = true;
            }
        }

        private double EvaluateExpression(List<string> expression)
        {
            List<string> postfix = ConvertToPostfix(expression);
            return EvaluatePostfix(postfix);
        }

        private List<string> ConvertToPostfix(List<string> infix)
        {
            Dictionary<string, int> precedence = new Dictionary<string, int>
            {
                { "+", 1 }, { "-", 1 }, { "*", 2 }, { "/", 2 }, { "^", 3 }
            };

            List<string> output = new List<string>();
            Stack<string> operators = new Stack<string>();

            foreach (string token in infix)
            {
                if (double.TryParse(token, out _))
                {
                    output.Add(token); // Numbers go directly to output
                }
                else if (token == "sin")
                {
                    output.Add(token); // Functions go directly to output
                }
                else
                {
                    while (operators.Count > 0 &&
                           precedence.ContainsKey(operators.Peek()) &&
                           precedence[operators.Peek()] >= precedence[token])
                    {
                        output.Add(operators.Pop()); // Pop higher-precedence operators
                    }
                    operators.Push(token);
                }
            }

            while (operators.Count > 0)
            {
                output.Add(operators.Pop());
            }

            return output;
        }

        private double EvaluatePostfix(List<string> postfix)
        {
            Stack<double> stack = new Stack<double>();

            foreach (string token in postfix)
            {
                if (double.TryParse(token, out double number))
                {
                    stack.Push(number);
                }
                else if (token == "sin")
                {
                    double value = stack.Pop();
                    if (_isDegrees)
                    {
                        value = value * Math.PI / 180;
                    }
                    stack.Push(Math.Sin(value));
                }
                else if (token == "cos")
                {
                    double value = stack.Pop();
                    if (_isDegrees)
                    {
                        value = value * Math.PI / 180;
                    }
                    stack.Push(Math.Cos(value));
                }
                else if (token == "sin")
                {
                    double value = stack.Pop();
                    if (_isDegrees)
                    {
                        value = value * Math.PI / 180;
                    }
                    stack.Push(Math.Tan(value));
                }
                // Factorial ???
                else
                {
                    double b = stack.Pop();
                    double a = stack.Pop();
                    stack.Push(token switch
                    {
                        "+" => a + b,
                        "-" => a - b,
                        "*" => a * b,
                        "/" => b != 0 ? a / b : 0,
                        "^" => Math.Pow(a, b),
                        _ => 0
                    });
                }
            }

            return stack.Pop();
        }

        private void OnClearClicked(object sender, EventArgs e)
        {
            _currentInput = "";
            _expression.Clear();
            ResultEntry.Text = "";
            _isNewEntry = false;
        }

        private void OnToggleDegreesClicked(object sender, EventArgs e)
        {
            _isDegrees = !_isDegrees;
            DegreeToggleButton.Text = _isDegrees ? "Deg" : "Rad";
        }
    }
}



//using System;
//using Microsoft.Maui.Controls;

//namespace MauiCalculator
//{
//    public partial class MainPage : ContentPage
//    {
//        private string _currentInput = "";
//        private string _operator = "";
//        private double _firstNumber = 0;
//        private bool _isNewEntry = false;

//        public MainPage()
//        {
//            InitializeComponent();
//        }

//        private void OnDigitClicked(object sender, EventArgs e)
//        {
//            if (sender is Button button)
//            {
//                if (_isNewEntry)
//                {
//                    _currentInput = "";
//                    _isNewEntry = false;
//                }

//                _currentInput += button.Text;
//                ResultEntry.Text = _currentInput;
//            }
//        }

//        private void OnOperatorClicked(object sender, EventArgs e)
//        {
//            if (!string.IsNullOrEmpty(_currentInput))
//            {
//                double secondNumber = double.Parse(_currentInput);

//                // If there's already an operator, calculate before assigning the new operator
//                if (!string.IsNullOrEmpty(_operator))
//                {
//                    _firstNumber = PerformCalculation(_firstNumber, secondNumber, _operator);
//                    ResultEntry.Text = _firstNumber.ToString();
//                }
//                else
//                {
//                    _firstNumber = secondNumber;
//                }

//                if (sender is Button button)
//                {
//                    _operator = button.Text;
//                }

//                _isNewEntry = true; // Prepare for next number input
//            }
//        }

//        private void OnCalculateClicked(object sender, EventArgs e)
//        {
//            if (!string.IsNullOrEmpty(_currentInput) && !string.IsNullOrEmpty(_operator))
//            {
//                double secondNumber = double.Parse(_currentInput);
//                _firstNumber = PerformCalculation(_firstNumber, secondNumber, _operator);

//                ResultEntry.Text = _firstNumber.ToString();
//                _currentInput = _firstNumber.ToString();
//                _operator = "";
//                _isNewEntry = true;
//            }
//        }

//        private double PerformCalculation(double first, double second, string op)
//        {
//            return op switch
//            {
//                "+" => first + second,
//                "-" => first - second,
//                "*" => first * second,
//                "/" => second != 0 ? first / second : 0,
//                "^" => Math.Pow(first, second),
//                _ => second
//            };
//        }

//        private void OnClearClicked(object sender, EventArgs e)
//        {
//            _currentInput = "";
//            _operator = "";
//            _firstNumber = 0;
//            _isNewEntry = false;
//            ResultEntry.Text = "";
//        }
//    }
//}


//using System;
//using Microsoft.Maui.Controls;

//namespace MauiCalculator
//{
//    public partial class MainPage : ContentPage
//    {
//        private string _currentInput = "";
//        private string _operator = "";
//        private double _firstNumber = 0;
//        private bool _isNewEntry = false;

//        public MainPage()
//        {
//            InitializeComponent();
//        }

//        private void OnDigitClicked(object sender, EventArgs e)
//        {
//            if (sender is Button button)
//            {
//                if (_isNewEntry)
//                {
//                    _currentInput = "";
//                    _isNewEntry = false;
//                }

//                _currentInput += button.Text;
//                ResultEntry.Text = _currentInput;
//            }
//        }

//        private void OnOperatorClicked(object sender, EventArgs e)
//        {
//            if (!string.IsNullOrEmpty(_currentInput) && sender is Button button)
//            {
//                _firstNumber = double.Parse(_currentInput);
//                _operator = button.Text;
//                _isNewEntry = true; // Allow new input but keep displaying the first number
//            }
//        }

//        private void OnCalculateClicked(object sender, EventArgs e)
//        {
//            if (!string.IsNullOrEmpty(_currentInput) && !string.IsNullOrEmpty(_operator))
//            {
//                double secondNumber = double.Parse(_currentInput);
//                double result = 0;

//                switch (_operator)
//                {
//                    case "+": result = _firstNumber + secondNumber; break;
//                    case "-": result = _firstNumber - secondNumber; break;
//                    case "*": result = _firstNumber * secondNumber; break;
//                    case "/": result = secondNumber != 0 ? _firstNumber / secondNumber : 0; break;
//                    case "^": result = Math.Pow(_firstNumber, secondNumber); break; // Exponentiation
//                }

//                ResultEntry.Text = result.ToString();
//                _currentInput = result.ToString();
//                _operator = "";
//                _isNewEntry = true;
//            }
//        }

//        private void OnClearClicked(object sender, EventArgs e)
//        {
//            _currentInput = "";
//            _operator = "";
//            _firstNumber = 0;
//            _isNewEntry = false;
//            ResultEntry.Text = "";
//        }
//    }
//}


//using System;
//using Microsoft.Maui.Controls;

//namespace MauiCalculator
//{
//    public partial class MainPage : ContentPage
//    {
//        private string _currentInput = "";
//        private string _operator = "";
//        private double _firstNumber = 0;

//        public MainPage()
//        {
//            InitializeComponent();
//        }

//        private void OnDigitClicked(object sender, EventArgs e)
//        {
//            if (sender is Button button)
//            {
//                _currentInput += button.Text;
//                ResultEntry.Text = _currentInput;
//            }
//        }

//        private void OnOperatorClicked(object sender, EventArgs e)
//        {
//            if (!string.IsNullOrEmpty(_currentInput) && sender is Button button)
//            {
//                _firstNumber = double.Parse(_currentInput);
//                _operator = button.Text;
//                _currentInput = "";
//                ResultEntry.Text = "";
//            }
//        }

//        private void OnCalculateClicked(object sender, EventArgs e)
//        {
//            if (!string.IsNullOrEmpty(_currentInput))
//            {
//                double secondNumber = double.Parse(_currentInput);
//                double result = 0;

//                switch (_operator)
//                {
//                    case "+": result = _firstNumber + secondNumber; break;
//                    case "-": result = _firstNumber - secondNumber; break;
//                    case "*": result = _firstNumber * secondNumber; break;
//                    case "/": result = secondNumber != 0 ? _firstNumber / secondNumber : 0; break;
//                }

//                ResultEntry.Text = result.ToString();
//                _currentInput = result.ToString();
//                _operator = "";
//            }
//        }

//        private void OnClearClicked(object sender, EventArgs e)
//        {
//            _currentInput = "";
//            _operator = "";
//            _firstNumber = 0;
//            ResultEntry.Text = "";
//        }
//    }
//}


//namespace MauiCalculator
//{
//    public partial class MainPage : ContentPage
//    {
//        int count = 0;

//        public MainPage()
//        {
//            InitializeComponent();
//        }

//        private void OnCounterClicked(object sender, EventArgs e)
//        {
//            count++;

//            if (count == 1)
//                CounterBtn.Text = $"Clicked {count} time";
//            else
//                CounterBtn.Text = $"Clicked {count} times";

//            SemanticScreenReader.Announce(CounterBtn.Text);
//        }
//    }

//}
