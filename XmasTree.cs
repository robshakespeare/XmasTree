using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Crayon;
using static System.Console;

{
  var n = int.TryParse(args.FirstOrDefault() ?? "20", out var nx) ? nx : 20;
  var xmasTree = BuildXmasTree(n);

  var cts = new CancellationTokenSource();
  CancelKeyPress += (sender, args) =>
  {
    args.Cancel = true;
    cts.Cancel();
  };
  Console.Clear();
  Console.CursorVisible = false;

  while (!cts.Token.IsCancellationRequested)
  {
    foreach (var pixel in xmasTree)
    {
      pixel.Update();
    }
    await Task.Delay(33, cts.Token).ContinueWith(_ => {});
  }  

  SetCursorPosition(0, Math.Min(n + 4, BufferHeight - 1));
  Console.CursorVisible = true;
}

IReadOnlyList<Pixel> BuildXmasTree(int size)
{
  const int messageLine = 5;
  const string message = "MERRY CHRISTMAS\n&\nHAPPY NEW YEAR!";
  var messageLines = message.Split('\n').Reverse().ToArray();
  var messageWidth = messageLines.Max(line => line.Length);

  var pixels = new List<Pixel>();
  var rnd = new Random();
  for (var i = 1; i <= size; i++)
  {
    for (var j = 1; j < (size - i) * 2; j++)
    {
      var x = i + j;
      var isWithinMessageRegion =
        i >= messageLine - 1 && i <= messageLine + messageLines.Length &&
        x >= (size - 1 - messageWidth / 2) && x <= (size + 1 + messageWidth / 2);
      var isLight = !isWithinMessageRegion && rnd.Next(0, 10) > 7;
      pixels.Add(new Pixel(x, size - i, isLight ? '@' : '*', isLight ? null : (0x42, 0x69, 0x2F)));
    }
  }

  for (var y = size; y < size + 3; y++)
  {
    for (var x = size - 1; x <= size + 1; x++)
    {
      pixels.Add(new Pixel(x, y, '#', (0x65, 0x43, 0x21)));
    }
  }

  foreach (var m in messageLines
    .SelectMany((line, y) => line
      .Select((c, i) => (c, y, i, mid: line.Length / 2))
      .Where(m => m.c != ' ')))
  {
    pixels.Add(new Pixel((size - m.mid) + m.i, size - messageLine - m.y, m.c, null));
  }

  return pixels;
}

class Pixel
{
  private static Random Rnd = new Random();

  private Rainbow _rainbow = new Rainbow(Rnd.Next(10, 30) / 100d);
  private bool hasRendered = false;

  public int X { get; init; }
  public int Y { get; init; }
  public string C { get; init; }
  public (byte r, byte g, byte b)? Color { get; init; }

  public Pixel(int x, int y, char c, (byte, byte, byte)? color)
  {
    X = x;
    Y = y;
    C = c.ToString();
    Color = color;

    foreach (var offset in Enumerable.Range(0, Rnd.Next(1, 100)))
    {
      _rainbow.Next();
    }
  }

  public void Update()
  {
    var offScreen = X <= 0 || Y <= 0 || X >= BufferWidth || Y >= BufferHeight;
    if (offScreen)
    {
      return;
    }

    var isFixedColor = Color != null;
    if (hasRendered && isFixedColor)
    {
      return;
    }

    SetCursorPosition(X, Y);
    var output = Color == null
      ? _rainbow.Next()
      : Output.FromRgb(Color.Value.r, Color.Value.g, Color.Value.b);
    Write(output.Text(C));

    hasRendered = true;
  }
}