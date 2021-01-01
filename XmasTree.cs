using System;

var n = Convert.ToInt32(args[0]);
var rnd = new Random();
for (var i = n; i > 1; i--) {
  var t = "".PadLeft(i, ' ');
  for (var j = 1; j < (n - i) * 2; j++)
    t += rnd.Next(0, 10) > 7 ? '@' : '*';
  Console.WriteLine(t);
}
var b = "".PadLeft(n - 2, ' ') + "###";
Console.WriteLine(b);
Console.WriteLine(b);
Console.WriteLine(b);
