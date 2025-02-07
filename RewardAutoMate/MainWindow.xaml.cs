using BaseTool;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RewardAutoMate;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private const int ROWS = 7;
    private const int COLUMNS = 52;
    [AllowNull]
    private List<QueueItem> HeatmapData = [];

    private readonly Task MainTask;

    public MainWindow()
    {
        InitializeComponent();

        HeatmapData = QueueService.DequeueTop365();

        CreateHeatmap();
        UpdateHeatmap();

        // 采用一发即忘的方式，让线程在后台持续运行
        MainTask = AutoSearchFunction();
    }

    private async Task AutoSearchFunction()
    {
        while (true)
        {
            QueueItem _item = QueueService.GetTodayCount();
            _item.Count += await EdgeServer.AutoSearch();

            // 自动搜索结束后更新数据库和热力图
            QueueService.UpdateItem(_item);
            HeatmapData = QueueService.DequeueTop365();
            Dispatcher.Invoke(UpdateHeatmap);

            await Task.Delay(TimeSpan.FromHours(12));
        }
    }

    private void CreateHeatmap()
    {
        for (int i = 0; i < ROWS; i++)
        {
            HeatmapGrid.RowDefinitions.Add(new RowDefinition());
        }
        for (int j = 0; j < COLUMNS; j++)
        {
            HeatmapGrid.ColumnDefinitions.Add(new ColumnDefinition());
        }

        for (int i = 0; i < ROWS; i++)
        {
            for (int j = 0; j < COLUMNS; j++)
            {
                Border border = new Border
                {
                    Width = 10,
                    Height = 10,
                    Margin = new Thickness(2),
                };
                Grid.SetRow(border, i);
                Grid.SetColumn(border, j);

                HeatmapGrid.Children.Add(border);
            }
        }
    }

    private void UpdateHeatmap()
    {
        // 假设从一年前的今天开始作为起始日期
        DateTime _start_time = DateTime.Today.AddYears(-1);
        while (_start_time.DayOfWeek != DayOfWeek.Monday)
        {
            _start_time = _start_time.AddDays(1);
        }
        for (int i = 0; i < ROWS; i++)
        {
            for (int j = 0; j < COLUMNS; j++)
            {
                DateTime currentDate = _start_time.AddDays( j * 7 + i);

                Border border = (Border)HeatmapGrid.Children[i * COLUMNS + j];
                border.ToolTip = new ToolTip
                {
                    Content = $"""
                        更新日期: {currentDate.ToShortDateString()}
                        更新次数: {GetCountByDate(currentDate)}
                        """,
                };
                border.Background = GetColorFromValue(GetCountByDate(currentDate));
            }
        }
    }

    private uint GetCountByDate(DateTime date) => HeatmapData.FirstOrDefault(_item => _item.Date == date)?.Count ?? 0;

    private static SolidColorBrush GetColorFromValue(uint value) => value switch
    {
        0 => new SolidColorBrush(Color.FromRgb(235, 237, 240)),
        <= 40 => new SolidColorBrush(Color.FromRgb(195, 232, 193)),
        <= 80 => new SolidColorBrush(Color.FromRgb(129, 201, 143)),
        <= 120 => new SolidColorBrush(Color.FromRgb(57, 163, 105)),
        >= 120 => new SolidColorBrush(Color.FromRgb(22, 99, 66)),
    };

    private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        BaseAction.Cancel();
        await MainTask;
    }
}