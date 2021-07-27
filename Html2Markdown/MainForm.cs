// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

// ReSharper disable CommentTypo
// ReSharper disable InconsistentNaming
// ReSharper disable ClassNeverInstantiated.Global

#region Using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using HtmlAgilityPack;
using HtmlDoc= HtmlAgilityPack.HtmlDocument;

#endregion

#nullable enable

namespace Html2Markdown
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private static string Convert
            (
                string html
            )
        {
            int SafeGet (IList<int> list, int index, int value)
            {
                for (var i = list.Count; i <= index; i++)
                {
                    list.Add (value);
                }

                return list[index];
            }
            
            bool IsTableCell (HtmlNode node)
            {
                var name = node.Name.ToLowerInvariant();
                return name == "th" || name == "td";
            }
            
            var result = new StringBuilder();

            var document = new HtmlDoc();
            document.LoadHtml (html);

            var tables = document.DocumentNode.Descendants ("table");
            var firstTable = true;
            foreach (var table in tables)
            {
                if (!firstTable)
                {
                    // добавляем перевод строки перед второй
                    // и последующими таблицами
                    result.AppendLine();
                }
                
                firstTable = false;
                
                var rows = table.Descendants ("tr");
                var firstRow = true;
                var columnWidth = new List<int>();
                foreach (var row in rows)
                {
                    // мы будем проходиться по этим ячейкам дважды
                    var cells = row.Descendants().Where (IsTableCell)
                        .ToArray();
                    result.Append ('|');
                    var index = 0;
                    foreach (var cell in cells)
                    {
                        var cellText = cell.InnerText;
                        var textLength = SafeGet (columnWidth, index++, cellText.Length + 2);

                        result.Append (' ');
                        result.Append (cellText.PadRight(textLength - 2));
                        result.Append (' ');
                        result.Append ('|');
                    }
                    
                    result.AppendLine();

                    index = 0;
                    if (firstRow)
                    {
                        result.Append ('|');
                        foreach (var _ in cells)
                        {
                            var filler = new string ('-', columnWidth[index++]) + "|";
                            result.Append (filler);
                        }

                        result.AppendLine();
                        firstRow = false;
                    }
                }
            }

            return result.ToString();
            
        } // method Convert

        private void _convertButton_Click
            (
                object sender, 
                EventArgs e
            )
        {
            var html = _htmlBox.Text.Trim();
            var markdown = Convert (html);
            _markdownBox.Text = markdown;
        }

        private void _pasteButton_Click
            (
                object sender, 
                EventArgs e
            )
        {
            var html = Clipboard.ContainsText (TextDataFormat.Html) 
                ? Clipboard.GetText (TextDataFormat.Html) 
                : Clipboard.GetText();
            
            _htmlBox.Text = html;
        }

        private void _copyButton_Click
            (
                object sender, 
                EventArgs e
            )
        {
            var markdown = _markdownBox.Text.Trim();
            Clipboard.SetText (markdown);
        }
    }
}
