using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Blacklist
{
    public class BlackList
    {
        public static readonly HashSet<string> Blacklist = new HashSet<string>(
            new string[]
            {
                // Demo
                "notepad.exe",

                // Adware
                "BonziBuddy.exe",
                "Gator.exe",
                "Spyware Doctor.exe",

                // Botnet
                "Botnets.exe",
                "Zeus.exe",
                "Zbot.exe",
                "Mirai.exe",

                // Ransomware
                "WannaCry.exe",
                "Petya.exe",
                "Ryuk.exe",
                "Locky.exe",
                "CryptoLocker.exe",

                // Rootkits
                "SubSeven.exe",
                "Fu.exe",
                "Hacker Defender.exe",

                // Spyware
                "FinFisher.exe",
                "FinSpy.exe",
                "CoolWebSearch.exe",
                "Zango.exe",

                // Trojans
                "BackOrifice.exe",
                "NetBus.exe",
                "Emotet.exe",
                "TrickBot.exe",
                "NanoCore.exe",

                // Viruses
                "ILoveYou.exe",
                "LoveBug.exe",
                "Melissa.exe",
                "Mydoom.exe",
                "Code Red.exe",
                "Stuxnet.exe",
                "Conficker.exe",

                // Worms
                "Blaster.exe",
                "Lovsan.exe",
                "SQL Slammerexe",
                "Mydoom.exe"
            },

            StringComparer.OrdinalIgnoreCase // Case-insensitive matching
        );

    }
}
