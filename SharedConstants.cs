namespace WingCrypt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal static class SharedConstants
{
	internal const string WORKING_NAME = "_wingcrypttemp.zip";
	internal const string DEFAULT_NAME = "wingcrypt.wnc";

	internal static byte[] IV => new byte[] { 69, 42, 39, 0, 3, 13, 87, 27, 121, 62, 1, 111, 245, 73, 199, 154 };
	// initialization vector bytes courtesy of Kraber Queen
}
