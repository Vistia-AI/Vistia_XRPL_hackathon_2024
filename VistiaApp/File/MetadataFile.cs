using System;

[Serializable]
public class MetadataFile
{
    public string Author = "Unknown";
    public string Description = "No description available";
    public string Checksum = "";
    public DateTime CreatedAt = DateTime.Now;
    public DateTime UpdatedAt = DateTime.Now;

    public override string ToString()
    {
        return $"Author: {Author}, Description: {Description}, Checksum: {Checksum}, CreatedAt: {CreatedAt}, UpdatedAt: {UpdatedAt}";
    }
}
