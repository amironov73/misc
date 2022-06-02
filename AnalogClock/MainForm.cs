// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming

#region Using directives

using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Configuration;
using Timer = System.Windows.Forms.Timer;

#endregion

namespace AnalogClock;

public sealed class MainForm 
    : Form
{
    #region Construction

    public MainForm()
    {
        SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

        Configure();
        Font = new Font(FontFamily.GenericSansSerif, 16.0f);
        FormBorderStyle = FormBorderStyle.None;
        StartPosition = FormStartPosition.Manual;
        Size = new Size(ClockSize, ClockSize);
        Location = new Point(LocationX, LocationY);

        var timer = new Timer
        {
            Enabled = true,
            Interval = 1000
        };
        timer.Tick += _timer_Tick;


        SetWindowShape();
    }

    #endregion

    #region Private members

    private IConfiguration configuration;

    private int ClockSize = 300;
    private int LocationX = 1000;
    private int LocationY = 100;

    private const int WM_NCLBUTTONDOWN = 0xA1;
    private const int HT_CAPTION = 0x2;

    [DllImport("user32.dll")]
    public static extern int SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

    [DllImport("user32.dll")]
    public static extern bool ReleaseCapture();

    private void Configure()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");

        configuration = builder.Build();
        ClockSize = int.Parse(configuration["size"]);
        LocationX = int.Parse(configuration["location-x"]);
        LocationY = int.Parse(configuration["location-y"]);
    }

    private void SetWindowShape()
    {
        using var graphics = Graphics.FromHwnd(Handle);
        var bounds = ClientRectangle;
        bounds.Width--;
        bounds.Height--;
        var path = new GraphicsPath();
        path.AddEllipse(bounds);
        graphics.FillPath(SystemBrushes.Window, path);
        var region = new Region(path);
        Region = region;
    }

    private void DrawClockHand
        (
            Graphics graphics,
            RectangleF bounds,
            float angle,
            float size,
            float thickness
        )
    {
        var centerX = bounds.Width / 2.0f;
        var centerY = bounds.Height / 2.0f;
        var pointX = centerX + MathF.Sin(angle) * size;
        var pointY = centerY - MathF.Cos(angle) * size;

        using var pen = new Pen(Color.Black, thickness);
        graphics.DrawLine(pen, centerX, centerY, pointX, pointY);
    }

    private void DrawDigit
        (
            Graphics graphics,
            RectangleF bounds,
            Font font,
            Brush brush,
            int digit
        )
    {
        var angle = FractionToAngle(digit / 12.0f);
        var size = bounds.Width / 2.0f * 0.85f;
        var centerX = bounds.Width / 2.0f;
        var centerY = bounds.Height / 2.0f;
        var x = centerX + MathF.Sin(angle) * size;
        var y= centerY - MathF.Cos(angle) * size;

        var text = digit.ToString(CultureInfo.InvariantCulture);
        var format = new StringFormat(StringFormatFlags.NoWrap)
        {
            Alignment = StringAlignment.Center,
            HotkeyPrefix = HotkeyPrefix.None,
            LineAlignment = StringAlignment.Center,
            Trimming = StringTrimming.None
        };
        graphics.DrawString(text, font, brush, new PointF(x, y), format);
    }

    private float FractionToAngle
        (
            float fraction
        )
    {
        return (float) (fraction * Math.PI * 2.0f);
    }

    private void _timer_Tick(object? sender, EventArgs eventArgs)
    {
        Invalidate();
    }

    #endregion

    #region Protected members

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        RectangleF bounds = ClientRectangle;
        bounds.Width--;
        bounds.Height--;
        var graphics = e.Graphics;
        var blackBrush = new SolidBrush(Color.Black);
        var whiteBrush = new SolidBrush(Color.White);
        var blackPen = new Pen(blackBrush, 1.0f);
        var size = bounds.Width / 2.0f;

        var ellipse = bounds;
        ellipse.Inflate(-1, -1);
        graphics.FillEllipse(whiteBrush, bounds);
        graphics.DrawEllipse(blackPen, ellipse);

        var now = DateTime.Now;
        var hour = (float) now.Hour;
        while (hour >= 12.0f)
        {
            hour -= 12.0f;
        }

        DrawClockHand(graphics, bounds, FractionToAngle(hour / 12.0f), 0.5f * size, 10.0f);

        var minute = (float) now.Minute;
        DrawClockHand(graphics, bounds, FractionToAngle(minute / 60.0f), 0.65f * size, 4.0f);

        var second = (float) now.Second;
        DrawClockHand(graphics, bounds, FractionToAngle(second / 60.0f), 0.8f * size, 1.0f);

        for (var digit = 1; digit < 13; digit++)
        {
            DrawDigit(graphics, bounds, Font, blackBrush, digit);
        }
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);

        if (e.Button == MouseButtons.Left)
        {
            ReleaseCapture();
            SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
        }

        //if (e.Button == MouseButtons.Right)
        //{
        //    ReleaseCapture();
        //    Close();
        //}
    }

    #endregion
}
