using Microsoft.Win32;
using NCalc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

#pragma warning disable SYSLIB0011 // Отключает предупреждение для BinaryFormatter

namespace GOB_Life_Wpf
{
    public partial class MainWindow : Window
    {
        static Simulation.main Main = new Simulation.main();

        int frame = 0;
        // Новые поля для обработки перетаскивания мыши по MapBox
        private bool isMouseDownOnMap = false;
        private int lastMouseCellX = -1;
        private int lastMouseCellY = -1;

        public MainWindow()
        {
            InitializeComponent();
            Simulation.Visualize.LoadGradients();

            // Подписываемся на дополнительные события мыши для MapBox
            // __MapBox__ — это Image в XAML, у которого уже присутствует обработчик MouseLeftButtonUp.
            // Здесь добавляем обработчики для зажатой кнопки и перемещения.
            MapBox.MouseLeftButtonDown += MapBox_MouseLeftButtonDown;
            MapBox.MouseMove += MapBox_MouseMove;
            MapBox.MouseLeave += MapBox_MouseLeave;
        }

        public static void RenderImage(byte[] pixelData, int width, int height, Image targetImage)
        {
            var bitmap = BitmapSource.Create(
                width, height, 96, 96,
                PixelFormats.Bgra32, null,
                pixelData, width * 4);

            targetImage.Source = bitmap;
        }

        public static void RenderImage(BitmapSource bitmap, Image targetImage)
        {
            targetImage.Source = bitmap;
        }

        private bool isRunning = false;
        private string generate = "";
        private SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        private async void StartSim_Click(object sender, RoutedEventArgs e)
        {
            await semaphore.WaitAsync();
            try
            {
                if (int.TryParse(mapWidthInput.Text, out int w) && int.TryParse(mapHeightInput.Text, out int h))
                {
                    Main.width = w;
                    Main.height = h;
                }

                if (generate == seedInput.Text || seedInput.Text == "")
                {
                    generate = seedInput.Text = Main.rnd.Next(int.MinValue, int.MaxValue).ToString();
                }
                else
                {
                    generate = "";
                }

                Main.rnd = new Random(int.Parse(seedInput.Text));

                await Task.Run(() => { Simulation.Formuls.Load(); Main.RandomFill(); });

                if (!isRunning)
                {
                    isRunning = true;
                    _ = StartSimulationAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                semaphore.Release();
            }
        }

        private async Task StartSimulationAsync()
        {
            while (isRunning)
            {
                await semaphore.WaitAsync();
                try
                {
                    if (isRunning)
                    {
                        await Task.Run(() => Tick());
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка в симуляции: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    isRunning = false;
                }
                finally
                {
                    semaphore.Release();
                }

                await Task.Delay(0);
            }
        }

        private void Tick()
        {
            try
            {
                Main.Tick();

                int w = (int)MapBorder.ActualWidth;
                int h = (int)MapBorder.ActualHeight;

                // Асинхронный вызов для рендеринга изображения и обновления UI

                Task.Run(() =>
                Application.Current.Dispatcher.Invoke(() =>
                {
                    int saveSteps = int.Parse(rocordInput.Text);
                    bool saveFrame = RecordingCheck.IsChecked.Value && (Main.step % saveSteps == 0);

                    simInfoText.Content = $"Шаг {Main.step}, {Main.queue.Count} клеток";
                    if (renderChexBox.IsChecked.Value || saveFrame)
                    {
                        var image = Simulation.Visualize.Map(ref w, ref h, vizMode.SelectedIndex, oxRengerBox.IsChecked.Value);
                        var bitmap = BitmapSource.Create(w, h, 96, 96, PixelFormats.Bgra32, null, image, w * 4);

                        if (renderChexBox.IsChecked.Value || saveFrame)
                            RenderImage(bitmap, MapBox);

                        if (saveFrame)
                        {
                            Save(bitmap, w, h);
                            frame++;
                        }
                    }

                    if (simsaveCheck.IsChecked.Value && Main.step % int.Parse(simsaveInput.Text) == 0)
                    {
                        if (!Directory.Exists("Saves"))
                            Directory.CreateDirectory("Saves");
                        Simulation.Serialization.Save($"Saves/{Main.step}.sim");
                    }
                })
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка в методе Tick: В {ex.Source} {ex.TargetSite} {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void Pause_Click(object sender, RoutedEventArgs e)
        {
            await semaphore.WaitAsync();
            try
            {
                isRunning = !isRunning;
                if (isRunning)
                {
                    _ = StartSimulationAsync();
                }
                pause.Content = isRunning ? "| |" : "▶";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при паузе: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                semaphore.Release();
            }
        }

        private async void Step_Click(object sender, RoutedEventArgs e)
        {
            await semaphore.WaitAsync();
            try
            {
                if (!isRunning)
                {
                    await Task.Run(() => Tick());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при шаге: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                semaphore.Release();
            }
        }

        private T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T)
                {
                    return (T)child;
                }
                else
                {
                    var result = FindVisualChild<T>(child);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }
            return null;
        }

        private async void MapBox_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            isMouseDownOnMap = true;
            // Сбрасываем последнее обработанное положение, чтобы первое попадание точно обработалось
            lastMouseCellX = lastMouseCellY = -1;
            await ProcessMapActionAsync(e.GetPosition(MapBox));
        }

        private async void MapBox_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!isMouseDownOnMap)
                return;

            await ProcessMapActionAsync(e.GetPosition(MapBox));
        }

        private void MapBox_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            // Останавливаем режим рисования при уходе курсора с MapBox
            isMouseDownOnMap = false;
            lastMouseCellX = lastMouseCellY = -1;
        }

        private async Task ProcessMapActionAsync(Point mousePositionInMapBox)
        {
            await semaphore.WaitAsync();
            try
            {
                var image = MapBox;

                if (image != null && image.Source != null)
                {
                    double imageWidth = image.Source.Width;
                    double imageHeight = image.Source.Height;
                    double containerWidth = image.ActualWidth;
                    double containerHeight = image.ActualHeight;
                    double scaleX = containerWidth / imageWidth;
                    double scaleY = containerHeight / imageHeight;

                    double relativeX = mousePositionInMapBox.X / scaleX;
                    double relativeY = mousePositionInMapBox.Y / scaleY;

                    int x = (int)Math.Round(relativeX / (imageWidth / Main.width));
                    int y = (int)Math.Round(relativeY / (imageHeight / Main.height));

                    // Проверяем границы
                    if (x < 0 || x >= Main.width || y < 0 || y >= Main.height)
                        return;

                    // Избегаем многократной обработки той же клетки при движении
                    if (x == lastMouseCellX && y == lastMouseCellY)
                        return;

                    lastMouseCellX = x;
                    lastMouseCellY = y;

                    switch (mouseAction.SelectedIndex)
                    {
                        case 0:
                            if (Main.cmap[x, y] != null)
                            {
                                var infoWin = new InfoWin();
                                infoWin.Show();
                                infoWin.Activate();

                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    RenderImage(Simulation.Visualize.Brain(Main.cmap[x, y], out int w, out int h), w, h, infoWin.BrainBox);
                                    RenderImage(Simulation.Visualize.Dna(Main.cmap[x, y], out int Dw, out int Dh), Dw, Dh, infoWin.DnaBox);

                                    StringBuilder dna = new StringBuilder();
                                    bool first = true;
                                    foreach (var n in Main.cmap[x, y].DNA)
                                    {
                                        if (!first)
                                            dna.Append(" ");
                                        dna.Append(n.ToString());
                                        first = false;
                                    }
                                    infoWin.dnaText.Text = dna.ToString();
                                });
                            }
                            break;

                        case 1:
                            if (Main.cmap[x, y] != null)
                                Main.cmap[x, y].nrj = 0;
                            break;

                        case 2:
                            if (Main.cmap[x, y] == null && Main.fmap[x, y] == null)
                                Main.fmap[x, y] = new Simulation.Food(x, y, 10);
                            break;

                        case 3:
                            Main.queue.Remove(Main.cmap[x, y]);
                            Main.bqueue.Remove(Main.cmap[x, y]);
                            Main.cmap[x, y] = null;
                            Main.fmap[x, y] = null;
                            break;

                        case 4:
                            if (Main.cmap[x, y] == null && Main.fmap[x, y] == null)
                            {
                                string[] tdna = Clipboard.GetText().Split();
                                Simulation.Gtype[] Idna = new Simulation.Gtype[tdna.Length];
                                for (int i = 0; i < Idna.Length; i++)
                                {
                                    if (!Enum.TryParse(tdna[i], out Idna[i]))
                                        return;
                                }
                                Main.queue.Add(new Simulation.Bot(x, y, 10, Main.rnd.Next(int.MinValue, int.MaxValue), Idna));
                                Main.cmap[x, y] = Main.queue.Last();
                            }
                            break;
                    }

                    if (!isRunning)
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            int renW = (int)MapBorder.ActualWidth;
                            int renH = (int)MapBorder.ActualHeight;
                            RenderImage(Simulation.Visualize.Map(ref renW, ref renH, vizMode.SelectedIndex, oxRengerBox.IsChecked.Value), renW, renH, MapBox);
                        });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обработке нажатия: В {ex.Source} {ex.TargetSite} {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                semaphore.Release();
            }
        }

        private async void MapBox_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Завершаем режим "зажата" и в любом случае один раз обрабатываем позицию отпускания
            isMouseDownOnMap = false;
            await ProcessMapActionAsync(e.GetPosition(MapBox));
            lastMouseCellX = lastMouseCellY = -1;
        }

        private void Save(BitmapSource bitmap, int width, int height)
        {
            if (!Directory.Exists("Record"))
                Directory.CreateDirectory("Record");
            using (var fileStream = new FileStream($"Record/{frame}.png", FileMode.Create))
            {
                new PngBitmapEncoder { Frames = { BitmapFrame.Create(bitmap) } }.Save(fileStream);
            }
        }

        private void ClearRecord(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists("Record"))
                return;

            frame = 0;
            string[] files = Directory.GetFiles("Record");
            foreach (string file in files)
            {
                File.Delete(file);
            }
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            SettingsWin win = new SettingsWin();
            win.Show();
            win.Activate();
        }

        private void SaveSim_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Simulation Files (*.sim)|*.sim|All Files (*.*)|*.*",
                DefaultExt = "sim"
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                Simulation.Serialization.Save(saveFileDialog.FileName);
            }
        }

        private void LoadSim_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
    "Для сохранения и загрузки симуляций используется уязвимый BinaryFormatter. Открывайте только те файлы, которым доверяете. Хотите продолжить?",
    "Предупреждение безопасности",
    MessageBoxButton.OKCancel,
    MessageBoxImage.Warning);

            if (result != MessageBoxResult.OK)
                return;

            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Simulation Files (*.sim)|*.sim|All Files (*.*)|*.*",
                DefaultExt = "sim"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                Simulation.Serialization.Load(openFileDialog.FileName);
                if (!isRunning)
                {
                    int renW = Main.width;
                    int renH = Main.height;
                    RenderImage(Simulation.Visualize.Map(ref renW, ref renH, vizMode.SelectedIndex, oxRengerBox.IsChecked.Value), renW, renH, MapBox);
                }
            }
        }

        private void VizMode_DropDownClosed(object sender, EventArgs e)
        {
            if (!isRunning && Main.cmap != null)
                Application.Current.Dispatcher.Invoke(() =>
                {
                    int renW = (int)MapBorder.ActualWidth;
                    int renH = (int)MapBorder.ActualHeight;
                    RenderImage(Simulation.Visualize.Map(ref renW, ref renH, vizMode.SelectedIndex, oxRengerBox.IsChecked.Value), renW, renH, MapBox);
                });
        }

        private void oxRengerBox_Click(object sender, RoutedEventArgs e)
        {
            if (!isRunning && Main.cmap != null)
                Application.Current.Dispatcher.Invoke(() =>
                {
                    int renW = (int)MapBorder.ActualWidth;
                    int renH = (int)MapBorder.ActualHeight;
                    RenderImage(Simulation.Visualize.Map(ref renW, ref renH, vizMode.SelectedIndex, oxRengerBox.IsChecked.Value), renW, renH, MapBox);
                });
        }


        static class Simulation
        {
            private static List<T[]> SplitByElement<T>(T[] array, T delimiter)
            {
                List<T[]> result = new List<T[]>();
                List<T> temp = new List<T>();

                foreach (var item in array)
                {
                    if (item.Equals(delimiter))
                    {
                        result.Add(temp.ToArray());
                        temp.Clear();
                    }
                    else
                    {
                        temp.Add(item);
                    }
                }

                // Добавляем остаток после последнего разделителя, если он есть
                if (temp.Count > 0)
                {
                    result.Add(temp.ToArray());
                }

                return result;
            }

            private static T[] CombineWithDelimiter<T>(T[][] arrays, T delimiter)
            {
                List<T> result = new List<T>();

                for (int i = 0; i < arrays.Length; i++)
                {
                    if (i > 0)
                    {
                        result.Add(delimiter);
                    }
                    result.AddRange(arrays[i]);
                }

                return result.ToArray();
            }

            public enum Gtype
            {
                empty,
                //coddons:
                start,
                stop,
                input,
                output,
                skip,
                undo,
                //operations:
                add,
                sub,
                mul,
                div,
                grate,
                less,
                equal,
                not,
                mod,
                memory,
                and,
                or,
                xor,
                dup2,
                dup3,
                //chyvstva:
                rand,
                btime,
                time,
                bot,
                rbot,
                food,
                nrj,
                posx,
                posy,
                gen,
                fgen,
                mut,
                listnr,
                //deistvia...
                wait,
                photosyntes,
                rep,
                sex,
                Rrot,
                Lrot,
                walk,
                atack,
                suicide,
                recomb,
                trnsmt,
                //constants...
                c0,
                c1,
                c2,
                c5,
                c11,
            }

            public static class Visualize
            {
                static byte[][,] gradients;
                public class CustomImage
                {
                    private readonly byte[] pixels;
                    private readonly int width;
                    private readonly int height;

                    public CustomImage(int width, int height)
                    {
                        this.width = width;
                        this.height = height;
                        this.pixels = new byte[width * height * 4];
                    }

                    public byte[] Pixels => pixels;

                    public void ClearImage(Color color)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            for (int x = 0; x < width; x++)
                            {
                                int index = (y * width + x) * 4;
                                pixels[index] = color.B;
                                pixels[index + 1] = color.G;
                                pixels[index + 2] = color.R;
                                pixels[index + 3] = color.A;
                            }
                        }
                    }

                    public void DrawText(string text, int x, int y, Color color, int fontSize = 16)
                    {
                        var formattedText = new FormattedText(
                            text,
                            System.Globalization.CultureInfo.InvariantCulture,
                            FlowDirection.LeftToRight,
                            new Typeface("Arial"),
                            fontSize,
                            new SolidColorBrush(color),
                            1.0);

                        var drawingVisual = new DrawingVisual();
                        using (var drawingContext = drawingVisual.RenderOpen())
                        {
                            drawingContext.DrawText(formattedText, new Point(x, y));
                        }

                        var renderTarget = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
                        renderTarget.Render(drawingVisual);

                        var renderPixels = new byte[width * height * 4];
                        renderTarget.CopyPixels(renderPixels, width * 4, 0);

                        // Наложение новых пикселей на старые
                        for (int i = 0; i < pixels.Length; i += 4)
                        {
                            byte alpha = renderPixels[i + 3];
                            if (alpha > 0)
                            {
                                pixels[i] = renderPixels[i];
                                pixels[i + 1] = renderPixels[i + 1];
                                pixels[i + 2] = renderPixels[i + 2];
                                pixels[i + 3] = renderPixels[i + 3];
                            }
                        }
                    }

                    public void DrawCenteredText(string text, int rectX, int rectY, int rectWidth, int rectHeight, Color color, int fontSize = 16)
                    {
                        var formattedText = new FormattedText(
                            text,
                            System.Globalization.CultureInfo.InvariantCulture,
                            FlowDirection.LeftToRight,
                            new Typeface("Arial"),
                            fontSize,
                            new SolidColorBrush(color),
                            1.0);

                        double centerX = rectX + rectWidth / 2.0;
                        double centerY = rectY + rectHeight / 2.0;

                        double textX = centerX - (formattedText.Width / 2.0);
                        double textY = centerY - (formattedText.Height / 2.0);

                        DrawText(text, (int)textX, (int)textY, color, fontSize);
                    }

                    public void DrawLine(int x1, int y1, int x2, int y2, Color color)
                    {
                        var pen = new Pen(new SolidColorBrush(color), 1);
                        var drawingVisual = new DrawingVisual();
                        using (var drawingContext = drawingVisual.RenderOpen())
                        {
                            drawingContext.DrawLine(pen, new Point(x1, y1), new Point(x2, y2));
                        }

                        var renderTarget = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
                        renderTarget.Render(drawingVisual);

                        var renderPixels = new byte[width * height * 4];
                        renderTarget.CopyPixels(renderPixels, width * 4, 0);

                        // Наложение новых пикселей на старые
                        for (int i = 0; i < pixels.Length; i += 4)
                        {
                            byte alpha = renderPixels[i + 3];
                            if (alpha > 0)
                            {
                                pixels[i] = renderPixels[i];
                                pixels[i + 1] = renderPixels[i + 1];
                                pixels[i + 2] = renderPixels[i + 2];
                                pixels[i + 3] = renderPixels[i + 3];
                            }
                        }
                    }

                    public void DrawPoint(int x, int y, Color color)
                    {
                        if (x >= 0 && x < width && y >= 0 && y < height)
                        {
                            int index = (y * width + x) * 4;

                            byte oldB = pixels[index];
                            byte oldG = pixels[index + 1];
                            byte oldR = pixels[index + 2];
                            byte oldA = pixels[index + 3];

                            byte newB = color.B;
                            byte newG = color.G;
                            byte newR = color.R;
                            byte newA = color.A;

                            // Композитный альфа-канал
                            byte alpha = (byte)(newA + oldA * (255 - newA) / 255);

                            // Наложение цвета с учетом альфа-канала
                            pixels[index] = (byte)((newB * newA + oldB * oldA * (255 - newA) / 255) / alpha);
                            pixels[index + 1] = (byte)((newG * newA + oldG * oldA * (255 - newA) / 255) / alpha);
                            pixels[index + 2] = (byte)((newR * newA + oldR * oldA * (255 - newA) / 255) / alpha);
                            pixels[index + 3] = alpha;
                        }
                    }

                    public void DrawRectangle(int x, int y, int rectWidth, int rectHeight, Color color, bool fill = false)
                    {
                        if (fill)
                        {
                            for (int i = y; i < y + rectHeight; i++)
                            {
                                for (int j = x; j < x + rectWidth; j++)
                                {
                                    if (i >= 0 && i < height && j >= 0 && j < width)
                                    {
                                        int index = (i * width + j) * 4;
                                        byte oldAlpha = pixels[index + 3];
                                        byte newAlpha = color.A;
                                        byte alpha = (byte)(newAlpha + oldAlpha * (255 - newAlpha) / 255);
                                        pixels[index] = (byte)((color.B * newAlpha + pixels[index] * (255 - newAlpha)) / 255);
                                        pixels[index + 1] = (byte)((color.G * newAlpha + pixels[index + 1] * (255 - newAlpha)) / 255);
                                        pixels[index + 2] = (byte)((color.R * newAlpha + pixels[index + 2] * (255 - newAlpha)) / 255);
                                        pixels[index + 3] = alpha;
                                    }
                                }
                            }
                        }
                        else
                        {
                            DrawLine(x, y, x + rectWidth - 1, y, color);
                            DrawLine(x + rectWidth - 1, y, x + rectWidth - 1, y + rectHeight - 1, color);
                            DrawLine(x + rectWidth - 1, y + rectHeight - 1, x, y + rectHeight - 1, color);
                            DrawLine(x, y + rectHeight - 1, x, y, color);
                        }
                    }
                }

                private static void ColorFromGradient(double val, double min, double max, int gradient, out byte a, out byte r, out byte g, out byte b)
                {
                    double Map(double value, double fromLow, double fromHigh, double toLow, double toHigh)
                    {
                        return toLow + (value - fromLow) * (toHigh - toLow) / (fromHigh - fromLow);
                    }
                    int x = (int)Math.Round(Map(val, min, max, 0, gradients[gradient].Length));
                    if (x >= gradients[gradient].GetLength(0))
                        x = gradients[gradient].GetLength(0) - 1;
                    if (x < 0)
                        x = 0;

                    a = gradients[gradient][x, 0];
                    r = gradients[gradient][x, 1];
                    g = gradients[gradient][x, 2];
                    b = gradients[gradient][x, 3];
                }

                public static void LoadGradients()
                {
                    string[] images = Directory.GetFiles("Images");
                    gradients = new byte[images.Length][,];
                    for (int i = 0; i < gradients.Length; i++)
                    {
                        System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(images[i]);
                        gradients[i] = new byte[bmp.Width, 4];
                        for (int x = 0; x < bmp.Width; x++)
                        {
                            System.Drawing.Color c = bmp.GetPixel(x, 0);
                            gradients[i][x, 0] = c.A;
                            gradients[i][x, 1] = c.R;
                            gradients[i][x, 2] = c.G;
                            gradients[i][x, 3] = c.B;
                        }
                    }
                }

                static void GenToColor(int seed, out byte red, out byte green, out byte blue)
                {
                    bool IsGrayShade(int r, int g, int b)
                    {
                        // Определяем пороговое значение для различия между каналами
                        int threshold = 20; // Порог можно настраивать

                        // Проверяем, находятся ли значения каналов в пределах порогового значения друг от друга
                        return Math.Abs(r - g) <= threshold &&
                               Math.Abs(r - b) <= threshold &&
                               Math.Abs(g - b) <= threshold;
                    }

                    Random random = new Random(seed);
                    do
                    {
                        red = (byte)random.Next(256);
                        green = (byte)random.Next(256);
                        blue = (byte)random.Next(256);
                    } while (IsGrayShade(red, green, blue));

                    return;
                }

                static string GtypeToSrt(Gtype type)
                {
                    string caption;
                    switch (type)
                    {
                        case Gtype.recomb:
                            caption = "♻️";
                            break;
                        case Gtype.gen:
                            caption = "🧬";
                            break;
                        case Gtype.fgen:
                            caption = "F🧬";
                            break;
                        case Gtype.mut:
                            caption = "⚠️";
                            break;
                        case Gtype.posx:
                            caption = "x";
                            break;
                        case Gtype.posy:
                            caption = "y";
                            break;
                        case Gtype.time:
                            caption = "⌚️";
                            break;
                        case Gtype.add:
                            caption = "➕";
                            break;
                        case Gtype.sub:
                            caption = "➖";
                            break;
                        case Gtype.mul:
                            caption = "✖️";
                            break;
                        case Gtype.div:
                            caption = "➗";
                            break;
                        case Gtype.grate:
                            caption = ">";
                            break;
                        case Gtype.less:
                            caption = "<";
                            break;
                        case Gtype.equal:
                            caption = "=";
                            break;
                        case Gtype.not:
                            caption = "not";
                            break;
                        case Gtype.mod:
                            caption = "mod";
                            break;
                        case Gtype.memory:
                            caption = "💾";
                            break;
                        case Gtype.and:
                            caption = "and";
                            break;
                        case Gtype.or:
                            caption = "or";
                            break;
                        case Gtype.xor:
                            caption = "xor";
                            break;
                        case Gtype.dup2:
                        case Gtype.dup3:
                            caption = "";
                            break;
                        case Gtype.rand:
                            caption = "🎲";
                            break;
                        case Gtype.btime:
                            caption = "✨⌚️";
                            break;
                        case Gtype.bot:
                            caption = "🦠";
                            break;
                        case Gtype.rbot:
                            caption = "R🦠";
                            break;
                        case Gtype.food:
                            caption = "🍽";
                            break;
                        case Gtype.nrj:
                            caption = "⚡️";
                            break;
                        case Gtype.wait:
                            caption = "💤";
                            break;
                        case Gtype.photosyntes:
                            caption = "🌿";
                            break;
                        case Gtype.rep:
                            caption = "👨‍👦";
                            break;
                        case Gtype.sex:
                            caption = "👨‍👩‍👧";
                            break;
                        case Gtype.Rrot:
                            caption = "↪";
                            break;
                        case Gtype.Lrot:
                            caption = "↩";
                            break;
                        case Gtype.walk:
                            caption = "🚶‍";
                            break;
                        case Gtype.atack:
                            caption = "⚔️";
                            break;
                        case Gtype.suicide:
                            caption = "💀";
                            break;
                        case Gtype.trnsmt:
                            caption = "🔊";
                            break;
                        case Gtype.listnr:
                            caption = "👂";
                            break;
                        case Gtype.c0:
                            caption = "0";
                            break;
                        case Gtype.c1:
                            caption = "1";
                            break;
                        case Gtype.c2:
                            caption = "2";
                            break;
                        case Gtype.c5:
                            caption = "5";
                            break;
                        case Gtype.c11:
                            caption = "11";
                            break;
                        default:
                            caption = "?";
                            break;
                    }
                    return caption;
                }

                private static readonly List<(Gate, Node)> queue = new List<(Gate, Node)>();
                static readonly List<Edge> edges = new List<Edge>();

                class Edge
                {
                    public int x1, y1, x2, y2, layer;
                    public Edge(int x1, int y1, int x2, int y2, int layer)
                    {
                        this.x1 = x1;
                        this.y1 = y1;
                        this.x2 = x2;
                        this.y2 = y2;
                        this.layer = layer;
                    }
                }
                class Node
                {
                    public Gate g;
                    public int x, y, w, h;
                    public (int, int)[] input, output;
                    public int layer;
                    public Node(Gate gate, int x, int y, int layer)
                    {
                        g = gate;
                        bool dupe = g.type == Gtype.dup2 || g.type == Gtype.dup3; //особая отрисовка (линии)
                        this.x = x;
                        this.y = dupe ? y + 5 : y;
                        this.layer = layer;

                        w = 40;
                        h = dupe ? 0 : Math.Max(g.output.Length, g.input.Length) * 15 + 10;

                        input = new (int, int)[g.input.Length];
                        output = new (int, int)[g.output.Length];

                        // Вычисляем начальные координаты для входных точек
                        int inputSpacing = h / (g.input.Length + 1);
                        for (int i = 0; i < g.input.Length; i++)
                        {
                            input[i].Item1 = x + w; // Горизонтальная координата
                            input[i].Item2 = dupe ? y + 5 : y + (i + 1) * inputSpacing; // Вертикальная координата
                        }

                        // Вычисляем начальные координаты для выходных точек
                        int outputSpacing = h / (g.output.Length + 1);
                        for (int i = 0; i < g.output.Length; i++)
                        {
                            output[i].Item1 = x; // Горизонтальная координата
                            output[i].Item2 = dupe ? y + 5 : y + (i + 1) * outputSpacing; // Вертикальная координата
                        }
                    }
                }

                public static byte[] Brain(Bot e, out int w, out int h)
                {
                    w = 0; h = 0;
                    int layer = 0;
                    queue.Clear();
                    edges.Clear();
                    int x = 10, y = 0;
                    List<int> layersH = new List<int>() { -1 };
                    bool mem = true; ; //запомнить гейт
                    (Gate, Node) flg = (null, null); //запомненый гейт

                    foreach (Gate gate in e.gates) //определяем точки выхода
                    {
                        if (Main.exp.Contains(gate.type) && gate.input[0].A != null)
                        {
                            var node = new Node(gate, x, y, layer);
                            y += node.h + 10;

                            if (layersH[node.layer] < node.y + node.h)
                            {
                                layersH[node.layer] = node.y + node.h;
                                if (h < layersH[node.layer])
                                    h = layersH[node.layer] + 20;
                            }

                            queue.Add((gate, node));
                            if (mem)
                            {
                                flg = queue.Last();
                                mem = false;
                            }
                        }
                    }

                    for (int i = 0; i < queue.Count; i++)
                    {
                        var gate1 = queue[i].Item1;
                        var node = queue[i].Item2;

                        if (flg == queue[i]) //начинается новый слой
                        {
                            layer++;
                            mem = true;
                            w = x;
                            x += 80;
                            y = 0;
                            layersH.Add(-1);
                        }

                        if (layersH[node.layer] < node.y + node.h)
                        {
                            layersH[node.layer] = node.y + node.h;
                            if (h < layersH[node.layer])
                                h = layersH[node.layer] + 20;
                        }

                        for (int j = 0; j < gate1.input.Length; j++)
                        {
                            foreach (var gate2 in e.gates)
                            {
                                for (int l = 0; l < gate2.output.Length; l++)
                                {
                                    var l1 = gate2.output[l];
                                    if (l1 == gate1.input[j])
                                    {
                                        /*
                                        Посторайтесь понять)
                                        Для каждого гейта проходим по всем его ВХОДНЫМ портам
                                        И среди всех ВЫХОДНЫХ портов всех гейтов ищем пару

                                        Тут мы её как раз нашли и теперь создаём новую ноду и линию
                                        */

                                        var node2 = new Node(gate2, x, y, layer);
                                        y += node2.h == 0 ? 15 : node2.h + 5;

                                        int x1 = node.input[j].Item1;
                                        int y1 = node.input[j].Item2;
                                        int x2 = node2.output[l].Item1;
                                        int y2 = node2.output[l].Item2;
                                        edges.Add(new Edge(x1, y1, x2, y2, layer));
                                        queue.Add((gate2, node2));
                                        if (mem)
                                        {
                                            /*
                                             * это первая нода на новом слое и мы её запоминаем
                                             * когда мы начнём проходить по её ВХОДАМ это будет означать,
                                             * что начался новый слой
                                            */
                                            flg = queue.Last();
                                            mem = false;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    w += 50;

                    //сдвигаем все ноды по центру

                    foreach (var gt in queue)
                    {
                        Node nod = gt.Item2;
                        int dy = (h - layersH[nod.layer]) / 2;

                        nod.y += dy;
                        for (int ii = 0; ii < nod.input.Length; ii++)
                            nod.input[ii].Item2 += dy;
                        for (int ii = 0; ii < nod.output.Length; ii++)
                            nod.output[ii].Item2 += dy;
                    }
                    foreach (var ed in edges)
                    {
                        int dy = (h - layersH[ed.layer]) / 2;
                        int ldy = layer == 0 ? 0 : (h - layersH[ed.layer - 1]) / 2;

                        ed.y1 += ldy;
                        ed.y2 += dy;
                    }

                    CustomImage img = new CustomImage(w, h);

                    //отрисовываем ноды и линии
                    foreach (var en in queue)
                    {
                        Node node = en.Item2;
                        string caption = GtypeToSrt(node.g.type);

                        img.DrawRectangle(node.x, node.y, node.w, node.h, Color.FromArgb(255, 0, 0, 0));

                        //сокращения названий гейтов
                        img.DrawCenteredText(caption, node.x, node.y, node.w, node.h, Color.FromArgb(255, 0, 0, 0), 16);
                    }
                    foreach (var edge in edges)
                    {
                        img.DrawLine(edge.x1, edge.y1, edge.x2, edge.y2, Color.FromArgb(255, 0, 0, 0));
                    }

                    return img.Pixels;
                }

                private static string IntToBase36(int value)
                {
                    if (value < 0)
                        throw new ArgumentException("Negative values are not supported.");

                    const string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                    string result = "";
                    while (value > 0)
                    {
                        int remainder = value % 36;
                        result = chars[remainder] + result;
                        value /= 36;
                    }

                    return result == "" ? "0" : result;
                }

                public static byte[] Dna(Bot e, out int w, out int h)
                {
                    Gtype[][] dna = SplitByElement(e.DNA, Gtype.start).ToArray();
                    int genw = dna.Length * 100 / dna.Length;
                    w = genw * dna.Length;
                    h = 50;
                    CustomImage img = new CustomImage(w, h);

                    for (int i = 0; i < dna.Length; i++)
                    {
                        int seed = dna[i].Length == 0 ? 0 : 1;
                        foreach (Gtype gate in dna[i])
                        {
                            int multiplier = (int)gate + 1;
                            seed = (int)((long)seed * multiplier % int.MaxValue);
                        }
                        GenToColor(seed, out byte r, out byte g, out byte b);
                        int x = i * genw + 1;
                        img.DrawRectangle(x, 0, genw - 2, h, Color.FromArgb(255, r, g, b), true);
                        img.DrawRectangle(x + (h / 4), h / 4, genw - (h / 4 * 2) - 2, h - (h / 4 * 2), Color.FromArgb(255, 255, 255, 255), true);
                        img.DrawCenteredText(IntToBase36(Math.Abs(seed)), x, 0, genw - 2, h, Color.FromArgb(255, 0, 0, 0));
                    }

                    return img.Pixels;
                }

                public static byte[] Map(ref int w, ref int h, int vizMode, bool oxygen)
                {
                    w = Main.width;
                    h = Main.height;

                    CustomImage img = new CustomImage(w, h);

                    img.ClearImage(Color.FromArgb(255, 0, 0, 0));

                    foreach (Bot bot in Main.queue)
                    {
                        byte a, r, g, b;

                        switch (vizMode)
                        {
                            case 0:
                                a = 255;
                                GenToColor(bot.gen, out r, out g, out b);
                                break;
                            case 1:
                                ColorFromGradient(bot.nrj, 0, 1000, 1, out a, out r, out g, out b);
                                break;
                            case 2:
                                ColorFromGradient(bot.predation, 0, 1, 2, out a, out r, out g, out b);
                                break;
                            case 3:
                                ColorFromGradient(Main.step - bot.btime, 0, 10000, 0, out a, out r, out g, out b);
                                break;
                            case 4:
                                a = 255;
                                GenToColor(bot.fgen, out r, out g, out b);
                                break;
                            default:
                                a = r = g = b = 0;
                                break;
                        }

                        img.DrawPoint(bot.x, bot.y, Color.FromArgb(a, r, g, b));
                    }

                    foreach (Food f in Main.fmap)
                    {
                        if (f == null)
                            continue;

                        img.DrawPoint(f.x, f.y, Color.FromArgb(255, 50, 50, 50));
                    }

                    if (oxygen)
                        for (int x = 0; x < Main.width; x++)
                        {
                            for (int y = 0; y < Main.height; y++)
                            {
                                ColorFromGradient(Main.oxymap[x, y] / (Main.oxymap[x, y] + Main.crbmap[x, y]), 0, 1, 3, out byte a, out byte r, out byte g, out byte b);
                                img.DrawPoint(x, y, Color.FromArgb(a, r, g, b));
                            }
                        }

                    return img.Pixels;
                }

            }

            [Serializable]
            public class Gate
            {
                public Link[] input;
                public Link[] output;
                public Gtype type;
                public Gate(Gtype type)
                {
                    this.type = type;
                    switch (type)
                    {
                        case Gtype.and:
                        case Gtype.or:
                        case Gtype.xor:
                        case Gtype.mod:
                        case Gtype.mul:
                        case Gtype.grate:
                        case Gtype.less:
                        case Gtype.equal:
                        case Gtype.div:
                        case Gtype.sub:
                        case Gtype.add:
                            input = new Link[2];
                            output = new Link[1];
                            break;

                        case Gtype.memory:
                        case Gtype.not:
                            input = new Link[1];
                            output = new Link[1];
                            break;

                        case Gtype.mut:
                        case Gtype.fgen:
                        case Gtype.gen:
                        case Gtype.btime:
                        case Gtype.c0:
                        case Gtype.c1:
                        case Gtype.c2:
                        case Gtype.c5:
                        case Gtype.c11:
                        case Gtype.posy:
                        case Gtype.posx:
                        case Gtype.nrj:
                        case Gtype.food:
                        case Gtype.rbot:
                        case Gtype.bot:
                        case Gtype.time:
                        case Gtype.rand:
                        case Gtype.listnr:
                            input = new Link[0];
                            output = new Link[1];
                            break;

                        case Gtype.trnsmt:
                        case Gtype.recomb:
                            input = new Link[2];
                            output = new Link[0];
                            break;
                        case Gtype.suicide:
                        case Gtype.photosyntes:
                        case Gtype.atack:
                        case Gtype.rep:
                        case Gtype.sex:
                        case Gtype.Rrot:
                        case Gtype.Lrot:
                        case Gtype.walk:
                        case Gtype.wait:
                            input = new Link[1];
                            output = new Link[0];
                            break;

                        case Gtype.dup2:
                            input = new Link[1];
                            output = new Link[2];
                            break;
                        case Gtype.dup3:
                            input = new Link[1];
                            output = new Link[3];
                            break;
                    }
                }

                public double[] Compute()
                {
                    switch (type)
                    {
                        case Gtype.add:
                            output[0].f = input[0].f + input[1].f;
                            break;
                        case Gtype.sub:
                            output[0].f = input[0].f - input[1].f;
                            break;
                        case Gtype.mul:
                            output[0].f = input[0].f * input[1].f;
                            break;
                        case Gtype.div:
                            output[0].f = input[0].f / input[1].f;
                            break;
                        case Gtype.equal:
                            output[0].f = input[0].f == input[1].f ? 1 : 0;
                            break;
                        case Gtype.mod:
                            output[0].f = input[0].f % input[1].f;
                            break;
                        case Gtype.grate:
                            output[0].f = input[0].f > input[1].f ? 1 : 0;
                            break;
                        case Gtype.less:
                            output[0].f = input[0].f < input[1].f ? 1 : 0;
                            break;
                        case Gtype.not:
                            output[0].f = 1 - input[0].f;
                            break;
                        case Gtype.memory:
                            output[0].f += input[0].f;
                            break;
                        case Gtype.and:
                            output[0].f = input[0].f > 0.5 && input[1].f > 0.5 ? 1 : 0;
                            break;
                        case Gtype.or:
                            output[0].f = input[0].f > 0.5 || input[1].f > 0.5 ? 1 : 0;
                            break;
                        case Gtype.xor:
                            output[0].f = input[0].f > 0.5 ^ input[1].f > 0.5 ? 1 : 0;
                            break;
                        case Gtype.dup2:
                            output[0].f = input[0].f;
                            output[1].f = input[0].f;
                            break;
                        case Gtype.dup3:
                            output[0].f = input[0].f;
                            output[1].f = input[0].f;
                            output[2].f = input[0].f;
                            break;

                        case Gtype.c0:
                            output[0].f = 0;
                            break;
                        case Gtype.c1:
                            output[0].f = 1;
                            break;
                        case Gtype.c2:
                            output[0].f = 2;
                            break;
                        case Gtype.c5:
                            output[0].f = 5;
                            break;
                        case Gtype.c11:
                            output[0].f = 11;
                            break;

                    }

                    foreach (Link ouputLink in output)
                    {
                        if (Double.IsNaN(ouputLink.f))
                        {
                            ouputLink.f = 0;
                        }
                        else if (Double.IsInfinity(ouputLink.f))
                        {
                            ouputLink.f = ouputLink.f > 0 ? 1 : -1;
                        }
                    }

                    if (input.Length == 0)
                        return null;

                    return Array.ConvertAll(input, x => x.f);
                }
            }

            [Serializable]
            public class Link
            {
                public Gate A;
                public Gate B;
                public double f;

                public Link(Gate A, Gate B)
                {
                    this.A = A;
                    this.B = B;
                }
            }

            public static class Formuls
            {
                private static Dictionary<string, (NCalc.Expression Expression, string[] Parameters)> customFunctions = new Dictionary<string, (NCalc.Expression, string[])>();
                private static Dictionary<string, NCalc.Expression> formuls = new Dictionary<string, NCalc.Expression>();

                public static void Load()
                {
                    customFunctions.Clear();
                    formuls.Clear();
                    string[] comands = File.ReadAllLines("formuls.txt");
                    for (int i = 0; i < comands.Length; i++)
                    {
                        if (comands[i].Length == 0)
                            continue;
                        if (comands[i][0] == '#')
                            continue;

                        string[] cmd = comands[i].Split(new string[] { "; " }, StringSplitOptions.None);

                        switch (cmd[0])
                        {
                            case "func":
                                AddFunction(cmd[1], cmd[2], cmd.Skip(3).ToArray());
                                break;
                            default:
                                formuls.Add(cmd[0], new NCalc.Expression(cmd[1]));
                                break;
                        }
                    }
                }

                private static void AddFunction(string functionName, string formula, params string[] parameterNames)
                {
                    customFunctions[functionName] = (new NCalc.Expression(formula), parameterNames);
                }

                public static double Compute(string formulaName, params (string, double)[] variables)
                {
                    if (!formuls.ContainsKey(formulaName))
                    {
                        MessageBox.Show($"Formula '{formulaName}' not found.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return double.NaN; // Or throw an exception if that's preferred
                    }

                    var expression = formuls[formulaName];

                    foreach (var variable in variables)
                    {
                        expression.Parameters[variable.Item1] = variable.Item2;
                    }

                    EvaluateFunctionHandler handler = null;
                    handler = (name, args) =>
                    {
                        if (customFunctions.ContainsKey(name))
                        {
                            var (functionExpression, parameterNames) = customFunctions[name];

                            foreach (var variable in variables)
                            {
                                functionExpression.Parameters[variable.Item1] = variable.Item2;
                            }

                            for (int i = 0; i < parameterNames.Length; i++)
                            {
                                functionExpression.Parameters[parameterNames[i]] = args.Parameters[i].Evaluate();
                            }

                            try
                            {
                                args.Result = functionExpression.Evaluate();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Error evaluating function '{name}': {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                throw;  // Re-throw to handle it further up the stack
                            }
                        }
                    };

                    expression.EvaluateFunction += handler;

                    try
                    {
                        var result = Convert.ToDouble(expression.Evaluate());
                        expression.EvaluateFunction -= handler;
                        return result;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error evaluating formula '{formulaName}': {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return double.NaN; // Or throw an exception if that's preferred

                    }
                }
            }

            public static class Serialization
            {

                public static void Save(string filename)
                {
                    using (var stream = File.Open(filename, FileMode.Create))
                    {
                        var formatter = new BinaryFormatter();

                        formatter.Serialize(stream, Main);
                    }
                }

                public static void Load(string filename)
                {
                    using (var stream = File.Open(filename, FileMode.Open))
                    {
                        var formatter = new BinaryFormatter();
                        Main = (main)formatter.Deserialize(stream);
                    }
                }
            }

            [Serializable]
            public class main
            {
                public int width = 350, height = 200;
                public Bot[,] cmap;
                public Food[,] fmap;
                public double[,] oxymap, crbmap;

                public List<Bot> queue = new List<Bot>();
                public List<Bot> bqueue = new List<Bot>();

                [NonSerialized]
                public Random rnd = new Random();
                public int step;
                public Gtype[] exp = { Gtype.wait, Gtype.photosyntes, Gtype.rep, Gtype.sex, Gtype.Rrot, Gtype.Lrot, Gtype.walk, Gtype.atack, Gtype.suicide, Gtype.recomb, Gtype.trnsmt };

                [OnDeserialized]
                private void OnDeserialized(StreamingContext context)
                {
                    rnd = new Random();
                }

                private double[,] bgasmap;

                private void Distribute(ref double[,] gasmap)
                {
                    Array.Clear(bgasmap, 0, bgasmap.Length);

                    int w = width;
                    int h = height;

                    for (int x = 0; x < w; x++)
                    {
                        int tx_m = x == 0 ? w - 1 : x - 1;
                        int tx_p = x == w - 1 ? 0 : x + 1;

                        for (int y = 0; y < h; y++)
                        {
                            double gas = gasmap[x, y];

                            if (gas == 0) continue;

                            int ty_m = y == 0 ? h - 1 : y - 1;
                            int ty_p = y == h - 1 ? 0 : y + 1;

                            bool centerFree = cmap[x, y] == null && fmap[x, y] == null;

                            if (centerFree)
                            {
                                double delta = gas / 9.0;

                                bgasmap[tx_m, ty_m] += delta; bgasmap[x, ty_m] += delta; bgasmap[tx_p, ty_m] += delta;
                                bgasmap[tx_m, y] += delta; bgasmap[x, y] += delta; bgasmap[tx_p, y] += delta;
                                bgasmap[tx_m, ty_p] += delta; bgasmap[x, ty_p] += delta; bgasmap[tx_p, ty_p] += delta;
                            }
                            else
                            {
                                int div = 1;

                                bool c_mm = cmap[tx_m, ty_m] == null && fmap[tx_m, ty_m] == null; if (c_mm) div++;
                                bool c_0m = cmap[x, ty_m] == null && fmap[x, ty_m] == null; if (c_0m) div++;
                                bool c_pm = cmap[tx_p, ty_m] == null && fmap[tx_p, ty_m] == null; if (c_pm) div++;

                                bool c_m0 = cmap[tx_m, y] == null && fmap[tx_m, y] == null; if (c_m0) div++;
                                bool c_p0 = cmap[tx_p, y] == null && fmap[tx_p, y] == null; if (c_p0) div++;

                                bool c_mp = cmap[tx_m, ty_p] == null && fmap[tx_m, ty_p] == null; if (c_mp) div++;
                                bool c_0p = cmap[x, ty_p] == null && fmap[x, ty_p] == null; if (c_0p) div++;
                                bool c_pp = cmap[tx_p, ty_p] == null && fmap[tx_p, ty_p] == null; if (c_pp) div++;

                                double delta = gas / div;

                                bgasmap[x, y] += delta;
                                if (c_mm) bgasmap[tx_m, ty_m] += delta;
                                if (c_0m) bgasmap[x, ty_m] += delta;
                                if (c_pm) bgasmap[tx_p, ty_m] += delta;
                                if (c_m0) bgasmap[tx_m, y] += delta;
                                if (c_p0) bgasmap[tx_p, y] += delta;
                                if (c_mp) bgasmap[tx_m, ty_p] += delta;
                                if (c_0p) bgasmap[x, ty_p] += delta;
                                if (c_pp) bgasmap[tx_p, ty_p] += delta;
                            }
                        }
                    }

                    (gasmap, bgasmap) = (bgasmap, gasmap);
                }

                public void RandomFill()
                {
                    step = 0;
                    cmap = new Bot[width, height];
                    fmap = new Food[width, height];
                    oxymap = new double[width, height];
                    crbmap = new double[width, height];
                    bgasmap = new double[width, height];

                    queue.Clear();

                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            if (rnd.Next(0, 100) < 50)
                            {
                                cmap[x, y] = new Bot(x, y, 10);
                                queue.Add(cmap[x, y]);
                            }
                            else if (rnd.Next(0, 100) < 10)
                                fmap[x, y] = new Food(x, y, 10);
                        }
                    }

                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            oxymap[x, y] = rnd.Next(10, 50);
                            crbmap[x, y] = rnd.Next(50, 200);
                        }
                    }
                }

                public void Tick()
                {
                    foreach (Bot bot in queue)
                    {
                        bot.Init();
                    }
                    (queue, bqueue) = (bqueue, queue);
                    bqueue.Clear();

                    Distribute(ref crbmap);
                    Distribute(ref oxymap);

                    step++;
                }

            }

            [Serializable]
            public class Food
            {
                public int x, y;
                public double nrj;
                public Food(int x, int y, double nrj)
                {
                    this.x = x;
                    this.y = y;
                    this.nrj = nrj;
                }
            }

            [Serializable]
            public class Bot
            {
                public Bot(int x, int y, double nrj)
                {
                    this.x = x;
                    this.y = y;
                    this.nrj = nrj;
                    mut = 0;
                    DNA = new Gtype[Main.rnd.Next(10, 200)];
                    gen = fgen = Main.rnd.Next(int.MinValue, int.MaxValue);
                    btime = Main.step;
                    rot = Main.rnd.Next(8);

                    switch (rot)
                    {
                        case 0:
                            dx = 1;
                            dy = 0;
                            break;
                        case 1:
                            dx = 1;
                            dy = 1;
                            break;
                        case 2:
                            dx = 0;
                            dy = 1;
                            break;
                        case 3:
                            dx = -1;
                            dy = 1;
                            break;
                        case 4:
                            dx = -1;
                            dy = 0;
                            break;
                        case 5:
                            dx = -1;
                            dy = -1;
                            break;
                        case 6:
                            dx = 0;
                            dy = -1;
                            break;
                        case 7:
                            dx = 1;
                            dy = -1;
                            break;
                    } //rotating

                    var nykls = Enum.GetValues(typeof(Gtype));
                    for (int i = 0; i < DNA.Length; i++)
                    {
                        DNA[i] = (Gtype)nykls.GetValue(Main.rnd.Next(1, nykls.Length));
                    }
                    FDNA = DNA;
                    Translation();
                }

                public Bot(int x, int y, double nrj, int gen, Gtype[] DNA)
                {
                    this.x = x;
                    this.y = y;
                    this.nrj = nrj;
                    this.gen = fgen = gen;
                    mut = 0;
                    btime = Main.step;
                    this.DNA = FDNA = DNA;

                    Translation();
                }

                private void Mutation(Gtype[] fDNA)
                {
                    bool SorE;
                    if (Main.rnd.Next(0, 100) < 6)
                    {
                        SorE = Main.rnd.Next(0, 100) < 50;
                        if (Main.rnd.Next(0, 100) < 50)
                        {
                            DNA = new Gtype[fDNA.Length + 1];
                            if (SorE)
                                Array.Copy(fDNA, 0, DNA, 1, fDNA.Length);
                            else
                                Array.Copy(fDNA, DNA, fDNA.Length);
                        }
                        else
                        {
                            DNA = new Gtype[fDNA.Length - 1];
                            if (SorE)
                                Array.Copy(fDNA, 1, DNA, 0, DNA.Length);
                            else
                                Array.Copy(fDNA, DNA, DNA.Length);
                            mut++;
                        }
                    } //изменение длины днк
                    else
                    {
                        DNA = new Gtype[fDNA.Length];
                        Array.Copy(fDNA, DNA, fDNA.Length);
                    }

                    var nykls = Enum.GetValues(typeof(Gtype));
                    for (int i = 0; i < DNA.Length; i++)
                    {
                        if (Main.rnd.Next(0, 100) < 3 || DNA[i] == 0)
                        {
                            DNA[i] = (Gtype)nykls.GetValue(Main.rnd.Next(1, nykls.Length));
                            mut++;
                        }
                    } //мутации
                } //копирует днк к текущему боту со случайными изменениями

                private Gtype[] Crossingover(Gtype[] dna1, Gtype[] dna2)
                {
                    Gtype[][] Sdna1 = SplitByElement(dna1, Gtype.start).ToArray();
                    Gtype[][] Sdna2 = SplitByElement(dna2, Gtype.start).ToArray();
                    Gtype[][] dna = new Gtype[Math.Max(Sdna1.Length, Sdna2.Length)][];

                    for (int i = 0; i < dna.Length; i++)
                    {
                        if (i >= Sdna1.Length || (i < Sdna2.Length && Main.rnd.Next(100) < 5))
                        {
                            dna[i] = Sdna2[i];
                            if (i < Sdna1.Length)
                            {
                                for (int j = 0; j < Math.Min(Sdna1[i].Length, Sdna2[i].Length); j++)
                                {
                                    if (i >= Sdna1.Length)
                                        mut++;
                                    else
                                    {
                                        if (Sdna1[i][j] != Sdna2[i][j])
                                            mut++;
                                    }
                                }
                                mut += Math.Abs(Sdna1.Length - Sdna2.Length);
                            }
                            else
                                mut += Sdna2[i].Length;
                        }
                        else
                            dna[i] = Sdna1[i];
                    }

                    return CombineWithDelimiter(dna, Gtype.start);
                }

                public Bot(int x, int y, double nrj, Bot f)
                {
                    this.x = x;
                    this.y = y;
                    this.nrj = nrj;
                    FDNA = f.FDNA;
                    mut = f.mut;
                    btime = Main.step;
                    dx = f.dx;
                    dy = f.dy;
                    rot = f.rot;
                    fgen = f.fgen;
                    predation = f.predation;

                    if (Main.rnd.Next(100) < 5)
                    {
                        Mutation(f.DNA);
                        Translation();
                    }
                    else
                    {
                        DNA = f.DNA;
                        gates = f.gates;
                        queue = f.queue;
                    }

                    if (mut > 2)
                    {
                        gen = Main.rnd.Next(int.MinValue, int.MaxValue);
                        mut = 0;
                    } //критичное колличество мутаций
                    else
                        gen = f.gen;
                }

                public Bot(int x, int y, double nrj, Bot p1, Bot p2)
                {
                    //условно p1 - отец - инициатор, p2 - мать - создаёт бота
                    this.x = x;
                    this.y = y;
                    this.nrj = nrj;
                    FDNA = p1.FDNA;
                    mut = p1.mut + p2.mut;
                    btime = Main.step;
                    dx = p2.dx;
                    dy = p2.dy;
                    rot = p2.rot;
                    fgen = p1.fgen;
                    predation = p1.predation + p2.predation;

                    Mutation(Crossingover(p1.DNA, p2.DNA));

                    if (mut / DNA.Length * 100 >= 10)
                    {
                        gen = Main.rnd.Next(int.MinValue, int.MaxValue);
                        mut = 0;
                    } //критичное колличество мутаций
                    else
                        gen = p1.gen;

                    Translation();
                }

                public int x, y; //сколько пропускать нуклеотидов (для визуализации)
                private int dx = 1;
                private int dy = 1; //направление поворота

                double recSignal = -1;
                public int mut { get; set; } //сколько мутаций было
                public int rot { get; set; } //поворот
                public int btime;
                public int gen, fgen; //ген и ген отца
                public double nrj;
                public double predation { get; private set; } = 0.5F;
                public List<Gate> gates;
                private List<Gate> queue;

                public Gtype[] DNA, FDNA;
                private static readonly HashSet<Gtype> coddons = new HashSet<Gtype> { Gtype.start, Gtype.input, Gtype.output, Gtype.stop, Gtype.skip, Gtype.undo, Gtype.empty };
                // Проверка уровня кислорода
                bool IsOxygenLevelValid(double delta)
                {
                    double oxFact = Main.oxymap[x, y] / (Main.oxymap[x, y] + Main.crbmap[x, y]) + delta;

                    double currentOxygen = Main.oxymap[x, y];
                    double currentCarbonDioxide = Main.crbmap[x, y];

                    double newOxygen = currentOxygen + delta;
                    double newCarbonDioxide = currentCarbonDioxide - delta;

                    double newTotal = newOxygen + newCarbonDioxide;
                    double targetOxygen = oxFact * newTotal;

                    return targetOxygen > 0 && newTotal - targetOxygen > 0 && oxFact > 0;
                }
                bool IsOxygenLevelValid(double delta, int x, int y)
                {
                    double oxFact = Main.oxymap[x, y] / (Main.oxymap[x, y] + Main.crbmap[x, y]) + delta;

                    double currentOxygen = Main.oxymap[x, y];
                    double currentCarbonDioxide = Main.crbmap[x, y];

                    double newOxygen = currentOxygen + delta;
                    double newCarbonDioxide = currentCarbonDioxide - delta;

                    double newTotal = newOxygen + newCarbonDioxide;
                    double targetOxygen = oxFact * newTotal;

                    return targetOxygen > 0 && newTotal - targetOxygen > 0 && oxFact > 0;
                }

                // Это временное решение
                void UpdateOxygen(double delta)
                {
                    double oxFact = Main.oxymap[x, y] / (Main.oxymap[x, y] + Main.crbmap[x, y]) + delta;

                    double currentOxygen = Main.oxymap[x, y];
                    double currentCarbonDioxide = Main.crbmap[x, y];

                    double newOxygen = currentOxygen + delta;
                    double newCarbonDioxide = currentCarbonDioxide - delta;

                    double newTotal = newOxygen + newCarbonDioxide;
                    double targetOxygen = oxFact * newTotal;

                    Main.oxymap[x, y] = targetOxygen;
                    Main.crbmap[x, y] = newTotal - targetOxygen;
                }
                void UpdateOxygen(double delta, int x, int y)
                {
                    double oxFact = Main.oxymap[x, y] / (Main.oxymap[x, y] + Main.crbmap[x, y]) + delta;

                    double currentOxygen = Main.oxymap[x, y];
                    double currentCarbonDioxide = Main.crbmap[x, y];

                    double newOxygen = currentOxygen + delta;
                    double newCarbonDioxide = currentCarbonDioxide - delta;

                    double newTotal = newOxygen + newCarbonDioxide;
                    double targetOxygen = oxFact * newTotal;

                    Main.oxymap[x, y] = targetOxygen;
                    Main.crbmap[x, y] = newTotal - targetOxygen;
                }

                public void Translation()
                {
                    gates = new List<Gate>();
                    queue = new List<Gate>();

                    List<Link> ins = new List<Link>();
                    List<Link> outs = new List<Link>();
                    Gtype wt = Gtype.start;
                    int adr = -1; //адресc подключения

                    for (int i = 0; i < DNA.Length; i++)
                    {
                        if (coddons.Contains(DNA[i]))
                        {
                            switch (DNA[i])
                            {
                                case Gtype.stop:
                                    ins.Clear();
                                    outs.Clear();
                                    adr = -1;
                                    break;
                                case Gtype.skip:
                                    adr--;
                                    break;
                                case Gtype.undo:
                                    adr++;
                                    break;
                                default:
                                    wt = DNA[i];
                                    break;
                            }
                            continue;
                        } //специальные кодоны

                        Gate gate = new Gate(DNA[i]);

                        void VFill()
                        {
                            for (int j = 0; j < gate.input.Length; j++)
                            {
                                if (gate.input[j] != null)
                                    continue;
                                gate.input[j] = new Link(null, gate);
                                ins.Add(gate.input[j]);
                            }
                            for (int j = 0; j < gate.output.Length; j++)
                            {
                                if (gate.output[j] != null)
                                    continue;
                                gate.output[j] = new Link(gate, null);
                                outs.Add(gate.output[j]);
                            }
                        }
                        Link link;

                        switch (wt)
                        {
                            case Gtype.input: //подключение ко входу
                                if (gate.output.Length == 0 || ins.Count == 0)
                                    continue;
                                int fiadr = adr - ins.Count * (int)Math.Floor(adr / (decimal)ins.Count);

                                link = ins[fiadr];
                                link.A = gate;
                                gate.output[0] = link;

                                ins.Remove(link);
                                break;
                            case Gtype.output: //подключение к выходу
                                if (gate.input.Length == 0 || outs.Count == 0)
                                    continue;
                                int foadr = adr - outs.Count * (int)Math.Floor(adr / (decimal)outs.Count);

                                link = outs[foadr];
                                link.B = gate;
                                gate.input[0] = link;

                                outs.Remove(link);
                                break;
                        }

                        VFill(); //заполняем порты
                        gates.Add(gate);
                    }

                    //создаём запечённую очередь
                    foreach (Gate gate in gates)
                    {
                        if (Main.exp.Contains(gate.type))
                            queue.Add(gate);
                    } //определяем точки выхода (действия)
                    for (int i = 0; i < queue.Count; i++)
                    {
                        var gate = queue[i];
                        foreach (var link in gate.input)
                            if (!queue.Contains(link.A) && link.A != null)
                                queue.Add(link.A);
                    } //продолжаем очередь
                } //создание мозга

                private Gtype Think(out double[] signals)
                {
                    for (int i = queue.Count - 1; i >= 0; i--) //сворачиваем очередь
                    {
                        var gate = queue[i];
                        signals = gate.Compute();
                        if (signals != null)
                            if (signals[0] > 0 && Main.exp.Contains(gate.type)) //выполнение действия
                                return gate.type;
                    }

                    signals = null;
                    return Gtype.wait;
                } //вызывает гейты в нужном порядке

                private readonly (string Name, double Value)[] param = new (string, double)[]
{
    ("energy",    0),
    ("predation", 0),
    ("mutation",  0),
    ("time",      0),
    ("dnal",      0),
    ("width",     0),
    ("x",         0),
    ("height",    0),
    ("y",         0),
    ("random",    0),
    ("oxygen",    0),
    ("energy2",    0),
    ("dnal2",    0),
    ("stealedEn",    0),
    ("fenergy",    0),
    ("genL",    0),
};

                public void Init()
                {
                    //стандартные переменные для формул
                    param[0].Value = nrj;
                    param[1].Value = predation;
                    param[2].Value = mut;
                    param[3].Value = Main.step;
                    param[4].Value = DNA.Length;
                    param[5].Value = Main.width;
                    param[6].Value = x;
                    param[7].Value = Main.height;
                    param[8].Value = y;
                    param[9].Value = Main.rnd.Next(0, 1000);
                    param[10].Value = Main.oxymap[x, y] / (Main.oxymap[x, y] + Main.crbmap[x, y]);

                    int tx = (x + dx + Main.width) % Main.width;
                    int ty = (y + dy + Main.height) % Main.height;

                    nrj += Formuls.Compute("pasEn", param.ToArray());
                    double pasOx = Formuls.Compute("pasOx", param.ToArray());

                    if (nrj <= 0 || !IsOxygenLevelValid(pasOx))
                    {
                        Main.cmap[x, y] = null;
                        Main.fmap[x, y] = new Food(x, y, Formuls.Compute("deadEn", param.ToArray()));
                        double deadOx = Formuls.Compute("deadOx", param.ToArray());
                        if (IsOxygenLevelValid(deadOx))
                        {
                            UpdateOxygen(deadOx);
                        }
                        return;
                    } //смерть
                    else
                        UpdateOxygen(pasOx);

                    foreach (var gate in gates)
                    {
                        switch (gate.type)
                        {
                            case Gtype.mut:
                                gate.output[0].f = mut;
                                break;
                            case Gtype.fgen:
                                gate.output[0].f = fgen;
                                break;
                            case Gtype.gen:
                                gate.output[0].f = gen;
                                break;
                            case Gtype.time:
                                gate.output[0].f = Main.step;
                                break;
                            case Gtype.rand:
                                gate.output[0].f = Main.rnd.Next(0, 10);
                                break;
                            case Gtype.bot:
                                gate.output[0].f = Main.cmap[tx, ty] != null ? 1 : 0;
                                break;
                            case Gtype.rbot:
                                bool rb = false;
                                if (Main.cmap[tx, ty] != null)
                                    rb = gen == Main.cmap[tx, ty].gen;
                                gate.output[0].f = rb ? 1 : 0;
                                break;
                            case Gtype.food:
                                gate.output[0].f = Main.fmap[tx, ty] == null ? 0 : 1;
                                break;
                            case Gtype.nrj:
                                gate.output[0].f = nrj;
                                break;
                            case Gtype.posx:
                                gate.output[0].f = x;
                                break;
                            case Gtype.posy:
                                gate.output[0].f = y;
                                break;
                            case Gtype.btime:
                                gate.output[0].f = btime;
                                break;
                            case Gtype.listnr:
                                gate.output[0].f = recSignal;
                                break;
                        }
                    } //обновление сенсоров


                    switch (Think(out double[] signals))
                    {
                        case Gtype.photosyntes: // фотосинтез
                            double photoOx = Formuls.Compute("photoOx", param.ToArray());
                            if (IsOxygenLevelValid(photoOx))
                            {
                                nrj += Formuls.Compute("photoEn", param.ToArray());
                                UpdateOxygen(photoOx);
                                predation += 0.01F;
                            }
                            break;

                        case Gtype.rep: // размножение
                            if (Main.cmap[tx, ty] == null && Main.fmap[tx, ty] == null)
                            {
                                double dupOx = Formuls.Compute("dupOx", param.ToArray());
                                if (IsOxygenLevelValid(dupOx))
                                {
                                    Main.cmap[tx, ty] = new Bot(tx, ty, Formuls.Compute("childEn", param.ToArray()), this);
                                    Main.bqueue.Add(Main.cmap[tx, ty]);
                                    nrj += Formuls.Compute("dupEn", param.ToArray());
                                    UpdateOxygen(dupOx);
                                }
                            }
                            break;

                        case Gtype.sex: // половое размножение
                            if (Main.cmap[tx, ty] != null) // есть ли второй родитель
                            {
                                Bot p2 = Main.cmap[tx, ty];
                                int tx2 = (p2.x + p2.dx + Main.width) % Main.width;
                                int ty2 = (p2.y + p2.dy + Main.height) % Main.height;

                                if (Main.cmap[tx2, ty2] == null && Main.fmap[tx2, ty2] == null) // пусто ли перед вторым родителем
                                {
                                    double sexP1Ox = Formuls.Compute("sexP1Ox", param.ToArray());
                                    double sexP2Ox = Formuls.Compute("sexP2Ox", param.ToArray());
                                    if (IsOxygenLevelValid(sexP1Ox) && IsOxygenLevelValid(sexP2Ox, p2.x, p2.y))
                                    {
                                        Main.cmap[tx2, ty2] = new Bot(tx2, ty2, Formuls.Compute("deadEn", param.ToArray()), this, p2);
                                        Main.bqueue.Add(Main.cmap[tx2, ty2]);

                                        param[11].Value = p2.nrj; //energy2
                                        param[12].Value = p2.DNA.Length; // dnal2

                                        nrj += Formuls.Compute("sexP1En", param.ToArray());
                                        p2.nrj += Formuls.Compute("sexP2En", param.ToArray());
                                        UpdateOxygen(sexP1Ox);
                                        UpdateOxygen(sexP2Ox, p2.x, p2.y);
                                    }
                                }
                            }
                            break;

                        case Gtype.Rrot: // поворот 1
                            double rot1Ox = Formuls.Compute("rot1Ox", param.ToArray());
                            if (IsOxygenLevelValid(rot1Ox))
                            {
                                rot = (rot + 1) % 8;
                                nrj += Formuls.Compute("rot1En", param.ToArray());
                                UpdateOxygen(rot1Ox);
                            }
                            break;

                        case Gtype.Lrot: // поворот 2
                            double rot2Ox = Formuls.Compute("rot2Ox", param.ToArray());
                            if (IsOxygenLevelValid(rot2Ox))
                            {
                                rot = (rot + 7) % 8;
                                nrj += Formuls.Compute("rot2En", param.ToArray());
                                UpdateOxygen(rot2Ox);
                            }
                            break;

                        case Gtype.walk: // ходьба
                            if (Main.cmap[tx, ty] == null && Main.fmap[tx, ty] == null)
                            {
                                double walkOx = Formuls.Compute("walkOx", param.ToArray());
                                if (IsOxygenLevelValid(walkOx))
                                {
                                    Main.cmap[tx, ty] = Main.cmap[x, y];
                                    Main.cmap[x, y] = null;
                                    x = tx;
                                    y = ty;
                                    nrj += Formuls.Compute("walkEn", param.ToArray());
                                    UpdateOxygen(walkOx);
                                }
                            }
                            break;

                        case Gtype.atack: // атака
                            if (Main.cmap[tx, ty] != null)
                            {
                                param[11].Value = Main.cmap[tx, ty].nrj; //energy2
                                double deadOx = Formuls.Compute("deadOx", param.ToArray());
                                if (IsOxygenLevelValid(deadOx))
                                {
                                    double dnrj = Math.Min(Formuls.Compute("deadEn", param.ToArray()), Main.cmap[tx, ty].nrj);
                                    Main.cmap[tx, ty].nrj -= Formuls.Compute("deadEn", param.ToArray());

                                    param[13].Value = dnrj; //stealedEn
                                    nrj += Formuls.Compute("deadEn", param.ToArray());
                                    UpdateOxygen(deadOx);
                                    predation -= 0.01F;
                                }
                            }
                            if (Main.fmap[tx, ty] != null)
                            {
                                param[14].Value = Main.fmap[tx, ty].nrj; //fenergy
                                double fEatOx = Formuls.Compute("fEatOx", param.ToArray());
                                if (IsOxygenLevelValid(fEatOx))
                                {
                                    nrj += Formuls.Compute("fEatEn", param.ToArray());
                                    UpdateOxygen(fEatOx);
                                    Main.fmap[tx, ty] = null;
                                    predation += 0.001F;
                                }
                            }
                            break;

                        case Gtype.suicide: // суицид
                            double sdeadOx = Formuls.Compute("sdeadOx", param.ToArray());
                            if (IsOxygenLevelValid(sdeadOx))
                            {
                                Main.fmap[x, y] = new Food(x, y, Formuls.Compute("sdeadEn", param.ToArray()));
                                UpdateOxygen(sdeadOx);
                                Main.cmap[x, y] = null;
                            }
                            return;

                        case Gtype.recomb: // рекомбинация
                            if (Main.cmap[tx, ty] != null)
                            {
                                double recombOx = Formuls.Compute("recombOx", param.ToArray());
                                if (IsOxygenLevelValid(recombOx))
                                {
                                    Gtype[][] dna1 = SplitByElement(DNA, Gtype.start).ToArray();
                                    List<Gtype[]> dna2 = SplitByElement(Main.cmap[tx, ty].DNA, Gtype.start);
                                    int maxL = Math.Max(dna1.Length, dna2.Count);
                                    int adr = Math.Abs((int)Math.Round(signals[1]) + maxL);

                                    Gtype[] gen = dna1[dna1.Length % dna1.Length];
                                    dna2.Insert(adr % dna2.Count, gen);
                                    Main.cmap[tx, ty].DNA = CombineWithDelimiter(dna2.ToArray(), Gtype.start);

                                    param[15].Value = gen.Length; //genL
                                    nrj += Formuls.Compute("recombEn", param.ToArray());
                                    UpdateOxygen(recombOx);
                                }
                            }
                            break;

                        case Gtype.trnsmt: // передача сигнала
                            if (Main.cmap[tx, ty] != null)
                            {
                                double trnsmtOx = Formuls.Compute("trnsmtOx", param.ToArray());
                                if (IsOxygenLevelValid(trnsmtOx))
                                {
                                    Main.cmap[tx, ty].recSignal = signals[1];
                                    nrj += Formuls.Compute("trnsmtEn", param.ToArray());
                                    UpdateOxygen(trnsmtOx);
                                }
                            }
                            break;
                    }


                    switch (rot)
                    {
                        case 0:
                            dx = 1;
                            dy = 0;
                            break;
                        case 1:
                            dx = 1;
                            dy = 1;
                            break;
                        case 2:
                            dx = 0;
                            dy = 1;
                            break;
                        case 3:
                            dx = -1;
                            dy = 1;
                            break;
                        case 4:
                            dx = -1;
                            dy = 0;
                            break;
                        case 5:
                            dx = -1;
                            dy = -1;
                            break;
                        case 6:
                            dx = 0;
                            dy = -1;
                            break;
                        case 7:
                            dx = 1;
                            dy = -1;
                            break;
                    } //rotating

                    Main.bqueue.Add(this);
                }

            }
        }
    }

}
