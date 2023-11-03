using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Задание_1
{
    public partial class MainWindow : Window
    {
        ObservableCollection<TaskItem> tasks = new ObservableCollection<TaskItem>();

        public MainWindow()
        {
            InitializeComponent();
            ToDoGrid.ItemsSource = tasks;
        }

        private void AddTask_Click(object sender, RoutedEventArgs e)
        {
            tasks.Add(new TaskItem());
        }

        private void DeleteTask_Click(object sender, RoutedEventArgs e)
        {
            if (ToDoGrid.SelectedItem != null)
            {
                tasks.Remove((TaskItem)ToDoGrid.SelectedItem);
            }
        }

        private void RefreshData_Click(object sender, RoutedEventArgs e)
        {
            ToDoGrid.Items.Refresh();
        }
    }

    public class TaskItem
    {
        public DateTime Date { get; set; }
        public int Priority { get; set; }
        public int Progress { get; set; }
        public string Task { get; set; }
    }
}