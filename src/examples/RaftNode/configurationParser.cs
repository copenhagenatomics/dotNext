using System.Text.Json;

namespace RaftNode
{

public class TestConfiguration
{
    public int LowerElectionTimeout { get; set; }
    public int UpperElectionTimeout { get; set; }
    public int TransmissionBlockSize { get; set; }
    public bool IsLocal { get; set; }
    public int NodeCount  { get; set; }
    public bool sensorData { get; set; }

    public Dictionary<string, Node_t> NodeList { get; set; }
    public class Node_t
    {
        public byte[] ip_byte {get; set; }
        public int[] ip { get; set; }
        public int port { get; set; }
    }
}   

public class ConfigurationParser
{
    public TestConfiguration config;
    public ConfigurationParser(string? path)
    {
        if (path is not null)
        {
            GetFromConfig(path);
        }

    }

    private bool GetFromConfig(string path)
    {

            string fileName = path;
            string jsonString = File.ReadAllText(fileName);
            config = JsonSerializer.Deserialize<TestConfiguration>(jsonString);
            foreach (var node in config.NodeList)
            {
                node.Value.ip_byte = new byte[4];
                for (int i = 0; i<4;i++)
                {
                    node.Value.ip_byte[i] = Convert.ToByte(node.Value.ip[i]);
                }
            }
        return true;
    }

}


}
