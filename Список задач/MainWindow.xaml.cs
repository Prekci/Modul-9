using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;

namespace Список_задач
{
    public partial class MainWindow : Window
    {
        ObservableCollection<TaskItem> tasks = new ObservableCollection<TaskItem>();
        public MainWindow()
        {
            InitializeComponent();
            ToDoGrid.ItemsSource = tasks;
            InitializeTimer(); // Инициализировать таймер
        }
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            tasks.Add(new TaskItem());
        }
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (ToDoGrid.SelectedItem != null)
            {
                tasks.Remove((TaskItem)ToDoGrid.SelectedItem);
            }
        }
        private void Update_Click(object sender, RoutedEventArgs e)
        {
            ToDoGrid.Items.Refresh();
            DateTime currentDateTime = DateTime.Now;
            foreach (var task in tasks)
            {
                TimeSpan timeRemaining = task.Date - currentDateTime;
                if (timeRemaining <= TimeSpan.FromDays(3) && timeRemaining >= TimeSpan.Zero)
                {
                    MessageBox.Show($"До конца задачи \"{task.Task}\" осталось {timeRemaining.Days} дня(ей).", "Скоро сдача", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (timeRemaining < TimeSpan.Zero)
                {
                    MessageBox.Show($"Задача \"{task.Task}\" просрочена.", "Просрочено", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void InitializeTimer()
        {
            var timer = new DispatcherTimer();
            timer.Tick += (sender, e) => CheckTaskDeadlines();
            timer.Interval = TimeSpan.FromMinutes(1); // Проверять каждую минуту
            timer.Start();
        }
        private void CheckTaskDeadlines()
        {
            DateTime currentDate = DateTime.Now;
            foreach (var task in tasks)
            {
                if (task.Date < currentDate)
                {
                    ShowNotification("Просрочено!", "Срок выполнения задачи \"" + task.Task + "\" истек.");
                }
                else if ((task.Date - currentDate).TotalDays <= 3)
                {
                    ShowNotification("Скорая сдача!", "Срок выполнения задачи \"" + task.Task + "\" скоро истекает.");
                }
            }
        }
        private void ShowNotification(string title, string message)
        {
            // Ваш код для отображения уведомления, например, использование NotificationService.
        }
        private int priorityClickCount = 0;
        private void ToDoGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.Column.Header.ToString() == "Приоритет")
            {
                var cellContent = e.EditingElement as TextBox;
                if (cellContent != null)
                {
                    var taskItem = ToDoGrid.SelectedItem as TaskItem;
                    if (taskItem != null)
                    {
                        int newPriority;
                        if (int.TryParse(cellContent.Text, out newPriority) && newPriority >= 1 && newPriority <= 3)
                        {
                            taskItem.Priority = newPriority;
                            e.Column.CellStyle = GetPriorityCellStyle(taskItem.Priority);
                        }
                        else
                        {
                            MessageBox.Show("Приоритет должен быть целым числом от 1 до 3. " +
                                            "1 - время на выполнение есть, 2 - скоро сдача, 3 - горит дедлайн.");
                            e.Cancel = true;
                        }
                    }
                }
            }
        }
        private Style GetPriorityCellStyle(int priority)
        {
            var style = new Style(typeof(DataGridCell));
            if (priority == 1)
            {
                style.Setters.Add(new Setter(Control.BackgroundProperty, new SolidColorBrush(Colors.Green)));
            }
            else if (priority == 2)
            {
                style.Setters.Add(new Setter(Control.BackgroundProperty, new SolidColorBrush(Colors.Yellow)));
            }
            else if (priority == 3)
            {
                style.Setters.Add(new Setter(Control.BackgroundProperty, new SolidColorBrush(Colors.Red)));
            }
            return style;
        }
    }
    public class ProgressToProgressBarConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int progress)
            {
                var progressBar = new ProgressBar
                {
                    Value = progress,
                    Maximum = 100
                };
                return progressBar;
            }
            return DependencyProperty.UnsetValue;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class TaskItem : INotifyPropertyChanged
    {
        private DateTime date;
        public DateTime Date
        {
            get { return date; }
            set
            {
                date = value;
                OnPropertyChanged(nameof(Date));
            }
        }
        private int priority;
        public int Priority
        {
            get { return priority; }
            set
            {
                if (value >= 1 && value <= 3)
                {
                    priority = value;
                }
                else
                {
                    MessageBox.Show("Приоритет должен быть целым числом от 1 до 3. " +
                                    "1 - время на выполнение есть, 2 - скоро сдача, 3 - горит дедлайн.");
                }
                OnPropertyChanged(nameof(Priority));
            }
        }
        private int progress;
        public int Progress
        {
            get { return progress; }
            set
            {
                if (value >= 0 && value <= 100)
                {
                    progress = value;
                }
                OnPropertyChanged(nameof(Progress));
            }
        }
        private string task;
        public string Task
        {
            get { return task; }
            set
            {
                task = value;
                OnPropertyChanged(nameof(Task));
            }
        }
        // Добавьте свойство DateTime
        private DateTime dateTime;
        public DateTime DateTime
        {
            get { return dateTime; }
            set
            {
                dateTime = value;
                OnPropertyChanged(nameof(DateTime));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}