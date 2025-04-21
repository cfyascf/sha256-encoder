using System.ComponentModel;
using System.Text;

uint a = 0x6a09e667, b = 0xbb67ae85, c = 0x3c6ef372, d = 0xa54ff53a;
uint e = 0x510e527f, f = 0x9b05688c, g = 0x1f83d9ab, h = 0x5be0cd19;

uint[] K =
[
    0x428a2f98, 0x71374491, 0xb5c0fbcf, 0xe9b5dba5, 0x3956c25b,
    0x59f111f1, 0x923f82a4, 0xab1c5ed5, 0xd807aa98, 0x12835b01,
    0x243185be, 0x550c7dc3, 0x72be5d74, 0x80deb1fe, 0x9bdc06a7,
    0xc19bf174, 0xe49b69c1, 0xefbe4786, 0x0fc19dc6, 0x240ca1cc,
    0x2de92c6f, 0x4a7484aa, 0x5cb0a9dc, 0x76f988da, 0x983e5152,
    0xa831c66d, 0xb00327c8, 0xbf597fc7, 0xc6e00bf3, 0xd5a79147,
    0x06ca6351, 0x14292967, 0x27b70a85, 0x2e1b2138, 0x4d2c6dfc,
    0x53380d13, 0x650a7354, 0x766a0abb, 0x81c2c92e, 0x92722c85,
    0xa2bfe8a1, 0xa81a664b, 0xc24b8b70, 0xc76c51a3, 0xd192e819,
    0xd6990624, 0xf40e3585, 0x106aa070, 0x19a4c116, 0x1e376c08,
    0x2748774c, 0x34b0bcb5, 0x391c0cb3, 0x4ed8aa4a, 0x5b9cca4f,
    0x682e6ff3, 0x748f82ee, 0x78a5636f, 0x84c87814, 0x8cc70208,
    0x90befffa, 0xa4506ceb, 0xbef9a3f7, 0xc67178f2
];

Console.WriteLine("SEND A MESSAGE TO BE CONVERTED TO SHA256:\n");
string input = Console.ReadLine();

// Convert to binary
var binaryInput = Encoding.UTF8.GetBytes(input!);

// Turn binary into string so it's easier to deal with
var sBinaryInput = ConvertToBinaryString(binaryInput);
Console.WriteLine($"{sBinaryInput} : INPUT TO BINARY");

var word = GetWord(sBinaryInput);

// Changed to handle multiple blocks
for (int i = 0; i < word.Length; i += 512)
{
    string block = word.Substring(i, Math.Min(512, word.Length - i));
    var words32bits = ExpandTo64Words(block);
    CalculateW4(words32bits);

    var hash = CalculateHash(words32bits);
    Console.WriteLine(hash);
}

string CalculateHash(List<uint> words) // Changed List<long> to List<uint>
{
    // Store initial hash values for this block
    uint aa = a, bb = b, cc = c, dd = d, ee = e, ff = f, gg = g, hh = h;

    for (int i = 0; i < 64; i++)
    {
        uint s0 = GetS0(aa);
        uint s1 = GetS1(ee);
        uint choice = GetChoice(ee, ff, gg);
        uint majority = GetMajority(aa, bb, cc);

        uint temp1 = hh + s1 + choice + K[i] + words[i];
        uint temp2 = s0 + majority;

        hh = gg;
        gg = ff;
        ff = ee;
        ee = dd + temp1;
        dd = cc;
        cc = bb;
        bb = aa;
        aa = temp1 + temp2;
    }

    // Update global hash values after processing block
    a += aa;
    b += bb;
    c += cc;
    d += dd;
    e += ee;
    f += ff;
    g += gg;
    h += hh;

    uint[] H = { a, b, c, d, e, f, g, h }; // Use updated values
    return GetFinalHash(H);
}

string GetFinalHash(uint[] H)
{
    var builder = new StringBuilder();
    foreach (var value in H)
    {
        builder.Append(value.ToString("x8"));
    }

    return builder.ToString();
}

uint GetS1(uint value)
{
    return (uint)(RotateRight(value, 6) ^ RotateRight(value, 11) ^ RotateRight(value, 25));
}

uint GetS0(uint value)
{
    return (uint)(RotateRight(value, 2) ^ RotateRight(value, 13) ^ RotateRight(value, 22));
}

uint GetChoice(uint e, uint f, uint g)
{
    return (e & f) ^ ((~e) & g);
}

uint GetMajority(uint a, uint b, uint c)
{
    return (a & b) ^ (a & c) ^ (b & c);
}

void CalculateW4(List<uint> words) // Changed List<long> to List<uint>
{
    // Fixed indices to match SHA-256 word expansion
    for (int i = 16; i < 64; i++)
    {
        var w0 = words[i - 16];
        var w1 = words[i - 15];
        var w2 = words[i - 7];
        var w3 = words[i - 2];

        var teta0 = (uint)CalculateTeta0(w1); // Cast to uint
        var teta1 = (uint)CalculateTeta1(w3); // Cast to uint
        words[i] = (uint)CalculateWord(w0, teta0, w2, teta1); // Cast to uint
    }
}

List<uint> ExpandTo64Words(string word) // Changed List<long> to List<uint>
{
    // Breaking into words of 32 bits
    List<uint> words32bits = new();
    FillExistingWords(words32bits, word);
    FillWordsLeft(words32bits);

    return words32bits;
}

void FillExistingWords(List<uint> words, string word) // Changed List<long> to List<uint>
{
    var startIndex = 0;
    var len = 32;
    try
    {
        while (startIndex < word.Length)
        {
            var sub = word.Substring(startIndex, Math.Min(len, word.Length - startIndex));
            words.Add(Convert.ToUInt32(sub.PadRight(32, '0'), 2)); // Changed to UInt32
            startIndex += 32;
        }
    }
    catch (Exception err)
    {
        Console.WriteLine($"Error when breaking up word: {err}");
    }
}

void FillWordsLeft(List<uint> words) // Changed List<long> to List<uint>
{
    // Adding the words left
    var wordsLeft = 64 - words.Count;
    for (int i = 0; i < wordsLeft; i++)
    {
        words.Add(0); // Simplified, no need for binary string
    }
}

string GetWord(string sBinaryInput)
{
    var word448bits = ExpandWord(sBinaryInput);
    var size64bits = GetSize64Bits(sBinaryInput.Length); // Fixed to use original length

    return word448bits + size64bits;
}

string ExpandWord(string word)
{
    // Adds 1 bit to the end
    var sBinaryInput1 = AddBits(word, 1, true, true);
    Console.WriteLine($"{sBinaryInput1} : 1 BIT ADDED TO THE END");

    // Fixed padding to ensure total length is multiple of 512
    var wordSize = word.Length;
    Console.WriteLine($"{wordSize} : WORD SIZE");
    var k = (448 - (wordSize + 1) % 512) % 512; // Number of zeros
    Console.WriteLine($"{k} : TOTAL MISSING BITS");

    return AddBits(sBinaryInput1, k, false, true);
}

string GetSize64Bits(int size)
{
    var binarySize = Convert.ToString(size, 2);
    var sizeOfBinarySize = binarySize.Length;

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

string AddBits(string input, int qty, bool bitValue, bool direction)
{
    for (int i = 0; i < qty; i++)
    {
        if (direction) input += bitValue ? "1" : "0";
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