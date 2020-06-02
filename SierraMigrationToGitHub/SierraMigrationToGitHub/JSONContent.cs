using System.Net.Http;
using System.Text;

namespace SierraMigrationToGitHub
{
    class JSONContent : StringContent
    {
        public JSONContent(string json) : base(json, Encoding.UTF8,
                                    "application/json")
        { }
    }
}
