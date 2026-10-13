using System.Text;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media;
using Windows.UI.Text;

namespace ColorCode.WinUITests
{
    /// <summary>
    /// Renders the inline structure the formatter produced as text, so the WinAppSDK and Uno outputs can be diffed.
    /// </summary>
    public static class InlineTreeDump
    {
        public static string Dump(RichTextBlock block)
        {
            var builder = new StringBuilder();
            foreach (var item in block.Blocks)
            {
                if (item is Paragraph paragraph)
                {
                    builder.AppendLine("Paragraph");
                    DumpInlines(builder, paragraph.Inlines, 1);
                }
                else
                {
                    builder.AppendLine(item.GetType().Name);
                }
            }

            return builder.ToString();
        }

        private static void DumpInlines(StringBuilder builder, InlineCollection inlines, int depth)
        {
            foreach (var inline in inlines)
            {
                builder.Append(' ', depth * 2);
                switch (inline)
                {
                    case Run run:
                        builder.Append("Run \"").Append(Escape(run.Text)).Append('"');
                        AppendStyle(builder, run);
                        builder.AppendLine();
                        break;

                    case Span span:
                        builder.Append(span.GetType().Name);
                        AppendStyle(builder, span);
                        builder.AppendLine();
                        DumpInlines(builder, span.Inlines, depth + 1);
                        break;

                    default:
                        builder.AppendLine(inline.GetType().Name);
                        break;
                }
            }
        }

        private static void AppendStyle(StringBuilder builder, TextElement element)
        {
            if (element.ReadLocalValue(TextElement.ForegroundProperty) is SolidColorBrush brush)
            {
                var c = brush.Color;
                builder.Append($" fg=#{c.A:X2}{c.R:X2}{c.G:X2}{c.B:X2}");
            }

            if (element.FontWeight.Weight != FontWeights.Normal.Weight)
            {
                builder.Append(" weight=").Append(element.FontWeight.Weight);
            }

            if (element.FontStyle != FontStyle.Normal)
            {
                builder.Append(" style=").Append(element.FontStyle);
            }
        }

        private static string Escape(string text) => text
            .Replace("\\", "\\\\")
            .Replace("\r", "\\r")
            .Replace("\n", "\\n")
            .Replace("\t", "\\t")
            .Replace("\"", "\\\"");
    }
}
