using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Crayon;
using static System.Console;

{
  var n = args.Any() && int.TryParse(args.First(), out var a) ? a : 20;
  var xmasTree = BuildXmasTree(n);

  var cts = new CancellationTokenSource();
  CancelKeyPress += (s, evnt) =>
  {
    evnt.Cancel = true;
    cts.Cancel();
  };
  Clear();
  CursorVisible = false;

  while (!cts.Token.IsCancellationRequested)
  {
    foreach (var pixel in xmasTree) pixel.Update();
    await Task.Delay(33, cts.Token).ContinueWith(_ => {});
  }  

  SetCursorPosition(0, Math.Min(n + 4, BufferHeight - 1));
  CursorVisible = true;
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
    for (var j = 1; j < (size - i) * 2; j++)
    {
      var x = i + j;
      var isWithinMessageRegion =
        i >= messageLine - 1 && i <= messageLine + messageLines.Length &&
        x >= (size - 1 - messageWidth / 2) && x <= (size + 1 + messageWidth / 2);
      var isLight = !isWithinMessageRegion && rnd.Next(0, 10) > 7;
      pixels.Add(new Pixel(x, size - i, isLight ? '@' : '*', isLight ? null : (0x42, 0x69, 0x2F)));
    }

  for (var y = size; y < size + 3; y++)
    for (var x = size - 1; x <= size + 1; x++)
      pixels.Add(new Pixel(x, y, '#', (0x65, 0x43, 0x21)));

  foreach (var m in messageLines.SelectMany((line, y) => line
    .Select((c, i) => (c, y, i, mid: line.Length / 2))
    .Where(m => m.c != ' ')))
  {
    pixels.Add(new Pixel((size - m.mid) + m.i, size - messageLine - m.y, m.c, null));
  }

  return pixels;
}

record Pixel(int X, int Y, char C, (byte r, byte g, byte b)? Color)
{
  private static Random Rnd = new Random();
  public string S = C.ToString();
  private Rainbow _rainbow = OffsetRainbow(new Rainbow(Rnd.Next(20, 40) / 100d));
  private bool _hasRendered = false;

  private static Rainbow OffsetRainbow(Rainbow rainbow) =>
    Enumerable.Range(0, Rnd.Next(1, 100)).Aggregate(rainbow, (r, i) => { r.Next(); return r; });

  public void Update()
  {
    var offScreen = X <= 0 || Y <= 0 || X >= BufferWidth || Y >= BufferHeight;
    var isFixedColor = Color != null;
    if (offScreen || (_hasRendered && isFixedColor)) return;

    SetCursorPosition(X, Y);
    var output = Color == null ? _rainbow.Next() : Output.FromRgb(Color.Value.r, Color.Value.g, Color.Value.b);
    Write(output.Text(S));
    _hasRendered = true;
  }
}