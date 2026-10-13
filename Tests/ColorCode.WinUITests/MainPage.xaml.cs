using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using ColorCode.Common;
using ColorCode.Styling;
using ColorCode.WinUI.Common;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;

namespace ColorCode.WinUITests
{
    public sealed partial class MainPage : Page
    {
        /// <summary>
        /// When set, the app renders every sample in both themes, writes a screenshot and an inline-tree dump
        /// per combination into this folder, then exits.
        /// </summary>
        private const string SnapshotDirectoryVariable = "COLORCODE_SNAPSHOT_DIR";

        public MainPage()
        {
            InitializeComponent();

            LanguagePicker.ItemsSource = Samples.All;
            LanguagePicker.SelectedIndex = 0;

            Loaded += OnLoaded;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            var snapshotDirectory = Environment.GetEnvironmentVariable(SnapshotDirectoryVariable);
            if (!string.IsNullOrEmpty(snapshotDirectory))
            {
                await WriteSnapshots(snapshotDirectory);
                Application.Current.Exit();
            }
        }

        private void OnSelectionChanged(object sender, object e) => Render();

        private void Render()
        {
            if (LanguagePicker?.SelectedItem is not Samples.Sample sample || DarkToggle is null)
            {
                return;
            }

            var theme = DarkToggle.IsOn ? ElementTheme.Dark : ElementTheme.Light;
            MainGrid.RequestedTheme = theme;

            var formatter = new RichTextBlockFormatter(theme);
            var plainText = formatter.Styles[ScopeName.PlainText];
            MainGrid.Background = (plainText?.Background ?? StyleDictionary.White).GetSolidColorBrush();

            PresentationBlock.Blocks.Clear();
            formatter.FormatRichTextBlock(sample.Code, sample.Language, PresentationBlock);

            InlineTree.Text = InlineTreeDump.Dump(PresentationBlock);
        }

        private async Task WriteSnapshots(string directory)
        {
            Directory.CreateDirectory(directory);
#if HAS_UNO
            const string platform = "skia";
#else
            const string platform = "winappsdk";
#endif

            foreach (var sample in Samples.All)
            {
                foreach (var dark in new[] { false, true })
                {
                    LanguagePicker.SelectedItem = sample;
                    DarkToggle.IsOn = dark;
                    Render();

                    // Let layout and a render pass complete before capturing.
                    await Task.Delay(300);

                    var name = $"{Sanitize(sample.Name)}-{(dark ? "dark" : "light")}";
                    // Not InlineTree.Text: TextBox normalizes the line breaks.
                    File.WriteAllText(Path.Join(directory, $"{name}.{platform}.txt"), InlineTreeDump.Dump(PresentationBlock));
                    await CapturePng(PresentationBlock, Path.Join(directory, $"{name}.{platform}.png"));
                }
            }
        }

        private async Task CapturePng(UIElement element, string path)
        {
            // Capture the grid region behind the block so the theme background is part of the image.
            var bitmap = new RenderTargetBitmap();
            // Uno's RenderTargetBitmap holds a native buffer and is IDisposable; the WinAppSDK one is not (null here).
            using var bitmapDisposable = (object)bitmap as IDisposable;

            await bitmap.RenderAsync(MainGrid);
            var pixels = (await bitmap.GetPixelsAsync()).ToArray();

            var bounds = element.TransformToVisual(MainGrid).TransformBounds(
                new Windows.Foundation.Rect(0, 0, element.ActualSize.X, element.ActualSize.Y));
            var scale = bitmap.PixelWidth / MainGrid.ActualWidth;

            var x = (int)(bounds.X * scale);
            var y = (int)(bounds.Y * scale);
            var width = Math.Min((int)Math.Ceiling(bounds.Width * scale), bitmap.PixelWidth - x);
            var height = Math.Min((int)Math.Ceiling(bounds.Height * scale), bitmap.PixelHeight - y);

            PngWriter.WriteBgra(path, pixels, bitmap.PixelWidth, x, y, width, height);
        }

        private static string Sanitize(string name) =>
            new string(name.Select(c => char.IsLetterOrDigit(c) ? char.ToLowerInvariant(c) : c == '#' ? 's' : c == '+' ? 'p' : '-').ToArray());
    }
}
