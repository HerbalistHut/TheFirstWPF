using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using Engine.ViewModels;
using Engine.Models;
using Engine.EventArgs;
using WpfApp1;
using System.Runtime.InteropServices;

namespace WPFUI
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly GameSession _gameSession = new GameSession();
        private readonly Dictionary<Key, Action> _userInputActions = 
            new Dictionary<Key, Action>();
        public MainWindow()
        {
            InitializeComponent();

            InitializeUserInputActions();

            DataContext = _gameSession;
            _gameSession.OnMessageRaised += OnGameMessageRaised;
        }

        private void OnClick_MoveNorth(object sender, RoutedEventArgs e)
        {
            _gameSession.MoveNorth();
        }

        private void OnClick_MoveWest(object sender, RoutedEventArgs e) 
        {
            _gameSession.MoveWest();
        }

        private void OnClick_MoveEast(object sender, RoutedEventArgs e) 
        {
            _gameSession.MoveEast();
        }
        
        private void OnClick_MoveSouth(object sender, RoutedEventArgs e) 
        {
            _gameSession.MoveSouth();
        }

        private void OnClick_AttackMonster(object sender, RoutedEventArgs e)
        {
            _gameSession.AttackCurrentMonster();
        }

        private void OnClick_UseCurrentConsumable(object sender, RoutedEventArgs e)
        {
            _gameSession.CurrentPlayer.UseCurrentConsumable();
        }

        private void OnClick_Craft(object sender, RoutedEventArgs e)
        {
            Recipe recipe = ((FrameworkElement)sender).DataContext as Recipe;
            _gameSession.CraftItemUsing(recipe);
        }

        private void InitializeUserInputActions()
        {
            _userInputActions.Add(Key.W, () => _gameSession.MoveNorth());
            _userInputActions.Add(Key.S, () => _gameSession.MoveSouth());
            _userInputActions.Add(Key.A, () => _gameSession.MoveWest());
            _userInputActions.Add(Key.D, () => _gameSession.MoveEast());
            _userInputActions.Add(Key.E, () => _gameSession.AttackCurrentMonster());
            _userInputActions.Add(Key.H, () => _gameSession.CurrentPlayer.UseCurrentConsumable());
        }

        private void MainWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (_userInputActions.ContainsKey(e.Key))
            {
                _userInputActions[e.Key].Invoke();
            }
        }
        private void OnClick_DisplayTradeScreen(object sender, RoutedEventArgs e)
        {
            TradeScreen tradeScreen = new TradeScreen();

            // Конкретно тут это используется для того, чтобы отцентрова TradeScreen относительно главного окна
            tradeScreen.Owner = this;
            tradeScreen.DataContext = _gameSession;

            // Используется ShowDialog(), а не Show() для того, чтобы пользователь не мог взаимодействовать с MainWindow, пока TradeScreen не будет закрыт 
            tradeScreen.ShowDialog();
        }

        // Таким образом мы общаемся между ViewModel и View
        // Есть более простые пути для реализации вывода сообщений в RichTextBox, но такой подход позволяет разделять View от ViewModel
        private void OnGameMessageRaised(object sender, GameMessageEventArgs e)
        {
            GameMessages.Document.Blocks.Add(new Paragraph(new Run(e.Message)));
            GameMessages.ScrollToEnd();
        }
        
        private void OnClick_KillMonster(object sender, RoutedEventArgs e)
        {
            _gameSession.CurrentMonster.TakeDamege(9999);
            _gameSession.GetMonsterAtLocation();
        }
    }
}
