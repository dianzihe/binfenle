using UnityEngine;
using System.Collections;
using System.IO;


public enum SerializerResult {
	Success = 0,
	ErrorInvalidFileType,
	ErrorNewerFileVersionNotSupported,
	UnknownIOError,
}

public interface ICustomSerializable {
	void WriteToStream(BinaryWriter writeStream);

	void ReadFromStream(int fileVersion, BinaryReader readStream);
}
	
/// <summary>
/// AbstractBoardSerializer
/// 
/// Serializes/Deserializes multiple <see cref="ICustomSerializable"/> objects to a version tagged binary file.
/// </summary>
public abstract class AbstractSerializer {
	protected string fileHeaderTag = "AbstractSerializer";
	protected int serializerVersion = 1;
	public string fileName;
	
	
	public AbstractSerializer(string _fileName) {
		fileName = _fileName;
	}
	
	public int SerializerVersion {
		get {
			return serializerVersion;
		}
	}

	public virtual SerializerResult WriteToFile() {
		FileStream fileStream = File.Create(fileName);
		BinaryWriter writeStream = new BinaryWriter(fileStream);
		
		// Write the file header tag to later identify the file type correctly.
		writeStream.Write(fileHeaderTag);
		// Write the file serialization version
		writeStream.Write(SerializerVersion);
		
		// Start writing the board data.
		WriteFileData(writeStream);
		
		fileStream.Close();
		
		return SerializerResult.Success;
	}
	
	public virtual SerializerResult ReadFromFile() {
		FileStream fileStream = File.OpenRead(fileName);
		BinaryReader readStream = new BinaryReader(fileStream);
		
		// Read the file header tag first and validate.
		string headerTag = readStream.ReadString();
		if (headerTag.Equals(fileHeaderTag) == false) {
			fileStream.Close();
			return SerializerResult.ErrorInvalidFileType;
		}
		
		// Read the saved file version.
		int fileVersion = readStream.ReadInt32();
		
		// Make sure we're not trying to read from a newer file version than our code currently supports.
		if (fileVersion > SerializerVersion) {
			fileStream.Close();
			return SerializerResult.ErrorNewerFileVersionNotSupported;
		}
		
		ReadFileData(fileVersion, readStream);

		fileStream.Close();
		
		return SerializerResult.Success;
	}
	
	/// <summary>
	/// Writes binary data to a file.
	/// </summary>
	/// <param name='writeStream'>
	/// Write stream.
	/// </param>
	public abstract void WriteFileData(BinaryWriter writeStream);
	
	/// <summary>
	/// Reads binary data from a file.
	/// </summary>
	/// <param name='fileVersion'>
	/// File version.
	/// </param>
	/// <param name='readStream'>
	/// Read stream.
	/// </param>
	public abstract void ReadFileData(int fileVersion, BinaryReader readStream);
	
	
	public static string GetFileHeaderTag(string fileName) {
		FileStream fileStream = File.OpenRead(fileName);
		BinaryReader readStream = new BinaryReader(fileStream);
		
		string headerTag = readStream.ReadString();
		
		fileStream.Close();
		
		return headerTag;
	}
	
	public static int GetFileVersion(string fileName) {
		FileStream fileStream = File.OpenRead(fileName);
		BinaryReader readStream = new BinaryReader(fileStream);
		
		// Read the file header tag.
		readStream.ReadString();
		// Read and save the file version.
		int fileVersion = readStream.ReadInt32();
		
		fileStream.Close();
		
		return fileVersion;
	}	
}
