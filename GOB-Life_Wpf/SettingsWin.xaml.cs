using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.IO;
using System.Text;
using NCalc;
using System.Windows.Media;

namespace GOB_Life_Wpf
{
    /// <summary>
    /// Логика взаимодействия для SettingsWin.xaml
    /// </summary>
    public partial class SettingsWin : Window
    {
        private readonly Dictionary<TextBox, int> sformuls = new Dictionary<TextBox, int>();
        private readonly Dictionary<int, string> snames = new Dictionary<int, string>();
        public SettingsWin()
        {
            InitializeComponent();
            LoadSettings();
        }

        private void LoadSettings()
        {
            bool head = true;
            string[] comands = File.ReadAllLines("formuls.txt", Encoding.Default);
            for (int i = 0; i < comands.Length; i++)
            {
                if (head == true)
                {
                    
                    if (comands[i] == "#***")
                    {
                        head = false;
                        continue;
                    }
                    if (comands[i].Length > 0)
                        InfoMemo.Text += comands[i].Substring(1) + '\n';
                    else
                        InfoMemo.Text += '\n';
                }
                else
                {
                    if (comands[i].Length == 0)
                        continue;
                    if (comands[i][0] == '#')
                        continue;

                    string[] cmd = comands[i].Split(new string[] { "; " }, StringSplitOptions.None);
                    string topText = "", editText, bottomText = "";

                    if (comands[i - 1].Length > 0)
                        if (comands[i - 1][0] == '#')
                            topText = comands[i - 1].Substring(1);

                    if (comands[i + 1].Length > 0)
                        if (comands[i + 1][0] == '#')
                            bottomText = comands[i + 1].Substring(1);

                    switch (cmd[0])
                    {
                        case "func":
                            editText = $"func; {cmd[2]}, {string.Join("; ", cmd.Skip(3).ToArray())}";
                            break;
                        default:
                            editText = cmd[1].ToString();
                            break;
                    }

                    sformuls.Add(AddComponent(StackPanel, topText, editText, bottomText, cmd[0].ToString()), i);
                    snames.Add(i, cmd[0].ToString());
                }
            }
        }

        private TextBox AddComponent(StackPanel container, string topText, string inputText, string bottomText = null, string leftText = null)
        {
            // Создаем контейнер для компонента
            StackPanel componentPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Margin = new Thickness(0) // Добавляем отступы для визуального разделения
            };

            // Создаем и добавляем текст сверху
            if (!string.IsNullOrWhiteSpace(topText))
            {
                TextBlock topTextBlock = new TextBlock
                {
                    Text = topText,
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(5, 10, 5, 5)
                };
                componentPanel.Children.Add(topTextBlock);
            }

            // Создаем Grid для размещения текста и текстового поля
            Grid grid = new Grid
            {
                Margin = new Thickness(5, 0, 5, 0)
            };

            // Определяем колонки для Grid с соотношением 1 к 4
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(7, GridUnitType.Star) });

            // Добавляем текст слева от TextBox
            if (!string.IsNullOrWhiteSpace(leftText))
            {
                TextBlock leftTextBlock = new TextBlock
                {
                    Text = leftText,
                    Margin = new Thickness(5, 0, 5, 0)
                };
                Grid.SetColumn(leftTextBlock, 0);
                grid.Children.Add(leftTextBlock);
            }

            // Создаем и добавляем поле ввода текста
            TextBox inputTextBox = new TextBox
            {
                Text = inputText,
                Margin = new Thickness(5, 0, 5, 0)
            };
            Grid.SetColumn(inputTextBox, 1);
            grid.Children.Add(inputTextBox);

            componentPanel.Children.Add(grid);

            // Добавляем текст снизу, если он не пустой
            if (!string.IsNullOrWhiteSpace(bottomText))
            {
                TextBlock bottomTextBlock = new TextBlock
                {
                    Text = bottomText,
                    FontStyle = FontStyles.Italic,
                    Margin = new Thickness(5, 5, 5, 0)
                };
                componentPanel.Children.Add(bottomTextBlock);
            }

            // Добавляем созданный компонент в контейнер
            container.Children.Add(componentPanel);
            return inputTextBox;
        }

        public bool IsValidFormula(string formula)
        {
            try
            {
                // Создаем экземпляр выражения
                var expression = new NCalc.Expression(formula);

                // Пытаемся выполнить вычисление выражения
                var result = expression.Evaluate();

                // Если формула корректна, результат должен быть не null
                return result != null;
            }
            catch (Exception)
            {
                // Если возникает исключение, формула некорректна
                return false;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            string[] comands = File.ReadAllLines("formuls.txt", Encoding.Default);
            foreach (TextBox tb in sformuls.Keys)
            {
                comands[sformuls[tb]] = $"{snames[sformuls[tb]]}; {tb.Text}";
            }
            File.WriteAllLines("formuls.txt", comands, Encoding.Default);
            Close();
        }
    }
}
