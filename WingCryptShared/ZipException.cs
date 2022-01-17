namespace WingCryptShared;
using System;

public class ZipException : Exception
{
	public Exception TrueException { get; private set; }

	public ZipException(Ionic.Zip.ZipException ex) => TrueException = ex;
}
