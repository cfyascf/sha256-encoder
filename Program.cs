using System.Text;

const string input = "batata";
Console.WriteLine($"{input} : INPUT");

// ..convert to binary
var binaryInput = Encoding.UTF8.GetBytes(input);

// ..turns binary into string so it's easier to deal with
var stringfiedBinary = ConvertToBinaryString(binaryInput);
Console.WriteLine($"{stringfiedBinary} : INPUT TO BINARY");

// ..adds 1 bit to the end
stringfiedBinary = AddBits(stringfiedBinary, 1, true, true);
Console.WriteLine($"{stringfiedBinary} : 1 BIT ADDED TO THE END");

// ..the final word must have 448 bits,
// so we found the bits missing and fill with 0..
var size = stringfiedBinary.Length;
var totalMissingBits = 448 - size;
Console.WriteLine($"{totalMissingBits} : TOTAL MISSING BITS");

// ..turn the size of the word into binary..
var binarySize = Convert.ToString(size, 2);
var sizeBinarySize = binarySize.Length;

// ..the last 64 bits must contain the size
// of the original binary word in binary as well;
var finalMissingBits = totalMissingBits - sizeBinarySize;
var fullBits = AddBits(binarySize, finalMissingBits, false, false);

var word = stringfiedBinary + fullBits;
Console.WriteLine($"{word} : WORD");

List<string> words32bits = new();
var startIndex = 0;
var len = 32;

try 
{
    while(startIndex <= word.Length)
    {
        var sub = word.Substring(startIndex, len);
        words32bits.Add(sub);
        var test = sub.Length;

        startIndex += 32;
    }
}
catch(Exception err)
{
    Console.WriteLine($"Error when breaking up word: {err.Message}");
} 

Console.WriteLine(words32bits.ToString());

// var word32bits = "";
// var aux = 0;
// for(int i = 0; i < 32; i++)
// {
//     word.Substring(0, 31);
// }

// ..direction true to add to right, false to left
string AddBits(string input, int qty, bool bitValue, bool direction)
{
    for(int i = 0; i < qty; i++)
    {
        if(direction) input += bitValue ? "1" : "0";

        else input = bitValue ? "1" : "0" + input;
    }

    return input;
}

string ConvertToBinaryString(byte[] bytes)
{
    string result = "";
    foreach (byte b in bytes)
    {
        result += Convert.ToString(b, 2).PadLeft(8, '0'); 
    }
    return result;
}