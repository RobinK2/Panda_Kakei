using System.IO;
using System.Threading.Tasks;

public interface ISave
{
    //Method to save document as a file and view the saved document
    void SaveAndView(string filename, string contentType, MemoryStream stream);

    Task CopyFilePickSrc(string srcFilename, string dstFilename);
    Task<string> CopyFilePickDest(string srcFilename, string dstFilename);
}

