using System.Text.Json;

namespace Frontend.Utils
{
    public class JsonAdapter
    {
        public string Serializable { get; set; }

        public JsonAdapter(Object _object)
        {
            Serializable = JsonSerializer.Serialize(_object);
        }
    }
}