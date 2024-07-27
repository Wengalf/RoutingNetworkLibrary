using RoutingNetworkLibrary;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RoutingNetworkUiPlayground
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            new DummyTest(this).InitWithTestDummys();
        }
    }

    public class DummyTest
    {
        private readonly Dictionary<Line, Button[]> LineButtonsMap = [];//TODO: Überlegen ob es wirklich sinn voll ist das getrent zu pflegen eigentlich hab ich am LineNodesMap bedarf.
        private readonly Dictionary<Line, Node[]> LineNodesMap = [];

        private readonly Dictionary<Button, Node> ButtonNodeMap = new()
        {
            { new(), new Node() { Name= "N1" } },
            { new(), new Node() { Name= "N2" } },
            { new(), new Node() { Name= "N3" } },
            { new(), new Node() { Name= "N4" } },
            { new(), new Node() { Name= "N5" } },
        };

        public DummyTest(MainWindow mainWindow)
        {
            MainWindow = mainWindow;
            MainWindow.Loaded += MainWindow_Loaded;
            MainWindow.RoutingAreaCanvas.MouseDown += OnRoutingAreaCanvasMouseDown;
        }

        public MainWindow MainWindow { get; }

        public void InitWithTestDummys()
        {
            ButtonNodeMap.Values.ElementAt(0).Connect(ButtonNodeMap.Values.ElementAt(1));
            ButtonNodeMap.Values.ElementAt(0).Connect(ButtonNodeMap.Values.ElementAt(2));
            ButtonNodeMap.Values.ElementAt(1).Connect(ButtonNodeMap.Values.ElementAt(4));
            ButtonNodeMap.Values.ElementAt(2).Connect(ButtonNodeMap.Values.ElementAt(3));
            ButtonNodeMap.Values.ElementAt(3).Connect(ButtonNodeMap.Values.ElementAt(1));
            ButtonNodeMap.Values.ElementAt(3).Connect(ButtonNodeMap.Values.ElementAt(4));

            double[,] possitions =
            {
                { 10,10},
                { 200,10},
                { 50,100},
                { 130,200},
                { 300,150},
            };

            int i = 0;
            foreach (var map in ButtonNodeMap)
            {
                map.Key.Content = map.Value.Name;
                map.Key.Click += OnNodeClick;
                map.Key.MouseEnter += OnNodeMouseEnter;
                map.Key.MouseLeave += OnNodeMouseLeave;
                AddToCanvas(map.Key, possitions[i, 0], possitions[i, 1]);
                i++;
            }
        }

        private void AddToCanvas<T>(T control, double left, double top) where T : Control
        {
            Canvas.SetLeft(control, left);
            Canvas.SetTop(control, top);
            MainWindow.RoutingAreaCanvas.Children.Add(control);
            Canvas.SetZIndex(control, 1);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var map in ButtonNodeMap)
            {
                DrawRouting(map.Key, map.Value);
            }
        }

        private void DrawRouting<T>(T rootControl, Node rootNode) where T : Button //Control
        {
            var rootLeft = Canvas.GetLeft(rootControl);
            var rootTop = Canvas.GetTop(rootControl);

            var neighborNodes = rootNode.GetNeighborNodes();

            foreach (var neighborNode in neighborNodes)
            {
                var neighborButton = ButtonNodeMap.First(map => map.Value == neighborNode).Key;

                var buttonLineMap = LineButtonsMap.Values.FirstOrDefault(buttons => buttons.Contains(rootControl) && buttons.Contains(neighborButton));
                if (buttonLineMap != null)
                {
                    //Linie zwischen den beiden Button wurde bereits gezeichnet.
                    continue;
                }

                var targetLeft = Canvas.GetLeft(neighborButton);
                var targetTop = Canvas.GetTop(neighborButton);

                Line line = new Line
                {
                    Stroke = Brushes.Black,
                    X1 = rootLeft + rootControl.ActualWidth / 2,
                    Y1 = rootTop + rootControl.ActualHeight / 2,
                    X2 = targetLeft + neighborButton.ActualWidth / 2,
                    Y2 = targetTop + neighborButton.ActualHeight / 2,
                    StrokeThickness = 4,
                    ToolTip = $"{rootNode.Name} -> {neighborNode.Name}",
                };

                LineButtonsMap[line] = [rootControl, neighborButton];
                LineNodesMap[line] = [rootNode, neighborNode];

                MainWindow.RoutingAreaCanvas.Children.Add(line);
                //Canvas.SetZIndex(line, 2);
            }
        }

        private Button? SelectedButton { get; set; }

        private void OnNodeClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                SwitchSelectedButtonTo(button);
            }
        }

        private void OnRoutingAreaCanvasMouseDown(object sender, MouseButtonEventArgs e)
        {
            SwitchSelectedButtonTo(null);
        }

        private void SwitchSelectedButtonTo(Button? button)
        {
            SelectedButton = button;
            MainWindow.SelectedNodeName.Content = SelectedButton?.Content;
        }

        private void OnNodeMouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Button button
                && SelectedButton != null
                && SelectedButton != button)
            {
                SwitchTargetButtonTo(button);
            }
        }

        private void OnNodeMouseLeave(object sender, MouseEventArgs e)
        {
            SwitchTargetButtonTo(null);
        }

        private void SwitchTargetButtonTo(Button? targetButton)
        {
            MainWindow.TargetNodeName.Content = targetButton?.Content;

            if (SelectedButton != null
                && targetButton != null)
            {
                DrawRoutingPaths(SelectedButton, targetButton);
            }
            else
            {
                MainWindow.BestPathText.Content = null;
                MainWindow.AllPathsText.Content = null;

                foreach (var line in LineNodesMap.Keys)
                {
                    line.Stroke = Brushes.Black;
                }
            }
        }

        private void DrawRoutingPaths(Button startButton, Button targetButton)
        {
            var startNode = ButtonNodeMap[startButton];
            var targetNode = ButtonNodeMap[targetButton];

            var bestPath = startNode.GetBestPathTo(targetNode);
            MainWindow.BestPathText.Content = $"({bestPath.Count()}) {string.Join(" -> ", bestPath.Select(n => n.Name))}";

            var allPaths = startNode.GetAllPathTo(targetNode);
            Dictionary<Line, List<Color>> LineColorsMap = [];
            TextBlock textBlock = new TextBlock();
            textBlock.Inlines.Add(new Run($"{allPaths.Count()} paths found") { Foreground = Brushes.Black });

            int colorIndex = 0;
            foreach (var path in allPaths)
            {
                var color = LineColors.ElementAt(colorIndex);

                for (int i = 0; i < path.Count() - 1; i++)
                {
                    var line = LineNodesMap.First(map => map.Value.Contains(path.ElementAt(i)) && map.Value.Contains(path.ElementAt(i + 1))).Key;
                    if (!LineColorsMap.ContainsKey(line)) LineColorsMap[line] = [];
                    LineColorsMap[line].Add(color);
                }

                Run run = new Run($"{Environment.NewLine}({path.Count()}) {string.Join(" -> ", path.Select(n => n.Name))}");
                run.Foreground = new SolidColorBrush(color);
                textBlock.Inlines.Add(run);

                colorIndex++;
            }

            foreach (var map in LineColorsMap)
            {
                map.Key.Stroke = GetColoredBrush(map.Value);
            }

            MainWindow.AllPathsText.Content = textBlock;
        }

        private List<Color> LineColors { get; } = new List<Color>()
        {
            Colors.Red,
            Colors.Green,
            Colors.Blue,
            Colors.Orange,
            Colors.Teal,
            Colors.Violet,
            Colors.Indigo,
            Colors.Yellow,
            Colors.Brown,
            Colors.Gray,
        };

        private Brush GetColoredBrush(IEnumerable<Color> colors)
        {
            LinearGradientBrush brush = new LinearGradientBrush();
            brush.StartPoint = new Point(0, 0);
            brush.EndPoint = new Point(0.25, 0);
            brush.SpreadMethod = GradientSpreadMethod.Repeat;

            int countColors = colors.Count();
            for (int i = 0; i < countColors; i++)
            {
                double position = i / (double)countColors;
                brush.GradientStops.Add(new GradientStop(colors.ElementAt(i), position));
                brush.GradientStops.Add(new GradientStop(colors.ElementAt(i), position + 1.0 / countColors));
            }

            return brush;
        }
    }
}