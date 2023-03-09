﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Engine.ViewModels;
using Engine.EventArgs;

namespace WPFUI
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        GameSession _gameSession;
        public MainWindow()
        {
            InitializeComponent();

            _gameSession = new GameSession();
            DataContext = _gameSession;
            _gameSession.OnMessageRised += OnGameMessageRaised;
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

        // Таким образом мы общаемся между ViewModel и View
        // Есть более простые пути для реализации вывода сообщений в RichTextBox, но такой подход позволяет разделять View от ViewModel
        private void OnGameMessageRaised(object sender, GameMessageEventArgs e)
        {
            GameMessages.Document.Blocks.Add(new Paragraph(new Run(e.Message)));
            GameMessages.ScrollToEnd();
        }
        
    }
}
