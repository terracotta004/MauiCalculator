using System;
using Microsoft.Maui.Controls;

namespace MauiCalculator
{
    public partial class MainPage : ContentPage
    {
        private string _currentInput = "";
        private string _operator = "";
        private double _firstNumber = 0;

        public MainPage()
        {
            InitializeComponent();
        }

        private void OnDigitClicked(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                _currentInput += button.Text;
                ResultEntry.Text = _currentInput;
            }
        }

        private void OnOperatorClicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_currentInput) && sender is Button button)
            {
                _firstNumber = double.Parse(_currentInput);
                _operator = button.Text;
                _currentInput = "";
                ResultEntry.Text = "";
            }
        }

        private void OnCalculateClicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_currentInput))
            {
                double secondNumber = double.Parse(_currentInput);
                double result = 0;

                switch (_operator)
                {
                    case "+": result = _firstNumber + secondNumber; break;
                    case "-": result = _firstNumber - secondNumber; break;
                    case "*": result = _firstNumber * secondNumber; break;
                    case "/": result = secondNumber != 0 ? _firstNumber / secondNumber : 0; break;
                }

                ResultEntry.Text = result.ToString();
                _currentInput = result.ToString();
                _operator = "";
            }
        }

        private void OnClearClicked(object sender, EventArgs e)
        {
            _currentInput = "";
            _operator = "";
            _firstNumber = 0;
            ResultEntry.Text = "";
        }
    }
}


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
