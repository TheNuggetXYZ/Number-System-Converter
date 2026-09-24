using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

namespace NumberSystemConverter.Views;

public partial class MainView : UserControl
{
    private readonly Dictionary<string, int> _numberSystems = new();
    private readonly char[] _digits = ['0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'];
    
    public MainView()
    {
        InitializeComponent();
        
        _numberSystems.Add("Binary", 2);
        _numberSystems.Add("Ternary", 3);
        _numberSystems.Add("Quaternary", 4);
        _numberSystems.Add("Octal", 8);
        _numberSystems.Add("Decimal", 10);
        _numberSystems.Add("Duodecimal", 12);
        _numberSystems.Add("Hexadecimal", 16);

        NumberSystemComboBox1.ItemsSource = new []{"Decimal"};
        NumberSystemComboBox2.ItemsSource = _numberSystems.Keys;
        
        NumberSystemComboBox1.SelectedItem = "Decimal";
        NumberSystemComboBox2.SelectedItem = "Binary";

        NumberSystemComboBox1.IsEnabled = false;
        
        NumberSystemComboBox2.PropertyChanged += NumberSystemComboBox2OnPropertyChanged;
        InputBox.TextChanged += InputBox1OnTextChanged;
        
        InputBox.AddHandler(TextInputEvent, InputBox_TextInput, Avalonia.Interactivity.RoutingStrategies.Tunnel);
    }

    private void InputBox1OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        ConvertBase10ToBaseN();
    }

    private void NumberSystemComboBox2OnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property.Name == nameof(NumberSystemComboBox1.SelectedIndex))
            ConvertBase10ToBaseN();
    }

    private void ConvertBase10ToBaseN()
    {
        if (NumberSystemComboBox1.SelectedItem == NumberSystemComboBox2.SelectedItem || string.IsNullOrEmpty(InputBox.Text))
        {
            OutputBox.Text = InputBox.Text;
            return;
        }

        if (!int.TryParse(InputBox.Text, out int inputNumber))
            return;

        int f10 = inputNumber;
        int z = _numberSystems[(string)NumberSystemComboBox2.SelectedItem];
        
        FindPowerAndExponent(z, f10, out var exponent);

        CalculateResultWithSubtraction(exponent, z, f10, out var result);

        OutputBox.Text = result;
    }

    private void CalculateResultWithSubtraction(int exponent, int z, int f10, out string result)
    {
        result = "";

        while (exponent >= 0)
        {
            int power = (int)MathF.Pow(z, exponent);

            int count = f10 / power;
            f10 %= power;
            
            result += _digits[count];

            exponent--;
        }
    }

    private void FindPowerAndExponent(int z, int f10, out int exponent)
    {
        exponent = 0;
        int power = 0;
        int safetyCounter = 0;
        
        while (safetyCounter < 100)
        {
            int newPower = (int)MathF.Pow(z, exponent + 1);
            if (newPower > f10)
                break;
            else
            {
                power = newPower;
                exponent++;
            }
            safetyCounter++;
        }
    }

    private void InputBox_TextInput(object? sender, TextInputEventArgs e)
    {
        if (string.IsNullOrEmpty(e.Text) || !e.Text.All(char.IsDigit))
            e.Handled = true;
    }
}