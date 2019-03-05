using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KamooniHost.IDependencyServices
{
	public interface IQRCodeService {
		Stream GenerateQR(string text, int width = 300, int height = 300);
	}
}
