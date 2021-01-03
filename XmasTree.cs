using System;
using System.Collections.Generic;
using System.Linq;
using Crayon;
using static System.Console;

{
  Console.Clear();

  var n = int.TryParse(args.FirstOrDefault() ?? "15", out var nx) ? nx : 15;
  var xmasTree = BuildXmasTree(n);
  foreach(var pixel in xmasTree)
  {
    pixel.Update();
  }

  SetCursorPosition(0, n + 4);
}

IReadOnlyList<Pixel> BuildXmasTree(int size)
{
  var pixels = new List<Pixel>();
  var rnd = new Random();
  for (var i = 1; i <= size; i++)
  {
    for (var j = 1; j < (size - i) * 2; j++)
    {
      var isLight = rnd.Next(0, 10) > 7;
      pixels.Add(new Pixel(
        i + j,
        size - i,
        isLight ? '@' : '*',
        isLight ? (0xFF, 0xD7, 0x00) : (0x42, 0x69, 0x2F)));
    }
  }

  for (var y = size; y < size + 3; y++)
  {
    for (var x = size - 1; x <= size + 1; x++)
    {
      pixels.Add(new Pixel(x, y, '#', (0x65, 0x43, 0x21)));
    }
  }

  return pixels;
}

class Pixel
{
  public int X { get; init; }
  public int Y { get; init; }
  public string C { get; init; }

  public (byte r, byte g, byte b) Color { get; init; }

  public Pixel(int x, int y, char c, (byte, byte, byte) color)
  {
    X = x;
    Y = y;
    C = c.ToString();
    Color = color;
  }

  public void Update()
  {
    SetCursorPosition(X, Y);
    Write(Output.FromRgb(Color.r, Color.g, Color.b).Text(C));
  }
}