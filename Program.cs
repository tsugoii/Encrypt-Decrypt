using System.IO;
using System.Text;

int menuSelection = -1;
string directory = "";
string fileName = "";
string absolutePath = "";
string cryptPassword = "";
int[] options = new int[] { 0, 1, 2, 3, 4, 5, 6, 7 };

while (true)
{
    MainMenu();
    if (menuSelection == 0) break;
    if (menuSelection == 1)
    {
        SelectDirectory();
    }
    else if (Directory.Exists(directory))
    {
        if (menuSelection == 2)
        {
            ListDirectoryContentShallow();
        }
        else if (menuSelection == 3)
        {
            ListDirectoryContentDeep();
        }
        else if (menuSelection == 4)
        {
            DeleteFile();
        }
        else if (menuSelection == 5)
        {
            DisplayFile();
        }
        else if (menuSelection == 6)
        {
            EncryptFile();
        }
        else if (menuSelection == 7)
        {
            DecryptFile();
        }
        else
        {
            Console.WriteLine("Please Enter an option 0-7");
        }
    }
    else
    {
        Console.WriteLine("Please select option 1 and input a valid directory to begin\n");
    }
}

Console.WriteLine("Exiting Now...");
Environment.Exit(0);


void MainMenu()
{
    menuSelection = -1;
    Console.WriteLine("Please Select A Menu Option:\n0 - Exit\n1 - Select Directory\n2 - List Directory Content (First Level)\n3 - List Directory Content (All Levels)\n4 - Delete File\n5 - Display File (In Hex)\n6 - Encrypt File (XOR w/ Password)\n7 - Decrypt File (XOR w/ Password)");
    while (!options.Contains(menuSelection))
    {
        try
        {
            menuSelection = Convert.ToInt32(Console.ReadLine());
            if (!options.Contains(menuSelection))
            {
                Console.WriteLine("Please Enter an option 0-7");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Please Enter an option 0-7", e);
        }
    }
}

void SelectDirectory()
{
    Console.WriteLine("The Current Directory you are operating on is: " + directory + "\nPlease enter the directory's absolute name you would like to operate on: ");
    directory = Console.ReadLine() ?? "";
    Console.WriteLine("Your new operating directory is: " + directory + "\n");
}

void ListDirectoryContentShallow()
{
    Console.WriteLine("\nDirectories: ");
    Directory.GetDirectories(directory).ToList().ForEach(Console.WriteLine);
    Console.WriteLine("\nFiles: ");
    Directory.GetFiles(directory).ToList().ForEach(Console.WriteLine);
    Console.WriteLine();
}
void ListDirectoryContentDeep()
{
    List<string> files = new();
    Console.WriteLine("\nDirectories: ");
    Directory.GetDirectories(directory).ToList().ForEach(PrintDirectoryEntries);

    void PrintDirectoryEntries(string dir)
    {
        Console.WriteLine(dir);
        Directory.GetDirectories(dir).ToList().ForEach(PrintDirectoryEntries);

        files.AddRange(Directory.GetFiles(dir));
    }

    Console.WriteLine("\nFiles: ");
    files.ForEach(Console.WriteLine);
    Console.WriteLine();
}

void DeleteFile()
{
    Console.WriteLine("Please enter a relative filename to delete from your selected directory: ");
    // D:\NieR_Tutorials
    fileName = Console.ReadLine() ?? "";
    absolutePath = Path.Combine(directory, fileName);
    if (File.Exists(absolutePath))
    {
        Console.WriteLine("Are you sure you'd like to delete " + absolutePath + " ? (yes/no)\n");
        if (Console.ReadLine()?.ToUpper() == "YES")
        {
            try
            {
                File.Delete(absolutePath);
                Console.WriteLine(fileName + " was deleted\n");
            }
            catch (Exception e)
            {
                Console.WriteLine("You threw an error: " + e);
            }
        }
        else Console.WriteLine(fileName + " was not deleted\n");
    }
    else
    {
        Console.WriteLine(absolutePath + " could not be found");
    }
}

void DisplayFile()
{
    Console.WriteLine("Please enter a relative filename to delete from your selected directory: ");
    fileName = Console.ReadLine() ?? "";
    absolutePath = Path.Combine(directory, fileName);
    if (File.Exists(absolutePath))
    {
        Console.WriteLine(fileName + " in hex: \n");
        var file = File.ReadAllBytes(absolutePath);
        var hex = Convert.ToHexString(file);
        Console.WriteLine(hex + "\n");
        var hexPath = Path.Combine(directory, Path.GetFileNameWithoutExtension(fileName)) + "Hex.txt";
        File.WriteAllText(hexPath, hex);
        Console.WriteLine("Hex dump is at: " + hexPath);
    }
    else
    {
        Console.WriteLine(absolutePath + " could not be found");
    }
}

void EncryptFile()
{
    Console.WriteLine("Please Enter A Password you would like to use for encryption: ");
    cryptPassword = Console.ReadLine() ?? "";
    // Qwertyuiop[123$4$567]
    Console.WriteLine("Please Enter the file you would like to encrypt: ");
    fileName = Console.ReadLine() ?? "";
    absolutePath = Path.Combine(directory, fileName);
    if (File.Exists(absolutePath))
    {
        var file = File.ReadAllBytes(absolutePath);
        List<byte> bytes = new();
        for (int i = 0; i < file.Length; i++)
        {
            var pwChar = (uint)cryptPassword[i % cryptPassword.Length];
            var fileChar = (uint)file[i];

            uint encryptedChar = pwChar ^ fileChar;
            bytes.Add(Convert.ToByte(encryptedChar));
        }

        Console.WriteLine("Where would you like to export this?");
        var encryptPath = Path.Combine(directory, Console.ReadLine() ?? "");
        File.WriteAllBytes(encryptPath, bytes.ToArray());
        Console.WriteLine("Encrypt dump is at: " + encryptPath);
    }
    else
    {
        Console.WriteLine(absolutePath + " could not be found");
    }

}
void DecryptFile()
{
    Console.WriteLine("Please Enter A Password you would like to XOR decrypt your file with: ");
    cryptPassword = Console.ReadLine() ?? "";
    Console.WriteLine("Please Enter the file you would like to decrypt: ");
    fileName = Console.ReadLine() ?? "";
    absolutePath = Path.Combine(directory, fileName);
    if (File.Exists(absolutePath))
    {
        var file = File.ReadAllBytes(absolutePath);
        List<byte> bytes = new();
        for (int i = 0; i < file.Length; i++)
        {
            var pwChar = (uint)cryptPassword[i % cryptPassword.Length];
            var fileChar = (uint)file[i];

            uint decryptedChar = pwChar ^ fileChar;
            bytes.Add(Convert.ToByte(decryptedChar));
        }

        Console.WriteLine("Where would you like to export this?");
        var decryptPath = Path.Combine(directory, Console.ReadLine() ?? "");
        File.WriteAllBytes(decryptPath, bytes.ToArray());
        Console.WriteLine("Decrypt dump is at: " + decryptPath);
    }
    else
    {
        Console.WriteLine(absolutePath + " could not be found");
    }
}