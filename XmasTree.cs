// Inspired by @LuigiArpino's Tweet, 25 Dec 2020, to @shanselman: https://twitter.com/luigiarpino/status/1342522409400860683
using System;
using System.Linq;
using static System.Console;

var n = int.TryParse(args.FirstOrDefault() ?? "15", out var nx) ? nx : 15;
var rnd = new Random();
for (var i = n; i > 1; i--) {
  var t = "".PadLeft(i, ' ');
  for (var j = 1; j < (n - i) * 2; j++)
    t += rnd.Next(0, 10) > 7 ? '@' : '*';
  WriteLine(t);
}
var b = "".PadLeft(n - 2, ' ') + "###";
WriteLine(b);
WriteLine(b);
WriteLine(b);
