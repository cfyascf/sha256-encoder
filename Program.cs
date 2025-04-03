using System.Security.Cryptography.X509Certificates;
using System.Text;

const string input = "batata";
Console.WriteLine($"{input} : INPUT");

// ..convert to binary
var binaryInput = Encoding.UTF8.GetBytes(input);

// ..turns binary into string so it's easier to deal with
var sBinaryInput = ConvertToBinaryString(binaryInput);
Console.WriteLine($"{sBinaryInput} : INPUT TO BINARY");

// ..adds 1 bit to the end
var sBinaryInput1 = AddBits(sBinaryInput, 1, true, true);
Console.WriteLine($"{sBinaryInput1} : 1 BIT ADDED TO THE END");

// ..the final word must have 448 bits,
// so we found the bits missing and fill with 0..
var wordSize = sBinaryInput.Length;
Console.WriteLine($"{wordSize} : WORD SIZE");
var totalMissingBits = 448 - wordSize;
Console.WriteLine($"{totalMissingBits} : TOTAL MISSING BITS");

var word448bits = AddBits(sBinaryInput1, totalMissingBits - 1, false, true);
Console.WriteLine($"{word448bits.Length} : INPUT 448 BITS");

// ..turn the size of the word into binary..
var binarySize = Convert.ToString(wordSize, 2);
var sizeOfBinarySize = binarySize.Length;

// // ..the last 64 bits must contain the size
// // of the original binary word in binary as well;
var missingBitsFromSize = 64 - sizeOfBinarySize;
var size64bits = AddBits(binarySize, missingBitsFromSize, false, false);
Console.WriteLine($"{size64bits} : INPUT SIZE 64 BITS");

var word = word448bits + size64bits;
Console.WriteLine($"{word} : WORD");

List<string> words32bits = new();
var startIndex = 0;
var len = 32;

var test = 0;
try 
{
    while(startIndex != word.Length)
    {
        var sub = word.Substring(startIndex, len);
        words32bits.Add(sub);
        test = sub.Length;

        startIndex += 32;

        if(startIndex == word.Length)
        {
            Console.WriteLine($"SUB : {sub} | TEST : {test} | START INDEX: {startIndex}");
        }
    }
}
catch(Exception err)
{
    Console.WriteLine($"Error when breaking up word: {err} | {test}");
} 

var wordsLeft = 64 - words32bits.Count;
for(var i = 0; i < wordsLeft; i++)
{
    var word32bits = "";
    word32bits = AddBits(word32bits, 32, false, true);
    words32bits.Add(word32bits); 
}

Console.WriteLine($"{words32bits.Count} : WORDS ARRAY SIZE");

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