using System.Text;

const string input = "batata";
Console.WriteLine($"{input} : INPUT");

// ..convert to binary
var binaryInput = Encoding.UTF8.GetBytes(input);

// ..turns binary into string so it's easier to deal with
var sBinaryInput = ConvertToBinaryString(binaryInput);
Console.WriteLine($"{sBinaryInput} : INPUT TO BINARY");

var word = GetWord(sBinaryInput);
var words32bits = ExpandTo64Words(word);
CalculateW4(words32bits);

// void CalculateHash(List<int> roots)
// {
//     var values = new List<int>();
//     for(var i = 0; i < roots.Count; i++)
//     {
//         values[i] = roots[i];
//     }
// }

// void UpdateWorkingVariables(List<int> values)
// {
//     values[]
// }

List<long> GetRoots()
{
    var roots = new List<long>();
    roots.Add(Convert.ToInt64("01101010000010011110011001100111", 2));
    roots.Add(Convert.ToInt64("10111011011001111010111010000101", 2));
    roots.Add(Convert.ToInt64("00111100011011101111001101110010", 2));
    roots.Add(Convert.ToInt64("10100101010011111111010100111010", 2));
    roots.Add(Convert.ToInt64("01010001000011100101001001111111", 2));
    roots.Add(Convert.ToInt64("10011011000001010110100010001100", 2));
    roots.Add(Convert.ToInt64("00011111100000111101100110101011", 2));
    roots.Add(Convert.ToInt64("01011011111000001100110100011001", 2));

    return roots;
}

void CalculateW4(List<long> words)
{
    int i = 0;
    var j = i + 1;
    var k = j + 8;
    var l = k + 5;
    var m = l + 2;

    for(i = 0; m < 64; i++)
    {
        var w0 = words32bits[i];
        var w1 = words32bits[j];
        var w2 = words32bits[k];
        var w3 = words32bits[l];

        var teta0 = CalculateTeta0(w1);
        var teta1 = CalculateTeta1(w3);
        words32bits[m] = CalculateWord(w0, teta0, w2, teta1);

        j = i + 1;
        k = j + 8;
        l = k + 5;
        m = l + 2;
    }
}

List<long> ExpandTo64Words(string word)
{
    // ..beaking into words of 32 bits..
    List<long> words32bits = new();
    FillExistingWords(words32bits, word);
    FillWordsLeft(words32bits);

    return words32bits;
}

void FillExistingWords(List<long> words, string word)
{
    var startIndex = 0;
    var len = 32;
    var test = 0;
    try 
    {
        while(startIndex != word.Length)
        {
            var sub = word.Substring(startIndex, len);
            words.Add(Convert.ToInt64(sub, 2));
            test = sub.Length;

            startIndex += 32;
        }
    }
    catch(Exception err)
    {
        Console.WriteLine($"Error when breaking up word: {err} | {test}");
    } 
}

void FillWordsLeft(List<long> words)
{
    // ..adding the words left..
    var i = 0;
    var wordsLeft = 64 - words.Count;
    for(i = 0; i < wordsLeft; i++)
    {
        var word32bits = "";
        word32bits = AddBits(word32bits, 32, false, true);
        words.Add(Convert.ToInt64(word32bits, 2)); 
    }
}

string GetWord(string sBinaryInput)
{
    var word448bits = ExpandWord(sBinaryInput);
    var size64bits = GetSize64Bits(word448bits.Length);

    return word448bits + size64bits;
}

string ExpandWord(string word)
{
    // ..adds 1 bit to the end
    var sBinaryInput1 = AddBits(word, 1, true, true);
    Console.WriteLine($"{sBinaryInput1} : 1 BIT ADDED TO THE END");

    // ..the final word must have 448 bits,
    // so we found the bits missing and fill with 0..
    var wordSize = sBinaryInput.Length;
    Console.WriteLine($"{wordSize} : WORD SIZE");
    var totalMissingBits = 448 - wordSize;
    Console.WriteLine($"{totalMissingBits} : TOTAL MISSING BITS");

    return AddBits(sBinaryInput1, totalMissingBits - 1, false, true);
}

string GetSize64Bits(int size)
{
    // ..turn the size of the word into binary..
    var binarySize = Convert.ToString(size, 2);
    var sizeOfBinarySize = binarySize.Length;

    // // ..the last 64 bits must contain the size
    // // of the original binary word in binary as well;
    var missingBitsFromSize = 64 - sizeOfBinarySize;
    return AddBits(binarySize, missingBitsFromSize, false, false);
}

long CalculateTeta0(long w)
{
    return RotateRight(w, 7) ^ RotateRight(w, 18) ^ ShiftRight(w, 3);
}

long CalculateTeta1(long w)
{
    return RotateRight(w, 17) ^ RotateRight(w, 19) ^ ShiftRight(w, 10);
}

long CalculateWord(long w0, long teta0, long w2, long teta1)
{
    return w0 + teta0 + w2 + teta1;
}

long RotateRight(long word, int times)
{
    return ((uint)word >> times) | ((uint)word << (32 - times));
}

long ShiftRight(long word, int times)
{
    return (uint)word >> times;
}

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